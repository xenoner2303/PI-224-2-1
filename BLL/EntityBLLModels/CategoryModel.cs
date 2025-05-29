namespace BLL.EntityBLLModels;

public class CategoryModel
{
    public int Id { get; set; }

    private string name = string.Empty;

    public string Name
    {
        get => name;
        set
        {
            if (string.IsNullOrWhiteSpace(value) || value.Length < 2)
            {
                throw new ArgumentException("Ім’я повинно містити щонайменше 2 літери.");
            }

            foreach (char c in value)
            {
                if (char.IsDigit(c))
                {
                    throw new ArgumentException("Ім’я не може містити цифри.");
                }
            }

            name = value;
        }
    }

    public CategoryModel? Parent { get; set; }

    public List<CategoryModel> Subcategories { get; set; } = new List<CategoryModel>();
}
