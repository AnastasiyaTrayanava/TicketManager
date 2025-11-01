using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TicketManager.Common.Models.Entities
{
	public class Cart
	{
		[Key]
		[Required]
		public Guid CartId { get; set; }
		
		public List<CartItem> Items { get; set; }

		public int? PaymentId { get; set; }

		[ForeignKey("PaymentId")]
		public Payment? Payment { get; set; }

		[NotMapped]
		public float TotalPrice => Items.Select(x => x.Price.PriceValue).Sum();
	}
}
