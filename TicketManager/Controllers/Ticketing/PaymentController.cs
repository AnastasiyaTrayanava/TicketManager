using Microsoft.AspNetCore.Mvc;
using TicketManager.Common.Enums;
using TicketManager.Common.Interface;
using TicketManager.Common.Models;
using TicketManager.Common.Models.Entities;
using TicketManager.DAL;

namespace TicketManager.Controllers.Ticketing
{
	[Route("payments")]
	public class PaymentController : Controller
	{
		private IRepository<Payment, int> _paymentRepository;
		private IRepository<Cart, Guid> _cartRepository;
		private IRepository<Seat, int> _seatRepository;
		private IAppDbContext _dbContext;
		private INotificationService _notificationService;

		public PaymentController(
			IRepository<Payment, int> paymentRepository,
			IRepository<Cart, Guid> cartRepository,
			IRepository<Seat, int> seatRepository,
			IAppDbContext dbContext,
			INotificationService notificationService)
		{
			_paymentRepository = paymentRepository;
			_cartRepository = cartRepository;
			_seatRepository = seatRepository;
			_dbContext = dbContext;
			_notificationService = notificationService;
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
			var transaction = await _dbContext.Database.BeginTransactionAsync();
			try
			{
				await UpdatePayment(paymentId, SeatState.Sold, PaymentStatus.Complete);

				await transaction.CommitAsync();
				return Ok();
			}
			catch (Exception e)
			{
				await transaction.RollbackAsync();
				Console.WriteLine(e);
				return StatusCode(500);
			}
		}

		[HttpPost]
		[Route("{paymentId}/failed")]
		public async Task<IActionResult> Failed(int paymentId)
		{
			var transaction = await _dbContext.Database.BeginTransactionAsync();
			try
			{
				await UpdatePayment(paymentId, SeatState.Available, PaymentStatus.Failed);

				await transaction.CommitAsync();
				return Ok();
			}
			catch (Exception e)
			{
				await transaction.RollbackAsync();
				Console.WriteLine(e);
				return StatusCode(500);
			}
		}

		private async Task UpdatePayment(int paymentId, SeatState seatState, PaymentStatus paymentStatus)
		{
			var paymentToUpdate = await _paymentRepository.GetByIdAsync(paymentId);
			paymentToUpdate.PaymentStatus = paymentStatus;

			var cart = await _cartRepository.GetByIdAsync(paymentToUpdate.CartId);
			var seats = cart.Items.Select(x => x.Seat);

			foreach (var seat in seats)
			{
				seat.SeatState = seatState;
				await _seatRepository.UpdateAsync(seat);
			}

			await _paymentRepository.UpdateAsync(paymentToUpdate);
		}
	}
}
