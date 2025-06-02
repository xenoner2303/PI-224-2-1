using AutoFixture;
using BLL.Commands.UserManipulationsCommands;
using BLL.EntityBLLModels;
using DAL.Data;
using DAL.Entities;
using NSubstitute;

namespace Test;

public class UserCommandManagerTests : CommandTestBase
{
    private readonly IGenericRepository<AuctionLot> lotRepositoryMock;
    private readonly IGenericRepository<ActionLog> actionLogRepositoryMock;
    private readonly IGenericRepository<RegisteredUser> registerUserRepositoryMock;

    public UserCommandManagerTests()
    {
        // заморожуємо репозиторії
        this.lotRepositoryMock = fixture.Freeze<IGenericRepository<AuctionLot>>();
        this.actionLogRepositoryMock = fixture.Freeze<IGenericRepository<ActionLog>>();
        this.registerUserRepositoryMock = fixture.Freeze<IGenericRepository<RegisteredUser>>();

        unitOfWorkMock.AuctionLotRepository.Returns(lotRepositoryMock);
        unitOfWorkMock.ActionLogRepository.Returns(actionLogRepositoryMock);
        unitOfWorkMock.RegisteredUserRepository.Returns(registerUserRepositoryMock);
    }


    [Fact]
    public void CreateBidCommand_ShouldCreateBid_WhenValid()
    {
        // Arrange
        var lot = new AuctionLot { Id = 2, StartPrice = 100, Bids = new List<Bid> { new Bid { Amount = 120 } } };
        var user = new RegisteredUser { Id = 1 };
        var lotModel = fixture.Build<AuctionLotModel>() // завелика генерація, тому виніс вище
            .With(l => l.Id, 2)
            .With(l => l.StartTime, DateTime.Now)
            .With(l => l.EndTime, DateTime.Now.AddDays(1))
            .Without(l => l.Image) // не потрібно для цього тесту + він не зможе створити ресовлер
            .Create();

        var bidModel = fixture.Build<BidModel>()
            .With(b => b.Amount, lot.StartPrice + 50)
            .With(b => b.User, fixture.Build<BaseUserModel>().With(u => u.Id, 1).Create())
            .With(b => b.Lot, lotModel)
            .Create();

        lotRepositoryMock.GetById(2).Returns(lot);
        registerUserRepositoryMock.GetById(1).Returns(user);

        var command = new CreateBidCommand(bidModel, unitOfWorkMock, mapper);

        // Act
        var result = command.Execute();

        // Assert
        Assert.True(result);
        lotRepositoryMock.Received().Update(lot);
        unitOfWorkMock.Received(2).Save(); // один для ставки, а другий для логу
    }

    //[Fact]
    //public void CreateLot_ValidModel_AddsLotAndSaves()
    //{
    //    // arrange
    //    var lotModel = fixture.Create<AuctionLotModel>();

    //    // act
    //    var result = manager.CreateLot(lotModel);

    //    // assert
    //    lotRepositoryMock.Received(1).Add(Arg.Any<AuctionLot>());
    //    unitOfWorkMock.Received(1).Save();
    //    result.Should().BeTrue();
    //}

    //[Fact]
    //public void CreateLot_NullModel_ThrowsException()
    //{
    //    // act & assert
    //    Assert.Throws<ArgumentNullException>(() => new CreateLotCommand(null!, unitOfWorkMock, mapper));
    //}
}
