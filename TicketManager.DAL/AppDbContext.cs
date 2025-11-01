using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using TicketManager.Common.Interface;
using TicketManager.Common.Models.Entities;

namespace TicketManager.DAL
{
	public class AppDbContext : DbContext, IAppDbContext
	{
		public virtual DbSet<Event> Events { get; set; }
		public virtual DbSet<Venue> Venues { get; set; }
		public virtual DbSet<Section> Sections { get; set; }
		public virtual DbSet<Row> Rows { get; set; }
		public virtual DbSet<Seat> Seats { get; set; }
		public virtual DbSet<Price> Prices { get; set; }
		public virtual DbSet<User> Users { get; set; }
		public virtual DbSet<Ticket> Tickets { get; set; }
		public virtual DbSet<Payment> Payments { get; set; }
		public virtual DbSet<Cart> Carts { get; set; }
		public virtual DatabaseFacade Database { get; set; }

		public AppDbContext()
		{
			Database = base.Database;
		}

		public AppDbContext(DbContextOptions<AppDbContext> contextOptions) : base(contextOptions)
		{
			Database = base.Database;
		}

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
		}

		public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
		{
			return await base.SaveChangesAsync(cancellationToken);
		}
	}
}
