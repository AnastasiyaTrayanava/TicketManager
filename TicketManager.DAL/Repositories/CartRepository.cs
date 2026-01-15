using Microsoft.EntityFrameworkCore;
using TicketManager.Common.Enums;
using TicketManager.Common.Interface;
using TicketManager.Common.Models;
using TicketManager.Common.Models.Entities;

namespace TicketManager.DAL.Repositories
{
	public class CartRepository : IRepository<Cart, Guid>
	{
		private IAppDbContext _context;

		public CartRepository(IAppDbContext context)
		{
			_context = context;
		}

		public async Task<Guid> CreateAsync(Cart entity)
		{
			entity.CartId = Guid.NewGuid();
			await _context.Carts.AddAsync(entity);
			await _context.SaveChangesAsync();
			return entity.CartId;
		}

		public async Task DeleteAsync(Guid id)
		{
			var cartToDelete = await _context.Carts.FindAsync(id);

			if (cartToDelete != null)
			{
				_context.Carts.Remove(cartToDelete);
				await _context.SaveChangesAsync();
			}
		}

		public async Task<IList<Cart>> GetAsync()
		{
			return await _context.Carts.ToListAsync();
		}

		public async Task<Cart> GetByIdAsync(Guid id)
		{
			var cartToFind = await _context.Carts
				.Include(x => x.User)
				.Include(x => x.Items)
				.ThenInclude(x => x.Seat)
				.ThenInclude(x => x.Row)
				.FirstOrDefaultAsync(x => x.CartId == id);

			if (cartToFind == null)
			{
				throw new ArgumentException($"Entry by id {id} was not found.");
			}

			return cartToFind;
		}

		public async Task<IList<Cart>> GetSortedAsync(SortingInstructions<Cart> sortingInstructions)
		{
			return sortingInstructions.Direction == OrderByDirection.Ascending
				? await _context.Carts.OrderBy(sortingInstructions.OrderBy).ToListAsync()
				: await _context.Carts.OrderByDescending(sortingInstructions.OrderBy).ToListAsync();
		}

		public async Task UpdateAsync(Cart entity)
		{
			_context.Carts.Update(entity);
			await _context.SaveChangesAsync();
		}
	}
}
