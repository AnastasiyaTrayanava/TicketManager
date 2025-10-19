using System.ComponentModel.DataAnnotations;
using TicketManager.Common.Enums;

namespace TicketManager.Common.Models
{
	public class User
	{
		[Required]
		public int Id { get; set; }
		[MaxLength(200), Required]
		public string Name { get; set; }
		[MaxLength(100), Required]
		public string Email { get; set; }
		[MaxLength(100), Required]
		public string Password { get; set; }
		public UserRole Role { get; set; }
	}
}
