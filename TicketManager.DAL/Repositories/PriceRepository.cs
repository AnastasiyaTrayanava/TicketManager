using Microsoft.EntityFrameworkCore;
using TicketManager.Common.Enums;
using TicketManager.Common.Interface;
using TicketManager.Common.Models;

namespace TicketManager.DAL.Repositories
{
	public class PriceRepository : IRepository<Price, int>
	{
		private IAppDbContext _context;

		public PriceRepository(IAppDbContext context)
		{
			_context = context;
		}

		public async Task<int> CreateAsync(Price entity)
		{
			await _context.Prices.AddAsync(entity);
			await _context.SaveChangesAsync();
			return entity.PriceId;
		}

		public async Task DeleteAsync(int id)
		{
			var priceToDelete = await _context.Prices.FindAsync(id);

			if (priceToDelete != null)
			{
				_context.Prices.Remove(priceToDelete);
				await _context.SaveChangesAsync();
			}
		}

		public async Task<IList<Price>> GetAsync()
		{
			return await _context.Prices.ToListAsync();
		}

		public async Task<Price> GetByIdAsync(int id)
		{
			var priceToFind = await _context.Prices.FindAsync(id);

			if (priceToFind == null)
			{
				throw new ArgumentException($"Entry by id {id} was not found.");
			}

			return priceToFind;
		}

		public async Task<IList<Price>> GetSortedAsync(SortingInstructions<Price> sortingInstructions)
		{
			return sortingInstructions.Direction == OrderByDirection.Ascending
				? await _context.Prices.OrderBy(sortingInstructions.OrderBy).ToListAsync()
				: await _context.Prices.OrderByDescending(sortingInstructions.OrderBy).ToListAsync();
		}

		public async Task UpdateAsync(Price entity)
		{
			_context.Prices.Update(entity);
			await _context.SaveChangesAsync();
		}
	}
}
