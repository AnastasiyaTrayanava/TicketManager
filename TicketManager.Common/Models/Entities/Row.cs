using System.ComponentModel.DataAnnotations;

namespace TicketManager.Common.Models
{
	public class Row
	{
		[Required]
		public int Id { get; set; }
		public List<Seat> Seats { get; set; }
		[MaxLength(100), Required]
		public int SectionId { get; set; }

		public Section Section { get; set; }
	}
}
