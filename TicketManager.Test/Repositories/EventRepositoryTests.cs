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
	public class EventRepositoryTests
	{
		private Mock<IAppDbContext> _dbContextMock;
		private Mock<DbSet<Event>> _eventDbSetMock;
		private EventRepository _eventRepository;

		private List<Event> _events;

		[TestInitialize]
		public void Setup()
		{
			_events = CreateEventList();
			_eventDbSetMock = _events.BuildMockDbSet();
			_dbContextMock = new Mock<IAppDbContext>();

			_dbContextMock.SetupGet(x => x.Events).Returns(_eventDbSetMock.Object);

			_eventRepository = new EventRepository(_dbContextMock.Object);
		}

		[TestMethod]
		public async Task CreateAsync_AddEvent_ReturnId()
		{
			var evnt = new Event() {DateTime = DateTime.Now, Description = "Test", Name = "Test", VenueId = 0, EventId = 2};

			var result = await _eventRepository.CreateAsync(evnt);

			Assert.IsNotNull(result);
			Assert.AreEqual(2, result);
			_eventDbSetMock.Verify(x => x.AddAsync(evnt, It.IsAny<CancellationToken>()), Times.Once);
			_dbContextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
		}

		[TestMethod]
		[ExpectedException(typeof(NullReferenceException))]
		public async Task CreateAsync_EntityIsNull_ThrowsException()
		{
			await _eventRepository.CreateAsync(null);
		}

		[TestMethod]
		public async Task DeleteAsync_EventFound_DeletesEntity()
		{
			var evnt = _events.First();
			_eventDbSetMock.Setup(x => x.FindAsync(evnt.EventId)).ReturnsAsync(evnt);

			await _eventRepository.DeleteAsync(evnt.EventId);

			_eventDbSetMock.Verify(x => x.Remove(It.IsAny<Event>()), Times.Once);
			_dbContextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
		}

		[TestMethod]
		public async Task DeleteAsync_EventNotExists_DoNothing()
		{
			var evntId = 5;
			_eventDbSetMock.Setup(x => x.FindAsync(evntId)).ReturnsAsync((Event)null);

			await _eventRepository.DeleteAsync(evntId);

			_eventDbSetMock.Verify(x => x.Remove(It.IsAny<Event>()), Times.Never);
			_dbContextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
		}

		[TestMethod]
		public async Task GetAsync_ReturnsEventsList()
		{
			var result = await _eventRepository.GetAsync();

			Assert.IsNotNull(result);
			Assert.AreEqual(2, result.Count);
		}

		[TestMethod]
		public async Task GetAsync_NoEvents_ReturnsEmptyList()
		{
			var evntDbMockSet = (new List<Event>()).BuildMockDbSet();
			var dbContextMock = new Mock<IAppDbContext>();
			dbContextMock.SetupGet(x => x.Events).Returns(evntDbMockSet.Object);
			var repository = new EventRepository(dbContextMock.Object);

			var result = await repository.GetAsync();

			Assert.IsNotNull(result);
			Assert.AreEqual(0, result.Count);
		}

		[TestMethod]
		public async Task GetByIdAsync_EventExists_ReturnsCart()
		{
			var evnt = _events.First();
			_eventDbSetMock.Setup(x => x.FindAsync(evnt.EventId)).ReturnsAsync(evnt);

			var result = await _eventRepository.GetByIdAsync(evnt.EventId);

			Assert.IsNotNull(result);
			Assert.AreEqual(evnt.EventId, result.EventId);
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public async Task GetByIdAsync_EventNotFound_ThrowsException()
		{
			var evntId = 5;

			await _eventRepository.GetByIdAsync(evntId);
		}

		[TestMethod]
		public async Task GetSortedAsync_SortByEventIdAscending_ReturnsSortedList()
		{
			var sortingInstructions = new SortingInstructions<Event>
			{
				OrderBy = c => c.EventId,
				Direction = OrderByDirection.Ascending
			};

			var result = await _eventRepository.GetSortedAsync(sortingInstructions);

			Assert.AreEqual(2, result.Count);
		}

		[TestMethod]
		public async Task GetSortedAsync_SortByEventIdAscendingNoEvents_ReturnsEmptyList()
		{
			var eventDbMockSet = (new List<Event>()).BuildMockDbSet();
			var dbContextMock = new Mock<IAppDbContext>();
			dbContextMock.SetupGet(x => x.Events).Returns(eventDbMockSet.Object);
			var repository = new EventRepository(dbContextMock.Object);

			var sortingInstructions = new SortingInstructions<Event>
			{
				OrderBy = c => c.EventId,
				Direction = OrderByDirection.Descending
			};

			var result = await repository.GetSortedAsync(sortingInstructions);

			Assert.AreEqual(0, result.Count);
		}

		[TestMethod]
		public async Task UpdateAsync_UpdatedModel_ReturnsUpdatedCart()
		{
			var evnt = new Event() { EventId = _events.First().EventId };

			await _eventRepository.UpdateAsync(evnt);

			_eventDbSetMock.Verify(s => s.Update(evnt), Times.Once);
			_dbContextMock.Verify(s => s.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
		}

		private static List<Event> CreateEventList()
		{
			return
			[
				new Event()
				{
					DateTime = DateTime.Now.AddDays(1),
					Description = "Test Description 1",
					EventId = 0,
					Name = "Test Event 1",
					VenueId = 0
				},
				new Event()
				{
					DateTime = DateTime.Now.AddDays(2),
					Description = "Test Description 2",
					EventId = 1,
					Name = "Test Event 2",
					VenueId = 1
				},
			];
		}
	}
}
