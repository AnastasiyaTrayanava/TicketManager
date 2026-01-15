using TicketManager.Common.Models;
using TicketManager.Common.Models.Entities;

namespace TicketManager.Common.Interface
{
	public interface INotificationService
	{
		public Task AddNotification(Notification payload);
		public Task<Notification> CreateNotification(string operationName, string customerEmail, string customerName, float orderAmount, List<CartItem> orderSummary);
	}
}
