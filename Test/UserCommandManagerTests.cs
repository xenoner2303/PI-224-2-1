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
    private readonly UserCommandManager manager;

    public UserCommandManagerTests()
    {
        // заморожуємо репозиторії
        this.lotRepositoryMock = fixture.Freeze<IGenericRepository<AuctionLot>>();
        this.actionLogRepositoryMock = fixture.Freeze<IGenericRepository<ActionLog>>();

        unitOfWorkMock.AuctionLotRepository.Returns(lotRepositoryMock);
        unitOfWorkMock.ActionLogRepository.Returns(actionLogRepositoryMock);

        manager = new UserCommandManager(unitOfWorkMock, mapper);
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
