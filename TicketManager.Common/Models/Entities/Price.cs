using System.ComponentModel.DataAnnotations;

namespace TicketManager.Common.Models
{
	public class Price
	{
		[Required]
		public int Id { get; set; }
		[MaxLength(100), Required]
		public float AdultPrice { get; set; }
		[MaxLength(100)]
		public float? ChildPrice { get; set; }
		[MaxLength(100)]
		public float? PromotionPrice { get; set; }
		[MaxLength(100)]
		public float? VipPrice { get; set; }
	}
}
