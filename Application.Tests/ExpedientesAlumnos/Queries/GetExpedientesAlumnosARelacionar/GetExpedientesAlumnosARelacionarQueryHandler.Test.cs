using Moq;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Unir.Expedientes.Application.Common.Interfaces;
using Unir.Expedientes.Application.ExpedientesAlumnos.Queries.GetExpedientesAlumnosARelacionar;
using Unir.Expedientes.Application.Tests.Common;
using Unir.Expedientes.Domain.Entities;
using Xunit;

namespace Unir.Expedientes.Application.Tests.ExpedientesAlumnos.Queries.GetExpedientesAlumnosARelacionar
{
    [Collection("CommonTestCollection")]
    public class GetExpedientesAlumnosARelacionarQueryHandlerTest : TestBase
    {
        #region Handle

        [Fact(DisplayName = "Cuando se obtienen los expedientes Retorna Ok")]
        public async Task Handle_Ok()
        {
            //ARRANGE
            var expedientesAlumnos = Enumerable.Range(1, 3).Select(e => new ExpedienteAlumno
            {
                Id = e
            }).ToList();
            await Context.ExpedientesAlumno.AddRangeAsync(expedientesAlumnos);
            await Context.SaveChangesAsync(CancellationToken.None);
            var sut = new Mock<GetExpedientesAlumnosARelacionarQueryHandler>(Context)
            {
                CallBase = true
            };
            sut.Setup(x => x.ApplyQuery(It.IsAny<IQueryable<ExpedienteAlumno>>(),
                It.IsAny<GetExpedientesAlumnosARelacionarQuery>())).Returns(Context.ExpedientesAlumno.AsQueryable());
            var request = new GetExpedientesAlumnosARelacionarQuery();

            //ACT
            var actual = await sut.Object.Handle(request, CancellationToken.None);

            //ASSERT
            Assert.Equal(expedientesAlumnos.Count, actual.Count);
            sut.Verify(x => x.ApplyQuery(It.IsAny<IQueryable<ExpedienteAlumno>>(),
                It.IsAny<GetExpedientesAlumnosARelacionarQuery>()), Times.Once);
        }

        #endregion

        #region ApplyQuery

        [Fact(DisplayName = "Cuando se aplica el filtro de id expediente Devuelve Registros")]
        public async Task ApplyQuery_IdExpediente_Ok()
        {
            //ARRANGE
            await Context.ExpedientesAlumno.AddRangeAsync(Enumerable.Range(1, 3).Select(c =>
                new ExpedienteAlumno
                {
                    Id = c
                }));
            await Context.SaveChangesAsync();

            var sut = new GetExpedientesAlumnosARelacionarQueryHandler(Context);

            var idExpedienteAlumno = 1;
            var request = new GetExpedientesAlumnosARelacionarQuery
            {
                IdExpedienteAlumno = idExpedienteAlumno
            };
            //ACT
            var actual = sut.ApplyQuery(Context.ExpedientesAlumno.AsQueryable(), request);

            //ASSERT
            Assert.NotEmpty(actual);
            Assert.Single(actual);
            Assert.True(actual.Any(a => a.Id == idExpedienteAlumno));
        }

        [Fact(DisplayName = "Cuando se aplica el filtro de id universidad Devuelve Registros")]
        public async Task ApplyQuery_IdRefUniversidad_Ok()
        {
            //ARRANGE
            await Context.ExpedientesAlumno.AddRangeAsync(Enumerable.Range(1, 3).Select(c =>
                new ExpedienteAlumno
                {
                    Id = c,
                    IdRefUniversidad = c.ToString()
                }));
            await Context.SaveChangesAsync();

            var sut = new GetExpedientesAlumnosARelacionarQueryHandler(Context);

            var idRefUniversidad = "1";
            var request = new GetExpedientesAlumnosARelacionarQuery
            {
                IdRefUniversidad = idRefUniversidad
            };
            //ACT
            var actual = sut.ApplyQuery(Context.ExpedientesAlumno.AsQueryable(), request);

            //ASSERT
            Assert.NotEmpty(actual);
            Assert.Single(actual);
            Assert.True(actual.Any(a => a.IdRefUniversidad == idRefUniversidad));
        }

        [Fact(DisplayName = "Cuando se aplica el filtro de id estudio Devuelve Registros")]
        public async Task ApplyQuery_IdRefEstudio_Ok()
        {
            //ARRANGE
            await Context.ExpedientesAlumno.AddRangeAsync(Enumerable.Range(1, 3).Select(c =>
                new ExpedienteAlumno
                {
                    Id = c,
                    IdRefEstudio = c.ToString()
                }));
            await Context.SaveChangesAsync();

            var sut = new GetExpedientesAlumnosARelacionarQueryHandler(Context);

            var idRefEstudio = "1";
            var request = new GetExpedientesAlumnosARelacionarQuery
            {
                IdRefEstudio = idRefEstudio
            };
            //ACT
            var actual = sut.ApplyQuery(Context.ExpedientesAlumno.AsQueryable(), request);

            //ASSERT
            Assert.NotEmpty(actual);
            Assert.Single(actual);
            Assert.True(actual.Any(a => a.IdRefEstudio == idRefEstudio));
        }
        
        [Fact(DisplayName = "Cuando se aplica el filtro de fecha apertura desde Devuelve Registros")]
        public async Task ApplyQuery_FechaAperturaDesde_Ok()
        {
            //ARRANGE
            var currentDate = DateTime.Now;
            await Context.ExpedientesAlumno.AddRangeAsync(Enumerable.Range(1, 3).Select(c =>
                new ExpedienteAlumno
                {
                    Id = c,
                    FechaApertura = currentDate.AddDays(c)
                }));
            await Context.SaveChangesAsync();

            var sut = new GetExpedientesAlumnosARelacionarQueryHandler(Context);

            var request = new GetExpedientesAlumnosARelacionarQuery
            {
                FechaAperturaDesde = currentDate
            };
            //ACT
            var actual = sut.ApplyQuery(Context.ExpedientesAlumno.AsQueryable(), request);

            //ASSERT
            Assert.NotEmpty(actual);
            Assert.Equal(Context.ExpedientesAlumno.Count(), actual.Count(a => a.FechaApertura >= request.FechaAperturaDesde.Value));
        }

        [Fact(DisplayName = "Cuando se aplica el filtro de fecha apertura hasta Devuelve Registros")]
        public async Task ApplyQuery_FechaAperturaHasta_Ok()
        {
            //ARRANGE
            var currentDate = DateTime.Now;
            await Context.ExpedientesAlumno.AddRangeAsync(Enumerable.Range(1, 3).Select(c =>
                new ExpedienteAlumno
                {
                    Id = c,
                    FechaApertura = currentDate.AddDays(c)
                }));
            await Context.SaveChangesAsync();

            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };
            var sut = new GetExpedientesAlumnosARelacionarQueryHandler(Context);

            var request = new GetExpedientesAlumnosARelacionarQuery
            {
                FechaAperturaHasta = currentDate.AddDays(3)
            };
            //ACT
            var actual = sut.ApplyQuery(Context.ExpedientesAlumno.AsQueryable(), request);

            //ASSERT
            Assert.NotEmpty(actual);
            Assert.Equal(Context.ExpedientesAlumno.Count(), actual.Count(a => a.FechaApertura <= request.FechaAperturaHasta.Value));
        }

        #endregion
    }
}
