using System.ComponentModel.DataAnnotations;

namespace DAL.Entities;

public class Category
{
    public int Id { get; set; } // айді для бази даних

    private string name;

    [StringLength(50, MinimumLength = 4, ErrorMessage = "Назва категорії має містити 4-50 символів")] // метадані для EF
    public string Name
    {
        get => name;
        set
        {
            if (string.IsNullOrWhiteSpace(value) || value.Length < 4 || value.Length > 50)
            {
                throw new ArgumentException("Назва категорії має містити 4-50 символів", nameof(Name));
            }

            name = value;
        }
    }

    public int? ParentId { get; set; } // nullable, бо у кореневих категорій нема батька
    public Category? Parent { get; set; } // батько для вкладеності категорії

    public List<Category> Subcategories { get; set; } = new List<Category>(); // підкатегорії
    public List<AuctionLot> Lots { get; set; } = new List<AuctionLot>(); // 1 категорія - багато лотів
}
