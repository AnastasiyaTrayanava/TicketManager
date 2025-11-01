using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TicketManager.Common.Models.Entities
{
	public class Venue
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int VenueId { get; set; }
		
		[MaxLength(200), Required]
		public string Name { get; set; }
		
		[MaxLength(200), Required]
		public string Address { get; set; }
		
		public List<Section> Sections { get; set;}
	}
}
