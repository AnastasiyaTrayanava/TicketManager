using TicketManager.Common.Models.Entities;

namespace TicketManager.Common.Models.ViewModels
{
	public class CartItemViewModel
	{
		public int? EventId { get; set; }
		public int? SeatId { get; set; }
		public int? PriceId { get; set; }
		public Event Event { get; set; }
		public Seat Seat { get; set; }
		public Price Price { get; set; }
	}
}
