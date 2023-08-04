using AutoMapper;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Unir.Expedientes.Application.Tests.Common;
using Unir.Expedientes.Application.TiposSeguimientosExpedientes.Queries.GetTiposSeguimientosExpedientes;
using Unir.Expedientes.Domain.Entities;
using Xunit;

namespace Unir.Expedientes.Application.Tests.TiposSeguimientosExpedientes.Queries
{
    [Collection("CommonTestCollection")]
    public class GetTiposSeguimientosExpedientesQueryHandlerTest : TestBase
    {
        private readonly IMapper _mapper;

        public GetTiposSeguimientosExpedientesQueryHandlerTest(CommonTestFixture fixture)
        {
            _mapper = fixture.Mapper;
        }

        #region Handle

        [Theory(DisplayName = "Cuando existen categorías Devuelve resultado sin paginar")]
        [InlineData(5)]
        [InlineData(10)]
        public async Task Handle_SinPaginar(int cantidad)
        {
            //ARRANGE
            await Context.TipoSeguimientosExpediente.AddRangeAsync(Enumerable.Range(1, cantidad).Select(i =>
                new TipoSeguimientoExpediente
                {
                    Id = i,
                    Nombre = Guid.NewGuid().ToString()
                }));
            await Context.SaveChangesAsync();
            var request = new GetTiposSeguimientosExpedientesQuery();
            var sut = new GetTiposSeguimientosExpedientesQueryHandler(Context, _mapper);

            //ACT
            var actual = await sut.Handle(request, CancellationToken.None);

            //ASSERT
            Assert.Equal(cantidad, actual.Length);
        }

        [Fact(DisplayName = "Cuando existen categorías filtradas por nombre Devuelve resultado sin paginar")]
        public async Task Handle_SinPaginar_FiltroNombre()
        {
            //ARRANGE
            await Context.TipoSeguimientosExpediente.AddRangeAsync(Enumerable.Range(1, 3).Select(i =>
                new TipoSeguimientoExpediente
                {
                    Id = i,
                    Nombre = $"{Guid.NewGuid()}-{i}abc"
                }));
            await Context.SaveChangesAsync();
            var request = new GetTiposSeguimientosExpedientesQuery
            {
                Nombre = "-1abc"
            };
            var sut = new GetTiposSeguimientosExpedientesQueryHandler(Context, _mapper);

            //ACT
            var actual = await sut.Handle(request, CancellationToken.None);

            //ASSERT
            Assert.Single(actual);
            Assert.Contains("-1abc", actual.First().Nombre);
        }

        [Theory(DisplayName = "Cuando existen categorías Devuelve resultado paginado")]
        [InlineData(5)]
        [InlineData(10)]
        public async Task Handle_Paginado(int cantidad)
        {
            //ARRANGE
            await Context.TipoSeguimientosExpediente.AddRangeAsync(Enumerable.Range(1, cantidad).Select(i =>
                new TipoSeguimientoExpediente
                {
                    Id = i,
                    Nombre = $"{Guid.NewGuid()}-{i}abc"
                }));
            await Context.SaveChangesAsync();
            var request = new GetTiposSeguimientosExpedientesQuery
            {
                Offset = 0,
                Limit = cantidad
            };
            var sut = new GetTiposSeguimientosExpedientesQueryHandler(Context, _mapper);

            //ACT
            var actual = await sut.Handle(request, CancellationToken.None);

            //ASSERT
            Assert.Equal(cantidad, actual.Length);
        }

        #endregion
    }
}
