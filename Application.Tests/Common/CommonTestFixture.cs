using AutoMapper;
using Xunit;

namespace Unir.Expedientes.Application.Tests.Common
{
    public class CommonTestFixture
    {
        public IMapper Mapper { get; private set; }
        public CommonTestFixture()
        {
            var configurationProvider = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MappingProfileTest>();
            });

            Mapper = configurationProvider.CreateMapper();
        }

        [CollectionDefinition("CommonTestCollection")]
        public class CommonTestCollection : ICollectionFixture<CommonTestFixture> { }
    }
}
