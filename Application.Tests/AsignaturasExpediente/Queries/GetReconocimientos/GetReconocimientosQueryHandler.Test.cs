using Microsoft.Extensions.Localization;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Unir.Expedientes.Application.AsignaturasExpediente.Queries.GetReconocimientos;
using Unir.Expedientes.Application.Common.Interfaces;
using Unir.Expedientes.Application.Common.Models.Evaluaciones;
using Unir.Expedientes.Application.Common.Models.ExpedientesGestorUnir;
using Unir.Expedientes.Application.Common.Models.GestorMapeos;
using Unir.Expedientes.Application.Resources.Globalization;
using Unir.Expedientes.Application.Tests.Common;
using Unir.Framework.ApplicationSuperTypes.Exceptions;
using Xunit;

namespace Unir.Expedientes.Application.Tests.AsignaturasExpediente.Queries.GetReconocimientos
{
    [Collection("CommonTestCollection")]
    public class GetReconocimientosQueryHandlerTests : TestBase
    {
        #region Handle

        [Fact(DisplayName = "Cuando no existe el plan de estudio en el Gestor de Mapeos Retorna error")]
        public async Task Handle_BadRequestException_PlanEstudio()
        {
            //ARRANGE
            var mockIStringLocalizer = new Mock<IStringLocalizer<GetReconocimientosQueryHandler>>
            {
                CallBase = true
            };
            var mensajeEsperado = "No está mapeado el plan de estudio con ningún estudio del Gestor y no se pueden recuperar reconocimientos";
            mockIStringLocalizer.Setup(s => s[It.Is<string>(msj => msj == mensajeEsperado)])
                .Returns(new LocalizedString(mensajeEsperado, mensajeEsperado));

            var mockIGestorMapeosServiceClient = new Mock<IGestorMapeosServiceClient>
            {
                CallBase = true
            };

            var sut = new GetReconocimientosQueryHandler(mockIGestorMapeosServiceClient.Object, 
                null, null, mockIStringLocalizer.Object);

            //ACT
            var ex = await Record.ExceptionAsync(async () =>
            {
                await sut.Handle(new GetReconocimientosQuery(null, null, null), CancellationToken.None);
            });

            //ASSERT
            Assert.IsType<BadRequestException>(ex);
            Assert.Equal(mensajeEsperado, ex.Message);
        }

        [Fact(DisplayName = "Cuando no existe reconocimiento Retorna null")]
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

            var sut = new GetReconocimientosQueryHandler(mockIGestorMapeosServiceClient.Object,
                mockIExpedientesGestorUnirServiceClient.Object, null, null);

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

            //ACT
            var actual = await sut.Handle(new GetReconocimientosQuery(null, null, null), 
                CancellationToken.None);

            //ASSERT
            Assert.Null(actual);
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

            var sutMock = new Mock<GetReconocimientosQueryHandler>(mockIGestorMapeosServiceClient.Object,
                mockIExpedientesGestorUnirServiceClient.Object, null, null);

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

            sutMock.Setup(x => x.GetAsignaturasReconocimiento(It.IsAny<ICollection<AsignaturaGestorModel>>(),
                It.IsAny<int>(), It.IsAny<ReconocimientoClasificacionDto>())).Returns(Task.CompletedTask);
            sutMock.Setup(x => x.GetAsignaturasReconocimientoTransversal(It.IsAny<TransversalGestorModel>(), 
                It.IsAny<List<AsignaturaReconocimientoDto>>()));
            sutMock.Setup(x => x.GetAsignaturasReconocimientoSeminario(It.IsAny<ICollection<SeminarioGestorModel>>(),
                It.IsAny<List<AsignaturaReconocimientoDto>>()));
            sutMock.Setup(x => x.GetAsignaturasReconocimientoUniversitaria(It.IsAny<ExtensionUniversitariaGestorModel>(),
                It.IsAny<List<AsignaturaReconocimientoDto>>()));

            //ACT
            var actual = await sutMock.Object.Handle(new GetReconocimientosQuery(null, null, null),
                CancellationToken.None);

            //ASSERT
            Assert.NotNull(actual);
            mockIGestorMapeosServiceClient.Verify(x => x.GetEstudios(It.IsAny<string>(),
                It.IsAny<string>(), It.IsAny<int?>()), Times.Once);
            mockIExpedientesGestorUnirServiceClient.Verify(x => x.GetReconocimientos(
                It.IsAny<string>(), It.IsAny<int>()), Times.Once);
            sutMock.Verify(x => x.GetAsignaturasReconocimiento(It.IsAny<ICollection<AsignaturaGestorModel>>(),
                It.IsAny<int>(), It.IsAny<ReconocimientoClasificacionDto>()), Times.Once);
            sutMock.Verify(x => x.GetAsignaturasReconocimientoTransversal(It.IsAny<TransversalGestorModel>(),
                It.IsAny<List<AsignaturaReconocimientoDto>>()), Times.Once);
            sutMock.Verify(x => x.GetAsignaturasReconocimientoSeminario(It.IsAny<ICollection<SeminarioGestorModel>>(),
                It.IsAny<List<AsignaturaReconocimientoDto>>()), Times.Once);
            sutMock.Verify(x => x.GetAsignaturasReconocimientoUniversitaria(It.IsAny<ExtensionUniversitariaGestorModel>(),
                It.IsAny<List<AsignaturaReconocimientoDto>>()), Times.Once);
        }

        #endregion

        #region GetAsignaturasReconocimientoTransversal

        [Fact(DisplayName = "Cuando no existen reconocimientos transversales Retorna lista vacía")]
        public void GetAsignaturasReconocimientoTransversal_Empty()
        {
            //ARRANGE

            var reconocimientos = new List<AsignaturaReconocimientoDto>();
            var sut = new GetReconocimientosQueryHandler(null, null, null, null);

            //ACT
            sut.GetAsignaturasReconocimientoTransversal(null, reconocimientos);

            //ASSERT
            Assert.Empty(reconocimientos);
        }

        [Fact(DisplayName = "Cuando existen reconocimientos transversales Retorna lista")]
        public void GetAsignaturasReconocimientoTransversal_Ok()
        {
            //ARRANGE
            var reconocimientoTransversal = new TransversalGestorModel
            {
                Ects = 6,
                NotaMedia = 9,
                Reconocimientos = new List<ReconocimientoCommonGestorModel>
                {
                    new()
                    {
                        AsignaturaExterna = Guid.NewGuid().ToString(),
                        EstudioExterno = Guid.NewGuid().ToString(),
                        EctsExterna = 10
                    }
                }
            };
            var reconocimientos = new List<AsignaturaReconocimientoDto>();
            var sutMock = new Mock<GetReconocimientosQueryHandler>(null, null, null, null)
            {
                CallBase = true
            };
            sutMock.Setup(x => x.AssignAsignaturaReconocimientoTransversal(It.IsAny<ReconocimientoCommonGestorModel>(), 
                It.IsAny<double>(), It.IsAny<string>(), It.IsAny<bool>())).Returns(new AsignaturaReconocimientoDto());
            sutMock.Setup(x => x.GetDescripcionCalificacion(
                It.IsAny<ReconocimientoCommonGestorModel>(), It.IsAny<AsignaturaReconocimientoDto>()));

            //ACT
            sutMock.Object.GetAsignaturasReconocimientoTransversal(reconocimientoTransversal, reconocimientos);

            //ASSERT
            Assert.NotEmpty(reconocimientos);
            sutMock.Verify(x => x.AssignAsignaturaReconocimientoTransversal(It.IsAny<ReconocimientoCommonGestorModel>(),
                It.IsAny<double>(), It.IsAny<string>(), It.IsAny<bool>()), Times.Exactly(2));
            sutMock.Verify(x => x.GetDescripcionCalificacion(
                It.IsAny<ReconocimientoCommonGestorModel>(), It.IsAny<AsignaturaReconocimientoDto>()), Times.Once);
        }

        #endregion

        #region GetAsignaturasReconocimiento

        [Fact(DisplayName = "Cuando no existen reconocimientos de asignaturas Retorna lista vacía")]
        public async Task GetAsignaturasReconocimiento_Empty()
        {
            //ARRANGE
            var reconocimientoClasificacion = new ReconocimientoClasificacionDto();
            var sut = new GetReconocimientosQueryHandler(null, null, null, null);

            //ACT
            await sut.GetAsignaturasReconocimiento(null,1 , reconocimientoClasificacion);

            //ASSERT
            Assert.Empty(reconocimientoClasificacion.AsignaturasReconocimientos);
        }

        [Fact(DisplayName = "Cuando no existen las asignaturas en el Gestor de Mapeos Retorna lista vacía")]
        public async Task GetAsignaturasReconocimiento_GestorMapeos_Empty()
        {
            //ARRANGE
            var mockIGestorMapeosServiceClient = new Mock<IGestorMapeosServiceClient>
            {
                CallBase = true
            };

            var reconocimientoClasificacion = new ReconocimientoClasificacionDto();
            var sut = new GetReconocimientosQueryHandler(mockIGestorMapeosServiceClient.Object, null, 
                null, null);

            mockIGestorMapeosServiceClient.Setup(x => x.GetAsignaturas(It.IsAny<int?>(), It.IsAny<int?>()))
                .ReturnsAsync(new List<AsignaturaGestorMapeoModel>());
            var asignaturas = new List<AsignaturaGestorModel>
            {
                new()
                {
                    IdAsignaturaUnir = 1
                },
                new()
                {
                    IdAsignaturaUnir = 2
                }
            };

            //ACT
            await sut.GetAsignaturasReconocimiento(asignaturas, 1, reconocimientoClasificacion);

            //ASSERT
            Assert.Empty(reconocimientoClasificacion.AsignaturasReconocimientos);
            mockIGestorMapeosServiceClient.Verify(x => x.GetAsignaturas(It.IsAny<int?>(), It.IsAny<int?>()), Times.Once);
        }

        [Fact(DisplayName = "Cuando el proceso de obtener las asignaturas de reconocimientos es correcto Retorna Ok")]
        public async Task GetAsignaturasReconocimiento_Ok()
        {
            //ARRANGE
            var mockIGestorMapeosServiceClient = new Mock<IGestorMapeosServiceClient>
            {
                CallBase = true
            };

            var reconocimientoClasificacion = new ReconocimientoClasificacionDto();
            var sutMock = new Mock<GetReconocimientosQueryHandler>(mockIGestorMapeosServiceClient.Object, null,
                null, null) { CallBase = true };

            var asignaturasGestor = new List<AsignaturaGestorMapeoModel>
            {
                new()
                {
                    IdAsignaturaEstudioGestor = 1
                },
                new()
                {
                    IdAsignaturaEstudioGestor = 2
                },
                new()
                {
                    IdAsignaturaEstudioGestor = 3
                }
            };

            mockIGestorMapeosServiceClient.Setup(x => x.GetAsignaturas(It.IsAny<int>(), It.IsAny<int?>()))
                .ReturnsAsync(asignaturasGestor);
            var asignaturas = new List<AsignaturaGestorModel>
            {
                new()
                {
                    IdAsignaturaUnir = 1,
                    NotaMedia = 10,
                    Reconocimientos = new List<ReconocimientoCommonGestorModel>
                    {
                        new()
                        {
                            AsignaturaExterna = Guid.NewGuid().ToString()
                        }
                    }
                },
                new()
                {
                    IdAsignaturaUnir = 2,
                    Reconocimientos = new List<ReconocimientoCommonGestorModel>
                    {
                        new()
                        {
                            AsignaturaExterna = Guid.NewGuid().ToString(),
                            IdEstadoSolicitud = 9
                        }
                    }
                },
                new()
                {
                    IdAsignaturaUnir = 3
                },
                new()
                {
                    IdAsignaturaUnir = 4,
                    Reconocimientos = new List<ReconocimientoCommonGestorModel>
                    {
                        new()
                        {
                            AsignaturaExterna = Guid.NewGuid().ToString()
                        }
                    }
                }
            };

            sutMock.Setup(x => x.GetCalificacionEvaluacionConfiguracionEscala(
                It.IsAny<AsignaturaGestorModel>(), It.IsAny<int>())).ReturnsAsync(string.Empty);
            sutMock.Setup(x => x.SetAsignaturasReconocimientos(It.IsAny<int>(),
                It.IsAny<string>(), It.IsAny<ICollection<ReconocimientoCommonGestorModel>>(), It.IsAny<List<AsignaturaReconocimientoDto>>()));

            //ACT
            await sutMock.Object.GetAsignaturasReconocimiento(asignaturas, 1, reconocimientoClasificacion);

            //ASSERT
            Assert.NotEmpty(reconocimientoClasificacion.MensajesError);
            mockIGestorMapeosServiceClient.Verify(x => x.GetAsignaturas(It.IsAny<int>(), It.IsAny<int?>()), Times.Once);
            sutMock.Verify(x => x.GetCalificacionEvaluacionConfiguracionEscala(
                It.IsAny<AsignaturaGestorModel>(), It.IsAny<int>()), Times.Once);
            sutMock.Verify(x => x.SetAsignaturasReconocimientos(It.IsAny<int>(),
                It.IsAny<string>(), It.IsAny<ICollection<ReconocimientoCommonGestorModel>>(), 
                It.IsAny<List<AsignaturaReconocimientoDto>>()), Times.Exactly(2));
        }

        #endregion

        #region SetAsignaturasReconocimientos

        [Fact(DisplayName = "Cuando se setean las asignaturas de reconocmiento Retorna lista")]
        public void SetAsignaturasReconocimientos_Ok()
        {
            //ARRANGE
            var reconocimientos = new List<ReconocimientoCommonGestorModel>
            {
                new()
                {
                    AsignaturaExterna = Guid.NewGuid().ToString(),
                    EstudioExterno = Guid.NewGuid().ToString(),
                    EctsExterna = 10
                }
            };
            var reconocimientosAsignaturas = new List<AsignaturaReconocimientoDto>();
            var sutMock = new Mock<GetReconocimientosQueryHandler>(null, null, null, null)
            {
                CallBase = true
            };
            sutMock.Setup(x => x.AssignAsignaturaReconocimiento(It.IsAny<int>(), It.IsAny<string>(), 
                It.IsAny<ReconocimientoCommonGestorModel>())).Returns(new AsignaturaReconocimientoDto());
            sutMock.Setup(x => x.GetDescripcionCalificacion(
                It.IsAny<ReconocimientoCommonGestorModel>(), It.IsAny<AsignaturaReconocimientoDto>()));
            sutMock.Setup(x => x.GetDescripcionTipoSolicitud(
                It.IsAny<string>())).Returns(Guid.NewGuid().ToString());

            //ACT
            sutMock.Object.SetAsignaturasReconocimientos(1, Guid.NewGuid().ToString(), 
                reconocimientos, reconocimientosAsignaturas);

            //ASSERT
            Assert.NotEmpty(reconocimientosAsignaturas);
            sutMock.Verify(x => x.GetDescripcionCalificacion(
                It.IsAny<ReconocimientoCommonGestorModel>(), It.IsAny<AsignaturaReconocimientoDto>()), Times.Once);
            sutMock.Verify(x => x.GetDescripcionTipoSolicitud(
                It.IsAny<string>()), Times.Once);
        }


        #endregion

        #region GetAsignaturasReconocimientoSeminario

        [Fact(DisplayName = "Cuando no existen reconocimientos seminarios Retorna lista vacía")]
        public void GetAsignaturasReconocimientoSeminario_Empty()
        {
            //ARRANGE
            var reconocimientos = new List<AsignaturaReconocimientoDto>();
            var sut = new GetReconocimientosQueryHandler(null, null, null, null);

            //ACT
            sut.GetAsignaturasReconocimientoSeminario(null, reconocimientos);

            //ASSERT
            Assert.Empty(reconocimientos);
        }

        [Fact(DisplayName = "Cuando existen reconocimientos seminarios Retorna lista")]
        public void GetAsignaturasReconocimientoSeminario_Ok()
        {
            //ARRANGE
            var reconocimientoSeminarios = new List<SeminarioGestorModel>
            {
                new()
                {
                    NombreSeminario = Guid.NewGuid().ToString(),
                    Ects = 10
                },
                new()
                {
                    NombreSeminario = Guid.NewGuid().ToString(),
                    Ects = 10
                }
            };
            var reconocimientos = new List<AsignaturaReconocimientoDto>();
            var sutMock = new Mock<GetReconocimientosQueryHandler>(null, null, null, null) { CallBase = true };
            sutMock.Setup(x => x.AssignAsignaturaReconocimientoSeminario(
                It.IsAny<SeminarioGestorModel>())).Returns(new AsignaturaReconocimientoDto());

            //ACT
            sutMock.Object.GetAsignaturasReconocimientoSeminario(reconocimientoSeminarios, reconocimientos);

            //ASSERT
            Assert.NotEmpty(reconocimientos);
            sutMock.Verify(x => x.AssignAsignaturaReconocimientoSeminario(
                It.IsAny<SeminarioGestorModel>()), Times.Exactly(2));
        }

        #endregion

        #region GetAsignaturasReconocimientoUniversitaria

        [Fact(DisplayName = "Cuando no existen reconocimientos de extensión universitaria Retorna lista vacía")]
        public void GetAsignaturasReconocimientoUniversitaria_Empty()
        {
            //ARRANGE
            var reconocimientos = new List<AsignaturaReconocimientoDto>();
            var sut = new GetReconocimientosQueryHandler(null, null, null, null);

            //ACT
            sut.GetAsignaturasReconocimientoUniversitaria(null, reconocimientos);

            //ASSERT
            Assert.Empty(reconocimientos);
        }

        [Fact(DisplayName = "Cuando existen reconocimientos de extensión universitaria Retorna lista")]
        public void GetAsignaturasReconocimientoUniversitaria_Ok()
        {
            //ARRANGE
            var extensionUniversitaria = new ExtensionUniversitariaGestorModel
            {
                Reconocimientos = new List<ReconocimientoCommonGestorModel>
                {
                    new()
                    {
                        AsignaturaExterna = Guid.NewGuid().ToString(),
                        EstudioExterno = Guid.NewGuid().ToString(),
                        EctsExterna = 10
                    }
                }
            };
            var reconocimientos = new List<AsignaturaReconocimientoDto>();
            var sutMock = new Mock<GetReconocimientosQueryHandler>(null, null, null, null)
            {
                CallBase = true
            };
            sutMock.Setup(x => x.AssignAsignaturaReconocimientoUniversitaria(
               It.IsAny<ReconocimientoCommonGestorModel>())).Returns(new AsignaturaReconocimientoDto());
            sutMock.Setup(x => x.GetDescripcionCalificacion(
                It.IsAny<ReconocimientoCommonGestorModel>(), It.IsAny<AsignaturaReconocimientoDto>()));

            //ACT
            sutMock.Object.GetAsignaturasReconocimientoUniversitaria(extensionUniversitaria, reconocimientos);

            //ASSERT
            Assert.NotEmpty(reconocimientos);
            sutMock.Verify(x => x.AssignAsignaturaReconocimientoUniversitaria(
               It.IsAny<ReconocimientoCommonGestorModel>()), Times.Once);
            sutMock.Verify(x => x.GetDescripcionCalificacion(
                It.IsAny<ReconocimientoCommonGestorModel>(), It.IsAny<AsignaturaReconocimientoDto>()), Times.Once);
        }

        #endregion

        #region GetDescripcionCalificacion

        [Fact(DisplayName = "Cuando se obtiene la calificación Retorna Ok")]
        public void GetCalificacion_Ok()
        {
            //ARRANGE
            var sut = new GetReconocimientosQueryHandler(null, null, 
                null, null);

            var reconocimiento = new ReconocimientoCommonGestorModel
            {
                Nota = 10,
                NivelAprobacionDescripcion = Guid.NewGuid().ToString(),
                IdEstadoSolicitud = 9
            };
            var asignatura = new AsignaturaReconocimientoDto();

            //ACT
            sut.GetDescripcionCalificacion(reconocimiento, asignatura);

            //ASSERT
            Assert.NotNull(asignatura.Calificacion);
        }

        [Fact(DisplayName = "Cuando se obtiene la calificación y el estado es menor que 9 Retorna se adiciona al texto (en trámite)")]
        public void GetCalificacion_EnTramite_Ok()
        {
            //ARRANGE
            var sut = new GetReconocimientosQueryHandler(null, null,
                null, null);

            var reconocimiento = new ReconocimientoCommonGestorModel
            {
                Nota = 10,
                NivelAprobacionDescripcion = Guid.NewGuid().ToString(),
                IdEstadoSolicitud = 8
            };
            var asignatura = new AsignaturaReconocimientoDto();
            const string enTramite = "(en trámite)";

            //ACT
            sut.GetDescripcionCalificacion(reconocimiento, asignatura);

            //ASSERT
            Assert.NotNull(asignatura.Calificacion);
            Assert.Contains(enTramite, asignatura.Calificacion);
        }

        [Theory(DisplayName = "Cuando la nota es 0 y el nivel de aprobación es Apto o No Apto Retorna la calificación sin el valor de la Nota")]
        [InlineData("Apto")]
        [InlineData("No Apto")]
        public void GetCalificacion_Apto_NoApto_Ok(string nivelAprobacion)
        {
            //ARRANGE
            var sut = new GetReconocimientosQueryHandler(null, null, null, null);

            var reconocimiento = new ReconocimientoCommonGestorModel
            {
                Nota = 0,
                NivelAprobacionDescripcion = nivelAprobacion,
                IdEstadoSolicitud = 9
            };
            var asignatura = new AsignaturaReconocimientoDto();

            //ACT
            sut.GetDescripcionCalificacion(reconocimiento, asignatura);

            //ASSERT
            Assert.Equal(nivelAprobacion, asignatura.Calificacion);
        }


        [Theory(DisplayName = "Cuando la nota es 0 y el nivel de aprobación es diferente que Apto y No Apto Retorna la calificación con el valor de la Nota")]
        [InlineData("Suspensa")]
        [InlineData("Superada")]
        public void GetCalificacion_Diferente_Apto_NoApto_Ok(string nivelAprobacion)
        {
            //ARRANGE
            var sut = new GetReconocimientosQueryHandler(null, null, null, null);

            var reconocimiento = new ReconocimientoCommonGestorModel
            {
                Nota = 0,
                NivelAprobacionDescripcion = nivelAprobacion,
                IdEstadoSolicitud = 9
            };
            var asignatura = new AsignaturaReconocimientoDto();

            //ACT
            sut.GetDescripcionCalificacion(reconocimiento, asignatura);

            //ASSERT
            Assert.Equal($"{reconocimiento.Nota} {nivelAprobacion}", asignatura.Calificacion);
        }

        #endregion

        #region GetCalificacionEvaluacionConfiguracionEscala

        [Fact(DisplayName = "Cuando no se encuentra configuración escala Retorna sólo el valor de nota media")]
        public async Task GetCalificacionEvaluacionConfiguracionEscala_Empty()
        {
            //ARRANGE
            var mockIEvaluacionesServiceClient = new Mock<IEvaluacionesServiceClient>
            {
                CallBase = true
            };
            
            var sut = new GetReconocimientosQueryHandler(null, null, mockIEvaluacionesServiceClient.Object, null);
            var asignatura = new AsignaturaGestorModel
            {
                NotaMedia = 10
            };

            //ACT
            var actual = await sut.GetCalificacionEvaluacionConfiguracionEscala(asignatura, 1);

            //ASSERT
            Assert.Equal(asignatura.NotaMedia.ToString(), actual);
        }

        [Fact(DisplayName = "Cuando se obtiene la calificación de configuración escala de nota mínima Retorna Ok")]
        public async Task GetCalificacionEvaluacionConfiguracionEscala_NotaMinima_Ok()
        {
            //ARRANGE
            var mockIEvaluacionesServiceClient = new Mock<IEvaluacionesServiceClient>
            {
                CallBase = true
            };

            var sutMock = new Mock<GetReconocimientosQueryHandler>(null, null, mockIEvaluacionesServiceClient.Object, null) 
            { CallBase = true };

            var nombreCalificacion = Guid.NewGuid().ToString();
            var configuracion = new ConfiguracionVersionEscalaModel
            {
                Configuracion = new ConfiguracionModel
                {
                    Calificacion = new CalificacionModel
                    {
                        AfectaNotaNumerica = true,
                        Calificaciones = new List<CalificacionListModel>
                        {
                            new()
                            {
                                Nombre = nombreCalificacion,
                                NotaMinima = 5
                            },
                            new()
                            {
                                NotaMinima = 10
                            }
                        }
                    }
                }
            };
            mockIEvaluacionesServiceClient.Setup(x => x.GetConfiguracionEscalaFromNivelesAsociadosEscalas(
                It.IsAny<int?>(), It.IsAny<int?>())).ReturnsAsync(configuracion);
            var asignatura = new AsignaturaGestorModel
            {
                NotaMedia = 8
            };

            //ACT
            var actual = await sutMock.Object.GetCalificacionEvaluacionConfiguracionEscala(asignatura, 1);

            //ASSERT
            Assert.Contains(nombreCalificacion, actual);
            mockIEvaluacionesServiceClient.Verify(x => x.GetConfiguracionEscalaFromNivelesAsociadosEscalas(
                It.IsAny<int?>(), It.IsAny<int?>()), Times.Once);
        }

        [Fact(DisplayName = "Cuando se obtiene la calificación de configuración escala de porcentaje mínimo Retorna Ok")]
        public async Task GetCalificacionEvaluacionConfiguracionEscala_PorcentajeMinimo_Ok()
        {
            //ARRANGE
            var mockIEvaluacionesServiceClient = new Mock<IEvaluacionesServiceClient>
            {
                CallBase = true
            };

            var sutMock = new Mock<GetReconocimientosQueryHandler>(null, null, mockIEvaluacionesServiceClient.Object, null)
            { CallBase = true };

            var nombreCalificacion = Guid.NewGuid().ToString();
            var configuracion = new ConfiguracionVersionEscalaModel
            {
                Configuracion = new ConfiguracionModel
                {
                    Calificacion = new CalificacionModel
                    {
                        AfectaPorcentaje = true,
                        Calificaciones = new List<CalificacionListModel>
                        {
                            new()
                            {
                                Nombre = nombreCalificacion,
                                PorcentajeMinimo = 5
                            },
                            new()
                            {
                                PorcentajeMinimo = 10
                            }
                        }
                    }
                }
            };
            mockIEvaluacionesServiceClient.Setup(x => x.GetConfiguracionEscalaFromNivelesAsociadosEscalas(
                It.IsAny<int?>(), It.IsAny<int?>())).ReturnsAsync(configuracion);
            var asignatura = new AsignaturaGestorModel
            {
                NotaMedia = 8
            };

            //ACT
            var actual = await sutMock.Object.GetCalificacionEvaluacionConfiguracionEscala(asignatura, 1);

            //ASSERT
            Assert.Contains(nombreCalificacion, actual);
            mockIEvaluacionesServiceClient.Verify(x => x.GetConfiguracionEscalaFromNivelesAsociadosEscalas(
                It.IsAny<int?>(), It.IsAny<int?>()), Times.Once);
        }

        #endregion

        #region GetDescripcionTipoSolicitud

        [Theory(DisplayName = "Cuando se obtiene el tipo de solicitud Retorna Ok")]
        [InlineData(1, "Reconocimiento")]
        [InlineData(2, "Adaptación")]
        [InlineData(3, "Movilidad")]
        public void GetDescripcionTipoSolicitud_Ok(int id, string tipoSolicitud)
        {
            //ARRANGE
            var sut = new GetReconocimientosQueryHandler(null, null, null, null);

            var resultado = id == 1 ? "(Reconocido*)" :
                id == 2 ? "(Adaptado*)" :
                id == 3 ? "(Movilidad*)" : string.Empty;

            //ACT
            var actual = sut.GetDescripcionTipoSolicitud(tipoSolicitud);

            //ASSERT
            Assert.Equal(resultado, actual);
        }

        #endregion

        #region GetNombreAsignaturaReconocimiento

        [Fact(DisplayName = "Cuando se obtiene el nombre de asignatura de reconocimiento Retorna Ok")]
        public void GetNombreAsignaturaReconocimiento_Ok()
        {
            //ARRANGE
            var sut = new GetReconocimientosQueryHandler(null, null, null, null);

            var asignaturaExterna = Guid.NewGuid().ToString();
            var estudioExterno = Guid.NewGuid().ToString();

            //ACT
            var actual = sut.GetNombreAsignaturaReconocimiento(asignaturaExterna, estudioExterno);

            //ASSERT
            Assert.NotNull(actual);
        }

        #endregion

        #region AssignAsignaturaReconocimientoTransversal

        [Fact(DisplayName = "Cuando se asigna la data de asignatura reconocimiento transversal y es principal Retorna Ok")]
        public void AssignAsignaturaReconocimientoTransversal_Principal_Ok()
        {
            //ARRANGE
            var asignaturaTransversal = CommonStrings.NombreAsignaturaTransversal;
            var tipoAsignaturaTransversal = CommonStrings.NombreTipoAsignaturaTransversal;
            var sut = new GetReconocimientosQueryHandler(null, null,
                null, null);
            var ects = 10;
            var calificacion = Guid.NewGuid().ToString();

            //ACT
            var actual = sut.AssignAsignaturaReconocimientoTransversal(null, ects, calificacion, true);

            //ASSERT
            Assert.NotNull(actual);
            Assert.Equal(asignaturaTransversal, actual.NombreAsignatura);
            Assert.Equal(tipoAsignaturaTransversal, actual.NombreTipoAsignatura);
            Assert.Equal(ects, actual.Ects);
            Assert.Equal(calificacion, actual.Calificacion);
            Assert.True(actual.EsTransversal);
            Assert.True(actual.EsTransversalPrincipal);
        }

        [Fact(DisplayName = "Cuando se asigna la data de asignatura reconocimiento transversal Retorna Ok")]
        public void AssignAsignaturaReconocimientoTransversal_Ok()
        {
            //ARRANGE
            var sut = new GetReconocimientosQueryHandler(null, null, null, null);

            var reconocimiento = new ReconocimientoCommonGestorModel
            {
                AsignaturaExterna = Guid.NewGuid().ToString(),
                EstudioExterno = Guid.NewGuid().ToString(),
                EctsExterna = 10,
                TipoAsignaturaExternaDescripcion = Guid.NewGuid().ToString()
            };

            //ACT
            var actual = sut.AssignAsignaturaReconocimientoTransversal(reconocimiento);

            //ASSERT
            Assert.NotNull(actual);
            Assert.Equal($"{reconocimiento.AsignaturaExterna} ({reconocimiento.EstudioExterno})", actual.NombreAsignatura);
            Assert.Equal(reconocimiento.TipoAsignaturaExternaDescripcion, actual.NombreTipoAsignatura);
            Assert.Equal(reconocimiento.EctsExterna, actual.Ects);
            Assert.True(actual.EsTransversal);
            Assert.False(actual.EsTransversalPrincipal);
        }

        #endregion

        #region AssignAsignaturaReconocimiento

        [Fact(DisplayName = "Cuando se asigna la data de asignatura reconocimiento Retorna Ok")]
        public void AssignAsignaturaReconocimiento_Ok()
        {
            //ARRANGE
            var sutMock = new Mock<GetReconocimientosQueryHandler>(null, null, null, null) 
            { CallBase = true };

            var nombreAsignatura = Guid.NewGuid().ToString();
            sutMock.Setup(x => x.GetNombreAsignaturaReconocimiento(
                It.IsAny<string>(), It.IsAny<string>())).Returns(nombreAsignatura);

            var reconocimiento = new ReconocimientoCommonGestorModel
            {
                AsignaturaExterna = Guid.NewGuid().ToString(),
                EstudioExterno = Guid.NewGuid().ToString(),
                EctsExterna = 10,
                TipoAsignaturaExternaDescripcion = Guid.NewGuid().ToString()
            };
            var idAsignaturaPlanErp = 1;
            var calificacion = Guid.NewGuid().ToString();

            //ACT
            var actual = sutMock.Object.AssignAsignaturaReconocimiento(
                idAsignaturaPlanErp, calificacion, reconocimiento);

            //ASSERT
            Assert.NotNull(actual);
            Assert.Equal(idAsignaturaPlanErp, actual.IdAsignaturaPlanErp);
            Assert.Equal(calificacion, actual.CalificacionAsignaturaErp);
            Assert.Equal(nombreAsignatura, actual.NombreAsignatura);
            Assert.Equal(reconocimiento.TipoAsignaturaExternaDescripcion, actual.NombreTipoAsignatura);
            Assert.Equal(reconocimiento.EctsExterna, actual.Ects);
            Assert.True(actual.EsAsignatura);
            sutMock.Verify(x => x.GetNombreAsignaturaReconocimiento(
                It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        #endregion

        #region AssignAsignaturaReconocimientoSeminario

        [Fact(DisplayName = "Cuando se asigna la data de asignatura reconocimiento Seminario Retorna Ok")]
        public void AssignAsignaturaReconocimientoSeminario_Ok()
        {
            //ARRANGE
            var sut = new GetReconocimientosQueryHandler(null, null, null, null);

            var reconocimiento = new SeminarioGestorModel
            {
                NombreSeminario = Guid.NewGuid().ToString(),
                Ects = 10
            };
            var notaPorDefectoSeminario = CommonStrings.NotaPorDefectoSeminario;
            var tipoAsignaturaSeminario = CommonStrings.NombreTipoAsignaturaSeminario;

            //ACT
            var actual = sut.AssignAsignaturaReconocimientoSeminario(reconocimiento);

            //ASSERT
            Assert.NotNull(actual);
            Assert.Equal(reconocimiento.NombreSeminario, actual.NombreAsignatura);
            Assert.Equal(notaPorDefectoSeminario, actual.Calificacion);
            Assert.Equal(tipoAsignaturaSeminario, actual.NombreTipoAsignatura);
            Assert.Equal(reconocimiento.Ects, actual.Ects);
            Assert.True(actual.EsSeminario);
        }

        #endregion

        #region AssignAsignaturaReconocimientoUniversitaria

        [Fact(DisplayName = "Cuando se asigna la data de asignatura reconocimiento extensión universitaria Retorna Ok")]
        public void AssignAsignaturaReconocimientoUniversitaria_Ok()
        {
            //ARRANGE
            var tipoAsignaturaUniversitaria = CommonStrings.NombreTipoAsignaturaUniversitaria;
            var sut = new GetReconocimientosQueryHandler(null, null, null, null);

            var reconocimiento = new ReconocimientoCommonGestorModel
            {
                AsignaturaExterna = Guid.NewGuid().ToString(),
                EctsExterna = 10
            };

            //ACT
            var actual = sut.AssignAsignaturaReconocimientoUniversitaria(reconocimiento);

            //ASSERT
            Assert.NotNull(actual);
            Assert.Equal(reconocimiento.AsignaturaExterna, actual.NombreAsignatura);
            Assert.Equal(reconocimiento.EctsExterna, actual.Ects);
            Assert.Equal(tipoAsignaturaUniversitaria, actual.NombreTipoAsignatura);
            Assert.True(actual.EsExtensionUniversitaria);
        }

        #endregion
    }
}
