using MediatR;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Unir.Expedientes.Application.Common.Models.ErpAcademico.Matriculacion;
using Unir.Expedientes.Application.Common.Models.ErpAcademico.OfertaEstudio;
using Unir.Expedientes.Application.Common.Models.ErpAcademico.PlanesEstudio;
using Unir.Expedientes.Application.ConsolidacionesRequisitosExpedientes.Commands.AddConsolidacionesRequisitosExpedienteUncommit;
using Unir.Expedientes.Application.Matriculacion.Common.Commands.EliminarConsolidacionRequisitosAnuladaCommon;
using Unir.Expedientes.Application.Matriculacion.Common.Queries.GetAlumnoByIdIntegracion;
using Unir.Expedientes.Application.Matriculacion.MatriculaVariacionAnulada.Commands;
using Unir.Expedientes.Application.Tests.Common;
using Unir.Expedientes.Domain.Entities;
using Xunit;

namespace Unir.Expedientes.Application.Tests.Matriculacion.MatriculaVariacionAnulada.Commands
{
    [Collection("CommonTestCollection")]
    public class CreateMatriculaAnuladaCommandHandlerTest : TestBase
    {
        #region Handle

        [Fact(DisplayName = "Cuando no existen matrículas activas Retorna Ok")]
        public async Task Handle_NoMatriculasActivas_Ok()
        {
            //ARRANGE
            var request = new CreateMatriculaVariacionAnuladaCommand
            {
                AlumnoIdIntegracion = "1",
                MatriculaIdIntegracion = "1",
                IdsAsignaturasOfertadasAdicionadas = new List<int> { 1, 2, 3 },
                IdsAsignaturasOfertadasExcluidas = new List<int> { 4, 5, 6 },
                FechaHora = DateTime.UtcNow,
                Mensaje = Guid.NewGuid().ToString(),
                Origen = Guid.NewGuid().ToString()
            };

            var mockIMediator = new Mock<IMediator> { CallBase = true };
            var mockExpedienteAlumno = new Mock<ExpedienteAlumno> { CallBase = true };
            mockExpedienteAlumno.SetupAllProperties();
            mockExpedienteAlumno.Setup(s => s.CambiarSituacionAsignaturas(It.IsAny<List<int>>(), It.IsAny<int>()));
            mockExpedienteAlumno.Setup(s => s.AddTipoSituacionEstadoExpediente(It.IsAny<TipoSituacionEstado>(), It.IsAny<DateTime>()));
            mockExpedienteAlumno.Setup(s => s.AddHitosConseguidos(It.IsAny<TipoHitoConseguido>(),
                It.IsAny<DateTime>(), It.IsAny<ExpedienteEspecializacion>()));
            mockExpedienteAlumno.Setup(s =>
                s.AddSeguimientoNoUser(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DateTime>(),
                    It.IsAny<string>(), It.IsAny<string>()));

            mockExpedienteAlumno.Object.Id = 1;
            mockExpedienteAlumno.Object.IdRefIntegracionAlumno = Guid.NewGuid().ToString();
            mockExpedienteAlumno.Object.IdRefPlan = Guid.NewGuid().ToString();
            mockExpedienteAlumno.Object.AsignaturasExpedientes = new List<AsignaturaExpediente>
            {
                new ()
                {
                    Id = 1,
                    NombreAsignatura = Guid.NewGuid().ToString(),
                    SituacionAsignaturaId = 1,
                    IdRefAsignaturaPlan = "1"
                },
                new ()
                {
                    Id = 2,
                    NombreAsignatura = Guid.NewGuid().ToString(),
                    SituacionAsignaturaId = 1,
                    IdRefAsignaturaPlan = "2"
                },
                new ()
                {
                    Id = 3,
                    NombreAsignatura = Guid.NewGuid().ToString(),
                    SituacionAsignaturaId = 1,
                    IdRefAsignaturaPlan = "3"
                },
                new ()
                {
                    Id = 4,
                    NombreAsignatura = Guid.NewGuid().ToString(),
                    SituacionAsignaturaId = 2,
                    IdRefAsignaturaPlan = "4"
                },
                new ()
                {
                    Id = 5,
                    NombreAsignatura = Guid.NewGuid().ToString(),
                    SituacionAsignaturaId = 2,
                    IdRefAsignaturaPlan = "5"
                },
                new ()
                {
                    Id = 6,
                    NombreAsignatura = Guid.NewGuid().ToString(),
                    SituacionAsignaturaId = 3,
                    IdRefAsignaturaPlan = "6"
                }
            };

            var alumnoAcademico = new AlumnoAcademicoModel
            {
                Id = 1,
                Matriculas = new List<MatriculaAcademicoModel>
                {
                    new () {
                        Id = 1,
                        IdIntegracion = "1",
                        Estado = new EstadoMatriculaAcademicoModel
                        {
                            EsAlta = false
                        },
                        IdRefExpedienteAlumno = "1",
                        FechaCambioEstado = DateTime.UtcNow,
                        AsignaturaMatriculadas = new List<AsignaturaMatriculadaModel>
                        {
                            new ()
                            {
                                Id = 1,
                                AsignaturaOfertada = new AsignaturaOfertadaModel
                                {
                                    Id = 1,
                                    AsignaturaPlan = new AsignaturaPlanErpAcademicoModel
                                    {
                                        Id = 1
                                    }
                                }
                            },
                            new ()
                            {
                                Id = 2,
                                AsignaturaOfertada = new AsignaturaOfertadaModel
                                {
                                    Id = 2,
                                    AsignaturaPlan = new AsignaturaPlanErpAcademicoModel
                                    {
                                        Id = 2
                                    }
                                }
                            },
                            new ()
                            {
                                Id = 3,
                                AsignaturaOfertada = new AsignaturaOfertadaModel
                                {
                                    Id = 3,
                                    AsignaturaPlan = new AsignaturaPlanErpAcademicoModel
                                    {
                                        Id = 3
                                    }
                                }
                            },
                            new ()
                            {
                                Id = 4,
                                AsignaturaOfertada = new AsignaturaOfertadaModel
                                {
                                    Id = 4,
                                    AsignaturaPlan = new AsignaturaPlanErpAcademicoModel
                                    {
                                        Id = 4
                                    }
                                }
                            },
                            new ()
                            {
                                Id = 5,
                                AsignaturaOfertada = new AsignaturaOfertadaModel
                                {
                                    Id = 5,
                                    AsignaturaPlan = new AsignaturaPlanErpAcademicoModel
                                    {
                                        Id = 5
                                    }
                                }
                            },
                            new ()
                            {
                                Id = 6,
                                AsignaturaOfertada = new AsignaturaOfertadaModel
                                {
                                    Id = 6,
                                    AsignaturaPlan = new AsignaturaPlanErpAcademicoModel
                                    {
                                        Id = 6
                                    }
                                }
                            }
                        }
                    },
                    new () {
                        Id = 2,
                        IdIntegracion = "2",
                        Estado = new EstadoMatriculaAcademicoModel
                        {
                            EsAlta = false
                        },
                        IdRefExpedienteAlumno = "2",
                        FechaCambioEstado = DateTime.UtcNow
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

            var tipoHitoConseguido = new TipoHitoConseguido
            {
                Id = 15,
                Nombre = "Anulado"
            };
            await Context.TiposHitoConseguidos.AddAsync(tipoHitoConseguido);

            var tipoSituacionEstado = new TipoSituacionEstado
            {
                Id = 11,
                Nombre = "Variación matrícula anulada"
            };
            await Context.TiposSituacionEstado.AddAsync(tipoSituacionEstado);
            await Context.SaveChangesAsync();

            var mock = new Mock<CreateMatriculaVariacionAnuladaCommandHandler>(Context, mockIMediator.Object)
            { CallBase = true };

            mockIMediator.Setup(s => s.Send(It.IsAny<EliminarConsolidacionRequisitosAnuladaCommonCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Unit.Value);

            //ACT
            var actual = await mock.Object.Handle(request, CancellationToken.None);

            //ASSERT
            Assert.IsType<Unit>(actual);
            mockExpedienteAlumno.Verify(s => s.CambiarSituacionAsignaturas(It.IsAny<List<int>>(), It.IsAny<int>()), Times.AtMost(2));
            mockExpedienteAlumno.Verify(s =>
                s.AddTipoSituacionEstadoExpediente(It.IsAny<TipoSituacionEstado>(), It.IsAny<DateTime>()), Times.Once);
            mockExpedienteAlumno.Verify(s => s.AddHitosConseguidos(It.IsAny<TipoHitoConseguido>(),
                It.IsAny<DateTime>(), It.IsAny<ExpedienteEspecializacion>()), Times.Once);
            mockExpedienteAlumno.Verify(s =>
                s.AddSeguimientoNoUser(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DateTime>(),
                    It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            mockIMediator.Verify(s => s.Send(It.IsAny<GetAlumnoByIdIntegracionQuery>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact(DisplayName = "Cuando sí existen matrículas activas Retorna Ok")]
        public async Task Handle_SiMatriculasActivas_Ok()
        {
            //ARRANGE
            var request = new CreateMatriculaVariacionAnuladaCommand
            {
                AlumnoIdIntegracion = "1",
                MatriculaIdIntegracion = "1",
                IdsAsignaturasOfertadasAdicionadas = new List<int> { 1, 2, 3 },
                IdsAsignaturasOfertadasExcluidas = new List<int> { 4, 5, 6 },
                FechaHora = DateTime.UtcNow,
                Mensaje = Guid.NewGuid().ToString(),
                Origen = Guid.NewGuid().ToString()
            };

            var mockIMediator = new Mock<IMediator> { CallBase = true };
            var mockExpedienteAlumno = new Mock<ExpedienteAlumno> { CallBase = true };
            mockExpedienteAlumno.SetupAllProperties();
            mockExpedienteAlumno.Setup(s => s.CambiarSituacionAsignaturas(It.IsAny<List<int>>(), It.IsAny<int>()));
            mockExpedienteAlumno.Setup(s => s.AddTipoSituacionEstadoExpediente(It.IsAny<TipoSituacionEstado>(), It.IsAny<DateTime>()));
            mockExpedienteAlumno.Setup(s => s.AddHitosConseguidos(It.IsAny<TipoHitoConseguido>(),
                It.IsAny<DateTime>(), It.IsAny<ExpedienteEspecializacion>()));
            mockExpedienteAlumno.Setup(s =>
                s.AddSeguimientoNoUser(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DateTime>(),
                    It.IsAny<string>(), It.IsAny<string>()));

            mockExpedienteAlumno.Object.Id = 1;
            mockExpedienteAlumno.Object.IdRefIntegracionAlumno = Guid.NewGuid().ToString();
            mockExpedienteAlumno.Object.IdRefPlan = Guid.NewGuid().ToString();
            mockExpedienteAlumno.Object.AsignaturasExpedientes = new List<AsignaturaExpediente>
            {
                new ()
                {
                    Id = 1,
                    NombreAsignatura = Guid.NewGuid().ToString(),
                    SituacionAsignaturaId = 1,
                    IdRefAsignaturaPlan = "1"
                },
                new ()
                {
                    Id = 2,
                    NombreAsignatura = Guid.NewGuid().ToString(),
                    SituacionAsignaturaId = 1,
                    IdRefAsignaturaPlan = "2"
                },
                new ()
                {
                    Id = 3,
                    NombreAsignatura = Guid.NewGuid().ToString(),
                    SituacionAsignaturaId = 1,
                    IdRefAsignaturaPlan = "3"
                },
                new ()
                {
                    Id = 4,
                    NombreAsignatura = Guid.NewGuid().ToString(),
                    SituacionAsignaturaId = 2,
                    IdRefAsignaturaPlan = "4"
                },
                new ()
                {
                    Id = 5,
                    NombreAsignatura = Guid.NewGuid().ToString(),
                    SituacionAsignaturaId = 2,
                    IdRefAsignaturaPlan = "5"
                },
                new ()
                {
                    Id = 6,
                    NombreAsignatura = Guid.NewGuid().ToString(),
                    SituacionAsignaturaId = 3,
                    IdRefAsignaturaPlan = "6"
                }
            };

            var alumnoAcademico = new AlumnoAcademicoModel
            {
                Id = 1,
                Matriculas = new List<MatriculaAcademicoModel>
                {
                    new () {
                        Id = 1,
                        IdIntegracion = "1",
                        Estado = new EstadoMatriculaAcademicoModel
                        {
                            EsAlta = false
                        },
                        IdRefExpedienteAlumno = "1",
                        FechaCambioEstado = DateTime.UtcNow,
                        AsignaturaMatriculadas = new List<AsignaturaMatriculadaModel>
                        {
                            new ()
                            {
                                Id = 1,
                                AsignaturaOfertada = new AsignaturaOfertadaModel
                                {
                                    Id = 1,
                                    AsignaturaPlan = new AsignaturaPlanErpAcademicoModel
                                    {
                                        Id = 1
                                    }
                                }
                            },
                            new ()
                            {
                                Id = 2,
                                AsignaturaOfertada = new AsignaturaOfertadaModel
                                {
                                    Id = 2,
                                    AsignaturaPlan = new AsignaturaPlanErpAcademicoModel
                                    {
                                        Id = 2
                                    }
                                }
                            },
                            new ()
                            {
                                Id = 3,
                                AsignaturaOfertada = new AsignaturaOfertadaModel
                                {
                                    Id = 3,
                                    AsignaturaPlan = new AsignaturaPlanErpAcademicoModel
                                    {
                                        Id = 3
                                    }
                                }
                            },
                            new ()
                            {
                                Id = 4,
                                AsignaturaOfertada = new AsignaturaOfertadaModel
                                {
                                    Id = 4,
                                    AsignaturaPlan = new AsignaturaPlanErpAcademicoModel
                                    {
                                        Id = 4
                                    }
                                }
                            },
                            new ()
                            {
                                Id = 5,
                                AsignaturaOfertada = new AsignaturaOfertadaModel
                                {
                                    Id = 5,
                                    AsignaturaPlan = new AsignaturaPlanErpAcademicoModel
                                    {
                                        Id = 5
                                    }
                                }
                            },
                            new ()
                            {
                                Id = 6,
                                AsignaturaOfertada = new AsignaturaOfertadaModel
                                {
                                    Id = 6,
                                    AsignaturaPlan = new AsignaturaPlanErpAcademicoModel
                                    {
                                        Id = 6
                                    }
                                }
                            }
                        }
                    },
                    new () {
                        Id = 2,
                        IdIntegracion = "2",
                        Estado = new EstadoMatriculaAcademicoModel
                        {
                            EsAlta = true
                        },
                        IdRefExpedienteAlumno = "2",
                        FechaCambioEstado = DateTime.UtcNow
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

            var tipoSituacionEstado = new TipoSituacionEstado
            {
                Id = 11,
                Nombre = "Variación matrícula anulada"
            };
            await Context.TiposSituacionEstado.AddAsync(tipoSituacionEstado);
            await Context.SaveChangesAsync();

            var mock = new Mock<CreateMatriculaVariacionAnuladaCommandHandler>(Context, mockIMediator.Object)
            { CallBase = true };

            mockIMediator.Setup(s => s.Send(It.IsAny<AddConsolidacionesRequisitosExpedienteUncommitCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Unit.Value);

            //ACT
            var actual = await mock.Object.Handle(request, CancellationToken.None);

            //ASSERT
            Assert.IsType<Unit>(actual);
            mockExpedienteAlumno.Verify(s => s.CambiarSituacionAsignaturas(It.IsAny<List<int>>(), It.IsAny<int>()), Times.AtMost(2));
            mockExpedienteAlumno.Verify(s =>
                s.AddTipoSituacionEstadoExpediente(It.IsAny<TipoSituacionEstado>(), It.IsAny<DateTime>()), Times.Once);
            mockExpedienteAlumno.Setup(s => s.AddHitosConseguidos(It.IsAny<TipoHitoConseguido>(),
                It.IsAny<DateTime>(), It.IsAny<ExpedienteEspecializacion>()));
            mockExpedienteAlumno.Verify(s =>
                s.AddSeguimientoNoUser(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DateTime>(),
                    It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            mockIMediator.Verify(s => s.Send(It.IsAny<GetAlumnoByIdIntegracionQuery>(), It.IsAny<CancellationToken>()), Times.Once);
            mockIMediator.Verify(s => s.Send(It.IsAny<AddConsolidacionesRequisitosExpedienteUncommitCommand>(),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        #endregion
    }
}
