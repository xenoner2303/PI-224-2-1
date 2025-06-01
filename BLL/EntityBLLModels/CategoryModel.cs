namespace BLL.EntityBLLModels;

public class CategoryModel
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public int? ParentId { get; set; }

    public List<CategoryModel> Subcategories { get; set; } = new List<CategoryModel>();
}
