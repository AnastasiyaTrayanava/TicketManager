using TicketManager.Common.Enums;

namespace TicketManager.Common.Models.ViewModels
{
	public class EventSectionSeatViewModel
	{
		public int SeatId { get; set; }
		public SeatState SeatState { get; set; }
		public List<EventSectionPriceViewModel> Prices { get; set; }
	}
}
