using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Localization;
using Unir.Expedientes.Application.Common.Interfaces;
using Unir.Expedientes.Application.Common.Models.ErpAcademico.Expedientes;
using Unir.Expedientes.Application.Common.Models.ErpAcademico.PlanesEstudio;
using Unir.Expedientes.Application.Common.Models.ExpedientesGestorUnir;
using Unir.Expedientes.Application.Common.Parameters;
using Unir.Expedientes.Application.ExpedientesAlumnos.Queries.CanQualifyAlumnoInPlanEstudio;
using Unir.Expedientes.Application.Tests.Common;
using Unir.Expedientes.Domain.Entities;
using Unir.Framework.ApplicationSuperTypes.Exceptions;
using Xunit;

namespace Unir.Expedientes.Application.Tests.ExpedientesAlumnos.Queries.CanQualifyAlumnoInPlanEstudio
{
    [Collection("CommonTestCollection")]
    public class CanQualifyAlumnoInPlanEstudioQueryHandlerTests : TestBase
    {
        #region Handle

        [Fact(DisplayName = "Cuando se encuentra el expediente alumno Devuelve ok")]
        public async Task Handle_ConExpediente_Ok()
        {
            //ARRANGE
            var idRefPlan = "54654";
            var idRefIntegracionAlumno = "56798";
            await Context.ExpedientesAlumno.AddAsync(new ExpedienteAlumno
            {
                Id = 1,
                IdRefPlan = idRefPlan,
                IdRefIntegracionAlumno = idRefIntegracionAlumno
            });
            await Context.SaveChangesAsync();

            var mockLocalizer = new Mock<IStringLocalizer<CanQualifyAlumnoInPlanEstudioQueryHandler>>();
            var mockExpedientesGestorUnirService = new Mock<IExpedientesGestorUnirServiceClient>();
            mockExpedientesGestorUnirService
                .Setup(s => s.GetExpedienteGestorFormatoComercialWithAsignaturasErp(It.IsAny<string>(), It.IsAny<int>()))
                .ReturnsAsync(new ExpedienteErpComercialIntegrationModel());
            var mockErpAcademicoService = new Mock<IErpAcademicoServiceClient>();
            var request = new CanQualifyAlumnoInPlanEstudioQuery
            {
                IdRefPlan = idRefPlan,
                IdRefIntegracionAlumno = idRefIntegracionAlumno
            };
            var sut = new Mock<CanQualifyAlumnoInPlanEstudioQueryHandler>(Context, mockLocalizer.Object,
                mockExpedientesGestorUnirService.Object, mockErpAcademicoService.Object);
            sut.Setup(s =>
                    s.GetIsPlanSurpassed(It.IsAny<ExpedienteAlumno>(),
                        It.IsAny<ExpedienteErpComercialIntegrationModel>()))
                .ReturnsAsync(new ExpedienteAlumnoTitulacionPlanDto
                {
                    EsPlanSuperado = new PlanSuperadoErpAcademicoModel {EsSuperado = true}
                });
            sut.Setup(s =>
                    s.GetCausasFallosComprobacionMatriculasDocumentacionErp(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(new ValidateAlumnoMatriculacionErpAcademicoModel());

            //ACT
            var actual = await sut.Object.Handle(request, CancellationToken.None);

            //ASSERT
            Assert.IsType<ExpedienteAlumnoTitulacionPlanDto>(actual);
            mockExpedientesGestorUnirService
                .Verify(s => s.GetExpedienteGestorFormatoComercialWithAsignaturasErp(It.IsAny<string>(), It.IsAny<int>()), Times.Once);
            sut.Verify(s =>
                s.GetIsPlanSurpassed(It.IsAny<ExpedienteAlumno>(),
                    It.IsAny<ExpedienteErpComercialIntegrationModel>()), Times.Once);
            sut.Verify(s =>
                    s.GetCausasFallosComprobacionMatriculasDocumentacionErp(It.IsAny<string>(), It.IsAny<string>()),
                Times.Once);
        }

        [Fact(DisplayName = "Cuando no encuentra expediente Devuelve excepción")]
        public async Task Handle_SinExpediente()
        {
            //ARRANGE
            var request = new CanQualifyAlumnoInPlanEstudioQuery
            {
                IdRefPlan = Guid.NewGuid().ToString(),
                IdRefIntegracionAlumno = Guid.NewGuid().ToString()
            };
            var mensajeEsperado =
                $"No se ha encontrado un Expediente con el IdPlan {request.IdRefPlan} y IdIntegracionAlumno {request.IdRefIntegracionAlumno}.";
            var mockLocalizer = new Mock<IStringLocalizer<CanQualifyAlumnoInPlanEstudioQueryHandler>>();
            mockLocalizer.Setup(s => s[It.Is<string>(msj => msj == mensajeEsperado)])
                .Returns(new LocalizedString(mensajeEsperado, mensajeEsperado));
            var mockExpedientesGestorUnirService = new Mock<IExpedientesGestorUnirServiceClient>();
            var mockErpAcademicoService = new Mock<IErpAcademicoServiceClient>();

            var sut = new CanQualifyAlumnoInPlanEstudioQueryHandler(Context, mockLocalizer.Object,
                mockExpedientesGestorUnirService.Object, mockErpAcademicoService.Object);

            //ACT
            var ex = await Record.ExceptionAsync(async () =>
            {
                await sut.Handle(request, CancellationToken.None);
            });

            //ASSERT
            Assert.IsType<BadRequestException>(ex);
            Assert.Equal(mensajeEsperado, ex.Message);
        }

        #endregion

        #region GetIsPlanSurpassed

        [Fact(DisplayName = "Cuando tiene asignaturas superadas de erp Devuelve expediente de erp")]
        public async Task GetIsPlanSurpassed_ConAsignaturasSuperadasEnErp()
         {
            //ARRANGE
            var mockLocalizer = new Mock<IStringLocalizer<CanQualifyAlumnoInPlanEstudioQueryHandler>>();
            var mockExpedientesGestorUnirService = new Mock<IExpedientesGestorUnirServiceClient>();
            var mockErpAcademicoService = new Mock<IErpAcademicoServiceClient>();
            var sut = new Mock<CanQualifyAlumnoInPlanEstudioQueryHandler>(Context, mockLocalizer.Object,
                mockExpedientesGestorUnirService.Object, mockErpAcademicoService.Object)
            {
                CallBase = true
            };
            sut.Setup(s => s.GetAsignaturasSurpassedErpComercial(It.IsAny<ExpedienteErpComercialIntegrationModel>()))
                .Returns(new List<AsignaturaErpComercialExpedientesIntegrationModel>
                {
                    new AsignaturaErpComercialExpedientesIntegrationModel()
                });
            sut.Setup(s => s.GetPlanSurpassedErp(It.IsAny<int>(), It.IsAny<ExpedienteAlumno>(),
                    It.IsAny<List<AsignaturaErpComercialExpedientesIntegrationModel>>()))
                .ReturnsAsync(new ExpedienteAlumnoTitulacionPlanDto());

            //ACT
            var actual = await sut.Object.GetIsPlanSurpassed(new ExpedienteAlumno{IdRefPlan = "546"}, new ExpedienteErpComercialIntegrationModel());

            //ASSERT
            Assert.IsType<ExpedienteAlumnoTitulacionPlanDto>(actual);
            sut.Verify(s => s.GetAsignaturasSurpassedErpComercial(It.IsAny<ExpedienteErpComercialIntegrationModel>()),
                Times.Once);
            sut.Verify(s => s.GetPlanSurpassedErp(It.IsAny<int>(), It.IsAny<ExpedienteAlumno>(),
                It.IsAny<List<AsignaturaErpComercialExpedientesIntegrationModel>>()), Times.Once);
        }

        [Fact(DisplayName = "Cuando no tiene asignaturas superadas de erp Devuelve mensaje")]
        public async Task GetIsPlanSurpassed_SinAsignaturasSuperadasEnErp()
        {
            //ARRANGE
            var mensajeEsperado = "El Alumno no tiene ninguna Asignatura Superada";
            var mockLocalizer = new Mock<IStringLocalizer<CanQualifyAlumnoInPlanEstudioQueryHandler>>();
            var mockExpedientesGestorUnirService = new Mock<IExpedientesGestorUnirServiceClient>();
            var mockErpAcademicoService = new Mock<IErpAcademicoServiceClient>();
            var sut = new Mock<CanQualifyAlumnoInPlanEstudioQueryHandler>(Context, mockLocalizer.Object,
                mockExpedientesGestorUnirService.Object, mockErpAcademicoService.Object)
            {
                CallBase = true
            };
            sut.Setup(s => s.GetAsignaturasSurpassedErpComercial(It.IsAny<ExpedienteErpComercialIntegrationModel>()))
                .Returns(new List<AsignaturaErpComercialExpedientesIntegrationModel>());
            sut.Setup(s => s.GetPlanSurpassedErp(It.IsAny<int>(), It.IsAny<ExpedienteAlumno>(),
                    It.IsAny<List<AsignaturaErpComercialExpedientesIntegrationModel>>()))
                .ReturnsAsync(new ExpedienteAlumnoTitulacionPlanDto());

            //ACT
            var actual = await sut.Object.GetIsPlanSurpassed(new ExpedienteAlumno { IdRefPlan = "546" }, new ExpedienteErpComercialIntegrationModel());

            //ASSERT
            Assert.IsType<ExpedienteAlumnoTitulacionPlanDto>(actual);
            sut.Verify(s => s.GetAsignaturasSurpassedErpComercial(It.IsAny<ExpedienteErpComercialIntegrationModel>()),
                Times.Once);
            sut.Verify(s => s.GetPlanSurpassedErp(It.IsAny<int>(), It.IsAny<ExpedienteAlumno>(),
                It.IsAny<List<AsignaturaErpComercialExpedientesIntegrationModel>>()), Times.Never);
            Assert.Equal(mensajeEsperado, actual.EsPlanSuperado.CausasInsuperacion[0]);
        }

        #endregion

        #region GetAsignaturasSurpassedErpComercial

        [Fact(DisplayName = "Cuando no tiene asignaturas superadas de erp Devuelve expediente lista vacía")]
        public void GetAsignaturasSurpassedErpComercial_SinAsignaturasSuperadasEnErp()
        {
            //ARRANGE
            var mockLocalizer = new Mock<IStringLocalizer<CanQualifyAlumnoInPlanEstudioQueryHandler>>();
            var mockExpedientesGestorUnirService = new Mock<IExpedientesGestorUnirServiceClient>();
            var mockErpAcademicoService = new Mock<IErpAcademicoServiceClient>();
            var sut = new Mock<CanQualifyAlumnoInPlanEstudioQueryHandler>(Context, mockLocalizer.Object,
                mockExpedientesGestorUnirService.Object, mockErpAcademicoService.Object)
            {
                CallBase = true
            };

            //ACT
            var actual = sut.Object.GetAsignaturasSurpassedErpComercial(new ExpedienteErpComercialIntegrationModel());

            //ASSERT
            Assert.IsType<List<AsignaturaErpComercialExpedientesIntegrationModel>>(actual);
            Assert.Empty(actual);
        }

        [Fact(DisplayName = "Cuando tiene asignaturas superadas de erp Devuelve lista filtrada")]
        public void GetAsignaturasSurpassedErpComercial_ConAsignaturasSuperadasEnErp()
        {
            //ARRANGE
            var mockLocalizer = new Mock<IStringLocalizer<CanQualifyAlumnoInPlanEstudioQueryHandler>>();
            var mockExpedientesGestorUnirService = new Mock<IExpedientesGestorUnirServiceClient>();
            var mockErpAcademicoService = new Mock<IErpAcademicoServiceClient>();
            var sut = new Mock<CanQualifyAlumnoInPlanEstudioQueryHandler>(Context, mockLocalizer.Object,
                mockExpedientesGestorUnirService.Object, mockErpAcademicoService.Object)
            {
                CallBase = true
            };

            //ACT
            var actual = sut.Object.GetAsignaturasSurpassedErpComercial(new ExpedienteErpComercialIntegrationModel
            {
                Asignaturas = new List<AsignaturaErpComercialExpedientesIntegrationModel>
                {
                    new AsignaturaErpComercialExpedientesIntegrationModel
                    {
                        IdAsignatura = -1
                    },
                    new AsignaturaErpComercialExpedientesIntegrationModel
                    {
                        IdAsignatura = 1,
                        Estado = 40
                    }
                }
            });

            //ASSERT
            Assert.IsType<List<AsignaturaErpComercialExpedientesIntegrationModel>>(actual);
            Assert.Single(actual);
        }

        #endregion

        #region GetPlanSurpassedErpAsync

        [Fact(DisplayName = "Cuando se envian los datos de expedientes erp Devuelve el plan del alumno")]
        public async Task GetPlanSurpassedErp_Ok()
        {
            //ARRANGE
            var mockLocalizer = new Mock<IStringLocalizer<CanQualifyAlumnoInPlanEstudioQueryHandler>>();
            var mockExpedientesGestorUnirService = new Mock<IExpedientesGestorUnirServiceClient>();
            var mockErpAcademicoService = new Mock<IErpAcademicoServiceClient>();
            mockErpAcademicoService
                .Setup(s => s.ItIsPlanSurpassed(It.IsAny<int>(), It.IsAny<EsPlanSuperadoParameters>()))
                .ReturnsAsync(new PlanSuperadoErpAcademicoModel());
            var sut = new Mock<CanQualifyAlumnoInPlanEstudioQueryHandler>(Context, mockLocalizer.Object,
                mockExpedientesGestorUnirService.Object, mockErpAcademicoService.Object)
            {
                CallBase = true
            };

            //ACT
            var actual = await sut.Object.GetPlanSurpassedErp(Int32.MaxValue, new ExpedienteAlumno
            {
                IdRefNodo = "1",
                IdRefVersionPlan = "2",
                
            }, new List<AsignaturaErpComercialExpedientesIntegrationModel>
            {
                new AsignaturaErpComercialExpedientesIntegrationModel
                {
                    IdAsignatura = 1
                }
            });

            //ASSERT
            Assert.IsType<ExpedienteAlumnoTitulacionPlanDto>(actual);
            mockErpAcademicoService
                .Verify(s => s.ItIsPlanSurpassed(It.IsAny<int>(), It.IsAny<EsPlanSuperadoParameters>()), Times.Once);
        }

        #endregion

        #region GetCausasFallosComprobacionMatriculasDocumentacionErp

        [Fact(DisplayName = "Cuando se envian los datos de expedientes erp Devuelve el plan del alumno")]
        public async Task GetCausasFallosComprobacionMatriculasDocumentacionErp_Ok()
        {
            //ARRANGE
            var mockLocalizer = new Mock<IStringLocalizer<CanQualifyAlumnoInPlanEstudioQueryHandler>>();
            var mockExpedientesGestorUnirService = new Mock<IExpedientesGestorUnirServiceClient>();
            var mockErpAcademicoService = new Mock<IErpAcademicoServiceClient>();
            mockErpAcademicoService
                .Setup(s => s.ValidateAlumnoMatriculacion(It.IsAny<ValidateAlumnoMatriculacionParameters>()))
                    .ReturnsAsync(new ValidateAlumnoMatriculacionErpAcademicoModel());
            var sut = new Mock<CanQualifyAlumnoInPlanEstudioQueryHandler>(Context, mockLocalizer.Object,
                mockExpedientesGestorUnirService.Object, mockErpAcademicoService.Object)
            {
                CallBase = true
            };

            //ACT
            var actual = await sut.Object.GetCausasFallosComprobacionMatriculasDocumentacionErp("154", "676");

            //ASSERT
            Assert.IsType<ValidateAlumnoMatriculacionErpAcademicoModel>(actual);
            mockErpAcademicoService
                .Verify(s => s.ValidateAlumnoMatriculacion(It.IsAny<ValidateAlumnoMatriculacionParameters>()),
                    Times.Once);
        }

        #endregion
    }
}
