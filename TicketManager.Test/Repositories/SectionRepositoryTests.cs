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
	public class SectionRepositoryTests
	{
		private Mock<IAppDbContext> _dbContextMock;
		private Mock<DbSet<Section>> _sectionDbSetMock;
		private SectionRepository _sectionRepository;

		private List<Section> _sections;

		[TestInitialize]
		public void Setup()
		{
			_sections = CreateSectionList();
			_sectionDbSetMock = _sections.BuildMockDbSet();
			_dbContextMock = new Mock<IAppDbContext>();

			_dbContextMock.SetupGet(x => x.Sections).Returns(_sectionDbSetMock.Object);

			_sectionRepository = new SectionRepository(_dbContextMock.Object);
		}

		[TestMethod]
		public async Task CreateAsync_AddSection_ReturnId()
		{
			var section = new Section() { SectionId = 4, Name = "4F", Rows = new List<Row>(), VenueId = 5 };

			var result = await _sectionRepository.CreateAsync(section);

			Assert.IsNotNull(result);
			Assert.AreEqual(4, result);
			_sectionDbSetMock.Verify(x => x.AddAsync(section, It.IsAny<CancellationToken>()), Times.Once);
			_dbContextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
		}

		[TestMethod]
		[ExpectedException(typeof(NullReferenceException))]
		public async Task CreateAsync_EntityIsNull_ThrowsException()
		{
			await _sectionRepository.CreateAsync(null);
		}

		[TestMethod]
		public async Task DeleteAsync_SectionFound_DeletesEntity()
		{
			var section = _sections.First();
			_sectionDbSetMock.Setup(x => x.FindAsync(section.SectionId)).ReturnsAsync(section);

			await _sectionRepository.DeleteAsync(section.SectionId);

			_sectionDbSetMock.Verify(x => x.Remove(It.IsAny<Section>()), Times.Once);
			_dbContextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
		}

		[TestMethod]
		public async Task DeleteAsync_SectionNotExists_DoNothing()
		{
			var sectionId = 5;
			_sectionDbSetMock.Setup(x => x.FindAsync(sectionId)).ReturnsAsync((Section)null);

			await _sectionRepository.DeleteAsync(sectionId);

			_sectionDbSetMock.Verify(x => x.Remove(It.IsAny<Section>()), Times.Never);
			_dbContextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
		}

		[TestMethod]
		public async Task GetAsync_ReturnsSectionList()
		{
			var result = await _sectionRepository.GetAsync();

			Assert.IsNotNull(result);
			Assert.AreEqual(3, result.Count);
		}

		[TestMethod]
		public async Task GetAsync_NoSections_ReturnsEmptyList()
		{
			var sectionDbMockSet = (new List<Section>()).BuildMockDbSet();
			var dbContextMock = new Mock<IAppDbContext>();
			dbContextMock.SetupGet(x => x.Sections).Returns(sectionDbMockSet.Object);
			var repository = new SectionRepository(dbContextMock.Object);

			var result = await repository.GetAsync();

			Assert.IsNotNull(result);
			Assert.AreEqual(0, result.Count);
		}

		[TestMethod]
		public async Task GetByIdAsync_SectionExists_ReturnsSection()
		{
			var sectionId = 0;
			_sectionDbSetMock.Setup(x => x.FindAsync(sectionId)).ReturnsAsync(_sections.First());

			var result = await _sectionRepository.GetByIdAsync(sectionId);

			Assert.IsNotNull(result);
			Assert.AreEqual(sectionId, result.SectionId);
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public async Task GetByIdAsync_SectionNotFound_ThrowsException()
		{
			var sectionId = 5;

			await _sectionRepository.GetByIdAsync(sectionId);
		}

		[TestMethod]
		public async Task GetSortedAsync_SortBySectionNameAscending_ReturnsSortedList()
		{
			var sortingInstructions = new SortingInstructions<Section>
			{
				OrderBy = c => c.Name,
				Direction = OrderByDirection.Ascending
			};

			var result = await _sectionRepository.GetSortedAsync(sortingInstructions);

			Assert.AreEqual(3, result.Count);
		}

		[TestMethod]
		public async Task GetSortedAsync_SortBySectionNameAscendingNoSections_ReturnsEmptyList()
		{
			var sectionDbMockSet = (new List<Section>()).BuildMockDbSet();
			var dbContextMock = new Mock<IAppDbContext>();
			dbContextMock.SetupGet(x => x.Sections).Returns(sectionDbMockSet.Object);
			var repository = new SectionRepository(dbContextMock.Object);

			var sortingInstructions = new SortingInstructions<Section>
			{
				OrderBy = c => c.Name,
				Direction = OrderByDirection.Descending
			};

			var result = await repository.GetSortedAsync(sortingInstructions);

			Assert.AreEqual(0, result.Count);
		}

		[TestMethod]
		public async Task UpdateAsync_UpdatedModel_ReturnsUpdatedSection()
		{
			var section = new Section() { SectionId = _sections.First().SectionId };

			await _sectionRepository.UpdateAsync(section);

			_sectionDbSetMock.Verify(s => s.Update(section), Times.Once);
			_dbContextMock.Verify(s => s.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
		}

		private static List<Section> CreateSectionList()
		{
			return
			[
				new Section { Name = "2A", Rows = new List<Row>(), SectionId = 0, VenueId = 0 },
				new Section { Name = "2B", Rows = new List<Row>(), SectionId = 1, VenueId = 0 },
				new Section { Name = "2C", Rows = new List<Row>(), SectionId = 2, VenueId = 0 },
			];
		}
	}
}
