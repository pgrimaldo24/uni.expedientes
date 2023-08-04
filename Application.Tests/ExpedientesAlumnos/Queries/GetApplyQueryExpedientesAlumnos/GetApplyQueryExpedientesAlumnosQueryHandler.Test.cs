using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Unir.Expedientes.Application.ExpedientesAlumnos.Queries.GetApplyQueryExpedientesAlumnos;
using Unir.Expedientes.Application.Tests.Common;
using Unir.Expedientes.Domain.Entities;
using Xunit;

namespace Unir.Expedientes.Application.Tests.ExpedientesAlumnos.Queries.GetApplyQueryExpedientesAlumnos
{
    [Collection("CommonTestCollection")]
    public class GetApplyQueryExpedientesAlumnosQueryHandlerTests : TestBase
    {
        #region Handle

        [Fact(DisplayName = "Cuando se aplica el filtro de id expediente Devuelve registros")]
        public async Task Handle_FilterIdExpedienteAlumno()
        {
            //ARRANGE
            var sut = new GetApplyQueryExpedientesAlumnosQueryHandler();

            await Context.ExpedientesAlumno.AddRangeAsync(Enumerable.Range(1, 3).Select(e =>
                new ExpedienteAlumno
                {
                    Id = e
                }));
            await Context.SaveChangesAsync();

            var queryable = Context.ExpedientesAlumno.AsQueryable();
            var filter = new ApplyQueryDto
            {
                FilterIdExpedienteAlumno = 1
            };
            var request = new GetApplyQueryExpedientesAlumnosQuery(queryable, filter);

            //ACT
            var actual = await sut.Handle(request, CancellationToken.None);

            //ASSERT
            Assert.NotEmpty(actual);
            Assert.Single(actual);
            Assert.True(actual.Any(a => a.Id == filter.FilterIdExpedienteAlumno));
        }

        [Fact(DisplayName = "Cuando se aplica el filtro de id ref universidad Devuelve registros")]
        public async Task Handle_FilterIdRefUniversidad()
        {
            //ARRANGE
            var sut = new GetApplyQueryExpedientesAlumnosQueryHandler();

            await Context.ExpedientesAlumno.AddRangeAsync(Enumerable.Range(1, 3).Select(e =>
                new ExpedienteAlumno
                {
                    Id = e,
                    IdRefUniversidad = e.ToString()
                }));
            await Context.SaveChangesAsync();

            var queryable = Context.ExpedientesAlumno.AsQueryable();
            var filter = new ApplyQueryDto
            {
                FilterIdRefUniversidad = 1
            };
            var request = new GetApplyQueryExpedientesAlumnosQuery(queryable, filter);

            //ACT
            var actual = await sut.Handle(request, CancellationToken.None);

            //ASSERT
            Assert.NotEmpty(actual);
            Assert.Single(actual);
            Assert.True(actual.Any(a => a.IdRefUniversidad == filter.FilterIdRefUniversidad.ToString()));
        }

        [Fact(DisplayName = "Cuando se aplica el filtro de ids ref plan Devuelve registros")]
        public async Task Handle_FiltersIdsRefPlan()
        {
            //ARRANGE
            var sut = new GetApplyQueryExpedientesAlumnosQueryHandler();

            await Context.ExpedientesAlumno.AddRangeAsync(Enumerable.Range(1, 3).Select(e =>
                new ExpedienteAlumno
                {
                    Id = e,
                    IdRefPlan = e.ToString()
                }));
            await Context.SaveChangesAsync();

            var queryable = Context.ExpedientesAlumno.AsQueryable();
            var filter = new ApplyQueryDto
            {
                FiltersIdsRefPlan = new List<string>
                {
                    "1", "2"
                }
            };
            var request = new GetApplyQueryExpedientesAlumnosQuery(queryable, filter);

            //ACT
            var actual = await sut.Handle(request, CancellationToken.None);

            //ASSERT
            Assert.NotEmpty(actual);
            Assert.Equal(2, actual.Count());
            Assert.True(actual.Any(a => a.IdRefPlan == "1"));
            Assert.True(actual.Any(a => a.IdRefPlan == "2"));
        }

        [Fact(DisplayName = "Cuando se aplica el filtro de id plan Devuelve registros")]
        public async Task Handle_FilterIdRefPlan()
        {
            //ARRANGE
            var sut = new GetApplyQueryExpedientesAlumnosQueryHandler();

            await Context.ExpedientesAlumno.AddRangeAsync(Enumerable.Range(1, 3).Select(e =>
                new ExpedienteAlumno
                {
                    Id = e,
                    IdRefPlan = e.ToString()
                }));
            await Context.SaveChangesAsync();

            var queryable = Context.ExpedientesAlumno.AsQueryable();
            var filter = new ApplyQueryDto
            {
                FilterIdRefPlan = 1
            };
            var request = new GetApplyQueryExpedientesAlumnosQuery(queryable, filter);

            //ACT
            var actual = await sut.Handle(request, CancellationToken.None);

            //ASSERT
            Assert.NotEmpty(actual);
            Assert.Single(actual);
            Assert.True(actual.Any(a => a.IdRefPlan == "1"));
        }

        [Fact(DisplayName = "Cuando se aplica el filtro de nombre del alumno Devuelve registros")]
        public async Task Handle_FilterNombreAlumno()
        {
            //ARRANGE
            var sut = new GetApplyQueryExpedientesAlumnosQueryHandler();

            var nombreAlumno = Guid.NewGuid().ToString();
            await Context.ExpedientesAlumno.AddRangeAsync(Enumerable.Range(1, 3).Select(e =>
                new ExpedienteAlumno
                {
                    Id = e,
                    AlumnoNombre = e == 1 ? nombreAlumno : null
                }));
            await Context.SaveChangesAsync();

            var queryable = Context.ExpedientesAlumno.AsQueryable();
            var filter = new ApplyQueryDto
            {
                FilterNombreAlumno = nombreAlumno[..8]
            };
            var request = new GetApplyQueryExpedientesAlumnosQuery(queryable, filter);

            //ACT
            var actual = await sut.Handle(request, CancellationToken.None);

            //ASSERT
            Assert.NotEmpty(actual);
            Assert.Single(actual);
            Assert.True(actual.Any(a => a.AlumnoNombre.StartsWith(nombreAlumno)));
        }

        [Fact(DisplayName = "Cuando se aplica el filtro de primer apellido del alumno Devuelve registros")]
        public async Task Handle_FilterPrimerApellido()
        {
            //ARRANGE
            var sut = new GetApplyQueryExpedientesAlumnosQueryHandler();

            var primerApellido = Guid.NewGuid().ToString();
            await Context.ExpedientesAlumno.AddRangeAsync(Enumerable.Range(1, 3).Select(e =>
                new ExpedienteAlumno
                {
                    Id = e,
                    AlumnoApellido1 = e == 1 ? primerApellido : null
                }));
            await Context.SaveChangesAsync();

            var queryable = Context.ExpedientesAlumno.AsQueryable();
            var filter = new ApplyQueryDto
            {
                FilterPrimerApellido = primerApellido[..8]
            };
            var request = new GetApplyQueryExpedientesAlumnosQuery(queryable, filter);

            //ACT
            var actual = await sut.Handle(request, CancellationToken.None);

            //ASSERT
            Assert.NotEmpty(actual);
            Assert.Single(actual);
            Assert.True(actual.Any(a => a.AlumnoApellido1.StartsWith(primerApellido)));
        }

        [Fact(DisplayName = "Cuando se aplica el filtro de segundo apellido del alumno Devuelve registros")]
        public async Task Handle_FilterSegundoApellido()
        {
            //ARRANGE
            var sut = new GetApplyQueryExpedientesAlumnosQueryHandler();

            var segundoApellido = Guid.NewGuid().ToString();
            await Context.ExpedientesAlumno.AddRangeAsync(Enumerable.Range(1, 3).Select(e =>
                new ExpedienteAlumno
                {
                    Id = e,
                    AlumnoApellido2 = e == 1 ? segundoApellido : null
                }));
            await Context.SaveChangesAsync();

            var queryable = Context.ExpedientesAlumno.AsQueryable();
            var filter = new ApplyQueryDto
            {
                FilterSegundoApellido = segundoApellido[..8]
            };
            var request = new GetApplyQueryExpedientesAlumnosQuery(queryable, filter);

            //ACT
            var actual = await sut.Handle(request, CancellationToken.None);

            //ASSERT
            Assert.NotEmpty(actual);
            Assert.Single(actual);
            Assert.True(actual.Any(a => a.AlumnoApellido2.StartsWith(segundoApellido)));
        }

        [Fact(DisplayName = "Cuando se aplica el filtro de nro identificación de alumno Devuelve registros")]
        public async Task Handle_FilterNroDocIdentificacion()
        {
            //ARRANGE
            var sut = new GetApplyQueryExpedientesAlumnosQueryHandler();

            var nroDocIdentificacion = Guid.NewGuid().ToString();
            await Context.ExpedientesAlumno.AddRangeAsync(Enumerable.Range(1, 3).Select(e =>
                new ExpedienteAlumno
                {
                    Id = e,
                    AlumnoNroDocIdentificacion = e == 1 ? nroDocIdentificacion : null
                }));
            await Context.SaveChangesAsync();

            var queryable = Context.ExpedientesAlumno.AsQueryable();
            var filter = new ApplyQueryDto
            {
                FilterNroDocIdentificacion = nroDocIdentificacion[..8]
            };
            var request = new GetApplyQueryExpedientesAlumnosQuery(queryable, filter);

            //ACT
            var actual = await sut.Handle(request, CancellationToken.None);

            //ASSERT
            Assert.NotEmpty(actual);
            Assert.Single(actual);
            Assert.True(actual.Any(a => a.AlumnoNroDocIdentificacion.StartsWith(nroDocIdentificacion)));
        }

        [Fact(DisplayName = "Cuando se aplica el filtro de id integración alumno Devuelve registros")]
        public async Task Handle_FilterIdRefIntegracionAlumno()
        {
            //ARRANGE
            var sut = new GetApplyQueryExpedientesAlumnosQueryHandler();

            await Context.ExpedientesAlumno.AddRangeAsync(Enumerable.Range(1, 3).Select(e =>
                new ExpedienteAlumno
                {
                    Id = e,
                    IdRefIntegracionAlumno = e.ToString()
                }));
            await Context.SaveChangesAsync();

            var queryable = Context.ExpedientesAlumno.AsQueryable();
            var filter = new ApplyQueryDto
            {
                FilterIdRefIntegracionAlumno = "1"
            };
            var request = new GetApplyQueryExpedientesAlumnosQuery(queryable, filter);

            //ACT
            var actual = await sut.Handle(request, CancellationToken.None);

            //ASSERT
            Assert.NotEmpty(actual);
            Assert.Single(actual);
            Assert.True(actual.Any(a => a.IdRefIntegracionAlumno == "1"));
        }

        #endregion
    }
}
