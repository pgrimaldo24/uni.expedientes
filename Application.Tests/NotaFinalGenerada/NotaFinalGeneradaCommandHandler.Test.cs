using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Moq;
using Unir.Expedientes.Application.AsignaturasCalificacion.Commands.CreateAsignaturaCalificacion;
using Unir.Expedientes.Application.Common.Interfaces;
using Unir.Expedientes.Application.Common.Models.ErpAcademico.Matriculacion;
using Unir.Expedientes.Application.Common.Models.ErpAcademico.OfertaEstudio;
using Unir.Expedientes.Application.Common.Models.ErpAcademico.PlanesEstudio;
using Unir.Expedientes.Application.Common.Models.Evaluaciones;
using Unir.Expedientes.Application.ExpedientesAlumnos.Queries.GetAllExpedientesAlumnos;
using Unir.Expedientes.Application.NotaFinalGenerada;
using Unir.Expedientes.Application.Tests.Common;
using Xunit;

namespace Unir.Expedientes.Application.Tests.NotaFinalGenerada
{
    [Collection("CommonTestCollection")]
    public class NotaFinalGeneradaCommandHandlerTests : TestBase
    {
        #region Handle
        [Fact(DisplayName = "Cuando ERP no devuelve nada, devuelve Unit Value")]
        public async Task Handle_Ok_SinRegistrosERP()
        {
            //ARRANGE
            var mockErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };
            var mockEvaluacionesServiceClient = new Mock<IEvaluacionesServiceClient>
            {
                CallBase = true
            };
            var mockIMediator = new Mock<IMediator>
            {
                CallBase = true
            };
            var sut = new Mock<NotaFinalGeneradaCommandHandler>(mockIMediator.Object, mockErpAcademicoServiceClient.Object, mockEvaluacionesServiceClient.Object)
            {
                CallBase = true
            };
            var request = new NotaFinalGeneradaCommand
            {
                IdCurso = 456,
                Notas = new List<NotaCommonStruct>
                {
                    new NotaCommonStruct
                    {
                        IdAlumno = 1
                    },
                    new NotaCommonStruct
                    {
                        IdAlumno = 2
                    },
                    new NotaCommonStruct
                    {
                        IdAlumno = 3
                    }
                }
            };

            mockErpAcademicoServiceClient
                .Setup(erp => erp.GetAsignaturasMatriculadasParaNotaFinal(It.IsAny<int>(), It.IsAny<List<string>>()))
                .ReturnsAsync((List<AsignaturaMatriculadaModel>)null);

            //ACT
            var result = await sut.Object.Handle(request, CancellationToken.None);

            //ASSERT
            Assert.IsType<Unit>(result);
            mockErpAcademicoServiceClient.Verify(erp => erp.GetAsignaturasMatriculadasParaNotaFinal(It.IsAny<int>(), It.IsAny<List<string>>()), Times.Once);
        }
        [Fact(DisplayName = "Cuando no hay expedientes para esos Alumnos, devuelve Unit Value")]
        public async Task Handle_Ok_SinExpedientes()
        {
            //ARRANGE
            var mockErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };
            var mockEvaluacionesServiceClient = new Mock<IEvaluacionesServiceClient>
            {
                CallBase = true
            };
            var mockIMediator = new Mock<IMediator>
            {
                CallBase = true
            };
            var sut = new Mock<NotaFinalGeneradaCommandHandler>(mockIMediator.Object, mockErpAcademicoServiceClient.Object, mockEvaluacionesServiceClient.Object)
            {
                CallBase = true
            };
            var request = new NotaFinalGeneradaCommand
            {
                IdCurso = 456,
                Notas = new List<NotaCommonStruct>
                {
                    new NotaCommonStruct
                    {
                        IdAlumno = 1
                    },
                    new NotaCommonStruct
                    {
                        IdAlumno = 2
                    },
                    new NotaCommonStruct
                    {
                        IdAlumno = 3
                    }
                }
            };

            mockErpAcademicoServiceClient
                .Setup(erp => erp.GetAsignaturasMatriculadasParaNotaFinal(It.IsAny<int>(), It.IsAny<List<string>>()))
                .ReturnsAsync(new List<AsignaturaMatriculadaModel> {new AsignaturaMatriculadaModel{Id = 1}});

            mockIMediator.Setup(s =>
                    s.Send(It.IsAny<GetAllExpedientesAlumnosQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Array.Empty<ExpedienteAlumnoListItemDto>());

            //ACT
            var result = await sut.Object.Handle(request, CancellationToken.None);

            //ASSERT
            Assert.IsType<Unit>(result);
            mockErpAcademicoServiceClient.Verify(
                erp => erp.GetAsignaturasMatriculadasParaNotaFinal(It.IsAny<int>(), It.IsAny<List<string>>()),
                Times.Once);
            mockIMediator.Verify(s => s.Send(
                It.IsAny<GetAllExpedientesAlumnosQuery>(), It.IsAny<CancellationToken>()), Times.Once);
        }
        [Fact(DisplayName = "Cuando no hay Asignaturas expedientes coincidentes con ERP, devuelve Unit Value")]
        public async Task Handle_Ok_SinAsignaturasExpedientes()
        {
            //ARRANGE
            var mockErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };
            var mockEvaluacionesServiceClient = new Mock<IEvaluacionesServiceClient>
            {
                CallBase = true
            };
            var mockIMediator = new Mock<IMediator>
            {
                CallBase = true
            };
            var sut = new Mock<NotaFinalGeneradaCommandHandler>(mockIMediator.Object, mockErpAcademicoServiceClient.Object, mockEvaluacionesServiceClient.Object)
            {
                CallBase = true
            };
            var request = new NotaFinalGeneradaCommand
            {
                IdCurso = 456,
                Notas = new List<NotaCommonStruct>
                {
                    new NotaCommonStruct
                    {
                        IdAlumno = 1
                    },
                    new NotaCommonStruct
                    {
                        IdAlumno = 2
                    },
                    new NotaCommonStruct
                    {
                        IdAlumno = 3
                    }
                }
            };

            mockErpAcademicoServiceClient
                .Setup(erp => erp.GetAsignaturasMatriculadasParaNotaFinal(It.IsAny<int>(), It.IsAny<List<string>>()))
                .ReturnsAsync(new List<AsignaturaMatriculadaModel> { new AsignaturaMatriculadaModel { Id = 1, AsignaturaOfertada = new AsignaturaOfertadaModel {AsignaturaPlan = new AsignaturaPlanErpAcademicoModel{Id = 2}}} });

            mockIMediator.Setup(s =>
                    s.Send(It.IsAny<GetAllExpedientesAlumnosQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new[] {new ExpedienteAlumnoListItemDto { Id = 1, AsignaturasExpedientes = new List<AsignaturaExpedienteDto>()}});

            //ACT
            var result = await sut.Object.Handle(request, CancellationToken.None);

            //ASSERT
            Assert.IsType<Unit>(result);
            mockErpAcademicoServiceClient.Verify(
                erp => erp.GetAsignaturasMatriculadasParaNotaFinal(It.IsAny<int>(), It.IsAny<List<string>>()),
                Times.Once);
            mockIMediator.Verify(s => s.Send(
                It.IsAny<GetAllExpedientesAlumnosQuery>(), It.IsAny<CancellationToken>()), Times.Once);
        }
        [Fact(DisplayName = "Cuando es todo Ok, pero sin Configuración Versión Escala, devuelve Unit Value")]
        public async Task Handle_Ok_SinConfiguracionEscala()
        {
            //ARRANGE
            var mockErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };
            var mockEvaluacionesServiceClient = new Mock<IEvaluacionesServiceClient>
            {
                CallBase = true
            };
            var mockIMediator = new Mock<IMediator>
            {
                CallBase = true
            };
            var sut = new Mock<NotaFinalGeneradaCommandHandler>(mockIMediator.Object, mockErpAcademicoServiceClient.Object, mockEvaluacionesServiceClient.Object)
            {
                CallBase = true
            };
            var request = new NotaFinalGeneradaCommand
            {
                IdCurso = 456,
                Notas = new List<NotaCommonStruct>
                {
                    new NotaCommonStruct
                    {
                        IdAlumno = 1
                    },
                    new NotaCommonStruct
                    {
                        IdAlumno = 2
                    },
                    new NotaCommonStruct
                    {
                        IdAlumno = 3
                    }
                }
            };

            mockErpAcademicoServiceClient
                .Setup(erp => erp.GetAsignaturasMatriculadasParaNotaFinal(It.IsAny<int>(), It.IsAny<List<string>>()))
                .ReturnsAsync(new List<AsignaturaMatriculadaModel> { new AsignaturaMatriculadaModel { Id = 1, AsignaturaOfertada = new AsignaturaOfertadaModel { AsignaturaPlan = new AsignaturaPlanErpAcademicoModel { Id = 2 } } } });

            mockIMediator.Setup(s =>
                    s.Send(It.IsAny<GetAllExpedientesAlumnosQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new[]
                {
                    new ExpedienteAlumnoListItemDto
                    {
                        Id = 1,
                        AsignaturasExpedientes = new List<AsignaturaExpedienteDto>
                            { new AsignaturaExpedienteDto { IdRefAsignaturaPlan = "2" } }
                    }
                });

            mockIMediator.Setup(s =>
                    s.Send(It.IsAny<CreateAsignaturaCalificacionCommand>(), It.IsAny<CancellationToken>()));

            mockEvaluacionesServiceClient
                .Setup(ev => ev.GetConfiguracionEscalaFromNivelesAsociadosEscalas(
                    It.IsAny<int>(), It.IsAny<int?>())).ReturnsAsync((ConfiguracionVersionEscalaModel)null);
            
            //ACT
            var result = await sut.Object.Handle(request, CancellationToken.None);

            //ASSERT
            Assert.IsType<Unit>(result);
            mockErpAcademicoServiceClient.Verify(
                erp => erp.GetAsignaturasMatriculadasParaNotaFinal(It.IsAny<int>(), It.IsAny<List<string>>()),
                Times.Once);
            mockIMediator.Verify(s => s.Send(
                It.IsAny<GetAllExpedientesAlumnosQuery>(), It.IsAny<CancellationToken>()), Times.Once);
            mockEvaluacionesServiceClient.Verify(
                ev => ev.GetConfiguracionEscalaFromNivelesAsociadosEscalas(
                    It.IsAny<int>(), It.IsAny<int?>()), Times.Once);
            mockIMediator.Verify(s => s.Send(
                It.IsAny<CreateAsignaturaCalificacionCommand>(), It.IsAny<CancellationToken>()), Times.Once);
        }
        [Fact(DisplayName = "Cuando es Todo Ok, Retorna Unit Value")]
        public async Task Handle_Ok()
        {
            //ARRANGE
            var mockErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };
            var mockEvaluacionesServiceClient = new Mock<IEvaluacionesServiceClient>
            {
                CallBase = true
            };
            var mockIMediator = new Mock<IMediator>
            {
                CallBase = true
            };
            var sut = new Mock<NotaFinalGeneradaCommandHandler>(mockIMediator.Object, mockErpAcademicoServiceClient.Object, mockEvaluacionesServiceClient.Object)
            {
                CallBase = true
            };
            var request = new NotaFinalGeneradaCommand
            {
                IdCurso = 456,
                Notas = new List<NotaCommonStruct>
                {
                    new NotaCommonStruct
                    {
                        IdAlumno = 1
                    },
                    new NotaCommonStruct
                    {
                        IdAlumno = 2
                    },
                    new NotaCommonStruct
                    {
                        IdAlumno = 3
                    }
                }
            };

            mockErpAcademicoServiceClient
                .Setup(erp => erp.GetAsignaturasMatriculadasParaNotaFinal(It.IsAny<int>(), It.IsAny<List<string>>()))
                .ReturnsAsync(new List<AsignaturaMatriculadaModel> { new AsignaturaMatriculadaModel { Id = 1, AsignaturaOfertada = new AsignaturaOfertadaModel { AsignaturaPlan = new AsignaturaPlanErpAcademicoModel { Id = 2 } } } });

            mockIMediator.Setup(s =>
                    s.Send(It.IsAny<GetAllExpedientesAlumnosQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new[]
                {
                    new ExpedienteAlumnoListItemDto
                    {
                        Id = 1,
                        AsignaturasExpedientes = new List<AsignaturaExpedienteDto>
                            { new AsignaturaExpedienteDto { IdRefAsignaturaPlan = "2" } }
                    }
                });

            mockIMediator.Setup(s =>
                    s.Send(It.IsAny<CreateAsignaturaCalificacionCommand>(), It.IsAny<CancellationToken>()));

            mockEvaluacionesServiceClient
                .Setup(ev => ev.GetConfiguracionEscalaFromNivelesAsociadosEscalas(
                    It.IsAny<int>(), It.IsAny<int?>())).ReturnsAsync(new ConfiguracionVersionEscalaModel());

            //ACT
            var result = await sut.Object.Handle(request, CancellationToken.None);

            //ASSERT
            Assert.IsType<Unit>(result);
            mockErpAcademicoServiceClient.Verify(
                erp => erp.GetAsignaturasMatriculadasParaNotaFinal(It.IsAny<int>(), It.IsAny<List<string>>()),
                Times.Once);
            mockIMediator.Verify(s => s.Send(
                It.IsAny<GetAllExpedientesAlumnosQuery>(), It.IsAny<CancellationToken>()), Times.Once);
            mockEvaluacionesServiceClient.Verify(
                ev => ev.GetConfiguracionEscalaFromNivelesAsociadosEscalas(
                    It.IsAny<int>(), It.IsAny<int?>()), Times.Once);
            mockIMediator.Verify(s => s.Send(
                It.IsAny<CreateAsignaturaCalificacionCommand>(), It.IsAny<CancellationToken>()), Times.Once);
        }
        #endregion
    }
}
