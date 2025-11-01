using Microsoft.EntityFrameworkCore;
using TicketManager.Common.Enums;
using TicketManager.Common.Interface;
using TicketManager.Common.Models;
using TicketManager.Common.Models.Entities;

namespace TicketManager.DAL.Repositories
{
	public class EventRepository : IRepository<Event, int>
	{
		private IAppDbContext _context;

		public EventRepository(IAppDbContext context)
		{
			_context = context;
		}

		public async Task<int> CreateAsync(Event entity)
		{
			await _context.Events.AddAsync(entity);
			await _context.SaveChangesAsync();
			return entity.EventId;
		}

		public async Task DeleteAsync(int id)
		{
			var eventToDelete = await _context.Events.FindAsync(id);

			if (eventToDelete != null)
			{
				_context.Events.Remove(eventToDelete);
				await _context.SaveChangesAsync();
			}
		}

		public async Task<IList<Event>> GetAsync()
		{
			return await _context.Events.ToListAsync();
		}

		public async Task<Event> GetByIdAsync(int id)
		{
			var eventToFind = await _context.Events
				.Include(e => e.Venue)
				.ThenInclude(v => v.Sections)
				.ThenInclude(s => s.Rows)
				.ThenInclude(r => r.Seats)
				.ThenInclude(seat => seat.Price)
				.FirstOrDefaultAsync(x => x.EventId == id);

			if (eventToFind == null)
			{
				throw new ArgumentException($"Entry by id {id} was not found.");
			}

			return eventToFind;
		}

		public async Task<IList<Event>> GetSortedAsync(SortingInstructions<Event> sortingInstructions)
		{
			return sortingInstructions.Direction == OrderByDirection.Ascending
				? await _context.Events.OrderBy(sortingInstructions.OrderBy).ToListAsync()
				: await _context.Events.OrderByDescending(sortingInstructions.OrderBy).ToListAsync();
		}

		public async Task UpdateAsync(Event entity)
		{
			_context.Events.Update(entity);
			await _context.SaveChangesAsync();
		}
	}
}
