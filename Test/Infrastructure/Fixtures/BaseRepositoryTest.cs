using AutoMapper;
using DesignPattern.Infrastructure.Mongo.Mappings;

namespace Test.Infrastructure.Fixtures;

public abstract class BaseRepositoryTest
{
    protected readonly IMapper Mapper;

    protected BaseRepositoryTest()
    {
        // Setup AutoMapper profile mà bạn dùng trong dự án thật
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<MappingProfile>();
        }, null);
        
        Mapper = config.CreateMapper();
    }
}
