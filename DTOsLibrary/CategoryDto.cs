namespace DTOsLibrary;

public class CategoryDto
{
    public int Id { get; set; }

    public string Name { get; set; }

    public int? ParentId { get; set; }

    public List<CategoryDto> Subcategories { get; set; } = new List<CategoryDto>();
}
