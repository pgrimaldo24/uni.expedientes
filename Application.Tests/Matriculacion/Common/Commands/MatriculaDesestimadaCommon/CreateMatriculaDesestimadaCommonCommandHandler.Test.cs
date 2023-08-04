using MediatR;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Unir.Expedientes.Application.Common.Models.ErpAcademico.Matriculacion;
using Unir.Expedientes.Application.ConsolidacionesRequisitosExpedientes.Commands.AddConsolidacionesRequisitosExpedienteUncommit;
using Unir.Expedientes.Application.Matriculacion.Common.Commands.MatriculaDesestimadaCommon;
using Unir.Expedientes.Application.Matriculacion.Common.Queries.GetAlumnoByIdIntegracion;
using Unir.Expedientes.Application.Tests.Common;
using Unir.Expedientes.Domain.Entities;
using Xunit;

namespace Unir.Expedientes.Application.Tests.Matriculacion.Common.Commands.MatriculaDesestimadaCommon
{
    [Collection("CommonTestCollection")]
    public class CreateMatriculaDesestimadaCommonCommandHandlerTest : TestBase
    {
        #region Handle
        [Fact(DisplayName = "Cuando no existe matrículas activas en las variaciones Retorna Ok")]
        public async Task Handle_NoExistenMatriculaActivas()
        {
            var request = new CreateMatriculaDesestimadaCommonCommand
            {
                AlumnoIdIntegracion = "1",
                MatriculaIdIntegracion = "1",
                UniversidadIdIntegracion = "1",
                IdVariacion = 1,
                VariacionIdIntegracion = "1",
                FechaHora = DateTime.UtcNow,
                Motivo = Guid.NewGuid().ToString(),
                EsAmpliacion = false,
                Mensaje = Guid.NewGuid().ToString(),
                Origen = Guid.NewGuid().ToString()
            };

            var mockIMediator = new Mock<IMediator> { CallBase = true };
            var sutMock = new Mock<CreateMatriculaDesestimadaCommonCommandHandler>(Context, mockIMediator.Object)
            { CallBase = true };

            mockIMediator.Setup(s => s.Send(It.IsAny<AddConsolidacionesRequisitosExpedienteUncommitCommand>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(Unit.Value);

            var mockExpedienteAlumno = new Mock<ExpedienteAlumno> { CallBase = true };
            mockExpedienteAlumno.SetupAllProperties();
            mockExpedienteAlumno.Setup(s =>
                s.AddTipoSituacionEstadoExpediente(It.IsAny<TipoSituacionEstado>(), It.IsAny<DateTime>()));
            mockExpedienteAlumno.Setup(s =>
                s.AddHitosConseguidos(It.IsAny<TipoHitoConseguido>(), It.IsAny<DateTime>(), It.IsAny<ExpedienteEspecializacion>()));
            mockExpedienteAlumno.Setup(s =>
                s.AddSeguimientoNoUser(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DateTime?>(),
                    It.IsAny<string>(), It.IsAny<string>()));

            var alumnoAcademico = new AlumnoAcademicoModel
            {
                Id = 1,
                Matriculas = new List<MatriculaAcademicoModel>
                    {
                        new()
                        {
                            Id = 1,
                            IdIntegracion = "1",
                            Estado = new EstadoMatriculaAcademicoModel
                            {
                                EsAlta = false
                            },
                            IdRefExpedienteAlumno = "1",
                            FechaCambioEstado = DateTime.UtcNow
                        },
                        new()
                        {
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

            mockIMediator.Setup(s =>
                    s.Send(It.IsAny<GetAlumnoByIdIntegracionQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(alumnoMatricula);

            var tipoSituacionEstado = new TipoSituacionEstado
            {
                Id = 19,
                Nombre = "Variación matrícula desestimada"
            };
            await Context.TiposSituacionEstado.AddAsync(tipoSituacionEstado);

            var tipoHitoConseguido = new TipoHitoConseguido
            {
                Id = 13,
                Nombre = "Desestimado"
            };
            await Context.TiposHitoConseguidos.AddAsync(tipoHitoConseguido);
            await Context.SaveChangesAsync();

            //ACT
            var actual = await sutMock.Object.Handle(request, CancellationToken.None);

            //ASSERT
            Assert.IsType<Unit>(actual);
            mockIMediator.Verify(
                s => s.Send(It.IsAny<GetAlumnoByIdIntegracionQuery>(), It.IsAny<CancellationToken>()), Times.Once);
            mockExpedienteAlumno.Verify(s =>
                    s.AddTipoSituacionEstadoExpediente(It.IsAny<TipoSituacionEstado>(), It.IsAny<DateTime>()),
                Times.Once);
            mockExpedienteAlumno.Verify(
                s => s.AddHitosConseguidos(It.IsAny<TipoHitoConseguido>(), It.IsAny<DateTime>(), It.IsAny<ExpedienteEspecializacion>()), Times.Once);
            mockExpedienteAlumno.Verify(s =>
                s.AddSeguimientoNoUser(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DateTime?>(),
                    It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [Fact(DisplayName = "Cuando si existe matrículas activas en las variaciones Retorna Ok")]
        public async Task Handle_SiExistenMatriculasActivas()
        {
            var request = new CreateMatriculaDesestimadaCommonCommand
            {
                AlumnoIdIntegracion = "1",
                MatriculaIdIntegracion = "1",
                UniversidadIdIntegracion = "1",
                IdVariacion = 1,
                VariacionIdIntegracion = "1",
                FechaHora = DateTime.UtcNow,
                Motivo = Guid.NewGuid().ToString(),
                Mensaje = Guid.NewGuid().ToString(),
                Origen = Guid.NewGuid().ToString()
            };

            var mockIMediator = new Mock<IMediator> { CallBase = true };
            var sutMock = new Mock<CreateMatriculaDesestimadaCommonCommandHandler>(Context, mockIMediator.Object)
            { CallBase = true };

            mockIMediator.Setup(s => s.Send(It.IsAny<AddConsolidacionesRequisitosExpedienteUncommitCommand>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(Unit.Value);

            var mockExpedienteAlumno = new Mock<ExpedienteAlumno> { CallBase = true };
            mockExpedienteAlumno.SetupAllProperties();
            mockExpedienteAlumno.Setup(s =>
                s.AddSeguimientoNoUser(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DateTime?>(),
                    It.IsAny<string>(), It.IsAny<string>()));

            var alumnoAcademico = new AlumnoAcademicoModel
            {
                Id = 1,
                Matriculas = new List<MatriculaAcademicoModel>
                    {
                        new()
                        {
                            Id = 1,
                            IdIntegracion = "1",
                            Estado = new EstadoMatriculaAcademicoModel
                            {
                                EsAlta = false
                            },
                            IdRefExpedienteAlumno = "1",
                            FechaCambioEstado = DateTime.UtcNow
                        },
                        new()
                        {
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

            mockIMediator.Setup(s =>
                    s.Send(It.IsAny<GetAlumnoByIdIntegracionQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(alumnoMatricula);

            var tipoSituacionEstado = new TipoSituacionEstado
            {
                Id = 19,
                Nombre = "Variación matrícula desestimada"
            };
            await Context.TiposSituacionEstado.AddAsync(tipoSituacionEstado);

            var tipoHitoConseguido = new TipoHitoConseguido
            {
                Id = 13,
                Nombre = "Desestimado"
            };
            await Context.TiposHitoConseguidos.AddAsync(tipoHitoConseguido);
            await Context.SaveChangesAsync();

            //ACT
            var actual = await sutMock.Object.Handle(request, CancellationToken.None);

            //ASSERT
            Assert.IsType<Unit>(actual);
            mockIMediator.Verify(s => s.Send(It.IsAny<AddConsolidacionesRequisitosExpedienteUncommitCommand>(),
                It.IsAny<CancellationToken>()), Times.Once);
            mockExpedienteAlumno.Verify(s =>
                s.AddSeguimientoNoUser(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DateTime?>(),
                    It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [Fact(DisplayName = "Cuando no existe matrículas activas en las ampliaciones Retorna Ok")]
        public async Task Handle_NoExistenMatriculaActivasAmpliaciones()
        {
            var request = new CreateMatriculaDesestimadaCommonCommand
            {
                AlumnoIdIntegracion = "1",
                MatriculaIdIntegracion = "1",
                UniversidadIdIntegracion = "1",
                IdVariacion = 1,
                VariacionIdIntegracion = "1",
                FechaHora = DateTime.UtcNow,
                Motivo = Guid.NewGuid().ToString(),
                EsAmpliacion = true,
                Mensaje = Guid.NewGuid().ToString(),
                Origen = Guid.NewGuid().ToString()
            };

            var mockIMediator = new Mock<IMediator> { CallBase = true };
            var sutMock = new Mock<CreateMatriculaDesestimadaCommonCommandHandler>(Context, mockIMediator.Object)
            { CallBase = true };

            mockIMediator.Setup(s => s.Send(It.IsAny<AddConsolidacionesRequisitosExpedienteUncommitCommand>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(Unit.Value);

            var mockExpedienteAlumno = new Mock<ExpedienteAlumno> { CallBase = true };
            mockExpedienteAlumno.SetupAllProperties();
            mockExpedienteAlumno.Setup(s =>
                s.AddTipoSituacionEstadoExpediente(It.IsAny<TipoSituacionEstado>(), It.IsAny<DateTime>()));
            mockExpedienteAlumno.Setup(s =>
                s.AddHitosConseguidos(It.IsAny<TipoHitoConseguido>(), It.IsAny<DateTime>(), It.IsAny<ExpedienteEspecializacion>()));
            mockExpedienteAlumno.Setup(s =>
                s.AddSeguimientoNoUser(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DateTime?>(),
                    It.IsAny<string>(), It.IsAny<string>()));

            var alumnoAcademico = new AlumnoAcademicoModel
            {
                Id = 1,
                Matriculas = new List<MatriculaAcademicoModel>
                    {
                        new()
                        {
                            Id = 1,
                            IdIntegracion = "1",
                            Estado = new EstadoMatriculaAcademicoModel
                            {
                                EsAlta = false
                            },
                            IdRefExpedienteAlumno = "1",
                            FechaCambioEstado = DateTime.UtcNow
                        },
                        new()
                        {
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

            mockIMediator.Setup(s =>
                    s.Send(It.IsAny<GetAlumnoByIdIntegracionQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(alumnoMatricula);
            var tipoSituacionEstadoVariacion = new TipoSituacionEstado
            {
                Id = 19,
                Nombre = "Variación matrícula desestimada"
            };
            await Context.TiposSituacionEstado.AddAsync(tipoSituacionEstadoVariacion);
            var tipoSituacionEstado = new TipoSituacionEstado
            {
                Id = 16,
                Nombre = "Ampliación matrícula desestimada"
            };
            await Context.TiposSituacionEstado.AddAsync(tipoSituacionEstado);

            var tipoHitoConseguido = new TipoHitoConseguido
            {
                Id = 13,
                Nombre = "Desestimado"
            };
            await Context.TiposHitoConseguidos.AddAsync(tipoHitoConseguido);
            await Context.SaveChangesAsync();

            //ACT
            var actual = await sutMock.Object.Handle(request, CancellationToken.None);

            //ASSERT
            Assert.IsType<Unit>(actual);
            mockIMediator.Verify(
                s => s.Send(It.IsAny<GetAlumnoByIdIntegracionQuery>(), It.IsAny<CancellationToken>()), Times.Once);
            mockExpedienteAlumno.Verify(s =>
                    s.AddTipoSituacionEstadoExpediente(It.IsAny<TipoSituacionEstado>(), It.IsAny<DateTime>()),
                Times.Once);
            mockExpedienteAlumno.Verify(
                s => s.AddHitosConseguidos(It.IsAny<TipoHitoConseguido>(), It.IsAny<DateTime>(), It.IsAny<ExpedienteEspecializacion>()), Times.Once);
            mockExpedienteAlumno.Verify(s =>
                s.AddSeguimientoNoUser(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DateTime?>(),
                    It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [Fact(DisplayName = "Cuando si existe matrículas activas en las ampliaciones Retorna Ok")]
        public async Task Handle_SiExistenMatriculasActivasAmpliaciones()
        {
            var request = new CreateMatriculaDesestimadaCommonCommand
            {
                AlumnoIdIntegracion = "1",
                MatriculaIdIntegracion = "1",
                UniversidadIdIntegracion = "1",
                IdVariacion = 1,
                VariacionIdIntegracion = "1",
                FechaHora = DateTime.UtcNow,
                Motivo = Guid.NewGuid().ToString(),
                EsAmpliacion = true,
                Mensaje = Guid.NewGuid().ToString(),
                Origen = Guid.NewGuid().ToString()
            };

            var mockIMediator = new Mock<IMediator> { CallBase = true };
            var sutMock = new Mock<CreateMatriculaDesestimadaCommonCommandHandler>(Context, mockIMediator.Object)
            { CallBase = true };

            mockIMediator.Setup(s => s.Send(It.IsAny<AddConsolidacionesRequisitosExpedienteUncommitCommand>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(Unit.Value);

            var mockExpedienteAlumno = new Mock<ExpedienteAlumno> { CallBase = true };
            mockExpedienteAlumno.SetupAllProperties();
            mockExpedienteAlumno.Setup(s =>
                s.AddSeguimientoNoUser(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DateTime?>(),
                    It.IsAny<string>(), It.IsAny<string>()));

            var alumnoAcademico = new AlumnoAcademicoModel
            {
                Id = 1,
                Matriculas = new List<MatriculaAcademicoModel>
                    {
                        new()
                        {
                            Id = 1,
                            IdIntegracion = "1",
                            Estado = new EstadoMatriculaAcademicoModel
                            {
                                EsAlta = false
                            },
                            IdRefExpedienteAlumno = "1",
                            FechaCambioEstado = DateTime.UtcNow
                        },
                        new()
                        {
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

            mockIMediator.Setup(s =>
                    s.Send(It.IsAny<GetAlumnoByIdIntegracionQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(alumnoMatricula);

            var tipoSituacionEstadoVariacion = new TipoSituacionEstado
            {
                Id = 19,
                Nombre = "Variación matrícula desestimada"
            };
            await Context.TiposSituacionEstado.AddAsync(tipoSituacionEstadoVariacion);
            var tipoSituacionEstado = new TipoSituacionEstado
            {
                Id = 16,
                Nombre = "Ampliación matrícula desestimada"
            };
            await Context.TiposSituacionEstado.AddAsync(tipoSituacionEstado);

            var tipoHitoConseguido = new TipoHitoConseguido
            {
                Id = 13,
                Nombre = "Desestimado"
            };
            await Context.TiposHitoConseguidos.AddAsync(tipoHitoConseguido);
            await Context.SaveChangesAsync();

            //ACT
            var actual = await sutMock.Object.Handle(request, CancellationToken.None);

            //ASSERT
            Assert.IsType<Unit>(actual);
            mockIMediator.Verify(s => s.Send(It.IsAny<AddConsolidacionesRequisitosExpedienteUncommitCommand>(),
                It.IsAny<CancellationToken>()), Times.Once);
            mockExpedienteAlumno.Verify(s =>
                s.AddSeguimientoNoUser(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DateTime?>(),
                    It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }
        #endregion
    }
}
