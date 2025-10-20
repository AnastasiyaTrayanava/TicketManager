using Microsoft.EntityFrameworkCore;
using TicketManager.Common.Enums;
using TicketManager.Common.Interface;
using TicketManager.Common.Models;

namespace TicketManager.DAL.Repositories
{
	public class SeatRepository : IRepository<Seat, int>
	{
		private AppDbContext _context;

		public SeatRepository(AppDbContext context)
		{
			_context = context;
		}

		public async Task<int> CreateAsync(Seat entity)
		{
			await _context.Seats.AddAsync(entity);
			await _context.SaveChangesAsync();
			return entity.SeatId;
		}

		public async Task DeleteAsync(int id)
		{
			var seatToDelete = await _context.Seats.FindAsync(id);

			if (seatToDelete != null)
			{
				_context.Seats.Remove(seatToDelete);
				await _context.SaveChangesAsync();
			}
		}

		public async Task<IList<Seat>> GetAsync()
		{
			return await _context.Seats.ToListAsync();
		}

		public async Task<Seat> GetByIdAsync(int id)
		{
			var seatToFind = await _context.Seats.FindAsync(id);

			if (seatToFind == null)
			{
				throw new ArgumentException($"Entry by id {id} was not found.");
			}

			return seatToFind;
		}

		public async Task<IList<Seat>> GetSortedAsync(SortingInstructions<Seat> sortingInstructions)
		{
			return sortingInstructions.Direction == OrderByDirection.Ascending
				? await _context.Seats.OrderBy(sortingInstructions.OrderBy).ToListAsync()
				: await _context.Seats.OrderByDescending(sortingInstructions.OrderBy).ToListAsync();
		}

		public async Task UpdateAsync(Seat entity)
		{
			_context.Seats.Update(entity);
			await _context.SaveChangesAsync();
		}
	}
}
