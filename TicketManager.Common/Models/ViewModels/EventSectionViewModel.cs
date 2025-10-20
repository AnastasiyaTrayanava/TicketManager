namespace TicketManager.Common.Models.ViewModels
{
	public class EventSectionViewModel
	{
		public int SectionId { get; set; }
		public List<EventSectionRowViewModel> Rows { get; set; }
	}
}
