using Microsoft.EntityFrameworkCore;
using TicketManager.Common.Enums;
using TicketManager.Common.Interface;
using TicketManager.Common.Models;
using TicketManager.Common.Models.Entities;

namespace TicketManager.DAL.Repositories
{
	public class SectionRepository : IRepository<Section, int>
	{
		private IAppDbContext _context;

		public SectionRepository(IAppDbContext context)
		{
			_context = context;
		}

		public async Task<int> CreateAsync(Section entity)
		{
			await _context.Sections.AddAsync(entity);
			await _context.SaveChangesAsync();
			return entity.SectionId;
		}

		public async Task DeleteAsync(int id)
		{
			var sectionToDelete = await _context.Sections.FindAsync(id);

			if (sectionToDelete != null)
			{
				_context.Sections.Remove(sectionToDelete);
				await _context.SaveChangesAsync();
			}
		}

		public async Task<IList<Section>> GetAsync()
		{
			return await _context.Sections.ToListAsync();
		}

		public async Task<Section> GetByIdAsync(int id)
		{
			//var sectionToFind = await _context.Sections.FindAsync(id);
			var sectionToFind = await _context.Sections
				.Include(s => s.Rows)
				.ThenInclude(r => r.Seats)
				.ThenInclude(seat => seat.Price)
				.FirstOrDefaultAsync(x => x.SectionId == id);

			if (sectionToFind == null)
			{
				throw new ArgumentException($"Entry by id {id} was not found.");
			}

			return sectionToFind;
		}

		public async Task<IList<Section>> GetSortedAsync(SortingInstructions<Section> sortingInstructions)
		{
			return sortingInstructions.Direction == OrderByDirection.Ascending
				? await _context.Sections.OrderBy(sortingInstructions.OrderBy).ToListAsync()
				: await _context.Sections.OrderByDescending(sortingInstructions.OrderBy).ToListAsync();
		}

		public async Task UpdateAsync(Section entity)
		{
			_context.Sections.Update(entity);
			await _context.SaveChangesAsync();
		}
	}
}
