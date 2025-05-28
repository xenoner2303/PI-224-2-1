namespace BLL.EntityBLLModels;

public class CategoryModel
{
    public int Id { get; set; }

    public string Name { get; set; }

    public CategoryModel? Parent { get; set; }

    public List<CategoryModel> Subcategories { get; set; } = new List<CategoryModel>();
}
