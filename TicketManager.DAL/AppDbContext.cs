using Microsoft.EntityFrameworkCore;
using TicketManager.Common.Models;

namespace TicketManager.DAL
{
	public class AppDbContext(string dbPath) : DbContext
	{
		public DbSet<Event> Events { get; set; }
		public DbSet<Venue> Venues {get; set; }
		public DbSet<Section> Sections { get; set; }
		public DbSet<Row> Rows { get; set; }
		public DbSet<Seat> Seats {get; set; }
		public DbSet<Price> Prices {get; set; }
		public DbSet<User> Users {get; set; }
		public DbSet<Ticket> Tickets {get; set; }

		public string DbPath { get; } = dbPath;

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			optionsBuilder.UseSqlServer(DbPath);
		}
	}
}
