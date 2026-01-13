using TicketManager.Common.Models;

namespace TicketManager.Common.Interface
{
	public interface INotificationService
	{
		public Task AddNotification(Notification payload);
		public Task CreateNotification(string operationName, string customerEmail, string customerName, float orderAmount, List<string> orderSummary);
	}
}
