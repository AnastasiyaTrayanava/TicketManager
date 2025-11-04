using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

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
		[DeleteBehavior(DeleteBehavior.Restrict)]
		public User User { get; set; }

		[ForeignKey("EventId")]
		[DeleteBehavior(DeleteBehavior.Restrict)]
		public Event Event { get; set; }

		[ForeignKey("VenueId")]
		[DeleteBehavior(DeleteBehavior.Restrict)]
		public Venue Venue { get; set; }

		[ForeignKey("SectionId")]
		[DeleteBehavior(DeleteBehavior.Restrict)]
		public Section Section { get; set; }

		[ForeignKey("RowId")]
		[DeleteBehavior(DeleteBehavior.Restrict)]
		public Row Row { get; set; }

		[ForeignKey("SeatId")]
		[DeleteBehavior(DeleteBehavior.Restrict)]
		public Seat Seat { get; set; }
	}
}
