using Microsoft.EntityFrameworkCore;
using MockQueryable.Moq;
using Moq;
using TicketManager.Common.Enums;
using TicketManager.Common.Interface;
using TicketManager.Common.Models;
using TicketManager.Common.Models.Entities;
using TicketManager.DAL.Repositories;

namespace TicketManager.Test.Repositories
{
	[TestClass]
	public class UserRepositoryTests
	{
		private Mock<IAppDbContext> _dbContextMock;
		private Mock<DbSet<User>> _userDbSetMock;
		private UserRepository _userRepository;

		private List<User> _users;

		[TestInitialize]
		public void Setup()
		{
			_users = CreateUserList();
			_userDbSetMock = _users.BuildMockDbSet();
			_dbContextMock = new Mock<IAppDbContext>();

			_dbContextMock.SetupGet(x => x.Users).Returns(_userDbSetMock.Object);

			_userRepository = new UserRepository(_dbContextMock.Object);
		}

		[TestMethod]
		public async Task CreateAsync_AddUser_ReturnId()
		{
			var user = new User() {UserId = 5, Email = "a@a.com", Name = "Test", Password = "Test", Role = UserRole.User};

			var result = await _userRepository.CreateAsync(user);

			Assert.IsNotNull(result);
			Assert.AreEqual(5, result);
			_userDbSetMock.Verify(x => x.AddAsync(user, It.IsAny<CancellationToken>()), Times.Once);
			_dbContextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
		}

		[TestMethod]
		[ExpectedException(typeof(NullReferenceException))]
		public async Task CreateAsync_EntityIsNull_ThrowsException()
		{
			await _userRepository.CreateAsync(null);
		}

		[TestMethod]
		public async Task DeleteAsync_UserFound_DeletesEntity()
		{
			var user = _users.First();
			_userDbSetMock.Setup(x => x.FindAsync(user.UserId)).ReturnsAsync(user);

			await _userRepository.DeleteAsync(user.UserId);

			_userDbSetMock.Verify(x => x.Remove(It.IsAny<User>()), Times.Once);
			_dbContextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
		}

		[TestMethod]
		public async Task DeleteAsync_UserNotExists_DoNothing()
		{
			var userId = 5;
			_userDbSetMock.Setup(x => x.FindAsync(userId)).ReturnsAsync((User)null);

			await _userRepository.DeleteAsync(userId);

			_userDbSetMock.Verify(x => x.Remove(It.IsAny<User>()), Times.Never);
			_dbContextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
		}

		[TestMethod]
		public async Task GetAsync_ReturnsUserList()
		{
			var result = await _userRepository.GetAsync();

			Assert.IsNotNull(result);
			Assert.AreEqual(3, result.Count);
		}

		[TestMethod]
		public async Task GetAsync_NoUsers_ReturnsEmptyList()
		{
			var userDbMockSet = (new List<User>()).BuildMockDbSet();
			var dbContextMock = new Mock<IAppDbContext>();
			dbContextMock.SetupGet(x => x.Users).Returns(userDbMockSet.Object);
			var repository = new UserRepository(dbContextMock.Object);

			var result = await repository.GetAsync();

			Assert.IsNotNull(result);
			Assert.AreEqual(0, result.Count);
		}

		[TestMethod]
		public async Task GetByIdAsync_UserExists_ReturnsUser()
		{
			var userId = 0;
			_userDbSetMock.Setup(x => x.FindAsync(userId)).ReturnsAsync(_users.First());

			var result = await _userRepository.GetByIdAsync(userId);

			Assert.IsNotNull(result);
			Assert.AreEqual(userId, result.UserId);
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public async Task GetByIdAsync_UserNotFound_ThrowsException()
		{
			var userId = 5;

			await _userRepository.GetByIdAsync(userId);
		}

		[TestMethod]
		public async Task GetSortedAsync_SortByUserStatusAscending_ReturnsSortedList()
		{
			var sortingInstructions = new SortingInstructions<User>
			{
				OrderBy = c => c.Name,
				Direction = OrderByDirection.Ascending
			};

			var result = await _userRepository.GetSortedAsync(sortingInstructions);

			Assert.AreEqual(3, result.Count);
		}

		[TestMethod]
		public async Task GetSortedAsync_SortByUserStatusAscendingNoUsers_ReturnsEmptyList()
		{
			var userDbMockSet = (new List<User>()).BuildMockDbSet();
			var dbContextMock = new Mock<IAppDbContext>();
			dbContextMock.SetupGet(x => x.Users).Returns(userDbMockSet.Object);
			var repository = new UserRepository(dbContextMock.Object);

			var sortingInstructions = new SortingInstructions<User>
			{
				OrderBy = c => c.Name,
				Direction = OrderByDirection.Descending
			};

			var result = await repository.GetSortedAsync(sortingInstructions);

			Assert.AreEqual(0, result.Count);
		}

		[TestMethod]
		public async Task UpdateAsync_UpdatedModel_ReturnsUpdatedUser()
		{
			var user = new User() { UserId = _users.First().UserId };

			await _userRepository.UpdateAsync(user);

			_userDbSetMock.Verify(s => s.Update(user), Times.Once);
			_dbContextMock.Verify(s => s.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
		}

		private static List<User> CreateUserList()
		{
			return
			[
				new User() {UserId = 0, Email = "b@b.com", Name = "Test1", Password = "Test2", Role = UserRole.Admin },
				new User() {UserId = 1, Email = "c@c.com", Name = "Test2", Password = "Test3", Role = UserRole.User },
				new User() {UserId = 2, Email = "d@d.com", Name = "Test3", Password = "Test4", Role = UserRole.EventManager }
			];
		}
	}
}
