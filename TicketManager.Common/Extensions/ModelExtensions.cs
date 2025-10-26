using TicketManager.Common.Models;
using TicketManager.Common.Models.ViewModels;

namespace TicketManager.Common.Extensions
{
	public static class ModelExtensions
	{
		public static EventSectionPriceViewModel ToViewModel(this Price price)
		{
			return new EventSectionPriceViewModel()
			{
				PriceId = price.PriceId,
				PriceTier = price.PriceTier,
				PriceValue = price.PriceValue
			};
		}

		public static EventSectionRowViewModel ToViewModel(this Row row)
		{
			return new EventSectionRowViewModel()
			{
				RowId = row.RowId,
				Seats = row.Seats.Select(ToViewModel).ToList()
			};
		}

		public static EventSectionSeatViewModel ToViewModel(this Seat seat)
		{
			return new EventSectionSeatViewModel()
			{
				SeatId = seat.SeatId,
				SeatState = seat.SeatState,
				Prices = seat.Price.Select(ToViewModel).ToList()
			};
		}

		public static EventSectionViewModel ToViewModel(this Section section)
		{
			return new EventSectionViewModel()
			{
				SectionId = section.SectionId,
				Rows = section.Rows.Select(ToViewModel).ToList()
			};
		}
	}
}
