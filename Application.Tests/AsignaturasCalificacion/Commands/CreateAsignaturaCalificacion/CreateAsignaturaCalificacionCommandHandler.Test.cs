using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Moq;
using Unir.Expedientes.Application.AsignaturasCalificacion.Commands.CreateAsignaturaCalificacion;
using Unir.Expedientes.Application.AsignaturasExpediente.Commands.EditAsignaturaExpedienteToSeguimientoByNotaFinal;
using Unir.Expedientes.Application.Common.Models.ErpAcademico.Matriculacion;
using Unir.Expedientes.Application.Common.Models.ErpAcademico.OfertaEstudio;
using Unir.Expedientes.Application.Common.Models.ErpAcademico.PlanesEstudio;
using Unir.Expedientes.Application.Common.Models.Evaluaciones;
using Unir.Expedientes.Application.ExpedientesAlumnos.Queries.GetAllExpedientesAlumnos;
using Unir.Expedientes.Application.NotaFinalGenerada;
using Unir.Expedientes.Application.Tests.Common;
using Unir.Expedientes.Domain.Entities;
using Xunit;

namespace Unir.Expedientes.Application.Tests.AsignaturasCalificacion.Commands.CreateAsignaturaCalificacion
{
    [Collection("CommonTestCollection")]
    public class CreateAsignaturaCalificacionCommandHandlerTests : TestBase
    {
        const int bimestral = 2;
        const int trimestral = 3;
        const int cuatrimestral = 4;
        const int semestral = 6;
        const string anual = "1";
        #region CalculoConvocatoria
        [Fact(DisplayName = "Cuando calcula el número de la convocatoria, Retorna int Value")]
        public async Task CalculoConvocatoria_int()
        {
            //ARRANGE
            var mockIMediator = new Mock<IMediator>
            {
                CallBase = true
            };
            var sut = new Mock<CreateAsignaturaCalificacionCommandHandler>(mockIMediator.Object,
                Context)
            {
                CallBase = true
            };

            var asignaturaCalificacion = new AsignaturaCalificacion()
            {
                TipoConvocatoria = new TipoConvocatoria
                {
                    Id = 2
                },
                AsignaturaExpediente = new AsignaturaExpediente
                {
                    Id = 3
                }
            };
            await Context.AsignaturasCalificaciones.AddAsync(asignaturaCalificacion);
            await Context.SaveChangesAsync();

            var asignaturaExpediente = new AsignaturaExpedienteDto
            {
                Id = 3
            };
            var tipoConvocatoria = new TipoConvocatoria
            {
                Id = 2
            };

            //ACT
            var result = sut.Object.CalcularNumeroConvocatoria(asignaturaExpediente, tipoConvocatoria);

            //ASSERT
            Assert.Equal(2, result);

        }
        #endregion

        #region CalculoNombreCalificacion
        [Fact(DisplayName = "Cuando calcula el nombre de la calificación cuando no se ha presentado, Retorna string Value")]
        public async Task CalculoNombreCalificacionNoPresentado_string()
        {
            //ARRANGE
            var mockIMediator = new Mock<IMediator>
            {
                CallBase = true
            };
            var sut = new Mock<CreateAsignaturaCalificacionCommandHandler>(mockIMediator.Object,
                Context)
            {
                CallBase = true
            };

            var resultadoEsperado = new CalificacionListModel
            {
                EsNoPresentado = true,
                Nombre = "No Presentado"
            };

            var configuracionVersionEscala = new ConfiguracionVersionEscalaModel
            {
                Configuracion = new ConfiguracionModel
                {
                    Calificacion = new CalificacionModel
                    {
                        AfectaNotaNumerica = true,
                        Calificaciones = new List<CalificacionListModel>
                        {
                            resultadoEsperado,
                            new CalificacionListModel
                            {
                                EsNoPresentado = false,
                                NotaMinima = 0,
                                Nombre = "No Presentado"
                            },
                            new CalificacionListModel
                            {
                                EsNoPresentado = false,
                                NotaMinima = 5,
                                Nombre = "Aprobado"
                            },
                            new CalificacionListModel
                            {
                                EsNoPresentado = false,
                                NotaMinima = 6,
                                Nombre = "Bien"
                            },
                            new CalificacionListModel
                            {
                                EsNoPresentado = false,
                                NotaMinima = 7,
                                Nombre = "Notable"
                            },
                            new CalificacionListModel
                            {
                                EsNoPresentado = false,
                                NotaMinima = 9,
                                Nombre = "La leche"
                            }
                        }
                    }
                }
            };

            var nota = new NotaCommonStruct
            {
                NoPresentado = true
            };
            //ACT
            var result = sut.Object.GetNombreCalificacion(nota, configuracionVersionEscala);

            //ASSERT
            Assert.Equal(resultadoEsperado.Nombre, result);
        }
        [Fact(DisplayName = "Cuando calcula el nombre de la calificación cuando afecta a numérica, Retorna string Value")]
        public async Task CalculoNombreCalificacionCuandoAfectaNumerica_string()
        {
            //ARRANGE
            var mockIMediator = new Mock<IMediator>
            {
                CallBase = true
            };
            var sut = new Mock<CreateAsignaturaCalificacionCommandHandler>(mockIMediator.Object,
                Context)
            {
                CallBase = true
            };

            var resultadoEsperado = new CalificacionListModel
            {
                Orden = 5,
                EsNoPresentado = false,
                NotaMinima = 9,
                Nombre = "La leche"
            };

            var configuracionVersionEscala = new ConfiguracionVersionEscalaModel
            {
                Configuracion = new ConfiguracionModel
                {
                    Calificacion = new CalificacionModel
                    {
                        AfectaNotaNumerica = true,
                        Calificaciones = new List<CalificacionListModel>
                        {
                            new CalificacionListModel
                            {
                                Orden = 0,
                                EsNoPresentado = true,
                                Nombre = "No Presentado"
                            },
                            new CalificacionListModel
                            {
                                Orden = 1,
                                EsNoPresentado = false,
                                NotaMinima = 0,
                                Nombre = "No Presentado"
                            },
                            new CalificacionListModel
                            {
                                Orden = 2,
                                EsNoPresentado = false,
                                NotaMinima = 5,
                                Nombre = "Aprobado"
                            },
                            new CalificacionListModel
                            {
                                Orden = 3,
                                EsNoPresentado = false,
                                NotaMinima = 6,
                                Nombre = "Bien"
                            },
                            new CalificacionListModel
                            {
                                Orden = 4,
                                EsNoPresentado = false,
                                NotaMinima = 7,
                                Nombre = "Notable"
                            },
                            resultadoEsperado
                        }
                    }
                }
            };

            var nota = new NotaCommonStruct
            {
                NoPresentado = false,
                Calificacion = 9
            };
            //ACT
            var result = sut.Object.GetNombreCalificacion(nota, configuracionVersionEscala);

            //ASSERT
            Assert.Equal(resultadoEsperado.Nombre, result);
        }
        [Fact(DisplayName = "Cuando calcula el nombre de la calificación cuando afecta a porcentaje, Retorna string Value")]
        public async Task CalculoNombreCalificacionCuandoAfectaPorcentaje_string()
        {
            //ARRANGE
            var mockIMediator = new Mock<IMediator>
            {
                CallBase = true
            };
            var sut = new Mock<CreateAsignaturaCalificacionCommandHandler>(mockIMediator.Object,
                Context)
            {
                CallBase = true
            };

            var resultadoEsperado = new CalificacionListModel
            {
                Orden = 5,
                EsNoPresentado = false,
                PorcentajeMinimo = 90,
                Nombre = "La leche"
            };

            var configuracionVersionEscala = new ConfiguracionVersionEscalaModel
            {
                Configuracion = new ConfiguracionModel
                {
                    Calificacion = new CalificacionModel
                    {
                        AfectaPorcentaje = true,
                        Calificaciones = new List<CalificacionListModel>
                        {
                            new CalificacionListModel
                            {
                                Orden = 0,
                                EsNoPresentado = true,
                                Nombre = "No Presentado"
                            },
                            new CalificacionListModel
                            {
                                Orden = 1,
                                EsNoPresentado = false,
                                PorcentajeMinimo = 0,
                                Nombre = "No Presentado"
                            },
                            new CalificacionListModel
                            {
                                Orden = 2,
                                EsNoPresentado = false,
                                PorcentajeMinimo = 50,
                                Nombre = "Aprobado"
                            },
                            new CalificacionListModel
                            {
                                Orden = 3,
                                EsNoPresentado = false,
                                PorcentajeMinimo = 60,
                                Nombre = "Bien"
                            },
                            new CalificacionListModel
                            {
                                Orden = 4,
                                EsNoPresentado = false,
                                PorcentajeMinimo = 70,
                                Nombre = "Notable"
                            },
                            resultadoEsperado
                        }
                    }
                }
            };

            var nota = new NotaCommonStruct
            {
                NoPresentado = false,
                Calificacion = 90
            };
            //ACT
            var result = sut.Object.GetNombreCalificacion(nota, configuracionVersionEscala);

            //ASSERT
            Assert.Equal(resultadoEsperado.Nombre, result);
        }
        #endregion

        #region CalculoCiclo
        [Fact(DisplayName = "Cuando calcula el ciclo pero viene sin fecha, Retorna string empty Value")]
        public async Task CalculoCicloSinFecha_string()
        {
            //ARRANGE
            var mockIMediator = new Mock<IMediator>
            {
                CallBase = true
            };
            var sut = new Mock<CreateAsignaturaCalificacionCommandHandler>(mockIMediator.Object,
                Context)
            {
                CallBase = true
            };

            //ACT
            var result = sut.Object.CalculoCiclo(null, "M");

            //ASSERT
            Assert.Equal("", result);
        }
        [Fact(DisplayName = "Cuando calcula el ciclo Mensual, Retorna string Value")]
        public async Task CalculoCicloMesual_string()
        {
            //ARRANGE
            var mockIMediator = new Mock<IMediator>
            {
                CallBase = true
            };
            var sut = new Mock<CreateAsignaturaCalificacionCommandHandler>(mockIMediator.Object,
                Context)
            {
                CallBase = true
            };

            //ACT
            var result = sut.Object.CalculoCiclo(DateTime.Now, "M");

            //ASSERT
            Assert.Equal(DateTime.Now.Month.ToString(), result);
        }
        [Fact(DisplayName = "Cuando calcula el ciclo Bimestral, Retorna string Value")]
        public async Task CalculoCicloBimestral_string()
        {
            //ARRANGE
            var mockIMediator = new Mock<IMediator>
            {
                CallBase = true
            };
            var sut = new Mock<CreateAsignaturaCalificacionCommandHandler>(mockIMediator.Object,
                Context)
            {
                CallBase = true
            };

            //ACT
            var result = sut.Object.CalculoCiclo(DateTime.Now, "B");

            //ASSERT
            Assert.Equal(
                DateTime.Now.Month % bimestral > 0
                    ? Convert.ToInt32(DateTime.Now.Month / bimestral + 1).ToString()
                    : Convert.ToInt32(DateTime.Now.Month / bimestral).ToString(), result);
        }
        [Fact(DisplayName = "Cuando calcula el ciclo Trimestral, Retorna string Value")]
        public async Task CalculoCicloTrimestral_string()
        {
            //ARRANGE
            var mockIMediator = new Mock<IMediator>
            {
                CallBase = true
            };
            var sut = new Mock<CreateAsignaturaCalificacionCommandHandler>(mockIMediator.Object,
                Context)
            {
                CallBase = true
            };

            //ACT
            var result = sut.Object.CalculoCiclo(DateTime.Now, "T");

            //ASSERT
            Assert.Equal(
                DateTime.Now.Month % trimestral > 0
                    ? Convert.ToInt32(DateTime.Now.Month / trimestral + 1).ToString()
                    : Convert.ToInt32(DateTime.Now.Month / trimestral).ToString(), result);
        }
        [Fact(DisplayName = "Cuando calcula el ciclo Cuatrimestral, Retorna string Value")]
        public async Task CalculoCicloCuatrimestral_string()
        {
            //ARRANGE
            var mockIMediator = new Mock<IMediator>
            {
                CallBase = true
            };
            var sut = new Mock<CreateAsignaturaCalificacionCommandHandler>(mockIMediator.Object,
                Context)
            {
                CallBase = true
            };

            //ACT
            var result = sut.Object.CalculoCiclo(DateTime.Now, "C");

            //ASSERT
            Assert.Equal(
                DateTime.Now.Month % cuatrimestral > 0
                    ? Convert.ToInt32(DateTime.Now.Month / cuatrimestral + 1).ToString()
                    : Convert.ToInt32(DateTime.Now.Month / cuatrimestral).ToString(), result);
        }
        [Fact(DisplayName = "Cuando calcula el ciclo Semestral, Retorna string Value")]
        public async Task CalculoCicloSemestral_string()
        {
            //ARRANGE
            var mockIMediator = new Mock<IMediator>
            {
                CallBase = true
            };
            var sut = new Mock<CreateAsignaturaCalificacionCommandHandler>(mockIMediator.Object,
                Context)
            {
                CallBase = true
            };

            //ACT
            var result = sut.Object.CalculoCiclo(DateTime.Now, "S");

            //ASSERT
            Assert.Equal(
                DateTime.Now.Month % semestral > 0
                    ? Convert.ToInt32(DateTime.Now.Month / semestral + 1).ToString()
                    : Convert.ToInt32(DateTime.Now.Month / semestral).ToString(), result);
        }
        [Fact(DisplayName = "Cuando calcula el ciclo Anual, Retorna string Value")]
        public async Task CalculoCicloAnual_string()
        {
            //ARRANGE
            var mockIMediator = new Mock<IMediator>
            {
                CallBase = true
            };
            var sut = new Mock<CreateAsignaturaCalificacionCommandHandler>(mockIMediator.Object,
                Context)
            {
                CallBase = true
            };

            //ACT
            var result = sut.Object.CalculoCiclo(DateTime.Now, "A");

            //ASSERT
            Assert.Equal(anual, result);
        }
        [Fact(DisplayName = "Cuando calcula el ciclo por Defecto, Retorna string Value")]
        public async Task CalculoCicloPorDefecto_string()
        {
            //ARRANGE
            var mockIMediator = new Mock<IMediator>
            {
                CallBase = true
            };
            var sut = new Mock<CreateAsignaturaCalificacionCommandHandler>(mockIMediator.Object,
                Context)
            {
                CallBase = true
            };

            //ACT
            var result = sut.Object.CalculoCiclo(DateTime.Now, "Z");

            //ASSERT
            Assert.Equal(DateTime.Now.Month.ToString(), result);
        }
        #endregion

        #region AssignAsignaturaCalificacion
        [Fact(DisplayName = "Cuando se Asigna una Asignatura Calificación, Retorna void")]
        public async Task AssignAsignaturaCalificacion_void()
        {
            //ARRANGE
            var mockIMediator = new Mock<IMediator>
            {
                CallBase = true
            };
            var sut = new Mock<CreateAsignaturaCalificacionCommandHandler>(mockIMediator.Object,
                Context)
            {
                CallBase = true
            };
            var nombreCalificacion = "La leche";
            var convocatoria = 2;
            sut.Setup(ca => ca.CalcularNumeroConvocatoria(It.IsAny<AsignaturaExpedienteDto>(), It.IsAny<TipoConvocatoria>()))
                .Returns(convocatoria);

            sut.Setup(ca => ca.CalculoCiclo(It.IsAny<DateTime>(), It.IsAny<string>())).Returns("6");
            sut.Setup(ca =>
                ca.GetNombreCalificacion(It.IsAny<NotaCommonStruct>(),
                    It.IsAny<ConfiguracionVersionEscalaModel>())).Returns(nombreCalificacion);
            var notaFinal = new NotaFinalGeneradaCommand
            {
                Plataforma = "Lol",
                IdUsuarioPublicadorConfirmador = 2,
                Provisional = false
            };
            var nota = new NotaCommonStruct
            {
                FechaPublicado = DateTime.Now,
                FechaConfirmado = DateTime.Now.AddDays(10),
                Calificacion = 9,
                EsMatriculaHonor = true,
                NoPresentado = false
            };
            var asignaturaMatriculada = new AsignaturaMatriculadaModel
            {
                Id = 3,
                IdRefCurso = "2023",
                AsignaturaOfertada = new AsignaturaOfertadaModel
                {
                    Id = 4,
                    PeriodoLectivo = new PeriodoLectivoModel
                    {
                        Id = 2,
                        DuracionPeriodoLectivo = new DuracionPeriodoLectivoErpAcademicoModel
                        {
                            Simbolo = "M"
                        },
                        PeriodoAcademico = new PeriodoAcademicoModel
                        {
                            AnyoAcademico = new AnyoAcademicoModel
                            {
                                AnyoInicio = 2022,
                                AnyoFin = 2023
                            }
                        }
                    }
                }
            };
            var asignaturaExpediente = new AsignaturaExpedienteDto { Id = 5 };
            var configuracionVersionEscala = new ConfiguracionVersionEscalaModel
            {
                Configuracion = new ConfiguracionModel
                {
                    Calificacion = new CalificacionModel
                    {
                        NotaMinAprobado = 5
                    }
                }
            };
            var newAsignaturaCalificacion = new AsignaturaCalificacion();
            var tipoConvocatoria = new TipoConvocatoria
            {
                Id = 6
            };


            //ACT
            sut.Object.AssignAsignaturaCalificacion(nota, asignaturaMatriculada, asignaturaExpediente,
                configuracionVersionEscala, newAsignaturaCalificacion, tipoConvocatoria, notaFinal,
                CancellationToken.None);

            //ASSERT
            Assert.Equal(nota.FechaConfirmado, newAsignaturaCalificacion.FechaConfirmado);
            Assert.Equal(nota.FechaPublicado, newAsignaturaCalificacion.FechaPublicado);
            Assert.Equal(nota.FechaPublicado?.Year.ToString() + "-" + 6, newAsignaturaCalificacion.FechaPublicado?.Year.ToString() + "-" + 6);
            Assert.Equal(nombreCalificacion, newAsignaturaCalificacion.NombreCalificacion);
            Assert.Equal(convocatoria, newAsignaturaCalificacion.Convocatoria);
            sut.Verify(ca => ca.CalcularNumeroConvocatoria(It.IsAny<AsignaturaExpedienteDto>(), It.IsAny<TipoConvocatoria>()),
                Times.Once);
            sut.Verify(ca => ca.CalculoCiclo(It.IsAny<DateTime>(), It.IsAny<string>()), Times.Once);
            sut.Verify(ca =>
                ca.GetNombreCalificacion(It.IsAny<NotaCommonStruct>(),
                    It.IsAny<ConfiguracionVersionEscalaModel>()), Times.Once);
        }
        #endregion

        #region Handler
        [Fact(DisplayName = "Cuando se crea una asignatura calificación, Retorna Unit Value")]
        public async Task Handle_OK()
        {
            //ARRANGE
            var mockIMediator = new Mock<IMediator>
            {
                CallBase = true
            };
            var sut = new Mock<CreateAsignaturaCalificacionCommandHandler>(mockIMediator.Object,
                Context)
            {
                CallBase = true
            };

            var request = new CreateAsignaturaCalificacionCommand
            {
                notaFinal = new NotaFinalGeneradaCommand
                {
                    Notas = new List<NotaCommonStruct>
                    {
                        new NotaCommonStruct
                        {
                            IdAlumno = 8,
                            FechaPublicado = DateTime.Now,
                            FechaConfirmado = DateTime.Now.AddDays(10),
                            Calificacion = 9,
                            Convocatoria = "1",
                            EsMatriculaHonor = true,
                            NoPresentado = false
                        },
                        new NotaCommonStruct
                        {
                            IdAlumno = 9,
                            FechaPublicado = DateTime.Now,
                            FechaConfirmado = DateTime.Now.AddDays(10),
                            Calificacion = 8,
                            Convocatoria = "2",
                            EsMatriculaHonor = true,
                            NoPresentado = false
                        },
                        new NotaCommonStruct
                        {
                            IdAlumno = 10,
                            FechaPublicado = DateTime.Now,
                            FechaConfirmado = DateTime.Now.AddDays(10),
                            Calificacion = 7,
                            Convocatoria = "3",
                            EsMatriculaHonor = true,
                            NoPresentado = false
                        },
                        new NotaCommonStruct
                        {
                            IdAlumno = 11,
                            FechaPublicado = DateTime.Now,
                            FechaConfirmado = DateTime.Now.AddDays(10),
                            Calificacion = 6,
                            Convocatoria = "4",
                            EsMatriculaHonor = true,
                            NoPresentado = false
                        },
                        new NotaCommonStruct
                        {
                            IdAlumno = 12,
                            FechaPublicado = DateTime.Now,
                            FechaConfirmado = DateTime.Now.AddDays(10),
                            Calificacion = 4,
                            Convocatoria = "5",
                            EsMatriculaHonor = true,
                            NoPresentado = false
                        }
                    }
                },
                asignaturasMatriculadas = new List<AsignaturaMatriculadaModel>
                {
                    new AsignaturaMatriculadaModel
                    {
                        Matricula = new MatriculaModel
                        {
                            Alumno = new AlumnoModel
                            {
                                IdIntegracion = "9"
                            }
                        },
                        AsignaturaOfertada = new AsignaturaOfertadaModel
                        {
                            AsignaturaPlan = new AsignaturaPlanErpAcademicoModel
                            {
                                Id = 12
                            }
                        }
                    },
                    new AsignaturaMatriculadaModel
                    {
                        Matricula = new MatriculaModel
                        {
                            Alumno = new AlumnoModel
                            {
                                IdIntegracion = "10"
                            }
                        },
                        AsignaturaOfertada = new AsignaturaOfertadaModel
                        {
                            AsignaturaPlan = new AsignaturaPlanErpAcademicoModel
                            {
                                Id = 11
                            }
                        }
                    },
                    new AsignaturaMatriculadaModel
                    {

                        Matricula = new MatriculaModel
                        {
                            Alumno = new AlumnoModel
                            {
                                IdIntegracion = "11"
                            }
                        },
                        AsignaturaOfertada = new AsignaturaOfertadaModel
                        {
                            AsignaturaPlan = new AsignaturaPlanErpAcademicoModel
                            {
                                Id = 14
                            }
                        }
                    },
                    new AsignaturaMatriculadaModel
                    {
                        ConfiguracionVersionEscala = new ConfiguracionVersionEscalaModel(),
                        Matricula = new MatriculaModel
                        {
                            Alumno = new AlumnoModel
                            {
                                IdIntegracion = "12"
                            }
                        },
                        AsignaturaOfertada = new AsignaturaOfertadaModel
                        {
                            AsignaturaPlan = new AsignaturaPlanErpAcademicoModel
                            {
                                Id = 15
                            }
                        }
                    }
                },
                Expedientes = new List<ExpedienteAlumnoListItemDto>
                {
                    new ExpedienteAlumnoListItemDto
                    {
                        IdRefIntegracionAlumno = "10",
                        AsignaturasExpedientes = new List<AsignaturaExpedienteDto>
                        {
                            new AsignaturaExpedienteDto
                            {
                                IdRefAsignaturaPlan = "13"
                            }
                        }
                    },
                    new ExpedienteAlumnoListItemDto
                    {
                        IdRefIntegracionAlumno = "11"
                        ,
                        AsignaturasExpedientes = new List<AsignaturaExpedienteDto>
                        {
                            new AsignaturaExpedienteDto
                            {
                                IdRefAsignaturaPlan = "14"
                            }
                        }
                    },
                    new ExpedienteAlumnoListItemDto
                    {
                        IdRefIntegracionAlumno = "12",
                        AsignaturasExpedientes = new List<AsignaturaExpedienteDto>
                        {
                            new AsignaturaExpedienteDto
                            {
                                IdRefAsignaturaPlan = "15"
                            }
                        }
                    },
                }
            };

            sut.Setup(h => h.AssignAsignaturaCalificacion(It.IsAny<NotaCommonStruct>(),
                It.IsAny<AsignaturaMatriculadaModel>(), It.IsAny<AsignaturaExpedienteDto>(),
                It.IsAny<ConfiguracionVersionEscalaModel>(), It.IsAny<AsignaturaCalificacion>(),
                It.IsAny<TipoConvocatoria>(), It.IsAny<NotaFinalGeneradaCommand>(),
                It.IsAny<CancellationToken>()));

            mockIMediator.Setup(me => me.Send(It.IsAny<EditAsignaturaExpedienteToSeguimientoByNotaFinalCommand>(),
                It.IsAny<CancellationToken>()));

            //ACT
            var result = await sut.Object.Handle(request, CancellationToken.None);

            //ASSERT
            var grabado = Context.AsignaturasCalificaciones.ToList();
            Assert.NotEmpty(grabado);
            Assert.Single(grabado);
            sut.Verify(h => h.AssignAsignaturaCalificacion(It.IsAny<NotaCommonStruct>(),
                It.IsAny<AsignaturaMatriculadaModel>(), It.IsAny<AsignaturaExpedienteDto>(),
                It.IsAny<ConfiguracionVersionEscalaModel>(), It.IsAny<AsignaturaCalificacion>(),
                It.IsAny<TipoConvocatoria>(), It.IsAny<NotaFinalGeneradaCommand>(),
                It.IsAny<CancellationToken>()), Times.Once);

            mockIMediator.Verify(me => me.Send(It.IsAny<EditAsignaturaExpedienteToSeguimientoByNotaFinalCommand>(),
                It.IsAny<CancellationToken>()), Times.Once);
        }
        #endregion
    }
}
