using AutoFixture;
using AutoMapper;
using NSubstitute;
using BLL.Commands.UserManipulationsCommands;
using BLL.EntityBLLModels;
using DAL.Data;
using DAL.Entities;

namespace Test;

public class UserCommandsTests : CommandTestBase
{
    private readonly IGenericRepository<AuctionLot> lotRepositoryMock;
    private readonly IGenericRepository<Category> categoryRepositoryMock;
    private readonly IGenericRepository<RegisteredUser> registerUserRepositoryMock;
    private readonly UserCommandManager manager;

    public UserCommandsTests()
    {
        // заморожуємо репозиторії
        this.lotRepositoryMock = fixture.Freeze<IGenericRepository<AuctionLot>>();
        this.categoryRepositoryMock = fixture.Freeze<IGenericRepository<Category>>();
        this.registerUserRepositoryMock = fixture.Freeze<IGenericRepository<RegisteredUser>>();

        unitOfWorkMock.AuctionLotRepository.Returns(lotRepositoryMock);
        unitOfWorkMock.CategoryRepository.Returns(categoryRepositoryMock);
        unitOfWorkMock.RegisteredUserRepository.Returns(registerUserRepositoryMock);
        manager = new UserCommandManager(unitOfWorkMock, mapper);
    }

    // ====== Тести для CreateBitCommand ======
    [Fact]
    public void CreateBid_ShouldCreateBid_WhenValid()
    {
        // Arrange
        var lot = new AuctionLot
        {
            Id = 2,
            StartPrice = 100,
            Bids = new List<Bid> { new Bid { Amount = 120 } }
        };
        var user = new RegisteredUser { Id = 1 };

        var lotModel = fixture.Build<AuctionLotModel>()
            .With(l => l.Id, lot.Id)
            .Without(l => l.StartTime)
            .Without(l => l.EndTime)
            .Without(l => l.Image)
            .Create();

        var bidModel = fixture.Build<BidModel>()
            .With(b => b.Amount, lot.StartPrice + 50) // 150, більша за існуючу ставку 120
            .With(b => b.User, fixture.Build<BaseUserModel>().With(u => u.Id, user.Id).Create())
            .With(b => b.Lot, lotModel)
            .Create();

        // Моки репозиторіїв
        lotRepositoryMock.GetById(lot.Id).Returns(lot); // на випадок, якщо десь використовується
        lotRepositoryMock.GetQueryable().Returns(new List<AuctionLot> { lot }.AsQueryable());

        registerUserRepositoryMock.GetById(user.Id).Returns(user);

        // Тут можна замокати Save у UnitOfWork, якщо потрібно
        unitOfWorkMock.When(uow => uow.Save());

        // Act
        var result = manager.CreateBid(bidModel);

        // Assert
        Assert.True(result);
        unitOfWorkMock.Received(2).Save(); // Один для збереження ставки, один для логування
    }

    [Fact]
    public void CreateBidCommand_PreValidateShouldThrowException_WhenUserIsNull()
    {
        // Arrange
        var lot = new AuctionLot { Id = 2, StartPrice = 100, Bids = new List<Bid> { new Bid { Amount = 120 } } };
        var user = new RegisteredUser { Id = 1 };
        var lotModel = fixture.Build<AuctionLotModel>() // залишаємо, щоб тест був повноцінним та не міг викликати інші виключення
            .With(l => l.Id, lot.Id)
            .Without(l => l.StartTime)// не потрібно для цього тесту
            .Without(l => l.EndTime)
            .Without(l => l.Image)
            .Create();

        var bidModel = fixture.Build<BidModel>()
            .With(b => b.Amount, lot.StartPrice + 50)
            .With(b => b.Lot, lotModel)
            .Without(b => b.User) // User == null
        .Create();

        lotRepositoryMock.GetById(lot.Id).Returns(lot);

        var manager = new UserCommandManager(unitOfWorkMock, mapper);

        // Act & Assert
        var ex = Assert.Throws<ArgumentNullException>(() => manager.CreateBid(bidModel));
        Assert.Contains("Виконавець не може бути пустим", ex.Message);

        unitOfWorkMock.BidRepository.DidNotReceive().Add(Arg.Any<Bid>());
        unitOfWorkMock.DidNotReceive().Save();
    }

    [Fact]
    public void CreateBidCommand_PreValidateShouldThrowException_WhenLotIsNull()
    {
        // Arrange
        var user = new RegisteredUser { Id = 1 };
        var bidModel = fixture.Build<BidModel>()
            .With(b => b.User, fixture.Build<BaseUserModel>().With(u => u.Id, user.Id).Create())
            .Create();

        bidModel.Lot = null; // робимо лот пустим

        registerUserRepositoryMock.GetById(user.Id).Returns(user);

        // Act & Assert
        var ex = Assert.Throws<ArgumentNullException>(() => manager.CreateBid(bidModel));
        Assert.Contains("Лот не може бути пустим", ex.Message);

        unitOfWorkMock.BidRepository.DidNotReceive().Add(Arg.Any<Bid>());
        unitOfWorkMock.DidNotReceive().Save();
    }

    [Fact]
    public void CreateBidCommand_ValidateShouldThrow_WhenAmountIsNegative()
    {
        // Arrange
        var lot = new AuctionLot { Id = 2, StartPrice = 50, Bids = new List<Bid>() };
        var user = new RegisteredUser { Id = 1 };
        var lotModel = fixture.Build<AuctionLotModel>() // залишаємо, щоб тест був повноцінним та не міг викликати інші виключення
           .With(l => l.Id, lot.Id)
           .Without(l => l.StartTime)// не потрібно для цього тесту
           .Without(l => l.EndTime)
           .Without(l => l.Image)
           .Create();

        var bidModel = fixture.Build<BidModel>()
            .With(b => b.Amount, lot.StartPrice - 500) // менша за стартову ціну
            .With(b => b.User, fixture.Build<BaseUserModel>().With(u => u.Id, user.Id).Create())
            .With(b => b.Lot, lotModel)
            .Create();

        lotRepositoryMock.GetById(lot.Id).Returns(lot);
        registerUserRepositoryMock.GetById(user.Id).Returns(user);

        // Act & Assert
        var ex = Assert.Throws<AutoMapperMappingException>(() => manager.CreateBid(bidModel));

        // перевіряємо, що внутрішня причина — ArgumentOutOfRangeException
        Assert.IsType<ArgumentOutOfRangeException>(ex.InnerException);
        Assert.Contains("Сума ставки має бути > 0", ex.InnerException.Message);

        unitOfWorkMock.BidRepository.DidNotReceive().Add(Arg.Any<Bid>());
        unitOfWorkMock.DidNotReceive().Save();
    }

    [Fact]
    public void CreateBidCommand_ValidateShouldThrow_WhenAmountLessThanStartPrice()
    {
        // Arrange
        var lot = new AuctionLot { Id = 2, StartPrice = 100, Bids = new List<Bid>() };
        var user = new RegisteredUser { Id = 1 };
        var lotModel = fixture.Build<AuctionLotModel>()
           .With(l => l.Id, lot.Id)
           .Without(l => l.StartTime)
           .Without(l => l.EndTime)
           .Without(l => l.Image)
           .Create();

        var bidModel = fixture.Build<BidModel>()
            .With(b => b.Amount, lot.StartPrice - 50) // сума менша за стартову ціну
            .With(b => b.User, fixture.Build<BaseUserModel>().With(u => u.Id, user.Id).Create())
            .With(b => b.Lot, lotModel)
            .Create();

        // Замокаємо обидва методи, які використовуються для пошуку лоту
        lotRepositoryMock.GetById(lot.Id).Returns(lot);
        lotRepositoryMock.GetQueryable().Returns(new List<AuctionLot> { lot }.AsQueryable());

        registerUserRepositoryMock.GetById(user.Id).Returns(user);

        // Act & Assert
        var ex = Assert.Throws<ArgumentOutOfRangeException>(() => manager.CreateBid(bidModel));
        Assert.Contains("Сума ставки не може бути меншою за мінімальну", ex.Message);

        unitOfWorkMock.BidRepository.DidNotReceive().Add(Arg.Any<Bid>());
        unitOfWorkMock.DidNotReceive().Save();
    }


    [Theory]
    [InlineData(true, false, "Лот не співпадає з бд")]
    [InlineData(false, true, "Виконавець не співпадає з бд")]
    public void CreateBidCommand_ShouldThrow_WhenUserOrLotNotFound(bool userExists, bool lotExists, string expectedMessage)
    {
        // Arrange
        var lotId = 10;
        var userId = 999;

        var lotModel = fixture.Build<AuctionLotModel>()
            .With(l => l.Id, lotId)
            .Without(l => l.StartTime)
            .Without(l => l.EndTime)
            .Without(l => l.Image)
            .Create();

        var bidModel = fixture.Build<BidModel>()
            .With(b => b.Amount, 110)
            .With(b => b.User, fixture.Build<BaseUserModel>().With(u => u.Id, userId).Create())
            .With(b => b.Lot, lotModel)
            .Create();

        if (lotExists)
        {
            var lot = new AuctionLot { Id = lotId, StartPrice = 100, Bids = new List<Bid>() };
            lotRepositoryMock.GetById(lotId).Returns(lot);
        }
        else
        {
            lotRepositoryMock.GetById(lotId).Returns((AuctionLot)null);
        }

        if (userExists)
        {
            registerUserRepositoryMock.GetById(userId).Returns(new RegisteredUser { Id = userId });
        }
        else
        {
            registerUserRepositoryMock.GetById(userId).Returns((RegisteredUser)null);
        }

        // Act & Assert
        var ex = Assert.Throws<ArgumentNullException>(() => manager.CreateBid(bidModel));
        Assert.Contains(expectedMessage, ex.Message);

        unitOfWorkMock.BidRepository.DidNotReceive().Add(Arg.Any<Bid>());
        unitOfWorkMock.DidNotReceive().Save();
    }

    [Fact]
    public void CreateBidCommand_ShouldThrow_WhenBidModelIsNull()
    {
        // Arrange, Act & Assert
        var ex = Assert.Throws<ArgumentNullException>(() => manager.CreateBid(null));
        Assert.Contains("bidModel", ex.Message);

        unitOfWorkMock.BidRepository.DidNotReceive().Add(Arg.Any<Bid>());
        unitOfWorkMock.DidNotReceive().Save();
    }

    // ====== Тести для CreateLotCommand ======
    [Fact]
    public void CreateLotCommand_ShouldCreateLot_WhenValid()
    {
        // Arrange
        var user = new RegisteredUser { Id = 1 };
        var category = new Category { Id = 2 };

        var lotModel = fixture.Build<AuctionLotModel>()
            .With(l => l.Owner, fixture.Build<BaseUserModel>().With(u => u.Id, user.Id).Create())
            .With(l => l.Category, fixture.Build<CategoryModel>().With(u => u.Id, category.Id).Create())
            .Without(l => l.StartTime)
            .Without(l => l.EndTime)
            .Create();

        registerUserRepositoryMock.GetById(user.Id).Returns(user);
        categoryRepositoryMock.GetById(category.Id).Returns(category);

        // Act
        var result = manager.CreateLot(lotModel);

        // Assert
        Assert.True(result);
        unitOfWorkMock.AuctionLotRepository.Received(1).Add(Arg.Any<AuctionLot>());
        unitOfWorkMock.Received(2).Save(); // 1 для лоту, 1 для логу дії
    }

    [Fact]
    public void CreateLotCommand_ShouldThrow_WhenLotModelIsNull()
    {
        // Arrange, Act & Assert
        var ex = Assert.Throws<ArgumentNullException>(() => manager.CreateLot(null));
        Assert.Contains("lotModel", ex.Message);

        unitOfWorkMock.AuctionLotRepository.DidNotReceive().Add(Arg.Any<AuctionLot>());
        unitOfWorkMock.DidNotReceive().Save();
    }

    [Fact]
    public void CreateLotCommand_PreValidateShouldThrow_WhenUserIsNull()
    {
        // Arrange
        var lotModel = fixture.Build<AuctionLotModel>()
            .Without(l => l.StartTime)
            .Without(l => l.EndTime)
            .Create();

        lotModel.Owner = null; // власника немає

        // Act & Assert
        var ex = Assert.Throws<ArgumentNullException>(() => manager.CreateLot(lotModel));
        Assert.Contains("Власник не може бути попрожнім", ex.Message);
    }

    [Fact]
    public void CreateLotCommand_ShouldThrow_WhenUserNotFoundInDatabase()
    {
        // Arrange
        var userId = 999;
        var lotModel = fixture.Build<AuctionLotModel>()
            .With(l => l.Owner, fixture.Build<BaseUserModel>().With(u => u.Id, userId).Create())
            .With(l => l.StartPrice, 100)
            .Without(l => l.StartTime)
            .Without(l => l.EndTime)
            .Create();

        registerUserRepositoryMock.GetById(userId).Returns((RegisteredUser)null); // не знайдено

        // Act & Assert
        var ex = Assert.Throws<ArgumentNullException>(() => manager.CreateLot(lotModel));
        Assert.Contains("Виконавець не співпадає з бд", ex.Message);

        unitOfWorkMock.AuctionLotRepository.DidNotReceive().Add(Arg.Any<AuctionLot>());
        unitOfWorkMock.DidNotReceive().Save();
    }

    [Fact]
    public void CreateLotCommand_ShouldThrow_WhenStartPriceIsNegative()
    {
        // Arrange
        var user = new RegisteredUser { Id = 1 };

        var lotModel = fixture.Build<AuctionLotModel>()
            .With(l => l.StartPrice, -100) // некоректне значення
            .With(l => l.Owner, fixture.Build<BaseUserModel>().With(u => u.Id, 1).Create())
            .Without(l => l.StartTime)
            .Without(l => l.EndTime)
            .Create();

        registerUserRepositoryMock.GetById(1).Returns(user);

        // Act & AssertArgumentOutOfRangeException
        var ex = Assert.Throws<AutoMapperMappingException>(() => manager.CreateLot(lotModel));

        // перевіряємо, що внутрішня причина — ArgumentOutOfRangeException
        Assert.IsType<ArgumentOutOfRangeException>(ex.InnerException);
        Assert.Contains("Стартова ціна має бути > 0", ex.InnerException.Message);

        unitOfWorkMock.AuctionLotRepository.DidNotReceive().Add(Arg.Any<AuctionLot>());
        unitOfWorkMock.DidNotReceive().Save();
    }

    // ====== Тести для DeleteLotCommand ======
    [Fact]
    public void DeleteLotCommand_ShouldDeleteLot_WhenValidId()
    {
        // Arrange
        var lotId = 5;

        // Act
        var result = manager.DeleteLot(lotId);

        // Assert
        Assert.True(result);
        unitOfWorkMock.AuctionLotRepository.Received(1).Remove(lotId);
        unitOfWorkMock.Received(2).Save(); // для видалення та лоту
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void DeleteLotCommand_ShouldThrow_WhenIdIsIncorrect(int lotId)
    {
        // Arrange, Act & Assert
        var ex = Assert.Throws<ArgumentException>(() => manager.DeleteLot(lotId));
        Assert.Contains("Id лоту повинне бути більше 0", ex.Message);

        unitOfWorkMock.AuctionLotRepository.DidNotReceive().Remove(Arg.Any<int>());
        unitOfWorkMock.DidNotReceive().Save();
    }

    // ====== Тести для LoadUserLotsCommand ======
    [Fact]
    public void LoadUserLotsCommand_ShouldReturnLotsModels_WhenUserHasLots()
    {
        // Arrange
        var userId = 3;

        var lots = new List<AuctionLot>
        {
            new AuctionLot { Id = 1, OwnerId = userId, Owner = new RegisteredUser { Id = userId }, Bids = new List<Bid>() },
            new AuctionLot { Id = 2, OwnerId = userId, Owner = new RegisteredUser { Id = userId }, Bids = new List<Bid>() }
        };

        unitOfWorkMock.AuctionLotRepository.GetQueryable().Returns(lots.AsQueryable());

        // Act
        var result = manager.LoadUserLots(userId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.All(result, lot => Assert.Contains(lot.Id, new[] { 1, 2 })); // перевіряємо отримані за лотами
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void LoadUserLotsCommand_ShouldThrow_WhenUserIdIsIncorrect(int userId)
    {
        // Arrange, Act & Assert
        var ex = Assert.Throws<ArgumentException>(() => manager.LoadUserLots(userId));
        Assert.Contains("Id користувача повинне бути більше 0", ex.Message);

        unitOfWorkMock.AuctionLotRepository.DidNotReceive().GetQueryable();
    }

    [Fact]
    public void LoadUserLotsCommand_ShouldReturnEmptyList_WhenNoUserLotsExist()
    {
        // Arrange
        var userId = 3;

        var lots = new List<AuctionLot>
        {
            new AuctionLot { Id = 1, OwnerId = 4, Owner = new RegisteredUser { Id = userId }, Bids = new List<Bid>() },
            new AuctionLot { Id = 2, OwnerId = 4, Owner = new RegisteredUser { Id = userId }, Bids = new List<Bid>() }
        };

        unitOfWorkMock.AuctionLotRepository.GetQueryable().Returns(lots.AsQueryable());

        // Act
        var result = manager.LoadUserLots(userId);

        // Assert
        Assert.Empty(result);
    }
}
