using Microsoft.EntityFrameworkCore;
using TicketManager.Common.Enums;
using TicketManager.Common.Interface;
using TicketManager.Common.Models;
using TicketManager.Common.Models.Entities;

namespace TicketManager.DAL.Repositories
{
	public class RowRepository : IRepository<Row, int>
	{
		private IAppDbContext _context;

		public RowRepository(IAppDbContext context)
		{
			_context = context;
		}

		public async Task<int> CreateAsync(Row entity)
		{
			await _context.Rows.AddAsync(entity);
			await _context.SaveChangesAsync();
			return entity.RowId;
		}

		public async Task DeleteAsync(int id)
		{
			var rowToDelete = await _context.Rows.FindAsync(id);

			if (rowToDelete != null)
			{
				_context.Rows.Remove(rowToDelete);
				await _context.SaveChangesAsync();
			}
		}

		public async Task<IList<Row>> GetAsync()
		{
			return await _context.Rows.ToListAsync();
		}

		public async Task<Row> GetByIdAsync(int id)
		{
			var rowToFind = await _context.Rows.FindAsync(id);

			if (rowToFind == null)
			{
				throw new ArgumentException($"Entry by id {id} was not found.");
			}

			return rowToFind;
		}

		public async Task<IList<Row>> GetSortedAsync(SortingInstructions<Row> sortingInstructions)
		{
			return sortingInstructions.Direction == OrderByDirection.Ascending
				? await _context.Rows.OrderBy(sortingInstructions.OrderBy).ToListAsync()
				: await _context.Rows.OrderByDescending(sortingInstructions.OrderBy).ToListAsync();
		}

		public async Task UpdateAsync(Row entity)
		{
			_context.Rows.Update(entity);
			await _context.SaveChangesAsync();
		}
	}
}
