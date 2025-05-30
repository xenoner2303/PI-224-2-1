using AutoMapper;
using DAL.Data;
using DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace BLL.Commands.UserManipulationsCommands.LotCommand;

internal class SearchLotsCommand : AbstrCommandWithDA<List<AuctionLot>>
{
    private readonly string? keyword;
    private readonly int? categoryId;

    public SearchLotsCommand(string? keyword, int? categoryId, IUnitOfWork unitOfWork, IMapper mapper)
        : base(unitOfWork, mapper)
    {
        this.keyword = keyword;
        this.categoryId = categoryId;
    }

    public override string Name => "Пошук лотів";

    public override List<AuctionLot> Execute()
    {
        var query = dAPoint.AuctionLotRepository.GetQueryable()
            .Include(l => l.Bids)
            .Include(l => l.Category).ToList();

        // пошук по ключовому слову - тексту в усіх string полях класу AuctionLot
        if (!string.IsNullOrWhiteSpace(keyword))
        {
            var stringProperties = typeof(AuctionLot)
                .GetProperties()
                .Where(p => p.PropertyType == typeof(string))
                .ToList();

            // cтворюємо предикат для пошуку
            query = query.Where(lot =>
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
            var rootCategory = allCategories.FirstOrDefault(c => c.Id == categoryId.Value);

            if (rootCategory != null)
            {
                var categoryIds = GetCategoryAndDescendantIds(rootCategory, allCategories);
                query = query.Where(lot => categoryIds.Contains(lot.CategoryId)).ToList();
            }
        }

        return query;
    }

    private List<int> GetCategoryAndDescendantIds(Category root, List<Category> all) // для полегшення, просто збираємо наші всі підкатегорії за айді
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
