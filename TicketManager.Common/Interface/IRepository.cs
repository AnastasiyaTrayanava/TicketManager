using TicketManager.Common.Models;

namespace TicketManager.Common.Interface
{
	public interface IRepository<T>
	{
		public Task<T> GetByIdAsync(int id);
		public Task<IList<T>> GetAsync();
		public Task<IList<T>> GetSortedAsync(SortingInstructions<T> sortingInstructions);
		public Task DeleteAsync(int id);
		public Task UpdateAsync(T entity);
		public Task CreateAsync(T entity);
	}
}