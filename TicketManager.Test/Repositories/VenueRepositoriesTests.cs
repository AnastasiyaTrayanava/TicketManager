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
	public class VenueRepositoriesTests
	{
		private Mock<IAppDbContext> _dbContextMock;
		private Mock<DbSet<Venue>> _venueDbSetMock;
		private VenueRepository _venueRepository;

		private List<Venue> _venues;

		[TestInitialize]
		public void Setup()
		{
			_venues = CreateVenueList();
			_venueDbSetMock = _venues.BuildMockDbSet();
			_dbContextMock = new Mock<IAppDbContext>();

			_dbContextMock.SetupGet(x => x.Venues).Returns(_venueDbSetMock.Object);

			_venueRepository = new VenueRepository(_dbContextMock.Object);
		}

		[TestMethod]
		public async Task CreateAsync_AddVenue_ReturnId()
		{
			var venue = new Venue() { VenueId = 3, Address = "FakeAddress", Name = "FakeName", Sections = new List<Section>() };

			var result = await _venueRepository.CreateAsync(venue);

			Assert.IsNotNull(result);
			Assert.AreEqual(3, result);
			_venueDbSetMock.Verify(x => x.AddAsync(venue, It.IsAny<CancellationToken>()), Times.Once);
			_dbContextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
		}

		[TestMethod]
		[ExpectedException(typeof(NullReferenceException))]
		public async Task CreateAsync_EntityIsNull_ThrowsException()
		{
			await _venueRepository.CreateAsync(null);
		}

		[TestMethod]
		public async Task DeleteAsync_VenueFound_DeletesEntity()
		{
			var venue = _venues.First();
			_venueDbSetMock.Setup(x => x.FindAsync(venue.VenueId)).ReturnsAsync(venue);

			await _venueRepository.DeleteAsync(venue.VenueId);

			_venueDbSetMock.Verify(x => x.Remove(It.IsAny<Venue>()), Times.Once);
			_dbContextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
		}

		[TestMethod]
		public async Task DeleteAsync_VenueNotExists_DoNothing()
		{
			var venueId = 5;
			_venueDbSetMock.Setup(x => x.FindAsync(venueId)).ReturnsAsync((Venue)null);

			await _venueRepository.DeleteAsync(venueId);

			_venueDbSetMock.Verify(x => x.Remove(It.IsAny<Venue>()), Times.Never);
			_dbContextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
		}

		[TestMethod]
		public async Task GetAsync_ReturnsVenueList()
		{
			var result = await _venueRepository.GetAsync();

			Assert.IsNotNull(result);
			Assert.AreEqual(3, result.Count);
		}

		[TestMethod]
		public async Task GetAsync_NoVenues_ReturnsEmptyList()
		{
			var venueDbMockSet = (new List<Venue>()).BuildMockDbSet();
			var dbContextMock = new Mock<IAppDbContext>();
			dbContextMock.SetupGet(x => x.Venues).Returns(venueDbMockSet.Object);
			var repository = new VenueRepository(dbContextMock.Object);

			var result = await repository.GetAsync();

			Assert.IsNotNull(result);
			Assert.AreEqual(0, result.Count);
		}

		[TestMethod]
		public async Task GetByIdAsync_VenueExists_ReturnsVenue()
		{
			var venueId = 0;
			_venueDbSetMock.Setup(x => x.FindAsync(venueId)).ReturnsAsync(_venues.First());

			var result = await _venueRepository.GetByIdAsync(venueId);

			Assert.IsNotNull(result);
			Assert.AreEqual(venueId, result.VenueId);
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public async Task GetByIdAsync_VenueNotFound_ThrowsException()
		{
			var venueId = 5;

			await _venueRepository.GetByIdAsync(venueId);
		}

		[TestMethod]
		public async Task GetSortedAsync_SortByVenueStatusAscending_ReturnsSortedList()
		{
			var sortingInstructions = new SortingInstructions<Venue>
			{
				OrderBy = c => c.Name,
				Direction = OrderByDirection.Ascending
			};

			var result = await _venueRepository.GetSortedAsync(sortingInstructions);

			Assert.AreEqual(3, result.Count);
		}

		[TestMethod]
		public async Task GetSortedAsync_SortByVenueStatusAscendingNoVenues_ReturnsEmptyList()
		{
			var venueDbMockSet = (new List<Venue>()).BuildMockDbSet();
			var dbContextMock = new Mock<IAppDbContext>();
			dbContextMock.SetupGet(x => x.Venues).Returns(venueDbMockSet.Object);
			var repository = new VenueRepository(dbContextMock.Object);

			var sortingInstructions = new SortingInstructions<Venue>
			{
				OrderBy = c => c.Name,
				Direction = OrderByDirection.Descending
			};

			var result = await repository.GetSortedAsync(sortingInstructions);

			Assert.AreEqual(0, result.Count);
		}

		[TestMethod]
		public async Task UpdateAsync_UpdatedModel_ReturnsUpdatedVenue()
		{
			var venue = new Venue() { VenueId = _venues.First().VenueId };

			await _venueRepository.UpdateAsync(venue);

			_venueDbSetMock.Verify(s => s.Update(venue), Times.Once);
			_dbContextMock.Verify(s => s.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
		}

		private static List<Venue> CreateVenueList()
		{
			return
			[
				new Venue() {VenueId = 0, Address = "Address 1", Name = "Name 1", Sections = new List<Section>()},
				new Venue() {VenueId = 1, Address = "Address 2", Name = "Name 2", Sections = new List<Section>()},
				new Venue() {VenueId = 2, Address = "Address 3", Name = "Name 3", Sections = new List<Section>()},
			];
		}
	}
}
