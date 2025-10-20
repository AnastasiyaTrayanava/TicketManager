using Microsoft.AspNetCore.Mvc;
using TicketManager.Common.Interface;
using TicketManager.Common.Models;
using TicketManager.Common.Models.ViewModels;

namespace TicketManager.Controllers.Ticketing
{
	[Route("events")]
	public class EventController : Controller
	{
		private IRepository<Event, int> _eventRepository;
		private IRepository<Section, int> _sectionRepository;

		public EventController(IRepository<Event, int> eventRepository, IRepository<Section, int> sectionRepository)
		{
			_eventRepository = eventRepository;
			_sectionRepository = sectionRepository;
		}

		[HttpGet]
		public async Task<IActionResult> Get()
		{
			try
			{
				return Ok(await _eventRepository.GetAsync());
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
				return StatusCode(500);
			}
		}

		[HttpGet]
		[Route("{eventId}/sections/{sectionId}/seats")]
		public async Task<IActionResult> GetEventSectionSeats(int eventId, int sectionId)
		{
			try
			{
				var section = await _sectionRepository.GetByIdAsync(sectionId);
				var viewModel = new EventSectionViewModel()
				{
					Rows = section.Rows.Select(x =>
						new EventSectionRowViewModel()
						{
							Seats = x.Seats.Select(y =>
								new EventSectionSeatViewModel()
								{
									Prices = y.Price.Select(z => new EventSectionPriceViewModel()
									{
										PriceId = z.PriceId,
										PriceTier = z.PriceTier,
										PriceValue = z.PriceValue
									}).ToList(),
									SeatId = y.SeatId,
									SeatState = y.SeatState
								}).ToList(),
							RowId = x.RowId
						}).ToList()
				};

				return Ok(viewModel);
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
				return StatusCode(500);
			}
		}
	}
}
