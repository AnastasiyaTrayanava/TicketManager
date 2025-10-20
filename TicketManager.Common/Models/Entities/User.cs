using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TicketManager.Common.Enums;

namespace TicketManager.Common.Models
{
	public class User
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int UserId { get; set; }
		
		[MaxLength(20), Required]
		public string Name { get; set; }
		
		[MaxLength(50), Required]
		public string Email { get; set; }
		
		[MaxLength(20), Required]
		public string Password { get; set; }
		
		public UserRole Role { get; set; }
	}
}
