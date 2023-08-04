using AutoMapper;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Unir.Expedientes.Application.RequisitosExpedientes.Queries.GetTiposRequisitosExpedientes;
using Unir.Expedientes.Application.Tests.Common;
using Unir.Expedientes.Domain.Entities;
using Xunit;

namespace Unir.Expedientes.Application.Tests.RequisitosExpedientes.Queries.GetTiposRequisitosExpedientes
{
    [Collection("CommonTestCollection")]
    public class GetTiposRequisitosExpedientesQueryHandlerTest : TestBase
    {
        private readonly IMapper _mapper;
        public GetTiposRequisitosExpedientesQueryHandlerTest(CommonTestFixture fixture)
        {
            _mapper = fixture.Mapper;
        }

        #region Handle

        [Theory(DisplayName = "Cuando existen tipos de requisitos Devuelve resultado sin paginar")]
        [InlineData(5)]
        [InlineData(10)]
        public async Task Handle_SinPaginar(int cantidad)
        {
            //ARRANGE
            await Context.TiposRequisitosExpedientes.AddRangeAsync(Enumerable.Range(1, cantidad).Select(i =>
                new TipoRequisitoExpediente
                {
                    Id = i,
                    Nombre = Guid.NewGuid().ToString()
                }));
            await Context.SaveChangesAsync();
            var request = new GetTiposRequisitosExpedientesQuery();
            var sut = new GetTiposRequisitosExpedientesQueryHandler(Context, _mapper);

            //ACT
            var actual = await sut.Handle(request, CancellationToken.None);

            //ASSERT
            Assert.Equal(cantidad, actual.Length);
        }

        [Fact(DisplayName = "Cuando existen tipos de requisitos filtrados por nombre Devuelve resultado sin paginar")]
        public async Task Handle_SinPaginar_FiltroNombre()
        {
            //ARRANGE
            var nombre = Guid.NewGuid().ToString();
            await Context.TiposRequisitosExpedientes.AddRangeAsync(Enumerable.Range(1, 3).Select(i =>
                new TipoRequisitoExpediente
                {
                    Id = i,
                    Nombre = $"{i}-{nombre}"
                }));
            await Context.SaveChangesAsync();
            var request = new GetTiposRequisitosExpedientesQuery
            {
                FilterNombre = nombre
            };
            var sut = new GetTiposRequisitosExpedientesQueryHandler(Context, _mapper);

            //ACT
            var actual = await sut.Handle(request, CancellationToken.None);

            //ASSERT
            Assert.NotEmpty(actual);
            Assert.Contains(nombre, actual.First().Nombre);
        }

        [Theory(DisplayName = "Cuando existen tipos de requisitos Devuelve resultado paginado")]
        [InlineData(5)]
        [InlineData(10)]
        public async Task Handle_Paginado(int cantidad)
        {
            //ARRANGE
            await Context.TiposRequisitosExpedientes.AddRangeAsync(Enumerable.Range(1, cantidad).Select(i =>
                new TipoRequisitoExpediente
                {
                    Id = i,
                    Nombre = $"{i}-{Guid.NewGuid()}"
                }));
            await Context.SaveChangesAsync();
            var request = new GetTiposRequisitosExpedientesQuery
            {
                Offset = 0,
                Limit = cantidad
            };
            var sut = new GetTiposRequisitosExpedientesQueryHandler(Context, _mapper);

            //ACT
            var actual = await sut.Handle(request, CancellationToken.None);

            //ASSERT
            Assert.Equal(cantidad, actual.Length);
        }

        #endregion
    }
}
