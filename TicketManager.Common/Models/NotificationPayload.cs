namespace TicketManager.Common.Models
{
	public class NotificationPayload
	{
		public Guid Id { get; set; }
		public string OperationName { get; set; }
		public DateTime Timestamp { get; set; }
		public string CustomerEmail { get; set; }
		public string CustomerName { get; set; }
		public float OrderAmount { get; set; }
		public List<string> OrderSummary { get; set; }
	}
}
