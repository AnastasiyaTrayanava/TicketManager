using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TicketManager.Common.Models.Entities
{
	public class Ticket
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int TicketId { get; set; }
		
		public int UserId { get; set; }
		
		public int EventId { get; set; }
		
		public int VenueId { get; set; }
		
		public int SectionId { get; set; }
		
		public int RowId { get; set; }
		
		public int SeatId { get; set; }

		[ForeignKey("UserId")]
		public User User { get; set; }

		[ForeignKey("EventId")]
		public Event Event { get; set; }

		[ForeignKey("VenueId")]
		public Venue Venue { get; set; }

		[ForeignKey("SectionId")]
		public Section Section { get; set; }

		[ForeignKey("RowId")]
		public Row Row { get; set; }

		[ForeignKey("SeatId")]
		public Seat Seat { get; set; }
	}
}
