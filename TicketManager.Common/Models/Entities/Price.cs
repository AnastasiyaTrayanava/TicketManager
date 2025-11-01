using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TicketManager.Common.Enums;

namespace TicketManager.Common.Models.Entities
{
	public class Price
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int PriceId { get; set; }
		
		public PriceTier PriceTier { get; set; }
		
		public float PriceValue { get; set; }
		
		public int SeatId { get; set; }

		[ForeignKey("SeatId")]
		public Seat Seat { get; set; }
	}
}
