using TicketManager.Common.Models;

namespace TicketManager.Common.Interface
{
	public interface IRepository<T, TKey>
	{
		public Task<T> GetByIdAsync(TKey id);
		public Task<IList<T>> GetAsync();
		public Task<IList<T>> GetSortedAsync(SortingInstructions<T> sortingInstructions);
		public Task DeleteAsync(TKey id);
		public Task UpdateAsync(T entity);
		public Task<TKey> CreateAsync(T entity);
	}
}