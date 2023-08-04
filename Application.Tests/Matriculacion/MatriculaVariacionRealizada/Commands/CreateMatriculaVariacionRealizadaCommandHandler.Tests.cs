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
using Unir.Expedientes.Application.Matriculacion.Common.Commands.EliminarConsolidacionRequisitosAnuladaCommon;
using Unir.Expedientes.Application.Matriculacion.Common.Queries.GetAlumnoByIdIntegracion;
using Unir.Expedientes.Application.Matriculacion.Common.Queries.GetAsignaturasAsociadasExpediente;
using Unir.Expedientes.Application.Matriculacion.MatriculaVariacionRealizada.Commands;
using Unir.Expedientes.Application.Tests.Common;
using Unir.Expedientes.Domain.Entities;
using Xunit;

namespace Unir.Expedientes.Application.Tests.Matriculacion.MatriculaVariacionRealizada.Commands
{
    [Collection("CommonTestCollection")]
    public class CreateMatriculaVariacionRealizadaCommandHandlerTest : TestBase
    {
        #region Handle
        [Fact(DisplayName = "Cuando se adiciona asignaturas y cambiar estado Abierto Retorna Ok")]
        public async Task Handle_EstadoExpedienteAbierto_Ok()
        {
            //ARRANGE
            var request = new CreateMatriculaVariacionRealizadaCommand
            {
                AlumnoIdIntegracion = "1",
                MatriculaIdIntegracion = "1",
                CausaEnumDominio = "ASIGNATURA_NO_IMPARTIDA",
                FechaHoraAlta = DateTime.UtcNow,
                IdsAsignaturasOfertadasAdicionadas = new[] { 1, 2, 3 },
                IdsAsignaturasOfertadasExcluidas = new[] { 5, 6, 7 },
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
                s.AddSeguimientoNoUser(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DateTime>(),
                    It.IsAny<string>(), It.IsAny<string>()));

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
                    SituacionAsignaturaId = SituacionAsignatura.Matriculada
                },
                new()
                {
                    Id = 2,
                    IdRefAsignaturaPlan = "2",
                    NombreAsignatura = Guid.NewGuid().ToString(),
                    CodigoAsignatura = Guid.NewGuid().ToString(),
                    OrdenAsignatura = 2,
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
                    SituacionAsignaturaId = SituacionAsignatura.Anulada
                }
            };

            var asignaturasAdicionar = new List<AsignaturaExpediente>()
            {
                new()
                {
                    Id = 3,
                    IdRefAsignaturaPlan = "3",
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
                    SituacionAsignaturaId = SituacionAsignatura.Matriculada
                }
            };

            await Context.ExpedientesAlumno.AddAsync(mockExpedienteAlumno.Object);
            await Context.SaveChangesAsync();

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

            mockIMediator.Setup(s => s.Send(It.IsAny<GetAsignaturasAsociadasQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(asignaturasAdicionar);

            mockIMediator.Setup(s => s.Send(It.IsAny<AddConsolidacionesRequisitosExpedienteUncommitCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Unit.Value);

            mockIMediator.Setup(s => s.Send(It.IsAny<EliminarConsolidacionRequisitosAnuladaCommonCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Unit.Value);

            var listaAsignaturasMatriculadas = new List<AsignaturaMatriculadaModel>
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
            mockIErpAcademicoServiceClient.Setup(x => x.GetAsignaturasMatricula(It.IsAny<int>()))
                .ReturnsAsync(listaAsignaturasMatriculadas);

            var tipoSituacionEstado = new TipoSituacionEstado
            {
                Id = 10,
                Nombre = "Variación Matriculada"
            };
            await Context.TiposSituacionEstado.AddAsync(tipoSituacionEstado);
            await Context.SaveChangesAsync();

            var mock = new Mock<CreateMatriculaVariacionRealizadaCommandHandler>(Context, mockIMediator.Object, mockIErpAcademicoServiceClient.Object)
            { CallBase = true };

            mock.Setup(x => x.AsignarSituacionAsignaturasAdicional(It.IsAny<int>(), It.IsAny<List<AsignaturaExpediente>>()))
                .Returns(Task.CompletedTask);

            //ACT
            var actual = await mock.Object.Handle(request, CancellationToken.None);

            //ASSERT
            Assert.IsType<Unit>(actual);
            mockExpedienteAlumno.Verify(s =>
                s.AddTipoSituacionEstadoExpediente(It.IsAny<TipoSituacionEstado>(), It.IsAny<DateTime>()), Times.Once);
            mockExpedienteAlumno.Verify(s =>
                s.AddSeguimientoNoUser(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DateTime>(),
                    It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            mockIMediator.Verify(s => s.Send(It.IsAny<GetAlumnoByIdIntegracionQuery>(), It.IsAny<CancellationToken>()), Times.Once);
            mockIMediator.Verify(s => s.Send(It.IsAny<GetAsignaturasAsociadasQuery>(), It.IsAny<CancellationToken>()), Times.Once);
            mockIMediator.Verify(s => s.Send(It.IsAny<AddConsolidacionesRequisitosExpedienteUncommitCommand>(),
                It.IsAny<CancellationToken>()), Times.Once);
            mockIMediator.Verify(s => s.Send(It.IsAny<EliminarConsolidacionRequisitosAnuladaCommonCommand>(), It.IsAny<CancellationToken>()), Times.Once);
            mockIErpAcademicoServiceClient.Verify(x => x.GetAsignaturasMatricula(It.IsAny<int>()), Times.Once);
            mock.Verify(x =>
                x.AsignarSituacionAsignaturasAdicional(It.IsAny<int>(), It.IsAny<List<AsignaturaExpediente>>()), Times.AtMost(2));
        }

        [Fact(DisplayName = "Cuando se cambia estado Cerrado Retorna Ok")]
        public async Task Handle_EstadoCerrado()
        {
            //ARRANGE
            var request = new CreateMatriculaVariacionRealizadaCommand
            {
                AlumnoIdIntegracion = "1",
                MatriculaIdIntegracion = "1",
                CausaEnumDominio = "ASIGNATURA_NO_IMPARTIDA",
                FechaHoraAlta = DateTime.UtcNow,
                IdsAsignaturasOfertadasAdicionadas = Array.Empty<int>(),
                IdsAsignaturasOfertadasExcluidas = new[] { 5, 6, 7 },
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
                s.AddSeguimientoNoUser(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DateTime>(),
                    It.IsAny<string>(), It.IsAny<string>()));

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
                    SituacionAsignaturaId = SituacionAsignatura.Anulada
                },
                new()
                {
                    Id = 2,
                    IdRefAsignaturaPlan = "2",
                    NombreAsignatura = Guid.NewGuid().ToString(),
                    CodigoAsignatura = Guid.NewGuid().ToString(),
                    OrdenAsignatura = 2,
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
                    SituacionAsignaturaId = SituacionAsignatura.Anulada
                }
            };

            await Context.ExpedientesAlumno.AddAsync(mockExpedienteAlumno.Object);
            await Context.SaveChangesAsync();

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

            mockIMediator.Setup(s => s.Send(It.IsAny<GetAsignaturasAsociadasQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<AsignaturaExpediente>());

            mockIMediator.Setup(s => s.Send(It.IsAny<EliminarConsolidacionRequisitosAnuladaCommonCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Unit.Value);

            var listaAsignaturasMatriculadas = new List<AsignaturaMatriculadaModel>
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
            mockIErpAcademicoServiceClient.Setup(x => x.GetAsignaturasMatricula(It.IsAny<int>()))
                .ReturnsAsync(listaAsignaturasMatriculadas);

            var tipoSituacionEstado = new TipoSituacionEstado
            {
                Id = 10,
                Nombre = "Variación matrícula"
            };
            await Context.TiposSituacionEstado.AddAsync(tipoSituacionEstado);
            await Context.SaveChangesAsync();

            var mock = new Mock<CreateMatriculaVariacionRealizadaCommandHandler>(Context, mockIMediator.Object, mockIErpAcademicoServiceClient.Object)
            { CallBase = true };

            mock.Setup(x => x.AsignarSituacionAsignaturasAdicional(It.IsAny<int>(), It.IsAny<List<AsignaturaExpediente>>()))
                .Returns(Task.CompletedTask);

            //ACT
            var actual = await mock.Object.Handle(request, CancellationToken.None);

            //ASSERT
            Assert.IsType<Unit>(actual);
            mockExpedienteAlumno.Verify(s =>
                s.AddTipoSituacionEstadoExpediente(It.IsAny<TipoSituacionEstado>(), It.IsAny<DateTime>()), Times.Once);
            mockExpedienteAlumno.Verify(s =>
                s.AddSeguimientoNoUser(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DateTime>(),
                    It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            mockIMediator.Verify(s => s.Send(It.IsAny<GetAlumnoByIdIntegracionQuery>(), It.IsAny<CancellationToken>()), Times.Once);
            mockIMediator.Verify(s => s.Send(It.IsAny<GetAsignaturasAsociadasQuery>(), It.IsAny<CancellationToken>()), Times.Once);
            mockIMediator.Verify(s => s.Send(It.IsAny<EliminarConsolidacionRequisitosAnuladaCommonCommand>(),
                It.IsAny<CancellationToken>()), Times.Once);
            mockIErpAcademicoServiceClient.Verify(x => x.GetAsignaturasMatricula(It.IsAny<int>()), Times.Once);
            mock.Verify(x => x.AsignarSituacionAsignaturasAdicional(It.IsAny<int>(), It.IsAny<List<AsignaturaExpediente>>()), Times.AtMost(2));
        }

        #endregion
    }
}
