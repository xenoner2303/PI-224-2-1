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
        if (source.ImageBytes == null || source.ImageBytes.Length == 0)
            return null;

        try
        {
            // генеруємо тимчасовий файл, зберігаємо байти і передаємо далі у service
            string tempFilePath = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".tmp");

            File.WriteAllBytes(tempFilePath, source.ImageBytes);

            // зберігаємо через сервіс і отримуємо шлях
            string relativePath = imageService.SaveImage(tempFilePath);

            // після цього можна видалити тимчасовий файл
            File.Delete(tempFilePath);

            return relativePath;
        }
        catch
        {
            return null;
        }
    }
}
