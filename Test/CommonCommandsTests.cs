using AutoFixture;
using BLL.Commands.UserManipulationsCommands;
using BLL.EntityBLLModels;
using DAL.Data;
using DAL.Entities;
using NSubstitute;

namespace Test;

public class CommonCommandsTests : CommandTestBase
{
    private readonly IGenericRepository<AuctionLot> lotRepositoryMock;
    private readonly IGenericRepository<Category> categoryRepositoryMock;
    private readonly UserCommandManager manager;

    public CommonCommandsTests()
    {
        // заморожуємо репозиторії
        this.lotRepositoryMock = fixture.Freeze<IGenericRepository<AuctionLot>>();
        this.categoryRepositoryMock = fixture.Freeze<IGenericRepository<Category>>();

        unitOfWorkMock.AuctionLotRepository.Returns(lotRepositoryMock);
        unitOfWorkMock.CategoryRepository.Returns(categoryRepositoryMock);
        manager = new UserCommandManager(unitOfWorkMock, mapper);
    }

    // ====== Тести для LoadAuctionLotsCommand ======
    [Fact]
    public void LoadAuctionLots_ShouldReturnLotsModels_ExcludingPendingAndRejected()
    {
        // Arrange
        var lots = new List<AuctionLot>
        {
            new AuctionLot { Id = 1, Status = EnumLotStatuses.Active},
            new AuctionLot { Id = 2, Status = EnumLotStatuses.Pending },
            new AuctionLot { Id = 3, Status = EnumLotStatuses.Rejected },
            new AuctionLot { Id = 4, Status = EnumLotStatuses.Active}
        };

        lotRepositoryMock.GetQueryable().Returns(lots.AsQueryable());

        // Act
        var result = manager.LoadAuctionLots();

        // Assert
        Assert.Equal(2, result.Count());
        Assert.All(result, lot => Assert.DoesNotContain(lot.Status, new[] { BusinessEnumLotStatuses.Pending, BusinessEnumLotStatuses.Rejected }));
    }

    [Fact]
    public void LoadAuctionLots_ShouldReturnEmptyList_IfAllAreFiltered()
    {
        // Arrange
        var lots = new List<AuctionLot>
        {
            new AuctionLot { Id = 2, Status = EnumLotStatuses.Pending },
            new AuctionLot { Id = 3, Status = EnumLotStatuses.Rejected },
        };

        lotRepositoryMock.GetQueryable().Returns(lots.AsQueryable());

        // Act
        var result = manager.LoadAuctionLots();

        // Assert
        Assert.Empty(result);
    }

    // ====== Тести для LoadCategoriesCommand ======
    [Fact]
    public void LoadCategories_ShouldReturnCategoryModels()
    {
        // Arrange
        var categories = new List<Category>
        {
            new Category { Id = 1, Name = "Electronics" },
            new Category { Id = 2, Name = "Books" }
        };

        categoryRepositoryMock.GetQueryable().Returns(categories.AsQueryable());

        // Act
        var result = manager.LoadCategories();

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Contains(result, c => c.Name == "Electronics");
        Assert.Contains(result, c => c.Name == "Books");
    }

    [Fact]
    public void LoadCategories_ShouldReturnEmptyList_WhenNoCategoriesExist()
    {
        // Arrange
        var emptyList = new List<Category>();
        categoryRepositoryMock.GetQueryable().Returns(emptyList.AsQueryable());

        // Act
        var result = manager.LoadCategories();

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void LoadCategories_ShouldSubcategoriesModels()
    {
        // Arrange
        var subcategory = new Category { Id = 3, Name = "Smartphones" };
        var parentCategory = new Category
        {
            Id = 1,
            Name = "Electronics",
            Subcategories = new List<Category> { subcategory }
        };

        categoryRepositoryMock.GetQueryable().Returns(new List<Category> { parentCategory }.AsQueryable());

        // Act
        var result = manager.LoadCategories();
        var categoryModel = result.First();

        // Assert
        Assert.Single(result);
        Assert.Equal("Electronics", categoryModel.Name);
        Assert.NotNull(categoryModel.Subcategories);
        Assert.Contains(categoryModel.Subcategories, sc => sc.Name == "Smartphones");
    }

    [Fact]
    public void LoadCategories_DontExistModelsClones() // перевірка конвертеру категорій на відсутність мапінг клонів
    {
        // Arrange
        var subcategory = new Category { Id = 3, Name = "Smartphones" };
        var parentCategory = new Category
        {
            Id = 1,
            Name = "Electronics",
            Subcategories = new List<Category> { subcategory }
        };

        categoryRepositoryMock.GetQueryable().Returns(new List<Category> { parentCategory, subcategory }.AsQueryable());

        // Act
        var result = manager.LoadCategories();
        var categoryModel = result.First();

        // Assert
        Assert.Single(result);
        Assert.NotNull(categoryModel.Subcategories);
    }

    // ====== Тести для SearchLotsCommand ======
    [Fact]
    public void SearchLots_ShouldReturnLotsModelsMatchingKeyword()
    {
        // Arrange
        var lots = new List<AuctionLot>
        {
            new AuctionLot { Id = 1, Title = "Smartphone", Status = EnumLotStatuses.Active },
            new AuctionLot { Id = 2, Title = "SmartTV", Status = EnumLotStatuses.Active },
            new AuctionLot { Id = 3, Title = "VeryBigBook", Status = EnumLotStatuses.Rejected }
        };

        lotRepositoryMock.GetQueryable().Returns(lots.AsQueryable());

        // Act
        var result = manager.SearchLots("phone", null);

        // Assert
        Assert.Single(result);
        Assert.Equal("Smartphone", result.First().Title);
    }

    [Fact]
    public void SearchLots_ShouldReturnLotsModelsInCategoryAndSubcategories()
    {
        // Arrange
        var categories = new List<Category> // додаткова перевірка на валідність використання субкатегорій
        {
            new Category { Id = 1, Name = "Electronics", ParentId = null },
            new Category { Id = 2, Name = "Phones", ParentId = 1 }
        };

        var lots = new List<AuctionLot>
        {
            new AuctionLot { Id = 1, Title = "Smartphone", Status = EnumLotStatuses.Active, CategoryId = 2 },
            new AuctionLot { Id = 2, Title = "SmartTV", Status = EnumLotStatuses.Active, CategoryId = 1 },
            new AuctionLot { Id = 3, Title = "VeryBigBook", Status = EnumLotStatuses.Active, CategoryId = 99 }
        };

        categoryRepositoryMock.GetQueryable().Returns(categories.AsQueryable());
        lotRepositoryMock.GetQueryable().Returns(lots.AsQueryable());

        // Act
        var result = manager.SearchLots("Smart", null);

        // Assert
        Assert.Equal(2, result.Count); // і з категорії і з підкатегорії
    }

    [Fact]
    public void SearchLots_ShouldFilterByKeywordAndCategory()
    {
        // Arrange
        var categories = new List<Category>
        {
            new Category { Id = 1, Name = "Electronics", ParentId = null },
            new Category { Id = 2, Name = "Phones", ParentId = 1 }
        };

        var lots = new List<AuctionLot>
        {
            new AuctionLot { Id = 1, Title = "Phone", Status = EnumLotStatuses.Active, CategoryId = 2 },
            new AuctionLot { Id = 2, Title = "SmartTV", Status = EnumLotStatuses.Active, CategoryId = 1 },
            new AuctionLot { Id = 3, Title = "VeryBigBook", Status = EnumLotStatuses.Active, CategoryId = 99 }
        };

        categoryRepositoryMock.GetQueryable().Returns(categories.AsQueryable());
        lotRepositoryMock.GetQueryable().Returns(lots.AsQueryable());

        // Act
        var result = manager.SearchLots("SmartTV", 1);

        // Assert
        Assert.Single(result);
        Assert.Equal("SmartTV", result[0].Title);
    }

    [Fact]
    public void SearchLots_ShouldExcludePendingAndRejectedLots()
    {
        // Arrange
        var lots = new List<AuctionLot>
        {
            new AuctionLot { Id = 1, Title = "Phone", Status = EnumLotStatuses.Active },
            new AuctionLot { Id = 2, Title = "SmartTV", Status = EnumLotStatuses.Pending },
            new AuctionLot { Id = 3, Title = "VeryBigBook", Status = EnumLotStatuses.Rejected }
        };

        lotRepositoryMock.GetQueryable().Returns(lots.AsQueryable());
        categoryRepositoryMock.GetQueryable().Returns(new List<Category>().AsQueryable());

        // Act
        var result = manager.SearchLots(null, null);

        // Assert
        Assert.Single(result);
        Assert.Equal("Phone", result.First().Title);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void SearchLots_ShouldThrow_WhenCategoryIdIsInvalid(int invalidCategoryId)
    {
        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() => manager.SearchLots(null, invalidCategoryId));
        Assert.Contains("Id категорії повинне бути більше 0", ex.Message);
    }
}
