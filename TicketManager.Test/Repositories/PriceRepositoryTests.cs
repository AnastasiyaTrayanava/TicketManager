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
	public class PriceRepositoryTests
	{
		private Mock<IAppDbContext> _dbContextMock;
		private Mock<DbSet<Price>> _priceDbSetMock;
		private PriceRepository _priceRepository;

		private List<Price> _prices;

		[TestInitialize]
		public void Setup()
		{
			_prices = CreatePriceList();
			_priceDbSetMock = _prices.BuildMockDbSet();
			_dbContextMock = new Mock<IAppDbContext>();

			_dbContextMock.SetupGet(x => x.Prices).Returns(_priceDbSetMock.Object);

			_priceRepository = new PriceRepository(_dbContextMock.Object);
		}

		[TestMethod]
		public async Task CreateAsync_AddPrice_ReturnId()
		{
			var price = new Price() { PriceId = 2, PriceTier = PriceTier.Promotion, PriceValue = 59.99f, SeatId = 5 };

			var result = await _priceRepository.CreateAsync(price);

			Assert.IsNotNull(result);
			Assert.AreEqual(2, result);
			_priceDbSetMock.Verify(x => x.AddAsync(price, It.IsAny<CancellationToken>()), Times.Once);
			_dbContextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
		}

		[TestMethod]
		[ExpectedException(typeof(NullReferenceException))]
		public async Task CreateAsync_EntityIsNull_ThrowsException()
		{
			await _priceRepository.CreateAsync(null);
		}

		[TestMethod]
		public async Task DeleteAsync_PriceFound_DeletesEntity()
		{
			var price = _prices.First();
			_priceDbSetMock.Setup(x => x.FindAsync(price.PriceId)).ReturnsAsync(price);

			await _priceRepository.DeleteAsync(price.PriceId);

			_priceDbSetMock.Verify(x => x.Remove(It.IsAny<Price>()), Times.Once);
			_dbContextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
		}

		[TestMethod]
		public async Task DeleteAsync_PriceNotExists_DoNothing()
		{
			var priceId = 5;
			_priceDbSetMock.Setup(x => x.FindAsync(priceId)).ReturnsAsync((Price)null);

			await _priceRepository.DeleteAsync(priceId);

			_priceDbSetMock.Verify(x => x.Remove(It.IsAny<Price>()), Times.Never);
			_dbContextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
		}

		[TestMethod]
		public async Task GetAsync_ReturnsPriceList()
		{
			var result = await _priceRepository.GetAsync();

			Assert.IsNotNull(result);
			Assert.AreEqual(3, result.Count);
		}

		[TestMethod]
		public async Task GetAsync_NoPrices_ReturnsEmptyList()
		{
			var priceDbMockSet = (new List<Price>()).BuildMockDbSet();
			var dbContextMock = new Mock<IAppDbContext>();
			dbContextMock.SetupGet(x => x.Prices).Returns(priceDbMockSet.Object);
			var repository = new PriceRepository(dbContextMock.Object);

			var result = await repository.GetAsync();

			Assert.IsNotNull(result);
			Assert.AreEqual(0, result.Count);
		}

		[TestMethod]
		public async Task GetByIdAsync_PriceExists_ReturnsPrice()
		{
			var priceId = 0;
			_priceDbSetMock.Setup(x => x.FindAsync(priceId)).ReturnsAsync(_prices.First());

			var result = await _priceRepository.GetByIdAsync(priceId);

			Assert.IsNotNull(result);
			Assert.AreEqual(priceId, result.PriceId);
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public async Task GetByIdAsync_PriceNotFound_ThrowsException()
		{
			var priceId = 5;

			await _priceRepository.GetByIdAsync(priceId);
		}

		[TestMethod]
		public async Task GetSortedAsync_SortByPriceStatusAscending_ReturnsSortedList()
		{
			var sortingInstructions = new SortingInstructions<Price>
			{
				OrderBy = c => c.PriceTier,
				Direction = OrderByDirection.Ascending
			};

			var result = await _priceRepository.GetSortedAsync(sortingInstructions);

			Assert.AreEqual(3, result.Count);
		}

		[TestMethod]
		public async Task GetSortedAsync_SortByPriceStatusAscendingNoPrices_ReturnsEmptyList()
		{
			var priceDbMockSet = (new List<Price>()).BuildMockDbSet();
			var dbContextMock = new Mock<IAppDbContext>();
			dbContextMock.SetupGet(x => x.Prices).Returns(priceDbMockSet.Object);
			var repository = new PriceRepository(dbContextMock.Object);

			var sortingInstructions = new SortingInstructions<Price>
			{
				OrderBy = c => c.PriceTier,
				Direction = OrderByDirection.Descending
			};

			var result = await repository.GetSortedAsync(sortingInstructions);

			Assert.AreEqual(0, result.Count);
		}

		[TestMethod]
		public async Task UpdateAsync_UpdatedModel_ReturnsUpdatedPrice()
		{
			var price = new Price() { PriceId = _prices.First().PriceId };

			await _priceRepository.UpdateAsync(price);

			_priceDbSetMock.Verify(s => s.Update(price), Times.Once);
			_dbContextMock.Verify(s => s.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
		}

		private static List<Price> CreatePriceList()
		{
			return
			[
				new Price
				{
					PriceId = 0,
					PriceTier = PriceTier.Adult,
					PriceValue = 100.0f,
					SeatId = 0,
				},
				new Price
				{
					PriceId = 1,
					PriceTier = PriceTier.Vip,
					PriceValue = 200.0f,
					SeatId = 1,
				},
				new Price
				{
					PriceId = 0,
					PriceTier = PriceTier.Child,
					PriceValue = 50.0f,
					SeatId = 3,
				}
			];
		}
	}
}
