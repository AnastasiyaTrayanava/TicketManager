using Microsoft.AspNetCore.Mvc;
using Moq;
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
		private Mock<AppDbContext> _dbContextMock;

		private OrderController _orderController;

		public OrderControllerTests()
		{
			_cartRepositoryMock = new Mock<IRepository<Cart, Guid>>();
			_seatRepositoryMock = new Mock<IRepository<Seat, int>>();
			_paymentRepositoryMock = new Mock<IRepository<Payment, int>>();
			_dbContextMock = new Mock<AppDbContext>();

			_orderController = new OrderController(_cartRepositoryMock.Object, _seatRepositoryMock.Object,
				_paymentRepositoryMock.Object, _dbContextMock.Object);
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

			var result = await _orderController.RemoveFromCart(cartGuid, eventId, priceId) as OkObjectResult;

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

			var result = await _orderController.RemoveFromCart(cartGuid, eventId, priceId) as OkObjectResult;

			Assert.IsNotNull(result);
			Assert.AreEqual(200, result.StatusCode);
			_cartRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Cart>()), Times.Once);
		}
	}
}
