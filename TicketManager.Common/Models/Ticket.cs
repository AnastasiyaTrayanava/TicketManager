namespace TicketManager.Common.Models
{
	public class Ticket
	{
		public int Id { get; set; }
		public int UserId { get; set; }
		public int EventId { get; set; }
		public int VenueId { get; set; }
		public int SectionId { get; set; }
		public int RowId { get; set; }
		public int SeatId { get; set; }

		public User User { get; set; }
		public Event Event { get; set; }
		public Venue Venue { get; set; }
		public Section Section { get; set; }
		public Row Row { get; set; }
		public Seat Seat { get; set; }
	}
}
