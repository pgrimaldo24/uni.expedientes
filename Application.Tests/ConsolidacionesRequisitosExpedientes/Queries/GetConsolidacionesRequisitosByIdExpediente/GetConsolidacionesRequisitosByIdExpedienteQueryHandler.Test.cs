using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Unir.Expedientes.Application.ConsolidacionesRequisitosExpedientes.Queries.GetConsolidacionesRequisitosByIdExpediente;
using Unir.Expedientes.Application.Tests.Common;
using Unir.Expedientes.Domain.Entities;
using Xunit;

namespace Unir.Expedientes.Application.Tests.ConsolidacionesRequisitosExpedientes.Queries.GetConsolidacionesRequisitosByIdExpediente
{
    [Collection("CommonTestCollection")]
    public class GetConsolidacionesRequisitosByIdExpedienteQueryHandlerTests : TestBase
    {
        private readonly IMapper _mapper;
        public GetConsolidacionesRequisitosByIdExpedienteQueryHandlerTests(
            CommonTestFixture fixture)
        {
            _mapper = fixture.Mapper;
        }

        #region Handle

        [Theory(DisplayName = "Cuando se obtienen la consolidaciones requisitos por id de expediente Devuelve Ok")]
        [InlineData(5)]
        [InlineData(10)]
        [InlineData(15)]
        public async Task Handle_Ok(int total)
        {
            //ARRANGE
            var request = new GetConsolidacionesRequisitosByIdExpedienteQuery(1);
            var sut = new GetConsolidacionesRequisitosByIdExpedienteQueryHandler(Context, _mapper);
            var consolidaciones = Enumerable.Range(1, total).Select(c => new ConsolidacionRequisitoExpediente
            {
                Id = c,
                EstadoRequisitoExpediente = new EstadoRequisitoExpediente
                {
                    Id = c,
                    Nombre = Guid.NewGuid().ToString()
                },
                TipoRequisitoExpediente = new TipoRequisitoExpediente
                {
                    Id = c,
                    Nombre = Guid.NewGuid().ToString()
                },
                RequisitoExpediente = new RequisitoExpediente
                {
                    Id = c,
                    Nombre = Guid.NewGuid().ToString(),
                    RequisitosExpedientesDocumentos = new List<RequisitoExpedienteDocumento>(),
                    RolesRequisitosExpedientes = new List<RolRequisitoExpediente>()
                },
                ExpedienteAlumnoId = 1
            });
            await Context.ConsolidacionesRequisitosExpedientes.AddRangeAsync(consolidaciones);
            await Context.SaveChangesAsync();

            //ACT
            var actual = await sut.Handle(request, CancellationToken.None);

            //ASSERT
            Assert.NotEmpty(actual);
            Assert.Equal(total, actual.Count);
        }

        #endregion
    }
}
