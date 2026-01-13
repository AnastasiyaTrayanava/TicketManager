using System.ComponentModel.DataAnnotations;
using TicketManager.Common.Enums;

namespace TicketManager.Common.Models
{
	public class Notification
	{
		[Key]
		[Required]
		public Guid Id { get; set; }

		[MaxLength(100), Required]
		public string OperationName { get; set; }

		public DateTime Timestamp { get; set; }

		public NotificationRequestStatus RequestStatus { get; set; }

		public string NotificationParameters { get; set; }

		public string NotificationContent { get; set; }
	}
}
