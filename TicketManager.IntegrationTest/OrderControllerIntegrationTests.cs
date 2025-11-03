using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using TicketManager.Common.Interface;
using TicketManager.Common.Models.Entities;
using TicketManager.Common.Models.ViewModels;
using TicketManager.Controllers.Ticketing;
using TicketManager.DAL.Repositories;

namespace TicketManager.IntegrationTest
{
	[TestClass]
	public sealed class OrderControllerIntegrationTests
	{
		private OrderController _orderController;
		private IAppDbContext _context;

		[TestInitialize]
		public void Initialize()
		{
			_context = IntegrationTestsSetup.SetupDatabase();
			IRepository<Cart, Guid> cartRepository = new CartRepository(_context);
			IRepository<Seat, int> seatRepository = new SeatRepository(_context);
			IRepository<Payment, int> paymentRepository = new PaymentRepository(_context);
			IMemoryCache memoryCache = new MemoryCache(new MemoryCacheOptions());

			IntegrationTestsSetup.SetupTestData(_context);

			_orderController = new OrderController(cartRepository, seatRepository, paymentRepository, _context, memoryCache);
		}

		[TestMethod]
		public async Task Add_Item_To_The_Cart()
		{
			var cartGuid = new Guid("c9d8e0bf-33d5-4233-8215-a24a825763e3");
			var cartItem = new CartItemViewModel() { EventId = 0, PriceId = 0, SeatId = 3 };

			var result = await _orderController.AddToCart(cartGuid, cartItem) as OkObjectResult;

			Assert.IsNotNull(result);
			Assert.AreEqual(200, result.StatusCode);
		}

		[TestMethod]
		public async Task Add_EmptyItem_To_The_Cart()
		{
			var cartGuid = new Guid("c9d8e0bf-33d5-4233-8215-a24a825763e3");
			var cartItem = new CartItemViewModel();

			var result = await _orderController.AddToCart(cartGuid, cartItem) as StatusCodeResult;

			Assert.IsNotNull(result);
			Assert.AreEqual(400, result.StatusCode);
		}

		[TestMethod]
		public async Task Remove_Item_From_The_Cart()
		{
			var cartGuid = new Guid("c9d8e0bf-33d5-4233-8215-a24a825763e3");

			var result = await _orderController.RemoveFromCart(cartGuid, 0, 1) as StatusCodeResult;

			Assert.IsNotNull(result);
			Assert.AreEqual(200, result.StatusCode);
		}

		[TestMethod]
		public async Task Remove_Non_Existing_Item_From_The_Cart()
		{
			var cartGuid = new Guid("c9d8e0bf-33d5-4233-8215-a24a825763e3");

			var result = await _orderController.RemoveFromCart(cartGuid, 10, 10) as StatusCodeResult;

			Assert.IsNotNull(result);
			Assert.AreEqual(500, result.StatusCode);
		}

		[TestMethod]
		public async Task Book_Tickets()
		{
			var cartGuid = new Guid("c9d8e0bf-33d5-4233-8215-a24a825763e3");

			var result = await _orderController.BookSeats(cartGuid) as OkObjectResult;

			Assert.IsNotNull(result);
			Assert.AreEqual(200, result.StatusCode);
		}

		[TestMethod]
		public async Task Book_Tickets_From_Empty_Cart()
		{
			var cartGuid = new Guid("87bdf9fe-e7a8-4662-b885-e040495d599e");

			var result = await _orderController.BookSeats(cartGuid) as StatusCodeResult;

			Assert.IsNotNull(result);
			Assert.AreEqual(400, result.StatusCode);
		}
	}
}
