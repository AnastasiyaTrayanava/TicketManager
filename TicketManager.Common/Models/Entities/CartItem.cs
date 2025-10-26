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

		public Event Event { get; set; }
		
		public Seat Seat { get; set; }
		
		public Price Price { get; set; }
		
		public Cart Cart { get; set; }
	}
}
