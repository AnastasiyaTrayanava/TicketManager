using Microsoft.EntityFrameworkCore;
using MockQueryable.Moq;
using Moq;
using TicketManager.Common.Enums;
using TicketManager.Common.Interface;
using TicketManager.Common.Models;
using TicketManager.DAL.Repositories;

namespace TicketManager.Test.Repositories
{
	[TestClass]
	public class SeatRepositoryTests
	{
		private Mock<IAppDbContext> _dbContextMock;
		private Mock<DbSet<Seat>> _seatDbSetMock;
		private SeatRepository _seatRepository;

		private List<Seat> _seats;

		[TestInitialize]
		public void Setup()
		{
			_seats = CreateSeatList();
			_seatDbSetMock = _seats.BuildMockDbSet();
			_dbContextMock = new Mock<IAppDbContext>();

			_dbContextMock.SetupGet(x => x.Seats).Returns(_seatDbSetMock.Object);

			_seatRepository = new SeatRepository(_dbContextMock.Object);
		}

		[TestMethod]
		public async Task CreateAsync_AddSeat_ReturnId()
		{
			var seat = new Seat() { SeatId = 2, RowId = 0, Price = new List<Price>(), SeatNumber = 1, SeatState = SeatState.Available};

			var result = await _seatRepository.CreateAsync(seat);

			Assert.IsNotNull(result);
			Assert.AreEqual(2, result);
			_seatDbSetMock.Verify(x => x.AddAsync(seat, It.IsAny<CancellationToken>()), Times.Once);
			_dbContextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
		}

		[TestMethod]
		[ExpectedException(typeof(NullReferenceException))]
		public async Task CreateAsync_EntityIsNull_ThrowsException()
		{
			await _seatRepository.CreateAsync(null);
		}

		[TestMethod]
		public async Task DeleteAsync_SeatFound_DeletesEntity()
		{
			var seat = _seats.First();
			_seatDbSetMock.Setup(x => x.FindAsync(seat.SeatId)).ReturnsAsync(seat);

			await _seatRepository.DeleteAsync(seat.SeatId);

			_seatDbSetMock.Verify(x => x.Remove(It.IsAny<Seat>()), Times.Once);
			_dbContextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
		}

		[TestMethod]
		public async Task DeleteAsync_SeatNotExists_DoNothing()
		{
			var seatId = 5;
			_seatDbSetMock.Setup(x => x.FindAsync(seatId)).ReturnsAsync((Seat)null);

			await _seatRepository.DeleteAsync(seatId);

			_seatDbSetMock.Verify(x => x.Remove(It.IsAny<Seat>()), Times.Never);
			_dbContextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
		}

		[TestMethod]
		public async Task GetAsync_ReturnsSeatList()
		{
			var result = await _seatRepository.GetAsync();

			Assert.IsNotNull(result);
			Assert.AreEqual(2, result.Count);
		}

		[TestMethod]
		public async Task GetAsync_NoSeats_ReturnsEmptyList()
		{
			var seatDbMockSet = (new List<Seat>()).BuildMockDbSet();
			var dbContextMock = new Mock<IAppDbContext>();
			dbContextMock.SetupGet(x => x.Seats).Returns(seatDbMockSet.Object);
			var repository = new SeatRepository(dbContextMock.Object);

			var result = await repository.GetAsync();

			Assert.IsNotNull(result);
			Assert.AreEqual(0, result.Count);
		}

		[TestMethod]
		public async Task GetByIdAsync_SeatExists_ReturnsSeat()
		{
			var seatId = 0;
			_seatDbSetMock.Setup(x => x.FindAsync(seatId)).ReturnsAsync(_seats.First());

			var result = await _seatRepository.GetByIdAsync(seatId);

			Assert.IsNotNull(result);
			Assert.AreEqual(seatId, result.SeatId);
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public async Task GetByIdAsync_SeatNotFound_ThrowsException()
		{
			var seatId = 5;

			await _seatRepository.GetByIdAsync(seatId);
		}

		[TestMethod]
		public async Task GetSortedAsync_SortBySeatNumberAscending_ReturnsSortedList()
		{
			var sortingInstructions = new SortingInstructions<Seat>
			{
				OrderBy = c => c.SeatNumber,
				Direction = OrderByDirection.Ascending
			};

			var result = await _seatRepository.GetSortedAsync(sortingInstructions);

			Assert.AreEqual(2, result.Count);
		}

		[TestMethod]
		public async Task GetSortedAsync_SortBySeatNumberAscendingNoSeats_ReturnsEmptyList()
		{
			var seatDbMockSet = (new List<Seat>()).BuildMockDbSet();
			var dbContextMock = new Mock<IAppDbContext>();
			dbContextMock.SetupGet(x => x.Seats).Returns(seatDbMockSet.Object);
			var repository = new SeatRepository(dbContextMock.Object);

			var sortingInstructions = new SortingInstructions<Seat>
			{
				OrderBy = c => c.SeatNumber,
				Direction = OrderByDirection.Descending
			};

			var result = await repository.GetSortedAsync(sortingInstructions);

			Assert.AreEqual(0, result.Count);
		}

		[TestMethod]
		public async Task UpdateAsync_UpdatedModel_ReturnsUpdatedSeat()
		{
			var seat = new Seat() { SeatId = _seats.First().SeatId };

			await _seatRepository.UpdateAsync(seat);

			_seatDbSetMock.Verify(s => s.Update(seat), Times.Once);
			_dbContextMock.Verify(s => s.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
		}

		private static List<Seat> CreateSeatList()
		{
			return
			[
				new Seat { SeatId = 0, RowId = 1, SeatNumber = 1, Price = new List<Price>(), SeatState = SeatState.Reserved },
				new Seat { SeatId = 1, RowId = 1, SeatNumber = 2, Price = new List<Price>(), SeatState = SeatState.Available }
			];
		}
	}
}
