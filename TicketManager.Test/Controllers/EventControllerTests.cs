using Microsoft.AspNetCore.Mvc;
using Moq;
using TicketManager.Common.Enums;
using TicketManager.Common.Interface;
using TicketManager.Common.Models;
using TicketManager.Common.Models.ViewModels;
using TicketManager.Controllers.Ticketing;

namespace TicketManager.Test.Controllers
{
	[TestClass]
	public sealed class EventControllerTests
	{
		private Mock<IRepository<Event, int>> _eventRepositoryMock;
		private Mock<IRepository<Section, int>> _sectionRepositoryMock;

		private EventController _eventController;

		public EventControllerTests()
		{
			_eventRepositoryMock = new Mock<IRepository<Event, int>>();
			_sectionRepositoryMock = new Mock<IRepository<Section, int>>();

			_eventController = new EventController(_eventRepositoryMock.Object, _sectionRepositoryMock.Object);
		}

		[TestMethod]
		public async Task Get_EventsExist_Returns200()
		{
			var mockedEventList = new List<Event> { new() { DateTime = DateTime.Now, Description = "Test", EventId = 0, Name = "Test0", VenueId = 0 } };

			_eventRepositoryMock.Setup(x => x.GetAsync()).ReturnsAsync(mockedEventList);

			var result = await _eventController.Get() as OkObjectResult;

			Assert.IsNotNull(result);
			Assert.AreEqual(200, result.StatusCode);
		}

		[TestMethod]
		public async Task Get_EventsDontExist_Returns500()
		{
			_eventRepositoryMock.Setup(x => x.GetAsync()).ThrowsAsync(new Exception("No data"));

			var result = await _eventController.Get() as StatusCodeResult;

			Assert.IsNotNull(result);
			Assert.AreEqual(500, result.StatusCode);
		}

		[TestMethod]
		public async Task GetEventSectionSeats_SectionExists_Returns200()
		{
			int eventId = 1, sectionId = 2;
			var mockedSection = new Section
			{
				Rows =
				[
					new Row
					{
						RowId = 10,
						Seats =
						[
							new Seat
							{
								SeatId = 100,
								SeatState = SeatState.Available,
								Price =
								[
									new Price { PriceId = 0, PriceTier = PriceTier.Vip, PriceValue = 123.45f },
									new Price { PriceId = 1, PriceTier = PriceTier.Adult, PriceValue = 90.50f }
								]
							},
							new Seat
							{
								SeatId = 101,
								SeatState = SeatState.Reserved,
								Price =
								[
									new Price { PriceId = 0, PriceTier = PriceTier.Vip, PriceValue = 123.45f },
									new Price { PriceId = 1, PriceTier = PriceTier.Adult, PriceValue = 90.50f }
								]
							}
						]
					}
				]
			};

			_sectionRepositoryMock.Setup(x => x.GetByIdAsync(sectionId)).ReturnsAsync(mockedSection);

			var result = await _eventController.GetEventSectionSeats(eventId, sectionId);

			var okResult = result as OkObjectResult;
			Assert.IsNotNull(okResult);
			Assert.AreEqual(200, okResult.StatusCode);
			var viewModel = okResult.Value as EventSectionViewModel;
			Assert.IsNotNull(viewModel);
			Assert.AreEqual(2, viewModel.Rows.First().Seats.Count);
		}

		[TestMethod]
		public async Task GetEventSectionSeats_SectionDoesntExits_Returns500()
		{
			int eventId = 1, sectionId = 2;
			_sectionRepositoryMock.Setup(x => x.GetByIdAsync(sectionId)).ThrowsAsync(new Exception("Database error"));

			var result = await _eventController.GetEventSectionSeats(eventId, sectionId) as StatusCodeResult;

			Assert.IsNotNull(result);
			Assert.AreEqual(500, result.StatusCode);
		}
	}
}
