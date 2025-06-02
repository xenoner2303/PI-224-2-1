using AutoMapper;
using BLL.EntityBLLModels;
using DAL.Data.Services;
using DAL.Entities;

namespace BLL.AutoMapperProfiles.ValueResolvers;

public class ImageToBytesImageModelResolver : IValueResolver<AuctionLot, AuctionLotModel, ImageModel?>
{
    private readonly IImageService imageService;

    public ImageToBytesImageModelResolver(IImageService imageService)
    {
        ArgumentNullException.ThrowIfNull(imageService, nameof(imageService));

        this.imageService = imageService;
    }

    public ImageModel? Resolve(AuctionLot source, AuctionLotModel destination, ImageModel? destMember, ResolutionContext context)
    {
        if (string.IsNullOrWhiteSpace(source.RelativeImagePath))
        {
            return null;
        }

        try
        {
            ImageModel imageModel = new ImageModel
            {
                Bytes = imageService.LoadImage(source.RelativeImagePath)
            };

            return imageModel;
        }
        catch
        {
            return null;
        }
    }
}
