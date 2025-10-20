namespace TicketManager.Common.Models.ViewModels
{
	public class EventSectionRowViewModel
	{
		public int RowId { get; set; }
		public List<EventSectionSeatViewModel> Seats { get; set; }
	}
}
