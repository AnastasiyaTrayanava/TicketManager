using TicketManager.Common.Models;

namespace TicketManager.Common.Interface
{
	public interface INotificationService
	{
		public Task AddNotification(NotificationPayload payload);
	}
}
