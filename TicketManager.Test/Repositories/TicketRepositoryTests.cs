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
	public class TicketRepositoryTests
	{
		private Mock<IAppDbContext> _dbContextMock;
		private Mock<DbSet<Ticket>> _ticketDbSetMock;
		private TicketRepository _ticketRepository;

		private List<Ticket> _tickets;

		[TestInitialize]
		public void Setup()
		{
			_tickets = CreateTicketList();
			_ticketDbSetMock = _tickets.BuildMockDbSet();
			_dbContextMock = new Mock<IAppDbContext>();

			_dbContextMock.SetupGet(x => x.Tickets).Returns(_ticketDbSetMock.Object);

			_ticketRepository = new TicketRepository(_dbContextMock.Object);
		}

		[TestMethod]
		public async Task CreateAsync_AddTicket_ReturnId()
		{
			var ticket = new Ticket() { EventId = 0, SectionId = 1, SeatId = 2, RowId = 3, UserId = 4, VenueId = 5, TicketId = 6 };

			var result = await _ticketRepository.CreateAsync(ticket);

			Assert.IsNotNull(result);
			Assert.AreEqual(6, result);
			_ticketDbSetMock.Verify(x => x.AddAsync(ticket, It.IsAny<CancellationToken>()), Times.Once);
			_dbContextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
		}

		[TestMethod]
		[ExpectedException(typeof(NullReferenceException))]
		public async Task CreateAsync_EntityIsNull_ThrowsException()
		{
			await _ticketRepository.CreateAsync(null);
		}

		[TestMethod]
		public async Task DeleteAsync_TicketFound_DeletesEntity()
		{
			var ticket = _tickets.First();
			_ticketDbSetMock.Setup(x => x.FindAsync(ticket.TicketId)).ReturnsAsync(ticket);

			await _ticketRepository.DeleteAsync(ticket.TicketId);

			_ticketDbSetMock.Verify(x => x.Remove(It.IsAny<Ticket>()), Times.Once);
			_dbContextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
		}

		[TestMethod]
		public async Task DeleteAsync_TicketNotExists_DoNothing()
		{
			var ticketId = 5;
			_ticketDbSetMock.Setup(x => x.FindAsync(ticketId)).ReturnsAsync((Ticket)null);

			await _ticketRepository.DeleteAsync(ticketId);

			_ticketDbSetMock.Verify(x => x.Remove(It.IsAny<Ticket>()), Times.Never);
			_dbContextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
		}

		[TestMethod]
		public async Task GetAsync_ReturnsTicketList()
		{
			var result = await _ticketRepository.GetAsync();

			Assert.IsNotNull(result);
			Assert.AreEqual(2, result.Count);
		}

		[TestMethod]
		public async Task GetAsync_NoTickets_ReturnsEmptyList()
		{
			var ticketDbMockSet = (new List<Ticket>()).BuildMockDbSet();
			var dbContextMock = new Mock<IAppDbContext>();
			dbContextMock.SetupGet(x => x.Tickets).Returns(ticketDbMockSet.Object);
			var repository = new TicketRepository(dbContextMock.Object);

			var result = await repository.GetAsync();

			Assert.IsNotNull(result);
			Assert.AreEqual(0, result.Count);
		}

		[TestMethod]
		public async Task GetByIdAsync_TicketExists_ReturnsTicket()
		{
			var ticketId = 0;
			_ticketDbSetMock.Setup(x => x.FindAsync(ticketId)).ReturnsAsync(_tickets.First());

			var result = await _ticketRepository.GetByIdAsync(ticketId);

			Assert.IsNotNull(result);
			Assert.AreEqual(ticketId, result.TicketId);
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public async Task GetByIdAsync_TicketNotFound_ThrowsException()
		{
			var ticketId = 5;

			await _ticketRepository.GetByIdAsync(ticketId);
		}

		[TestMethod]
		public async Task GetSortedAsync_SortByEventIdAscending_ReturnsSortedList()
		{
			var sortingInstructions = new SortingInstructions<Ticket>
			{
				OrderBy = c => c.EventId,
				Direction = OrderByDirection.Ascending
			};

			var result = await _ticketRepository.GetSortedAsync(sortingInstructions);

			Assert.AreEqual(2, result.Count);
		}

		[TestMethod]
		public async Task GetSortedAsync_SortByEventIdAscendingNoTickets_ReturnsEmptyList()
		{
			var ticketDbMockSet = (new List<Ticket>()).BuildMockDbSet();
			var dbContextMock = new Mock<IAppDbContext>();
			dbContextMock.SetupGet(x => x.Tickets).Returns(ticketDbMockSet.Object);
			var repository = new TicketRepository(dbContextMock.Object);

			var sortingInstructions = new SortingInstructions<Ticket>
			{
				OrderBy = c => c.EventId,
				Direction = OrderByDirection.Descending
			};

			var result = await repository.GetSortedAsync(sortingInstructions);

			Assert.AreEqual(0, result.Count);
		}

		[TestMethod]
		public async Task UpdateAsync_UpdatedModel_ReturnsUpdatedTicket()
		{
			var ticket = new Ticket() { TicketId = _tickets.First().TicketId };

			await _ticketRepository.UpdateAsync(ticket);

			_ticketDbSetMock.Verify(s => s.Update(ticket), Times.Once);
			_dbContextMock.Verify(s => s.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
		}

		private static List<Ticket> CreateTicketList()
		{
			return
			[
				new Ticket { EventId = 0, RowId = 0, SeatId = 0, SectionId = 0, TicketId = 0, VenueId = 0 },
				new Ticket { EventId = 0, RowId = 0, SeatId = 1, SectionId = 0, TicketId = 1, VenueId = 0 },
			];
		}
	}
}
