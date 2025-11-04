using TicketManager.Common.Models.Entities;

namespace TicketManager.Common.Extensions
{
	public static class CartExtensions
	{
		public static void CalculateTotalPrice(this Cart cart)
		{
			cart.TotalPrice = cart.Items.Select(x => x.Price.PriceValue).Sum();
		}
	}
}
