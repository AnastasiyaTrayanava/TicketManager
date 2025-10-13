namespace TicketManager.Common.Models
{
	public class Price
	{
		public int Id { get; set; }
		public float AdultPrice { get; set; }
		public float? ChildPrice { get; set; }
		public float? PromotionPrice { get; set; }
		public float? VipPrice { get; set; }
	}
}
