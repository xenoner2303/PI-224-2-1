using Xunit;
using AutoFixture;
using AutoFixture.AutoNSubstitute;
using NSubstitute;
using AutoMapper;
using BLL.Commands.ManagerManipulationCommands;
using DAL.Data;
using System.Collections.Generic;
using System.Linq;
using DAL.Entities;

namespace Test
{
    public class CategoryCommandTests
    {
        private readonly IFixture _fixture;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGenericRepository<Category> _categoryRepository;
        private readonly IGenericRepository<AuctionLot> _lotRepository;

        public CategoryCommandTests()
        {
            _fixture = new Fixture().Customize(new AutoNSubstituteCustomization());

            _mapper = _fixture.Create<IMapper>();
            _categoryRepository = _fixture.Freeze<IGenericRepository<Category>>();
            _lotRepository = _fixture.Freeze<IGenericRepository<AuctionLot>>();
            _unitOfWork = _fixture.Create<IUnitOfWork>();
        }

        [Fact]
        public void CreateCategoryCommand_Should_Add_Category_Without_Parent()
        {
            // Arrange
            var categoryName = _fixture.Create<string>();
            var command = new CreateCategoryCommand(categoryName, _unitOfWork, _mapper);

            // Act
            var result = command.Execute();

            // Assert
            Assert.True(result);
            _categoryRepository.Received().Add(Arg.Is<Category>(c => c.Name == categoryName && c.ParentId == null));
            _unitOfWork.Received().Save();
        }

        [Fact]
        public void CreateCategoryCommand_Should_Add_Subcategory()
        {
            // Arrange
            var categoryName = _fixture.Create<string>();
            var parentCategoryId = _fixture.Create<int>();

            var parentCategory = new Category
            {
                Id = parentCategoryId,
                Name = "Parent",
                Subcategories = new List<Category>()
            };

            _categoryRepository.GetAll().Returns(new List<Category> { parentCategory });

            var command = new CreateCategoryCommand(categoryName, _unitOfWork, _mapper);

            // Act
            var result = command.Execute(parentCategoryId);

            // Assert
            Assert.True(result);
            _categoryRepository.Received().Add(Arg.Is<Category>(c => c.Name == categoryName && c.ParentId == parentCategoryId));
            _categoryRepository.Received().Update(parentCategory);
            _unitOfWork.Received(2).Save();
        }

        [Fact]
        public void DeleteCategoryCommand_Should_Remove_Category_And_Subcategories()
        {
            // Arrange
            var categoryId = _fixture.Create<int>();
            var subcategories = _fixture.Build<Category>()
                .With(c => c.ParentId, categoryId)
                .CreateMany(2)
                .ToList();

            var mainCategory = _fixture.Build<Category>()
                .With(c => c.Id, categoryId)
                .With(c => c.Name, "MainCategory")
                .Create();

            var allCategories = subcategories.Append(mainCategory).ToList();

            _categoryRepository.GetAll().Returns(allCategories);

            var command = new DeleteCategoryCommand(categoryId, _unitOfWork, _mapper);

            // Act
            var result = command.Execute();

            // Assert
            Assert.True(result);
            foreach (var sub in subcategories)
            {
                _categoryRepository.Received().Remove(sub.Id);
            }

            _categoryRepository.Received().Remove(categoryId);
            _unitOfWork.Received(subcategories.Count + 1).Save();
        }

        [Fact]
        public void ReadCategoryCommand_Should_Return_Category_If_Exists()
        {
            // Arrange
            var category = _fixture.Create<Category>();
            _categoryRepository.GetById(category.Id).Returns(category);

            var command = new ReadCategoryCommand(category.Id, _unitOfWork, _mapper);

            // Act
            var result = command.Execute();

            // Assert
            Assert.Equal(category, result);
        }

        [Fact]
        public void ReadCategoryCommand_Should_Return_Null_If_Not_Found()
        {
            // Arrange
            var id = _fixture.Create<int>();
            _categoryRepository.GetById(id).Returns((Category)null);

            var command = new ReadCategoryCommand(id, _unitOfWork, _mapper);

            // Act
            var result = command.Execute();

            // Assert
            Assert.Null(result);
        }
        [Fact]
        public void ApproveLotCommand_Should_Set_Status_And_StartTime()
        {
            // Arrange
            var lotId = _fixture.Create<int>();
            var lot = new AuctionLot { Id = lotId};

            _lotRepository.GetAll().Returns(new List<AuctionLot> { lot });

            var command = new ApproveLotCommand(lotId, _unitOfWork, _mapper);

            // Act
            var result = command.Execute();

            // Assert
            Assert.True(result);
            Assert.Equal(EnumLotStatuses.Active, lot.Status);
            Assert.True(lot.StartTime <= DateTime.Now);
            _lotRepository.Received().Update(lot);
            _unitOfWork.Received().Save();
        }

        [Fact]
        public void RejectLotCommand_Should_Set_Status_To_Rejected()
        {
            // Arrange
            var lotId = _fixture.Create<int>();
            var lot = new AuctionLot { Id = lotId};

            _lotRepository.GetAll().Returns(new List<AuctionLot> { lot });

            var command = new RejectLotCommand(lotId, _unitOfWork, _mapper);

            // Act
            var result = command.Execute();

            // Assert
            Assert.True(result);
            Assert.Equal(EnumLotStatuses.Rejected, lot.Status);
            _lotRepository.Received().Update(lot);
            _unitOfWork.Received().Save();
        }

        [Fact]
        public void StopLotCommand_Should_Set_Status_To_Completed_And_EndTime()
        {
            // Arrange
            var lotId = _fixture.Create<int>();
            var lot = new AuctionLot { Id = lotId};

            _lotRepository.GetAll().Returns(new List<AuctionLot> { lot });

            var command = new StopLotCommand(lotId, _unitOfWork, _mapper);

            // Act
            var result = command.Execute();

            // Assert
            Assert.True(result);
            Assert.Equal(EnumLotStatuses.Completed, lot.Status);
            Assert.True(lot.EndTime <= DateTime.Now);
            _lotRepository.Received().Update(lot);
            _unitOfWork.Received().Save();
        }

        [Fact]
        public void ApproveLotCommand_Should_Throw_If_Lot_Not_Found()
        {
            // Arrange
            var lotId = _fixture.Create<int>();
            _lotRepository.GetAll().Returns(Enumerable.Empty<AuctionLot>());
            var command = new ApproveLotCommand(lotId, _unitOfWork, _mapper);

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => command.Execute());
        }

        [Fact]
        public void RejectLotCommand_Should_Throw_If_Lot_Not_Found()
        {
            // Arrange
            var lotId = _fixture.Create<int>();
            _lotRepository.GetAll().Returns(Enumerable.Empty<AuctionLot>());
            var command = new RejectLotCommand(lotId, _unitOfWork, _mapper);

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => command.Execute());
        }

        [Fact]
        public void StopLotCommand_Should_Throw_If_Lot_Not_Found()
        {
            // Arrange
            var lotId = _fixture.Create<int>();
            _lotRepository.GetAll().Returns(Enumerable.Empty<AuctionLot>());
            var command = new StopLotCommand(lotId, _unitOfWork, _mapper);

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => command.Execute());
        }
    }
}