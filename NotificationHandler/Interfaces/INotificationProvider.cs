using TicketManager.Common.Models;

namespace NotificationHandler.Interfaces
{
	public interface INotificationProvider
	{
		Task SendNotification(Notification notification);
	}
}
