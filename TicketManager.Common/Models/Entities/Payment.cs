using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TicketManager.Common.Enums;

namespace TicketManager.Common.Models.Entities
{
	public class Payment
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int PaymentId { get; set; }
		
		public Guid CartId { get; set; }
		
		public PaymentStatus PaymentStatus { get; set; }

		public Cart Cart { get; set; }
	}
}
