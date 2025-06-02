using AutoMapper;
using AutoFixture;
using AutoFixture.AutoNSubstitute;
using DAL.Data;
using BLL.AutoMapperProfiles;

namespace Test;

public abstract class CommandTestBase
{
    protected readonly IFixture fixture;
    protected readonly IMapper mapper;
    protected readonly IUnitOfWork unitOfWorkMock;

    protected CommandTestBase()
    {
        fixture = new Fixture().Customize(new AutoNSubstituteCustomization());
        // заморожуємо юніт оф ворк в одному екземплярі fixture, щоб всі посилалися на одні й ті ж мок-об'єкти

        fixture.Behaviors.Add(new OmitOnRecursionBehavior(1));
        unitOfWorkMock = fixture.Freeze<IUnitOfWork>();

        var config = new MapperConfiguration(cfg => cfg.AddMaps(typeof(ActionLogProfile).Assembly));
        mapper = config.CreateMapper();
    }
}
