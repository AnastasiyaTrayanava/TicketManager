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

		public int UserId { get; set; }

		[ForeignKey("PaymentId")]
		public Payment? Payment { get; set; }

		[ForeignKey("UserId")]
		public User User { get; set; }

		[NotMapped]
		public float TotalPrice { get; set; }
	}
}
