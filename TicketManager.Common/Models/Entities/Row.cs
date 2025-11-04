using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TicketManager.Common.Models.Entities
{
	public class Row
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int RowId { get; set; }
		public int RowNumber { get; set; }
		
		public List<Seat> Seats { get; set; }
		
		public int SectionId { get; set; }

		[ForeignKey("SectionId")]
		public Section Section { get; set; }
	}
}
