using AutoMapper;
using Moq;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Unir.Expedientes.Application.Common.Interfaces;
using Unir.Expedientes.Application.RequisitosExpedientes.Queries.GetRequisitoById;
using Unir.Expedientes.Application.Tests.Common;
using Unir.Expedientes.Domain.Entities;
using Unir.Framework.ApplicationSuperTypes.Exceptions;
using Xunit;

namespace Unir.Expedientes.Application.Tests.RequisitosExpedientes.Queries.GetRequisitoById
{
    [Collection("CommonTestCollection")]
    public class GetRequisitoByIdQueryHandlerTests : TestBase
    {
        private readonly IMapper _mapper;

        public GetRequisitoByIdQueryHandlerTests(CommonTestFixture fixture)
        {
            _mapper = fixture.Mapper;
        }

        #region Handle

        [Fact(DisplayName = "Cuando se encuentra un requisito devuelve Ok")]
        public async Task Handle_ExisteElemento_Ok()
        {
            //ARRANGE
            var id = 1;
            var requisito = new RequisitoExpediente
            {
                Id = 1
            };
            await Context.RequisitosExpedientes.AddAsync(requisito);
            await Context.SaveChangesAsync();
            var mockErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>();
            var sut = new Mock<GetRequisitoByIdQueryHandler>(Context, _mapper, mockErpAcademicoServiceClient.Object)
            { CallBase = true };

            //ACT
            var actual = await sut.Object.Handle(new GetRequisitoByIdQuery(id),
                CancellationToken.None);

            //ASSERT
            Assert.NotNull(actual);
            Assert.IsType<RequisitoDto>(actual);
            Assert.Equal(Context.RequisitosExpedientes.FirstOrDefault()?.Id, requisito.Id);
        }

        [Fact(DisplayName = "Cuando no se encuentra un requerimiento devuelve una excepción")]
        public async Task Handle_NoExisteElemento()
        {
            //ARRANGE
            var mockErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>();
            var sut = new Mock<GetRequisitoByIdQueryHandler>(Context, _mapper, mockErpAcademicoServiceClient.Object)
            { CallBase = true };

            //ACT
            var ex = await Record.ExceptionAsync(async () =>
            {
                await sut.Object.Handle(new GetRequisitoByIdQuery(99), CancellationToken.None);
            });

            //ASSERT
            Assert.IsType<NotFoundException>(ex);
            Assert.Contains("not found", ex.Message);
        }

        #endregion
    }
}
