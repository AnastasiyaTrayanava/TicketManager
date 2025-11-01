using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TicketManager.Common.Models.Entities
{
	public class CartItem
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int CartItemId { get; set; }
		
		[MaxLength(100), Required]
		public int EventId { get; set; }
		
		[MaxLength(100), Required]
		public int SeatId { get; set; }
		
		[MaxLength(100), Required]
		public int PriceId { get; set; }
		
		[MaxLength(100), Required]
		public Guid CartId { get; set; }

		[ForeignKey("EventId")]
		public Event Event { get; set; }

		[ForeignKey("SeatId")]
		public Seat Seat { get; set; }

		[ForeignKey("PriceId")]
		public Price Price { get; set; }

		[ForeignKey("CartId")]
		public Cart Cart { get; set; }
	}
}
