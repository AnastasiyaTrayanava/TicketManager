using Microsoft.EntityFrameworkCore;
using TicketManager.Common.Enums;
using TicketManager.Common.Interface;
using TicketManager.Common.Models;

namespace TicketManager.DAL.Repositories
{
	public class TicketRepository : IRepository<Ticket, int>
	{
		private AppDbContext _context;

		public TicketRepository(AppDbContext context)
		{
			_context = context;
		}

		public async Task<int> CreateAsync(Ticket entity)
		{
			await _context.Tickets.AddAsync(entity);
			await _context.SaveChangesAsync();
			return entity.TicketId;
		}

		public async Task DeleteAsync(int id)
		{
			var ticketToDelete = await _context.Tickets.FindAsync(id);

			if (ticketToDelete != null)
			{
				_context.Tickets.Remove(ticketToDelete);
				await _context.SaveChangesAsync();
			}
		}

		public async Task<IList<Ticket>> GetAsync()
		{
			return await _context.Tickets.ToListAsync();
		}

		public async Task<Ticket> GetByIdAsync(int id)
		{
			var ticketToFind = await _context.Tickets.FindAsync(id);

			if (ticketToFind == null)
			{
				throw new ArgumentException($"Entry by id {id} was not found.");
			}

			return ticketToFind;
		}

		public async Task<IList<Ticket>> GetSortedAsync(SortingInstructions<Ticket> sortingInstructions)
		{
			return sortingInstructions.Direction == OrderByDirection.Ascending
				? await _context.Tickets.OrderBy(sortingInstructions.OrderBy).ToListAsync()
				: await _context.Tickets.OrderByDescending(sortingInstructions.OrderBy).ToListAsync();
		}

		public async Task UpdateAsync(Ticket entity)
		{
			_context.Tickets.Update(entity);
			await _context.SaveChangesAsync();
		}
	}
}
