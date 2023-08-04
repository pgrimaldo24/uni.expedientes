using AutoMapper;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Unir.Expedientes.Application.ExpedientesAlumnos.Queries.GetEspecializacionesExpedienteByIdExpediente;
using Unir.Expedientes.Application.Tests.Common;
using Unir.Expedientes.Domain.Entities;
using Xunit;

namespace Unir.Expedientes.Application.Tests.ExpedientesAlumnos.Queries.GetEspecializacionesExpedienteByIdExpediente
{
    [Collection("CommonTestCollection")]
    public class GetEspecializacionesExpedienteByIdExpedienteQueryHandlerTests : TestBase
    {
        private readonly IMapper _mapper;
        public GetEspecializacionesExpedienteByIdExpedienteQueryHandlerTests(
            CommonTestFixture fixture)
        {
            _mapper = fixture.Mapper;
        }

        #region Handle

        [Fact(DisplayName = "Cuando el expediente no tiene hito tipo especialización Devuelve lista vacía")]
        public async Task Handle_Empty() 
        {
            //ARRANGE
            var request = new GetEspecializacionesExpedienteByIdExpedienteQuery(1);
            var sut = new GetEspecializacionesExpedienteByIdExpedienteQueryHandler(Context, _mapper);

            //ACT
            var actual = await sut.Handle(request, CancellationToken.None);

            //ASSERT
            Assert.Empty(actual);
        }

        [Theory(DisplayName = "Cuando el expediente tiene hito tipo especialización Devuelve resultado")]
        [InlineData(5)]
        [InlineData(10)]
        [InlineData(20)]
        public async Task Handle_Ok(int total)
        {
            //ARRANGE
            const int idExpedienteAlumno = 1;
            await Context.HitosConseguidos.AddAsync(new HitoConseguido
            {
                Id = 1,
                TipoConseguidoId = TipoHitoConseguido.Especializacion,
                ExpedienteAlumnoId = idExpedienteAlumno
            });

            var especializaciones = Enumerable.Range(1, total).Select(e => new ExpedienteEspecializacion
            {
                Id = e,
                ExpedienteAlumnoId = idExpedienteAlumno,
                IdRefEspecializacion = e.ToString()
            });
            await Context.ExpedientesEspecializaciones.AddRangeAsync(especializaciones);
            await Context.SaveChangesAsync();

            var request = new GetEspecializacionesExpedienteByIdExpedienteQuery(idExpedienteAlumno);
            var sut = new GetEspecializacionesExpedienteByIdExpedienteQueryHandler(Context, _mapper);

            //ACT
            var actual = await sut.Handle(request, CancellationToken.None);

            //ASSERT
            Assert.NotEmpty(actual);
            Assert.Equal(total, actual.Count);
        }

        #endregion
    }
}
