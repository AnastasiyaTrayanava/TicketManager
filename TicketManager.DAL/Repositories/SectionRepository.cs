using Microsoft.EntityFrameworkCore;
using TicketManager.Common.Interface;
using TicketManager.Common.Models;

namespace TicketManager.DAL.Repositories
{
	public class SectionRepository : IRepository<Section>
	{
		private AppDbContext _context;

		public SectionRepository(AppDbContext context)
		{
			_context = context;
		}

		public async Task CreateAsync(Section entity)
		{
			await _context.Sections.AddAsync(entity);
			await _context.SaveChangesAsync();
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
			var sectionToFind = await _context.Sections.FindAsync(id);

			if (sectionToFind == null)
			{
				throw new ArgumentException($"Entry by id {id} was not found.");
			}

			return sectionToFind;
		}

		public async Task UpdateAsync(Section entity)
		{
			_context.Sections.Update(entity);
			await _context.SaveChangesAsync();
		}
	}
}
