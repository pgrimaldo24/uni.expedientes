using MediatR;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Unir.Expedientes.Application.Common.Models.ErpAcademico.Matriculacion;
using Unir.Expedientes.Application.ConsolidacionesRequisitosExpedientes.Commands.AddConsolidacionesRequisitosExpedienteUncommit;
using Unir.Expedientes.Application.Matriculacion.Common.Commands.MatriculaReiniciadaCommon;
using Unir.Expedientes.Application.Matriculacion.Common.Queries.GetAlumnoByIdIntegracion;
using Unir.Expedientes.Application.Tests.Common;
using Unir.Expedientes.Domain.Entities;
using Xunit;

namespace Unir.Expedientes.Application.Tests.Matriculacion.Common.Commands.MatriculaReiniciadaCommon
{
    [Collection("CommonTestCollection")]
    public class CreateMatriculaReiniciadaCommonCommandHandlerTest : TestBase
    {
        #region Handle
        [Fact(DisplayName = "Cuando Matrícula Reiniciada es correcto Retorna Ok")]
        public async Task Handle_MatriculaReiniciada_Ok()
        {
            //ARRANGE
            var request = new CreateMatriculaReiniciadaCommonCommand
            {
                MatriculaIdIntegracion = "1",
                AlumnoIdIntegracion = "1",
                FechaHora = DateTime.UtcNow,
                TipoSituacionId = TipoSituacionEstado.CanceladaDesestimacionMatricula,
                MensajeSeguimiento = "Desestimar matrícula cancelado,{0}",
                Mensaje = Guid.NewGuid().ToString(),
                Origen = Guid.NewGuid().ToString()
            };

            var mockIMediator = new Mock<IMediator> { CallBase = true };
            var mockExpedienteAlumno = new Mock<ExpedienteAlumno> { CallBase = true };
            mockExpedienteAlumno.SetupAllProperties();
            mockExpedienteAlumno.Setup(s =>
                s.AddTipoSituacionEstadoExpediente(It.IsAny<TipoSituacionEstado>(), It.IsAny<DateTime>()));
            mockExpedienteAlumno.Setup(s =>
                s.AddSeguimientoNoUser(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DateTime>(),
                    It.IsAny<string>(), It.IsAny<string>()));

            var sutMock = new Mock<CreateMatriculaReiniciadaCommonCommandHandler>(Context, mockIMediator.Object)
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

            var alumnoMatricula = new AlumnoMatricula
            {
                ExpedienteAlumno = mockExpedienteAlumno.Object,
                AlumnoAcademicoModel = alumnoAcademico,
                MatriculaAcademicoModel = alumnoAcademico.Matriculas.First()
            };

            var hitoConseguido = new HitoConseguido
            {
                Id = 1,
                Nombre = "Desestimado",
                FechaInicio = DateTime.UtcNow,
                TipoConseguido = new TipoHitoConseguido
                {
                    Id = 13,
                    Nombre = "Desestimado"
                },
                ExpedienteAlumnoId = mockExpedienteAlumno.Object.Id
            };
            await Context.HitosConseguidos.AddAsync(hitoConseguido);

            mockIMediator.Setup(s => s.Send(It.IsAny<GetAlumnoByIdIntegracionQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(alumnoMatricula);

            mockIMediator.Setup(s => s.Send(It.IsAny<AddConsolidacionesRequisitosExpedienteUncommitCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Unit.Value);

            mockExpedienteAlumno.Object.Id = 1;
            mockExpedienteAlumno.Object.IdRefIntegracionAlumno = Guid.NewGuid().ToString();
            mockExpedienteAlumno.Object.IdRefPlan = Guid.NewGuid().ToString();
            mockExpedienteAlumno.Object.HitosConseguidos.Add(hitoConseguido);

            var tipoSituacionEstado = new TipoSituacionEstado
            {
                Id = 2,
                Nombre = "Cancelada desestimación de matrícula",
                Estado = new EstadoExpediente
                {
                    Id = 1,
                    Nombre = "Inicial"
                }
            };
            await Context.TiposSituacionEstado.AddAsync(tipoSituacionEstado);
            await Context.SaveChangesAsync();

            //ACT
            var actual = await sutMock.Object.Handle(request, CancellationToken.None);

            //ASSERT
            Assert.IsType<Unit>(actual);
            mockIMediator.Verify(s => s.Send(It.IsAny<GetAlumnoByIdIntegracionQuery>(), It.IsAny<CancellationToken>()), Times.Once);
            mockExpedienteAlumno.Verify(s =>
                s.AddTipoSituacionEstadoExpediente(It.IsAny<TipoSituacionEstado>(), It.IsAny<DateTime>()), Times.Once);
            mockExpedienteAlumno.Verify(s =>
                s.AddSeguimientoNoUser(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DateTime>(),
                    It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            mockIMediator.Verify(s => s.Send(It.IsAny<AddConsolidacionesRequisitosExpedienteUncommitCommand>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact(DisplayName = "Cuando Matrícula Ampliación Reiniciada es correcto Retorna Ok")]
        public async Task Handle_MatriculaAmpliacionReiniciada_Ok()
        {
            //ARRANGE
            var request = new CreateMatriculaReiniciadaCommonCommand
            {
                MatriculaIdIntegracion = "1",
                AlumnoIdIntegracion = "1",
                FechaHora = DateTime.UtcNow,
                TipoSituacionId = TipoSituacionEstado.CanceladaDesestimacionAmpliacionMatricula,
                MensajeSeguimiento = "Desestimar ampliación matrícula cancelado,{0}",
                Mensaje = Guid.NewGuid().ToString(),
                Origen = Guid.NewGuid().ToString()
            };

            var mockIMediator = new Mock<IMediator> { CallBase = true };
            var mockExpedienteAlumno = new Mock<ExpedienteAlumno> { CallBase = true };
            mockExpedienteAlumno.SetupAllProperties();
            mockExpedienteAlumno.Setup(s =>
                s.AddTipoSituacionEstadoExpediente(It.IsAny<TipoSituacionEstado>(), It.IsAny<DateTime>()));
            mockExpedienteAlumno.Setup(s =>
                s.AddSeguimientoNoUser(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DateTime>(),
                    It.IsAny<string>(), It.IsAny<string>()));

            var sutMock = new Mock<CreateMatriculaReiniciadaCommonCommandHandler>(Context, mockIMediator.Object)
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

            var alumnoMatricula = new AlumnoMatricula
            {
                ExpedienteAlumno = mockExpedienteAlumno.Object,
                AlumnoAcademicoModel = alumnoAcademico,
                MatriculaAcademicoModel = alumnoAcademico.Matriculas.First()
            };

            var hitoConseguido = new HitoConseguido
            {
                Id = 1,
                Nombre = "Desestimado",
                FechaInicio = DateTime.UtcNow,
                TipoConseguido = new TipoHitoConseguido
                {
                    Id = 13,
                    Nombre = "Desestimado"
                },
                ExpedienteAlumnoId = mockExpedienteAlumno.Object.Id
            };
            await Context.HitosConseguidos.AddAsync(hitoConseguido);

            mockIMediator.Setup(s => s.Send(It.IsAny<GetAlumnoByIdIntegracionQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(alumnoMatricula);

            mockIMediator.Setup(s => s.Send(It.IsAny<AddConsolidacionesRequisitosExpedienteUncommitCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Unit.Value);

            mockExpedienteAlumno.Object.Id = 1;
            mockExpedienteAlumno.Object.IdRefIntegracionAlumno = Guid.NewGuid().ToString();
            mockExpedienteAlumno.Object.IdRefPlan = Guid.NewGuid().ToString();
            mockExpedienteAlumno.Object.HitosConseguidos.Add(hitoConseguido);

            var tipoSituacionEstado = new TipoSituacionEstado
            {
                Id = 3,
                Nombre = "Cancelada desestimación de ampliación de matrícula",
                Estado = new EstadoExpediente
                {
                    Id = 1,
                    Nombre = "Inicial"
                }
            };
            await Context.TiposSituacionEstado.AddAsync(tipoSituacionEstado);
            await Context.SaveChangesAsync();

            //ACT
            var actual = await sutMock.Object.Handle(request, CancellationToken.None);

            //ASSERT
            Assert.IsType<Unit>(actual);
            mockIMediator.Verify(s => s.Send(It.IsAny<GetAlumnoByIdIntegracionQuery>(), It.IsAny<CancellationToken>()), Times.Once);
            mockExpedienteAlumno.Verify(s =>
                s.AddTipoSituacionEstadoExpediente(It.IsAny<TipoSituacionEstado>(), It.IsAny<DateTime>()), Times.Once);
            mockExpedienteAlumno.Verify(s =>
                s.AddSeguimientoNoUser(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DateTime>(),
                    It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            mockIMediator.Verify(s => s.Send(It.IsAny<AddConsolidacionesRequisitosExpedienteUncommitCommand>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact(DisplayName = "Cuando Matrícula Variación Reiniciada es correcto Retorna Ok")]
        public async Task Handle_MatriculaVariacionReiniciada_Ok()
        {
            //ARRANGE
            var request = new CreateMatriculaReiniciadaCommonCommand
            {
                MatriculaIdIntegracion = "1",
                AlumnoIdIntegracion = "1",
                FechaHora = DateTime.UtcNow,
                TipoSituacionId = TipoSituacionEstado.CanceladaDesestimacionVariacionMatricula,
                MensajeSeguimiento = "Desestimar variación matrícula cancelado,{0}",
                Mensaje = Guid.NewGuid().ToString(),
                Origen = Guid.NewGuid().ToString()
            };

            var mockIMediator = new Mock<IMediator> { CallBase = true };
            var mockExpedienteAlumno = new Mock<ExpedienteAlumno> { CallBase = true };
            mockExpedienteAlumno.SetupAllProperties();
            mockExpedienteAlumno.Setup(s =>
                s.AddTipoSituacionEstadoExpediente(It.IsAny<TipoSituacionEstado>(), It.IsAny<DateTime>()));
            mockExpedienteAlumno.Setup(s =>
                s.AddSeguimientoNoUser(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DateTime>(),
                    It.IsAny<string>(), It.IsAny<string>()));

            var sutMock = new Mock<CreateMatriculaReiniciadaCommonCommandHandler>(Context, mockIMediator.Object)
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

            var alumnoMatricula = new AlumnoMatricula
            {
                ExpedienteAlumno = mockExpedienteAlumno.Object,
                AlumnoAcademicoModel = alumnoAcademico,
                MatriculaAcademicoModel = alumnoAcademico.Matriculas.First()
            };

            var hitoConseguido = new HitoConseguido
            {
                Id = 1,
                Nombre = "Desestimado",
                FechaInicio = DateTime.UtcNow,
                TipoConseguido = new TipoHitoConseguido
                {
                    Id = 13,
                    Nombre = "Desestimado"
                },
                ExpedienteAlumnoId = mockExpedienteAlumno.Object.Id
            };
            await Context.HitosConseguidos.AddAsync(hitoConseguido);

            mockIMediator.Setup(s => s.Send(It.IsAny<GetAlumnoByIdIntegracionQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(alumnoMatricula);

            mockIMediator.Setup(s => s.Send(It.IsAny<AddConsolidacionesRequisitosExpedienteUncommitCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Unit.Value);

            mockExpedienteAlumno.Object.Id = 1;
            mockExpedienteAlumno.Object.IdRefIntegracionAlumno = Guid.NewGuid().ToString();
            mockExpedienteAlumno.Object.IdRefPlan = Guid.NewGuid().ToString();
            mockExpedienteAlumno.Object.HitosConseguidos.Add(hitoConseguido);

            var tipoSituacionEstado = new TipoSituacionEstado
            {
                Id = 4,
                Nombre = "Cancelada desestimación de variación de matrícula",
                Estado = new EstadoExpediente
                {
                    Id = 1,
                    Nombre = "Inicial"
                }
            };
            await Context.TiposSituacionEstado.AddAsync(tipoSituacionEstado);
            await Context.SaveChangesAsync();

            //ACT
            var actual = await sutMock.Object.Handle(request, CancellationToken.None);

            //ASSERT
            Assert.IsType<Unit>(actual);
            mockIMediator.Verify(s => s.Send(It.IsAny<GetAlumnoByIdIntegracionQuery>(), It.IsAny<CancellationToken>()), Times.Once);
            mockExpedienteAlumno.Verify(s =>
                s.AddTipoSituacionEstadoExpediente(It.IsAny<TipoSituacionEstado>(), It.IsAny<DateTime>()), Times.Once);
            mockExpedienteAlumno.Verify(s =>
                s.AddSeguimientoNoUser(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DateTime>(),
                    It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            mockIMediator.Verify(s => s.Send(It.IsAny<AddConsolidacionesRequisitosExpedienteUncommitCommand>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        #endregion
    }
}
