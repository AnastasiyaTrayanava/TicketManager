using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System.Data;
using TicketManager.Common.Enums;
using TicketManager.Common.Interface;
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
		private IAppDbContext _dbContext;
		private IMemoryCache _memoryCache;

		public OrderController(
			IRepository<Cart, Guid> cartRepository,
			IRepository<Seat, int> seatRepository,
			IRepository<Payment, int> paymentRepository,
			IAppDbContext dbContext,
			IMemoryCache memoryCache)
		{
			_cartRepository = cartRepository;
			_seatRepository = seatRepository;
			_paymentRepository = paymentRepository;
			_dbContext = dbContext;
			_memoryCache = memoryCache;
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
		public async Task<IActionResult> AddToCart(Guid cartId, [FromBody] CartItemViewModel addedCartItem)
		{
			try
			{
				if (addedCartItem.EventId == null || addedCartItem.PriceId == null || addedCartItem.SeatId == null)
				{
					return BadRequest();
				}

				var cart = await _cartRepository.GetByIdAsync(cartId);
				cart.Items.Add(new CartItem()
				{
					EventId = (int)addedCartItem.EventId,
					PriceId = (int)addedCartItem.PriceId,
					SeatId = (int)addedCartItem.SeatId,
					Event = addedCartItem.Event,
					Price = addedCartItem.Price,
					Seat = addedCartItem.Seat
				});
				await _cartRepository.UpdateAsync(cart);

				if (addedCartItem.SeatId != null)
				{
					_memoryCache.Remove($"{addedCartItem.EventId}:{addedCartItem.Seat.Row.SectionId}:seats");
					_memoryCache.Remove("events");
				}

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
			await using var transaction = await _dbContext.Database.BeginTransactionAsync();

			try
			{
				var cart = await _cartRepository.GetByIdAsync(cartId);
				var seats = cart.Items.Select(x => x.Seat);

				if (!seats.Any() || seats.Any(x => x.SeatState == SeatState.Reserved || x.SeatState == SeatState.Sold))
				{
					await transaction.RollbackAsync();
					return BadRequest();
				}

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

				foreach (var item in cart.Items)
				{
					_memoryCache.Remove($"{item.EventId}:{item.Seat.Row.SectionId}:seats");
				}
				_memoryCache.Remove("events");

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

		[HttpPut]
		[Route("{cartId}/book-optimistic")]
		public async Task<IActionResult> BookSeatsOptimistic(Guid cartId)
		{
			await using var transaction = await _dbContext.Database.BeginTransactionAsync();

			try
			{
				var cart = await _cartRepository.GetByIdAsync(cartId);
				var seats = cart.Items.Select(x => x.Seat);

				if (!seats.Any() || seats.Any(x => x.SeatState == SeatState.Reserved || x.SeatState == SeatState.Sold))
				{
					await transaction.RollbackAsync();
					return BadRequest();
				}

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

				foreach (var item in cart.Items)
				{
					_memoryCache.Remove($"{item.EventId}:{item.Seat.Row.SectionId}:seats");
				}
				_memoryCache.Remove("events");

				await transaction.CommitAsync();
				return Ok(id);
			}
			catch (DbUpdateConcurrencyException ex)
			{
				await transaction.RollbackAsync();
				Console.WriteLine(ex);
				return Conflict("Resource was already modified. Please retry");
			}
			catch (Exception e)
			{
				await transaction.RollbackAsync();
				Console.WriteLine(e);
				return StatusCode(500);
			}
		}

		[HttpPut]
		[Route("{cartId}/book-pessimistic")]
		public async Task<IActionResult> BookSeatsPessimistic(Guid cartId, CancellationToken token)
		{
			await using var transaction = await _dbContext.Database.BeginTransactionAsync(IsolationLevel.Serializable, token);

			try
			{
				var cart = await _cartRepository.GetByIdAsync(cartId);
				var seats = cart.Items.Select(x => x.Seat);

				if (!seats.Any() || seats.Any(x => x.SeatState == SeatState.Reserved || x.SeatState == SeatState.Sold))
				{
					await transaction.RollbackAsync(token);
					return BadRequest();
				}

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

				foreach (var item in cart.Items)
				{
					_memoryCache.Remove($"{item.EventId}:{item.Seat.Row.SectionId}:seats");
				}
				_memoryCache.Remove("events");

				await transaction.CommitAsync(token);
				return Ok(id);
			}
			catch (Exception e)
			{
				await transaction.RollbackAsync(token);
				Console.WriteLine(e);
				return StatusCode(500);
			}
		}
	}
}
