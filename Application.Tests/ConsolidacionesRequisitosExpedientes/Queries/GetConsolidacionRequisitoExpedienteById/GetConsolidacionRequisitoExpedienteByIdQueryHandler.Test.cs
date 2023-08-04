using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Unir.Expedientes.Application.ConsolidacionesRequisitosExpedientes.Queries.GetConsolidacionRequisitoExpedienteById;
using Unir.Expedientes.Application.Tests.Common;
using Unir.Expedientes.Domain.Entities;
using Unir.Framework.ApplicationSuperTypes.Exceptions;
using Xunit;

namespace Unir.Expedientes.Application.Tests.ConsolidacionesRequisitosExpedientes.Queries.GetConsolidacionRequisitoExpedienteById
{
    [Collection("CommonTestCollection")]
    public class GetConsolidacionRequisitoExpedienteByIdQueryHandlerTests : TestBase
    {
        private readonly IMapper _mapper;
        public GetConsolidacionRequisitoExpedienteByIdQueryHandlerTests(CommonTestFixture fixture)
        {
            _mapper = fixture.Mapper;
        }

        #region Handle

        [Fact(DisplayName = "Cuando no existe la consolidación requisito expediente Devuelve una excepción")]
        public async Task Handle_NotFoundException()
        {
            //ARRANGE
            var request = new GetConsolidacionRequisitoExpedienteByIdQuery(1);
            var sut = new GetConsolidacionRequisitoExpedienteByIdQueryHandler(Context, _mapper);

            //ACT
            var ex = await Record.ExceptionAsync(async () =>
            {
                await sut.Handle(request, CancellationToken.None);
            });

            //ASSERT
            Assert.NotNull(ex);
            Assert.IsType<NotFoundException>(ex);
        }

        [Fact(DisplayName = "Cuando se obtiene la consolidación requisito expediente Devuelve Ok")]
        public async Task Handle_Ok()
        {
            //ARRANGE
            var request = new GetConsolidacionRequisitoExpedienteByIdQuery(1);
            var sut = new GetConsolidacionRequisitoExpedienteByIdQueryHandler(Context, _mapper);
            var consolidacion = new ConsolidacionRequisitoExpediente
            {
                Id = 1,
                EstadoRequisitoExpediente = new EstadoRequisitoExpediente
                {
                    Id = 1,
                    Nombre = Guid.NewGuid().ToString()
                },
                TipoRequisitoExpediente = new TipoRequisitoExpediente
                {
                    Id = 1,
                    Nombre = Guid.NewGuid().ToString()
                },
                RequisitoExpediente = new RequisitoExpediente
                {
                    Id = 1,
                    Nombre = Guid.NewGuid().ToString(),
                    RequisitosExpedientesDocumentos = new List<RequisitoExpedienteDocumento>(),
                    RolesRequisitosExpedientes = new List<RolRequisitoExpediente>()
                }
            };
            await Context.ConsolidacionesRequisitosExpedientes.AddAsync(consolidacion);
            await Context.SaveChangesAsync();

            //ACT
            var actual = await sut.Handle(request, CancellationToken.None);

            //ASSERT
            Assert.NotNull(actual);
            Assert.Equal(consolidacion.Id, actual.Id);
            Assert.Equal(consolidacion.EstadoRequisitoExpediente.Id, actual.EstadoRequisitoExpediente.Id);
            Assert.Equal(consolidacion.EstadoRequisitoExpediente.Nombre, actual.EstadoRequisitoExpediente.Nombre);
            Assert.Equal(consolidacion.TipoRequisitoExpediente.Id, actual.TipoRequisitoExpediente.Id);
            Assert.Equal(consolidacion.TipoRequisitoExpediente.Nombre, actual.TipoRequisitoExpediente.Nombre);
            Assert.Equal(consolidacion.RequisitoExpediente.Id, actual.RequisitoExpediente.Id);
            Assert.Equal(consolidacion.RequisitoExpediente.Nombre, actual.RequisitoExpediente.Nombre);
        }

        #endregion
    }
}
