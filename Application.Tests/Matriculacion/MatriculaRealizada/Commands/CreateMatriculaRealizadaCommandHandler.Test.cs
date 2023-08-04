using MediatR;
using Microsoft.Extensions.Localization;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Unir.Expedientes.Application.Common.Interfaces;
using Unir.Expedientes.Application.Common.Models.ErpAcademico.Global;
using Unir.Expedientes.Application.Common.Models.ErpAcademico.Matriculacion;
using Unir.Expedientes.Application.Common.Models.ErpAcademico.OfertaEstudio;
using Unir.Expedientes.Application.Common.Models.ErpAcademico.PlanesEstudio;
using Unir.Expedientes.Application.ConsolidacionesRequisitosExpedientes.Commands.AddConsolidacionesRequisitosExpedienteUncommit;
using Unir.Expedientes.Application.ExpedientesAlumnos.Queries.HasPrimeraMatriculaExpediente;
using Unir.Expedientes.Application.Matriculacion.Common.Queries.GetAlumnoByIdIntegracion;
using Unir.Expedientes.Application.Matriculacion.Common.Queries.GetAsignaturasAsociadasExpediente;
using Unir.Expedientes.Application.Matriculacion.MatriculaRealizada.Commands;
using Unir.Expedientes.Application.Tests.Common;
using Unir.Expedientes.Domain.Entities;
using Xunit;

namespace Unir.Expedientes.Application.Tests.Matriculacion.MatriculaRealizada.Commands
{
    [Collection("CommonTestCollection")]
    public class CreateMatriculaRealizadaCommandHandlerTest : TestBase
    {
        #region Handle

        [Fact(DisplayName = "Cuando es primera matrícula Retorna Ok")]
        public async Task Handle_EsPrimeraMatricula()
        {
            //ARRANGE
            var request = new CreateMatriculaRealizadaCommand
            {
                AlumnoIdIntegracion = "1",
                MatriculaIdIntegracion = "1",
                FechaRecepcion = DateTime.Now.Date,
                Mensaje = Guid.NewGuid().ToString(),
                Origen = Guid.NewGuid().ToString()
            };

            var mockIStringLocalizer = new Mock<IStringLocalizer<CreateMatriculaRealizadaCommandHandler>> { CallBase = true };
            var mockIMediator = new Mock<IMediator> { CallBase = true };
            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient> { CallBase = true };

            var sutMock = new Mock<CreateMatriculaRealizadaCommandHandler>(Context, mockIStringLocalizer.Object, mockIMediator.Object, mockIErpAcademicoServiceClient.Object)
            { CallBase = true };

            var mockExpedienteAlumno = new Mock<ExpedienteAlumno> { CallBase = true };
            mockExpedienteAlumno.SetupAllProperties();
            mockExpedienteAlumno.Setup(s => s.AddHitosConseguidos(It.IsAny<TipoHitoConseguido>(), It.IsAny<DateTime>(), It.IsAny<ExpedienteEspecializacion>()));
            mockExpedienteAlumno.Setup(s => s.AddSeguimientoNoUser(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DateTime>(),
                    It.IsAny<string>(), It.IsAny<string>()));

            var alumnoAcademico = new AlumnoAcademicoModel
            {
                Id = 1,
                Matriculas = new List<MatriculaAcademicoModel>()
                {
                    new () {
                        Id = 1,
                        IdIntegracion = "1",
                        Estado = new EstadoMatriculaAcademicoModel
                        {
                            EsAlta = true
                        },
                        IdRefExpedienteAlumno = "1"
                    },
                    new () {
                        Id = 2,
                        IdIntegracion = "2",
                        Estado = new EstadoMatriculaAcademicoModel
                        {
                            EsAlta = false
                        },
                        IdRefExpedienteAlumno = "2"
                    }
                }
            };

            var listaAsignaturasMatriculadas = new List<AsignaturaMatriculadaModel>();
            var asignaturaMatriculada = new AsignaturaMatriculadaModel
            {
                Id = 1,
                AsignaturaOfertada = new AsignaturaOfertadaModel
                {
                    Id = 1,
                    AsignaturaPlan = new AsignaturaPlanErpAcademicoModel
                    {
                        Id = 1,
                        DisplayName = Guid.NewGuid().ToString(),
                        Asignatura = new AsignaturaErpAcademicoModel
                        {
                            Id = 1,
                            Nombre = Guid.NewGuid().ToString(),
                            Codigo = Guid.NewGuid().ToString(),
                            IdiomaImparticion = new IdiomaAcademicoModel
                            {
                                Id = 1,
                                Nombre = Guid.NewGuid().ToString(),
                                Siglas = Guid.NewGuid().ToString(),
                            }
                        },
                        Orden = 1
                    },
                    TipoAsignatura = new TipoAsignaturaErpAcademicoModel
                    {
                        Id = 1,
                        Nombre = Guid.NewGuid().ToString(),
                        Orden = 1
                    },
                    PeriodoLectivo = new PeriodoLectivoModel
                    {
                        Id = 1,
                        Nombre = Guid.NewGuid().ToString(),
                        PeriodoAcademico = new PeriodoAcademicoModel
                        {
                            Id = 1,
                            Nombre = Guid.NewGuid().ToString(),
                            AnyoAcademico = new AnyoAcademicoModel
                            {
                                AnyoInicio = 2020,
                                AnyoFin = 2021
                            }
                        },
                        DuracionPeriodoLectivo = new DuracionPeriodoLectivoErpAcademicoModel
                        {
                            Nombre = Guid.NewGuid().ToString(),
                            Simbolo = Guid.NewGuid().ToString()
                        }
                    }
                }
            };
            listaAsignaturasMatriculadas.Add(asignaturaMatriculada);

            var alumnoMatricula = new AlumnoMatricula
            {
                ExpedienteAlumno = mockExpedienteAlumno.Object,
                AlumnoAcademicoModel = alumnoAcademico,
                MatriculaAcademicoModel = alumnoAcademico.Matriculas.First()
            };

            mockIMediator.Setup(s => s.Send(It.IsAny<GetAlumnoByIdIntegracionQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(alumnoMatricula);

            mockIMediator.Setup(s => s.Send(It.IsAny<GetAsignaturasAsociadasQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(mockExpedienteAlumno.Object.AsignaturasExpedientes);

            mockIMediator.Setup(s => s.Send(It.IsAny<HasPrimeraMatriculaExpedienteQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            mockIMediator.Setup(s => s.Send(It.IsAny<AddConsolidacionesRequisitosExpedienteUncommitCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Unit.Value);

            mockIErpAcademicoServiceClient.Setup(x => x.GetAsignaturasMatricula(It.IsAny<int>()))
                .ReturnsAsync(listaAsignaturasMatriculadas);

            var situacionAsignatura = new SituacionAsignatura
            {
                Id = 1,
                Nombre = "Matriculada"
            };
            await Context.SituacionesAsignaturas.AddAsync(situacionAsignatura);

            var tiposHitos = new TipoHitoConseguido()
            {
                Id = 2,
                Nombre = "Primera matrícula",
                Icono = Guid.NewGuid().ToString()
            };
            await Context.TiposHitoConseguidos.AddAsync(tiposHitos);

            mockExpedienteAlumno.Object.Id = 1;
            mockExpedienteAlumno.Object.IdRefIntegracionAlumno = Guid.NewGuid().ToString();
            mockExpedienteAlumno.Object.IdRefPlan = Guid.NewGuid().ToString();
            mockExpedienteAlumno.Object.AsignaturasExpedientes = new List<AsignaturaExpediente>
            {
                new()
                {
                    Id = 1,
                    IdRefAsignaturaPlan = "1",
                    NombreAsignatura = Guid.NewGuid().ToString(),
                    CodigoAsignatura = Guid.NewGuid().ToString(),
                    OrdenAsignatura = 1,
                    Ects = 0,
                    IdRefTipoAsignatura = Guid.NewGuid().ToString(),
                    SimboloTipoAsignatura = Guid.NewGuid().ToString(),
                    OrdenTipoAsignatura = 1,
                    NombreTipoAsignatura = Guid.NewGuid().ToString(),
                    IdRefCurso = Guid.NewGuid().ToString(),
                    NumeroCurso = 1,
                    AnyoAcademicoInicio = 2020,
                    AnyoAcademicoFin = 2022,
                    PeriodoLectivo = Guid.NewGuid().ToString(),
                    DuracionPeriodo = Guid.NewGuid().ToString(),
                    SimboloDuracionPeriodo = Guid.NewGuid().ToString(),
                    IdRefIdiomaImparticion = Guid.NewGuid().ToString(),
                    SimboloIdiomaImparticion = Guid.NewGuid().ToString(),
                    Reconocida = false,
                    SituacionAsignatura = situacionAsignatura
                }
            };

            await Context.ExpedientesAlumno.AddAsync(mockExpedienteAlumno.Object);
            await Context.SaveChangesAsync();

            //ACT
            var actual = await sutMock.Object.Handle(request, CancellationToken.None);

            //ASSERT
            Assert.IsType<Unit>(actual);
            mockIMediator.Verify(s => s.Send(It.IsAny<GetAlumnoByIdIntegracionQuery>(), It.IsAny<CancellationToken>()), Times.Once);
            mockIMediator.Verify(s => s.Send(It.IsAny<HasPrimeraMatriculaExpedienteQuery>(), It.IsAny<CancellationToken>()), Times.Once);
            mockIMediator.Verify(s => s.Send(It.IsAny<AddConsolidacionesRequisitosExpedienteUncommitCommand>(), It.IsAny<CancellationToken>()), Times.Once);
            mockIErpAcademicoServiceClient.Verify(x => x.GetAsignaturasMatricula(It.IsAny<int>()), Times.Once);
            mockExpedienteAlumno.Verify(s => s.AddHitosConseguidos(It.IsAny<TipoHitoConseguido>(), It.IsAny<DateTime>(), It.IsAny<ExpedienteEspecializacion>()), Times.Once);
            mockExpedienteAlumno.Verify(s => s.AddSeguimientoNoUser(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DateTime>(),
                    It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            mockIMediator.Verify(s => s.Send(It.IsAny<GetAsignaturasAsociadasQuery>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact(DisplayName = "Cuando no es primera matrícula Retorna Ok")]
        public async Task Handle_NoEsPrimeraMatricula()
        {
            //ARRANGE
            var request = new CreateMatriculaRealizadaCommand
            {
                AlumnoIdIntegracion = "1",
                MatriculaIdIntegracion = "1",
                FechaRecepcion = DateTime.Now.Date,
                Mensaje = Guid.NewGuid().ToString(),
                Origen = Guid.NewGuid().ToString()
            };

            var mockIStringLocalizer = new Mock<IStringLocalizer<CreateMatriculaRealizadaCommandHandler>> { CallBase = true };
            var mockIMediator = new Mock<IMediator> { CallBase = true };
            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient> { CallBase = true };

            var mockExpedienteAlumno = new Mock<ExpedienteAlumno> { CallBase = true };
            mockExpedienteAlumno.SetupAllProperties();
            mockExpedienteAlumno.Setup(s => s.AddSeguimientoNoUser(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DateTime>(),
                    It.IsAny<string>(), It.IsAny<string>()));

            var sutMock = new Mock<CreateMatriculaRealizadaCommandHandler>(Context, mockIStringLocalizer.Object, mockIMediator.Object, mockIErpAcademicoServiceClient.Object)
            { CallBase = true };

            mockIMediator.Setup(s => s.Send(It.IsAny<HasPrimeraMatriculaExpedienteQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            mockIMediator.Setup(s => s.Send(It.IsAny<AddConsolidacionesRequisitosExpedienteUncommitCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Unit.Value);

            var alumnoAcademico = new AlumnoAcademicoModel
            {
                Id = 1,
                Matriculas = new List<MatriculaAcademicoModel>()
                {
                    new() {
                      Id = 1,
                      IdIntegracion = "1",
                      Estado = new EstadoMatriculaAcademicoModel
                      {
                          EsAlta = false
                      },
                      IdRefExpedienteAlumno = "1"
                    },
                    new () {
                      Id = 2,
                      IdIntegracion = "2",
                      Estado = new EstadoMatriculaAcademicoModel
                      {
                          EsAlta = false
                      },
                      IdRefExpedienteAlumno = "2"
                    }
                }
            };

            var listaAsignaturasMatriculadas = new List<AsignaturaMatriculadaModel>();
            var asignaturaMatriculada = new AsignaturaMatriculadaModel
            {
                Id = 1,
                AsignaturaOfertada = new AsignaturaOfertadaModel
                {
                    Id = 1,
                    AsignaturaPlan = new AsignaturaPlanErpAcademicoModel
                    {
                        Id = 1,
                        DisplayName = Guid.NewGuid().ToString(),
                        Asignatura = new AsignaturaErpAcademicoModel
                        {
                            Id = 1,
                            Nombre = Guid.NewGuid().ToString(),
                            Codigo = Guid.NewGuid().ToString(),
                            IdiomaImparticion = new IdiomaAcademicoModel
                            {
                                Id = 1,
                                Nombre = Guid.NewGuid().ToString(),
                                Siglas = Guid.NewGuid().ToString(),
                            }
                        },
                        Orden = 1
                    },
                    TipoAsignatura = new TipoAsignaturaErpAcademicoModel
                    {
                        Id = 1,
                        Nombre = Guid.NewGuid().ToString(),
                        Orden = 1
                    },
                    PeriodoLectivo = new PeriodoLectivoModel
                    {
                        Id = 1,
                        Nombre = Guid.NewGuid().ToString(),
                        PeriodoAcademico = new PeriodoAcademicoModel
                        {
                            Id = 1,
                            Nombre = Guid.NewGuid().ToString(),
                            AnyoAcademico = new AnyoAcademicoModel
                            {
                                AnyoInicio = 2020,
                                AnyoFin = 2021
                            }
                        },
                        DuracionPeriodoLectivo = new DuracionPeriodoLectivoErpAcademicoModel
                        {
                            Nombre = Guid.NewGuid().ToString(),
                            Simbolo = Guid.NewGuid().ToString()
                        }
                    }
                }
            };

            var alumnoMatricula = new AlumnoMatricula
            {
                ExpedienteAlumno = mockExpedienteAlumno.Object,
                AlumnoAcademicoModel = alumnoAcademico,
                MatriculaAcademicoModel = alumnoAcademico.Matriculas.First()
            };

            mockIMediator.Setup(s => s.Send(It.IsAny<GetAlumnoByIdIntegracionQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(alumnoMatricula);

            mockIMediator.Setup(s => s.Send(It.IsAny<GetAsignaturasAsociadasQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(mockExpedienteAlumno.Object.AsignaturasExpedientes);

            listaAsignaturasMatriculadas.Add(asignaturaMatriculada);

            mockIErpAcademicoServiceClient.Setup(x => x.GetAsignaturasMatricula(It.IsAny<int>()))
                .ReturnsAsync(listaAsignaturasMatriculadas);


            var situacionAsignatura = new SituacionAsignatura
            {
                Id = 1,
                Nombre = "Matriculada"
            };
            await Context.SituacionesAsignaturas.AddAsync(situacionAsignatura);

            mockExpedienteAlumno.Object.Id = 1;
            mockExpedienteAlumno.Object.IdRefIntegracionAlumno = Guid.NewGuid().ToString();
            mockExpedienteAlumno.Object.IdRefPlan = Guid.NewGuid().ToString();

            await Context.ExpedientesAlumno.AddAsync(mockExpedienteAlumno.Object);
            await Context.SaveChangesAsync();

            //ACT
            var actual = await sutMock.Object.Handle(request, CancellationToken.None);

            //ASSERT
            Assert.IsType<Unit>(actual);
            mockIMediator.Verify(s => s.Send(It.IsAny<AddConsolidacionesRequisitosExpedienteUncommitCommand>(), It.IsAny<CancellationToken>()), Times.Once);
            mockIErpAcademicoServiceClient.Verify(x => x.GetAsignaturasMatricula(It.IsAny<int>()), Times.Once);
            mockExpedienteAlumno.Verify(s => s.AddSeguimientoNoUser(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DateTime>(),
                    It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            mockIMediator.Verify(s => s.Send(It.IsAny<GetAsignaturasAsociadasQuery>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        #endregion
    }
}
