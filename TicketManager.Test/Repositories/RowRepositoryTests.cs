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
	public class RowRepositoryTests
	{
		private Mock<IAppDbContext> _dbContextMock;
		private Mock<DbSet<Row>> _rowDbSetMock;
		private RowRepository _rowRepository;

		private List<Row> _rows;

		[TestInitialize]
		public void Setup()
		{
			_rows = CreateRowList();
			_rowDbSetMock = _rows.BuildMockDbSet();
			_dbContextMock = new Mock<IAppDbContext>();

			_dbContextMock.SetupGet(x => x.Rows).Returns(_rowDbSetMock.Object);

			_rowRepository = new RowRepository(_dbContextMock.Object);
		}

		[TestMethod]
		public async Task CreateAsync_AddRow_ReturnId()
		{
			var row = new Row() {RowId = 3, Seats = new List<Seat>(), SectionId = 5};

			var result = await _rowRepository.CreateAsync(row);

			Assert.IsNotNull(result);
			Assert.AreEqual(3, result);
			_rowDbSetMock.Verify(x => x.AddAsync(row, It.IsAny<CancellationToken>()), Times.Once);
			_dbContextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
		}

		[TestMethod]
		[ExpectedException(typeof(NullReferenceException))]
		public async Task CreateAsync_EntityIsNull_ThrowsException()
		{
			await _rowRepository.CreateAsync(null);
		}

		[TestMethod]
		public async Task DeleteAsync_RowFound_DeletesEntity()
		{
			var row = _rows.First();
			_rowDbSetMock.Setup(x => x.FindAsync(row.RowId)).ReturnsAsync(row);

			await _rowRepository.DeleteAsync(row.RowId);

			_rowDbSetMock.Verify(x => x.Remove(It.IsAny<Row>()), Times.Once);
			_dbContextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
		}

		[TestMethod]
		public async Task DeleteAsync_RowNotExists_DoNothing()
		{
			var rowId = 5;
			_rowDbSetMock.Setup(x => x.FindAsync(rowId)).ReturnsAsync((Row)null);

			await _rowRepository.DeleteAsync(rowId);

			_rowDbSetMock.Verify(x => x.Remove(It.IsAny<Row>()), Times.Never);
			_dbContextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
		}

		[TestMethod]
		public async Task GetAsync_ReturnsRowList()
		{
			var result = await _rowRepository.GetAsync();

			Assert.IsNotNull(result);
			Assert.AreEqual(2, result.Count);
		}

		[TestMethod]
		public async Task GetAsync_NoRows_ReturnsEmptyList()
		{
			var rowDbMockSet = (new List<Row>()).BuildMockDbSet();
			var dbContextMock = new Mock<IAppDbContext>();
			dbContextMock.SetupGet(x => x.Rows).Returns(rowDbMockSet.Object);
			var repository = new RowRepository(dbContextMock.Object);

			var result = await repository.GetAsync();

			Assert.IsNotNull(result);
			Assert.AreEqual(0, result.Count);
		}

		[TestMethod]
		public async Task GetByIdAsync_RowExists_ReturnsRow()
		{
			var rowId = 0;
			_rowDbSetMock.Setup(x => x.FindAsync(rowId)).ReturnsAsync(_rows.First());

			var result = await _rowRepository.GetByIdAsync(rowId);

			Assert.IsNotNull(result);
			Assert.AreEqual(rowId, result.RowId);
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public async Task GetByIdAsync_RowNotFound_ThrowsException()
		{
			var rowId = 5;

			await _rowRepository.GetByIdAsync(rowId);
		}

		[TestMethod]
		public async Task GetSortedAsync_SortByRowStatusAscending_ReturnsSortedList()
		{
			var sortingInstructions = new SortingInstructions<Row>
			{
				OrderBy = c => c.SectionId,
				Direction = OrderByDirection.Ascending
			};

			var result = await _rowRepository.GetSortedAsync(sortingInstructions);

			Assert.AreEqual(2, result.Count);
		}

		[TestMethod]
		public async Task GetSortedAsync_SortByRowStatusAscendingNoRows_ReturnsEmptyList()
		{
			var rowDbMockSet = (new List<Row>()).BuildMockDbSet();
			var dbContextMock = new Mock<IAppDbContext>();
			dbContextMock.SetupGet(x => x.Rows).Returns(rowDbMockSet.Object);
			var repository = new RowRepository(dbContextMock.Object);

			var sortingInstructions = new SortingInstructions<Row>
			{
				OrderBy = c => c.SectionId,
				Direction = OrderByDirection.Descending
			};

			var result = await repository.GetSortedAsync(sortingInstructions);

			Assert.AreEqual(0, result.Count);
		}

		[TestMethod]
		public async Task UpdateAsync_UpdatedModel_ReturnsUpdatedRow()
		{
			var row = new Row() { RowId = _rows.First().RowId };

			await _rowRepository.UpdateAsync(row);

			_rowDbSetMock.Verify(s => s.Update(row), Times.Once);
			_dbContextMock.Verify(s => s.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
		}

		private static List<Row> CreateRowList()
		{
			return
			[
				new Row()
				{
					RowId = 0,
					SectionId = 1,
					Seats = new List<Seat>()
				},
				new Row()
				{
					RowId = 1,
					SectionId = 1,
					Seats = new List<Seat>()
				}
			];
		}
	}
}
