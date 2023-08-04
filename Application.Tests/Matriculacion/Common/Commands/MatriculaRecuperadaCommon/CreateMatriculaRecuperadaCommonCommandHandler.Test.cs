using System;
using MediatR;
using Moq;
using Xunit;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using Unir.Expedientes.Application.Common.Interfaces;
using Unir.Expedientes.Application.Tests.Common;
using Unir.Expedientes.Domain.Entities;
using Unir.Expedientes.Application.Matriculacion.Common.Commands.MatriculaRecuperadaCommon;
using Unir.Expedientes.Application.Common.Models.ErpAcademico.Matriculacion;
using Unir.Expedientes.Application.Common.Models.ErpAcademico.OfertaEstudio;
using Unir.Expedientes.Application.Common.Models.ErpAcademico.PlanesEstudio;
using Unir.Expedientes.Application.Matriculacion.Common.Queries.GetAlumnoByIdIntegracion;
using Unir.Expedientes.Application.ConsolidacionesRequisitosExpedientes.Commands.AddConsolidacionesRequisitosExpedienteUncommit;

namespace Unir.Expedientes.Application.Tests.Matriculacion.Common.Commands.MatriculaRecuperadaCommon
{
    [Collection("CommonTestCollection")]
    public class CreateMatriculaRecuperadaCommonCommandHandlerTest : TestBase
    {
        #region Handle
        [Fact(DisplayName = "Cuando es matrícula recuperada Retorna Ok")]
        public async Task Handle_MatriculaRecuperada()
        {
            //ARRANGE
            var request = new CreateMatriculaRecuperadaCommonCommand
            {
                MatriculaIdIntegracion = "1",
                AlumnoIdIntegracion = "1",
                IdsAsignaturasOfertadas = new List<int> { 1, 2 },
                IsAmpliacion = false,
                Mensaje = Guid.NewGuid().ToString(),
                Origen = Guid.NewGuid().ToString()
            };

            var mockIMediator = new Mock<IMediator> { CallBase = true };
            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };

            var mockExpedienteAlumno = new Mock<ExpedienteAlumno> { CallBase = true };
            mockExpedienteAlumno.SetupAllProperties();
            mockExpedienteAlumno.Setup(s =>
                s.AddTipoSituacionEstadoExpediente(It.IsAny<TipoSituacionEstado>(), It.IsAny<DateTime>()));
            mockExpedienteAlumno.Setup(s =>
                s.AddSeguimientoNoUser(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DateTime>(),
                    It.IsAny<string>(), It.IsAny<string>()));

            var sutMock = new Mock<CreateMatriculaRecuperadaCommonCommandHandler>(Context, mockIMediator.Object, mockIErpAcademicoServiceClient.Object)
            { CallBase = true };

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
                            EsAlta = false,
                            EsRecibida = false,
                            EsPrematricula = false
                        },
                        IdRefExpedienteAlumno = "1",
                        FechaCambioEstado = DateTime.UtcNow
                    },
                    new () {
                        Id = 2,
                        IdIntegracion = "2",
                        Estado = new EstadoMatriculaAcademicoModel
                        {
                            EsAlta = false,
                            EsRecibida = false,
                            EsPrematricula = false
                        },
                        IdRefExpedienteAlumno = "2",
                        FechaCambioEstado = DateTime.UtcNow
                    }
                }
            };

            var listaAsignaturasMatriculadas = new List<AsignaturaMatriculadaModel>()
            {
                new ()
                {
                    AsignaturaOfertada = new AsignaturaOfertadaModel
                    {
                        Id = 1,
                        AsignaturaPlan = new AsignaturaPlanErpAcademicoModel
                        {
                            Id = 1
                        },
                    },
                },
                new ()
                {
                    AsignaturaOfertada = new AsignaturaOfertadaModel
                    {
                        Id = 2,
                        AsignaturaPlan = new AsignaturaPlanErpAcademicoModel
                        {
                            Id = 2
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

            mockIMediator.Setup(s => s.Send(It.IsAny<AddConsolidacionesRequisitosExpedienteUncommitCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Unit.Value);

            mockIErpAcademicoServiceClient.Setup(x => x.GetAsignaturasMatricula(It.IsAny<int>()))
                .ReturnsAsync(listaAsignaturasMatriculadas);

            mockExpedienteAlumno.Object.Id = 1;
            mockExpedienteAlumno.Object.IdRefIntegracionAlumno = Guid.NewGuid().ToString();
            mockExpedienteAlumno.Object.IdRefPlan = Guid.NewGuid().ToString();
            mockExpedienteAlumno.Object.EstadoId = 3;
            mockExpedienteAlumno.Object.TiposSituacionEstadoExpedientes = new List<TipoSituacionEstadoExpediente>
            {
                new()
                {
                    Id = 1,
                    Descripcion = Guid.NewGuid().ToString(),
                    FechaInicio = new DateTime(2020, 1, 1),
                    FechaFin = new DateTime(2022, 10, 1),

                },
                new()
                {
                    Id = 2,
                    Descripcion = Guid.NewGuid().ToString(),
                    FechaInicio = new DateTime(2022, 10, 1),
                    FechaFin = null
                }
            };

            mockExpedienteAlumno.Object.HitosConseguidos = new List<HitoConseguido>
            {
                new ()
                {
                    Id = 1,
                    Nombre = "Anulado",
                    FechaInicio = DateTime.UtcNow,
                    TipoConseguidoId = 15,
                    ExpedienteAlumnoId = mockExpedienteAlumno.Object.Id
                }
            };

            await Context.ExpedientesAlumno.AddAsync(mockExpedienteAlumno.Object);

            var tipoSituacionEstado = new TipoSituacionEstado
            {
                Id = 7,
                Nombre = "Cancelacion Baja nuevo ingreso",
                Estado = new EstadoExpediente
                {
                    Id = 1,
                    Nombre = Guid.NewGuid().ToString()
                }
            };
            await Context.TiposSituacionEstado.AddAsync(tipoSituacionEstado);

            var hitoConseguido = new HitoConseguido
            {
                Id = 1,
                Nombre = "Anulado",
                FechaInicio = DateTime.UtcNow,
                TipoConseguidoId = 15,
                ExpedienteAlumno = mockExpedienteAlumno.Object
            };
            await Context.HitosConseguidos.AddAsync(hitoConseguido);
            await Context.SaveChangesAsync();

            //ACT
            var actual = await sutMock.Object.Handle(request, CancellationToken.None);

            //ASSERT
            Assert.IsType<Unit>(actual);
            mockIMediator.Verify(s => s.Send(It.IsAny<GetAlumnoByIdIntegracionQuery>(), It.IsAny<CancellationToken>()), Times.Once);
            mockIErpAcademicoServiceClient.Verify(x => x.GetAsignaturasMatricula(It.IsAny<int>()), Times.Once);
            mockExpedienteAlumno.Verify(s =>
                s.AddTipoSituacionEstadoExpediente(It.IsAny<TipoSituacionEstado>(), It.IsAny<DateTime>()), Times.Once);
            mockExpedienteAlumno.Verify(s =>
                s.AddSeguimientoNoUser(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DateTime>(),
                    It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            mockIMediator.Verify(s => s.Send(It.IsAny<AddConsolidacionesRequisitosExpedienteUncommitCommand>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact(DisplayName = "Cuando es matrícula recuperada ampliación Retorna Ok")]
        public async Task Handle_MatriculaRecuperadaAmpliacion()
        {
            //ARRANGE
            var request = new CreateMatriculaRecuperadaCommonCommand
            {
                MatriculaIdIntegracion = "1",
                AlumnoIdIntegracion = "1",
                IdsAsignaturasOfertadas = new List<int> { 1, 2 },
                IsAmpliacion = true,
                Mensaje = Guid.NewGuid().ToString(),
                Origen = Guid.NewGuid().ToString()
            };

            var mockIMediator = new Mock<IMediator> { CallBase = true };
            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };

            var mockExpedienteAlumno = new Mock<ExpedienteAlumno> { CallBase = true };
            mockExpedienteAlumno.SetupAllProperties();
            mockExpedienteAlumno.Setup(s =>
                s.AddTipoSituacionEstadoExpediente(It.IsAny<TipoSituacionEstado>(), It.IsAny<DateTime>()));
            mockExpedienteAlumno.Setup(s =>
                s.AddSeguimientoNoUser(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DateTime>(),
                    It.IsAny<string>(), It.IsAny<string>()));

            var sutMock = new Mock<CreateMatriculaRecuperadaCommonCommandHandler>(Context, mockIMediator.Object, mockIErpAcademicoServiceClient.Object)
            { CallBase = true };

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
                            EsAlta = false,
                            EsRecibida = false,
                            EsPrematricula = false
                        },
                        IdRefExpedienteAlumno = "1",
                        FechaCambioEstado = DateTime.UtcNow
                    },
                    new () {
                        Id = 2,
                        IdIntegracion = "2",
                        Estado = new EstadoMatriculaAcademicoModel
                        {
                            EsAlta = false,
                            EsRecibida = false,
                            EsPrematricula = false
                        },
                        IdRefExpedienteAlumno = "2",
                        FechaCambioEstado = DateTime.UtcNow
                    }
                }
            };

            var listaAsignaturasMatriculadas = new List<AsignaturaMatriculadaModel>()
            {
                new ()
                {
                    AsignaturaOfertada = new AsignaturaOfertadaModel
                    {
                        Id = 1,
                        AsignaturaPlan = new AsignaturaPlanErpAcademicoModel
                        {
                            Id = 1
                        },
                    },
                },
                new ()
                {
                    AsignaturaOfertada = new AsignaturaOfertadaModel
                    {
                        Id = 2,
                        AsignaturaPlan = new AsignaturaPlanErpAcademicoModel
                        {
                            Id = 2
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

            mockIMediator.Setup(s => s.Send(It.IsAny<AddConsolidacionesRequisitosExpedienteUncommitCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Unit.Value);

            mockIErpAcademicoServiceClient.Setup(x => x.GetAsignaturasMatricula(It.IsAny<int>()))
                .ReturnsAsync(listaAsignaturasMatriculadas);

            mockExpedienteAlumno.Object.Id = 1;
            mockExpedienteAlumno.Object.IdRefIntegracionAlumno = Guid.NewGuid().ToString();
            mockExpedienteAlumno.Object.IdRefPlan = Guid.NewGuid().ToString();
            mockExpedienteAlumno.Object.EstadoId = 3;
            mockExpedienteAlumno.Object.TiposSituacionEstadoExpedientes = new List<TipoSituacionEstadoExpediente>
            {
                new()
                {
                    Id = 1,
                    Descripcion = Guid.NewGuid().ToString(),
                    FechaInicio = new DateTime(2020, 1, 1),
                    FechaFin = new DateTime(2022, 10, 1),

                },
                new()
                {
                    Id = 2,
                    Descripcion = Guid.NewGuid().ToString(),
                    FechaInicio = new DateTime(2022, 10, 1),
                    FechaFin = null
                }
            };

            mockExpedienteAlumno.Object.HitosConseguidos = new List<HitoConseguido>
            {
                new ()
                {
                    Id = 1,
                    Nombre = "Anulado",
                    FechaInicio = DateTime.UtcNow,
                    TipoConseguidoId = 15,
                    ExpedienteAlumnoId = mockExpedienteAlumno.Object.Id
                }
            };

            await Context.ExpedientesAlumno.AddAsync(mockExpedienteAlumno.Object);

            var tipoSituacionEstado = new TipoSituacionEstado
            {
                Id = 9,
                Nombre = "Cancelación de baja de ampliación de matrícula",
                Estado = new EstadoExpediente
                {
                    Id = 1,
                    Nombre = Guid.NewGuid().ToString()
                }
            };
            await Context.TiposSituacionEstado.AddAsync(tipoSituacionEstado);

            var hitoConseguido = new HitoConseguido
            {
                Id = 1,
                Nombre = "Anulado",
                FechaInicio = DateTime.UtcNow,
                TipoConseguidoId = 15,
                ExpedienteAlumno = mockExpedienteAlumno.Object
            };
            await Context.HitosConseguidos.AddAsync(hitoConseguido);
            await Context.SaveChangesAsync();

            //ACT
            var actual = await sutMock.Object.Handle(request, CancellationToken.None);

            //ASSERT
            Assert.IsType<Unit>(actual);
            mockIMediator.Verify(s => s.Send(It.IsAny<GetAlumnoByIdIntegracionQuery>(), It.IsAny<CancellationToken>()), Times.Once);
            mockIErpAcademicoServiceClient.Verify(x => x.GetAsignaturasMatricula(It.IsAny<int>()), Times.Once);
            mockExpedienteAlumno.Verify(s =>
                s.AddTipoSituacionEstadoExpediente(It.IsAny<TipoSituacionEstado>(), It.IsAny<DateTime>()), Times.Once);
            mockExpedienteAlumno.Verify(s =>
                s.AddSeguimientoNoUser(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DateTime>(),
                    It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            mockIMediator.Verify(s => s.Send(It.IsAny<AddConsolidacionesRequisitosExpedienteUncommitCommand>(), It.IsAny<CancellationToken>()), Times.Once);
        }
        [Fact(DisplayName = "Cuando es matrícula recuperada variación abriendo expediente Retorna Ok")]
        public async Task Handle_MatriculaRecuperadaVariacionAbriendoExpediente()
        {
            //ARRANGE
            var request = new CreateMatriculaRecuperadaCommonCommand
            {
                MatriculaIdIntegracion = "1",
                AlumnoIdIntegracion = "1",
                IdsAsignaturasOfertadas = new List<int> { 1, 2 },
                IdsAsignaturasExcluidas = new List<int> { 3, 4 },
                IsAmpliacion = false,
                EsVariacion = true,
                Mensaje = Guid.NewGuid().ToString(),
                Origen = Guid.NewGuid().ToString()
            };

            var mockIMediator = new Mock<IMediator> { CallBase = true };
            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };

            var mockExpedienteAlumno = new Mock<ExpedienteAlumno> { CallBase = true };
            mockExpedienteAlumno.SetupAllProperties();
            mockExpedienteAlumno.Setup(s =>
                s.AddTipoSituacionEstadoExpediente(It.IsAny<TipoSituacionEstado>(), It.IsAny<DateTime>()));
            mockExpedienteAlumno.Setup(s =>
                s.AddSeguimientoNoUser(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DateTime>(),
                    It.IsAny<string>(), It.IsAny<string>()));

            var sutMock = new Mock<CreateMatriculaRecuperadaCommonCommandHandler>(Context, mockIMediator.Object, mockIErpAcademicoServiceClient.Object)
            { CallBase = true };

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
                            EsAlta = false,
                            EsRecibida = false,
                            EsPrematricula = false
                        },
                        IdRefExpedienteAlumno = "1",
                        FechaCambioEstado = DateTime.UtcNow
                    },
                    new () {
                        Id = 2,
                        IdIntegracion = "2",
                        Estado = new EstadoMatriculaAcademicoModel
                        {
                            EsAlta = false,
                            EsRecibida = false,
                            EsPrematricula = false
                        },
                        IdRefExpedienteAlumno = "2",
                        FechaCambioEstado = DateTime.UtcNow
                    }
                }
            };

            var listaAsignaturasMatriculadas = new List<AsignaturaMatriculadaModel>()
            {
                new ()
                {
                    AsignaturaOfertada = new AsignaturaOfertadaModel
                    {
                        Id = 1,
                        AsignaturaPlan = new AsignaturaPlanErpAcademicoModel
                        {
                            Id = 1
                        },
                    },
                },
                new ()
                {
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
                    AsignaturaOfertada = new AsignaturaOfertadaModel
                    {
                        Id = 3,
                        AsignaturaPlan = new AsignaturaPlanErpAcademicoModel
                        {
                            Id = 3
                        },
                    },
                },
                new ()
                {
                    AsignaturaOfertada = new AsignaturaOfertadaModel
                    {
                        Id = 4,
                        AsignaturaPlan = new AsignaturaPlanErpAcademicoModel
                        {
                            Id = 4
                        }
                    }
                }
            };

            var asignaturaExpediente = new List<AsignaturaExpediente>()
            {
                new AsignaturaExpediente
                {
                    IdRefAsignaturaPlan = "1",
                    SituacionAsignaturaId = 1
                },
                new AsignaturaExpediente
                {
                    IdRefAsignaturaPlan = "2",
                    SituacionAsignaturaId = 1
                },
                new AsignaturaExpediente
                {
                    IdRefAsignaturaPlan = "3",
                    SituacionAsignaturaId = 1
                },
                new AsignaturaExpediente
                {
                    IdRefAsignaturaPlan = "4",
                    SituacionAsignaturaId = 1
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

            mockIMediator.Setup(s => s.Send(It.IsAny<AddConsolidacionesRequisitosExpedienteUncommitCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Unit.Value);

            mockIErpAcademicoServiceClient.Setup(x => x.GetAsignaturasMatricula(It.IsAny<int>()))
                .ReturnsAsync(listaAsignaturasMatriculadas);

            mockExpedienteAlumno.Object.Id = 1;
            mockExpedienteAlumno.Object.IdRefIntegracionAlumno = Guid.NewGuid().ToString();
            mockExpedienteAlumno.Object.IdRefPlan = Guid.NewGuid().ToString();
            mockExpedienteAlumno.Object.EstadoId = 3;
            mockExpedienteAlumno.Object.AsignaturasExpedientes = asignaturaExpediente;
            mockExpedienteAlumno.Object.TiposSituacionEstadoExpedientes = new List<TipoSituacionEstadoExpediente>
            {
                new()
                {
                    Id = 1,
                    Descripcion = Guid.NewGuid().ToString(),
                    FechaInicio = new DateTime(2020, 1, 1),
                    FechaFin = new DateTime(2022, 10, 1),

                },
                new()
                {
                    Id = 2,
                    Descripcion = Guid.NewGuid().ToString(),
                    FechaInicio = new DateTime(2022, 10, 1),
                    FechaFin = null
                }
            };

            mockExpedienteAlumno.Object.HitosConseguidos = new List<HitoConseguido>
            {
                new ()
                {
                    Id = 1,
                    Nombre = "Anulado",
                    FechaInicio = DateTime.UtcNow,
                    TipoConseguidoId = 15,
                    ExpedienteAlumnoId = mockExpedienteAlumno.Object.Id
                }
            };

            await Context.ExpedientesAlumno.AddAsync(mockExpedienteAlumno.Object);

            var tipoSituacionEstado = new TipoSituacionEstado
            {
                Id = 12,
                Nombre = "Variación matrícula recuperada",
                Estado = new EstadoExpediente
                {
                    Id = 1,
                    Nombre = Guid.NewGuid().ToString()
                }
            };
            await Context.TiposSituacionEstado.AddAsync(tipoSituacionEstado);

            var hitoConseguido = new HitoConseguido
            {
                Id = 1,
                Nombre = "Anulado",
                FechaInicio = DateTime.UtcNow,
                TipoConseguidoId = 15,
                ExpedienteAlumno = mockExpedienteAlumno.Object
            };
            await Context.HitosConseguidos.AddAsync(hitoConseguido);
            await Context.SaveChangesAsync();

            //ACT
            var actual = await sutMock.Object.Handle(request, CancellationToken.None);

            //ASSERT
            Assert.IsType<Unit>(actual);
            mockIMediator.Verify(s => s.Send(It.IsAny<GetAlumnoByIdIntegracionQuery>(), It.IsAny<CancellationToken>()), Times.Once);
            mockIErpAcademicoServiceClient.Verify(x => x.GetAsignaturasMatricula(It.IsAny<int>()), Times.Once);
            mockExpedienteAlumno.Verify(s =>
                s.AddTipoSituacionEstadoExpediente(It.IsAny<TipoSituacionEstado>(), It.IsAny<DateTime>()), Times.Once);
            mockExpedienteAlumno.Verify(s =>
                s.AddSeguimientoNoUser(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DateTime>(),
                    It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            mockIMediator.Verify(s => s.Send(It.IsAny<AddConsolidacionesRequisitosExpedienteUncommitCommand>(), It.IsAny<CancellationToken>()), Times.Once);
        }
        [Fact(DisplayName = "Cuando es matrícula recuperada variación cerrando expediente Retorna Ok")]
        public async Task Handle_MatriculaRecuperadaVariacionCerrandoExpediente()
        {
            //ARRANGE
            var request = new CreateMatriculaRecuperadaCommonCommand
            {
                MatriculaIdIntegracion = "1",
                AlumnoIdIntegracion = "1",
                IdsAsignaturasOfertadas = new List<int>(),
                IdsAsignaturasExcluidas = new List<int> { 3, 4 },
                IsAmpliacion = false,
                EsVariacion = true,
                Mensaje = Guid.NewGuid().ToString(),
                Origen = Guid.NewGuid().ToString()
            };

            var mockIMediator = new Mock<IMediator> { CallBase = true };
            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };

            var mockExpedienteAlumno = new Mock<ExpedienteAlumno> { CallBase = true };
            mockExpedienteAlumno.SetupAllProperties();
            mockExpedienteAlumno.Setup(s =>
                s.AddTipoSituacionEstadoExpediente(It.IsAny<TipoSituacionEstado>(), It.IsAny<DateTime>()));
            mockExpedienteAlumno.Setup(s =>
                s.AddSeguimientoNoUser(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DateTime>(),
                    It.IsAny<string>(), It.IsAny<string>()));

            var sutMock = new Mock<CreateMatriculaRecuperadaCommonCommandHandler>(Context, mockIMediator.Object, mockIErpAcademicoServiceClient.Object)
            { CallBase = true };

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
                            EsAlta = false,
                            EsRecibida = false,
                            EsPrematricula = false
                        },
                        IdRefExpedienteAlumno = "1",
                        FechaCambioEstado = DateTime.UtcNow
                    },
                    new () {
                        Id = 2,
                        IdIntegracion = "2",
                        Estado = new EstadoMatriculaAcademicoModel
                        {
                            EsAlta = false,
                            EsRecibida = false,
                            EsPrematricula = false
                        },
                        IdRefExpedienteAlumno = "2",
                        FechaCambioEstado = DateTime.UtcNow
                    }
                }
            };

            var listaAsignaturasMatriculadas = new List<AsignaturaMatriculadaModel>()
            {
                new ()
                {
                    AsignaturaOfertada = new AsignaturaOfertadaModel
                    {
                        Id = 1,
                        AsignaturaPlan = new AsignaturaPlanErpAcademicoModel
                        {
                            Id = 1
                        },
                    },
                },
                new ()
                {
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
                    AsignaturaOfertada = new AsignaturaOfertadaModel
                    {
                        Id = 3,
                        AsignaturaPlan = new AsignaturaPlanErpAcademicoModel
                        {
                            Id = 3
                        },
                    },
                },
                new ()
                {
                    AsignaturaOfertada = new AsignaturaOfertadaModel
                    {
                        Id = 4,
                        AsignaturaPlan = new AsignaturaPlanErpAcademicoModel
                        {
                            Id = 4
                        }
                    }
                }
            };

            var asignaturaExpediente = new List<AsignaturaExpediente>()
            {
                new AsignaturaExpediente
                {
                    IdRefAsignaturaPlan = "1",
                    SituacionAsignaturaId = 6
                },
                new AsignaturaExpediente
                {
                    IdRefAsignaturaPlan = "2",
                    SituacionAsignaturaId = 6
                },
                new AsignaturaExpediente
                {
                    IdRefAsignaturaPlan = "3",
                    SituacionAsignaturaId = 1
                },
                new AsignaturaExpediente
                {
                    IdRefAsignaturaPlan = "4",
                    SituacionAsignaturaId = 1
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

            mockIMediator.Setup(s => s.Send(It.IsAny<AddConsolidacionesRequisitosExpedienteUncommitCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Unit.Value);

            mockIErpAcademicoServiceClient.Setup(x => x.GetAsignaturasMatricula(It.IsAny<int>()))
                .ReturnsAsync(listaAsignaturasMatriculadas);

            mockExpedienteAlumno.Object.Id = 1;
            mockExpedienteAlumno.Object.IdRefIntegracionAlumno = Guid.NewGuid().ToString();
            mockExpedienteAlumno.Object.IdRefPlan = Guid.NewGuid().ToString();
            mockExpedienteAlumno.Object.EstadoId = 3;
            mockExpedienteAlumno.Object.AsignaturasExpedientes = asignaturaExpediente;
            mockExpedienteAlumno.Object.TiposSituacionEstadoExpedientes = new List<TipoSituacionEstadoExpediente>
            {
                new()
                {
                    Id = 1,
                    Descripcion = Guid.NewGuid().ToString(),
                    FechaInicio = new DateTime(2020, 1, 1),
                    FechaFin = new DateTime(2022, 10, 1),

                },
                new()
                {
                    Id = 2,
                    Descripcion = Guid.NewGuid().ToString(),
                    FechaInicio = new DateTime(2022, 10, 1),
                    FechaFin = null
                }
            };

            mockExpedienteAlumno.Object.HitosConseguidos = new List<HitoConseguido>
            {
                new ()
                {
                    Id = 1,
                    Nombre = "Anulado",
                    FechaInicio = DateTime.UtcNow,
                    TipoConseguidoId = 15,
                    ExpedienteAlumnoId = mockExpedienteAlumno.Object.Id
                }
            };

            await Context.ExpedientesAlumno.AddAsync(mockExpedienteAlumno.Object);

            var tipoSituacionEstado = new TipoSituacionEstado
            {
                Id = 12,
                Nombre = "Variación matrícula recuperada",
                Estado = new EstadoExpediente
                {
                    Id = 1,
                    Nombre = Guid.NewGuid().ToString()
                }
            };
            await Context.TiposSituacionEstado.AddAsync(tipoSituacionEstado);
            var tipoSituacionEstadoSinASignaturas = new TipoSituacionEstado
            {
                Id = 21,
                Nombre = "Variación matrícula recuperada deja sin asignaturas",
                Estado = new EstadoExpediente
                {
                    Id = 2,
                    Nombre = Guid.NewGuid().ToString()
                }
            };
            await Context.TiposSituacionEstado.AddAsync(tipoSituacionEstadoSinASignaturas);

            var hitoConseguido = new HitoConseguido
            {
                Id = 1,
                Nombre = "Anulado",
                FechaInicio = DateTime.UtcNow,
                TipoConseguidoId = 15,
                ExpedienteAlumno = mockExpedienteAlumno.Object
            };
            await Context.HitosConseguidos.AddAsync(hitoConseguido);
            await Context.SaveChangesAsync();

            //ACT
            var actual = await sutMock.Object.Handle(request, CancellationToken.None);

            //ASSERT
            Assert.IsType<Unit>(actual);
            mockIMediator.Verify(s => s.Send(It.IsAny<GetAlumnoByIdIntegracionQuery>(), It.IsAny<CancellationToken>()), Times.Once);
            mockIErpAcademicoServiceClient.Verify(x => x.GetAsignaturasMatricula(It.IsAny<int>()), Times.Once);
            mockExpedienteAlumno.Verify(s =>
                s.AddTipoSituacionEstadoExpediente(It.IsAny<TipoSituacionEstado>(), It.IsAny<DateTime>()), Times.Once);
            mockExpedienteAlumno.Verify(s =>
                s.AddSeguimientoNoUser(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DateTime>(),
                    It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            mockIMediator.Verify(s => s.Send(It.IsAny<AddConsolidacionesRequisitosExpedienteUncommitCommand>(), It.IsAny<CancellationToken>()), Times.Never);
        }
        #endregion
    }
}
