using Microsoft.EntityFrameworkCore;
using TicketManager.Common.Enums;
using TicketManager.Common.Interface;
using TicketManager.Common.Models;

namespace TicketManager.DAL.Repositories
{
	public class NotificationRepository : IRepository<Notification, Guid>
	{
		private IAppDbContext _context;

		public NotificationRepository(IAppDbContext context)
		{
			_context = context;
		}

		public async Task<Guid> CreateAsync(Notification entity)
		{
			await _context.Notifications.AddAsync(entity);
			await _context.SaveChangesAsync();
			return entity.Id;
		}

		public async Task DeleteAsync(Guid id)
		{
			var eventToDelete = await _context.Notifications.FindAsync(id);

			if (eventToDelete != null)
			{
				_context.Notifications.Remove(eventToDelete);
				await _context.SaveChangesAsync();
			}
		}

		public async Task<IList<Notification>> GetAsync()
		{
			return await _context.Notifications.ToListAsync();
		}

		public async Task<Notification> GetByIdAsync(Guid id)
		{
			var notificationToFind = await _context.Notifications.FindAsync(id);

			if (notificationToFind == null)
			{
				throw new ArgumentException($"Entry by id {id} was not found.");
			}

			return notificationToFind;
		}

		public async Task<IList<Notification>> GetSortedAsync(SortingInstructions<Notification> sortingInstructions)
		{
			return sortingInstructions.Direction == OrderByDirection.Ascending
				? await _context.Notifications.OrderBy(sortingInstructions.OrderBy).ToListAsync()
				: await _context.Notifications.OrderByDescending(sortingInstructions.OrderBy).ToListAsync();
		}

		public async Task UpdateAsync(Notification entity)
		{
			_context.Notifications.Update(entity);
			await _context.SaveChangesAsync();
		}
	}
}
