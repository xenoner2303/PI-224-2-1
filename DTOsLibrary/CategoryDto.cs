namespace DTOsLibrary;

public class CategoryDto
{
    public int Id { get; set; }

    public string Name { get; set; }

    public CategoryDto? Parent { get; set; }

    public List<CategoryDto> Subcategories { get; set; } = new List<CategoryDto>();

    public override string ToString() => Parent != null ? $"{Parent.Name} > {Name}" : Name;
}
