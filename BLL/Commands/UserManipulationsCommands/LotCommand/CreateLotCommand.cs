using AutoMapper;
using BLL.EntityBLLModels;
using DAL.Data;
using DAL.Entities;

namespace BLL.Commands.UserManipulationsCommands;

public class CreateLotCommand : AbstrCommandWithDA<bool>
{
    private readonly AuctionLotModel lotModel;

    public CreateLotCommand(AuctionLotModel lotModel, IUnitOfWork unitOfWork, IMapper mapper)
        : base(unitOfWork, mapper)
    {
        ArgumentNullException.ThrowIfNull(lotModel, nameof(lotModel));
        this.lotModel = lotModel;

        PreValidateModel();
    }

    public override string Name => "Створення лоту";

    public override bool Execute()
    {
        // мапимо модель лоту
        var newLot = mapper.Map<AuctionLot>(lotModel);

        // перевіряємо чи існують об'єкти в БД
        var existingOwner = dAPoint.RegisteredUserRepository.GetById(newLot.OwnerId);

        var existingCategory = dAPoint.CategoryRepository.GetById(newLot.Category.Id);

        ValidateModel(existingOwner, existingCategory);

        // додатково для безпеки встановлюємо зв'язки якщо фінальна валідація пройдена
        newLot.Owner = existingOwner;
        newLot.OwnerId = existingOwner.Id;

        newLot.Category = existingCategory;
        newLot.CategoryId = existingCategory.Id;

        dAPoint.AuctionLotRepository.Add(newLot);
        dAPoint.Save();

        LogAction($"Створено новий лот: {lotModel.Title}");
        return true;
    }

    private void PreValidateModel()
    {
        ArgumentNullException.ThrowIfNull(lotModel.Owner, nameof(lotModel.Owner));

        if (lotModel.StartPrice <= 0)
        {
            throw new ArgumentException("Стартова ціна має бути більшою за 0");
        }

        if (lotModel.DurationDays <= 0)
        {
            throw new ArgumentException("Тривалість має бути більшою за 0 днів");
        }
    }

    private void ValidateModel(AbstractUser existingOwner, Category existingCategory)
    {
        // якщо не існує — додаємо
        if (existingOwner == null)
        {
            throw new ArgumentNullException("Виконавець не співпадає з бд");
        }

        // категорія не критична - додаємо якщо в бд нема
        if (existingCategory == null)
        {
            existingCategory = mapper.Map<Category>(lotModel.Category);
            dAPoint.CategoryRepository.Add(existingCategory);
            dAPoint.Save();
        }
    }
}
