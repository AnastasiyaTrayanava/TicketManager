using Microsoft.EntityFrameworkCore;
using TicketManager.Common.Enums;
using TicketManager.Common.Interface;
using TicketManager.Common.Models;
using TicketManager.Common.Models.Entities;

namespace TicketManager.DAL.Repositories
{
	public class PaymentRepository : IRepository<Payment, int>
	{
		private IAppDbContext _context;

		public PaymentRepository(IAppDbContext context)
		{
			_context = context;
		}

		public async Task<int> CreateAsync(Payment entity)
		{
			await _context.Payments.AddAsync(entity);
			await _context.SaveChangesAsync();
			return entity.PaymentId;
		}

		public async Task DeleteAsync(int id)
		{
			var paymentToDelete = await _context.Payments.FindAsync(id);

			if (paymentToDelete != null)
			{
				_context.Payments.Remove(paymentToDelete);
				await _context.SaveChangesAsync();
			}
		}

		public async Task<IList<Payment>> GetAsync()
		{
			return await _context.Payments.ToListAsync();
		}

		public async Task<Payment> GetByIdAsync(int id)
		{
			var paymentToFind = await _context.Payments.FindAsync(id);

			if (paymentToFind == null)
			{
				throw new ArgumentException($"Entry by id {id} was not found.");
			}

			return paymentToFind;
		}

		public async Task<IList<Payment>> GetSortedAsync(SortingInstructions<Payment> sortingInstructions)
		{
			return sortingInstructions.Direction == OrderByDirection.Ascending
				? await _context.Payments.OrderBy(sortingInstructions.OrderBy).ToListAsync()
				: await _context.Payments.OrderByDescending(sortingInstructions.OrderBy).ToListAsync();
		}

		public async Task UpdateAsync(Payment entity)
		{
			_context.Payments.Update(entity);
			await _context.SaveChangesAsync();
		}
	}
}
