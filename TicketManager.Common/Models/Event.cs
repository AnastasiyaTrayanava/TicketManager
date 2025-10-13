namespace TicketManager.Common.Models
{
	public class Event
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public string VenueId { get; set; }
		public DateTime DateTime { get; set; }

		public Venue Venue { get; set; }
	}
}
