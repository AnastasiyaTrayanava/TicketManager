using Microsoft.EntityFrameworkCore;
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
		[DeleteBehavior(DeleteBehavior.Restrict)]
		public Event Event { get; set; }

		[ForeignKey("SeatId")]
		[DeleteBehavior(DeleteBehavior.Restrict)]
		public Seat Seat { get; set; }

		[ForeignKey("PriceId")]
		[DeleteBehavior(DeleteBehavior.Restrict)]
		public Price Price { get; set; }

		[ForeignKey("CartId")]
		[DeleteBehavior(DeleteBehavior.Restrict)]
		public Cart Cart { get; set; }
	}
}
