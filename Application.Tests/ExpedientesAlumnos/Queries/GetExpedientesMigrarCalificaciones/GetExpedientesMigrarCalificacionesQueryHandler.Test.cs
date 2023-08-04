using Moq;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Unir.Expedientes.Application.ExpedientesAlumnos.Queries.GetExpedientesMigrarCalificaciones;
using Unir.Expedientes.Application.Tests.Common;
using Unir.Expedientes.Domain.Entities;
using Xunit;

namespace Unir.Expedientes.Application.Tests.ExpedientesAlumnos.Queries.GetExpedientesMigrarCalificaciones
{
    [Collection("CommonTestCollection")]
    public class GetExpedientesMigrarCalificacionesQueryHandlerTest : TestBase
    {
        #region Handle
        [Fact(DisplayName = "Cuando Handle es correcto Retorna lista de expedientes")]
        public async Task Handle_Ok()
        {
            //ARRANGE
            var cantidadEsperada = 1;
            var expedienteAlumno = new ExpedienteAlumno
            {
                Id = 1
            };
            await Context.ExpedientesAlumno.AddAsync(expedienteAlumno);
            await Context.SaveChangesAsync();

            var sutMock = new Mock<GetExpedientesMigrarCalificacionesQueryHandler>(Context);
            sutMock.Setup(x => x.ApplyQuery(It.IsAny<IQueryable<ExpedienteAlumno>>(), It.IsAny<GetExpedientesMigrarCalificacionesQuery>()))
            .Returns(Context.ExpedientesAlumno.AsQueryable());

            //ACT
            var actual = await sutMock.Object.Handle(new GetExpedientesMigrarCalificacionesQuery(), CancellationToken.None);

            //ASSERT
            Assert.NotNull(actual);
            Assert.Equal(cantidadEsperada, actual.Count);
            sutMock.Verify(x => x.ApplyQuery(It.IsAny<IQueryable<ExpedienteAlumno>>(), It.IsAny<GetExpedientesMigrarCalificacionesQuery>()), Times.Once);
        }

        #endregion

        #region ApplyQuery
        [Fact(DisplayName = "Cuando los filtros coinciden Retorna lista de expedientes")]
        public async Task ApplyQuery_Ok()
        {
            //ARRANGE
            var expedienteAlumno = new ExpedienteAlumno
            {
                Id = 1,
                IdRefUniversidad = "1",
                IdRefEstudio = "1",

            };
            await Context.ExpedientesAlumno.AddAsync(expedienteAlumno);
            await Context.SaveChangesAsync();

            var sut = new GetExpedientesMigrarCalificacionesQueryHandler(Context);

            //ACT
            var actual = sut.ApplyQuery(Context.ExpedientesAlumno.AsQueryable(), new GetExpedientesMigrarCalificacionesQuery { IdRefEstudio = "1", IdRefUniversidad = "1" });

            //ASSERT
            Assert.NotNull(actual);
            Assert.IsAssignableFrom<IQueryable<ExpedienteAlumno>>(actual);
            Assert.True(actual.ToList().Count == 1);
        }

        [Fact(DisplayName = "Cuando los filtros no coinciden Retorna lista de vacía")]
        public async Task ApplyQuery_Vacio()
        {
            //ARRANGE
            var expedienteAlumno = new ExpedienteAlumno
            {
                Id = 1,
                IdRefUniversidad = null,
                IdRefEstudio = null,

            };
            await Context.ExpedientesAlumno.AddAsync(expedienteAlumno);
            await Context.SaveChangesAsync();

            var sut = new GetExpedientesMigrarCalificacionesQueryHandler(Context);

            //ACT
            var actual = sut.ApplyQuery(Context.ExpedientesAlumno.AsQueryable(), new GetExpedientesMigrarCalificacionesQuery { IdRefEstudio = "1", IdRefUniversidad = "1" });

            //ASSERT
            Assert.NotNull(actual);
            Assert.IsAssignableFrom<IQueryable<ExpedienteAlumno>>(actual);
            Assert.True(actual.ToList().Count == 0);
        }

        #endregion

    }
}
