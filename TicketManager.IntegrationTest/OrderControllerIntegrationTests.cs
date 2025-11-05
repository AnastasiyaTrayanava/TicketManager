using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Text;
using Newtonsoft.Json;
using TicketManager.Common.Models.Entities;
using TicketManager.Common.Models.ViewModels;
using TicketManager.DAL;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace TicketManager.IntegrationTest
{
	[TestClass]
	public sealed class OrderControllerIntegrationTests
	{
		private WebApplicationFactory<Program> _factory;
		private HttpClient _client;

		private AppDbContext _appDbContext;

		private Guid _cartGuid;

		[TestInitialize]
		public async Task Initialize()
		{
			_cartGuid = Guid.NewGuid();
			_factory = new WebApplicationFactory<Program>();
			_client = _factory.CreateClient();

			var scope = _factory.Services.CreateScope();
			_appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

			var cart = new Cart() { CartId = _cartGuid };
			await _appDbContext.Carts.AddAsync(cart);
			await _appDbContext.SaveChangesAsync();
		}

		[TestCleanup]
		public async Task Cleanup()
		{
			var cart = await _appDbContext.Carts.Include(c => c.Items).FirstOrDefaultAsync(x => x.CartId == _cartGuid);
			if (cart != null)
			{
				_appDbContext.Carts.Remove(cart);
				await _appDbContext.SaveChangesAsync();
			}
			await _factory.DisposeAsync();
		}

		[TestMethod]
		public async Task Get_OrderEndpointReturnSuccessAndCorrectContentType()
		{
			var response = await _client.GetAsync($"orders/carts/cartId?cartId={_cartGuid}");

			response.EnsureSuccessStatusCode();
			Assert.AreEqual("application/json; charset=utf-8",
				response.Content.Headers.ContentType.ToString());
		}

		[TestMethod]
		public async Task AddToCart_AddsToCartAndReturnsSuccess()
		{
			var cartItemViewModel = new CartItemViewModel()
			{
				EventId = 1,
				PriceId = 37,
				SeatId = 10,
				Seat = new Seat()
				{
					SeatId = 10,
					RowId = 2,
					Row = new Row()
					{
						RowId = 2,
						SectionId = 1
					}
				}
			};
			var content = new StringContent(JsonSerializer.Serialize(cartItemViewModel), Encoding.UTF8, "application/json");

			var response = await _client.PostAsync($"orders/carts/cartId?cartId={_cartGuid}", content);
			response.EnsureSuccessStatusCode();

			var responseData = JsonConvert.DeserializeObject<Cart>(await response.Content.ReadAsStringAsync());

			Assert.IsTrue(responseData.Items.Any());
		}

		[TestMethod]
		public async Task BookSeats_BookSeatsAndReturnSuccess()
		{
			var cartItemViewModel = new CartItemViewModel()
			{
				EventId = 1,
				PriceId = 37,
				SeatId = 10,
				Seat = new Seat()
				{
					SeatId = 10,
					RowId = 2,
					Row = new Row()
					{
						RowId = 2,
						SectionId = 1
					}
				}
			};
			var content = new StringContent(JsonSerializer.Serialize(cartItemViewModel), Encoding.UTF8, "application/json");

			var addItemResponse = await _client.PostAsync($"orders/carts/cartId?cartId={_cartGuid}", content);
			addItemResponse.EnsureSuccessStatusCode();

			var bookItemResponse = await _client.PutAsync($"orders/carts/{_cartGuid}/book", new StringContent(""));
			bookItemResponse.EnsureSuccessStatusCode();

			var responseData = JsonConvert.DeserializeObject<int>(await bookItemResponse.Content.ReadAsStringAsync());

			Assert.IsTrue(responseData != 0);
		}
	}
}
