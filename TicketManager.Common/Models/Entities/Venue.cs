using System.ComponentModel.DataAnnotations;

namespace TicketManager.Common.Models
{
	public class Venue
	{
		[Required]
		public int Id { get; set; }
		[MaxLength(200), Required]
		public string Name { get; set; }
		[MaxLength(200)]
		public string Address { get; set; }
		public List<Section> Sections { get; set;}
	}
}
