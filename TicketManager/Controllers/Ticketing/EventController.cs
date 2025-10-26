using Microsoft.AspNetCore.Mvc;
using TicketManager.Common.Extensions;
using TicketManager.Common.Interface;
using TicketManager.Common.Models;

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
				var viewModel = section.ToViewModel();

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
