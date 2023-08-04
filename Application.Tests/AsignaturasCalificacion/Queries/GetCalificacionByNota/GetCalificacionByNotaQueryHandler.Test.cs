using Moq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Unir.Expedientes.Application.AsignaturasCalificacion.Queries.GetCalificacionByNota;
using Unir.Expedientes.Application.Common.Interfaces;
using Unir.Expedientes.Application.Common.Models.Evaluaciones;
using Unir.Expedientes.Application.Tests.Common;
using Xunit;
namespace Unir.Expedientes.Application.Tests.AsignaturasCalificacion.Queries.GetCalificacionByNota
{
    [Collection("CommonTestCollection")]
    public class GetCalificacionByNotaQueryHandlerTests : TestBase
    {
        #region Handle
        [Fact(DisplayName = "Cuando el servicio de configuración escala erp es nulo Retorna Nulo")]
        public async Task Handle_ConfiguracionEscala_Nulo()
        {
            //ARRANGE
            var request = new GetCalificacionByNotaQuery(1, 10);

            var mockEvaluacionServiceClient = new Mock<IEvaluacionesServiceClient> { CallBase = true };
            mockEvaluacionServiceClient.Setup(s => s.GetConfiguracionEscalaFromNivelesAsociadosEscalas(
                    It.IsAny<int>(), It.IsAny<int?>())).ReturnsAsync((ConfiguracionVersionEscalaModel)null);

            var sut = new GetCalificacionByNotaQueryHandler(mockEvaluacionServiceClient.Object);

            //ACT
            var act = await sut.Handle(request, CancellationToken.None);

            //ASSERT
            Assert.Null(act);
            mockEvaluacionServiceClient.Verify(s =>
                s.GetConfiguracionEscalaFromNivelesAsociadosEscalas(It.IsAny<int>(), It.IsAny<int?>()), Times.Once);
        }
        [Fact(DisplayName = "Cuando el servicio de configuración escala erp es nulo Retorna Nulo")]
        public async Task Handle_SinCalificacion_Nulo()
        {
            //ARRANGE
            var request = new GetCalificacionByNotaQuery(1, 2);

            var evaluacionEscala = new ConfiguracionVersionEscalaModel
            {
                Configuracion = new ConfiguracionModel
                {
                    Calificacion = new CalificacionModel
                    {
                        AfectaNotaNumerica = true,
                        AfectaPorcentaje = false,
                        Calificaciones = new List<CalificacionListModel>
                        {
                            new ()
                            {
                                NotaMinima = 5,
                                Nombre = "Este no",
                                EsNoPresentado = false,
                                Orden = 1
                            },
                            new ()
                            {
                                NotaMinima = 10,
                                Nombre = "Este si",
                                EsNoPresentado = false,
                                Orden = 2
                            },
                            new ()
                            {
                                NotaMinima = 15,
                                Nombre = "Este tampoco",
                                EsNoPresentado = false,
                                Orden = 3
                            }
                        }
                    }
                }
            };

            var mockEvaluacionServiceClient = new Mock<IEvaluacionesServiceClient> { CallBase = true };
            mockEvaluacionServiceClient.Setup(s => s.GetConfiguracionEscalaFromNivelesAsociadosEscalas(
                    It.IsAny<int>(), It.IsAny<int?>()))
                .ReturnsAsync(evaluacionEscala);

            var sut = new GetCalificacionByNotaQueryHandler(mockEvaluacionServiceClient.Object);

            //ACT
            var act = await sut.Handle(request, CancellationToken.None);

            //ASSERT
            Assert.Null(act);
            mockEvaluacionServiceClient.Verify(s =>
                s.GetConfiguracionEscalaFromNivelesAsociadosEscalas(It.IsAny<int>(), It.IsAny<int?>()), Times.Once);
        }

        [Fact(DisplayName = "Cuando se asigna una calificación por AfectaNotaNumerica Retorna Calificación")]
        public async Task Handle_AfectaNotaNumerica_Ok()
        {
            //ARRANGE
            var request = new GetCalificacionByNotaQuery(1, 10);

            var evaluacionEscala = new ConfiguracionVersionEscalaModel
            {
                Configuracion = new ConfiguracionModel
                {
                    Calificacion = new CalificacionModel
                    {
                        AfectaNotaNumerica = true,
                        AfectaPorcentaje = false,
                        Calificaciones = new List<CalificacionListModel>
                        {
                            new ()
                            {
                                NotaMinima = 5,
                                Nombre = "Este no",
                                EsNoPresentado = false,
                                Orden = 1
                            },
                            new ()
                            {
                                NotaMinima = 10,
                                Nombre = "Este si",
                                EsNoPresentado = false,
                                Orden = 2
                            },
                            new ()
                            {
                                NotaMinima = 15,
                                Nombre = "Este tampoco",
                                EsNoPresentado = false,
                                Orden = 3
                            }
                        }
                    }
                }
            };

            var mockEvaluacionServiceClient = new Mock<IEvaluacionesServiceClient> { CallBase = true };
            mockEvaluacionServiceClient.Setup(s => s.GetConfiguracionEscalaFromNivelesAsociadosEscalas(
                    It.IsAny<int>(), It.IsAny<int?>()))
                .ReturnsAsync(evaluacionEscala);
            var sut = new GetCalificacionByNotaQueryHandler(mockEvaluacionServiceClient.Object);

            //ACT
            var calificacion = await sut.Handle(request, CancellationToken.None);

            //ASSERT
            Assert.NotNull(calificacion);
            mockEvaluacionServiceClient.Verify(s =>
                s.GetConfiguracionEscalaFromNivelesAsociadosEscalas(It.IsAny<int>(), It.IsAny<int?>()), Times.Once);
        }
        [Fact(DisplayName = "Cuando se asigna una calificación por AfectaPorcentaje Retorna Calificación")]
        public async Task Handle_AfectaPorcentaje_Ok()
        {
            //ARRANGE
            var request = new GetCalificacionByNotaQuery(1, 95);

            var evaluacionEscala = new ConfiguracionVersionEscalaModel
            {
                Configuracion = new ConfiguracionModel
                {
                    Calificacion = new CalificacionModel
                    {
                        AfectaNotaNumerica = false,
                        AfectaPorcentaje = true,
                        Calificaciones = new List<CalificacionListModel>
                        {
                            new ()
                            {
                                PorcentajeMinimo = 10,
                                Nombre = "Este no",
                                EsNoPresentado = false,
                                Orden = 1
                            },
                            new ()
                            {
                                PorcentajeMinimo = 50,
                                Nombre = "Este tampoco",
                                EsNoPresentado = false,
                                Orden = 2
                            },
                            new ()
                            {
                                PorcentajeMinimo = 90,
                                Nombre = "Este sí",
                                EsNoPresentado = false,
                                Orden = 3
                            }
                        }
                    }
                }
            };

            var mockEvaluacionServiceClient = new Mock<IEvaluacionesServiceClient> { CallBase = true };
            mockEvaluacionServiceClient.Setup(s => s.GetConfiguracionEscalaFromNivelesAsociadosEscalas(
                    It.IsAny<int>(), It.IsAny<int?>())).ReturnsAsync(evaluacionEscala);
            var sut = new GetCalificacionByNotaQueryHandler(mockEvaluacionServiceClient.Object);

            //ACT
            var calificacion = await sut.Handle(request, CancellationToken.None);

            //ASSERT
            Assert.NotNull(calificacion);
            mockEvaluacionServiceClient.Verify(s =>
                s.GetConfiguracionEscalaFromNivelesAsociadosEscalas(It.IsAny<int>(), It.IsAny<int?>()), Times.Once);
        }

        #endregion
    }
}
