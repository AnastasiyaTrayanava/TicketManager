using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TicketManager.Common.Enums;

namespace TicketManager.Common.Models.Entities
{
	public class Seat
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int SeatId { get; set; }
		
		public int SeatNumber { get; set; }
		
		public List<Price> Price { get; set; }
		
		public int RowId { get; set; }
		
		public SeatState SeatState { get; set; }

		[ForeignKey("RowId")]
		public Row Row { get; set; }
	}
}
