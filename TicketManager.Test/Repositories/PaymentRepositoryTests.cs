using Microsoft.EntityFrameworkCore;
using MockQueryable.Moq;
using Moq;
using TicketManager.Common.Enums;
using TicketManager.Common.Interface;
using TicketManager.Common.Models;
using TicketManager.Common.Models.Entities;
using TicketManager.DAL.Repositories;

namespace TicketManager.Test.Repositories
{
	[TestClass]
	public class PaymentRepositoryTests
	{
		private Mock<IAppDbContext> _dbContextMock;
		private Mock<DbSet<Payment>> _paymentDbSetMock;
		private PaymentRepository _paymentRepository;

		private List<Payment> _payments;

		[TestInitialize]
		public void Setup()
		{
			_payments = CreatePaymentList();
			_paymentDbSetMock = _payments.BuildMockDbSet();
			_dbContextMock = new Mock<IAppDbContext>();

			_dbContextMock.SetupGet(x => x.Payments).Returns(_paymentDbSetMock.Object);

			_paymentRepository = new PaymentRepository(_dbContextMock.Object);
		}

		[TestMethod]
		public async Task CreateAsync_AddPayment_ReturnId()
		{
			var payment = new Payment() {CartId = Guid.NewGuid(), PaymentId = 3, PaymentStatus = PaymentStatus.Pending};

			var result = await _paymentRepository.CreateAsync(payment);

			Assert.IsNotNull(result);
			Assert.AreEqual(3, result);
			_paymentDbSetMock.Verify(x => x.AddAsync(payment, It.IsAny<CancellationToken>()), Times.Once);
			_dbContextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
		}

		[TestMethod]
		[ExpectedException(typeof(NullReferenceException))]
		public async Task CreateAsync_EntityIsNull_ThrowsException()
		{
			await _paymentRepository.CreateAsync(null);
		}

		[TestMethod]
		public async Task DeleteAsync_PaymentFound_DeletesEntity()
		{
			var payment = _payments.First();
			_paymentDbSetMock.Setup(x => x.FindAsync(payment.PaymentId)).ReturnsAsync(payment);

			await _paymentRepository.DeleteAsync(payment.PaymentId);

			_paymentDbSetMock.Verify(x => x.Remove(It.IsAny<Payment>()), Times.Once);
			_dbContextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
		}

		[TestMethod]
		public async Task DeleteAsync_PaymentNotExists_DoNothing()
		{
			var paymentId = 5;
			_paymentDbSetMock.Setup(x => x.FindAsync(paymentId)).ReturnsAsync((Payment)null);

			await _paymentRepository.DeleteAsync(paymentId);

			_paymentDbSetMock.Verify(x => x.Remove(It.IsAny<Payment>()), Times.Never);
			_dbContextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
		}

		[TestMethod]
		public async Task GetAsync_ReturnsPaymentList()
		{
			var result = await _paymentRepository.GetAsync();

			Assert.IsNotNull(result);
			Assert.AreEqual(3, result.Count);
		}

		[TestMethod]
		public async Task GetAsync_NoPayments_ReturnsEmptyList()
		{
			var paymentDbMockSet = (new List<Payment>()).BuildMockDbSet();
			var dbContextMock = new Mock<IAppDbContext>();
			dbContextMock.SetupGet(x => x.Payments).Returns(paymentDbMockSet.Object);
			var repository = new PaymentRepository(dbContextMock.Object);

			var result = await repository.GetAsync();

			Assert.IsNotNull(result);
			Assert.AreEqual(0, result.Count);
		}

		[TestMethod]
		public async Task GetByIdAsync_PaymentExists_ReturnsPayment()
		{
			var paymentId = 0;
			_paymentDbSetMock.Setup(x => x.FindAsync(paymentId)).ReturnsAsync(_payments.First());

			var result = await _paymentRepository.GetByIdAsync(paymentId);

			Assert.IsNotNull(result);
			Assert.AreEqual(paymentId, result.PaymentId);
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public async Task GetByIdAsync_PaymentNotFound_ThrowsException()
		{
			var paymentId = 5;

			await _paymentRepository.GetByIdAsync(paymentId);
		}

		[TestMethod]
		public async Task GetSortedAsync_SortByPaymentStatusAscending_ReturnsSortedList()
		{
			var sortingInstructions = new SortingInstructions<Payment>
			{
				OrderBy = c => c.PaymentStatus,
				Direction = OrderByDirection.Ascending
			};

			var result = await _paymentRepository.GetSortedAsync(sortingInstructions);

			Assert.AreEqual(3, result.Count);
		}

		[TestMethod]
		public async Task GetSortedAsync_SortByPaymentStatusAscendingNoPayments_ReturnsEmptyList()
		{
			var paymentDbMockSet = (new List<Payment>()).BuildMockDbSet();
			var dbContextMock = new Mock<IAppDbContext>();
			dbContextMock.SetupGet(x => x.Payments).Returns(paymentDbMockSet.Object);
			var repository = new PaymentRepository(dbContextMock.Object);

			var sortingInstructions = new SortingInstructions<Payment>
			{
				OrderBy = c => c.PaymentStatus,
				Direction = OrderByDirection.Descending
			};

			var result = await repository.GetSortedAsync(sortingInstructions);

			Assert.AreEqual(0, result.Count);
		}

		[TestMethod]
		public async Task UpdateAsync_UpdatedModel_ReturnsUpdatedPayment()
		{
			var payment = new Payment() { PaymentId = _payments.First().PaymentId };

			await _paymentRepository.UpdateAsync(payment);

			_paymentDbSetMock.Verify(s => s.Update(payment), Times.Once);
			_dbContextMock.Verify(s => s.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
		}

		private static List<Payment> CreatePaymentList()
		{
			return
			[
				new Payment()
				{
					CartId = Guid.NewGuid(),
					PaymentId = 0,
					PaymentStatus = PaymentStatus.Pending,
				},
				new Payment()
				{
					CartId = Guid.NewGuid(),
					PaymentId = 1,
					PaymentStatus = PaymentStatus.Complete
				},
				new Payment()
				{
					CartId = Guid.NewGuid(),
					PaymentId = 2,
					PaymentStatus = PaymentStatus.Failed,
				}
			];
		}
	}
}
