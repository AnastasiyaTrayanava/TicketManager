using Microsoft.AspNetCore.Mvc;
using TicketManager.Common.Interface;
using TicketManager.Common.Models.Entities;

namespace TicketManager.Controllers.Ticketing
{
	[Route("venues")]
	public class VenueController : Controller
	{
		private IRepository<Venue, int> _venueRepository;

		public VenueController(IRepository<Venue, int> venueRepository)
		{
			_venueRepository = venueRepository;
		}

		[HttpGet]
		public async Task<IActionResult> Get()
		{
			try
			{
				return Ok(await _venueRepository.GetAsync());
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
				return StatusCode(500);
			}
		}

		[HttpGet]
		[Route("{venueId}/sections")]
		public async Task<IActionResult> GetSections(int venueId)
		{
			try
			{
				var venues = await _venueRepository.GetByIdAsync(venueId);
				return Ok(venues.Sections);
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
				return StatusCode(500);
			}
		}
	}
}
