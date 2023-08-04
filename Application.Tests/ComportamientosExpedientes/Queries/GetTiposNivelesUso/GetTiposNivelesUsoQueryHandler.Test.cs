using AutoMapper;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Unir.Expedientes.Application.ComportamientosExpedientes.Queries.GetTiposNivelesUso;
using Unir.Expedientes.Application.Tests.Common;
using Unir.Expedientes.Domain.Entities;
using Xunit;

namespace Unir.Expedientes.Application.Tests.ComportamientosExpedientes.Queries.GetTiposNivelesUso
{
    [Collection("CommonTestCollection")]
    public class GetTiposNivelesUsoQueryHandlerTest : TestBase
    {
        private readonly IMapper _mapper;
        public GetTiposNivelesUsoQueryHandlerTest(CommonTestFixture fixture)
        {
            _mapper = fixture.Mapper;
        }

        #region Handle

        [Theory(DisplayName = "Cuando existen niveles de uso Devuelve resultado sin paginar")]
        [InlineData(5)]
        [InlineData(10)]
        public async Task Handle_SinPaginar(int cantidad)
        {
            //ARRANGE
            await Context.TiposNivelesUso.AddRangeAsync(Enumerable.Range(1, cantidad).Select(i =>
                new TipoNivelUso
                {
                    Id = i,
                    Nombre = Guid.NewGuid().ToString()
                }));
            await Context.SaveChangesAsync();
            var request = new GetTiposNivelesUsoQuery();
            var sut = new GetTiposNivelesUsoQueryHandler(Context, _mapper);

            //ACT
            var actual = await sut.Handle(request, CancellationToken.None);

            //ASSERT
            Assert.Equal(cantidad, actual.Length);
        }

        [Fact(DisplayName = "Cuando existen niveles de uso filtrados por nombre Devuelve resultado sin paginar")]
        public async Task Handle_SinPaginar_FiltroNombre()
        {
            //ARRANGE
            var nombre = Guid.NewGuid().ToString();
            await Context.TiposNivelesUso.AddRangeAsync(Enumerable.Range(1, 3).Select(i =>
                new TipoNivelUso
                {
                    Id = i,
                    Nombre = $"{i}-{nombre}"
                }));
            await Context.SaveChangesAsync();
            var request = new GetTiposNivelesUsoQuery
            {
                FilterNombre = nombre
            };
            var sut = new GetTiposNivelesUsoQueryHandler(Context, _mapper);

            //ACT
            var actual = await sut.Handle(request, CancellationToken.None);

            //ASSERT
            Assert.NotEmpty(actual);
            Assert.Contains(nombre, actual.First().Nombre);
        }

        [Theory(DisplayName = "Cuando existen niveles de uso Devuelve resultado paginado")]
        [InlineData(5)]
        [InlineData(10)]
        public async Task Handle_Paginado(int cantidad)
        {
            //ARRANGE
            await Context.TiposNivelesUso.AddRangeAsync(Enumerable.Range(1, cantidad).Select(i =>
                new TipoNivelUso
                {
                    Id = i,
                    Nombre = $"{i}-{Guid.NewGuid()}"
                }));
            await Context.SaveChangesAsync();
            var request = new GetTiposNivelesUsoQuery
            {
                Offset = 0,
                Limit = cantidad
            };
            var sut = new GetTiposNivelesUsoQueryHandler(Context, _mapper);

            //ACT
            var actual = await sut.Handle(request, CancellationToken.None);

            //ASSERT
            Assert.Equal(cantidad, actual.Length);
        }

        #endregion
    }
}
