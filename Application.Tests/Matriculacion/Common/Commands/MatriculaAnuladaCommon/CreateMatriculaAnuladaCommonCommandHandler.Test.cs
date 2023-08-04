using MediatR;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Unir.Expedientes.Application.Common.Interfaces;
using Unir.Expedientes.Application.Common.Models.ErpAcademico.Matriculacion;
using Unir.Expedientes.Application.ConsolidacionesRequisitosExpedientes.Commands.AddConsolidacionesRequisitosExpedienteUncommit;
using Unir.Expedientes.Application.Matriculacion.Common.Commands.EliminarConsolidacionRequisitosAnuladaCommon;
using Unir.Expedientes.Application.Matriculacion.Common.Commands.MatriculaAnuladaCommon;
using Unir.Expedientes.Application.Matriculacion.Common.Queries.GetAlumnoByIdIntegracion;
using Unir.Expedientes.Application.Tests.Common;
using Unir.Expedientes.Domain.Entities;
using Xunit;

namespace Unir.Expedientes.Application.Tests.Matriculacion.Common.Commands.MatriculaAnuladaCommon
{
    [Collection("CommonTestCollection")]
    public class CreateMatriculaAnuladaCommonCommandHandlerTest : TestBase
    {
        #region Handle
        [Fact(DisplayName = "Cuando no existen matrículas activas Retorna Ok")]
        public async Task Handle_Anulada_MatriculasActivas_Ok()
        {
            //ARRANGE
            var request = new CreateMatriculaAnuladaCommonCommand
            {
                AlumnoIdIntegracion = "1",
                MatriculaIdIntegracion = "1",
                IdCausaBaja = 21,
                IdTipoBaja = 1,
                IdsAsignaturasOfertadas = new List<int> { 1, 2, 3 },
                IsAmpliacion = false,
                Mensaje = Guid.NewGuid().ToString(),
                Origen = Guid.NewGuid().ToString()
            };

            var causaBaja = new CausaBajaMatriculaModel
            {
                Id = 21,
                Nombre = "Documentación - No acceso"
            };

            var tipoBaja = new TipoBajaMatriculaModel
            {
                IdTipoBajaMatricula = 1,
                Nombre = "Total"
            };

            var mockIMediator = new Mock<IMediator> { CallBase = true };
            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient> { CallBase = true };
            var mockExpedienteAlumno = new Mock<ExpedienteAlumno> { CallBase = true };
            mockExpedienteAlumno.SetupAllProperties();
            mockExpedienteAlumno.Setup(s =>
                s.AddTipoSituacionEstadoExpediente(It.IsAny<TipoSituacionEstado>(), It.IsAny<DateTime>()));
            mockExpedienteAlumno.Setup(s => s.AddHitosConseguidos(It.IsAny<TipoHitoConseguido>(),
                It.IsAny<DateTime>(), It.IsAny<ExpedienteEspecializacion>()));
            mockExpedienteAlumno.Setup(s =>
                s.AddSeguimientoNoUser(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DateTime>(),
                    It.IsAny<string>(), It.IsAny<string>()));
            mockIErpAcademicoServiceClient.Setup(x => x.GetCausaBajaMatricula(It.IsAny<int>()))
                .ReturnsAsync(causaBaja);
            mockIErpAcademicoServiceClient.Setup(x => x.GetTipoBajaMatricula(It.IsAny<int>()))
                .ReturnsAsync(tipoBaja);

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
                        FechaCambioEstado = DateTime.UtcNow
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
                Id = 15,
                Nombre = "Baja matrícula de nuevo ingreso"
            };
            await Context.TiposSituacionEstado.AddAsync(tipoSituacionEstado);
            await Context.SaveChangesAsync();

            var mock = new Mock<CreateMatriculaAnuladaCommonCommandHandler>(Context, mockIMediator.Object, mockIErpAcademicoServiceClient.Object)
            { CallBase = true };

            mockIMediator.Setup(s => s.Send(It.IsAny<EliminarConsolidacionRequisitosAnuladaCommonCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Unit.Value);

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
                }
            };

            //ACT
            var actual = await mock.Object.Handle(request, CancellationToken.None);

            //ASSERT
            Assert.IsType<Unit>(actual);
            mockExpedienteAlumno.Verify(s =>
                s.AddTipoSituacionEstadoExpediente(It.IsAny<TipoSituacionEstado>(), It.IsAny<DateTime>()), Times.Once);
            mockExpedienteAlumno.Verify(s => s.AddHitosConseguidos(It.IsAny<TipoHitoConseguido>(),
                It.IsAny<DateTime>(), It.IsAny<ExpedienteEspecializacion>()), Times.Once);
            mockExpedienteAlumno.Verify(s =>
                s.AddSeguimientoNoUser(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DateTime>(),
                    It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            mockIErpAcademicoServiceClient.Verify(x => x.GetCausaBajaMatricula(It.IsAny<int>()), Times.Once);
            mockIErpAcademicoServiceClient.Verify(x => x.GetTipoBajaMatricula(It.IsAny<int>()), Times.Once);
            mockIMediator.Verify(s => s.Send(It.IsAny<GetAlumnoByIdIntegracionQuery>(), It.IsAny<CancellationToken>()), Times.Once);
            mockIMediator.Verify(s => s.Send(It.IsAny<EliminarConsolidacionRequisitosAnuladaCommonCommand>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact(DisplayName = "Cuando si existen matrículas activas Retorna Ok")]
        public async Task Handle_Anulada_NoMatriculasActivas_Ok()
        {
            //ARRANGE
            var request = new CreateMatriculaAnuladaCommonCommand
            {
                AlumnoIdIntegracion = "1",
                MatriculaIdIntegracion = "1",
                IdCausaBaja = 21,
                IdTipoBaja = 1,
                IdsAsignaturasOfertadas = new List<int> { 1, 2, 3 },
                IsAmpliacion = false,
                Mensaje = Guid.NewGuid().ToString(),
                Origen = Guid.NewGuid().ToString()
            };

            var causaBaja = new CausaBajaMatriculaModel
            {
                Id = 21,
                Nombre = "Documentación - No acceso"
            };

            var tipoBaja = new TipoBajaMatriculaModel
            {
                IdTipoBajaMatricula = 1,
                Nombre = "Total"
            };

            var mockIMediator = new Mock<IMediator> { CallBase = true };
            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient> { CallBase = true };
            var mockExpedienteAlumno = new Mock<ExpedienteAlumno> { CallBase = true };
            mockExpedienteAlumno.SetupAllProperties();
            mockExpedienteAlumno.Setup(s =>
                s.AddSeguimientoNoUser(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DateTime>(),
                    It.IsAny<string>(), It.IsAny<string>()));
            mockIMediator.Setup(s => s.Send(It.IsAny<AddConsolidacionesRequisitosExpedienteUncommitCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Unit.Value);
            mockIErpAcademicoServiceClient.Setup(x => x.GetCausaBajaMatricula(It.IsAny<int>()))
                .ReturnsAsync(causaBaja);
            mockIErpAcademicoServiceClient.Setup(x => x.GetTipoBajaMatricula(It.IsAny<int>()))
                .ReturnsAsync(tipoBaja);

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
                            EsAlta = true
                        },
                        IdRefExpedienteAlumno = "1",
                        FechaCambioEstado = DateTime.UtcNow
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

            var tipoHitoConseguido = new TipoHitoConseguido
            {
                Id = 15,
                Nombre = "Anulado"
            };
            await Context.TiposHitoConseguidos.AddAsync(tipoHitoConseguido);

            var tipoSituacionEstado = new TipoSituacionEstado
            {
                Id = 15,
                Nombre = "Baja matrícula de nuevo ingreso"
            };
            await Context.TiposSituacionEstado.AddAsync(tipoSituacionEstado);
            await Context.SaveChangesAsync();

            var mock = new Mock<CreateMatriculaAnuladaCommonCommandHandler>(Context, mockIMediator.Object, mockIErpAcademicoServiceClient.Object)
            { CallBase = true };

            mockIMediator.Setup(s => s.Send(It.IsAny<EliminarConsolidacionRequisitosAnuladaCommonCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Unit.Value);

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
                }
            };

            //ACT
            var actual = await mock.Object.Handle(request, CancellationToken.None);

            //ASSERT
            Assert.IsType<Unit>(actual);
            mockExpedienteAlumno.Verify(s =>
                s.AddSeguimientoNoUser(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DateTime>(),
                    It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            mockIMediator.Verify(s => s.Send(It.IsAny<AddConsolidacionesRequisitosExpedienteUncommitCommand>(), It.IsAny<CancellationToken>()), Times.Once);
            mockIErpAcademicoServiceClient.Verify(x => x.GetCausaBajaMatricula(It.IsAny<int>()), Times.Once);
            mockIErpAcademicoServiceClient.Verify(x => x.GetTipoBajaMatricula(It.IsAny<int>()), Times.Once);
            mockIMediator.Verify(s => s.Send(It.IsAny<GetAlumnoByIdIntegracionQuery>(), It.IsAny<CancellationToken>()), Times.Once);
            mockIMediator.Verify(s => s.Send(It.IsAny<EliminarConsolidacionRequisitosAnuladaCommonCommand>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact(DisplayName = "Ampliación cuando no existen matrículas activas Retorna Ok")]
        public async Task Handle_AmpliacionAnulada_MatriculasActivas_Ok()
        {
            //ARRANGE
            var request = new CreateMatriculaAnuladaCommonCommand
            {
                AlumnoIdIntegracion = "1",
                MatriculaIdIntegracion = "1",
                IdCausaBaja = 21,
                IdTipoBaja = 1,
                IdsAsignaturasOfertadas = new List<int> { 1, 2, 3 },
                IsAmpliacion = true,
                Mensaje = Guid.NewGuid().ToString(),
                Origen = Guid.NewGuid().ToString()
            };

            var mockIMediator = new Mock<IMediator> { CallBase = true };
            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient> { CallBase = true };
            var mockExpedienteAlumno = new Mock<ExpedienteAlumno> { CallBase = true };
            mockExpedienteAlumno.SetupAllProperties();
            mockExpedienteAlumno.Setup(s =>
                s.AddTipoSituacionEstadoExpediente(It.IsAny<TipoSituacionEstado>(), It.IsAny<DateTime>()));
            mockExpedienteAlumno.Setup(s => s.AddHitosConseguidos(It.IsAny<TipoHitoConseguido>(),
                It.IsAny<DateTime>(), It.IsAny<ExpedienteEspecializacion>()));
            mockExpedienteAlumno.Setup(s =>
                s.AddSeguimientoNoUser(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DateTime>(),
                    It.IsAny<string>(), It.IsAny<string>()));

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
                        FechaCambioEstado = DateTime.UtcNow
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
                Id = 17,
                Nombre = "Baja ampliación matrícula"
            };
            await Context.TiposSituacionEstado.AddAsync(tipoSituacionEstado);
            await Context.SaveChangesAsync();

            var mock = new Mock<CreateMatriculaAnuladaCommonCommandHandler>(Context, mockIMediator.Object, mockIErpAcademicoServiceClient.Object)
            { CallBase = true };

            mockIMediator.Setup(s => s.Send(It.IsAny<EliminarConsolidacionRequisitosAnuladaCommonCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Unit.Value);

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
                }
            };

            //ACT
            var actual = await mock.Object.Handle(request, CancellationToken.None);

            //ASSERT
            Assert.IsType<Unit>(actual);
            mockExpedienteAlumno.Verify(s =>
                s.AddTipoSituacionEstadoExpediente(It.IsAny<TipoSituacionEstado>(), It.IsAny<DateTime>()), Times.Once);
            mockExpedienteAlumno.Setup(s => s.AddHitosConseguidos(It.IsAny<TipoHitoConseguido>(),
                It.IsAny<DateTime>(), It.IsAny<ExpedienteEspecializacion>()));
            mockExpedienteAlumno.Verify(s =>
                s.AddSeguimientoNoUser(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DateTime>(),
                    It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            mockIMediator.Verify(s => s.Send(It.IsAny<GetAlumnoByIdIntegracionQuery>(), It.IsAny<CancellationToken>()), Times.Once);
            mockIMediator.Verify(s => s.Send(It.IsAny<EliminarConsolidacionRequisitosAnuladaCommonCommand>(),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact(DisplayName = "Ampliación cuando si existen matrículas activas Retorna Ok")]
        public async Task Handle_AmpliacionAnulada_NoMatriculasActivas_Ok()
        {
            //ARRANGE
            var request = new CreateMatriculaAnuladaCommonCommand
            {
                AlumnoIdIntegracion = "1",
                MatriculaIdIntegracion = "1",
                IdCausaBaja = 21,
                IdTipoBaja = 1,
                IdsAsignaturasOfertadas = new List<int> { 1, 2, 3 },
                IsAmpliacion = true,
                Mensaje = Guid.NewGuid().ToString(),
                Origen = Guid.NewGuid().ToString()
            };

            var mockIMediator = new Mock<IMediator> { CallBase = true };
            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient> { CallBase = true };
            var mockExpedienteAlumno = new Mock<ExpedienteAlumno> { CallBase = true };
            mockExpedienteAlumno.SetupAllProperties();
            mockExpedienteAlumno.Setup(s =>
                s.AddSeguimientoNoUser(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DateTime>(),
                    It.IsAny<string>(), It.IsAny<string>()));
            mockIMediator.Setup(s => s.Send(It.IsAny<AddConsolidacionesRequisitosExpedienteUncommitCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Unit.Value);

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
                            EsAlta = true
                        },
                        IdRefExpedienteAlumno = "1",
                        FechaCambioEstado = DateTime.UtcNow
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

            var tipoHitoConseguido = new TipoHitoConseguido
            {
                Id = 15,
                Nombre = "Anulado"
            };
            await Context.TiposHitoConseguidos.AddAsync(tipoHitoConseguido);

            var tipoSituacionEstado = new TipoSituacionEstado
            {
                Id = 17,
                Nombre = "Baja ampliación matrícula"
            };
            await Context.TiposSituacionEstado.AddAsync(tipoSituacionEstado);
            await Context.SaveChangesAsync();

            var mock = new Mock<CreateMatriculaAnuladaCommonCommandHandler>(Context, mockIMediator.Object, mockIErpAcademicoServiceClient.Object)
            { CallBase = true };

            mockIMediator.Setup(s => s.Send(It.IsAny<EliminarConsolidacionRequisitosAnuladaCommonCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Unit.Value);

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
                }
            };

            //ACT
            var actual = await mock.Object.Handle(request, CancellationToken.None);

            //ASSERT
            Assert.IsType<Unit>(actual);
            mockExpedienteAlumno.Verify(s =>
                s.AddSeguimientoNoUser(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DateTime>(),
                    It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            mockIMediator.Verify(s => s.Send(It.IsAny<AddConsolidacionesRequisitosExpedienteUncommitCommand>(), It.IsAny<CancellationToken>()), Times.Once);
            mockIMediator.Verify(s => s.Send(It.IsAny<GetAlumnoByIdIntegracionQuery>(), It.IsAny<CancellationToken>()), Times.Once);
            mockIMediator.Verify(s => s.Send(It.IsAny<EliminarConsolidacionRequisitosAnuladaCommonCommand>(), It.IsAny<CancellationToken>()), Times.Once);
        }
        #endregion
    }
}
