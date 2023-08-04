using System.Collections.Generic;
using AutoMapper;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Unir.Expedientes.Application.AsignaturasExpediente.Queries.GetAsignaturasExpedientesByIdExpediente;
using Unir.Expedientes.Application.Tests.Common;
using Unir.Expedientes.Domain.Entities;
using Xunit;

namespace Unir.Expedientes.Application.Tests.AsignaturasExpediente.Queries.GetAsignaturasExpedientesByIdExpediente
{
    [Collection("CommonTestCollection")]
    public class GetAsignaturasExpedientesByIdExpedienteQueryHandlerTests : TestBase
    {
        private readonly IMapper _mapper;

        public GetAsignaturasExpedientesByIdExpedienteQueryHandlerTests(
            CommonTestFixture fixture)
        {
            _mapper = fixture.Mapper;
        }

        #region Handle

        [Theory(DisplayName = "Cuando se encuentran las asignaturas Devuelve Ok")]
        [InlineData(10)]
        [InlineData(20)]
        public async Task Handle_Ok(int total)
        {
            //ARRANGE
            const int idExpedienteAlumno = 1;
            var asignaturas = Enumerable.Range(1, total).Select(a => new AsignaturaExpediente
            {
                Id = a,
                ExpedienteAlumnoId = idExpedienteAlumno
            }).ToList();
            await Context.AsignaturasExpedientes.AddRangeAsync(asignaturas);
            await Context.SaveChangesAsync();

            var sut = new GetAsignaturasExpedientesByIdExpedienteQueryHandler(Context, _mapper);

            //ACT
            var actual = await sut.Handle(new GetAsignaturasExpedientesByIdExpedienteQuery(idExpedienteAlumno),
                CancellationToken.None);

            //ASSERT
            Assert.NotNull(actual);
            Assert.NotEmpty(actual);
            Assert.Equal(total, actual.Count);
        }

        #endregion

        #region GetCalificacion

        [Fact(DisplayName = "Cuando se obtiene la calificación Devuelve vacío")]
        public void GetCalificacion_Empty()
        {
            //ARRANGE
            var asignaturasCalificaciones = new List<AsignaturaCalificacionDto>();
            var sut = new GetAsignaturasExpedientesByIdExpedienteQueryHandler(Context, _mapper);

            //ACT
            var actual = sut.GetCalificacion(asignaturasCalificaciones);

            //ASSERT
            Assert.Empty(actual);
        }

        [Theory(DisplayName = "Cuando las calificaciones son superadas o no superadas Devuelve la calificación con la nota más alta")]
        [InlineData(true)]
        [InlineData(false)]
        public void GetCalificacion_Ok(bool superada)
        {
            //ARRANGE
            const int calificacion = 6;
            const string nombre = "B";
            var asignaturasCalificaciones = new List<AsignaturaCalificacionDto>
            {
                new()
                {
                    Calificacion = 5,
                    Convocatoria = 2,
                    NombreCalificacion = "A",
                    Superada = superada
                },
                new()
                {
                    Calificacion = calificacion,
                    Convocatoria = 5,
                    NombreCalificacion = nombre,
                    Superada = superada
                },
                new()
                {
                    Calificacion = 6,
                    Convocatoria = 4,
                    NombreCalificacion = "C",
                    Superada = superada
                }
            };

            var sut = new GetAsignaturasExpedientesByIdExpedienteQueryHandler(Context, _mapper);

            //ACT
            var actual = sut.GetCalificacion(asignaturasCalificaciones);

            //ASSERT
            Assert.Equal($"{calificacion} {nombre}", actual);
        }

        [Theory(DisplayName = "Cuando la nota es 0 y el nombre es Apto o No Apto Retorna la calificación sin el valor de la Nota")]
        [InlineData("Apto")]
        [InlineData("No Apto")]
        public void GetCalificacion_Apto_NoApto_Ok(string nombre)
        {
            //ARRANGE
            var asignaturasCalificaciones = new List<AsignaturaCalificacionDto>
            {
                new()
                {
                    Calificacion = 0,
                    Convocatoria = 5,
                    NombreCalificacion = nombre,
                    Superada = true
                }
            };
            var sut = new GetAsignaturasExpedientesByIdExpedienteQueryHandler(Context, _mapper);

            //ACT
            var actual = sut.GetCalificacion(asignaturasCalificaciones);

            //ASSERT
            Assert.Equal(nombre, actual);
        }

        [Theory(DisplayName = "Cuando la nota es 0 y el nombre es diferente que Apto y No Apto Retorna la calificación con el valor de la Nota")]
        [InlineData("Suspensa")]
        [InlineData("Superada")]
        public void GetCalificacion_Diferente_Apto_NoApto_Ok(string nombre)
        {
            //ARRANGE
            var asignaturasCalificaciones = new List<AsignaturaCalificacionDto>
            {
                new()
                {
                    Calificacion = 0,
                    Convocatoria = 5,
                    NombreCalificacion = nombre,
                    Superada = true
                }
            };
            var sut = new GetAsignaturasExpedientesByIdExpedienteQueryHandler(Context, _mapper);

            //ACT
            var actual = sut.GetCalificacion(asignaturasCalificaciones);

            //ASSERT
            Assert.Equal($"{asignaturasCalificaciones.First().Calificacion} {nombre}", actual);
        }

        #endregion
    }
}
