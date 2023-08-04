using AutoMapper;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Unir.Expedientes.Application.HitosConseguidos.Queries.GetHitosConseguidosByIdExpediente;
using Unir.Expedientes.Application.Tests.Common;
using Unir.Expedientes.Domain.Entities;
using Xunit;

namespace Unir.Expedientes.Application.Tests.HitosConseguidos.Queries.GetHitosConseguidosByIdExpediente
{
    [Collection("CommonTestCollection")]
    public class GetHitosConseguidosByIdExpedienteQueryHandlerTest : TestBase
    {
        private readonly IMapper _mapper;

        public GetHitosConseguidosByIdExpedienteQueryHandlerTest(
            CommonTestFixture fixture)
        {
            _mapper = fixture.Mapper;
        }

        #region Handle

        [Theory(DisplayName = "Cuando el expediente tiene hitos conseguidos Devuelve lista de hitos")]
        [InlineData(5)]
        [InlineData(10)]
        [InlineData(15)]
        public async Task Handle_Ok(int cantidad)
        {
            //ARRANGE
            const int idExpedienteAlumno = 1;
            var request = new GetHitosConseguidosByIdExpedienteQuery(idExpedienteAlumno);
            var sut = new GetHitosConseguidosByIdExpedienteQueryHandler(Context, _mapper);
            var hitosConseguidos = Enumerable.Range(1, cantidad)
                .Select(r => new HitoConseguido
                {
                    Id = r,
                    Nombre = $"{Guid.NewGuid()} - {r}",
                    ExpedienteAlumnoId = idExpedienteAlumno,
                    TipoConseguido = new TipoHitoConseguido
                    {
                        Id = r
                    }
                }).ToList();

            await Context.HitosConseguidos.AddRangeAsync(hitosConseguidos);
            await Context.SaveChangesAsync();

            //ACT
            var actual = await sut.Handle(request, CancellationToken.None);

            //ASSERT
            Assert.NotEmpty(actual);
            Assert.Equal(cantidad, actual.Count);
        }

        #endregion
    }
}
