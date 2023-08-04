using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Unir.Expedientes.Application.ExpedientesAlumnos.Queries.GetAllExpedientesAlumnos;
using Unir.Expedientes.Application.Tests.Common;
using Unir.Expedientes.Domain.Entities;
using Xunit;

namespace Unir.Expedientes.Application.Tests.ExpedientesAlumnos.Queries.GetAllExpedientesAlumnos
{
    [Collection("CommonTestCollection")]
    public class GetAllExpedientesAlumnosQueryHandlerTest : TestBase
    {
        private readonly IMapper _mapper;

        public GetAllExpedientesAlumnosQueryHandlerTest(CommonTestFixture fixture)
        {
            _mapper = fixture.Mapper;
        }

        #region Handle

        [Theory(DisplayName = "Cuando existen registros Devuelve elementos")]
        [InlineData(5)]
        [InlineData(10)]
        [InlineData(20)]
        public async Task Handle_Ok(int cantidad)
        {
            //ARRANGE
            await Context.ExpedientesAlumno.AddRangeAsync(Enumerable.Range(1, cantidad)
                .Select(e => new ExpedienteAlumno
                {
                    Id = e
                }));
            await Context.SaveChangesAsync(CancellationToken.None);

            var request = new GetAllExpedientesAlumnosQuery
            {
                Limit = cantidad,
                Offset = 1
            };
            
            var sut = new Mock<GetAllExpedientesAlumnosQueryHandler>(Context, _mapper)
            {
                CallBase = true
            };
            sut.Setup(s => s.ApplyQuery(It.IsAny<IQueryable<ExpedienteAlumno>>(),
                    It.IsAny<GetAllExpedientesAlumnosQuery>()))
                .Returns(Context.ExpedientesAlumno.AsQueryable());

            //ACT
            var actual = await sut.Object.Handle(request, CancellationToken.None);

            //ASSERT
            Assert.Equal(cantidad, actual.Length);
        }

        #endregion

        #region ApplyQuery

        [Fact(DisplayName = "Cuando se aplica el filtro de id expediente alumno Devuelve registro")]
        public async Task ApplyQuery_FilterIdExpedienteAlumno()
        {
            //ARRANGE
            var sut = new GetAllExpedientesAlumnosQueryHandler(Context, _mapper);

            await Context.ExpedientesAlumno.AddRangeAsync(Enumerable.Range(1, 3).Select(c =>
                new ExpedienteAlumno
                {
                    Id = c
                }));
            await Context.SaveChangesAsync();

            var request = new GetAllExpedientesAlumnosQuery
            {
                FilterIdExpedienteAlumno = 1
            };

            //ACT
            var actual = sut.ApplyQuery(Context.ExpedientesAlumno.AsQueryable(), request);

            //ASSERT
            Assert.NotEmpty(actual);
            Assert.Single(actual);
            Assert.True(actual.Any(a => a.Id == 1));
        }

        [Fact(DisplayName = "Cuando se aplica el filtro de ids expedientes alumnos Devuelve registros")]
        public async Task ApplyQuery_FiltersIdsExpedientesAlumnos()
        {
            //ARRANGE
            var sut = new GetAllExpedientesAlumnosQueryHandler(Context, _mapper);

            await Context.ExpedientesAlumno.AddRangeAsync(Enumerable.Range(1, 3).Select(c =>
                new ExpedienteAlumno
                {
                    Id = c
                }));
            await Context.SaveChangesAsync();

            var request = new GetAllExpedientesAlumnosQuery
            {
                FiltersIdsExpedientesAlumnos = new List<int> {1, 2, 3}
            };

            //ACT
            var actual = sut.ApplyQuery(Context.ExpedientesAlumno.AsQueryable(), request);

            //ASSERT
            Assert.NotEmpty(actual);
            Assert.True(actual.All(a => request.FiltersIdsExpedientesAlumnos.Contains(a.Id)));
        }

        [Fact(DisplayName = "Cuando se aplica el filtro de id plan Devuelve registros")]
        public async Task ApplyQuery_FilterIdPlan()
        {
            //ARRANGE
            var sut = new GetAllExpedientesAlumnosQueryHandler(Context, _mapper);

            await Context.ExpedientesAlumno.AddRangeAsync(Enumerable.Range(1, 3).Select(c =>
                new ExpedienteAlumno
                {
                    Id = c,
                    IdRefPlan = c.ToString()
                }));
            await Context.SaveChangesAsync();

            var request = new GetAllExpedientesAlumnosQuery
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
            var sut = new GetAllExpedientesAlumnosQueryHandler(Context, _mapper);

            await Context.ExpedientesAlumno.AddRangeAsync(Enumerable.Range(1, 3).Select(c =>
                new ExpedienteAlumno
                {
                    Id = c,
                    IdRefIntegracionAlumno = c.ToString()
                }));
            await Context.SaveChangesAsync();

            var request = new GetAllExpedientesAlumnosQuery
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
            var sut = new GetAllExpedientesAlumnosQueryHandler(Context, _mapper);

            await Context.ExpedientesAlumno.AddRangeAsync(Enumerable.Range(1, 3).Select(c =>
                new ExpedienteAlumno
                {
                    Id = c,
                    IdRefPlan = c.ToString(),
                    IdRefIntegracionAlumno = c.ToString(),
                    IdRefVersionPlan = c.ToString()
                }));
            await Context.SaveChangesAsync();

            var request = new GetAllExpedientesAlumnosQuery
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
            var sut = new GetAllExpedientesAlumnosQueryHandler(Context, _mapper);

            await Context.ExpedientesAlumno.AddRangeAsync(Enumerable.Range(1, 3).Select(c =>
                new ExpedienteAlumno
                {
                    Id = c,
                    IdRefPlan = c.ToString()
                }));
            await Context.SaveChangesAsync();

            var request = new GetAllExpedientesAlumnosQuery
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
            var sut = new GetAllExpedientesAlumnosQueryHandler(Context, _mapper);

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

            var request = new GetAllExpedientesAlumnosQuery
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
            var sut = new GetAllExpedientesAlumnosQueryHandler(Context, _mapper);

            await Context.ExpedientesAlumno.AddRangeAsync(Enumerable.Range(1, 3).Select(c =>
                new ExpedienteAlumno
                {
                    Id = c,
                    IdRefNodo = c.ToString()
                }));
            await Context.SaveChangesAsync();

            var request = new GetAllExpedientesAlumnosQuery
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
