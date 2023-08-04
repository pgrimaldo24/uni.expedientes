using AutoMapper;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Unir.Expedientes.Application.ExpedientesAlumnos.Queries.GetTiposRelacionesExpedientes;
using Unir.Expedientes.Application.Tests.Common;
using Unir.Expedientes.Domain.Entities;
using Xunit;

namespace Unir.Expedientes.Application.Tests.ExpedientesAlumnos.Queries.GetTiposRelacionesExpedientes
{
    [Collection("CommonTestCollection")]
    public class GetTiposRelacionesExpedientesQueryHandlerTest : TestBase
    {
        private readonly IMapper _mapper;

        public GetTiposRelacionesExpedientesQueryHandlerTest(CommonTestFixture fixture)
        {
            _mapper = fixture.Mapper;
        }

        #region Handle

        [Theory(DisplayName = "Cuando existen Tipos de relación expediente sin filtrar por nombre Devuelve elementos sin paginar")]
        [InlineData(5)]
        [InlineData(10)]
        public async Task Handle_SinPaginar_SinFiltroNombre(int cantidad)
        {
            //ARRANGE
            await Context.TiposRelacionesExpediente.AddRangeAsync(Enumerable.Range(1, cantidad).Select(i =>
                new TipoRelacionExpediente
                {
                    Id = i,
                    Nombre = Guid.NewGuid().ToString(),
                    EsLogro = true
                }));
            await Context.SaveChangesAsync();
            var request = new GetTiposRelacionesExpedientesQuery();
            var sut = new GetTiposRelacionesExpedientesQueryHandler(Context, _mapper);

            //ACT
            var actual = await sut.Handle(request, CancellationToken.None);

            //ASSERT
            Assert.Equal(cantidad, actual.Length);
        }

        [Fact(DisplayName = "Cuando existen Tipos de relación expediente filtrados por nombre Devuelve resultado sin paginar")]
        public async Task Handle_SinPaginar_FiltroNombre()
        {
            //ARRANGE
            var nombre = Guid.NewGuid().ToString();
            await Context.TiposRelacionesExpediente.AddRangeAsync(Enumerable.Range(1, 3).Select(i =>
                new TipoRelacionExpediente
                {
                    Id = i,
                    Nombre = $"{i}-{nombre}",
                    EsLogro = true
                }));
            await Context.SaveChangesAsync();
            var request = new GetTiposRelacionesExpedientesQuery
            {
                FilterNombre = nombre
            };
            var sut = new GetTiposRelacionesExpedientesQueryHandler(Context, _mapper);

            //ACT
            var actual = await sut.Handle(request, CancellationToken.None);

            //ASSERT
            Assert.NotEmpty(actual);
            Assert.Contains(nombre, actual.First().Nombre);
        }

        [Theory(DisplayName = "Cuando existen Tipos de relación expediente sin filtrar por nombre Devuelve resultado paginado")]
        [InlineData(5)]
        [InlineData(10)]
        public async Task Handle_Paginado_SinFiltroNombre(int cantidad)
        {
            //ARRANGE
            await Context.TiposRelacionesExpediente.AddRangeAsync(Enumerable.Range(1, cantidad).Select(i =>
                new TipoRelacionExpediente
                {
                    Id = i,
                    Nombre = $"{i}-{Guid.NewGuid()}",
                    EsLogro = true
                }));
            await Context.SaveChangesAsync();
            var request = new GetTiposRelacionesExpedientesQuery
            {
                Offset = 0,
                Limit = cantidad
            };
            var sut = new GetTiposRelacionesExpedientesQueryHandler(Context, _mapper);

            //ACT
            var actual = await sut.Handle(request, CancellationToken.None);

            //ASSERT
            Assert.Equal(cantidad, actual.Length);
        }

        #endregion
    }
}
