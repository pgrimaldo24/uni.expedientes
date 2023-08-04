using AutoMapper;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Unir.Expedientes.Application.ExpedientesAlumnos.Queries.GetAllEstadosExpedientes;
using Unir.Expedientes.Application.Tests.Common;
using Unir.Expedientes.Domain.Entities;
using Xunit;

namespace Unir.Expedientes.Application.Tests.ExpedientesAlumnos.Queries.GetAllEstadosExpedientes
{
    [Collection("CommonTestCollection")]
    public class GetAllEstadosExpedientesQueryHandlerTest : TestBase
    {
        private readonly IMapper _mapper;

        public GetAllEstadosExpedientesQueryHandlerTest(CommonTestFixture fixture)
        {
            _mapper = fixture.Mapper;
        }

        #region Handle

        [Theory(DisplayName = "Cuando existen Estados de expediente sin filtrar por nombre Devuelve elementos sin paginar")]
        [InlineData(5)]
        [InlineData(10)]
        public async Task Handle_SinPaginar_SinFiltroNombre(int cantidad)
        {
            //ARRANGE
            await Context.EstadosExpedientes.AddRangeAsync(Enumerable.Range(1, cantidad).Select(i =>
                new EstadoExpediente
                {
                    Id = i,
                    Nombre = Guid.NewGuid().ToString()
                }));
            await Context.SaveChangesAsync();
            var request = new GetAllEstadosExpedientesQuery();
            var sut = new GetAllEstadosExpedientesQueryHandler(Context, _mapper);

            //ACT
            var actual = await sut.Handle(request, CancellationToken.None);

            //ASSERT
            Assert.Equal(cantidad, actual.Length);
        }

        [Fact(DisplayName = "Cuando existen Estados de expediente filtrados por nombre Devuelve resultado sin paginar")]
        public async Task Handle_SinPaginar_FiltroNombre()
        {
            //ARRANGE
            var nombre = Guid.NewGuid().ToString();
            await Context.EstadosExpedientes.AddRangeAsync(Enumerable.Range(1, 3).Select(i =>
                new EstadoExpediente
                {
                    Id = i,
                    Nombre = $"{i}-{nombre}"
                }));
            await Context.SaveChangesAsync();
            var request = new GetAllEstadosExpedientesQuery
            {
                FilterNombre = nombre
            };
            var sut = new GetAllEstadosExpedientesQueryHandler(Context, _mapper);

            //ACT
            var actual = await sut.Handle(request, CancellationToken.None);

            //ASSERT
            Assert.NotEmpty(actual);
            Assert.Contains(nombre, actual.First().Nombre);
        }

        [Theory(DisplayName = "Cuando existen Estados de expediente sin filtrar por nombre Devuelve resultado paginado")]
        [InlineData(5)]
        [InlineData(10)]
        public async Task Handle_Paginado_SinFiltroNombre(int cantidad)
        {
            //ARRANGE
            await Context.EstadosExpedientes.AddRangeAsync(Enumerable.Range(1, cantidad).Select(i =>
                new EstadoExpediente
                {
                    Id = i,
                    Nombre = $"{i}-{Guid.NewGuid()}"
                }));
            await Context.SaveChangesAsync();
            var request = new GetAllEstadosExpedientesQuery
            {
                Offset = 0,
                Limit = cantidad
            };
            var sut = new GetAllEstadosExpedientesQueryHandler(Context, _mapper);

            //ACT
            var actual = await sut.Handle(request, CancellationToken.None);

            //ASSERT
            Assert.Equal(cantidad, actual.Length);
        }

        #endregion
    }
}
