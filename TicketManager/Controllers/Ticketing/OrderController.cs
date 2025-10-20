using Microsoft.AspNetCore.Mvc;
using TicketManager.Common.Enums;
using TicketManager.Common.Interface;
using TicketManager.Common.Models;
using TicketManager.Common.Models.Entities;
using TicketManager.Common.Models.ViewModels;

namespace TicketManager.Controllers.Ticketing
{
	[Route("orders/carts")]
	public class OrderController : Controller
	{
		private IRepository<Cart, Guid> _cartRepository;
		private IRepository<Seat, int> _seatRepository;
		private IRepository<Payment, int> _paymentRepository;

		public OrderController(IRepository<Cart, Guid> cartRepository, IRepository<Seat, int> seatRepository, IRepository<Payment, int> paymentRepository)
		{
			_cartRepository = cartRepository;
			_seatRepository = seatRepository;
			_paymentRepository = paymentRepository;
		}

		[HttpGet]
		[Route("cartId")]
		public async Task<IActionResult> Get(string cartId)
		{
			try
			{
				var cart = await _cartRepository.GetByIdAsync(new Guid(cartId));
				return Ok(cart.Items);
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
				return StatusCode(500);
			}
		}

		[HttpPost]
		[Route("cartId")]
		public async Task<IActionResult> AddToCart(string cartId, [FromBody] CartItemViewModel addedSeat)
		{
			try
			{
				var cart = await _cartRepository.GetByIdAsync(new Guid(cartId));
				cart.Items.Add(new CartItem()
				{
					EventId = addedSeat.EventId,
					PriceId = addedSeat.PriceId,
					SeatId = addedSeat.SeatId
				});
				await _cartRepository.UpdateAsync(cart);

				return Ok(cart);
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
				return StatusCode(500);
			}
		}

		[HttpDelete]
		[Route("{cartId}/events/{eventId}/seats/{seatId}")]
		public async Task<IActionResult> RemoveFromCart(string cartId, int eventId, int seatId)
		{
			try
			{
				var cart = await _cartRepository.GetByIdAsync(new Guid(cartId));
				var seatToRemove = cart.Items.First(x => x.EventId == eventId && x.SeatId == seatId);
				cart.Items.Remove(seatToRemove);
				await _cartRepository.UpdateAsync(cart);

				return Ok();
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
				return StatusCode(500);
			}
		}

		[HttpPut]
		[Route("{cartId}/book")]
		public async Task<IActionResult> BookSeats(string cartId) //to GUID
		{
			try
			{
				var cart = await _cartRepository.GetByIdAsync(new Guid(cartId));
				var seats = cart.Items.Select(x => x.Seat);

				var payment = new Payment()
				{
					CartId = cart.CartId,
					PaymentStatus = PaymentStatus.Pending
				};

				foreach (var seat in seats)
				{
					seat.SeatState = SeatState.Reserved;
					await _seatRepository.UpdateAsync(seat);
				}

				var id = await _paymentRepository.CreateAsync(payment);
				
				return Ok(id);
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
				return StatusCode(500);
			}
		}
	}
}
