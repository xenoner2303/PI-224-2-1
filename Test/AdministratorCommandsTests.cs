using AutoFixture;
using NSubstitute;
using BLL.Commands;
using DAL.Data;
using DAL.Entities;
using BLL.EntityBLLModels;
using AutoMapper;
using BLL.Services;
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

    // ====== Тести для LoadUsers ======
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

    // ====== Тести для RemoveCodeRealizatorByIdCommand ======

    [Fact]
    public void RemoveSecretCodeRealizator_WithValidId_ShouldRemoveAndSaveAndReturnTrue()
    {
        // Arrange
        const int validId = 7;

        // Act
        var result = manager.RemoveSecretCodeRealizator(validId);

        // Assert
        Assert.True(result);
        codeRealizatorRepositoryMock.Received(1).Remove(validId);
        unitOfWorkMock.Received(2).Save(); // перший раз для збереження змін, другий раз для логування дії
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-3)]
    public void RemoveSecretCodeRealizator_WithNonPositiveId_ShouldThrowArgumentException(int invalidId)
    {
        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(
            () => manager.RemoveSecretCodeRealizator(invalidId));

        Assert.Equal("id", ex.ParamName); // перевіримо правильний параметр
    }

    [Fact]
    public void RemoveSecretCodeRealizator_WhenRepositoryThrows_ShouldPropagateException()
    {
        // Arrange
        const int id = 10;
        codeRealizatorRepositoryMock
            .When(r => r.Remove(id))
            .Do(_ => throw new InvalidOperationException("DB failure"));

        // Act & Assert
        var ex = Assert.Throws<InvalidOperationException>(
            () => manager.RemoveSecretCodeRealizator(id));

        Assert.Equal("DB failure", ex.Message);
        codeRealizatorRepositoryMock.Received(1).Remove(id);
        unitOfWorkMock.DidNotReceive().Save();
    }

    // ====== Тести для CreateCodeRealizatorCommand ======

    [Fact]
    public void CreateCodeRealizator_WithValidManagerModel_ShouldReturnTrue()
    {
        // Arrange
        var model = new SecretCodeRealizatorModel
        {
            SecretCode = "manager123",
            CodeUses = 5,
            RealizatorType = BusinessEnumInterfaceType.Manager
        };

        // Act
        var result = manager.CreateCodeRealizator(model);

        // Assert
        Assert.True(result);
        codeRealizatorRepositoryMock.Received(1)
            .Add(Arg.Is<AbstractSecretCodeRealizator>(r =>
                r is ManagerSecretCodeRealizator &&
                r.CodeUses == model.CodeUses));
        unitOfWorkMock.Received(2).Save(); // 2 рази: перший для збереження, другий для логування дії
    }

    [Fact]
    public void CreateCodeRealizator_WithValidAdministratorModel_ShouldReturnTrue()
    {
        // Arrange
        var model = new SecretCodeRealizatorModel
        {
            SecretCode = "admin456",
            CodeUses = 3,
            RealizatorType = BusinessEnumInterfaceType.Administrator
        };

        // Act
        var result = manager.CreateCodeRealizator(model);

        // Assert
        Assert.True(result);
        codeRealizatorRepositoryMock.Received(1)
            .Add(Arg.Is<AbstractSecretCodeRealizator>(r =>
                r is AdministratorSecretCodeRealization &&
                r.CodeUses == model.CodeUses));
        unitOfWorkMock.Received(2).Save(); // 2 рази: перший для збереження, другий для логування дії
    }

    [Fact]
    public void CreateCodeRealizator_WithEmptyCode_ShouldThrowArgumentException()
    {
        // Arrange
        var model = new SecretCodeRealizatorModel
        {
            SecretCode = "",
            CodeUses = 5,
            RealizatorType = BusinessEnumInterfaceType.Manager
        };

        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() => manager.CreateCodeRealizator(model));
        Assert.Contains("Секретний код не може бути порожнім", ex.Message);
        codeRealizatorRepositoryMock.DidNotReceive().Add(Arg.Any<AbstractSecretCodeRealizator>());
    }

    [Fact]
    public void CreateCodeRealizator_WithZeroUses_ShouldThrowArgumentException()
    {
        // Arrange
        var model = new SecretCodeRealizatorModel
        {
            SecretCode = "validcode",
            CodeUses = 0,
            RealizatorType = BusinessEnumInterfaceType.Administrator
        };

        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() => manager.CreateCodeRealizator(model));
        Assert.Contains("Кількість використань повинна бути більше 0", ex.Message);
        codeRealizatorRepositoryMock.DidNotReceive().Add(Arg.Any<AbstractSecretCodeRealizator>());
    }

    [Fact]
    public void CreateCodeRealizator_WithUnknownType_ShouldThrowArgumentException()
    {
        // Arrange
        var unknownType = (BusinessEnumInterfaceType)999; // інвалідний тип
        var model = new SecretCodeRealizatorModel
        {
            SecretCode = "validcode",
            CodeUses = 3,
            RealizatorType = unknownType
        };

        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() => manager.CreateCodeRealizator(model));
        Assert.Contains("Невідомий тип реалізатора", ex.Message);
        codeRealizatorRepositoryMock.DidNotReceive().Add(Arg.Any<AbstractSecretCodeRealizator>());
    }

    // ====== Тести для RemoveUserByIdCommand ======

    [Fact]
    public void RemoveUser_WithValidId_ShouldReturnTrue_AndCallRemoveAndSave()
    {
        // Arrange
        int userId = 1;

        // Act
        var result = manager.RemoveUser(userId);

        // Assert
        Assert.True(result);
        unitOfWorkMock.UserRepository.Received(1).Remove(userId);
        unitOfWorkMock.Received(2).Save(); // 2 рази: перший для збереження, другий для логування дії
    }

    [Fact]
    public void RemoveUser_WithInvalidId_ShouldThrowArgumentException()
    {
        // Arrange
        int invalidId = 0;

        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() => manager.RemoveUser(invalidId));
        Assert.Contains("ID не може бути менше або дорівнювати нулю", ex.Message);

        // Перевірка, що нічого не викликалося
        unitOfWorkMock.UserRepository.DidNotReceive().Remove(Arg.Any<int>());
        unitOfWorkMock.DidNotReceive().Save();
    }

    [Fact]
    public void RemoveUser_WhenRepositoryThrows_ShouldRethrowAsCommandException()
    {
        // Arrange
        int userId = 10;
        unitOfWorkMock.UserRepository
            .When(r => r.Remove(userId))
            .Do(_ => throw new Exception("DB error"));

        // Act & Assert
        var ex = Assert.Throws<Exception>(() => manager.RemoveUser(userId));
        Assert.Contains("DB error", ex.Message);

        unitOfWorkMock.UserRepository.Received(1).Remove(userId);
        unitOfWorkMock.Received(0).Save(); // Save не викликається, бо Remove падає
    }

    // ====== Тести для SaveUserChanges ======

    [Fact]
    public void SaveUsersChanges_WithValidChanges_ShouldUpdateUser()
    {
        // Arrange
        var userId = 1;
        var existingUser = new RegisteredUser
        {
            Id = userId,
            Login = "oldLogin",
            FirstName = "Old",
            LastName = "User",
            Email = "old@example.com",
            PhoneNumber = "1234567890",
            Age = 25
            // PasswordHash залишаємо null, аби перевірити, що воно зміниться
        };

        var updatedModel = new BaseUserModel
        {
            Id = userId,
            Login = "newLogin",
            FirstName = "New",
            LastName = "User",
            Email = "new@example.com",
            PhoneNumber = "0987654321",
            Age = 30,
            Password = "newpass",
            InterfaceType = BusinessEnumInterfaceType.Registered
        };

        userRepositoryMock.GetById(userId).Returns(existingUser);

        // Act
        var result = manager.SaveUsersChanges(new List<BaseUserModel> { updatedModel });

        // Assert
        Assert.True(result);

        userRepositoryMock.Received(1).Update(Arg.Is<AbstractUser>(u =>
            u.Login == "newLogin" &&
            u.FirstName == "New" &&
            u.Email == "new@example.com" &&
            !string.IsNullOrWhiteSpace(u.PasswordHash)   // перевіряємо просто, що воно не порожнє
        ));

        unitOfWorkMock.Received(2).Save(); // перший раз для логування, другий раз для збереження змін
    }

    [Fact]
    public void SaveUsersChanges_WhenUserNotFound_ShouldSkipUpdate()
    {
        // Arrange
        var model = new BaseUserModel
        {
            Id = 42,
            Login = "ghost",
            InterfaceType = BusinessEnumInterfaceType.Registered
        };

        userRepositoryMock.GetById(42).Returns((AbstractUser?)null);

        // Act
        var result = manager.SaveUsersChanges(new List<BaseUserModel> { model });

        // Assert
        Assert.True(result);
        userRepositoryMock.DidNotReceive().Update(Arg.Any<AbstractUser>());
        unitOfWorkMock.Received(2).Save(); // перший раз для логування, другий раз для збереження
    }

    [Fact]
    public void SaveUsersChanges_WhenRoleChanged_ShouldReplaceUser()
    {
        // Arrange
        var userId = 7;
        var oldUser = new RegisteredUser
        {
            Id = userId,
            Login = "roleTest",
            FirstName = "Old"
        };

        var updatedModel = new BaseUserModel
        {
            Id = userId,
            Login = "roleTest",
            FirstName = "Old",
            InterfaceType = BusinessEnumInterfaceType.Manager // роль змінилася
        };

        userRepositoryMock.GetById(userId).Returns(oldUser);

        // Act
        var result = manager.SaveUsersChanges(new List<BaseUserModel> { updatedModel });

        // Assert
        Assert.True(result);
        userRepositoryMock.Received(1).Remove(userId);
        userRepositoryMock.Received(1).Add(Arg.Is<AbstractUser>(u =>
            u.Login == "roleTest" && u is Manager));
        unitOfWorkMock.Received(4).Save(); // Save всередині ChangeUserRole + фінальне
    }

    [Fact]
    public void SaveUsersChanges_WhenNoChanges_ShouldNotCallUpdateOrRemove()
    {
        // Arrange
        var userId = 99;
        var user = new RegisteredUser
        {
            Id = userId,
            Login = "same",
            FirstName = "Same",
            LastName = "Same",
            Email = "same@mail.com",
            PhoneNumber = "+380987654321",
            Age = 20,
            PasswordHash = PasswordHasher.HashPassword("unchanged"),
        };

        var model = new BaseUserModel
        {
            Id = userId,
            Login = "same",
            FirstName = "Same",
            LastName = "Same",
            Email = "same@mail.com",
            PhoneNumber = "+380987654321",
            Age = 20,
            Password = "", // password не змінено
            InterfaceType = BusinessEnumInterfaceType.Registered
        };

        userRepositoryMock.GetById(userId).Returns(user);

        // Act
        var result = manager.SaveUsersChanges(new List<BaseUserModel> { model });

        // Assert
        Assert.True(result);
        userRepositoryMock.DidNotReceive().Update(Arg.Any<AbstractUser>());
        userRepositoryMock.DidNotReceive().Remove(Arg.Any<int>());
        unitOfWorkMock.Received(2).Save(); // 2 рази: перший для логування, другий для збереження
    }
}