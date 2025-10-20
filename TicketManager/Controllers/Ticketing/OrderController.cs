using Microsoft.AspNetCore.Mvc;
using TicketManager.Common.Enums;
using TicketManager.Common.Interface;
using TicketManager.Common.Models;
using TicketManager.Common.Models.Entities;
using TicketManager.Common.Models.ViewModels;
using TicketManager.DAL;

namespace TicketManager.Controllers.Ticketing
{
	[Route("orders/carts")]
	public class OrderController : Controller
	{
		private IRepository<Cart, Guid> _cartRepository;
		private IRepository<Seat, int> _seatRepository;
		private IRepository<Payment, int> _paymentRepository;
		private AppDbContext _dbContext;

		public OrderController(IRepository<Cart, Guid> cartRepository, IRepository<Seat, int> seatRepository, IRepository<Payment, int> paymentRepository, AppDbContext dbContext)
		{
			_cartRepository = cartRepository;
			_seatRepository = seatRepository;
			_paymentRepository = paymentRepository;
			_dbContext = dbContext;
		}

		[HttpGet]
		[Route("cartId")]
		public async Task<IActionResult> Get(Guid cartId)
		{
			try
			{
				var cart = await _cartRepository.GetByIdAsync(cartId);
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
		public async Task<IActionResult> AddToCart(Guid cartId, [FromBody] CartItemViewModel addedSeat)
		{
			try
			{
				var cart = await _cartRepository.GetByIdAsync(cartId);
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
		public async Task<IActionResult> RemoveFromCart(Guid cartId, int eventId, int seatId)
		{
			try
			{
				var cart = await _cartRepository.GetByIdAsync(cartId);
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
		public async Task<IActionResult> BookSeats(Guid cartId)
		{
			var transaction = await _dbContext.Database.BeginTransactionAsync();

			try
			{
				var cart = await _cartRepository.GetByIdAsync(cartId);
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

				await transaction.CommitAsync();
				return Ok(id);
			}
			catch (Exception e)
			{
				await transaction.RollbackAsync();
				Console.WriteLine(e);
				return StatusCode(500);
			}
		}
	}
}
