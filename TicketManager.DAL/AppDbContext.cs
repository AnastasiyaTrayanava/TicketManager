using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using TicketManager.Common.Interface;
using TicketManager.Common.Models;
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

		public string DbPath { get; }

		public AppDbContext(string dbPath)
		{
			DbPath = dbPath;
			Database = base.Database;
		}

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			optionsBuilder.UseSqlServer(DbPath);
		}

		public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
		{
			if (cancellationToken != default)
			{
				return await base.SaveChangesAsync(cancellationToken);
			}
			else
			{
				return await base.SaveChangesAsync();
			}
				
		}
	}
}
