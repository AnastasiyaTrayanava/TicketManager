using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TicketManager.Common.Models
{
	public class Row
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int RowId { get; set; }
		
		public List<Seat> Seats { get; set; }
		
		public int SectionId { get; set; }

		public Section Section { get; set; }
	}
}
