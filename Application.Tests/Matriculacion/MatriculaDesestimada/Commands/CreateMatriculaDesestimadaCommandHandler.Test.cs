using MediatR;
using Microsoft.Extensions.Localization;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Unir.Expedientes.Application.Common.Interfaces;
using Unir.Expedientes.Application.Common.Models.ErpAcademico.Matriculacion;
using Unir.Expedientes.Application.ConsolidacionesRequisitosExpedientes.Commands.AddConsolidacionesRequisitosExpedienteUncommit;
using Unir.Expedientes.Application.Matriculacion.Common.Queries.GetAlumnoByIdIntegracion;
using Unir.Expedientes.Application.Matriculacion.MatriculaDesestimada.Commands;
using Unir.Expedientes.Application.Tests.Common;
using Unir.Expedientes.Domain.Entities;
using Xunit;

namespace Unir.Expedientes.Application.Tests.Matriculacion.MatriculaDesestimada.Commands
{
    [Collection("CommonTestCollection")]
    public class CreateMatriculaDesestimadaCommandHandlerTest : TestBase
    {
        #region Handle

        [Fact(DisplayName = "Cuando existen matrículas activas Retorna Ok")]
        public async Task OtrasMatriculasActivas_Ok()
        {
            //ARRANGE
            var request = new CreateMatriculaDesestimadaCommand
            {
                MatriculaIdIntegracion = "1",
                AlumnoIdIntegracion = "1",
                IdCausa = 1,
                Mensaje = Guid.NewGuid().ToString(),
                Origen = Guid.NewGuid().ToString()
            };

            var mockIStringLocalizer = new Mock<IStringLocalizer<CreateMatriculaDesestimadaCommandHandler>> { CallBase = true };
            var mockIMediator = new Mock<IMediator> { CallBase = true };
            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };
            var mockExpedienteAlumno = new Mock<ExpedienteAlumno> { CallBase = true };
            mockExpedienteAlumno.SetupAllProperties();
            mockExpedienteAlumno.Setup(s =>
                s.AddSeguimientoNoUser(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DateTime>(),
                    It.IsAny<string>(), It.IsAny<string>()));

            var sutMock = new Mock<CreateMatriculaDesestimadaCommandHandler>(Context, mockIStringLocalizer.Object, mockIMediator.Object, mockIErpAcademicoServiceClient.Object)
            { CallBase = true };

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

            var causaBaja = new CausaBajaMatriculaModel
            {
                Id = 13,
                Nombre = "Desestimado"
            };

            mockIMediator.Setup(s => s.Send(It.IsAny<GetAlumnoByIdIntegracionQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(alumnoMatricula);
            mockIMediator.Setup(s => s.Send(It.IsAny<AddConsolidacionesRequisitosExpedienteUncommitCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Unit.Value);
            mockIErpAcademicoServiceClient.Setup(x => x.GetCausaBajaMatricula(It.IsAny<int>()))
                .ReturnsAsync(causaBaja);

            mockExpedienteAlumno.Object.Id = 1;
            mockExpedienteAlumno.Object.IdRefIntegracionAlumno = Guid.NewGuid().ToString();
            mockExpedienteAlumno.Object.IdRefPlan = Guid.NewGuid().ToString();
            mockExpedienteAlumno.Object.TiposSituacionEstadoExpedientes = new List<TipoSituacionEstadoExpediente>()
            {
                new()
                {
                    Id = 1,
                    Descripcion = Guid.NewGuid().ToString(),
                    FechaInicio = new DateTime(2020, 1, 1),
                    FechaFin = new DateTime(2022, 10, 1)
                },
                new()
                {
                    Id = 2,
                    Descripcion = Guid.NewGuid().ToString(),
                    FechaInicio = new DateTime(2022, 10, 1),
                    FechaFin = null
                }
            };

            await Context.ExpedientesAlumno.AddAsync(mockExpedienteAlumno.Object);
            await Context.SaveChangesAsync();

            var tipoSituacionEstado = new TipoSituacionEstado
            {
                Id = 14,
                Nombre = "Desestimada",
                Estado = new EstadoExpediente
                {
                    Id = 3,
                    Nombre = "Cerrado"
                }
            };
            await Context.TiposSituacionEstado.AddAsync(tipoSituacionEstado);
            await Context.SaveChangesAsync();

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
            mockIMediator.Verify(s => s.Send(It.IsAny<GetAlumnoByIdIntegracionQuery>(), It.IsAny<CancellationToken>()), Times.Once);
            mockExpedienteAlumno.Verify(s =>
                s.AddSeguimientoNoUser(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DateTime>(),
                    It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            mockIMediator.Verify(s => s.Send(It.IsAny<AddConsolidacionesRequisitosExpedienteUncommitCommand>(), It.IsAny<CancellationToken>()), Times.Once);
            mockIErpAcademicoServiceClient.Verify(x => x.GetCausaBajaMatricula(It.IsAny<int>()), Times.Once);
        }

        [Fact(DisplayName = "Cuando no existen matrículas activas Retorna Ok")]
        public async Task SinOtrasMatriculasActivas_Ok()
        {
            //ARRANGE
            var request = new CreateMatriculaDesestimadaCommand
            {
                MatriculaIdIntegracion = "1",
                AlumnoIdIntegracion = "1",
                IdCausa = 1,
                Mensaje = Guid.NewGuid().ToString(),
                Origen = Guid.NewGuid().ToString()
            };

            var mockIStringLocalizer = new Mock<IStringLocalizer<CreateMatriculaDesestimadaCommandHandler>> { CallBase = true };
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
                s.AddHitosConseguidos(It.IsAny<TipoHitoConseguido>(), It.IsAny<DateTime>(), It.IsAny<ExpedienteEspecializacion>()));
            mockExpedienteAlumno.Setup(s =>
                s.AddSeguimientoNoUser(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DateTime>(),
                    It.IsAny<string>(), It.IsAny<string>()));

            mockExpedienteAlumno.Object.Id = 1;
            mockExpedienteAlumno.Object.IdRefIntegracionAlumno = Guid.NewGuid().ToString();
            mockExpedienteAlumno.Object.IdRefPlan = Guid.NewGuid().ToString();
            mockExpedienteAlumno.Object.TiposSituacionEstadoExpedientes = new List<TipoSituacionEstadoExpediente>
            {
                new()
                {
                    Id = 1,
                    Descripcion = Guid.NewGuid().ToString(),
                    FechaInicio = new DateTime(2020, 1, 1),
                    FechaFin = new DateTime(2022, 10, 1)
                },
                new()
                {
                    Id = 2,
                    Descripcion = Guid.NewGuid().ToString(),
                    FechaInicio = new DateTime(2022, 10, 1),
                    FechaFin = null
                }
            };

            await Context.ExpedientesAlumno.AddAsync(mockExpedienteAlumno.Object);
            await Context.SaveChangesAsync();

            var sutMock = new Mock<CreateMatriculaDesestimadaCommandHandler>(Context, mockIStringLocalizer.Object, mockIMediator.Object, mockIErpAcademicoServiceClient.Object)
            { CallBase = true };

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
                            EsAlta = false,
                            EsRecibida = false,
                            EsPrematricula = false
                        },
                        IdRefExpedienteAlumno = "1",
                        FechaCambioEstado = DateTime.UtcNow
                    },
                    new (){
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

            var causaBaja = new CausaBajaMatriculaModel
            {
                Id = 13,
                Nombre = "Desestimado"
            };

            mockIMediator.Setup(s => s.Send(It.IsAny<GetAlumnoByIdIntegracionQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(alumnoMatricula);

            mockIErpAcademicoServiceClient.Setup(x => x.GetCausaBajaMatricula(It.IsAny<int>()))
                .ReturnsAsync(causaBaja);

            var tipoSituacionEstado = new TipoSituacionEstado
            {
                Id = 14,
                Nombre = "Desestimada",
                Estado = new EstadoExpediente
                {
                    Id = 3,
                    Nombre = "Cerrado"
                }
            };
            await Context.TiposSituacionEstado.AddAsync(tipoSituacionEstado);
            await Context.SaveChangesAsync();

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
            mockIErpAcademicoServiceClient.Verify(x => x.GetCausaBajaMatricula(It.IsAny<int>()), Times.Once);
            mockExpedienteAlumno.Verify(s =>
                s.AddTipoSituacionEstadoExpediente(It.IsAny<TipoSituacionEstado>(), It.IsAny<DateTime>()), Times.Once);
            mockExpedienteAlumno.Verify(s =>
                s.AddHitosConseguidos(It.IsAny<TipoHitoConseguido>(), It.IsAny<DateTime>(), It.IsAny<ExpedienteEspecializacion>()), Times.Once);
            mockExpedienteAlumno.Verify(s =>
                s.AddSeguimientoNoUser(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DateTime>(),
                    It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        #endregion
    }
}
