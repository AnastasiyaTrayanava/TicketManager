using Microsoft.EntityFrameworkCore;
using TicketManager.Common.Enums;
using TicketManager.Common.Interface;
using TicketManager.Common.Models;

namespace TicketManager.DAL.Repositories
{
	public class UserRepository : IRepository<User, int>
	{
		private IAppDbContext _context;

		public UserRepository(IAppDbContext context)
		{
			_context = context;
		}

		public async Task<int> CreateAsync(User entity)
		{
			await _context.Users.AddAsync(entity);
			await _context.SaveChangesAsync();
			return entity.UserId;
		}

		public async Task DeleteAsync(int id)
		{
			var userToDelete = await _context.Users.FindAsync(id);

			if (userToDelete != null)
			{
				_context.Users.Remove(userToDelete);
				await _context.SaveChangesAsync();
			}
		}

		public async Task<IList<User>> GetAsync()
		{
			return await _context.Users.ToListAsync();
		}

		public async Task<User> GetByIdAsync(int id)
		{
			var userToFind = await _context.Users.FindAsync(id);

			if (userToFind == null)
			{
				throw new ArgumentException($"Entry by id {id} was not found.");
			}

			return userToFind;
		}

		public async Task<IList<User>> GetSortedAsync(SortingInstructions<User> sortingInstructions)
		{
			return sortingInstructions.Direction == OrderByDirection.Ascending
				? await _context.Users.OrderBy(sortingInstructions.OrderBy).ToListAsync()
				: await _context.Users.OrderByDescending(sortingInstructions.OrderBy).ToListAsync();
		}

		public async Task UpdateAsync(User entity)
		{
			_context.Users.Update(entity);
			await _context.SaveChangesAsync();
		}
	}
}
