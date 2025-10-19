using System.ComponentModel.DataAnnotations;

namespace TicketManager.Common.Models
{
	public class Section
	{
		[Required]
		public int Id { get; set; }
		[MaxLength(200), Required]
		public string Name { get; set; }
		public List<Row> Rows { get; set; }
		[MaxLength(100)]
		public int VenueId { get; set; }

		public Venue Venue { get; set; }
	}
}
