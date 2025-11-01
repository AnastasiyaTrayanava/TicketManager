using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using TicketManager.Common.Enums;
using TicketManager.Common.Interface;
using TicketManager.Common.Models.Entities;
using TicketManager.DAL;

namespace TicketManager.IntegrationTest
{
	internal static class IntegrationTestsSetup
	{
		public static IAppDbContext SetupDatabase()
		{
			var contextOptions = new DbContextOptionsBuilder<AppDbContext>()
				.UseInMemoryDatabase(Guid.NewGuid().ToString())
				.EnableSensitiveDataLogging()
				.ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
				.Options;

			IAppDbContext appDbContext = new AppDbContext(contextOptions);

			appDbContext.Database.EnsureCreated();
			appDbContext.Database.EnsureDeleted();

			return appDbContext;
		}

		public static void SetupTestData(IAppDbContext appDbContext)
		{
			var cartGuid = new Guid("c9d8e0bf-33d5-4233-8215-a24a825763e3");

			var priceList1 = new List<Price>
			{
				new Price() { PriceTier = PriceTier.Adult, PriceValue = 100.0f, SeatId = 1 },
				new Price() { PriceTier = PriceTier.Vip, PriceValue = 200.0f, SeatId = 1 }
			};

			var priceList2 = new List<Price>
			{
				new Price() { PriceTier = PriceTier.Adult, PriceValue = 100.0f, SeatId = 2 },
				new Price() { PriceTier = PriceTier.Vip, PriceValue = 200.0f, SeatId = 2 }
			};

			var seatList = new List<Seat>
			{
				new Seat()
				{
					SeatState = SeatState.Available,
					SeatNumber = 1,
					Price = priceList1
				},
				new Seat()
				{
					SeatState = SeatState.Available,
					SeatNumber = 2,
					Price = priceList2
				}
			};

			var cartItemsList = new List<CartItem>()
			{
				new CartItem() { CartId = cartGuid, EventId = 0, PriceId = 0, SeatId = 1 },
				new CartItem() { CartId = cartGuid, EventId = 0, PriceId = 0, SeatId = 2 }
			};

			var cart = new Cart()
			{
				CartId = cartGuid,
				Items = cartItemsList,
				PaymentId = null
			};

			var emptyCart = new Cart()
			{
				CartId = new Guid("87bdf9fe-e7a8-4662-b885-e040495d599e")
			};

			appDbContext.Prices.AddRange(priceList1);
			appDbContext.Prices.AddRange(priceList2);
			appDbContext.Seats.AddRange(seatList);
			appDbContext.Carts.AddRange(cart, emptyCart);

			appDbContext.SaveChangesAsync();
		}
	}
}
