namespace TicketManager.Common.Models
{
	public class Row
	{
		public int Id { get; set; }
		public List<Seat> Seats { get; set; }
		public int SectionId { get; set; }

		public Section Section { get; set; }
	}
}
