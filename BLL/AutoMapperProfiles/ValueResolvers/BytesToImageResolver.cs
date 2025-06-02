using AutoMapper;
using BLL.EntityBLLModels;
using DAL.Data.Services;
using DAL.Entities;

namespace BLL.AutoMapperProfiles.ValueResolvers;

public class BytesToImageResolver : IValueResolver<AuctionLotModel, AuctionLot, string?>
{
    private readonly IImageService imageService;

    public BytesToImageResolver(IImageService imageService)
    {
        ArgumentNullException.ThrowIfNull(imageService, nameof(imageService));
        this.imageService = imageService;
    }

    public string? Resolve(AuctionLotModel source, AuctionLot destination, string? destMember, ResolutionContext context)
    {
        if (source.Image == null || source.Image.Bytes == null || source.Image.Bytes.Length == 0)
        {
            return null;
        }

        try
        {
            // розширення вже збережено у ContentType, наприклад ".jpg"
            string extension = NormalizeExtension(source.Image.ContentType);
            string tempFilePath = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + extension);

            File.WriteAllBytes(tempFilePath, source.Image.Bytes);

            string relativePath = imageService.SaveImage(tempFilePath);

            File.Delete(tempFilePath);

            return relativePath;
        }
        catch
        {
            return null;
        }
    }

    private string NormalizeExtension(string extension)
    {
        if (string.IsNullOrWhiteSpace(extension))
        {
            return ".jpg"; // дефолтне розширення, якщо немає інформації
        }

        // гарантуємо, що розширення починається з крапки
        if (!extension.StartsWith('.'))
        {
            return "." + extension.ToLowerInvariant();
        }

        return extension.ToLowerInvariant();
    }
}
