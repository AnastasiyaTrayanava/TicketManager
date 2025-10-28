using Microsoft.EntityFrameworkCore;
using Moq;
using MockQueryable.Moq;
using TicketManager.Common.Enums;
using TicketManager.Common.Interface;
using TicketManager.Common.Models;
using TicketManager.Common.Models.Entities;
using TicketManager.DAL.Repositories;

namespace TicketManager.Test.Repositories
{
	[TestClass]
	public class CartRepositoryTests
	{
		private Mock<IAppDbContext> _dbContextMock;
		private Mock<DbSet<Cart>> _cartDbSetMock;
		private CartRepository _cartRepository;

		private List<Cart> _carts;

		[TestInitialize]
		public void Setup()
		{
			_carts = CreateCartList();
			_cartDbSetMock = _carts.BuildMockDbSet();
			_dbContextMock = new Mock<IAppDbContext>();

			_dbContextMock.SetupGet(x => x.Carts).Returns(_cartDbSetMock.Object);

			_cartRepository = new CartRepository(_dbContextMock.Object);
		}

		[TestMethod]
		public async Task CreateAsync_AddCart_ReturnId()
		{
			var cart = new Cart() { Items = new List<CartItem>()};

			var result = await _cartRepository.CreateAsync(cart);
			
			Assert.IsNotNull(result);
			Assert.AreNotEqual(Guid.Empty, result);
			_cartDbSetMock.Verify(x => x.AddAsync(cart, It.IsAny<CancellationToken>()), Times.Once);
			_dbContextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
		}

		[TestMethod]
		[ExpectedException(typeof(NullReferenceException))]
		public async Task CreateAsync_EntityIsNull_ThrowsException()
		{
			await _cartRepository.CreateAsync(null);
		}

		[TestMethod]
		public async Task DeleteAsync_CartFound_DeletesEntity()
		{
			var cartId = Guid.NewGuid();
			var cart = new Cart { CartId = cartId };
			_cartDbSetMock.Setup(x => x.FindAsync(cartId)).ReturnsAsync(cart);

			await _cartRepository.DeleteAsync(cartId);

			_cartDbSetMock.Verify(x => x.Remove(It.IsAny<Cart>()), Times.Once);
			_dbContextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
		}

		[TestMethod]
		public async Task DeleteAsync_CartNotExists_DoNothing()
		{
			var cartId = Guid.NewGuid();
			_cartDbSetMock.Setup(x => x.FindAsync(cartId)).ReturnsAsync((Cart)null);

			await _cartRepository.DeleteAsync(cartId);

			_cartDbSetMock.Verify(x => x.Remove(It.IsAny<Cart>()), Times.Never);
			_dbContextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
		}

		[TestMethod]
		public async Task GetAsync_ReturnsCartsList()
		{
			var result = await _cartRepository.GetAsync();

			Assert.IsNotNull(result);
			Assert.AreEqual(2, result.Count);
		}

		[TestMethod]
		public async Task GetAsync_NoCarts_ReturnsEmptyList()
		{
			var cartDbMockSet = (new List<Cart>()).BuildMockDbSet();
			var dbContextMock = new Mock<IAppDbContext>();
			dbContextMock.SetupGet(x => x.Carts).Returns(cartDbMockSet.Object);
			var repository = new CartRepository(dbContextMock.Object);

			var result = await repository.GetAsync();

			Assert.IsNotNull(result);
			Assert.AreEqual(0, result.Count);
		}

		[TestMethod]
		public async Task GetByIdAsync_CartExists_ReturnsCart()
		{
			var cartId = _carts.First().CartId;
			var result = await _cartRepository.GetByIdAsync(cartId);

			Assert.IsNotNull(result);
			Assert.AreEqual(cartId, result.CartId);
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public async Task GetByIdAsync_CartNotFound_ThrowsException()
		{
			var cartId = Guid.NewGuid();

			await _cartRepository.GetByIdAsync(cartId);
		}

		[TestMethod]
		public async Task GetSortedAsync_SortByCartIdAscending_ReturnsSortedList()
		{
			var sortingInstructions = new SortingInstructions<Cart>
			{
				OrderBy = c => c.CartId,
				Direction = OrderByDirection.Ascending
			};

			var result = await _cartRepository.GetSortedAsync(sortingInstructions);

			Assert.AreEqual(2, result.Count);
		}

		[TestMethod]
		public async Task GetSortedAsync_SortByCartIdAscendingNoCarts_ReturnsEmptyList()
		{
			var cartDbMockSet = (new List<Cart>()).BuildMockDbSet();
			var dbContextMock = new Mock<IAppDbContext>();
			dbContextMock.SetupGet(x => x.Carts).Returns(cartDbMockSet.Object);
			var repository = new CartRepository(dbContextMock.Object);

			var sortingInstructions = new SortingInstructions<Cart>
			{
				OrderBy = c => c.CartId,
				Direction = OrderByDirection.Descending
			};

			var result = await repository.GetSortedAsync(sortingInstructions);

			Assert.AreEqual(0, result.Count);
		}

		[TestMethod]
		public async Task UpdateAsync_UpdatedModel_ReturnsUpdatedCart()
		{
			var cart = new Cart { CartId = _carts.First().CartId };

			await _cartRepository.UpdateAsync(cart);

			_cartDbSetMock.Verify(s => s.Update(cart), Times.Once);
			_dbContextMock.Verify(s => s.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
		}
		
		private static List<Cart> CreateCartList()
		{
			return
			[
				new Cart()
				{
					CartId = Guid.NewGuid(), PaymentId = 0, Items = new List<CartItem>()
					{
						new CartItem() { CartItemId = 1, EventId = 1, PriceId = 1, SeatId = 1 },
						new CartItem() { CartItemId = 2, EventId = 1, PriceId = 1, SeatId = 2 }
					}
				},
				new Cart()
				{
					CartId = Guid.NewGuid(), PaymentId = 0, Items =
					[
						new CartItem() { CartItemId = 3, EventId = 2, PriceId = 4, SeatId = 1 },
						new CartItem() { CartItemId = 4, EventId = 2, PriceId = 4, SeatId = 2 }
					]
				}
			];
		}
	}
}
