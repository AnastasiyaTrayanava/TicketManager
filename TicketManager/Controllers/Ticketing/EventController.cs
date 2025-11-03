using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using TicketManager.Common.Extensions;
using TicketManager.Common.Interface;
using TicketManager.Common.Models.Entities;
using TicketManager.Common.Models.ViewModels;

namespace TicketManager.Controllers.Ticketing
{
	[Route("events")]
	public class EventController : Controller
	{
		private IRepository<Event, int> _eventRepository;
		private IRepository<Section, int> _sectionRepository;
		private IMemoryCache _memoryCache;

		public EventController(IRepository<Event, int> eventRepository, IRepository<Section, int> sectionRepository, IMemoryCache memoryCache)
		{
			_eventRepository = eventRepository;
			_sectionRepository = sectionRepository;
			_memoryCache = memoryCache;
		}

		[HttpGet]
		[ResponseCache(Duration = 3600, Location = ResponseCacheLocation.Any, NoStore = false)]
		public async Task<IActionResult> Get()
		{
			try
			{
				var cacheKey = "events";

				if (_memoryCache.TryGetValue<IList<Event>>(cacheKey, out var values))
				{
					return Ok(values);
				}

				var events = await _eventRepository.GetAsync();
				SetMemoryCacheEntry(cacheKey, events);

				return Ok(events);
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
				return StatusCode(500);
			}
		}

		[HttpGet]
		[Route("{eventId}/sections/{sectionId}/seats")]
		[ResponseCache(Duration = 3600, Location = ResponseCacheLocation.Any, NoStore = false)]
		public async Task<IActionResult> GetEventSectionSeats(int eventId, int sectionId)
		{
			try
			{
				var cacheKey = $"{eventId}:{sectionId}:seats";

				if (_memoryCache.TryGetValue<EventSectionViewModel>(cacheKey, out var values))
				{
					return Ok(values);
				}

				var section = await _sectionRepository.GetByIdAsync(sectionId);
				var viewModel = section.ToViewModel();

				SetMemoryCacheEntry(cacheKey, viewModel);

				return Ok(viewModel);
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
				return StatusCode(500);
			}
		}

		private void SetMemoryCacheEntry(string cacheKey, object data)
		{
			_memoryCache.Set(cacheKey, data, new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromDays(1)));
		}
	}
}
