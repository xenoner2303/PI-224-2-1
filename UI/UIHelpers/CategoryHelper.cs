using DTOsLibrary;

namespace UI.UIHelpers;

public static class CategoryHelper
{
    public static List<CategoryDto> FlattenCategoriesWithNumbers(List<CategoryDto> categories)
    {
        var result = new List<CategoryDto>();
        int counter = 1;
        Flatten(categories, null, result, ref counter);
        return result;
    }

    private static void Flatten(List<CategoryDto> categories, CategoryDto? parent, List<CategoryDto> result, ref int counter)
    {
        foreach (var category in categories)
        {
            category.Parent = parent;

            category.DisplayName = $"{counter}. {(parent != null ? $"{parent.DisplayName.Split(". ", 2).Last()} > {category.Name}" : category.Name)}";
            result.Add(category);
            counter++;

            if (category.Subcategories?.Any() == true)
            {
                Flatten(category.Subcategories, category, result, ref counter);
            }
        }
    }
}
