using AutoFixture;
using BLL.Commands.PreUsersManipulationCommands;
using BLL.Commands.UserManipulationsCommands;
using BLL.EntityBLLModels;
using BLL.Services;
using DAL.Data;
using DAL.Entities;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test;

public class PreUserCommandsTests : CommandTestBase
{
    private readonly IGenericRepository<AbstractUser> userRepositoryMock;
    private readonly IGenericRepository<AbstractSecretCodeRealizator> codeRealizatorRepositoryMock;
    private readonly PreUserCommandsManager manager;

    public PreUserCommandsTests()
    {
        // заморожуємо репозиторії
        this.userRepositoryMock = fixture.Freeze<IGenericRepository<AbstractUser>>();
        this.codeRealizatorRepositoryMock = fixture.Freeze<IGenericRepository<AbstractSecretCodeRealizator>>();

        unitOfWorkMock.UserRepository.Returns(userRepositoryMock);
        unitOfWorkMock.SecretCodeRealizatorRepository.Returns(codeRealizatorRepositoryMock);

        manager = new PreUserCommandsManager(unitOfWorkMock, mapper);
    }

    // ====== Тести для CreateUserCommand ======
    [Theory]
    [InlineData("qwerty123", "qwerty", typeof(ManagerSecretCodeRealizator), EnumUserInterfaceType.Registered)]
    [InlineData("qwerty", "qwerty", typeof(ManagerSecretCodeRealizator), EnumUserInterfaceType.Manager)]
    [InlineData("qwerty", "qwerty", typeof(AdministratorSecretCodeRealization), EnumUserInterfaceType.Administrator)]
    public void CreateUser_ShouldCreateRightUserType_WhenModelIsValid(string userSecretCode, 
        string rightSecretCode,
        Type realizatorType, 
        EnumUserInterfaceType requiredType)
    {
        // Arrange
        var model = fixture.Build<BaseUserModel>()
            .With(x => x.Password, "securePass123")
            .With(x => x.Login, "uniqueLogin")
            .With(x => x.FirstName, "Іван")
            .With(x => x.LastName, "Іванов")
            .With(x => x.Email, "valid@email.com")
            .With(x => x.PhoneNumber, "+380123456789")
            .With(x => x.Age, 25)
            .Create();

        model.SecretCode = userSecretCode; // секретний код для тесту

        // створюємо реалізатор динамічно через Activator з автоподбором контруктору за типу
        var realizator = (AbstractSecretCodeRealizator)Activator.CreateInstance(realizatorType, "hash", 3);
        realizator.SecretCodeHash = PasswordHasher.HashPassword(rightSecretCode);

        userRepositoryMock.GetQueryable().Returns(Enumerable.Empty<AbstractUser>().AsQueryable());
        codeRealizatorRepositoryMock.GetQueryable().Returns(new[] { realizator }.AsQueryable());

        // Act
        var result = manager.CreateUser(model);

        // Assert
        Assert.True(result);
        userRepositoryMock.Received(1).Add(Arg.Is<AbstractUser>(u => u.Login == model.Login
                           && u.InterfaceType == requiredType));
        unitOfWorkMock.Received(2).Save(); // один для користувача, другий для реалізатора
    }

    [Theory]
    [InlineData("", "Прізвище", "user123", "valid@email.com", "+380123456789", 25, "password", "Ім'я користувача обов'язкове і не може бути порожнім")]
    [InlineData("Ім'я", "", "user123", "valid@email.com", "+380123456789", 25, "password", "Прізвище користувача обов'язкове і не може бути порожнім")]
    [InlineData("Ім'я", "Прізвище", "", "valid@email.com", "+380123456789", 25, "password", "Логін користувача обов'язковий і не може бути порожнім")]
    [InlineData("Ім'я", "Прізвище", "user123", "wrongEmail", "+380123456789", 25, "password", "Невірний формат електронної пошти")]
    [InlineData("Ім'я", "Прізвище", "user123", "valid@email.com", "invalidPhone", 25, "password", "Невірний формат номеру телефону")]
    [InlineData("Ім'я", "Прізвище", "user123", "valid@email.com", "+380123456789", 0, "password", "Вік користувача повинен бути більше 0")]
    [InlineData("Ім'я", "Прізвище", "user123", "valid@email.com", "+380123456789", 25, "123", "Пароль повинен містити щонайменше 6 символів")]
    public void CreateUser_ShouldThrowValidationException_WhenModelIsInvalid(
     string firstName,
     string lastName,
     string login,
     string email,
     string phone,
     int age,
     string password,
     string expectedErrorMessagePart)
    {
        // Arrange
        var model = new BaseUserModel
        {
            FirstName = firstName,
            LastName = lastName,
            Login = login,
            Email = email,
            PhoneNumber = phone,
            Age = age,
            Password = password,
            SecretCode = "qwerty"
        };

        // порожній репозиторій користувачів та реалізаторів
        userRepositoryMock.GetQueryable().Returns(Enumerable.Empty<AbstractUser>().AsQueryable());
        codeRealizatorRepositoryMock.GetQueryable().Returns(Enumerable.Empty<AbstractSecretCodeRealizator>().AsQueryable());

        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() => manager.CreateUser(model));
        Assert.Contains(expectedErrorMessagePart, ex.Message);
    }

    // ====== Тести для AuthorizeUserCommand ======
    [Fact]
    public void AuthorizeUser_ShouldReturnUserModel_WhenParametrsAreCorrect()
    {
        // Arrange
        var user = Substitute.For<RegisteredUser>();  // для тестування просто зареєстрований юзер
        user.Login = "validLogin";
        user.PasswordHash = PasswordHasher.HashPassword("correctPassword");

        userRepositoryMock.GetQueryable().Returns(new[] { user }.AsQueryable());

        // Act
        var result = manager.AuthorizeUser(user.Login, "correctPassword");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(user.Login, result.Login);
        Assert.Equal((BusinessEnumInterfaceType)user.InterfaceType, result.InterfaceType);
    }

    [Fact]
    public void AuthorizeUser_ShouldThrowException_WhenLoginIsIncorrect()
    {
        // Arrange
        var user = Substitute.For<RegisteredUser>();
        user.Login = "validLogin";
        user.PasswordHash = PasswordHasher.HashPassword("correctPassword");

        userRepositoryMock.GetQueryable().Returns(new[] { user }.AsQueryable());

        // Act & Assert
        var ex = Assert.Throws<UnauthorizedAccessException>(() => manager.AuthorizeUser("wrongLogin", "correctPassword"));
        Assert.Contains("Невірний логін або пароль", ex.Message);
    }

    [Fact]
    public void AuthorizeUser_ShouldThrowException_WhenPasswordIsIncorrect()
    {
        // Arrange
        var user = Substitute.For<RegisteredUser>();
        user.Login = "validLogin";
        user.PasswordHash = PasswordHasher.HashPassword("correctPassword");

        userRepositoryMock.GetQueryable().Returns(new[] { user }.AsQueryable());

        // Act & Assert
        var ex = Assert.Throws<UnauthorizedAccessException>(() => manager.AuthorizeUser("validLogin", "wrongPassword"));
        Assert.Contains("Невірний логін або пароль", ex.Message);
    }

    [Theory]
    [InlineData(null, "password", typeof(ArgumentNullException))]
    [InlineData("login", null, typeof(ArgumentNullException))]
    [InlineData("", "password", typeof(ArgumentException))]
    [InlineData("login", "", typeof(ArgumentException))]
    public void AuthorizeUser_ShouldThrowException_WhenLoginOrPasswordIsEmpty(string login, string password, Type expectedExceptionType)
    {
        // Act & Assert
        Assert.Throws(expectedExceptionType, () => manager.AuthorizeUser(login, password));
    }
}
