using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TicketManager.Common.Interface;
using TicketManager.Common.Models;

namespace TicketManager.DAL.Repositories
{
	public class TicketRepository : IRepository<Ticket>
	{
		private AppDbContext _context;

		public TicketRepository(AppDbContext context)
		{
			_context = context;
		}

		public async Task CreateAsync(Ticket entity)
		{
			await _context.Tickets.AddAsync(entity);
			await _context.SaveChangesAsync();
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

		public async Task UpdateAsync(Ticket entity)
		{
			_context.Tickets.Update(entity);
			await _context.SaveChangesAsync();
		}
	}
}
