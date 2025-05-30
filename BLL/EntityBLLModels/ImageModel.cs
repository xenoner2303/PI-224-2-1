namespace BLL.EntityBLLModels;

public class ImageModel
{
    public byte[] Bytes { get; set; }
    public string? ContentType { get; set; } // наприклад, ".jpg"
}