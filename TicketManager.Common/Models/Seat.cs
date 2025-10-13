using TicketManager.Common.Enums;

namespace TicketManager.Common.Models
{
	public class Seat
	{
		public int Id { get; set; }
		public int SeatNumber { get; set; }
		public int PriceId { get; set; }
		public int RowId { get; set; }
		public SeatState SeatState { get; set; }

		public Row Row { get; set; }
		public Price Price { get; set; }
	}
}
