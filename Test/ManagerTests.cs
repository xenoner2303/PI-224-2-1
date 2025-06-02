using AutoFixture;
using AutoFixture.AutoNSubstitute;
using AutoMapper;
using BLL.Commands.ManagerManipulationCommands;
using BLL.EntityBLLModels;
using DAL.Data;
using DAL.Entities;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Test
{
    public class ManagerTests : CommandTestBase
    {
        public ManagerTests() : base()
        {
            // Уникнення проблем рекурсії в CategoryModel
            fixture.Behaviors.Add(new OmitOnRecursionBehavior(1));
        }
        private AuctionLot CreateAuctionLotModel(int id)
        {
            var owner = new RegisteredUser
            {
                Id = 1,
                FirstName = "John",
                LastName = "Doe",
                Login = "johndoe"
            };

            return new AuctionLot
            {
                Id = id,
                Status = EnumLotStatuses.Active,
                Owner = owner,
                StartTime = null
            };
        }

        [Fact]
        public void CreateCategoryCommand_ShouldAddCategory_WhenValidCategoryModel()
        {
            // Arrange
            var categoryModel = fixture.Create<CategoryModel>();
            categoryModel.ParentId = null;

            var command = new CreateCategoryCommand(categoryModel, unitOfWorkMock, mapper);

            // Act
            var result = command.Execute();

            // Assert
            Assert.True(result);
            unitOfWorkMock.CategoryRepository.Received(1).Add(Arg.Is<Category>(c => c.Name == categoryModel.Name && c.ParentId == null));
            unitOfWorkMock.Received(2).Save();
        }

        [Fact]
        public void CreateCategoryCommand_ShouldReturnFalse_WhenCategoryNameInvalid()
        {
            // Arrange
            var categoryModel = fixture.Build<CategoryModel>()
                .With(c => c.Name, "a") // Назва менше 4 символів - invalid
                .Create();

            var command = new CreateCategoryCommand(categoryModel, unitOfWorkMock, mapper);

            // Act
            var result = command.Execute();

            // Assert
            Assert.False(result);
            unitOfWorkMock.CategoryRepository.DidNotReceive().Add(Arg.Any<Category>());
            unitOfWorkMock.DidNotReceive().Save();
        }

        [Fact]
        public void DeleteCategoryCommand_ShouldRemoveCategoryAndSubcategories_WhenCategoryExists()
        {
            // Arrange
            int categoryId = 1;

            var categoryToDelete = new Category { Id = categoryId, Name = "TestCat" };
            var subcategories = new List<Category>
    {
        new Category { Id = 2, ParentId = categoryId, Name = "Sub1" },
        new Category { Id = 3, ParentId = categoryId, Name = "Sub2" }
    };

            // Мок повинен повертати і категорію, і підкатегорії
            unitOfWorkMock.CategoryRepository.GetAll().Returns(new List<Category> { categoryToDelete }.Concat(subcategories).ToList());

            var command = new DeleteCategoryCommand(categoryId, unitOfWorkMock, mapper);

            // Act
            var result = command.Execute();

            // Assert
            Assert.True(result);

            // Перевіряємо, що Remove викликали з правильними Id для підкатегорій
            foreach (var sub in subcategories)
            {
                unitOfWorkMock.CategoryRepository.Received(1).Remove(sub.Id);
            }

            // Перевіряємо, що Remove викликали для головної категорії
            unitOfWorkMock.CategoryRepository.Received(1).Remove(categoryId);

            unitOfWorkMock.Received(4).Save();
        }


        [Fact]
        public void DeleteCategoryCommand_ExecuteWithParentId_ShouldRemoveSubcategoryAndUpdateParent()
        {
            // Arrange
            int categoryId = 2;
            int parentCategoryId = 1;

            var categoryToDelete = new Category { Id = categoryId, Name = "SubCat", ParentId = parentCategoryId };
            var parentCategory = new Category
            {
                Id = parentCategoryId,
                Name = "ParentCat",
                Subcategories = new List<Category> { categoryToDelete }
            };

            unitOfWorkMock.CategoryRepository.GetAll().Returns(new List<Category> { categoryToDelete, parentCategory });


            var command = new DeleteCategoryCommand(categoryId, unitOfWorkMock, mapper);

            // Act
            var result = command.Execute();

            // Assert
            Assert.True(result);
            Assert.DoesNotContain(categoryToDelete, parentCategory.Subcategories);
            unitOfWorkMock.CategoryRepository.Received(1).Update(parentCategory);
            unitOfWorkMock.CategoryRepository.Received(1).Remove(categoryId);
            unitOfWorkMock.Received(3).Save();
        }

        [Fact]
        public void DeleteCategoryCommand_ExecuteWithParentId_ShouldThrowIfParentNotFound()
        {
            // Arrange
            int categoryId = 2;
            int parentCategoryId = 99; // Неіснуючий

            var categoryToDelete = new Category { Id = categoryId, Name = "SubCat", ParentId = parentCategoryId };

            unitOfWorkMock.CategoryRepository.GetAll().Returns(new List<Category> { categoryToDelete });


            var command = new DeleteCategoryCommand(categoryId, unitOfWorkMock, mapper);

            // Act & Assert
            var ex = Assert.Throws<ArgumentNullException>(() => command.Execute());
            Assert.Equal("parentCategoryId", ex.ParamName);
        }

        [Fact]
        public void ReadCategoryCommand_ShouldReturnCategory_WhenExists()
        {
            // Arrange
            int categoryId = 1;
            var category = new Category { Id = categoryId, Name = "Cat1" };

            unitOfWorkMock.CategoryRepository.GetById(categoryId).Returns(category);

            var command = new ReadCategoryCommand(categoryId, unitOfWorkMock, mapper);

            // Act
            var result = command.Execute();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(category.Name, result.Name);
        }

        [Fact]
        public void ReadCategoryCommand_ShouldReturnNull_WhenCategoryNotFound()
        {
            // Arrange
            int categoryId = 999;

            unitOfWorkMock.CategoryRepository.GetById(categoryId).Returns((Category)null);

            var command = new ReadCategoryCommand(categoryId, unitOfWorkMock, mapper);

            // Act
            var result = command.Execute();

            // Assert
            Assert.Null(result);
        }


        [Fact]
        public void StopLotCommand_ShouldSetCompletedStatusAndEndTime()
        {
            // Arrange
            int lotId = 1;
            var lot = CreateAuctionLotModel(lotId);

            var auctionLots = new List<AuctionLot> { lot };

            var lotEntities = mapper.Map<List<AuctionLot>>(auctionLots);
            // Репозиторій повертає модель, а не ентіті
            unitOfWorkMock.AuctionLotRepository.GetAll().Returns(lotEntities);

            var command = new StopLotCommand(lotId, unitOfWorkMock, mapper);

            // Act
            var result = command.Execute();

            // Assert
            Assert.True(result);
            Assert.Equal(EnumLotStatuses.Completed, lot.Status);
            Assert.NotNull(lot.EndTime);

            unitOfWorkMock.AuctionLotRepository.Received(1).Update(lot);
            unitOfWorkMock.Received(2).Save();
        }

        [Fact]
        public void RejectLotCommand_ShouldSetRejectedStatus()
        {
            // Arrange
            int lotId = 2;
            var lot = CreateAuctionLotModel(lotId);

            var auctionLots = new List<AuctionLot> { lot };
            unitOfWorkMock.AuctionLotRepository.GetAll().Returns(auctionLots);

            var command = new RejectLotCommand(lotId, unitOfWorkMock, mapper);

            // Act
            var result = command.Execute();

            // Assert
            Assert.True(result);
            Assert.Equal(EnumLotStatuses.Rejected, lot.Status);

            unitOfWorkMock.AuctionLotRepository.Received(1).Update(lot);
            unitOfWorkMock.Received(2).Save();
        }

        [Fact]
        public void ApproveLotCommand_ShouldSetActiveStatusAndStartTime()
        {
            // Arrange
            int lotId = 3;
            var lot = CreateAuctionLotModel(lotId);
            var auctionLots = new List<AuctionLot> { lot };

            unitOfWorkMock.AuctionLotRepository.GetAll().Returns(auctionLots);

            var command = new ApproveLotCommand(lotId, unitOfWorkMock, mapper);

            // Act
            var result = command.Execute();

            // Assert
            Assert.True(result);
            Assert.Equal(EnumLotStatuses.Active, lot.Status);
            Assert.NotNull(lot.StartTime);

            unitOfWorkMock.AuctionLotRepository.Received(1).Update(lot);
            unitOfWorkMock.Received(2).Save();
        }
    }
}