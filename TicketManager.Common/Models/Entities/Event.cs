using System.ComponentModel.DataAnnotations;

namespace TicketManager.Common.Models
{
	public class Event
	{
		[Required]
		public int Id { get; set; }
		[MaxLength(500), Required]
		public string Name { get; set; }
		[MaxLength(2000), Required]
		public string Description { get; set; }
		[MaxLength(100)]
		public string VenueId { get; set; }
		[Timestamp]
		public DateTime DateTime { get; set; }

		public Venue Venue { get; set; }
	}
}
