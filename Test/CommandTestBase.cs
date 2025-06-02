using AutoMapper;
using AutoFixture;
using AutoFixture.AutoNSubstitute;
using DAL.Data;
using BLL.AutoMapperProfiles;
using DAL.Data.Services;
using BLL.AutoMapperProfiles.ValueResolvers;
using Microsoft.Extensions.DependencyInjection;
using System;
using BLL.Services;
using NSubstitute;

namespace Test;

public abstract class CommandTestBase
{
    protected readonly IFixture fixture;
    protected readonly IMapper mapper;
    protected readonly IUnitOfWork unitOfWorkMock;
    protected readonly ServiceProvider serviceProvider;

    protected CommandTestBase()
    {
        fixture = new Fixture().Customize(new AutoNSubstituteCustomization());
        // заморожуємо юніт оф ворк в одному екземплярі fixture, щоб всі посилалися на одні й ті ж мок-об'єкти

        fixture.Behaviors.Add(new OmitOnRecursionBehavior(1));
        unitOfWorkMock = fixture.Freeze<IUnitOfWork>();

        // налаштовуємо для зручності діай контйнер для автомапінгу (вирішує проблему з ресолверами та конвенторами)
        var services = new ServiceCollection();
        BLLInitializer.AddAutoMapperToServices(services);
        var imageServiceMock = Substitute.For<IImageService>(); // мок для IImageService для тестування
        services.AddSingleton(imageServiceMock);

        serviceProvider = services.BuildServiceProvider();

        mapper = serviceProvider.GetRequiredService<IMapper>();
    }
}
