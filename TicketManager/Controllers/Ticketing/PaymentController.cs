using Microsoft.AspNetCore.Mvc;
using TicketManager.Common.Enums;
using TicketManager.Common.Interface;
using TicketManager.Common.Models;
using TicketManager.Common.Models.Entities;

namespace TicketManager.Controllers.Ticketing
{
	[Route("payments")]
	public class PaymentController : Controller
	{
		private IRepository<Payment, int> _paymentRepository;
		private IRepository<Cart, Guid> _cartRepository;
		private IRepository<Seat, int> _seatRepository;

		public PaymentController(IRepository<Payment, int> paymentRepository, IRepository<Cart, Guid> cartRepository, IRepository<Seat, int> seatRepository)
		{
			_paymentRepository = paymentRepository;
			_cartRepository = cartRepository;
			_seatRepository = seatRepository;
		}

		[HttpGet]
		[Route("{paymentId}")]
		public async Task<IActionResult> GetById(int paymentId)
		{
			try
			{
				var payment = await _paymentRepository.GetByIdAsync(paymentId);
				return Ok(payment.PaymentStatus);
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
				return StatusCode(500);
			}
		}

		[HttpPost]
		[Route("{paymentId}/complete")]
		public async Task<IActionResult> Complete(int paymentId)
		{
			try
			{
				var paymentToUpdate = await _paymentRepository.GetByIdAsync(paymentId);
				paymentToUpdate.PaymentStatus = PaymentStatus.Complete;

				var cart = await _cartRepository.GetByIdAsync(paymentToUpdate.CartId);
				var seats = cart.Items.Select(x => x.Seat);

				foreach (var seat in seats)
				{
					seat.SeatState = SeatState.Booked;
					await _seatRepository.UpdateAsync(seat);
				}

				await _paymentRepository.UpdateAsync(paymentToUpdate);

				return Ok();
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
				return StatusCode(500);
			}
		}

		[HttpPost]
		[Route("{paymentId}/failed")]
		public async Task<IActionResult> Failed(int paymentId)
		{
			try
			{
				var paymentToUpdate = await _paymentRepository.GetByIdAsync(paymentId);
				paymentToUpdate.PaymentStatus = PaymentStatus.Failed;

				var cart = await _cartRepository.GetByIdAsync(paymentToUpdate.CartId);
				var seats = cart.Items.Select(x => x.Seat);

				foreach (var seat in seats)
				{
					seat.SeatState = SeatState.Available;
					await _seatRepository.UpdateAsync(seat);
				}

				await _paymentRepository.UpdateAsync(paymentToUpdate);

				return Ok();
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
				return StatusCode(500);
			}
		}
	}
}
