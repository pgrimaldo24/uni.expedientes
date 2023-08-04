using System;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Routing.Constraints;
using Microsoft.EntityFrameworkCore;
using Unir.Expedientes.Application.AsignaturasCalificacion.Commands.RelateExpedientesAsignaturas;
using Unir.Expedientes.Application.Common.Interfaces;
using Unir.Expedientes.Application.Common.Models.ExpedientesGestorUnir;
using Unir.Expedientes.Application.Common.Models.GestorMapeos;
using Unir.Expedientes.Application.Tests.Common;
using Unir.Expedientes.Domain.Entities;
using Xunit;

namespace Unir.Expedientes.Application.Tests.AsignaturasCalificacion.Commands.RelateExpedientesAsignaturas
{
    [Collection("CommonTestCollection")]
    public class RelateExpedientesAsignaturasCommandHandlerTest : TestBase
    {
        #region Handle

        [Fact(DisplayName = "Cuando no existe el plan de estudio en el Gestor de Mapeos Retorna lista de error")]
        public async Task Handle_Error_PlanEstudio()
        {
            //ARRANGE
            var mockIGestorMapeosServiceClient = new Mock<IGestorMapeosServiceClient>
            {
                CallBase = true
            };

            var sut = new RelateExpedientesAsignaturasCommandHandler(Context, 
                mockIGestorMapeosServiceClient.Object, null);

            var errorMessage = "No está mapeado el plan de estudio con ningún estudio del Gestor " +
                    "y no se pueden recuperar reconocimientos";
            var request = new RelateExpedientesAsignaturasCommand(new ExpedienteAlumno());

            //ACT
            var actual = await sut.Handle(request, CancellationToken.None);

            //ASSERT
            Assert.Equal(errorMessage, actual.First());
        }

        [Fact(DisplayName = "Cuando no existe reconocimiento Retorna lista de error")]
        public async Task Handle_Reconocimiento_Null()
        {
            //ARRANGE
            var mockIGestorMapeosServiceClient = new Mock<IGestorMapeosServiceClient>
            {
                CallBase = true
            };
            var mockIExpedientesGestorUnirServiceClient = new Mock<IExpedientesGestorUnirServiceClient>
            {
                CallBase = true
            };

            var sut = new RelateExpedientesAsignaturasCommandHandler(Context,
                mockIGestorMapeosServiceClient.Object, mockIExpedientesGestorUnirServiceClient.Object);

            var estudios = new List<EstudioGestorMapeoModel>
            {
                new()
                {
                    IdEstudioGestor = 1
                },
                new()
                {
                    IdEstudioGestor = 2
                }
            };

            mockIGestorMapeosServiceClient.Setup(x => x.GetEstudios(It.IsAny<string>(),
                It.IsAny<string>(), It.IsAny<int?>())).ReturnsAsync(estudios);

            mockIExpedientesGestorUnirServiceClient.Setup(x => x.GetReconocimientos(It.IsAny<string>(),
                It.IsAny<int>())).ReturnsAsync(null as ReconocimientoIntegrationGestorModel);
            
            var errorMessage = "No se encontraron reconocimientos para el expediente";
            var request = new RelateExpedientesAsignaturasCommand(new ExpedienteAlumno());

            //ACT
            var actual = await sut.Handle(request, CancellationToken.None);

            //ASSERT
            Assert.Equal(errorMessage, actual.First());
            mockIGestorMapeosServiceClient.Verify(x => x.GetEstudios(It.IsAny<string>(),
                It.IsAny<string>(), It.IsAny<int?>()), Times.Once);
            mockIExpedientesGestorUnirServiceClient.Verify(x => x.GetReconocimientos(
                It.IsAny<string>(), It.IsAny<int>()), Times.Exactly(2));
        }

        [Fact(DisplayName = "Cuando el proceso es correcto Retorna Ok")]
        public async Task Handle_Ok()
        {
            //ARRANGE
            var mockIGestorMapeosServiceClient = new Mock<IGestorMapeosServiceClient>
            {
                CallBase = true
            };
            var mockIExpedientesGestorUnirServiceClient = new Mock<IExpedientesGestorUnirServiceClient>
            {
                CallBase = true
            };

            var sutMock = new Mock<RelateExpedientesAsignaturasCommandHandler>(Context,
                mockIGestorMapeosServiceClient.Object, mockIExpedientesGestorUnirServiceClient.Object);

            var estudios = new List<EstudioGestorMapeoModel>
            {
                new()
                {
                    IdEstudioGestor = 1
                }
            };

            mockIGestorMapeosServiceClient.Setup(x => x.GetEstudios(It.IsAny<string>(),
                It.IsAny<string>(), It.IsAny<int?>())).ReturnsAsync(estudios);

            var reconocimiento = new ReconocimientoIntegrationGestorModel
            {
                CodigoResultado = 0,
                Reconocimiento = new ReconocimientoGestorModel()
            };
            mockIExpedientesGestorUnirServiceClient.Setup(x => x.GetReconocimientos(It.IsAny<string>(),
                It.IsAny<int>())).ReturnsAsync(reconocimiento);
            sutMock.Setup(x => x.GetSeminariosReconocimientos(It.IsAny<List<SeminarioGestorModel>>(), 
                It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
            sutMock.Setup(x => x.GetAsignaturasReconocimientoAdaptacion(It.IsAny<List<AsignaturaGestorModel>>(), 
                It.IsAny<int>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

            var request = new RelateExpedientesAsignaturasCommand(new ExpedienteAlumno());

            //ACT
            var actual = await sutMock.Object.Handle(request, CancellationToken.None);

            //ASSERT
            Assert.Empty(actual);
            mockIGestorMapeosServiceClient.Verify(x => x.GetEstudios(It.IsAny<string>(),
                It.IsAny<string>(), It.IsAny<int?>()), Times.Once);
            mockIExpedientesGestorUnirServiceClient.Verify(x => x.GetReconocimientos(
                It.IsAny<string>(), It.IsAny<int>()), Times.Once);
            sutMock.Verify(x => x.GetSeminariosReconocimientos(It.IsAny<List<SeminarioGestorModel>>(),
                It.IsAny<CancellationToken>()), Times.Once);
            sutMock.Verify(x => x.GetAsignaturasReconocimientoAdaptacion(It.IsAny<List<AsignaturaGestorModel>>(),
                It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        #endregion

        #region GetAsignaturasReconocimientoAdaptacion

        [Fact(DisplayName = "Cuando no existen asignaturas reconocimientos Termina el proceso")]
        public async Task GetAsignaturasReconocimientoAdaptacion_Empty()
        {
            //ARRANGE
            var asignaturas = new List<AsignaturaGestorModel>();
            var idEstudioGestor = 1;
            var sut = new RelateExpedientesAsignaturasCommandHandler(null, null, null);

            //ACT
            await sut.GetAsignaturasReconocimientoAdaptacion(asignaturas, idEstudioGestor, CancellationToken.None);

            //ASSERT
            Assert.Empty(asignaturas);
        }

        [Fact(DisplayName = "Cuando no existen asignaturas reconocimientos de tipo adaptación Termina el proceso")]
        public async Task GetAsignaturasReconocimientoAdaptacion_Adaptacion_Empty()
        {
            //ARRANGE
            var estadoSolicitud = 8;
            var tipoSolicitud = Guid.NewGuid().ToString();
            var asignaturas = new List<AsignaturaGestorModel>
            {
                new()
                {
                    Reconocimientos = new List<ReconocimientoCommonGestorModel>
                    {
                        new()
                        {
                            IdEstadoSolicitud = estadoSolicitud,
                            TipoSolicitud = tipoSolicitud
                        }
                    }
                }
            };
            var idEstudioGestor = 1;
            var sut = new RelateExpedientesAsignaturasCommandHandler(null, null, null);

            //ACT
            await sut.GetAsignaturasReconocimientoAdaptacion(asignaturas, idEstudioGestor, CancellationToken.None);

            //ASSERT
            Assert.NotEqual(estadoSolicitud, RelateExpedientesAsignaturasCommandHandler.EstadoSolicitudValido);
            Assert.NotEqual(tipoSolicitud, RelateExpedientesAsignaturasCommandHandler.TipoSolicitudValido);
        }

        [Fact(DisplayName = "Cuando no existen asignaturas reconocimientos en el Gestor de Mapeos Termina el proceso")]
        public async Task GetAsignaturasReconocimientoAdaptacion_AsignaturasEstudio_Empty()
        {
            //ARRANGE

            var mockIGestorMapeosServiceClient = new Mock<IGestorMapeosServiceClient>
            {
                CallBase = true
            };
            var sut = new RelateExpedientesAsignaturasCommandHandler(null, mockIGestorMapeosServiceClient.Object, null);

            var asignaturasGestor = new List<AsignaturaGestorMapeoModel>();
            mockIGestorMapeosServiceClient.Setup(x => x.GetAsignaturas(It.IsAny<int?>(), It.IsAny<int?>()))
                .ReturnsAsync(asignaturasGestor);

            var asignaturas = new List<AsignaturaGestorModel>
            {
                new()
                {
                    Reconocimientos = new List<ReconocimientoCommonGestorModel>
                    {
                        new()
                        {
                            IdEstadoSolicitud = RelateExpedientesAsignaturasCommandHandler.EstadoSolicitudValido,
                            TipoSolicitud = RelateExpedientesAsignaturasCommandHandler.TipoSolicitudValido
                        }
                    }
                }
            };
            var idEstudioGestor = 1;

            //ACT
            await sut.GetAsignaturasReconocimientoAdaptacion(asignaturas, idEstudioGestor, CancellationToken.None);

            //ASSERT
            Assert.Empty(asignaturasGestor);
            mockIGestorMapeosServiceClient.Verify(x => x.GetAsignaturas(It.IsAny<int?>(), It.IsAny<int?>()), Times.Once);
        }

        [Fact(DisplayName = "Cuando al obtener la asignatura expediente del reconocimiento devuelve Null Termina el proceso")]
        public async Task GetAsignaturasReconocimientoAdaptacion_AsignaturaExpediente_Null()
        {
            //ARRANGE

            var mockIGestorMapeosServiceClient = new Mock<IGestorMapeosServiceClient>
            {
                CallBase = true
            };
            var sutMock = new Mock<RelateExpedientesAsignaturasCommandHandler>(null, mockIGestorMapeosServiceClient.Object, null)
                { CallBase = true };

            var asignaturasGestor = new List<AsignaturaGestorMapeoModel>
            {
                new()
            };
            mockIGestorMapeosServiceClient.Setup(x => x.GetAsignaturas(It.IsAny<int?>(), It.IsAny<int?>()))
                .ReturnsAsync(asignaturasGestor);
            sutMock.Setup(x => x.GetAsignaturaExpediente(It.IsAny<int>(),
                    It.IsAny<List<AsignaturaGestorMapeoModel>>(), It.IsAny<AsignaturaGestorModel>()))
                .Returns(null as AsignaturaExpediente);

            var asignaturas = new List<AsignaturaGestorModel>
            {
                new()
                {
                    Reconocimientos = new List<ReconocimientoCommonGestorModel>
                    {
                        new()
                        {
                            IdEstadoSolicitud = RelateExpedientesAsignaturasCommandHandler.EstadoSolicitudValido,
                            TipoSolicitud = RelateExpedientesAsignaturasCommandHandler.TipoSolicitudValido
                        }
                    }
                }
            };
            var idEstudioGestor = 1;

            //ACT
            await sutMock.Object.GetAsignaturasReconocimientoAdaptacion(asignaturas, idEstudioGestor, CancellationToken.None);

            //ASSERT
            mockIGestorMapeosServiceClient.Verify(x => x.GetAsignaturas(It.IsAny<int?>(), It.IsAny<int?>()), Times.Once);
            sutMock.Verify(x => x.GetAsignaturaExpediente(It.IsAny<int>(),
                It.IsAny<List<AsignaturaGestorMapeoModel>>(), It.IsAny<AsignaturaGestorModel>()), Times.Once);
        }

        [Fact(DisplayName = "Cuando al obtener la asignatura origen es Null Termina el proceso")]
        public async Task GetAsignaturasReconocimientoAdaptacion_AsignaturaOrigen_Null()
        {
            //ARRANGE

            var mockIGestorMapeosServiceClient = new Mock<IGestorMapeosServiceClient>
            {
                CallBase = true
            };
            var sutMock = new Mock<RelateExpedientesAsignaturasCommandHandler>(null, mockIGestorMapeosServiceClient.Object, null)
            { CallBase = true };

            var asignaturasGestor = new List<AsignaturaGestorMapeoModel>
            {
                new()
            };
            mockIGestorMapeosServiceClient.Setup(x => x.GetAsignaturas(It.IsAny<int?>(), It.IsAny<int?>()))
                .ReturnsAsync(asignaturasGestor);
            sutMock.Setup(x => x.GetAsignaturaExpediente(It.IsAny<int>(),
                    It.IsAny<List<AsignaturaGestorMapeoModel>>(), It.IsAny<AsignaturaGestorModel>()))
                .Returns(new AsignaturaExpediente());
            sutMock.Setup(x => x.GetAsignaturaOrigen(It.IsAny<int>()))
                .ReturnsAsync(null as AsignaturaIntegrationGestorMapeoModel);

            var asignaturas = new List<AsignaturaGestorModel>
            {
                new()
                {
                    Reconocimientos = new List<ReconocimientoCommonGestorModel>
                    {
                        new()
                        {
                            IdEstadoSolicitud = RelateExpedientesAsignaturasCommandHandler.EstadoSolicitudValido,
                            TipoSolicitud = RelateExpedientesAsignaturasCommandHandler.TipoSolicitudValido
                        }
                    }
                }
            };
            var idEstudioGestor = 1;

            //ACT
            await sutMock.Object.GetAsignaturasReconocimientoAdaptacion(asignaturas, idEstudioGestor, CancellationToken.None);

            //ASSERT
            mockIGestorMapeosServiceClient.Verify(x => x.GetAsignaturas(It.IsAny<int?>(), It.IsAny<int?>()), Times.Once);
            sutMock.Verify(x => x.GetAsignaturaExpediente(It.IsAny<int>(),
                It.IsAny<List<AsignaturaGestorMapeoModel>>(), It.IsAny<AsignaturaGestorModel>()), Times.Once);
            sutMock.Verify(x => x.GetAsignaturaOrigen(It.IsAny<int>()), Times.Once);
        }

        [Fact(DisplayName = "Cuando al obtener las asignaturas origen es Null Termina el proceso")]
        public async Task GetAsignaturasReconocimientoAdaptacion_AsignaturasOrigen_Null()
        {
            //ARRANGE

            var mockIGestorMapeosServiceClient = new Mock<IGestorMapeosServiceClient>
            {
                CallBase = true
            };
            var sutMock = new Mock<RelateExpedientesAsignaturasCommandHandler>(null, mockIGestorMapeosServiceClient.Object, null)
            { CallBase = true };

            var asignaturasGestor = new List<AsignaturaGestorMapeoModel>
            {
                new()
            };
            var asignatura = new AsignaturaIntegrationGestorMapeoModel
            {
                Id = 1,
                AsignaturaUnir = new AsignaturaUnirIntegrationGestorMapeoModel
                {
                    EstudioUnir = new EstudioUnirIntegrationGestorMapeoModel
                    {
                        Id = 1
                    }
                }
            };
            mockIGestorMapeosServiceClient.Setup(x => x.GetAsignaturas(It.IsAny<int?>(), It.IsAny<int?>()))
                .ReturnsAsync(asignaturasGestor);
            sutMock.Setup(x => x.GetAsignaturaExpediente(It.IsAny<int>(),
                    It.IsAny<List<AsignaturaGestorMapeoModel>>(), It.IsAny<AsignaturaGestorModel>()))
                .Returns(new AsignaturaExpediente());
            sutMock.Setup(x => x.GetAsignaturaOrigen(It.IsAny<int>()))
                .ReturnsAsync(asignatura);
            sutMock.Setup(x => x.GetAsignaturasGestorOrigen(It.IsAny<int>()))
                .ReturnsAsync(null as List<AsignaturaGestorMapeoModel>);

            var asignaturas = new List<AsignaturaGestorModel>
            {
                new()
                {
                    Reconocimientos = new List<ReconocimientoCommonGestorModel>
                    {
                        new()
                        {
                            IdEstadoSolicitud = RelateExpedientesAsignaturasCommandHandler.EstadoSolicitudValido,
                            TipoSolicitud = RelateExpedientesAsignaturasCommandHandler.TipoSolicitudValido
                        }
                    }
                }
            };
            var idEstudioGestor = 1;

            //ACT
            await sutMock.Object.GetAsignaturasReconocimientoAdaptacion(asignaturas, idEstudioGestor, CancellationToken.None);

            //ASSERT
            mockIGestorMapeosServiceClient.Verify(x => x.GetAsignaturas(It.IsAny<int?>(), It.IsAny<int?>()), Times.Once);
            sutMock.Verify(x => x.GetAsignaturaExpediente(It.IsAny<int>(),
                It.IsAny<List<AsignaturaGestorMapeoModel>>(), It.IsAny<AsignaturaGestorModel>()), Times.Once);
            sutMock.Verify(x => x.GetAsignaturaOrigen(It.IsAny<int>()), Times.Once);
            sutMock.Verify(x => x.GetAsignaturasGestorOrigen(It.IsAny<int>()), Times.Once);
        }


        [Fact(DisplayName = "Cuando el expediente a relacionar es Null Termina el proceso")]
        public async Task GetAsignaturasReconocimientoAdaptacion_ExpedienteAlumnoARelacionar_Null()
        {
            //ARRANGE

            var mockIGestorMapeosServiceClient = new Mock<IGestorMapeosServiceClient>
            {
                CallBase = true
            };
            var sutMock = new Mock<RelateExpedientesAsignaturasCommandHandler>(null, mockIGestorMapeosServiceClient.Object, null)
            { CallBase = true };

            var asignaturasGestor = new List<AsignaturaGestorMapeoModel>
            {
                new()
            };
            var asignatura = new AsignaturaIntegrationGestorMapeoModel
            {
                Id = 1,
                AsignaturaUnir = new AsignaturaUnirIntegrationGestorMapeoModel
                {
                    EstudioUnir = new EstudioUnirIntegrationGestorMapeoModel
                    {
                        Id = 1
                    }
                },
                AsignaturaPlan = new AsignaturaPlanIntegrationGestorMapeoModel
                {
                    Plan = new PlanIntegrationGestorMapeoModel
                    {
                        Id = 1
                    }
                }
            };
            var asignaturasOrigen = new List<AsignaturaGestorMapeoModel>
            {
                new()
            };
            mockIGestorMapeosServiceClient.Setup(x => x.GetAsignaturas(It.IsAny<int?>(), It.IsAny<int?>()))
                .ReturnsAsync(asignaturasGestor);
            sutMock.Setup(x => x.GetAsignaturaExpediente(It.IsAny<int>(),
                    It.IsAny<List<AsignaturaGestorMapeoModel>>(), It.IsAny<AsignaturaGestorModel>()))
                .Returns(new AsignaturaExpediente());
            sutMock.Setup(x => x.GetAsignaturaOrigen(It.IsAny<int>()))
                .ReturnsAsync(asignatura);
            sutMock.Setup(x => x.GetAsignaturasGestorOrigen(It.IsAny<int>()))
                .ReturnsAsync(asignaturasOrigen);
            sutMock.Setup(x => x.GetExpedienteAlumnoARelacionar(It.IsAny<string>(), 
                    It.IsAny<CancellationToken>())).ReturnsAsync(null as ExpedienteAlumno);

            var asignaturas = new List<AsignaturaGestorModel>
            {
                new()
                {
                    Reconocimientos = new List<ReconocimientoCommonGestorModel>
                    {
                        new()
                        {
                            IdEstadoSolicitud = RelateExpedientesAsignaturasCommandHandler.EstadoSolicitudValido,
                            TipoSolicitud = RelateExpedientesAsignaturasCommandHandler.TipoSolicitudValido
                        }
                    }
                }
            };
            var idEstudioGestor = 1;

            //ACT
            await sutMock.Object.GetAsignaturasReconocimientoAdaptacion(asignaturas, idEstudioGestor, CancellationToken.None);

            //ASSERT
            mockIGestorMapeosServiceClient.Verify(x => x.GetAsignaturas(It.IsAny<int?>(), It.IsAny<int?>()), Times.Once);
            sutMock.Verify(x => x.GetAsignaturaExpediente(It.IsAny<int>(),
                It.IsAny<List<AsignaturaGestorMapeoModel>>(), It.IsAny<AsignaturaGestorModel>()), Times.Once);
            sutMock.Verify(x => x.GetAsignaturaOrigen(It.IsAny<int>()), Times.Once);
            sutMock.Verify(x => x.GetAsignaturasGestorOrigen(It.IsAny<int>()), Times.Once);
            sutMock.Verify(x => x.GetExpedienteAlumnoARelacionar(It.IsAny<string>(),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact(DisplayName = "Cuando se agregan las relaciones entre expedientes y asignaturas Retorna Ok")]
        public async Task GetAsignaturasReconocimientoAdaptacion_Ok()
        {
            //ARRANGE

            var mockIGestorMapeosServiceClient = new Mock<IGestorMapeosServiceClient>
            {
                CallBase = true
            };
            var sutMock = new Mock<RelateExpedientesAsignaturasCommandHandler>(null, mockIGestorMapeosServiceClient.Object, null)
            { CallBase = true };

            var asignaturasGestor = new List<AsignaturaGestorMapeoModel>
            {
                new()
            };
            var asignatura = new AsignaturaIntegrationGestorMapeoModel
            {
                Id = 1,
                AsignaturaUnir = new AsignaturaUnirIntegrationGestorMapeoModel
                {
                    EstudioUnir = new EstudioUnirIntegrationGestorMapeoModel
                    {
                        Id = 1
                    }
                },
                AsignaturaPlan = new AsignaturaPlanIntegrationGestorMapeoModel
                {
                    Plan = new PlanIntegrationGestorMapeoModel
                    {
                        Id = 1
                    }
                }
            };
            var asignaturasOrigen = new List<AsignaturaGestorMapeoModel>
            {
                new()
            };
            mockIGestorMapeosServiceClient.Setup(x => x.GetAsignaturas(It.IsAny<int?>(), It.IsAny<int?>()))
                .ReturnsAsync(asignaturasGestor);
            sutMock.Setup(x => x.GetAsignaturaExpediente(It.IsAny<int>(),
                    It.IsAny<List<AsignaturaGestorMapeoModel>>(), It.IsAny<AsignaturaGestorModel>()))
                .Returns(new AsignaturaExpediente());
            sutMock.Setup(x => x.GetAsignaturaOrigen(It.IsAny<int>()))
                .ReturnsAsync(asignatura);
            sutMock.Setup(x => x.GetAsignaturasGestorOrigen(It.IsAny<int>()))
                .ReturnsAsync(asignaturasOrigen);
            sutMock.Setup(x => x.GetExpedienteAlumnoARelacionar(It.IsAny<string>(),
                    It.IsAny<CancellationToken>())).ReturnsAsync(new ExpedienteAlumno());
            sutMock.Setup(x => x.RelacionarAsignaturas(It.IsAny<AsignaturaExpediente>(),
                    It.IsAny<List<ReconocimientoCommonGestorModel>>(), It.IsAny<ExpedienteAlumno>(), 
                    It.IsAny<List<AsignaturaGestorMapeoModel>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);
            sutMock.Setup(x => x.AssignRelacionExpediente(It.IsAny<int>(),
                It.IsAny<int>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

            var asignaturas = new List<AsignaturaGestorModel>
            {
                new()
                {
                    Reconocimientos = new List<ReconocimientoCommonGestorModel>
                    {
                        new()
                        {
                            IdEstadoSolicitud = RelateExpedientesAsignaturasCommandHandler.EstadoSolicitudValido,
                            TipoSolicitud = RelateExpedientesAsignaturasCommandHandler.TipoSolicitudValido
                        }
                    }
                }
            };
            var idEstudioGestor = 1;

            //ACT
            await sutMock.Object.GetAsignaturasReconocimientoAdaptacion(asignaturas, idEstudioGestor, CancellationToken.None);

            //ASSERT
            mockIGestorMapeosServiceClient.Verify(x => x.GetAsignaturas(It.IsAny<int?>(), It.IsAny<int?>()), Times.Once);
            sutMock.Verify(x => x.GetAsignaturaExpediente(It.IsAny<int>(),
                It.IsAny<List<AsignaturaGestorMapeoModel>>(), It.IsAny<AsignaturaGestorModel>()), Times.Once);
            sutMock.Verify(x => x.GetAsignaturaOrigen(It.IsAny<int>()), Times.Once);
            sutMock.Verify(x => x.GetAsignaturasGestorOrigen(It.IsAny<int>()), Times.Once);
            sutMock.Verify(x => x.GetExpedienteAlumnoARelacionar(It.IsAny<string>(),
                It.IsAny<CancellationToken>()), Times.Once);
            sutMock.Verify(x => x.RelacionarAsignaturas(It.IsAny<AsignaturaExpediente>(),
                It.IsAny<List<ReconocimientoCommonGestorModel>>(), It.IsAny<ExpedienteAlumno>(),
                It.IsAny<List<AsignaturaGestorMapeoModel>>(), It.IsAny<CancellationToken>()), Times.Once);
            sutMock.Verify(x => x.AssignRelacionExpediente(It.IsAny<int>(),
                It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        #endregion

        #region GetAsignaturaExpediente

        [Fact(DisplayName = "Cuando no existe la asignatura del reconocimiento en el gestor Retorna lista de error")]
        public void GetAsignaturaExpediente_Error_Asignatura()
        {
            //ARRANGE
            var idEstudioGestor = 1;
            var asignaturasGestor = new List<AsignaturaGestorMapeoModel>();
            var asignatura = new AsignaturaGestorModel();
            var sut = new RelateExpedientesAsignaturasCommandHandler(null, null, null);
            var errorMessage = $"No se encontró la asignatura con id {asignatura.IdAsignaturaUnir} " +
                               $"en el estudio {idEstudioGestor} del Gestor";

            //ACT
            var actual = sut.GetAsignaturaExpediente(idEstudioGestor, asignaturasGestor, asignatura);

            //ASSERT
            Assert.Null(actual);
            Assert.Equal(errorMessage, sut.Mensajes.First());
        }

        [Fact(DisplayName = "Cuando existe la asignatura del reconocimiento en el gestor Retorna Ok")]
        public void GetAsignaturaExpediente_Ok()
        {
            //ARRANGE
            var idEstudioGestor = 1;
            var asignaturasGestor = new List<AsignaturaGestorMapeoModel>
            {
                new()
                {
                    IdAsignaturaEstudioGestor = 1,
                    IdAsignaturaPlanErp = 123
                }
            };
            var asignatura = new AsignaturaGestorModel
            {
                IdAsignaturaUnir = 1
            };
            var sut = new RelateExpedientesAsignaturasCommandHandler(null, null, null);
            sut.ExpedienteAlumno = new ExpedienteAlumno
            {
                AsignaturasExpedientes = new List<AsignaturaExpediente>
                {
                    new()
                    {
                        IdRefAsignaturaPlan = "123"
                    }
                }
            };

            //ACT
            var actual = sut.GetAsignaturaExpediente(idEstudioGestor, asignaturasGestor, asignatura);

            //ASSERT
            Assert.NotNull(actual);
            Assert.Empty(sut.Mensajes);
        }

        #endregion

        #region GetAsignaturaOrigen

        [Fact(DisplayName = "Cuando no se encuentra la asignatura origen en el Gestor de Mapeos Retorna lista de error")]
        public async Task GetAsignaturaOrigen_Error_AsignaturaOrigen()
        {
            //ARRANGE
            var mockIGestorMapeosServiceClient = new Mock<IGestorMapeosServiceClient>
            {
                CallBase = true
            };

            var sut = new RelateExpedientesAsignaturasCommandHandler(Context,
                mockIGestorMapeosServiceClient.Object, null);

            mockIGestorMapeosServiceClient.Setup(x => x.GetAsignatura(It.IsAny<int>()))
                .ReturnsAsync(null as AsignaturaIntegrationGestorMapeoModel);

            var idAsignaturaOrigen = 1;
            var errorMessage = $"No se encontro la asignatura origen {idAsignaturaOrigen} del reconocimiento";

            //ACT
            var actual = await sut.GetAsignaturaOrigen(idAsignaturaOrigen);

            //ASSERT
            Assert.Null(actual);
            Assert.Equal(errorMessage, sut.Mensajes.First());
        }

        [Fact(DisplayName = "Cuando se encuentra la asignatura origen en el Gestor de Mapeos Retorna Ok")]
        public async Task GetAsignaturaOrigen_Ok()
        {
            //ARRANGE
            var mockIGestorMapeosServiceClient = new Mock<IGestorMapeosServiceClient>
            {
                CallBase = true
            };

            var sut = new RelateExpedientesAsignaturasCommandHandler(Context,
                mockIGestorMapeosServiceClient.Object, null);

            var idAsignaturaOrigen = 1;
            var asignatura = new AsignaturaIntegrationGestorMapeoModel
            {
                Id = 1
            };
            mockIGestorMapeosServiceClient.Setup(x => x.GetAsignatura(It.IsAny<int>()))
                .ReturnsAsync(asignatura);

            //ACT
            var actual = await sut.GetAsignaturaOrigen(idAsignaturaOrigen);

            //ASSERT
            Assert.NotNull(actual);
            Assert.Equal(asignatura.Id, actual.Id);
            mockIGestorMapeosServiceClient.Verify(x => x.GetAsignatura(It.IsAny<int>()), Times.Once);
        }

        #endregion

        #region GetAsignaturasGestorOrigen

        [Fact(DisplayName = "Cuando no se encuentran las asignaturas origen en el estudio del Gestor de Mapeos Retorna lista de error")]
        public async Task GetAsignaturasGestorOrigen_Error_AsignaturaEstudio()
        {
            //ARRANGE
            var mockIGestorMapeosServiceClient = new Mock<IGestorMapeosServiceClient>
            {
                CallBase = true
            };

            var sut = new RelateExpedientesAsignaturasCommandHandler(Context,
                mockIGestorMapeosServiceClient.Object, null);

            mockIGestorMapeosServiceClient.Setup(x => x.GetAsignaturas(It.IsAny<int?>(),
                It.IsAny<int?>())).ReturnsAsync(new List<AsignaturaGestorMapeoModel>());

            var idEstudioGestorOrigen = 1;
            var errorMessage = $"No se encontraron las asignaturas origen en el estudio {idEstudioGestorOrigen} del Gestor";

            //ACT
            var actual = await sut.GetAsignaturasGestorOrigen(idEstudioGestorOrigen);

            //ASSERT
            Assert.Null(actual);
            Assert.Equal(errorMessage, sut.Mensajes.First());
            mockIGestorMapeosServiceClient.Verify(x => x.GetAsignaturas(It.IsAny<int?>(),
                It.IsAny<int?>()), Times.Once);
        }

        [Fact(DisplayName = "Cuando se encuentran las asignaturas origen en el estudio del Gestor de Mapeos Retorna Ok")]
        public async Task GetAsignaturasGestorOrigen_Ok()
        {
            //ARRANGE
            var mockIGestorMapeosServiceClient = new Mock<IGestorMapeosServiceClient>
            {
                CallBase = true
            };

            var sut = new RelateExpedientesAsignaturasCommandHandler(Context,
                mockIGestorMapeosServiceClient.Object, null);

            var idEstudioGestorOrigen = 1;
            var asignaturas = new List<AsignaturaGestorMapeoModel>
            {
                new()
            };
            mockIGestorMapeosServiceClient.Setup(x => x.GetAsignaturas(It.IsAny<int?>(), 
                    It.IsAny<int?>())).ReturnsAsync(asignaturas);

            //ACT
            var actual = await sut.GetAsignaturasGestorOrigen(idEstudioGestorOrigen);

            //ASSERT
            Assert.NotNull(actual);
            Assert.NotEmpty(actual);
            mockIGestorMapeosServiceClient.Verify(x => x.GetAsignaturas(It.IsAny<int?>(), 
                It.IsAny<int?>()), Times.Once);
        }

        #endregion

        #region GetExpedienteAlumnoARelacionar

        [Fact(DisplayName = "Cuando no se encuentra la asignatura expediente a relacionar Retorna Null")]
        public async Task GetExpedienteAlumnoARelacionar_Null()
        {
            //ARRANGE
            var idPlan = "1";
            var sut = new RelateExpedientesAsignaturasCommandHandler(Context, null, null);
            sut.ExpedienteAlumno = new ExpedienteAlumno
            {
                Id = 1,
                IdRefIntegracionAlumno = Guid.NewGuid().ToString()
            };

            //ACT
            var actual = await sut.GetExpedienteAlumnoARelacionar(idPlan, CancellationToken.None);

            //ASSERT
            Assert.Null(actual);
        }

        [Fact(DisplayName = "Cuando se encuentra la asignatura expediente a relacionar Retorna Ok")]
        public async Task GetExpedienteAlumnoARelacionar_Ok()
        {
            //ARRANGE
            var idPlan = "1";
            var idRefIntegracionAlumno = Guid.NewGuid().ToString();
            var sut = new RelateExpedientesAsignaturasCommandHandler(Context, null, null);
            sut.ExpedienteAlumno = new ExpedienteAlumno
            {
                Id = 1,
                IdRefIntegracionAlumno = idRefIntegracionAlumno
            };

            var expediente = new ExpedienteAlumno
            {
                Id = 2,
                IdRefIntegracionAlumno = idRefIntegracionAlumno,
                IdRefPlan = idPlan
            };
            await Context.ExpedientesAlumno.AddAsync(expediente);
            await Context.SaveChangesAsync();

            //ACT
            var actual = await sut.GetExpedienteAlumnoARelacionar(idPlan, CancellationToken.None);

            //ASSERT
            Assert.NotNull(actual);
        }

        #endregion

        #region RelacionarAsignaturas

        [Fact(DisplayName = "Cuando no existe la asignatura origen en el gestor Retorna lista de error")]
        public async Task RelacionarAsignaturas_Error_Asignatura()
        {
            //ARRANGE
            var idEstudioGestor = 1;
            var idAsignaturaOrigen = 1;
            var asignaturaExpediente = new AsignaturaExpediente();
            var reconocimientos = new List<ReconocimientoCommonGestorModel>
            {
                new()
                {
                    IdAsignaturaOrigen = idAsignaturaOrigen
                }
            };
            var expedienteAlumnoARelacionar = new ExpedienteAlumno();
            var asignaturasGestorOrigen = new List<AsignaturaGestorMapeoModel>
            {
                new()
                {
                    IdEstudioGestor = idEstudioGestor,
                    IdAsignaturaEstudioGestor = 2
                }
            };

            var sut = new RelateExpedientesAsignaturasCommandHandler(null, null, null);
            var errorMessage = $"No se encontró la asignatura origen con id {idAsignaturaOrigen} " + 
                               $"en el estudio {idEstudioGestor} del Gestor";
            //ACT
            var actual = await sut.RelacionarAsignaturas(asignaturaExpediente, reconocimientos, 
                expedienteAlumnoARelacionar, asignaturasGestorOrigen, CancellationToken.None);

            //ASSERT
            Assert.False(actual);
            Assert.Equal(errorMessage, sut.Mensajes.First());
        }

        [Fact(DisplayName = "Cuando no se encuentra la asignatura del expediente a relacionar Termina el proceso")]
        public async Task RelacionarAsignaturas_NotFound_AsignaturaExpedienteARelacionar()
        {
            //ARRANGE
            var idEstudioGestor = 1;
            var idAsignaturaOrigen = 1;
            var asignaturaExpediente = new AsignaturaExpediente();
            var reconocimientos = new List<ReconocimientoCommonGestorModel>
            {
                new()
                {
                    IdAsignaturaOrigen = idAsignaturaOrigen
                }
            };
            var expedienteAlumnoARelacionar = new ExpedienteAlumno();
            var asignaturasGestorOrigen = new List<AsignaturaGestorMapeoModel>
            {
                new()
                {
                    IdEstudioGestor = idEstudioGestor,
                    IdAsignaturaEstudioGestor = idAsignaturaOrigen
                }
            };

            var sut = new RelateExpedientesAsignaturasCommandHandler(null, null, null);
            
            //ACT
            var actual = await sut.RelacionarAsignaturas(asignaturaExpediente, reconocimientos,
                expedienteAlumnoARelacionar, asignaturasGestorOrigen, CancellationToken.None);

            //ASSERT
            Assert.False(actual);
            Assert.Empty(sut.Mensajes);
        }

        [Fact(DisplayName = "Cuando se encuentra y agrega la asignatura del expediente a relacionar Retorna Ok")]
        public async Task RelacionarAsignaturas_Ok()
        {
            //ARRANGE
            var idEstudioGestor = 1;
            var idAsignaturaOrigen = 1;
            var idAsignaturaPlanErp = 123;
            var asignaturaExpediente = new AsignaturaExpediente();
            var reconocimientos = new List<ReconocimientoCommonGestorModel>
            {
                new()
                {
                    IdAsignaturaOrigen = idAsignaturaOrigen
                }
            };
            var expedienteAlumnoARelacionar = new ExpedienteAlumno
            {
                AsignaturasExpedientes = new List<AsignaturaExpediente>
                {
                    new()
                    {
                        IdRefAsignaturaPlan = idAsignaturaPlanErp.ToString()
                    }
                }
            };
            var asignaturasGestorOrigen = new List<AsignaturaGestorMapeoModel>
            {
                new()
                {
                    IdEstudioGestor = idEstudioGestor,
                    IdAsignaturaEstudioGestor = idAsignaturaOrigen,
                    IdAsignaturaPlanErp = idAsignaturaPlanErp
                }
            };

            var sutMock = new Mock<RelateExpedientesAsignaturasCommandHandler>(null, null, null) { CallBase = true };
            sutMock.Setup(x => x.AssignAsignaturaExpedienteRelacionada(It.IsAny<AsignaturaExpediente>(),
                    It.IsAny<AsignaturaExpediente>(), It.IsAny<ReconocimientoCommonGestorModel>(),
                    It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

            //ACT
            var actual = await sutMock.Object.RelacionarAsignaturas(asignaturaExpediente, reconocimientos,
                expedienteAlumnoARelacionar, asignaturasGestorOrigen, CancellationToken.None);

            //ASSERT
            Assert.True(actual);
            Assert.True(asignaturaExpediente.Reconocida);
            Assert.Equal(asignaturaExpediente.SituacionAsignaturaId, SituacionAsignatura.Superada);
            Assert.Empty(sutMock.Object.Mensajes);
            sutMock.Verify(x => x.AssignAsignaturaExpedienteRelacionada(It.IsAny<AsignaturaExpediente>(),
                It.IsAny<AsignaturaExpediente>(), It.IsAny<ReconocimientoCommonGestorModel>(),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        #endregion

        #region AssignAsignaturaExpedienteRelacionada

        [Fact(DisplayName = "Cuando se modifican los datos de la asignatura relacionada del expediente Retorna Ok")]
        public async Task AssignAsignaturaExpedienteRelacionada_Modify_Ok()
        {
            //ARRANGE
            var asignaturaExpedienteOrigen = new AsignaturaExpediente
            {
                Id = 1
            };
            var asignaturaExpedienteDestino = new AsignaturaExpediente
            {
                Id = 2
            };
            var reconocimiento = new ReconocimientoCommonGestorModel
            {
                FechaFinalizacion = "16/05/2019 15:16:18"
            };
            var sut = new RelateExpedientesAsignaturasCommandHandler(Context, null, null)
            {
                ExpedienteAlumno = new ExpedienteAlumno()
            };

            var asignaturaExpedienteRelacionada = new AsignaturaExpedienteRelacionada
            {
                AsignaturaExpedienteOrigenId = 1,
                AsignaturaExpedienteDestinoId = 2
            };
            await Context.AsignaturasExpedientesRelacionadas.AddAsync(asignaturaExpedienteRelacionada);
            await Context.SaveChangesAsync();

            //ACT
            await sut.AssignAsignaturaExpedienteRelacionada(asignaturaExpedienteOrigen, 
                asignaturaExpedienteDestino, reconocimiento, CancellationToken.None);

            //ASSERT
            await Context.SaveChangesAsync();
            Assert.True((await Context.AsignaturasExpedientesRelacionadas.FirstAsync()).Adaptada);
        }

        [Fact(DisplayName = "Cuando se agrega la asignatura relacionada del expediente Retorna Ok")]
        public async Task AssignAsignaturaExpedienteRelacionada_Create_Ok()
        {
            //ARRANGE
            var asignaturaExpedienteOrigen = new AsignaturaExpediente
            {
                Id = 1
            };
            var asignaturaExpedienteDestino = new AsignaturaExpediente
            {
                Id = 1
            };
            var reconocimiento = new ReconocimientoCommonGestorModel
            {
                FechaFinalizacion = "16/05/2019 15:16:18"
            };
            var sut = new RelateExpedientesAsignaturasCommandHandler(Context, null, null)
            {
                ExpedienteAlumno = new ExpedienteAlumno()
            };

            //ACT
            await sut.AssignAsignaturaExpedienteRelacionada(asignaturaExpedienteOrigen,
                asignaturaExpedienteDestino, reconocimiento, CancellationToken.None);

            //ASSERT
            await Context.SaveChangesAsync();
            Assert.True(await Context.AsignaturasExpedientesRelacionadas.AnyAsync());
        }

        #endregion

        #region AssignRelacionExpediente

        [Fact(DisplayName = "Cuando se agrega la relación del expediente Retorna Ok")]
        public async Task AssignRelacionExpediente_Ok()
        {
            //ARRANGE
            var idTipoRelacion = TipoRelacionExpediente.CambioPlan;
            var idExpedienteAlumnoARelacionar = 1;
            var sut = new RelateExpedientesAsignaturasCommandHandler(Context, null, null)
            {
                ExpedienteAlumno = new ExpedienteAlumno()
            };

            //ACT
            await sut.AssignRelacionExpediente(idTipoRelacion, idExpedienteAlumnoARelacionar, CancellationToken.None);

            //ASSERT
            await Context.SaveChangesAsync();
            Assert.True(await Context.RelacionesExpedientes.AnyAsync());
        }

        #endregion

        #region GetSeminariosReconocimientos

        [Fact(DisplayName = "Cuando no existen reconocimientos seminarios Termina el proceso")]
        public async Task GetAsignaturasReconocimientoSeminario_Empty()
        {
            //ARRANGE
            var seminarios = new List<SeminarioGestorModel>();
            var sut = new RelateExpedientesAsignaturasCommandHandler(null, null, null);

            //ACT
            await sut.GetSeminariosReconocimientos(seminarios, CancellationToken.None);

            //ASSERT
            Assert.Empty(seminarios);
        }

        [Fact(DisplayName = "Cuando no existe el estudio del seminarios Retorna lista de error")]
        public async Task GetAsignaturasReconocimientoSeminario_Estudio_Null()
        {
            //ARRANGE
            var mockIGestorMapeosServiceClient = new Mock<IGestorMapeosServiceClient>
            {
                CallBase = true
            };

            var sut = new RelateExpedientesAsignaturasCommandHandler(Context, 
                mockIGestorMapeosServiceClient.Object, null);
            var idEstudioSeminario = 1;
            var seminarios = new List<SeminarioGestorModel>
            {
                new()
                {
                    IdEstudioSeminario = idEstudioSeminario
                }
            };

            var errorMessage = $"No se encontró el estudio {idEstudioSeminario} del seminario";

            //ACT
            await sut.GetSeminariosReconocimientos(seminarios, CancellationToken.None);

            //ASSERT
            Assert.Equal(errorMessage, sut.Mensajes.First());
        }

        [Fact(DisplayName = "Cuando no existe el expediente a relacionar Retorna lista de error")]
        public async Task GetAsignaturasReconocimientoSeminario_ExpedienteAlumnoARelacionar_Null()
        {
            //ARRANG
            var mockIGestorMapeosServiceClient = new Mock<IGestorMapeosServiceClient>
            {
                CallBase = true
            };

            var sut = new RelateExpedientesAsignaturasCommandHandler(Context,
                mockIGestorMapeosServiceClient.Object, null);

            var idPlan = 1;
            sut.ExpedienteAlumno = new ExpedienteAlumno
            {
                Id = 1,
                IdRefIntegracionAlumno = Guid.NewGuid().ToString()
            };
            var expediente = new ExpedienteAlumno
            {
                Id = 2,
                IdRefIntegracionAlumno = Guid.NewGuid().ToString(),
                IdRefPlan = idPlan.ToString()
            };
            await Context.ExpedientesAlumno.AddAsync(expediente);
            await Context.SaveChangesAsync();

            var estudio = new EstudioIntegrationGestorMapeoModel
            {
                PlantillaEstudioIntegracion = new PlantillaEstudioIntegracionGestorMapeoModel
                {
                    Plan = new PlanIntegrationGestorMapeoModel
                    {
                        Id = idPlan
                    }
                }
            };
            mockIGestorMapeosServiceClient.Setup(x => x.GetEstudio(
                It.IsAny<int>())).ReturnsAsync(estudio);

            var seminarios = new List<SeminarioGestorModel>
            {
                new()
                {
                    IdEstudioSeminario = 1
                }
            };
            var errorMessage = "El seminario no existe como expediente del alumno";

            //ACT
            await sut.GetSeminariosReconocimientos(seminarios, CancellationToken.None);

            //ASSERT
            Assert.Equal(errorMessage, sut.Mensajes.First());
        }

        [Fact(DisplayName = "Cuando los seminarios encuentras expedientes a Relacionar Retorna Ok")]
        public async Task GetAsignaturasReconocimientoSeminario_Ok()
        {
            //ARRANG
            var mockIGestorMapeosServiceClient = new Mock<IGestorMapeosServiceClient>
            {
                CallBase = true
            };

            var sutMock = new Mock<RelateExpedientesAsignaturasCommandHandler>(Context,
                mockIGestorMapeosServiceClient.Object, null) {CallBase = true};

            var idPlan = 1;
            var idRefIntegracionAlumno = Guid.NewGuid().ToString();

            sutMock.Object.ExpedienteAlumno = new ExpedienteAlumno
            {
                Id = 1,
                IdRefIntegracionAlumno = idRefIntegracionAlumno
            };
            var expediente = new ExpedienteAlumno
            {
                Id = 2,
                IdRefIntegracionAlumno = idRefIntegracionAlumno,
                IdRefPlan = idPlan.ToString()
            };
            await Context.ExpedientesAlumno.AddAsync(expediente);
            await Context.SaveChangesAsync();

            var estudio = new EstudioIntegrationGestorMapeoModel
            {
                PlantillaEstudioIntegracion = new PlantillaEstudioIntegracionGestorMapeoModel
                {
                    Plan = new PlanIntegrationGestorMapeoModel
                    {
                        Id = idPlan
                    }
                }
            };
            mockIGestorMapeosServiceClient.Setup(x => x.GetEstudio(
                It.IsAny<int>())).ReturnsAsync(estudio);
            sutMock.Setup(x => x.AssignRelacionExpediente(It.IsAny<int>(),
                It.IsAny<int>(),It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

            var seminarios = new List<SeminarioGestorModel>
            {
                new()
                {
                    IdEstudioSeminario = 1
                }
            };

            //ACT
            await sutMock.Object.GetSeminariosReconocimientos(seminarios, CancellationToken.None);

            //ASSERT
            Assert.Empty(sutMock.Object.Mensajes);
            sutMock.Verify(x => x.AssignRelacionExpediente(It.IsAny<int>(),
                It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        #endregion

    }
}
