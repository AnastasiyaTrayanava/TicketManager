using System.ComponentModel.DataAnnotations;

namespace TicketManager.Common.Models
{
	public class Ticket
	{
		[Required]
		public int Id { get; set; }
		[MaxLength(100), Required]
		public int UserId { get; set; }
		[MaxLength(100), Required]
		public int EventId { get; set; }
		[MaxLength(100), Required]
		public int VenueId { get; set; }
		[MaxLength(100), Required]
		public int SectionId { get; set; }
		[MaxLength(100), Required]
		public int RowId { get; set; }
		[MaxLength(100), Required]
		public int SeatId { get; set; }

		public User User { get; set; }
		public Event Event { get; set; }
		public Venue Venue { get; set; }
		public Section Section { get; set; }
		public Row Row { get; set; }
		public Seat Seat { get; set; }
	}
}
