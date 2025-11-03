using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Moq;
using TicketManager.Common.Enums;
using TicketManager.Common.Interface;
using TicketManager.Common.Models.Entities;
using TicketManager.Controllers.Ticketing;
using TicketManager.DAL;

namespace TicketManager.Test.Controllers
{
	[TestClass]
	public class PaymentControllerTests
	{
		private Mock<IRepository<Payment, int>> _paymentRepositoryMock;
		private Mock<IRepository<Cart, Guid>>_cartRepositoryMock;
		private Mock<IRepository<Seat, int>> _seatRepositoryMock;
		private Mock<AppDbContext> _dbContextMock;

		private PaymentController _paymentController;

		public PaymentControllerTests()
		{
			_paymentRepositoryMock = new Mock<IRepository<Payment, int>>();
			_cartRepositoryMock = new Mock<IRepository<Cart, Guid>>();
			_seatRepositoryMock = new Mock<IRepository<Seat, int>>();

			_dbContextMock = new Mock<AppDbContext>();

			_paymentController = new PaymentController(_paymentRepositoryMock.Object, _cartRepositoryMock.Object,
				_seatRepositoryMock.Object, _dbContextMock.Object);
		}

		[TestMethod]
		public async Task GetById_PaymentExists_Returns200()
		{
			var paymentId = 0;
			var payment = new Payment()
			{
				CartId = Guid.NewGuid(),
				PaymentId = paymentId,
				PaymentStatus = PaymentStatus.Pending
			};

			_paymentRepositoryMock.Setup(x => x.GetByIdAsync(paymentId)).ReturnsAsync(payment);

			var result = await _paymentController.GetById(paymentId) as OkObjectResult;

			Assert.IsNotNull(result);
			Assert.AreEqual(200, result.StatusCode);
		}

		[TestMethod]
		public async Task GetById_PaymentDoesntExist_Returns500()
		{
			var paymentId = 0;
			_paymentRepositoryMock.Setup(x => x.GetByIdAsync(paymentId)).Throws(new Exception());

			var result = await _paymentController.GetById(paymentId) as StatusCodeResult;

			Assert.IsNotNull(result);
			Assert.AreEqual(500, result.StatusCode);
		}

		[TestMethod]
		public async Task Complete_PaymentExists_Returns200()
		{
			var paymentId = 0;
			var cartId = Guid.NewGuid();
			var payment = new Payment()
			{
				CartId = cartId,
				PaymentId = paymentId,
				PaymentStatus = PaymentStatus.Pending
			};
			var cart = new Cart()
			{
				CartId = cartId,
				PaymentId = paymentId,
				Items = new List<CartItem>()
				{
					new CartItem()
					{
						CartId = cartId,
						CartItemId = 0,
						EventId = 0,
						PriceId = 0,
						SeatId = 0,
						Seat = new Seat()
						{
							RowId = 1,
							SeatId = 1,
							SeatNumber = 1,
							SeatState = SeatState.Reserved
						}
					}
				}
			};

			SetupDbMock();

			_paymentRepositoryMock.Setup(x => x.GetByIdAsync(paymentId)).ReturnsAsync(payment);
			_cartRepositoryMock.Setup(x => x.GetByIdAsync(cartId)).ReturnsAsync(cart);

			var result = await _paymentController.Complete(paymentId) as StatusCodeResult;

			Assert.IsNotNull(result);
			Assert.AreEqual(200, result.StatusCode);
			Assert.AreEqual(SeatState.Sold, cart.Items.First().Seat.SeatState);
			_seatRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Seat>()), Times.Once);
			_paymentRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Payment>()), Times.Once);
		}

		[TestMethod]
		public async Task Complete_PaymentDoesntExist_Returns500()
		{
			var paymentId = 0;
			SetupDbMock();
			_paymentRepositoryMock.Setup(x => x.GetByIdAsync(paymentId)).Throws(new Exception());

			var result = await _paymentController.Complete(paymentId) as StatusCodeResult;

			Assert.IsNotNull(result);
			Assert.AreEqual(500, result.StatusCode);
		}

		[TestMethod]
		public async Task Failed_PaymentExists_Returns200()
		{
			var paymentId = 0;
			var cartId = Guid.NewGuid();
			var payment = new Payment()
			{
				CartId = cartId,
				PaymentId = paymentId,
				PaymentStatus = PaymentStatus.Pending
			};
			var cart = new Cart()
			{
				CartId = cartId,
				PaymentId = paymentId,
				Items = new List<CartItem>()
				{
					new CartItem()
					{
						CartId = cartId,
						CartItemId = 0,
						EventId = 0,
						PriceId = 0,
						SeatId = 0,
						Seat = new Seat()
						{
							RowId = 1,
							SeatId = 1,
							SeatNumber = 1,
							SeatState = SeatState.Reserved
						}
					}
				}
			};

			SetupDbMock();

			_paymentRepositoryMock.Setup(x => x.GetByIdAsync(paymentId)).ReturnsAsync(payment);
			_cartRepositoryMock.Setup(x => x.GetByIdAsync(cartId)).ReturnsAsync(cart);

			var result = await _paymentController.Failed(paymentId) as StatusCodeResult;

			Assert.IsNotNull(result);
			Assert.AreEqual(200, result.StatusCode);
			Assert.AreEqual(SeatState.Available, cart.Items.First().Seat.SeatState);
			_seatRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Seat>()), Times.Once);
			_paymentRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Payment>()), Times.Once);
		}



		[TestMethod]
		public async Task Failed_PaymentDoesntExist_Returns500()
		{
			var paymentId = 0;
			SetupDbMock();
			_paymentRepositoryMock.Setup(x => x.GetByIdAsync(paymentId)).Throws(new Exception());

			var result = await _paymentController.Failed(paymentId) as StatusCodeResult;

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
