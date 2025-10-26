using Microsoft.AspNetCore.Mvc;
using Moq;
using TicketManager.Common.Interface;
using TicketManager.Common.Models;
using TicketManager.Controllers.Ticketing;

namespace TicketManager.Test.Controllers
{
	[TestClass]
	public class VenueControllerTests
	{
		private Mock<IRepository<Venue, int>> _venueRepositoryMock;

		private VenueController _venueController;

		public VenueControllerTests()
		{
			_venueRepositoryMock = new Mock<IRepository<Venue, int>>();

			_venueController = new VenueController(_venueRepositoryMock.Object);
		}

		[TestMethod]
		public async Task Get_VenueExists_Returns200()
		{
			var venue1 = new Venue()
			{
				Address = "FakeAddress1",
				Name = "FakeName1",
				VenueId = 1,
				Sections = new List<Section>()
			};
			var venue2 = new Venue()
			{
				Address = "FakeAddress2",
				Name = "FakeName2",
				VenueId = 2,
				Sections = new List<Section>()
			};

			_venueRepositoryMock.Setup(x => x.GetAsync()).ReturnsAsync(new List<Venue> { venue1, venue2 });

			var result = await _venueController.Get() as OkObjectResult;

			Assert.IsNotNull(result);
			Assert.AreEqual(200, result.StatusCode);
		}

		[TestMethod]
		public async Task Get_VenueDoesntExist_Returns500()
		{
			_venueRepositoryMock.Setup(x => x.GetAsync()).Throws(new Exception());

			var result = await _venueController.Get() as StatusCodeResult;

			Assert.IsNotNull(result);
			Assert.AreEqual(500, result.StatusCode);
		}

		[TestMethod]
		public async Task GetSections_SectionsExist_Returns200()
		{
			var venueId = 1;
			var sections = new List<Section>()
			{
				new Section() { Name = "3A", SectionId = 0, Rows = new List<Row>(), VenueId = venueId },
				new Section() { Name = "3B", SectionId = 1, Rows = new List<Row>(), VenueId = venueId },
				new Section() { Name = "3C", SectionId = 2, Rows = new List<Row>(), VenueId = venueId }
			};
			var venue = new Venue()
			{
				Address = "FakeAddress1",
				Name = "FakeName1",
				VenueId = venueId,
				Sections = sections
			};

			_venueRepositoryMock.Setup(x => x.GetByIdAsync(venueId)).ReturnsAsync(venue);

			var result = await _venueController.Get() as OkObjectResult;

			Assert.IsNotNull(result);
			Assert.AreEqual(200, result.StatusCode);
		}

		[TestMethod]
		public async Task GetSections_SectionsDontExist_Returns500()
		{
			var venueId = 1;
			_venueRepositoryMock.Setup(x => x.GetByIdAsync(venueId)).Throws(new Exception());

			var result = await _venueController.GetSections(venueId) as StatusCodeResult;

			Assert.IsNotNull(result);
			Assert.AreEqual(500, result.StatusCode);
		}
	}
}
