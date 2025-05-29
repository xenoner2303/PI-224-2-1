using AutoMapper;
using BLL.EntityBLLModels;
using DAL.Data.Services;
using DAL.Entities;

namespace BLL.AutoMapperProfiles.ValueResolvers;

public class ImageToBytesResolver : IValueResolver<AuctionLot, AuctionLotModel, byte[]?>
{
    private readonly IImageService imageService;

    public ImageToBytesResolver(IImageService imageService)
    {
        ArgumentNullException.ThrowIfNull(imageService, nameof(imageService));

        this.imageService = imageService;
    }

    public byte[]? Resolve(AuctionLot source, AuctionLotModel destination, byte[]? destMember, ResolutionContext context)
    {
        if (string.IsNullOrWhiteSpace(source.RelativeImagePath))
        {
            return null;
        }

        try
        {
            // завантажуємо з DAL масив байтів
            return imageService.LoadImage(source.RelativeImagePath);
        }
        catch
        {
            return null;
        }
    }
}
