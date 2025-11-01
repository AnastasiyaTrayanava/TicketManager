using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TicketManager.Common.Models.Entities
{
	public class Section
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int SectionId { get; set; }
		
		[MaxLength(200), Required]
		public string Name { get; set; }
		
		public List<Row> Rows { get; set; }
		
		public int VenueId { get; set; }

		[ForeignKey("VenueId")]
		public Venue Venue { get; set; }
	}
}
