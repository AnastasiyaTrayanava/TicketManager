using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using TicketManager.Common.Models;
using TicketManager.Common.Models.Entities;

namespace TicketManager.Common.Interface
{
	public interface IAppDbContext
	{
		public DbSet<Event> Events { get; set; }
		public DbSet<Venue> Venues { get; set; }
		public DbSet<Section> Sections { get; set; }
		public DbSet<Row> Rows { get; set; }
		public DbSet<Seat> Seats { get; set; }
		public DbSet<Price> Prices { get; set; }
		public DbSet<User> Users { get; set; }
		public DbSet<Ticket> Tickets { get; set; }
		public DbSet<Payment> Payments { get; set; }
		public DbSet<Cart> Carts { get; set; }
		public DatabaseFacade Database { get; set; }

		public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
	}
}
