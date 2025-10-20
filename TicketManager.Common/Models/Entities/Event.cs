using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TicketManager.Common.Models
{
	public class Event
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int EventId { get; set; }

		[MaxLength(500), Required]
		public string Name { get; set; }

		[MaxLength(2000), Required]
		public string Description { get; set; }

		public int VenueId { get; set; }

		[Required]
		public DateTime DateTime { get; set; }

		public Venue Venue { get; set; }
	}
}
