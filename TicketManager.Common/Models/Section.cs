namespace TicketManager.Common.Models
{
	public class Section
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public List<Row> Rows { get; set; }
		public int VenueId { get; set; }

		public Venue Venue { get; set; }
	}
}
