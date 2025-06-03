using AutoFixture;
using AutoMapper;
using NSubstitute;
using Xunit;
using BLL.Commands.ManagerManipulationCommands;
using BLL.EntityBLLModels;
using DAL.Data;
using DAL.Entities;

namespace Test
{
    public class ManagerCommandsManagerTests : CommandTestBase
    {
        private readonly IGenericRepository<Category> _categoryRepositoryMock;
        private readonly IGenericRepository<AuctionLot> _auctionLotRepositoryMock;
        private readonly ManagerCommandManager _manager;
        private readonly IMapper _mapper;

        public ManagerCommandsManagerTests()
        {
            // Заморожуємо репозиторій категорій
            _categoryRepositoryMock = fixture.Freeze<IGenericRepository<Category>>();
            _auctionLotRepositoryMock = fixture.Freeze<IGenericRepository<AuctionLot>>();
            unitOfWorkMock.CategoryRepository.Returns(_categoryRepositoryMock);
            unitOfWorkMock.AuctionLotRepository.Returns(_auctionLotRepositoryMock);
            _mapper = Substitute.For<IMapper>();
            _manager = new ManagerCommandManager(unitOfWorkMock, _mapper);
            _categoryRepositoryMock.GetAll().Returns(new List<Category>());
        }

        [Fact]
        public void CreateCategory_ShouldReturnTrue_WhenCategoryIsValid()
        {
            // Arrange
            var categoryModel = fixture.Build<CategoryModel>()
                .Without(c => c.Id)
                .With(c => c.Name, "New Category")
                .Without(c => c.ParentId)
                .Create();

            _categoryRepositoryMock.When(x => x.Add(Arg.Any<Category>()));

            // Act
            var result = _manager.CreateCategory(categoryModel);

            // Assert
            Assert.True(result);
            _categoryRepositoryMock.Received(1).Add(Arg.Any<Category>());
            unitOfWorkMock.Received(2).Save(); // 2 рази через LogAction
        }

        [Fact]
        public void CreateCategory_ShouldThrowArgumentNullException_WhenCategoryModelIsNull()
        {
            // Arrange
            CategoryModel? categoryModel = null;

            // Act & Assert
            var ex = Assert.Throws<ArgumentNullException>(() => _manager.CreateCategory(categoryModel!));

            // Перевірка, що виняток стосується параметра category
            Assert.Equal("category", ex.ParamName);

            // Або перевірка частини повідомлення, яке генерує ThrowIfNull
            Assert.Contains("Value cannot be null", ex.Message);

            _categoryRepositoryMock.DidNotReceive().Add(Arg.Any<Category>());
            unitOfWorkMock.DidNotReceive().Save();
        }

        [Fact]
        public void CreateCategory_ShouldThrowArgumentException_WhenCategoryNameIsNullOrEmpty()
        {
            // Arrange
            var categoryModel = fixture.Build<CategoryModel>()
                .With(c => c.Name, string.Empty)
                .Without(c => c.ParentId)
                .Create();

            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() => _manager.CreateCategory(categoryModel));

            Assert.Contains("Назва категорії не може бути порожньою", ex.Message);

            _categoryRepositoryMock.DidNotReceive().Add(Arg.Any<Category>());
            unitOfWorkMock.DidNotReceive().Save();
        }

        [Fact]
        public void CreateCategory_ShouldThrowInvalidOperationException_WhenCategoryAlreadyExists()
        {
            // Arrange
            var categoryName = "Category";

            // Щоб AutoFixture не ліз у навігаційні властивості й не створював AuctionLot
            fixture.Customize<Category>(c => c.OmitAutoProperties());

            // Існуюча категорія з такою самою назвою
            var existingCategory = fixture.Build<Category>()
                .With(c => c.Name, categoryName)
                .Without(c => c.Lots)
                .Without(c => c.Parent)
                .Create();

            // Репозиторій має повернути список, у якому вже є ця категорія
            _categoryRepositoryMock.GetAll()
                .Returns(new List<Category> { existingCategory });

            // Нова модель із дублікатом назви
            var categoryModel = fixture.Build<CategoryModel>()
                .With(c => c.Name, categoryName)
                .Without(c => c.ParentId)
                .Create();

            // Act & Assert
            var ex = Assert.Throws<InvalidOperationException>(() => _manager.CreateCategory(categoryModel));
            Assert.Contains("Категорія з такою назвою вже існує", ex.Message);

            // Переконуємося, що нічого не додається й не зберігається
            _categoryRepositoryMock.DidNotReceive().Add(Arg.Any<Category>());
            unitOfWorkMock.DidNotReceive().Save();
        }

        [Fact]
        public void CreateCategory_ShouldTrimCategoryNameBeforeAdding()
        {
            // Arrange
            var categoryModel = fixture.Build<CategoryModel>()
                .With(c => c.Name, "  Category Name  ")
                .Without(c => c.ParentId)
                .Create();

            Category? addedCategory = null;

            // Налаштовуємо мок репозиторію для відлову доданого об'єкта
            _categoryRepositoryMock
                .When(x => x.Add(Arg.Any<Category>()))
                .Do(ci => addedCategory = ci.Arg<Category>());

            // Налаштовуємо мапер, щоб повертав об'єкт Category з потрібним ім'ям
            _mapper
                .Map<Category>(Arg.Any<CategoryModel>())
                .Returns(ci => new Category { Name = ((CategoryModel)ci[0]).Name });

            // Налаштовуємо Save() - щоб не кидав виключень і працював як пустий метод
            unitOfWorkMock
                .When(x => x.Save())
                .Do(_ => { });

            // Забезпечуємо, що UnitOfWork повертає наш мок репозиторію
            unitOfWorkMock.CategoryRepository.Returns(_categoryRepositoryMock);

            // Act
            var result = _manager.CreateCategory(categoryModel);

            // Assert
            _categoryRepositoryMock.Received(1).Add(Arg.Any<Category>());
            Assert.True(result);
            Assert.NotNull(addedCategory);
            Assert.Equal("Category Name", addedCategory!.Name); // перевірка обрізаних пробілів
        }

        //ТЕСТИ ВИДАЛЕННЯ КАТЕГОРІЇ

        [Fact]
        public void DeleteCategory_ShouldReturnTrue_WhenCategoryExists()
        {
            // Arrange
            var category = fixture.Build<Category>()
                .With(c => c.Id, 1)
                .With(c => c.Name, "ToDelete")
                .Without(c => c.Lots)
                .Create();

            _categoryRepositoryMock.GetById(1).Returns(category);

            // Act
            var result = _manager.DeleteCategory(1);

            // Assert
            Assert.True(result);
            _categoryRepositoryMock.Received(1).Remove(1);
            unitOfWorkMock.Received(2).Save(); // 2 рази через LogAction
        }

        [Fact]
        public void DeleteCategory_ShouldThrowInvalidOperationException_WhenCategoryDoesNotExist()
        {
            // Arrange
            int categoryId = 999;
            _categoryRepositoryMock.GetById(categoryId).Returns((Category?)null);

            // Act & Assert
            var ex = Assert.Throws<InvalidOperationException>(() => _manager.DeleteCategory(categoryId));
            Assert.Contains("Не вдалося видалити категорію", ex.Message);

            _categoryRepositoryMock.DidNotReceive().Remove(Arg.Any<int>());
            unitOfWorkMock.Received(1).Save();
        }

        [Fact]
        public void DeleteCategory_ShouldThrowArgumentException_WhenIdIsInvalid()
        {
            // Arrange
            var invalidId = 0;

            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() => _manager.DeleteCategory(invalidId));
            Assert.Contains("Id категорії повинне бути більше 0", ex.Message);

            _categoryRepositoryMock.DidNotReceive().Remove(Arg.Any<int>());
            unitOfWorkMock.DidNotReceive().Save();
        }

        [Fact]
        public void DeleteCategory_ShouldCallRemoveExactlyOnce_WhenCategoryExists()
        {
            // Arrange
            var category = fixture.Build<Category>()
                .With(c => c.Id, 5)
                .With(c => c.Name, "TestCat")
                .Without(c => c.Lots)
                .Create();

            _categoryRepositoryMock.GetById(5).Returns(category);

            // Act
            var result = _manager.DeleteCategory(5);

            // Assert
            Assert.True(result);
            _categoryRepositoryMock.Received(1).Remove(5);
        }

        [Fact]
        public void DeleteCategory_ShouldNotCallRemove_WhenCategoryNotFound()
        {
            // Arrange
            int categoryId = 12345;
            _categoryRepositoryMock.GetById(categoryId).Returns((Category?)null);

            // Act & Assert
            var ex = Assert.Throws<InvalidOperationException>(() => _manager.DeleteCategory(categoryId));
            Assert.Contains("Не вдалося видалити категорію", ex.Message);

            _categoryRepositoryMock.DidNotReceive().Remove(Arg.Any<int>());
        }

        // ТЕСТИ ЧИТАННЯ КАТЕГОРІЇ

        [Fact]
        public void ReadCategory_ShouldReturnCategoryModel_WhenCategoryExists()
        {
            // Arrange
            int categoryId = 1;

            var categoryEntity = fixture.Build<Category>()
                .With(c => c.Id, categoryId)
                .Without(c => c.Lots)
                .Create();

            _categoryRepositoryMock.GetById(categoryId).Returns(categoryEntity);
            unitOfWorkMock.CategoryRepository.Returns(_categoryRepositoryMock);

            _mapper.Map<CategoryModel>(Arg.Any<Category>()).Returns(ci =>
            {
                var category = ci.Arg<Category>();
                return new CategoryModel
                {
                    Id = category.Id,
                    Name = category.Name,
                    ParentId = category.ParentId
                };
            });

            // Act
            var result = _manager.ReadCategory(categoryId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(categoryEntity.Id, result.Id);
            Assert.Equal(categoryEntity.Name, result.Name);
        }

        [Fact]
        public void ReadCategory_ShouldThrowException_WhenCategoryNotFound()
        {
            // Arrange
            var categoryId = 999;
            _categoryRepositoryMock.GetById(categoryId).Returns((Category?)null);

            // Act & Assert
            var ex = Assert.Throws<InvalidOperationException>(() => _manager.ReadCategory(categoryId));
            Assert.Equal("Не вдалося прочитати категорію", ex.Message);
        }


        [Theory]
        [InlineData(0)]
        [InlineData(-5)]
        public void ReadCategory_ShouldThrowArgumentException_WhenCategoryIdIsInvalid(int invalidId)
        {
            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() => _manager.ReadCategory(invalidId));
            Assert.Contains("Id категорії повинне бути більше 0", ex.Message);
        }

        [Fact]
        public void ReadCategory_ShouldCallGetById_OnceWithCorrectId()
        {
            // Arrange
            var categoryId = 2;
            var category = fixture.Build<Category>().Without(c => c.Lots).Create();

            _categoryRepositoryMock.GetById(categoryId).Returns(category);
            unitOfWorkMock.CategoryRepository.Returns(_categoryRepositoryMock);

            _mapper.Map<CategoryModel>(Arg.Any<Category>()).Returns(ci =>
            {
                var cat = ci.Arg<Category>();
                return new CategoryModel
                {
                    Id = cat.Id,
                    Name = cat.Name,
                    ParentId = cat.ParentId
                };
            });

            // Act
            _manager.ReadCategory(categoryId);

            // Assert
            _categoryRepositoryMock.Received(1).GetById(categoryId);
        }

        [Fact]
        public void ReadCategory_ShouldMapCategory_WhenFound()
        {
            // Arrange
            var categoryId = 5;
            var category = fixture.Build<Category>()
                .With(c => c.Id, categoryId)
                .Without(c => c.Lots)
                .Create();

            var expectedModel = fixture.Build<CategoryModel>()
                .With(c => c.Id, categoryId)
                .Create();

            _categoryRepositoryMock.GetById(categoryId).Returns(category);

            _mapper.Map<CategoryModel>(Arg.Is<Category>(c => c.Id == categoryId)).Returns(expectedModel);

            // Act
            var result = _manager.ReadCategory(categoryId);

            // Assert
            _mapper.Received(1).Map<CategoryModel>(Arg.Is<Category>(c => c.Id == categoryId));
            Assert.Equal(expectedModel.Id, result.Id);
        }

        // ТЕСТИ ДЛЯ ПІДТВЕРДЖЕННЯ ЛОТУ

        [Fact]
        public void ApproveLot_ShouldReturnTrue_WhenLotExists()
        {
            // Arrange
            int lotId = 1;
            var owner = new RegisteredUser { FirstName = "John", LastName = "Doe" };
            var auctionLot = new AuctionLot
            {
                Id = lotId,
                Status = DAL.Entities.EnumLotStatuses.Pending,
                Owner = owner
            };

            // Підготовка IQueryable для Include
            var lotsQueryable = new List<AuctionLot> { auctionLot }.AsQueryable();
            _auctionLotRepositoryMock.GetQueryable().Returns(lotsQueryable);

            // Act
            var result = _manager.ApproveLot(lotId);

            // Assert
            Assert.True(result);
            Assert.Equal(DAL.Entities.EnumLotStatuses.Active, auctionLot.Status);
            Assert.NotNull(auctionLot.StartTime);

            _auctionLotRepositoryMock.Received(1).Update(auctionLot);
            unitOfWorkMock.Received(2).Save(); // 2 рази через LogAction
        }

        [Fact]
        public void ApproveLot_ShouldThrowArgumentException_WhenLotIdIsZeroOrLess()
        {
            // Arrange
            int invalidLotId = 0;

            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() => _manager.ApproveLot(invalidLotId));
            Assert.Contains("Id лоту повинне бути більше 0", ex.Message);
        }

        [Fact]
        public void ApproveLot_ShouldThrowInvalidOperationException_WhenLotDoesNotExist()
        {
            // Arrange
            int lotId = 100;
            var emptyQueryable = new List<AuctionLot>().AsQueryable();
            _auctionLotRepositoryMock.GetQueryable().Returns(emptyQueryable);

            // Act & Assert
            var ex = Assert.Throws<InvalidOperationException>(() => _manager.ApproveLot(lotId));
            Assert.Contains($"Лот з ID {lotId} не знайдено.", ex.Message);

            _auctionLotRepositoryMock.DidNotReceive().Update(Arg.Any<AuctionLot>());
            unitOfWorkMock.DidNotReceive().Save();
        }


        // ТЕСТИ ДЛЯ ВІДХИЛЕННЯ ЛОТУ
        [Fact]
        public void RejectLot_ShouldReturnTrue_WhenLotIsValid()
        {
            // Arrange
            var lotId = 1;
            var lotModel = fixture.Build<AuctionLotModel>()
                .With(l => l.Id, lotId)
                .With(l => l.RejectionReason, "Порушення правил")
                .Create();

            var auctionLotEntity = new AuctionLot
            {
                Id = lotId,
                Status = EnumLotStatuses.Active,
                EndTime = null,
                RejectionReason = null
            };

            // Повертаємо лот при GetById
            _auctionLotRepositoryMock.GetById(lotId).Returns(auctionLotEntity);

            // Act
            var result = _manager.RejectLot(lotModel);

            // Assert
            Assert.True(result);
            Assert.Equal(EnumLotStatuses.Rejected, auctionLotEntity.Status);
            Assert.NotNull(auctionLotEntity.EndTime);
            Assert.Equal(lotModel.RejectionReason, auctionLotEntity.RejectionReason);

            _auctionLotRepositoryMock.Received(1).Update(auctionLotEntity);
            unitOfWorkMock.Received(2).Save(); // 2 рази через LogAction
        }

        [Fact]
        public void RejectLot_ShouldThrowInvalidOperationException_WhenLotNotFound()
        {
            // Arrange
            var lotModel = fixture.Build<AuctionLotModel>()
                .With(l => l.Id, 999)
                .With(l => l.RejectionReason, "Причина")
                .Create();

            // Повертаємо null при GetById
            _auctionLotRepositoryMock.GetById(lotModel.Id).Returns((AuctionLot?)null);

            // Act & Assert
            var ex = Assert.Throws<InvalidOperationException>(() => _manager.RejectLot(lotModel));
            Assert.Contains("Лот не знайдено", ex.Message);

            _auctionLotRepositoryMock.DidNotReceive().Update(Arg.Any<AuctionLot>());
            unitOfWorkMock.DidNotReceive().Save();
        }

        [Fact]
        public void RejectLot_ShouldThrowArgumentNullException_WhenLotModelIsNull()
        {
            // Act & Assert
            var ex = Assert.Throws<ArgumentNullException>(() => _manager.RejectLot(null!));
            Assert.Contains("Value cannot be null", ex.Message);
        }

        // ТЕСТИ ДЛЯ ЗУПИНКИ ЛОТУ
        [Fact]
        public void StopLot_ShouldReturnTrue_WhenLotExists()
        {
            // Arrange
            var lotId = 1;
            var auctionLotEntity = new AuctionLot
            {
                Id = lotId,
                Status = EnumLotStatuses.Active,
                Owner = new RegisteredUser { FirstName = "Іван", LastName = "Петров" },
                EndTime = null
            };

            // Повертаємо лот при запиті через GetQueryable()
            var queryable = new List<AuctionLot> { auctionLotEntity }.AsQueryable();
            _auctionLotRepositoryMock.GetQueryable().Returns(queryable);

            // Act
            var result = _manager.StopLot(lotId);

            // Assert
            Assert.True(result);
            Assert.Equal(EnumLotStatuses.Completed, auctionLotEntity.Status);
            Assert.NotNull(auctionLotEntity.EndTime);

            _auctionLotRepositoryMock.Received(1).Update(auctionLotEntity);
            unitOfWorkMock.Received(2).Save(); // 2 рази через LogAction
        }

        [Fact]
        public void StopLot_ShouldThrowInvalidOperationException_WhenLotNotFound()
        {
            // Arrange
            var lotId = 999;

            // Повертаємо пустий список — лота немає
            _auctionLotRepositoryMock.GetQueryable().Returns(new List<AuctionLot>().AsQueryable());

            // Act & Assert
            var ex = Assert.Throws<InvalidOperationException>(() => _manager.StopLot(lotId));
            Assert.Contains($"Лот з ID {lotId} не знайдено.", ex.Message);

            _auctionLotRepositoryMock.DidNotReceive().Update(Arg.Any<AuctionLot>());
            unitOfWorkMock.DidNotReceive().Save();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-5)]
        public void StopLot_ShouldThrowArgumentException_WhenLotIdIsInvalid(int invalidLotId)
        {
            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() => _manager.StopLot(invalidLotId));
            Assert.Contains("Id лоту повинне бути більше 0", ex.Message);
        }
    }
}
