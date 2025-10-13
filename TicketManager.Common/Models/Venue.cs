namespace TicketManager.Common.Models
{
	public class Venue
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string Address { get; set; }
		public List<Section> Sections { get; set;}
	}
}
