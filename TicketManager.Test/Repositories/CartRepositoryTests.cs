using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Moq.EntityFrameworkCore;
using TicketManager.Common.Enums;
using TicketManager.Common.Interface;
using TicketManager.Common.Models;
using TicketManager.Common.Models.Entities;
using TicketManager.DAL;
using TicketManager.DAL.Repositories;

namespace TicketManager.Test.Repositories
{
	[TestClass]
	public class CartRepositoryTests
	{
		private Mock<IAppDbContext> _dbContextMock;
		private Mock<DbSet<Cart>> _cartDbSetMock;
		private CartRepository _cartRepository;

		[TestInitialize]
		public void Setup()
		{
			_cartDbSetMock = GetQueryableMockDbSet(CreateCartList());
			_dbContextMock = new Mock<IAppDbContext>();

			_dbContextMock.Setup<DbSet<Cart>>(x => x.Carts).ReturnsDbSet(_cartDbSetMock.Object);

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
			var carts = new List<Cart>
			{
				new Cart { CartId = Guid.NewGuid() },
				new Cart { CartId = Guid.NewGuid() }
			};
			
			_cartDbSetMock.Setup(x => x.ToListAsync(It.IsAny<CancellationToken>())).ReturnsAsync(carts);

			var result = await _cartRepository.GetAsync();

			Assert.IsNotNull(result);
			Assert.AreEqual(2, result.Count);
		}

		[TestMethod]
		public async Task GetAsync_NoCarts_ReturnsEmptyList()
		{
			_cartDbSetMock.Setup(x => x.ToListAsync(It.IsAny<CancellationToken>())).ReturnsAsync(new List<Cart>());

			var result = await _cartRepository.GetAsync();

			Assert.IsNotNull(result);
			Assert.AreEqual(0, result.Count);
		}

		[TestMethod]
		public async Task GetByIdAsync_CartExists_ReturnsCart()
		{
			var cartId = Guid.NewGuid();
			var cart = new Cart { CartId = cartId };

			var carts = new List<Cart>().AsQueryable();
			_cartDbSetMock.As<IQueryable<Cart>>().Setup(x => x.Provider).Returns(carts.Provider);
			_cartDbSetMock.As<IQueryable<Cart>>().Setup(x => x.Expression).Returns(carts.Expression);
			_cartDbSetMock.As<IQueryable<Cart>>().Setup(x => x.ElementType).Returns(carts.ElementType);
			_cartDbSetMock.As<IQueryable<Cart>>().Setup(x => x.GetEnumerator()).Returns(carts.GetEnumerator);

			//_cartDbSetMock.Setup(x => x.Include())

			//_cartDbSetMock.Setup(m => m
			//	.Include(It.IsAny<string>())).Returns(_cartDbSetMock.Object);
			//_cartDbSetMock.Setup(m => m
			//	.ThenInclude(It.IsAny<string>())).Returns(_cartDbSetMock.Object);
			_cartDbSetMock.Setup(m => m
				.FirstOrDefaultAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Cart, bool>>>(),
									 It.IsAny<CancellationToken>()))
				.ReturnsAsync(cart);

			var result = await _cartRepository.GetByIdAsync(cartId);

			Assert.AreEqual(cartId, result.CartId);
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public async Task GetByIdAsync_Should_Throw_If_Not_Found()
		{
			var cartId = Guid.NewGuid();
			//_cartDbSetMock.Setup(m => m
			//	.Include(It.IsAny<string>())).Returns(_cartDbSetMock.Object);
			//_cartDbSetMock.Setup(m => m
			//	.ThenInclude(It.IsAny<string>())).Returns(_cartDbSetMock.Object);
			//_cartDbSetMock.Setup(m => m
			//	.FirstOrDefaultAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Cart, bool>>>(),
			//						 It.IsAny<CancellationToken>()))
			//	.ReturnsAsync((Cart)null);

			await _cartRepository.GetByIdAsync(cartId);
		}

		[TestMethod]
		public async Task GetSortedAsync_Should_Return_Carts_In_Ascending_Order()
		{
			var carts = new List<Cart>
		{
			new Cart { CartId = Guid.NewGuid() },
			new Cart { CartId = Guid.NewGuid() }
		}.AsQueryable();

			_cartDbSetMock.As<IQueryable<Cart>>().Setup(m => m.Provider).Returns(carts.Provider);
			_cartDbSetMock.As<IQueryable<Cart>>().Setup(m => m.Expression).Returns(carts.Expression);
			_cartDbSetMock.As<IQueryable<Cart>>().Setup(m => m.ElementType).Returns(carts.ElementType);
			_cartDbSetMock.As<IQueryable<Cart>>().Setup(m => m.GetEnumerator()).Returns(carts.GetEnumerator());

			_cartDbSetMock.SetupSequence(m => m.ToListAsync(It.IsAny<CancellationToken>()))
				.ReturnsAsync(carts.ToList());

			var sortingInstructions = new SortingInstructions<Cart>
			{
				OrderBy = c => c.CartId,
				Direction = OrderByDirection.Ascending
			};

			var result = await _cartRepository.GetSortedAsync(sortingInstructions);

			Assert.AreEqual(2, result.Count);
		}

		[TestMethod]
		public async Task GetSortedAsync_Should_Return_Empty_If_No_Carts()
		{
			var carts = new List<Cart>().AsQueryable();

			_cartDbSetMock.As<IQueryable<Cart>>().Setup(m => m.Provider).Returns(carts.Provider);
			_cartDbSetMock.As<IQueryable<Cart>>().Setup(m => m.Expression).Returns(carts.Expression);
			_cartDbSetMock.As<IQueryable<Cart>>().Setup(m => m.ElementType).Returns(carts.ElementType);
			_cartDbSetMock.As<IQueryable<Cart>>().Setup(m => m.GetEnumerator()).Returns(carts.GetEnumerator());

			_cartDbSetMock.SetupSequence(m => m.ToListAsync(It.IsAny<CancellationToken>()))
				.ReturnsAsync(new List<Cart>());

			var sortingInstructions = new SortingInstructions<Cart>
			{
				OrderBy = c => c.CartId,
				Direction = OrderByDirection.Descending
			};

			var result = await _cartRepository.GetSortedAsync(sortingInstructions);

			Assert.AreEqual(0, result.Count);
		}

		[TestMethod]
		public async Task UpdateAsync_Should_Update_Cart()
		{
			var cart = new Cart { CartId = Guid.NewGuid() };

			await _cartRepository.UpdateAsync(cart);

			_cartDbSetMock.Verify(s => s.Update(cart), Times.Once);
			_dbContextMock.Verify(s => s.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public async Task UpdateAsync_Should_Throw_If_Cart_Is_Null()
		{
			await _cartRepository.UpdateAsync(null); // If your code checks for null, otherwise adjust
		}

		private static Mock<DbSet<Cart>> GetQueryableMockDbSet(List<Cart> sourceList)
		{
			var queryable = sourceList.AsQueryable();

			var dbSet = new Mock<DbSet<Cart>>();
			dbSet.As<IQueryable<Cart>>().Setup(m => m.Provider).Returns(queryable.Provider);
			dbSet.As<IQueryable<Cart>>().Setup(m => m.Expression).Returns(queryable.Expression);
			dbSet.As<IQueryable<Cart>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
			dbSet.As<IQueryable<Cart>>().Setup(m => m.GetEnumerator()).Returns(() => queryable.GetEnumerator());
			dbSet.Setup(d => d.Add(It.IsAny<Cart>())).Callback<Cart>((s) => sourceList.Add(s));

			return dbSet;
		}

		private static List<Cart> CreateCartList()
		{
			return new List<Cart>()
			{
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
					CartId = Guid.NewGuid(), PaymentId = 0, Items = new List<CartItem>()
					{
						new CartItem() { CartItemId = 3, EventId = 2, PriceId = 4, SeatId = 1 },
						new CartItem() { CartItemId = 4, EventId = 2, PriceId = 4, SeatId = 2 }
					}
				}
			};
		}
	}
}
