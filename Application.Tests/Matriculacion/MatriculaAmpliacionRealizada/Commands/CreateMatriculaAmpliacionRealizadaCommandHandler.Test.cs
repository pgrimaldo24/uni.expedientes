using MediatR;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Unir.Expedientes.Application.Common.Interfaces;
using Unir.Expedientes.Application.Common.Models.ErpAcademico.Matriculacion;
using Unir.Expedientes.Application.Common.Models.ErpAcademico.OfertaEstudio;
using Unir.Expedientes.Application.Common.Models.ErpAcademico.PlanesEstudio;
using Unir.Expedientes.Application.ConsolidacionesRequisitosExpedientes.Commands.AddConsolidacionesRequisitosExpedienteUncommit;
using Unir.Expedientes.Application.Matriculacion.Common.Queries.GetAlumnoByIdIntegracion;
using Unir.Expedientes.Application.Matriculacion.Common.Queries.GetAsignaturasAsociadasExpediente;
using Unir.Expedientes.Application.Matriculacion.MatriculaAmpliacionRealizada.Commands;
using Unir.Expedientes.Application.Tests.Common;
using Unir.Expedientes.Domain.Entities;
using Xunit;

namespace Unir.Expedientes.Application.Tests.Matriculacion.MatriculaAmpliacionRealizada.Commands
{
    [Collection("CommonTestCollection")]
    public class CreateMatriculaAmpliacionRealizadaCommandHandlerTest : TestBase
    {
        #region Handle

        [Fact(DisplayName = "Cuando todo es correcto Retorna Ok")]
        public async Task Handle_Ok()
        {
            //ARRANGE
            var request = new CreateMatriculaAmpliacionRealizadaCommand
            {
                MatriculaIdIntegracion = "1",
                AlumnoIdIntegracion = "1",
                IdsAsignaturasOfertadasAdicionadas = new List<int> { 1 },
                FechaHoraAlta = DateTime.UtcNow,
                Mensaje = Guid.NewGuid().ToString(),
                Origen = Guid.NewGuid().ToString()
            };

            var mockIMediator = new Mock<IMediator> { CallBase = true };
            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient> { CallBase = true };

            var mockExpedienteAlumno = new Mock<ExpedienteAlumno> { CallBase = true };
            mockExpedienteAlumno.SetupAllProperties();
            mockExpedienteAlumno.Setup(s =>
                s.AddTipoSituacionEstadoExpediente(It.IsAny<TipoSituacionEstado>(), It.IsAny<DateTime>()));
            mockExpedienteAlumno.Setup(s =>
                s.AddSeguimientoNoUser(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DateTime?>(),
                    It.IsAny<string>(), It.IsAny<string>()));

            var sutMock = new Mock<CreateMatriculaAmpliacionRealizadaCommandHandler>(Context, mockIMediator.Object, mockIErpAcademicoServiceClient.Object)
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

            mockIMediator.Setup(s => s.Send(It.IsAny<GetAsignaturasAsociadasQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(mockExpedienteAlumno.Object.AsignaturasExpedientes);

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

            await Context.ExpedientesAlumno.AddAsync(mockExpedienteAlumno.Object);

            var tipoSituacionEstado = new TipoSituacionEstado
            {
                Id = 8,
                Nombre = "Ampliación Matricula",
                Estado = new EstadoExpediente
                {
                    Id = 2,
                    Nombre = Guid.NewGuid().ToString()
                }
            };
            await Context.TiposSituacionEstado.AddAsync(tipoSituacionEstado);
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
                s.AddSeguimientoNoUser(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DateTime?>(),
                    It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            mockIMediator.Verify(s => s.Send(It.IsAny<AddConsolidacionesRequisitosExpedienteUncommitCommand>(), It.IsAny<CancellationToken>()), Times.Once);
            mockIMediator.Verify(s => s.Send(It.IsAny<GetAsignaturasAsociadasQuery>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        #endregion
    }
}
