using AutoMapper;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Unir.Expedientes.Application.ConsolidacionesRequisitosExpedientesDocumentos.Queries.GetConsolidacionRequisitoDocumentosByIdConsolidacion;
using Unir.Expedientes.Application.Tests.Common;
using Unir.Expedientes.Domain.Entities;
using Xunit;

namespace Unir.Expedientes.Application.Tests.ConsolidacionesRequisitosExpedientesDocumentos.Queries.GetConsolidacionRequisitoDocumentosByIdConsolidacion
{
    [Collection("CommonTestCollection")]
    public class GetConsolidacionRequisitoDocumentosByIdConsolidacionQueryHandlerTests : TestBase
    {
        private readonly IMapper _mapper;
        public GetConsolidacionRequisitoDocumentosByIdConsolidacionQueryHandlerTests(CommonTestFixture fixture)
        {
            _mapper = fixture.Mapper;
        }
        
        [Theory(DisplayName = "Cuando se obtienen las consolidaciones requisitos expedientes documentos Devuelve lista")]
        [InlineData(5)]
        [InlineData(10)]
        public async Task Handle_Ok(int cantidad)
        {
            //ARRANGE
            var request = new GetConsolidacionRequisitoDocumentosByIdConsolidacionQuery(1);
            var sut = new GetConsolidacionRequisitoDocumentosByIdConsolidacionQueryHandler(Context, _mapper);
            await Context.ConsolidacionesRequisitosExpedientesDocumentos.AddRangeAsync(Enumerable.Range(1, cantidad)
                .Select(i => new ConsolidacionRequisitoExpedienteDocumento
                {
                    Id = i,
                    ConsolidacionRequisitoExpediente = new ConsolidacionRequisitoExpediente
                    {
                        Id = i
                    }
                }));
            await Context.SaveChangesAsync();

            //ACT
            var actual = await sut.Handle(request, CancellationToken.None);

            //ASSERT
            Assert.NotEmpty(actual);
        }
    }
}
