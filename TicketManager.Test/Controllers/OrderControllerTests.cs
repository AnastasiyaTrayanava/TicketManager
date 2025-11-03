using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using TicketManager.Common.Enums;
using TicketManager.Common.Interface;
using TicketManager.Common.Models;
using TicketManager.Common.Models.Entities;
using TicketManager.Common.Models.ViewModels;
using TicketManager.Controllers.Ticketing;
using TicketManager.DAL;

namespace TicketManager.Test.Controllers
{
	[TestClass]
	public class OrderControllerTests
	{
		private Mock<IRepository<Cart, Guid>> _cartRepositoryMock;
		private Mock<IRepository<Seat, int>> _seatRepositoryMock;
		private Mock<IRepository<Payment, int>> _paymentRepositoryMock;
		private Mock<IAppDbContext> _dbContextMock;
		private Mock<IMemoryCache> _memoryCacheMock;

		private OrderController _orderController;

		public OrderControllerTests()
		{
			_cartRepositoryMock = new Mock<IRepository<Cart, Guid>>();
			_seatRepositoryMock = new Mock<IRepository<Seat, int>>();
			_paymentRepositoryMock = new Mock<IRepository<Payment, int>>();

			_dbContextMock = new Mock<IAppDbContext>();
			_memoryCacheMock = new Mock<IMemoryCache>();

			_orderController = new OrderController(_cartRepositoryMock.Object, _seatRepositoryMock.Object,
				_paymentRepositoryMock.Object, _dbContextMock.Object, _memoryCacheMock.Object);
		}

		[TestMethod]
		public async Task Get_CartItemsExist_Returns200()
		{
			var cartGuid = Guid.NewGuid();
			var cartItem = new Cart
			{
				CartId = cartGuid,
				Items =
				[
					new() { CartItemId = 0, EventId = 1, PriceId = 2, SeatId = 3 }
				]
			};

			_cartRepositoryMock.Setup(x => x.GetByIdAsync(cartGuid)).ReturnsAsync(cartItem);

			var result = await _orderController.Get(cartGuid) as OkObjectResult;

			Assert.IsNotNull(result);
			Assert.AreEqual(200, result.StatusCode);
		}

		[TestMethod]
		public async Task Get_RepositoryThrowsException_Returns500()
		{
			var cartGuid = Guid.NewGuid();

			_cartRepositoryMock.Setup(x => x.GetByIdAsync(cartGuid)).ThrowsAsync(new Exception("Not found"));

			var result = await _orderController.Get(cartGuid) as StatusCodeResult;

			Assert.IsNotNull(result);
			Assert.AreEqual(500, result.StatusCode);
		}

		[TestMethod]
		public async Task AddToCart_CartExists_Returns200()
		{
			var cartGuid = Guid.NewGuid();
			var cartItem = new Cart
			{
				CartId = cartGuid,
				Items =
				[
					new() { CartItemId = 0, EventId = 1, PriceId = 2, SeatId = 3 }
				]
			};

			_cartRepositoryMock.Setup(x => x.GetByIdAsync(cartGuid)).ReturnsAsync(cartItem);

			var result = await _orderController.AddToCart(cartGuid, new CartItemViewModel() {EventId = 1, PriceId = 2, SeatId = 4}) as OkObjectResult;

			Assert.IsNotNull(result);
			Assert.AreEqual(200, result.StatusCode);
			_cartRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Cart>()), Times.Once);
		}

		[TestMethod]
		public async Task AddToCart_CartDoesntExist_Returns500()
		{
			var cartGuid = Guid.NewGuid();
			_cartRepositoryMock.Setup(x => x.GetByIdAsync(cartGuid)).Throws(new Exception("Not found"));

			var result = await _orderController.AddToCart(cartGuid, new CartItemViewModel() { EventId = 1, PriceId = 2, SeatId = 4 }) as StatusCodeResult;

			Assert.IsNotNull(result);
			Assert.AreEqual(500, result.StatusCode);
		}

		[TestMethod]
		public async Task RemoveFromCart_Success_Returns200()
		{
			int eventId = 1, priceId = 2;
			var cartGuid = Guid.NewGuid();
			var cartItem = new Cart
			{
				CartId = cartGuid,
				Items =
				[
					new() { CartItemId = 0, EventId = eventId, PriceId = priceId, SeatId = 3 },
					new() { CartItemId = 0, EventId = eventId, PriceId = priceId, SeatId = 4 }
				]
			};

			_cartRepositoryMock.Setup(x => x.GetByIdAsync(cartGuid)).ReturnsAsync(cartItem);

			var result = await _orderController.RemoveFromCart(cartGuid, eventId, 3) as StatusCodeResult;

			Assert.IsNotNull(result);
			Assert.AreEqual(200, result.StatusCode);
			_cartRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Cart>()), Times.Once);
		}

		[TestMethod]
		public async Task RemoveFromCart_CartDoesntExist_Returns200()
		{
			int eventId = 1, priceId = 2;
			var cartGuid = Guid.NewGuid();

			_cartRepositoryMock.Setup(x => x.GetByIdAsync(cartGuid)).Throws(new Exception("Not Found"));

			var result = await _orderController.RemoveFromCart(cartGuid, eventId, priceId) as StatusCodeResult;

			Assert.IsNotNull(result);
			Assert.AreEqual(500, result.StatusCode);
		}

		[TestMethod]
		public async Task BookSeats_Success_Returns200()
		{
			int eventId = 1, priceId = 2, paymentId = 0;
			var cartGuid = Guid.NewGuid();
			var cartItem = new Cart
			{
				CartId = cartGuid,
				Items =
				[
					new()
					{
						CartItemId = 0, 
						EventId = eventId, 
						PriceId = priceId, 
						SeatId = 3,
						Seat = new Seat()
						{
							RowId = 0,
							SeatId = 3,
							SeatNumber = 3,
							SeatState = SeatState.Available
						}
					},
					new()
					{
						CartItemId = 0, 
						EventId = eventId,
						PriceId = priceId, 
						SeatId = 4,
						Seat = new Seat()
						{
							RowId = 0,
							SeatId = 4,
							SeatNumber = 4,
							SeatState = SeatState.Available
						}
					}
				]
			};

			SetupDbMock();

			_cartRepositoryMock.Setup(x => x.GetByIdAsync(cartGuid)).ReturnsAsync(cartItem);
			_paymentRepositoryMock.Setup(x => x.CreateAsync(It.IsAny<Payment>())).ReturnsAsync(paymentId);

			var result = await _orderController.BookSeats(cartGuid) as OkObjectResult;

			Assert.IsNotNull(result);
			Assert.AreEqual(200, result.StatusCode);
			_seatRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Seat>()), Times.AtLeast(2));
		}

		[TestMethod]
		public async Task BookSeats_CartDoesntExist_Returns500()
		{
			int eventId = 1, priceId = 2, paymentId = 0;
			var cartGuid = Guid.NewGuid();

			SetupDbMock();

			_cartRepositoryMock.Setup(x => x.GetByIdAsync(cartGuid)).Throws(new Exception());

			var result = await _orderController.BookSeats(cartGuid) as StatusCodeResult;

			Assert.IsNotNull(result);
			Assert.AreEqual(500, result.StatusCode);
		}

		private void SetupDbMock()
		{
			var databaseFacadeMock = new Mock<DatabaseFacade>(_dbContextMock.Object);
			var dbTransactionMock = new Mock<IDbContextTransaction>();

			_dbContextMock.Setup(x => x.Database).Returns(databaseFacadeMock.Object);
			databaseFacadeMock.Setup(x => x.BeginTransactionAsync(It.IsAny<CancellationToken>())).ReturnsAsync(dbTransactionMock.Object);
		}
	}
}
