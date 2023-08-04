using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Unir.Expedientes.Application.ConsolidacionesRequisitosExpedientes.Queries.GetCausasEstadosRequisitosConsolidadasByIdRequisito;
using Unir.Expedientes.Application.Tests.Common;
using Unir.Expedientes.Domain.Entities;
using Xunit;

namespace Unir.Expedientes.Application.Tests.ConsolidacionesRequisitosExpedientes.Queries.GetCausasEstadosRequisitosConsolidadasByIdRequisito
{
    [Collection("CommonTestCollection")]
    public class GetCausasEstadosRequisitosConsolidadasByIdRequisitoQueryHandlerTests : TestBase
    {
        private readonly IMapper _mapper;
        public GetCausasEstadosRequisitosConsolidadasByIdRequisitoQueryHandlerTests(CommonTestFixture fixture)
        {
            _mapper = fixture.Mapper;
        }

        #region Handle

        [Fact(DisplayName = "Cuando existen las causas de estados de requisitos Devuelve resultado")]
        public async Task Handle_Ok()
        {
            //ARRANGE
            await Context.RequisitosExpedientes.AddAsync(new RequisitoExpediente
            {
                Id = 1
            });
            await Context.EstadosRequisitosExpedientes.AddAsync(new EstadoRequisitoExpediente
            {
                Id = 1
            });
            await Context.SaveChangesAsync();
            await Context.CausasEstadosRequisitosConsolidadasExpedientes.AddAsync(new CausaEstadoRequisitoConsolidadaExpediente
            {
                Id = 1,
                Nombre = Guid.NewGuid().ToString(),
                RequisitoExpediente = await Context.RequisitosExpedientes.FirstAsync(),
                EstadoRequisitoExpediente = await Context.EstadosRequisitosExpedientes.FirstAsync()
            });
            await Context.CausasEstadosRequisitosConsolidadasExpedientes.AddAsync(new CausaEstadoRequisitoConsolidadaExpediente
            {
                Id = 2,
                Nombre = Guid.NewGuid().ToString(),
                RequisitoExpediente = await Context.RequisitosExpedientes.FirstAsync(),
                EstadoRequisitoExpediente = await Context.EstadosRequisitosExpedientes.FirstAsync()
            });
            await Context.SaveChangesAsync();
            var request = new GetCausasEstadosRequisitosConsolidadasByIdRequisitoQuery
            {
                IdRequisito = 1,
                IdEstadoConsolidacion = 1
            };
            var sut = new GetCausasEstadosRequisitosConsolidadasByIdRequisitoQueryHandler(Context, _mapper);

            //ACT
            var actual = await sut.Handle(request, CancellationToken.None);

            //ASSERT
            Assert.Equal(2, actual.Length);
        }

        [Fact(DisplayName = "Cuando se filtra por el nombre Devuelve resultado")]
        public async Task Handle_FiltroNombre_Ok()
        {
            //ARRANGE
            var nombre = Guid.NewGuid().ToString();
            await Context.RequisitosExpedientes.AddAsync(new RequisitoExpediente
            {
                Id = 1
            });
            await Context.EstadosRequisitosExpedientes.AddAsync(new EstadoRequisitoExpediente
            {
                Id = 1
            });
            await Context.SaveChangesAsync();
            await Context.CausasEstadosRequisitosConsolidadasExpedientes.AddAsync(new CausaEstadoRequisitoConsolidadaExpediente
            {
                Id = 1,
                Nombre = $"{1}-{nombre}",
                RequisitoExpediente = await Context.RequisitosExpedientes.FirstAsync(),
                EstadoRequisitoExpediente = await Context.EstadosRequisitosExpedientes.FirstAsync()
            });
            await Context.CausasEstadosRequisitosConsolidadasExpedientes.AddAsync(new CausaEstadoRequisitoConsolidadaExpediente
            {
                Id = 2,
                Nombre = $"{2}-{nombre}",
                RequisitoExpediente = await Context.RequisitosExpedientes.FirstAsync(),
                EstadoRequisitoExpediente = await Context.EstadosRequisitosExpedientes.FirstAsync()
            });
            await Context.SaveChangesAsync();
            var request = new GetCausasEstadosRequisitosConsolidadasByIdRequisitoQuery
            {
                IdRequisito = 1,
                IdEstadoConsolidacion = 1,
                FilterNombre = nombre
            };
            var sut = new GetCausasEstadosRequisitosConsolidadasByIdRequisitoQueryHandler(Context, _mapper);

            //ACT
            var actual = await sut.Handle(request, CancellationToken.None);

            //ASSERT
            Assert.NotEmpty(actual);
            Assert.True(actual.All(a => a.Nombre.Contains(nombre)));
        }

        #endregion
    }
}
