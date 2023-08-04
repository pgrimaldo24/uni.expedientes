using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Unir.Expedientes.Application.RequisitosComportamientosExpedientes.Queries.GetRequisitosComportamientosExpedientesAConsolidar;
using Unir.Expedientes.Application.Tests.Common;
using Unir.Expedientes.Domain.Entities;
using Xunit;

namespace Unir.Expedientes.Application.Tests.RequisitosComportamientosExpedientes.Queries.GetRequisitosComportamientosExpedientesAConsolidar
{
    [Collection("CommonTestCollection")]
    public class GetRequisitosComportamientosExpedientesAConsolidarQueryHandlerTests : TestBase
    {
        #region Handle

        [Fact(DisplayName = "Cuando no existen comportamientos Termina el proceso")]
        public async Task Handle_Empty_Requisitos()
        {
            //ARRANGE
            var expediente = new ExpedienteAlumno
            {
                Id = 1
            };
            var request = new GetRequisitosComportamientosExpedientesAConsolidarQuery(expediente);
            var sutMock = new Mock<GetRequisitosComportamientosExpedientesAConsolidarQueryHandler>(Context)
            {
                CallBase = true
            };

            sutMock.Setup(x => x.GetIdsComportamientosAsignaturasNivelesDeUso(
                It.IsAny<int>(), It.IsAny<CancellationToken>())).ReturnsAsync(new List<int>());
            sutMock.Setup(x => x.GetIdsComportamientosExpedientesNivelesDeUso(
                It.IsAny<ExpedienteAlumno>(), It.IsAny<CancellationToken>())).ReturnsAsync(new List<int>());

            //ACT
            var actual = await sutMock.Object.Handle(request, CancellationToken.None);

            //ASSERT
            Assert.Null(actual);
            sutMock.Verify(x => x.GetIdsComportamientosAsignaturasNivelesDeUso(
                It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Once);
            sutMock.Verify(x => x.GetIdsComportamientosExpedientesNivelesDeUso(
                It.IsAny<ExpedienteAlumno>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact(DisplayName = "Cuando se obtienen los requisitos comportamientos Devuelve lista")]
        public async Task Handle_Ok()
        {
            //ARRANGE
            var expediente = new ExpedienteAlumno
            {
                Id = 1,
                ConsolidacionesRequisitosExpedientes = new List<ConsolidacionRequisitoExpediente>()
            };
            var request = new GetRequisitosComportamientosExpedientesAConsolidarQuery(expediente);
            var sutMock = new Mock<GetRequisitosComportamientosExpedientesAConsolidarQueryHandler>(Context)
            {
                CallBase = true
            };

            sutMock.Setup(x => x.GetIdsComportamientosAsignaturasNivelesDeUso(
                It.IsAny<int>(), It.IsAny<CancellationToken>())).ReturnsAsync(new List<int> { 1 });
            sutMock.Setup(x => x.GetIdsComportamientosExpedientesNivelesDeUso(
                It.IsAny<ExpedienteAlumno>(), It.IsAny<CancellationToken>())).ReturnsAsync(new List<int> { 2 });

            var comportamientos = Enumerable.Range(1, 2).Select(c => new ComportamientoExpediente
            {
                Id = c,
                RequisitosComportamientosExpedientes = new List<RequisitoComportamientoExpediente>
                {
                    new()
                    {
                        Id = c,
                        RequisitoExpediente = new RequisitoExpediente
                        {
                            Id = c,
                            Bloqueado = false
                        }
                    }
                }
            });
            await Context.ComportamientosExpedientes.AddRangeAsync(comportamientos);
            await Context.SaveChangesAsync();

            //ACT
            var actual = await sutMock.Object.Handle(request, CancellationToken.None);

            //ASSERT
            Assert.NotEmpty(actual);
            sutMock.Verify(x => x.GetIdsComportamientosAsignaturasNivelesDeUso(
                It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Once);
            sutMock.Verify(x => x.GetIdsComportamientosExpedientesNivelesDeUso(
                It.IsAny<ExpedienteAlumno>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        #endregion

        #region GetIdsComportamientosAsignaturasNivelesDeUso

        [Fact(DisplayName = "Cuando no existen asignaturas por el id del expediente Devuelve lista vacía")]
        public async Task GetIdsComportamientosAsignaturasNivelesDeUso_Empty()
        {
            //ARRANGE
            var expediente = new ExpedienteAlumno
            {
                Id = 1
            };
            var sut = new GetRequisitosComportamientosExpedientesAConsolidarQueryHandler(Context);
            await Context.AsignaturasExpedientes.AddAsync(new AsignaturaExpediente
            {
                Id = 1
            });
            await Context.SaveChangesAsync();

            //ACT
            var actual = await sut.GetIdsComportamientosAsignaturasNivelesDeUso(expediente.Id, CancellationToken.None);

            //ASSERT
            Assert.Empty(actual);
        }

        [Fact(DisplayName = "Cuando se obtienen los ids de comportamientos de las asignaturas Devuelve lista")]
        public async Task GetIdsComportamientosAsignaturasNivelesDeUso_Ok()
        {
            //ARRANGE
            var expediente = new ExpedienteAlumno
            {
                Id = 1
            };
            var sut = new GetRequisitosComportamientosExpedientesAConsolidarQueryHandler(Context);
            await Context.AsignaturasExpedientes.AddAsync(new AsignaturaExpediente
            {
                Id = 1,
                ExpedienteAlumnoId = 1,
                IdRefTipoAsignatura = "1",
                SituacionAsignaturaId = SituacionAsignatura.Matriculada
            });
            await Context.AsignaturasExpedientes.AddAsync(new AsignaturaExpediente
            {
                Id = 2,
                ExpedienteAlumnoId = 1,
                IdRefAsignaturaPlan = "2",
                SituacionAsignaturaId = SituacionAsignatura.NoPresentada
            });

            await Context.NivelesUsoComportamientosExpedientes.AddAsync(new NivelUsoComportamientoExpediente
            {
                Id = 1,
                TipoNivelUsoId = TipoNivelUso.TipoAsignatura,
                IdRefTipoAsignatura = "1",
                ComportamientoExpedienteId = 1
            });
            await Context.NivelesUsoComportamientosExpedientes.AddAsync(new NivelUsoComportamientoExpediente
            {
                Id = 2,
                TipoNivelUsoId = TipoNivelUso.AsignaturaPlan,
                IdRefAsignaturaPlan = "2",
                ComportamientoExpedienteId = 2
            });
            await Context.SaveChangesAsync();

            //ACT
            var actual = await sut.GetIdsComportamientosAsignaturasNivelesDeUso(expediente.Id, CancellationToken.None);

            //ASSERT
            Assert.NotEmpty(actual);
        }

        #endregion

        #region GetIdsComportamientosExpedientesNivelesDeUso

        [Fact(DisplayName = "Cuando se obtienen los ids de comportamientos del expediente Devuelve lista")]
        public async Task GetIdsComportamientosExpedientesNivelesDeUso_Ok()
        {
            //ARRANGE
            var expediente = new ExpedienteAlumno
            {
                Id = 1,
                IdRefUniversidad = "1",
                IdRefTipoEstudio = "2",
                IdRefEstudio = "3",
                IdRefPlan = "4"
            };
            var sut = new GetRequisitosComportamientosExpedientesAConsolidarQueryHandler(Context);

            var nivelesUsoUniversidad = new NivelUsoComportamientoExpediente
            {
                Id = 1,
                TipoNivelUsoId = TipoNivelUso.Universidad,
                IdRefUniversidad = "1",
                ComportamientoExpedienteId = 1
            };
            var nivelesUsoTipoEstudio = new NivelUsoComportamientoExpediente
            {
                Id = 2,
                TipoNivelUsoId = TipoNivelUso.TipoEstudio,
                IdRefTipoEstudio = "2",
                ComportamientoExpedienteId = 2
            };
            var nivelesUsoEstudio = new NivelUsoComportamientoExpediente
            {
                Id = 3,
                TipoNivelUsoId = TipoNivelUso.Estudio,
                IdRefEstudio = "3",
                ComportamientoExpedienteId = 3
            };
            var nivelesUsoPlanEstudio = new NivelUsoComportamientoExpediente
            {
                Id = 4,
                TipoNivelUsoId = TipoNivelUso.PlanEstudio,
                IdRefPlan = "4",
                ComportamientoExpedienteId = 4
            };
            await Context.NivelesUsoComportamientosExpedientes.AddAsync(nivelesUsoUniversidad);
            await Context.NivelesUsoComportamientosExpedientes.AddAsync(nivelesUsoTipoEstudio);
            await Context.NivelesUsoComportamientosExpedientes.AddAsync(nivelesUsoEstudio);
            await Context.NivelesUsoComportamientosExpedientes.AddAsync(nivelesUsoPlanEstudio);
            await Context.SaveChangesAsync();

            //ACT
            var actual = await sut.GetIdsComportamientosExpedientesNivelesDeUso(expediente, CancellationToken.None);

            //ASSERT
            Assert.NotEmpty(actual);
        }

        #endregion
    }
}
