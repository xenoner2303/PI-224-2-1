using AutoMapper;
using BLL.EntityBLLModels;
using DAL.Data;
using BLL.EntityBLLModels;
using Microsoft.EntityFrameworkCore;

namespace BLL.Commands.CommonCommands;

internal class SearchLotsCommand : AbstrCommandWithDA<List<AuctionLotModel>>
{
    IMapper _mapper;
    private readonly string? keyword;
    private readonly int? categoryId;

    public SearchLotsCommand(string? keyword, int? categoryId, IUnitOfWork unitOfWork, IMapper mapper)
        : base(unitOfWork, mapper)
    {
        _mapper = mapper;
        this.keyword = keyword;
        this.categoryId = categoryId;
    }

    public override string Name => "Пошук лотів";

    public override List<AuctionLotModel> Execute()
    {
        var query = dAPoint.AuctionLotRepository.GetQueryable()
            .Include(lot => lot.Owner)
            .Include(lot => lot.Bids)
            .Include(lot => lot.Category)
            .Where(lot => lot.Status != EnumLotStatuses.Pending &&
                        lot.Status != EnumLotStatuses.Rejected)
            .ToList();

        // пошук по ключовому слову - тексту в усіх string полях класу AuctionLot
        if (!string.IsNullOrWhiteSpace(keyword))
        {
            var stringProperties = typeof(AuctionLotModel)
                .GetProperties()
                .Where(p => p.PropertyType == typeof(string))
                .ToList();

            // cтворюємо предикат для пошуку
            queryModel = queryModel.Where(lot =>
                stringProperties.Any(prop =>
                {
                    var value = (string?)prop.GetValue(lot);
                    return !string.IsNullOrEmpty(value) &&
                           value.Contains(keyword, StringComparison.OrdinalIgnoreCase);
                })
            ).ToList();
        }

        if (categoryId.HasValue)
        {
            var allCategories = dAPoint.CategoryRepository.GetQueryable().ToList();
            var allCategoriesModel = _mapper.Map<List<CategoryModel>>(allCategories);
            var rootCategory = allCategories.FirstOrDefault(c => c.Id == categoryId.Value);
            var rootCategoryModel = _mapper.Map<CategoryModel>(rootCategory);
            if (rootCategory != null)
            {
                var categoryIds = GetCategoryAndDescendantIds(rootCategoryModel, allCategoriesModel);
                queryModel = queryModel.Where(lot => categoryIds.Contains(lot.Category.Id)).ToList();
            }
        }

        return mapper.Map<List<AuctionLotModel>>(query);
    }

    private List<int> GetCategoryAndDescendantIds(CategoryModel root, List<CategoryModel> all) // для полегшення, просто збираємо наші всі підкатегорії за айді
    {
        var result = new List<int> { root.Id };

        var children = all.Where(c => c.ParentId == root.Id).ToList();

        foreach (var child in children)
        {
            result.AddRange(GetCategoryAndDescendantIds(child, all));
        }

        return result;
    }
}
