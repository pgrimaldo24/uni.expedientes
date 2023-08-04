using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using MediatR;
using Microsoft.Extensions.Localization;
using Moq;
using System.Threading;
using System.Threading.Tasks;
using Unir.Expedientes.Application.Common.Interfaces;
using Unir.Expedientes.Application.Common.Models.ErpAcademico.Expedientes;
using Unir.Expedientes.Application.Common.Models.ErpAcademico.Matriculacion;
using Unir.Expedientes.Application.Common.Models.ErpAcademico.PlanesEstudio;
using Unir.Expedientes.Application.ExpedientesAlumnos.Queries.GetExpedienteAlumnoById;
using Unir.Expedientes.Application.ExpedientesAlumnos.Queries.GetExpedienteAlumnoErpById;
using Unir.Expedientes.Application.Tests.Common;
using Unir.Framework.ApplicationSuperTypes.Exceptions;
using Xunit;

namespace Unir.Expedientes.Application.Tests.ExpedientesAlumnos.Queries.GetExpedienteAlumnoErpById
{
    [Collection("CommonTestCollection")]
    public class GetExpedienteAlumnoErpByIdQueryHandlerTest : TestBase
    {
        #region Handle

        [Fact(DisplayName = "Cuando se obtiene el expediente de erp Devuelve ok")]
        public async Task Handle_Ok()
        {
            //ARRANGE
            var expedienteEsperado = new ExpedienteAlumnoItemDto
            {
                Id = 1,
                IdRefIntegracionAlumno = "123456",
                IdRefPlan = "789456",
                IdRefVersionPlan = "456321"
            };
            var mockIMediator = new Mock<IMediator>
            {
                CallBase = true
            };
            mockIMediator.Setup(s => s.Send(It.IsAny<GetExpedienteAlumnoByIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expedienteEsperado);
            var mockIStringLocalizer = new Mock<IStringLocalizer<GetExpedienteAlumnoErpByIdQueryHandler>>
            {
                CallBase = true
            };
            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };
            mockIErpAcademicoServiceClient.Setup(s => s.GetExpediente(It.Is<int>(i => i == expedienteEsperado.Id)))
                .ReturnsAsync(new ExpedienteAcademicoModel());

            var request = new GetExpedienteAlumnoErpByIdQuery(1);
            var sut = new Mock<GetExpedienteAlumnoErpByIdQueryHandler>(mockIErpAcademicoServiceClient.Object, mockIStringLocalizer.Object, mockIMediator.Object)
            {
                CallBase = true
            };
            sut.Setup(s =>
                s.ValidateDatosExpediente(It.IsAny<ExpedienteAcademicoModel>(), It.IsAny<ExpedienteAlumnoItemDto>()));

            //ACT
            var actual = await sut.Object.Handle(request, CancellationToken.None);

            //ASSERT
            Assert.Equal(expedienteEsperado.IdRefIntegracionAlumno, actual.IdRefIntegracionAlumno);
            Assert.Equal(expedienteEsperado.IdRefPlan, actual.IdRefPlan);
            Assert.Equal(expedienteEsperado.IdRefVersionPlan, actual.IdRefVersionPlan);
        }

        [Fact(DisplayName = "Cuando se obtiene el expediente con Titulación de acceso desde erp Devuelve ok")]
        public async Task Handle_con_titulacion_acceso_Ok()
        {
            //ARRANGE
            var expedienteEsperado = new ExpedienteAlumnoItemDto
            {
                Id = 1,
                IdRefIntegracionAlumno = "123456",
                IdRefPlan = "789456",
                IdRefVersionPlan = "456321",
                TitulacionAcceso = new TitulacionAccesoDto
                {
                    Titulo = Guid.NewGuid().ToString(),
                    CodigoColegiadoProfesional = Guid.NewGuid().ToString(),
                    FechaInicioTitulo = DateTime.Now,
                    FechafinTitulo = DateTime.Now,
                    IdRefTerritorioInstitucionDocente = "56468",
                    InstitucionDocente = Guid.NewGuid().ToString(),
                    NroSemestreRealizados = 5,
                    TipoEstudio = Guid.NewGuid().ToString(),
                    IdRefInstitucionDocente = Guid.NewGuid().ToString()
                }
            };
            var mockIMediator = new Mock<IMediator>
            {
                CallBase = true
            };
            mockIMediator.Setup(s => s.Send(It.IsAny<GetExpedienteAlumnoByIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expedienteEsperado);
            var mockIStringLocalizer = new Mock<IStringLocalizer<GetExpedienteAlumnoErpByIdQueryHandler>>
            {
                CallBase = true
            };
            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };
            mockIErpAcademicoServiceClient.Setup(s => s.GetExpediente(It.Is<int>(i => i == expedienteEsperado.Id)))
                .ReturnsAsync(new ExpedienteAcademicoModel());

            var request = new GetExpedienteAlumnoErpByIdQuery(1);
            var sut = new Mock<GetExpedienteAlumnoErpByIdQueryHandler>(mockIErpAcademicoServiceClient.Object, mockIStringLocalizer.Object, mockIMediator.Object)
            {
                CallBase = true
            };
            sut.Setup(s =>
                s.ValidateDatosExpediente(It.IsAny<ExpedienteAcademicoModel>(), It.IsAny<ExpedienteAlumnoItemDto>()));

            //ACT
            var actual = await sut.Object.Handle(request, CancellationToken.None);

            //ASSERT
            Assert.Equal(expedienteEsperado.IdRefIntegracionAlumno, actual.IdRefIntegracionAlumno);
            Assert.Equal(expedienteEsperado.IdRefPlan, actual.IdRefPlan);
            Assert.Equal(expedienteEsperado.IdRefVersionPlan, actual.IdRefVersionPlan);
            Assert.Equal(expedienteEsperado.TitulacionAcceso.Titulo, actual.TitulacionAcceso.Titulo);
            Assert.Equal(expedienteEsperado.TitulacionAcceso.CodigoColegiadoProfesional, actual.TitulacionAcceso.CodigoColegiadoProfesional);
            Assert.Equal(expedienteEsperado.TitulacionAcceso.FechaInicioTitulo, actual.TitulacionAcceso.FechaInicioTitulo);
            Assert.Equal(expedienteEsperado.TitulacionAcceso.FechafinTitulo, actual.TitulacionAcceso.FechafinTitulo);
            Assert.Equal(expedienteEsperado.TitulacionAcceso.IdRefTerritorioInstitucionDocente, actual.TitulacionAcceso.IdRefTerritorioInstitucionDocente);
            Assert.Equal(expedienteEsperado.TitulacionAcceso.InstitucionDocente, actual.TitulacionAcceso.InstitucionDocente);
            Assert.Equal(expedienteEsperado.TitulacionAcceso.NroSemestreRealizados, actual.TitulacionAcceso.NroSemestreRealizados);
            Assert.Equal(expedienteEsperado.TitulacionAcceso.TipoEstudio, actual.TitulacionAcceso.TipoEstudio);
            Assert.Equal(expedienteEsperado.TitulacionAcceso.IdRefInstitucionDocente, actual.TitulacionAcceso.IdRefInstitucionDocente);

        }

        #endregion

        #region ValidateDatosExpediente

        [Fact(DisplayName = "Cuando el plan del expediente no coincide con el de erp Devuelve excepción")]
        public void ValidateDatosExpediente_PlanInconsistente()
        {
            //ARRANGE
            var expedienteDto = new ExpedienteAlumnoItemDto
            {
                Id = 1,
                IdRefIntegracionAlumno = "123456",
                IdRefPlan = "789456",
                IdRefVersionPlan = "456321"
            };
            var mockIMediator = new Mock<IMediator>
            {
                CallBase = true
            };
            var mockIStringLocalizer = new Mock<IStringLocalizer<GetExpedienteAlumnoErpByIdQueryHandler>>
            {
                CallBase = true
            };
            var mensajeEsperado = $"ERP Académico: El Plan con Id {expedienteDto.IdRefPlan} no coincide";
            mockIStringLocalizer.Setup(s => s[It.Is<string>(msj => msj == mensajeEsperado)])
                .Returns(new LocalizedString(mensajeEsperado, mensajeEsperado));
            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };

            var sut = new GetExpedienteAlumnoErpByIdQueryHandler(mockIErpAcademicoServiceClient.Object,
                mockIStringLocalizer.Object, mockIMediator.Object);

            var expedienteErp = new ExpedienteAcademicoModel
            {
                Plan = new PlanAcademicoModel
                {
                    Id = 1
                }
            };

            //ACT
            var ex = Record.Exception(() =>
            {
                sut.ValidateDatosExpediente(expedienteErp, expedienteDto);
            });

            //ASSERT
            Assert.IsType<BadRequestException>(ex);
            Assert.Equal(mensajeEsperado, ex.Message);
        }

        [Fact(DisplayName = "Cuando el alumno del expediente no coincide con el de erp Devuelve excepción")]
        public void ValidateDatosExpediente_AlumnoInconsistente()
        {
            //ARRANGE
            var expedienteDto = new ExpedienteAlumnoItemDto
            {
                Id = 1,
                IdRefIntegracionAlumno = "123456",
                IdRefPlan = "789456",
                IdRefVersionPlan = "456321"
            };
            var mockIMediator = new Mock<IMediator>
            {
                CallBase = true
            };
            var mockIStringLocalizer = new Mock<IStringLocalizer<GetExpedienteAlumnoErpByIdQueryHandler>>
            {
                CallBase = true
            };
            var mensajeEsperado = $"ERP Académico: El Alumno con Id Integración {expedienteDto.IdRefIntegracionAlumno} no coincide";
            mockIStringLocalizer.Setup(s => s[It.Is<string>(msj => msj == mensajeEsperado)])
                .Returns(new LocalizedString(mensajeEsperado, mensajeEsperado));
            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };

            var sut = new GetExpedienteAlumnoErpByIdQueryHandler(mockIErpAcademicoServiceClient.Object,
                mockIStringLocalizer.Object, mockIMediator.Object);

            var expedienteErp = new ExpedienteAcademicoModel
            {
                Plan = new PlanAcademicoModel
                {
                    Id = 789456
                },
                Alumno = new AlumnoAcademicoModel
                {
                    IdIntegracion = "1"
                }
            };

            //ACT
            var ex = Record.Exception(() =>
            {
                sut.ValidateDatosExpediente(expedienteErp, expedienteDto);
            });

            //ASSERT
            Assert.IsType<BadRequestException>(ex);
            Assert.Equal(mensajeEsperado, ex.Message);
        }

        [Fact(DisplayName = "Cuando la versión no pertenece al plan de estudio Devuelve excepción")]
        public void ValidateDatosExpediente_VersionPlanInconsistente()
        {
            //ARRANGE
            var expedienteDto = new ExpedienteAlumnoItemDto
            {
                Id = 1,
                IdRefIntegracionAlumno = "123456",
                IdRefPlan = "789456",
                IdRefVersionPlan = "456321"
            };
            var mockIMediator = new Mock<IMediator>
            {
                CallBase = true
            };
            var mockIStringLocalizer = new Mock<IStringLocalizer<GetExpedienteAlumnoErpByIdQueryHandler>>
            {
                CallBase = true
            };
            const string mensajeEsperado = "La Versión no pertenece al Plan de Estudio";
            mockIStringLocalizer.Setup(s => s[It.Is<string>(msj => msj == mensajeEsperado)])
                .Returns(new LocalizedString(mensajeEsperado, mensajeEsperado));
            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };

            var sut = new GetExpedienteAlumnoErpByIdQueryHandler(mockIErpAcademicoServiceClient.Object,
                mockIStringLocalizer.Object, mockIMediator.Object);

            var expedienteErp = new ExpedienteAcademicoModel
            {
                Plan = new PlanAcademicoModel
                {
                    Id = 789456,
                    VersionesPlanes = new List<VersionPlanAcademicoModel>
                    {
                        new()
                        {
                            Id = 1
                        }
                    }
                },
                Alumno = new AlumnoAcademicoModel
                {
                    IdIntegracion = "123456"
                }
            };

            //ACT
            var ex = Record.Exception(() =>
            {
                sut.ValidateDatosExpediente(expedienteErp, expedienteDto);
            });

            //ASSERT
            Assert.IsType<WarningException>(ex);
            Assert.Equal(mensajeEsperado, ex.Message);
        }

        [Fact(DisplayName = "Cuando el nodo no coincide con el de erp Devuelve excepción")]
        public void ValidateDatosExpediente_NodoInconsistente()
        {
            //ARRANGE
            var expedienteDto = new ExpedienteAlumnoItemDto
            {
                Id = 1,
                IdRefIntegracionAlumno = "123456",
                IdRefPlan = "789456",
                IdRefVersionPlan = "456321",
                IdRefNodo = "123"
            };
            var mockIMediator = new Mock<IMediator>
            {
                CallBase = true
            };
            var mockIStringLocalizer = new Mock<IStringLocalizer<GetExpedienteAlumnoErpByIdQueryHandler>>
            {
                CallBase = true
            };
            var mensajeEsperado = $"ERP Académico: El Nodo con Id {expedienteDto.IdRefNodo} no coincide";
            mockIStringLocalizer.Setup(s => s[It.Is<string>(msj => msj == mensajeEsperado)])
                .Returns(new LocalizedString(mensajeEsperado, mensajeEsperado));
            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };

            var sut = new GetExpedienteAlumnoErpByIdQueryHandler(mockIErpAcademicoServiceClient.Object,
                mockIStringLocalizer.Object, mockIMediator.Object);

            var expedienteErp = new ExpedienteAcademicoModel
            {
                Plan = new PlanAcademicoModel
                {
                    Id = 789456,
                    VersionesPlanes = new List<VersionPlanAcademicoModel>
                    {
                        new()
                        {
                            Id = 456321
                        }
                    }
                },
                Alumno = new AlumnoAcademicoModel
                {
                    IdIntegracion = "123456"
                },
                ViaAccesoPlan = new ViaAccesoPlanAcademicoModel
                {
                    Nodo = new NodoErpAcademicoModel
                    {
                        Id = 1
                    }
                }
            };

            //ACT
            var ex = Record.Exception(() =>
            {
                sut.ValidateDatosExpediente(expedienteErp, expedienteDto);
            });

            //ASSERT
            Assert.IsType<BadRequestException>(ex);
            Assert.Equal(mensajeEsperado, ex.Message);
        }

        [Fact(DisplayName = "Cuando no se incumple ninguna validación No realiza ninguna acción")]
        public void ValidateDatosExpediente_Ok()
        {
            //ARRANGE
            var expedienteDto = new ExpedienteAlumnoItemDto
            {
                Id = 1,
                IdRefIntegracionAlumno = "123456",
                IdRefPlan = "789456",
                IdRefVersionPlan = "456321",
                IdRefNodo = "123"
            };
            var mockIMediator = new Mock<IMediator>
            {
                CallBase = true
            };
            var mockIStringLocalizer = new Mock<IStringLocalizer<GetExpedienteAlumnoErpByIdQueryHandler>>
            {
                CallBase = true
            };
            var mensajeEsperado = $"ERP Académico: El Nodo con Id {expedienteDto.IdRefNodo} no coincide";
            mockIStringLocalizer.Setup(s => s[It.Is<string>(msj => msj == mensajeEsperado)])
                .Returns(new LocalizedString(mensajeEsperado, mensajeEsperado));
            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };

            var sut = new GetExpedienteAlumnoErpByIdQueryHandler(mockIErpAcademicoServiceClient.Object,
                mockIStringLocalizer.Object, mockIMediator.Object);

            var expedienteErp = new ExpedienteAcademicoModel
            {
                Plan = new PlanAcademicoModel
                {
                    Id = 789456,
                    VersionesPlanes = new List<VersionPlanAcademicoModel>
                    {
                        new()
                        {
                            Id = 456321
                        }
                    }
                },
                Alumno = new AlumnoAcademicoModel
                {
                    IdIntegracion = "123456"
                },
                ViaAccesoPlan = new ViaAccesoPlanAcademicoModel
                {
                    Nodo = new NodoErpAcademicoModel
                    {
                        Id = 123
                    }
                }
            };

            //ACT
            sut.ValidateDatosExpediente(expedienteErp, expedienteDto);

            //ASSERT
            Assert.Equal(expedienteDto.IdRefPlan, expedienteErp.Plan.Id.ToString());
            Assert.Equal(expedienteDto.IdRefIntegracionAlumno, expedienteErp.Alumno.IdIntegracion);
            Assert.Equal(expedienteDto.IdRefVersionPlan, expedienteErp.Plan.VersionesPlanes.First().Id.ToString());
            Assert.Equal(expedienteDto.IdRefNodo, expedienteErp.ViaAccesoPlan.Nodo.Id.ToString());
        }

        #endregion
    }
}
