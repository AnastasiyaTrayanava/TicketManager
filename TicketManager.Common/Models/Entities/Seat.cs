using System.ComponentModel.DataAnnotations;
using TicketManager.Common.Enums;

namespace TicketManager.Common.Models
{
	public class Seat
	{
		[Required]
		public int Id { get; set; }
		[MaxLength(100), Required]
		public int SeatNumber { get; set; }
		[MaxLength(100)]
		public int PriceId { get; set; }
		[MaxLength(100)]
		public int RowId { get; set; }
		public SeatState SeatState { get; set; }

		public Row Row { get; set; }
		public Price Price { get; set; }
	}
}
