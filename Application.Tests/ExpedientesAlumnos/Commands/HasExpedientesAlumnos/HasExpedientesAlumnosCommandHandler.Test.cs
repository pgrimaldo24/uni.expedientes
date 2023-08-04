using Microsoft.EntityFrameworkCore;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Unir.Expedientes.Application.ExpedientesAlumnos.Commands.HasExpedientesAlumnos;
using Unir.Expedientes.Application.Tests.Common;
using Unir.Expedientes.Domain.Entities;
using Xunit;

namespace Unir.Expedientes.Application.Tests.ExpedientesAlumnos.Commands.HasExpedientesAlumnos
{
    [Collection("CommonTestCollection")]
    public class HasExpedientesAlumnosCommandHandlerTest : TestBase
    {
        #region Handle

        [Theory(DisplayName = "Cuando existen expedientes de alumnos Devuelve true")]
        [InlineData(1)]
        [InlineData(5)]
        [InlineData(20)]
        public async Task Handle_True(int cantidad)
        {
            //ARRANGE
            await Context.ExpedientesAlumno.AddRangeAsync(Enumerable.Range(1, cantidad).Select(i => new ExpedienteAlumno
            {
                Id = i
            }));
            await Context.SaveChangesAsync(CancellationToken.None);
            var request = new HasExpedientesAlumnosCommand();
            var sut = new Mock<HasExpedientesAlumnosCommandHandler>(Context)
            {
                CallBase = true
            };
            var queryableEsperado = Context.ExpedientesAlumno.AsQueryable();
            sut.Setup(s => s.ApplyQuery(It.IsAny<IQueryable<ExpedienteAlumno>>(),
                It.Is<HasExpedientesAlumnosCommand>(c => c == request))).Returns(queryableEsperado);

            //ACT
            var actual = await sut.Object.Handle(request, CancellationToken.None);

            //ASSERT
            Assert.True(actual);
        }

        [Fact(DisplayName = "Cuando no existen expedientes de alumnos Devuelve false")]
        public async Task Handle_False()
        {
            //ARRANGE
            var request = new HasExpedientesAlumnosCommand();
            var sut = new Mock<HasExpedientesAlumnosCommandHandler>(Context)
            {
                CallBase = true
            };
            var queryableEsperado = Context.ExpedientesAlumno.AsQueryable();
            sut.Setup(s => s.ApplyQuery(It.IsAny<IQueryable<ExpedienteAlumno>>(),
                It.Is<HasExpedientesAlumnosCommand>(c => c == request))).Returns(queryableEsperado);

            //ACT
            var actual = await sut.Object.Handle(request, CancellationToken.None);

            //ASSERT
            Assert.False(actual);
        }

        #endregion

        #region ApplyQuery

        [Fact(DisplayName = "Cuando se aplica el filtro de id plan Devuelve registros")]
        public async Task ApplyQuery_FilterIdPlan()
        {
            //ARRANGE
            var sut = new HasExpedientesAlumnosCommandHandler(Context);

            await Context.ExpedientesAlumno.AddRangeAsync(Enumerable.Range(1, 3).Select(c =>
                new ExpedienteAlumno
                {
                    Id = c,
                    IdRefPlan = c.ToString()
                }));
            await Context.SaveChangesAsync();

            var request = new HasExpedientesAlumnosCommand
            {
                FilterIdPlan = 1
            };

            //ACT
            var actual = sut.ApplyQuery(Context.ExpedientesAlumno.AsQueryable(), request);

            //ASSERT
            Assert.NotEmpty(actual);
            Assert.Single(actual);
            Assert.True(actual.Any(a => a.IdRefPlan == "1"));
        }

        [Fact(DisplayName = "Cuando se aplica el filtro de id integración alumno Devuelve registros")]
        public async Task ApplyQuery_FilterIdRefIntegracionAlumno()
        {
            //ARRANGE
            var sut = new HasExpedientesAlumnosCommandHandler(Context);

            await Context.ExpedientesAlumno.AddRangeAsync(Enumerable.Range(1, 3).Select(c =>
                new ExpedienteAlumno
                {
                    Id = c,
                    IdRefIntegracionAlumno = c.ToString()
                }));
            await Context.SaveChangesAsync();

            var request = new HasExpedientesAlumnosCommand
            {
                FilterIdRefIntegracionAlumno = "1"
            };

            //ACT
            var actual = sut.ApplyQuery(Context.ExpedientesAlumno.AsQueryable(), request);

            //ASSERT
            Assert.NotEmpty(actual);
            Assert.Single(actual);
            Assert.True(actual.Any(a => a.IdRefIntegracionAlumno == "1"));
        }

        [Fact(DisplayName = "Cuando se aplica el filtro de ids versión plan Devuelve registros")]
        public async Task ApplyQuery_FiltersIdsRefVersionPlan()
        {
            //ARRANGE
            var sut = new HasExpedientesAlumnosCommandHandler(Context);

            await Context.ExpedientesAlumno.AddRangeAsync(Enumerable.Range(1, 3).Select(c =>
                new ExpedienteAlumno
                {
                    Id = c,
                    IdRefPlan = c.ToString(),
                    IdRefIntegracionAlumno = c.ToString(),
                    IdRefVersionPlan = c.ToString()
                }));
            await Context.SaveChangesAsync();

            var request = new HasExpedientesAlumnosCommand
            {
                FiltersIdsRefVersionPlan = new List<string>
                {
                    "1", "2"
                }
            };

            //ACT
            var actual = sut.ApplyQuery(Context.ExpedientesAlumno.AsQueryable(), request);

            //ASSERT
            Assert.NotEmpty(actual);
            Assert.Equal(2, actual.Count());
            Assert.True(actual.Any(a => a.IdRefVersionPlan == "1"));
            Assert.True(actual.Any(a => a.IdRefVersionPlan == "2"));
        }

        [Fact(DisplayName = "Cuando se aplica el filtro de ids ref plan Devuelve registros")]
        public async Task ApplyQuery_FiltersIdsRefPlan()
        {
            //ARRANGE
            var sut = new HasExpedientesAlumnosCommandHandler(Context);

            await Context.ExpedientesAlumno.AddRangeAsync(Enumerable.Range(1, 3).Select(c =>
                new ExpedienteAlumno
                {
                    Id = c,
                    IdRefPlan = c.ToString()
                }));
            await Context.SaveChangesAsync();

            var request = new HasExpedientesAlumnosCommand
            {
                FiltersIdsRefPlan = new List<string>
                {
                    "1", "2"
                }
            };

            //ACT
            var actual = sut.ApplyQuery(Context.ExpedientesAlumno.AsQueryable(), request);

            //ASSERT
            Assert.NotEmpty(actual);
            Assert.Equal(2, actual.Count());
            Assert.True(actual.Any(a => a.IdRefPlan == "1"));
            Assert.True(actual.Any(a => a.IdRefPlan == "2"));
        }

        [Fact(DisplayName = "Cuando se aplica el filtro de id seguimiento Devuelve registros")]
        public async Task ApplyQuery_FilterIdSeguimientos()
        {
            //ARRANGE
            var sut = new HasExpedientesAlumnosCommandHandler(Context);

            await Context.ExpedientesAlumno.AddRangeAsync(Enumerable.Range(1, 3).Select(c =>
                new ExpedienteAlumno
                {
                    Id = c,
                    IdRefPlan = c.ToString()
                }));
            await Context.SaveChangesAsync();
            const int idSeguimientoEsperado = 100000;
            await Context.SeguimientosExpediente.AddAsync(new SeguimientoExpediente
            {
                Id = idSeguimientoEsperado,
                ExpedienteAlumno = await Context.ExpedientesAlumno.FirstAsync(CancellationToken.None)
            });
            await Context.SaveChangesAsync(CancellationToken.None);

            var request = new HasExpedientesAlumnosCommand
            {
                FilterIdSeguimientos = idSeguimientoEsperado
            };

            //ACT
            var actual = sut.ApplyQuery(Context.ExpedientesAlumno.AsQueryable(), request);

            //ASSERT
            Assert.NotEmpty(actual);
            Assert.Single(actual);
            Assert.True(actual.Any(a => a.Seguimientos.First().Id == idSeguimientoEsperado));
        }

        [Fact(DisplayName = "Cuando se aplica el filtro de id ref nodo Devuelve registros")]
        public async Task ApplyQuery_FilterIdRefNodo()
        {
            //ARRANGE
            var sut = new HasExpedientesAlumnosCommandHandler(Context);

            await Context.ExpedientesAlumno.AddRangeAsync(Enumerable.Range(1, 3).Select(c =>
                new ExpedienteAlumno
                {
                    Id = c,
                    IdRefNodo = c.ToString()
                }));
            await Context.SaveChangesAsync();

            var request = new HasExpedientesAlumnosCommand
            {
                FilterIdRefNodo = "1"
            };

            //ACT
            var actual = sut.ApplyQuery(Context.ExpedientesAlumno.AsQueryable(), request);

            //ASSERT
            Assert.NotEmpty(actual);
            Assert.Single(actual);
            Assert.True(actual.Any(a => a.IdRefNodo == "1"));
        }

        #endregion
    }
}
