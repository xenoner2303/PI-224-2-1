using AutoFixture;
using NSubstitute;
using BLL.Commands;
using DAL.Data;
using DAL.Entities;
using BLL.EntityBLLModels;

namespace Test;

public class AdministratorCommandsTests : CommandTestBase
{
    private readonly IGenericRepository<AbstractUser> userRepositoryMock;
    private readonly IGenericRepository<ActionLog> logsRepositoryMock;
    private readonly IGenericRepository<AbstractSecretCodeRealizator> codeRealizatorRepositoryMock;
    private readonly AdministratorCommandsManager manager;

    public AdministratorCommandsTests()
    {
        // заморожуємо репозиторії
        this.userRepositoryMock = fixture.Freeze<IGenericRepository<AbstractUser>>();
        this.codeRealizatorRepositoryMock = fixture.Freeze<IGenericRepository<AbstractSecretCodeRealizator>>();
        this.logsRepositoryMock = fixture.Freeze<IGenericRepository<ActionLog>>();

        unitOfWorkMock.UserRepository.Returns(userRepositoryMock);
        unitOfWorkMock.SecretCodeRealizatorRepository.Returns(codeRealizatorRepositoryMock);
        unitOfWorkMock.ActionLogRepository.Returns(logsRepositoryMock);

        manager = new AdministratorCommandsManager(unitOfWorkMock, mapper);
    }

    // ====== Тести для LoadLogs ======
    [Fact]
    public void LoadLogs_WithNullTime_ShouldReturnAllLogsModels()
    {
        // Arrange
        var logs = new List<ActionLog> // час для логів автоматично призначається
        {
           new ActionLog("LogName1", "LogDescription1"),
           new ActionLog("LogName2", "LogDescription1")
        };

        logsRepositoryMock.GetAll().Returns(logs);

        // Act
        var result = manager.LoadLogs(null);

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Contains(result, l => l.Description == logs[0].Description);
        Assert.Contains(result, l => l.Description == logs[1].Description);
    }

    [Fact]
    public void LoadLogs_WithNoMatchingDate_ShouldReturnEmptyList()
    {
        // Arrange
        var logs = new List<ActionLog>
        {
            new ActionLog("LogName1", "LogDescription1"),
            new ActionLog("LogName2", "LogDescription1")
        };

        logsRepositoryMock.GetAll().Returns(logs);

        // Act
        var result = manager.LoadLogs(new DateTime(1999, 1, 1));

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void LoadLogs_ShouldReturnEmptyList_WhenNoUserLogsExist()
    {
        // Arrange
        logsRepositoryMock.GetAll().Returns(new List<ActionLog>());

        // Act
        var result = manager.LoadLogs(null);

        // Assert
        Assert.Empty(result);
    }

    // ====== Тести для LoadSecretCodeRealizators ======
    [Fact]
    public void LoadSecretCodeRealizators_ShouldReturnAllRealizatorsModels()
    {
        // Arrange
        var codes = new List<AbstractSecretCodeRealizator>
        {
            new ManagerSecretCodeRealizator("SecretCode1", 10), // код без хешування, бо тест не до цього
            new AdministratorSecretCodeRealization("SecretCode2", 10)
        };

        codes[0].Id = 1; // окремо для тесту призначаємо айді, бо в реальному житті це робить база даних
        codes[1].Id = 2;

        codeRealizatorRepositoryMock.GetAll().Returns(codes);

        // Act
        var result = manager.LoadSecretCodeRealizators();

        // Assert
        Assert.Equal(2, result.Count);

        Assert.Contains(result, r => r.SecretCode == null && r.Id == codes[0].Id);
        Assert.Contains(result, r => r.SecretCode == null && r.Id == codes[1].Id);
    }

    [Fact]
    public void LoadSecretCodeRealizators_ShouldReturnEmptyList_WhenNoUserSecretCodesExist()
    {
        // Arrange
        codeRealizatorRepositoryMock.GetAll().Returns(new List<AbstractSecretCodeRealizator>());

        // Act
        var result = manager.LoadSecretCodeRealizators();

        // Assert
        Assert.Empty(result);
    }

    // ====== Тести для LoadSecretCodeRealizators ======
    [Fact]
    public void LoadUsers_ShouldReturnAllUsersModels()
    {
        // Arrange
        var user = fixture.Build<RegisteredUser>()
            .With(u => u.OwnLots, new List<AuctionLot>())
            .With(u => u.Bids, new List<Bid>())
            .Without(u => u.Email)
            .Without(u => u.PhoneNumber)
            .Create();

        var managerUser = fixture.Build<Manager>()
            .Without(u => u.Email)
            .Without(u => u.PhoneNumber)
            .Create();

        var users = new List<AbstractUser> { user, managerUser };

        userRepositoryMock.GetAll().Returns(users);

        // Act
        var result = manager.LoadUsers();

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Contains(result, u => u.Id == users[0].Id && u.Login == users[0].Login);
        Assert.Contains(result, u => u.Id == users[1].Id && u.Login == users[1].Login);
    }

    [Fact]
    public void LoadUsers_ShouldReturnEmptyList_WhenNoUsersExist()
    {
        // Arrange
        userRepositoryMock.GetAll().Returns(new List<AbstractUser>());

        // Act
        var result = manager.LoadUsers();

        // Assert
        Assert.Empty(result);
    }
}
