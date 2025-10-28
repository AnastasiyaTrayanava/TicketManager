using Microsoft.EntityFrameworkCore;
using TicketManager.Common.Enums;
using TicketManager.Common.Interface;
using TicketManager.Common.Models;

namespace TicketManager.DAL.Repositories
{
	public class VenueRepository : IRepository<Venue, int>
	{
		private IAppDbContext _context;

		public VenueRepository(IAppDbContext context)
		{
			_context = context;
		}

		public async Task<int> CreateAsync(Venue entity)
		{
			await _context.Venues.AddAsync(entity);
			await _context.SaveChangesAsync();
			return entity.VenueId;
		}

		public async Task DeleteAsync(int id)
		{
			var venueToDelete = await _context.Venues.FindAsync(id);

			if (venueToDelete != null)
			{
				_context.Venues.Remove(venueToDelete);
				await _context.SaveChangesAsync();
			}
		}

		public async Task<IList<Venue>> GetAsync()
		{
			return await _context.Venues.ToListAsync();
		}

		public async Task<Venue> GetByIdAsync(int id)
		{
			var venueToFind = await _context.Venues.Include(v => v.Sections).FirstOrDefaultAsync(x => x.VenueId == id);

			if (venueToFind == null)
			{
				throw new ArgumentException($"Entry by id {id} was not found.");
			}

			return venueToFind;
		}

		public async Task<IList<Venue>> GetSortedAsync(SortingInstructions<Venue> sortingInstructions)
		{
			return sortingInstructions.Direction == OrderByDirection.Ascending
				? await _context.Venues.OrderBy(sortingInstructions.OrderBy).ToListAsync()
				: await _context.Venues.OrderByDescending(sortingInstructions.OrderBy).ToListAsync();
		}

		public async Task UpdateAsync(Venue entity)
		{
			_context.Venues.Update(entity);
			await _context.SaveChangesAsync();
		}
	}
}
