using System.Linq.Expressions;
using TicketManager.Common.Enums;

namespace TicketManager.Common.Models
{
	public class SortingInstructions<T>
	{
		public Expression<Func<T, object>> OrderBy { get; set; }
		public OrderByDirection Direction { get; set; }
	}
}
