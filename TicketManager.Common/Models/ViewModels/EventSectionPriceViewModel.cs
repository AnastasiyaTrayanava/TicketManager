using TicketManager.Common.Enums;

namespace TicketManager.Common.Models.ViewModels
{
	public class EventSectionPriceViewModel
	{
		public int PriceId { get; set; }
		public PriceTier PriceTier { get; set; }
		public float PriceValue { get; set; }
	}
}
