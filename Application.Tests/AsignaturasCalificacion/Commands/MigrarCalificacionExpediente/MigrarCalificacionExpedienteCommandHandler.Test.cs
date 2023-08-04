using MediatR;
using Microsoft.Extensions.Localization;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Castle.Components.DictionaryAdapter;
using Unir.Expedientes.Application.AsignaturasCalificacion.Queries.GetCalificacionByNota;
using Unir.Expedientes.Application.AsignaturasCalificacion.Commands.MigrarCalificacionExpediente;
using Unir.Expedientes.Application.AsignaturasCalificacion.Commands.RelateExpedientesAsignaturas;
using Unir.Expedientes.Application.Common.Interfaces;
using Unir.Expedientes.Application.Common.Models.ErpAcademico.OfertaEstudio;
using Unir.Expedientes.Application.Common.Models.ErpAcademico.PlanesEstudio;
using Unir.Expedientes.Application.Common.Models.ErpAcademico.Titulos;
using Unir.Expedientes.Application.Common.Models.Evaluaciones;
using Unir.Expedientes.Application.Common.Models.ExpedientesGestorUnir;
using Unir.Expedientes.Application.Common.Models.Results;
using Unir.Expedientes.Application.Matriculacion.Common.Queries.GetAsignaturasAsociadasExpediente;
using Unir.Expedientes.Application.Tests.Common;
using Unir.Expedientes.Domain.Entities;
using Unir.Framework.ApplicationSuperTypes.Exceptions;
using Xunit;
using Unir.Expedientes.Application.AsignaturasCalificacion.Queries.GetCicloPeriodoLectivo;
using Unir.Expedientes.Application.Common.Models.ErpAcademico.OfertaEstudio.Migracion;
using System.Numerics;

namespace Unir.Expedientes.Application.Tests.AsignaturasCalificacion.Commands.MigrarCalificacionExpediente
{
    [Collection("CommonTestCollection")]
    public class MigrarCalificacionExpedienteCommandHandlerTest : TestBase
    {
        #region Handle

        [Fact(DisplayName = "Cuando se migra calificaciones Retorna Ok")]
        public async Task Handle_Ok()
        {
            //ARRANGE
            var mockExpedienteAlumno = new Mock<ExpedienteAlumno>
            {
                CallBase = true
            };
            mockExpedienteAlumno.Object.Id = 1;
            mockExpedienteAlumno.Object.IdRefIntegracionAlumno = Guid.NewGuid().ToString();
            mockExpedienteAlumno.Object.IdRefPlan = "1";
            mockExpedienteAlumno.Object.FechaFinalizacion = DateTime.UtcNow.AddYears(5);
            mockExpedienteAlumno.Object.NotaMedia = 0;
            mockExpedienteAlumno.Object.Migrado = false;
            mockExpedienteAlumno.Setup(s =>
                s.AddSeguimientoNoUser(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DateTime>(),
                    It.IsAny<string>(), It.IsAny<string>()));

            await Context.ExpedientesAlumno.AddAsync(mockExpedienteAlumno.Object);
            await Context.SaveChangesAsync();

            var asignaturasOfertadasRequest = new List<AsignaturaOfertadaMigracionModel>()
            {
                new ()
                {
                    Id = 1
                }
            };

            var request = new MigrarCalificacionExpedienteCommand(mockExpedienteAlumno.Object, asignaturasOfertadasRequest);

            var mockIMediator = new Mock<IMediator> { CallBase = true };
            var mockIExpedientesGestorUnirServiceClient = new Mock<IExpedientesGestorUnirServiceClient> { CallBase = true };
            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient> { CallBase = true };
            var mockIStringLocalizer = new Mock<IStringLocalizer<MigrarCalificacionExpedienteCommandHandler>> { CallBase = true };
            var sut = new Mock<MigrarCalificacionExpedienteCommandHandler>(Context, mockIMediator.Object, mockIExpedientesGestorUnirServiceClient.Object, mockIErpAcademicoServiceClient.Object, mockIStringLocalizer.Object)
            { CallBase = true };

            var expedienteGestor = new ResultPrimitiveCustomValue<ExpedienteExpedientesIntegrationModel>
            {
                Value = new ExpedienteExpedientesIntegrationModel
                {
                    FechaFinEstudio = DateTime.UtcNow,
                    NotaMedia = 17
                }
            };
            mockIExpedientesGestorUnirServiceClient.Setup(s => s.GetExpedienteGestorFormatoErp(It.IsAny<string>(), It.IsAny<int>()))
                .ReturnsAsync(expedienteGestor);

            sut.Setup(s => s.AssignTrabajoFinEstudio(It.IsAny<ExpedienteAlumno>(), It.IsAny<ExpedienteExpedientesIntegrationModel>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
            sut.Setup(s => s.AssignEspecializacion(It.IsAny<ExpedienteAlumno>(), It.IsAny<ExpedienteExpedientesIntegrationModel>()))
                .Returns(Task.CompletedTask);
            sut.Setup(s => s.AssignTitulacion(It.IsAny<ExpedienteAlumno>(), It.IsAny<ExpedienteExpedientesIntegrationModel>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
            sut.Setup(s => s.AssignAsignaturasExpediente(It.IsAny<ExpedienteAlumno>(), It.IsAny<ExpedienteExpedientesIntegrationModel>(),
                It.IsAny<List<AsignaturaOfertadaMigracionModel>>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
            sut.Setup(s => s.GetRelacionExpedientesAsignaturas(It.IsAny<ExpedienteAlumno>(), 
                    It.IsAny<CancellationToken>())).ReturnsAsync(null as string);

            //ACT
            var actual = await sut.Object.Handle(request, CancellationToken.None);

            //ASSERT
            Assert.Null(actual);
            mockExpedienteAlumno.Verify(s =>
                s.AddSeguimientoNoUser(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DateTime>(),
                    It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            mockIExpedientesGestorUnirServiceClient.Verify(s => s.GetExpedienteGestorFormatoErp(It.IsAny<string>(), It.IsAny<int>()), Times.Once);
            sut.Verify(s => s.AssignTrabajoFinEstudio(It.IsAny<ExpedienteAlumno>(), It.IsAny<ExpedienteExpedientesIntegrationModel>(), It.IsAny<CancellationToken>()), Times.Once);
            sut.Verify(s => s.AssignEspecializacion(It.IsAny<ExpedienteAlumno>(), It.IsAny<ExpedienteExpedientesIntegrationModel>()), Times.Once);
            sut.Verify(s => s.AssignTitulacion(It.IsAny<ExpedienteAlumno>(), It.IsAny<ExpedienteExpedientesIntegrationModel>(), It.IsAny<CancellationToken>()), Times.Once);
            sut.Verify(s => s.AssignAsignaturasExpediente(It.IsAny<ExpedienteAlumno>(), It.IsAny<ExpedienteExpedientesIntegrationModel>(),
                It.IsAny<List<AsignaturaOfertadaMigracionModel>>(), It.IsAny<CancellationToken>()), Times.Once);
            sut.Verify(s => s.GetRelacionExpedientesAsignaturas(It.IsAny<ExpedienteAlumno>(),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact(DisplayName = "Cuando el servicio de gestor es inválido Retorna BadRequest")]
        public async Task Handle_Gestor_BadRequest()
        {
            //ARRANGE
            var expedienteAlumno = new ExpedienteAlumno
            {
                Id = 1,
                IdRefIntegracionAlumno = "1",
                IdRefPlan = "1"
            };
            await Context.ExpedientesAlumno.AddAsync(expedienteAlumno);
            await Context.SaveChangesAsync();
            string mensajeEsperado = $"[Expediente id:{expedienteAlumno.Id}] - El alumno con id {expedienteAlumno.IdRefIntegracionAlumno} no fue encontrado en gestor Erp";
            var asignaturasOfertadas = new List<AsignaturaOfertadaMigracionModel>
            {
                new ()
                {
                    Id = 1
                }
            };
            var request = new MigrarCalificacionExpedienteCommand(expedienteAlumno, asignaturasOfertadas);
            var mockIMediator = new Mock<IMediator> { CallBase = true };
            var mockIExpedientesGestorUnirServiceClient = new Mock<IExpedientesGestorUnirServiceClient> { CallBase = true };
            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient> { CallBase = true };
            var mockIStringLocalizer = new Mock<IStringLocalizer<MigrarCalificacionExpedienteCommandHandler>> { CallBase = true };
            mockIExpedientesGestorUnirServiceClient.Setup(s => s.GetExpedienteGestorFormatoErp(It.IsAny<string>(), It.IsAny<int>()))
                .ReturnsAsync((ResultPrimitiveCustomValue<ExpedienteExpedientesIntegrationModel>)null);
            var sut = new MigrarCalificacionExpedienteCommandHandler(Context, mockIMediator.Object, mockIExpedientesGestorUnirServiceClient.Object,
                mockIErpAcademicoServiceClient.Object, mockIStringLocalizer.Object);
            mockIStringLocalizer.Setup(s => s[It.Is<string>(msj => msj == mensajeEsperado)])
                .Returns(new LocalizedString(mensajeEsperado, mensajeEsperado));

            //ACT
            var ex = (BadRequestException)await Record.ExceptionAsync(async () =>
            {
                await sut.Handle(request, CancellationToken.None);
            });

            //ASSERT
            Assert.NotNull(ex);
            Assert.IsType<BadRequestException>(ex);
            Assert.Equal(mensajeEsperado, ex.Message);
            mockIExpedientesGestorUnirServiceClient.Verify(s =>
                s.GetExpedienteGestorFormatoErp(It.IsAny<string>(), It.IsAny<int>()), Times.Once);
        }

        [Fact(DisplayName = "Cuando la lista de asignaturas ofertadas es vacía Retorna BadRequest")]
        public async Task Handle_AsignaturasOfertadas_BadRequest()
        {
            //ARRANGE
            var expediente = new ExpedienteAlumno
            {
                Id = 1
            };
            var request = new MigrarCalificacionExpedienteCommand(expediente, new List<AsignaturaOfertadaMigracionModel>());

            var mockIMediator = new Mock<IMediator> { CallBase = true };
            var mockIExpedientesGestorUnirServiceClient = new Mock<IExpedientesGestorUnirServiceClient> { CallBase = true };
            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient> { CallBase = true };
            var mockIStringLocalizer = new Mock<IStringLocalizer<MigrarCalificacionExpedienteCommandHandler>> { CallBase = true };
            var sut = new MigrarCalificacionExpedienteCommandHandler(Context, mockIMediator.Object, mockIExpedientesGestorUnirServiceClient.Object,
                mockIErpAcademicoServiceClient.Object, mockIStringLocalizer.Object);
            string mensajeEsperado = $"El expediente con id : {expediente.Id}, no cuenta con asignaturas ofertadas";
            mockIStringLocalizer.Setup(s => s[It.Is<string>(msj => msj == mensajeEsperado)])
                .Returns(new LocalizedString(mensajeEsperado, mensajeEsperado));
            //ACT
            var ex = (BadRequestException)await Record.ExceptionAsync(async () =>
            {
                await sut.Handle(request, CancellationToken.None);
            });

            //ASSERT
            Assert.NotNull(ex);
            Assert.IsType<BadRequestException>(ex);
            Assert.Equal(mensajeEsperado, ex.Message);
        } 
        #endregion

        #region AssignTrabajoFinEstudio

        [Fact(DisplayName = "Cuando hito TrabajoFinEstudio no existe y se añade Retorna Ok")]
        public async Task TrabajoFinEstudio_hito_no_existe()
        {
            //ARRANGE
            var expediente = new ExpedienteAlumno
            {
                Id = 1
            };

            var gestor = new ExpedienteExpedientesIntegrationModel
            {
                TituloTfmTfg = "Título personalizado",
                FechaTfmTfg = DateTime.UtcNow,
            };

            await Context.TiposHitoConseguidos.AddAsync(new TipoHitoConseguido
            {
                Id = TipoHitoConseguido.TrabajoFinEstudio,
                Nombre = "Trabajo Fin Estudio"
            });
            await Context.SaveChangesAsync(CancellationToken.None);

            var mockIMediator = new Mock<IMediator> { CallBase = true };
            var mockIExpedientesGestorUnirServiceClient = new Mock<IExpedientesGestorUnirServiceClient> { CallBase = true };
            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient> { CallBase = true };
            var mockIStringLocalizer = new Mock<IStringLocalizer<MigrarCalificacionExpedienteCommandHandler>> { CallBase = true };
            var sut = new MigrarCalificacionExpedienteCommandHandler(Context, mockIMediator.Object, mockIExpedientesGestorUnirServiceClient.Object, mockIErpAcademicoServiceClient.Object, mockIStringLocalizer.Object);

            //ACT
            await sut.AssignTrabajoFinEstudio(expediente, gestor, CancellationToken.None);

            //ASSERT
            Assert.Single(expediente.HitosConseguidos);
            Assert.True(!string.IsNullOrEmpty(gestor.TituloTfmTfg));
            Assert.True(gestor.FechaTfmTfg != null);
        }

        [Fact(DisplayName = "Cuando hito TrabajoFinEstudio si existe y actualiza Retorna Ok")]
        public async Task TrabajoFinEstudio_hito_existe()
        {
            //ARRANGE
            var expediente = new ExpedienteAlumno
            {
                Id = 1,
                HitosConseguidos = new List<HitoConseguido>
                {
                    new ()
                    {
                        Id = 1,
                        FechaInicio = DateTime.UtcNow,
                        FechaFin = DateTime.UtcNow,
                        TipoConseguidoId = TipoHitoConseguido.TrabajoFinEstudio
                    }
                }
            };

            var gestor = new ExpedienteExpedientesIntegrationModel
            {
                TituloTfmTfg = "Título personalizado",
                FechaTfmTfg = DateTime.UtcNow,
            };

            await Context.TiposHitoConseguidos.AddAsync(new TipoHitoConseguido
            {
                Id = TipoHitoConseguido.TrabajoFinEstudio,
                Nombre = "Trabajo Fin Estudio"
            });
            await Context.SaveChangesAsync(CancellationToken.None);

            var mockIMediator = new Mock<IMediator> { CallBase = true };
            var mockIExpedientesGestorUnirServiceClient = new Mock<IExpedientesGestorUnirServiceClient> { CallBase = true };
            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient> { CallBase = true };
            var mockIStringLocalizer = new Mock<IStringLocalizer<MigrarCalificacionExpedienteCommandHandler>> { CallBase = true };
            var sut = new MigrarCalificacionExpedienteCommandHandler(Context, mockIMediator.Object, mockIExpedientesGestorUnirServiceClient.Object, mockIErpAcademicoServiceClient.Object, mockIStringLocalizer.Object);

            //ACT
            await sut.AssignTrabajoFinEstudio(expediente, gestor, CancellationToken.None);

            //ASSERT
            Assert.True(expediente.HitosConseguidos.First().Nombre == gestor.TituloTfmTfg);
            Assert.True(expediente.HitosConseguidos.First().FechaInicio == gestor.FechaTfmTfg);
        }

        [Fact(DisplayName = "Cuando TrabajoFinEstudio recibe el atributo titulo y fecha retornan vacíos y retorna vacío hitoConseguido")]
        public async Task Handle_AssignTrabajoFinEstudio_BadRequest()
        {
            //ARRANGE
            var expedienteAlumno = new ExpedienteAlumno();
            var expedienteIntegrationModel = new ExpedienteExpedientesIntegrationModel(); 
            var mockIMediator = new Mock<IMediator> { CallBase = true };
            var mockIExpedientesGestorUnirServiceClient = new Mock<IExpedientesGestorUnirServiceClient> { CallBase = true };
            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient> { CallBase = true };
            var mockIStringLocalizer = new Mock<IStringLocalizer<MigrarCalificacionExpedienteCommandHandler>> { CallBase = true };
            var sut = new MigrarCalificacionExpedienteCommandHandler(Context, mockIMediator.Object, mockIExpedientesGestorUnirServiceClient.Object, mockIErpAcademicoServiceClient.Object, mockIStringLocalizer.Object);

            //ACT
            await sut.AssignTrabajoFinEstudio(expedienteAlumno, expedienteIntegrationModel, CancellationToken.None);

            //ASSERT 
            Assert.True(string.IsNullOrEmpty(expedienteIntegrationModel.TituloTfmTfg));
            Assert.True(expedienteIntegrationModel.FechaTfmTfg == null);
        }

        #endregion

        #region AssignEspecializacion
        [Fact(DisplayName = "Cuando se añade expediente especializaciones Retorna Ok")]
        public async Task Especializacion_no_existe_se_agrega()
        {
            //ARRANGE
            var expedienteAlumno = new ExpedienteAlumno
            {
                Id = 1
            };

            var gestor = new ExpedienteExpedientesIntegrationModel
            {
                TituloTfmTfg = "Título personalizado",
                FechaTfmTfg = DateTime.UtcNow,
                ItinerariosFinalizados = new List<ItinerariosFinalizadosIntegrationModel>
                {
                    new ()
                    {
                        IdEspecializacionErp = "1",
                        Nombre = Guid.NewGuid().ToString()
                    },
                    new ()
                    {
                        IdEspecializacionErp = "2",
                        Nombre = Guid.NewGuid().ToString()
                    }
                }
            };

            var mockIMediator = new Mock<IMediator> { CallBase = true };
            var mockIExpedientesGestorUnirServiceClient = new Mock<IExpedientesGestorUnirServiceClient> { CallBase = true };
            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient> { CallBase = true };
            var mockIStringLocalizer = new Mock<IStringLocalizer<MigrarCalificacionExpedienteCommandHandler>> { CallBase = true };

            mockIErpAcademicoServiceClient.Setup(s => s.GetEspecializacion(It.IsAny<int>()))
                .ReturnsAsync(new EspecializacionAcademicoModel { Id = 1, Nombre = "Nombre especialización" });
            var sut = new MigrarCalificacionExpedienteCommandHandler(Context, mockIMediator.Object, mockIExpedientesGestorUnirServiceClient.Object, mockIErpAcademicoServiceClient.Object, mockIStringLocalizer.Object);

            //ACT
            await sut.AssignEspecializacion(expedienteAlumno, gestor);

            //ASSERT
            Assert.True(expedienteAlumno.HitosConseguidos.Count == gestor.ItinerariosFinalizados.Count);
            mockIErpAcademicoServiceClient.Verify(s => s.GetEspecializacion(It.IsAny<int>()), Times.AtMost(2));
        }

        [Fact(DisplayName = "Cuando no encuentra especialización en Erp Retorna BadRequest")]
        public async Task Especializacion_BadRequest()
        {
            //ARRANGE
            var expedienteAlumno = new ExpedienteAlumno();
            expedienteAlumno.Id = 1;

            var gestor = new ExpedienteExpedientesIntegrationModel
            {
                TituloTfmTfg = "Título personalizado",
                FechaTfmTfg = DateTime.UtcNow,
                ItinerariosFinalizados = new List<ItinerariosFinalizadosIntegrationModel>
                {
                    new ()
                    {
                        IdEspecializacionErp = "1",
                        Nombre = Guid.NewGuid().ToString()
                    },
                    new ()
                    {
                        IdEspecializacionErp = "2",
                        Nombre = Guid.NewGuid().ToString()
                    }
                }
            };

            string mensajeEsperado = $"[Expediente id:{expedienteAlumno.Id}] - La especialización con id:{gestor.ItinerariosFinalizados[0].IdEspecializacionErp} no fue encontrado";
            var mockIMediator = new Mock<IMediator> { CallBase = true };
            var mockIExpedientesGestorUnirServiceClient = new Mock<IExpedientesGestorUnirServiceClient> { CallBase = true };
            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient> { CallBase = true };
            var mockIStringLocalizer = new Mock<IStringLocalizer<MigrarCalificacionExpedienteCommandHandler>> { CallBase = true };
            mockIErpAcademicoServiceClient.Setup(s => s.GetEspecializacion(It.IsAny<int>()))
                .ReturnsAsync((EspecializacionAcademicoModel)null);
            mockIStringLocalizer.Setup(s => s[It.Is<string>(msj => msj == mensajeEsperado)])
                .Returns(new LocalizedString(mensajeEsperado, mensajeEsperado));
            var sut = new MigrarCalificacionExpedienteCommandHandler(Context, mockIMediator.Object, mockIExpedientesGestorUnirServiceClient.Object, mockIErpAcademicoServiceClient.Object, mockIStringLocalizer.Object);

            //ACT
            var ex = (BadRequestException)await Record.ExceptionAsync(async () =>
            {
                await sut.AssignEspecializacion(expedienteAlumno, gestor);
            });

            //ASSERT
            Assert.NotNull(ex);
            Assert.IsType<BadRequestException>(ex);
            Assert.Equal(mensajeEsperado, ex.Message);
            mockIErpAcademicoServiceClient.Verify(s => s.GetEspecializacion(It.IsAny<int>()), Times.Once);
        }

        [Fact(DisplayName = "Cuando Especializacion recibe una lista vacía de ItinerariosFinalizados y no registra expediente de finalización")]
        public async Task Handle_AssignEspecializacion_BadRequest()
        {
            //ARRANGE
            var expedienteAlumno = new ExpedienteAlumno();
            var gestor = new ExpedienteExpedientesIntegrationModel(); 
            var mockIMediator = new Mock<IMediator> { CallBase = true };
            var mockIExpedientesGestorUnirServiceClient = new Mock<IExpedientesGestorUnirServiceClient> { CallBase = true };
            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient> { CallBase = true };
            var mockIStringLocalizer = new Mock<IStringLocalizer<MigrarCalificacionExpedienteCommandHandler>> { CallBase = true }; 
            var sut = new MigrarCalificacionExpedienteCommandHandler(Context, mockIMediator.Object, mockIExpedientesGestorUnirServiceClient.Object, mockIErpAcademicoServiceClient.Object, mockIStringLocalizer.Object);
            
            //ACT
            await sut.AssignEspecializacion(expedienteAlumno, gestor); 

            //ASSERT
            Assert.True(!gestor.ItinerariosFinalizados.Any()); 
        }
        #endregion

        #region AssignTitulacion
        [Fact(DisplayName = "Cuando hito titulación no existe y se añade nuevo Retorna Ok")]
        public async Task Titulacion_hito_no_existe()
        {
            //ARRANGE
            var mockExpedienteAlumno = new Mock<ExpedienteAlumno> { CallBase = true };
            mockExpedienteAlumno.SetupAllProperties();
            mockExpedienteAlumno.Object.Id = 1;
            mockExpedienteAlumno.Setup(s => s.AddTipoSituacionEstadoExpediente(It.IsAny<TipoSituacionEstado>(), It.IsAny<DateTime>()));

            var gestor = new ExpedienteExpedientesIntegrationModel
            {
                FechaExpedicion = DateTime.UtcNow,
            };

            await Context.TiposHitoConseguidos.AddAsync(new TipoHitoConseguido
            {
                Id = TipoHitoConseguido.Titulacion,
                Nombre = "Titulación"
            });

            await Context.TiposSituacionEstado.AddAsync(new TipoSituacionEstado
            {
                Id = 22,
                Nombre = "Titulo Expedido"
            });

            await Context.SaveChangesAsync(CancellationToken.None);

            var mockIMediator = new Mock<IMediator> { CallBase = true };
            var mockIExpedientesGestorUnirServiceClient = new Mock<IExpedientesGestorUnirServiceClient> { CallBase = true };
            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient> { CallBase = true };
            var mockIStringLocalizer = new Mock<IStringLocalizer<MigrarCalificacionExpedienteCommandHandler>> { CallBase = true };
            var sut = new MigrarCalificacionExpedienteCommandHandler(Context, mockIMediator.Object, mockIExpedientesGestorUnirServiceClient.Object, mockIErpAcademicoServiceClient.Object, mockIStringLocalizer.Object);

            //ACT
            await sut.AssignTitulacion(mockExpedienteAlumno.Object, gestor, CancellationToken.None);

            //ASSERT
            Assert.Single(mockExpedienteAlumno.Object.HitosConseguidos);
            Assert.True(mockExpedienteAlumno.Object.EstadoId == EstadoExpediente.Cerrado);
            mockExpedienteAlumno.Verify(s => s.AddTipoSituacionEstadoExpediente(It.IsAny<TipoSituacionEstado>(), It.IsAny<DateTime>()), Times.Once);
        }

        [Fact(DisplayName = "Cuando hito titulación si existe y se actualiza Retorna Ok")]
        public async Task Titulacion_hito_si_existe()
        {
            var mockExpedienteAlumno = new Mock<ExpedienteAlumno> { CallBase = true };
            mockExpedienteAlumno.SetupAllProperties();
            mockExpedienteAlumno.Object.Id = 1;
            mockExpedienteAlumno.Object.HitosConseguidos = new List<HitoConseguido>
            {
                new ()
                {
                    Id = 1,
                    FechaInicio = DateTime.UtcNow,
                    FechaFin = DateTime.UtcNow,
                    TipoConseguidoId = TipoHitoConseguido.Titulacion
                }
            };
            mockExpedienteAlumno.Setup(s => s.AddTipoSituacionEstadoExpediente(It.IsAny<TipoSituacionEstado>(), It.IsAny<DateTime>()));

            var gestor = new ExpedienteExpedientesIntegrationModel
            {
                FechaExpedicion = DateTime.UtcNow,
            };

            await Context.TiposHitoConseguidos.AddAsync(new TipoHitoConseguido
            {
                Id = TipoHitoConseguido.Titulacion,
                Nombre = "Titulación"
            });

            await Context.TiposSituacionEstado.AddAsync(new TipoSituacionEstado
            {
                Id = 22,
                Nombre = "Titulo Expedido"
            });
            await Context.SaveChangesAsync(CancellationToken.None);

            var mockIMediator = new Mock<IMediator> { CallBase = true };
            var mockIExpedientesGestorUnirServiceClient = new Mock<IExpedientesGestorUnirServiceClient> { CallBase = true };
            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient> { CallBase = true };
            var mockIStringLocalizer = new Mock<IStringLocalizer<MigrarCalificacionExpedienteCommandHandler>> { CallBase = true };
            var sut = new MigrarCalificacionExpedienteCommandHandler(Context, mockIMediator.Object, mockIExpedientesGestorUnirServiceClient.Object, mockIErpAcademicoServiceClient.Object, mockIStringLocalizer.Object);

            //ACT
            await sut.AssignTitulacion(mockExpedienteAlumno.Object, gestor, CancellationToken.None);

            //ASSERT
            Assert.True(mockExpedienteAlumno.Object.EstadoId == EstadoExpediente.Cerrado);
            Assert.True(mockExpedienteAlumno.Object.HitosConseguidos.First().Nombre == "Titulación");
            Assert.True(mockExpedienteAlumno.Object.HitosConseguidos.First().FechaInicio == gestor.FechaExpedicion);
            mockExpedienteAlumno.Verify(s => s.AddTipoSituacionEstadoExpediente(It.IsAny<TipoSituacionEstado>(), It.IsAny<DateTime>()), Times.Once);
        }

        #endregion

        #region AssignAsignaturasExpediente
        [Fact(DisplayName = " Cuando no existe asignaturas con id mayor a 0 Retorna BadRequest")]
        public async Task AsignaturasGestor_invalidas()
        {
            //ARRANGE
            var gestor = new ExpedienteExpedientesIntegrationModel
            {
                Asignaturas = new List<AsignaturaErpAcademicoExpedientesIntegrationModel>
                {
                    new ()
                    {
                        IdAsignatura = -1
                    },
                    new ()
                    {
                        IdAsignatura = -2
                    }
                }
            };

            var mockIMediator = new Mock<IMediator> { CallBase = true };
            var mockIExpedientesGestorUnirServiceClient = new Mock<IExpedientesGestorUnirServiceClient> { CallBase = true };
            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient> { CallBase = true };
            var mockIStringLocalizer = new Mock<IStringLocalizer<MigrarCalificacionExpedienteCommandHandler>> { CallBase = true };
            var sut = new MigrarCalificacionExpedienteCommandHandler(Context, mockIMediator.Object, mockIExpedientesGestorUnirServiceClient.Object, mockIErpAcademicoServiceClient.Object, mockIStringLocalizer.Object);

            //ACT
            var ex = (BadRequestException)await Record.ExceptionAsync(async () =>
            {
                await sut.AssignAsignaturasExpediente(new ExpedienteAlumno(), gestor, new List<AsignaturaOfertadaMigracionModel>(), CancellationToken.None);
            });

            //ASSERT
            Assert.NotNull(ex);
            Assert.IsType<BadRequestException>(ex);
        }

        [Fact(DisplayName = "Cuando se añade asignaturas expediente y asignaturas calificación Retorna Ok")]
        public async Task AgregandoAsignaturasExpediente_And_AsignaturasCalificaciones()
        {
            //ARRANGE
            var gestor = new ExpedienteExpedientesIntegrationModel
            {
                Asignaturas = new List<AsignaturaErpAcademicoExpedientesIntegrationModel>
                {
                    new ()
                    {
                        IdAsignatura = 1,
                        NotaNumerica = -1,
                        Superado = false
                    },
                    new ()
                    {
                        IdAsignatura = 2,
                        NotaNumerica = -12,
                        Superado = false
                    },
                    new ()
                    {
                        IdAsignatura = 3,
                        Superado = true,
                        NotaAlfanumerica = "Matrícula de Honor"
                    },
                    new ()
                    {
                        IdAsignatura = 4,
                        Superado = true
                    },
                    new ()
                    {
                        IdAsignatura = 5,
                        NotaNumerica = -100,
                    }
                }
            };

            var asignaturasOfertadas = new List<AsignaturaOfertadaMigracionModel>
            {
                new ()
                {
                    Id = 1,
                    AsignaturaPlan = new AsignaturaPlanErpAcademicoModel
                    {
                        Id = 1
                    }
                },
                new ()
                {
                    Id = 2,
                    AsignaturaPlan = new AsignaturaPlanErpAcademicoModel
                    {
                        Id = 2
                    }
                }
            };

            var mockIMediator = new Mock<IMediator> { CallBase = true };
            var mockIExpedientesGestorUnirServiceClient = new Mock<IExpedientesGestorUnirServiceClient> { CallBase = true };
            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient> { CallBase = true };
            var mockIStringLocalizer = new Mock<IStringLocalizer<MigrarCalificacionExpedienteCommandHandler>> { CallBase = true };

            var asignaturasExpediente = new List<AsignaturaExpediente>
            {
                new ()
                {
                    Id = 1
                }
            };

            mockIMediator.Setup(s =>
                    s.Send(It.IsAny<GetAsignaturasAsociadasQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(asignaturasExpediente);

            var asignaturaCalificacion = new AsignaturaCalificacion
            {
                NombreCalificacion = Guid.NewGuid().ToString()
            };

            var sut = new Mock<MigrarCalificacionExpedienteCommandHandler>(Context, mockIMediator.Object, mockIExpedientesGestorUnirServiceClient.Object, mockIErpAcademicoServiceClient.Object, mockIStringLocalizer.Object)
            {
                CallBase = true
            };
            sut.Setup(s => s.AssignAsignaturasCalificaciones(It.IsAny<List<TipoConvocatoria>>(), It.IsAny<AsignaturaErpAcademicoExpedientesIntegrationModel>(), It.IsAny<AsignaturaOfertadaMigracionModel>(), It.IsAny<int>()))
                .ReturnsAsync(asignaturaCalificacion);

            //ACT
            await sut.Object.AssignAsignaturasExpediente(new ExpedienteAlumno(), gestor, asignaturasOfertadas, CancellationToken.None);

            //ASSERT
            Assert.Single(asignaturasExpediente[0].AsignaturasCalificaciones);
            mockIMediator.Verify(s =>
                s.Send(It.IsAny<GetAsignaturasAsociadasQuery>(), It.IsAny<CancellationToken>()), Times.AtLeastOnce);
            sut.Verify(s => s.AssignAsignaturasCalificaciones(It.IsAny<List<TipoConvocatoria>>(), It.IsAny<AsignaturaErpAcademicoExpedientesIntegrationModel>(),
                It.IsAny<AsignaturaOfertadaMigracionModel>(), It.IsAny<int>()), Times.AtMost(5));
        }
        [Fact(DisplayName = "Cuando se actualiza asignaturas expediente agregando asignaturas calificación Retorna Ok")]
        public async Task ActualizandoAsignaturasExpediente_And_AsignaturaCalificaciones()
        {
            //ARRANGE
            var gestor = new ExpedienteExpedientesIntegrationModel
            {
                Asignaturas = new List<AsignaturaErpAcademicoExpedientesIntegrationModel>
                {
                    new ()
                    {
                        IdAsignatura = 1,
                        NotaNumerica = -1,
                        Superado = false
                    },
                    new ()
                    {
                        IdAsignatura = 2,
                        NotaNumerica = -12,
                        Superado = false
                    },
                    new ()
                    {
                        IdAsignatura = 3,
                        Superado = true,
                        NotaAlfanumerica = "Matrícula de Honor"
                    },
                    new ()
                    {
                        IdAsignatura = 4,
                        Superado = true
                    },
                    new ()
                    {
                        IdAsignatura = 5,
                        NotaNumerica = -100,
                    }
                }
            };

            var asignaturasOfertadas = new List<AsignaturaOfertadaMigracionModel>
            {
                new ()
                    {
                        Id = 1,
                        AsignaturaPlan = new AsignaturaPlanErpAcademicoModel
                        {
                            Id = 1
                        }
                    },
                new ()
                    {
                        Id = 2,
                        AsignaturaPlan = new AsignaturaPlanErpAcademicoModel
                        {
                            Id = 2
                        }
                    }
            };

            var expediente = new ExpedienteAlumno
            {
                Id = 1,
                AsignaturasExpedientes = new List<AsignaturaExpediente>
                {
                    new () {
                        Id = 1,
                        NombreAsignatura = Guid.NewGuid().ToString(),
                        IdRefAsignaturaPlan = "1"
                    }
                }
            };
            var mockIMediator = new Mock<IMediator> { CallBase = true };
            var mockIExpedientesGestorUnirServiceClient = new Mock<IExpedientesGestorUnirServiceClient> { CallBase = true };
            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient> { CallBase = true };
            var mockIStringLocalizer = new Mock<IStringLocalizer<MigrarCalificacionExpedienteCommandHandler>> { CallBase = true };

            mockIMediator.Setup(s =>
                    s.Send(It.IsAny<GetAsignaturasAsociadasQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<AsignaturaExpediente>());

            var asignaturaCalificacion = new AsignaturaCalificacion
            {
                NombreCalificacion = Guid.NewGuid().ToString()
            };

            var sut = new Mock<MigrarCalificacionExpedienteCommandHandler>(Context, mockIMediator.Object, mockIExpedientesGestorUnirServiceClient.Object, mockIErpAcademicoServiceClient.Object, mockIStringLocalizer.Object)
            {
                CallBase = true
            };
            sut.Setup(s => s.AssignAsignaturasCalificaciones(It.IsAny<List<TipoConvocatoria>>(), It.IsAny<AsignaturaErpAcademicoExpedientesIntegrationModel>(),
                It.IsAny<AsignaturaOfertadaMigracionModel>(), It.IsAny<int>()))
                .ReturnsAsync(asignaturaCalificacion);

            //ACT
            await sut.Object.AssignAsignaturasExpediente(expediente, gestor, asignaturasOfertadas, CancellationToken.None);

            //ASSERT
            Assert.Single(expediente.AsignaturasExpedientes[0].AsignaturasCalificaciones);
            mockIMediator.Verify(s =>
                s.Send(It.IsAny<GetAsignaturasAsociadasQuery>(), It.IsAny<CancellationToken>()), Times.AtLeastOnce);
            sut.Verify(s => s.AssignAsignaturasCalificaciones(It.IsAny<List<TipoConvocatoria>>(), It.IsAny<AsignaturaErpAcademicoExpedientesIntegrationModel>(), It.IsAny<AsignaturaOfertadaMigracionModel>(), It.IsAny<int>()), Times.AtMost(5));
        }

        #endregion

        #region AssingAsignaturasCalificaciones
        [Fact(DisplayName = "Cuando se asigna calificaciones Retorna Calificación")]
        public async Task ConfiguracinoEscala_notaNumerica_Ok()
        {
            //ARRANGE
            var tiposConvocatorias = new List<TipoConvocatoria>
            {
                new ()
                {
                    Id = 1,
                    Codigo = "ORD",
                    Nombre = "Ordinaria"
                },
                new ()
                {
                    Id = 2,
                    Codigo = "EXT",
                    Nombre = "Extraordinaria"
                }
            };

            var asignaturaGestor = new AsignaturaErpAcademicoExpedientesIntegrationModel
            {
                NotaNumerica = 5,
                NotaAlfanumerica = null,
                Convocatoria = "ORD"
            };

            var asignaturaOfertadaModel = new AsignaturaOfertadaMigracionModel
            {
                Id = 1,
                PeriodoLectivo = new PeriodoLectivoModel
                {
                    Id = 1,
                    FechaInicio = DateTime.UtcNow,
                    FechaFin = DateTime.UtcNow.AddMonths(5),
                    PeriodoAcademico = new PeriodoAcademicoModel
                    {
                        Id = 1,
                        Nombre = Guid.NewGuid().ToString(),
                        FechaInicio = DateTime.UtcNow,
                        FechaFin = DateTime.UtcNow,
                        AnyoAcademico = new AnyoAcademicoModel
                        {
                            AnyoInicio = 2022,
                            AnyoFin = 2023
                        }
                    },
                    DuracionPeriodoLectivo = new DuracionPeriodoLectivoErpAcademicoModel
                    {
                        Id = 5,
                        Nombre = Guid.NewGuid().ToString(),
                        Simbolo = Guid.NewGuid().ToString()
                    }
                },
                AsignaturaMatriculadas = new EditableList<AsignaturaMatriculadaMigracionModel>
                {
                    new ()
                    {
                        Id = 1
                    }
                }
            };

            var calificacion = new CalificacionListModel
            {
                Nombre = Guid.NewGuid().ToString(),
                NotaMinima = 10,
                Orden = 1
            };

            var mockIMediator = new Mock<IMediator> { CallBase = true };
            var mockIExpedientesGestorUnirServiceClient = new Mock<IExpedientesGestorUnirServiceClient> { CallBase = true };
            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient> { CallBase = true };
            var mockIStringLocalizer = new Mock<IStringLocalizer<MigrarCalificacionExpedienteCommandHandler>> { CallBase = true };

            mockIMediator.Setup(s =>
                    s.Send(It.IsAny<GetCalificacionByNotaQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(calificacion);

            var sut = new Mock<MigrarCalificacionExpedienteCommandHandler>(Context, mockIMediator.Object,
                mockIExpedientesGestorUnirServiceClient.Object, mockIErpAcademicoServiceClient.Object,
                mockIStringLocalizer.Object)
            { CallBase = true };

            sut.Setup(s => s.GetCicloPeriodoLectivo(It.IsAny<DateTime>(), It.IsAny<int>()))
                .Returns(Task.FromResult(Guid.NewGuid().ToString()));

            //ACT
            var asignaturaCalificacion = await sut.Object.AssignAsignaturasCalificaciones(tiposConvocatorias, asignaturaGestor, asignaturaOfertadaModel, SituacionAsignatura.MatriculaHonor);

            //ASSERT
            Assert.NotNull(asignaturaCalificacion);
            Assert.True(!string.IsNullOrEmpty(asignaturaCalificacion.NombreCalificacion));
            mockIMediator.Verify(s =>
                    s.Send(It.IsAny<GetCalificacionByNotaQuery>(), It.IsAny<CancellationToken>()), Times.Once);
            sut.Verify(s => s.GetCicloPeriodoLectivo(It.IsAny<DateTime>(), It.IsAny<int>()), Times.Once);
        }
        [Fact(DisplayName = "Cuando tipo de convocatoria no existe Retorna BadRequest")]
        public async Task Convocatoria_BadRequest()
        {
            //ARRANGE
            var tiposConvocatorias = new List<TipoConvocatoria>
            {
                new ()
                {
                    Id = 1,
                    Codigo = "ORD",
                    Nombre = "Ordinaria"
                },
                new ()
                {
                    Id = 2,
                    Codigo = "EXT",
                    Nombre = "Extraordinaria"
                }
            };

            var asignaturaOfertadaMigracion = new AsignaturaOfertadaMigracionModel
            {
                AsignaturaMatriculadas = new List<AsignaturaMatriculadaMigracionModel>
                {
                    new ()
                    {
                        Id = 1,
                        IdRefCurso = Guid.NewGuid().ToString()
                    }
                }
            };

            var asignaturaGestor = new AsignaturaErpAcademicoExpedientesIntegrationModel
            {
                NotaNumerica = -10,
                NotaAlfanumerica = null,
                Convocatoria = ""
            };
            string mensajeEsperado = $"El campo Convocatoria con valor: {asignaturaGestor.Convocatoria} , no fue encontrado";

            var mockIMediator = new Mock<IMediator> { CallBase = true };
            var mockIExpedientesGestorUnirServiceClient = new Mock<IExpedientesGestorUnirServiceClient> { CallBase = true };
            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient> { CallBase = true };
            var mockIStringLocalizer = new Mock<IStringLocalizer<MigrarCalificacionExpedienteCommandHandler>> { CallBase = true };
            var sut = new MigrarCalificacionExpedienteCommandHandler(Context, mockIMediator.Object, mockIExpedientesGestorUnirServiceClient.Object,
                mockIErpAcademicoServiceClient.Object, mockIStringLocalizer.Object);

            mockIStringLocalizer.Setup(s => s[It.Is<string>(msj => msj == mensajeEsperado)])
                .Returns(new LocalizedString(mensajeEsperado, mensajeEsperado));

            //ACT
            var ex = (BadRequestException)await Record.ExceptionAsync(async () =>
            {
                await sut.AssignAsignaturasCalificaciones(tiposConvocatorias, asignaturaGestor, asignaturaOfertadaMigracion, SituacionAsignatura.Matriculada);
            });

            //ASSERT
            Assert.NotNull(ex);
            Assert.IsType<BadRequestException>(ex);
            Assert.Equal(mensajeEsperado, ex.Message);
        }
        [Fact(DisplayName = "Cuando el servicio calificación es nulo Retorna BadRequest")]
        public async Task Calificacion_BadRequest()
        {
            //ARRANGE
            var tiposConvocatorias = new List<TipoConvocatoria>
            {
                new ()
                {
                    Id = 1,
                    Codigo = "ORD",
                    Nombre = "Ordinaria"
                },
                new ()
                {
                    Id = 2,
                    Codigo = "EXT",
                    Nombre = "Extraordinaria"
                }
            };

            var asignaturaGestor = new AsignaturaErpAcademicoExpedientesIntegrationModel
            {
                NotaNumerica = 10,
                NotaAlfanumerica = null,
                Convocatoria = "Ord"
            };

            var asignaturaOfertadaModel = new AsignaturaOfertadaMigracionModel
            {
                Id = 1,
                AsignaturaMatriculadas = new List<AsignaturaMatriculadaMigracionModel>
                {
                    new ()
                    {
                        Id = 1,
                        IdRefCurso = Guid.NewGuid().ToString()
                    }
                }
            };

            string mensajeEsperado = $"La configuración escala o calificación no fue encontrado para asignatura ofertada con id : {asignaturaOfertadaModel.Id}";

            var mockIMediator = new Mock<IMediator> { CallBase = true };
            var mockIExpedientesGestorUnirServiceClient = new Mock<IExpedientesGestorUnirServiceClient> { CallBase = true };
            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient> { CallBase = true };
            var mockIStringLocalizer = new Mock<IStringLocalizer<MigrarCalificacionExpedienteCommandHandler>> { CallBase = true };
            var sut = new MigrarCalificacionExpedienteCommandHandler(Context, mockIMediator.Object, mockIExpedientesGestorUnirServiceClient.Object, mockIErpAcademicoServiceClient.Object, mockIStringLocalizer.Object);

            mockIMediator.Setup(s => s.Send(It.IsAny<GetCalificacionByNotaQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((CalificacionListModel)null);

            mockIStringLocalizer.Setup(s => s[It.Is<string>(msj => msj == mensajeEsperado)])
                .Returns(new LocalizedString(mensajeEsperado, mensajeEsperado));

            //ACT
            var ex = (BadRequestException)await Record.ExceptionAsync(async () =>
            {
                await sut.AssignAsignaturasCalificaciones(tiposConvocatorias, asignaturaGestor, asignaturaOfertadaModel, SituacionAsignatura.Matriculada);
            });

            //ASSERT
            Assert.NotNull(ex);
            Assert.IsType<BadRequestException>(ex);
            Assert.Equal(mensajeEsperado, ex.Message);
            mockIMediator.Verify(s => s.Send(It.IsAny<GetCalificacionByNotaQuery>(), It.IsAny<CancellationToken>()), Times.Once);
        }
        [Fact(DisplayName = "Cuando la lista de asignaturas matriculadas de ERP es vacía Retorna BadRequest")]
        public async Task AsignaturasMatriculadas_BadRequest()
        {
            //ARRANGE
            var tiposConvocatorias = new List<TipoConvocatoria>
            {
                new ()
                {
                    Id = 1,
                    Codigo = "ORD",
                    Nombre = "Ordinaria"
                },
                new ()
                {
                    Id = 2,
                    Codigo = "EXT",
                    Nombre = "Extraordinaria"
                }
            };

            var asignaturaGestor = new AsignaturaErpAcademicoExpedientesIntegrationModel
            {
                NotaNumerica = 5,
                NotaAlfanumerica = null,
                Convocatoria = "ORD"
            };

            var asignaturaOfertadaModel = new AsignaturaOfertadaMigracionModel
            {
                Id = 1,
                PeriodoLectivo = new PeriodoLectivoModel
                {
                    Id = 1,
                    FechaInicio = DateTime.UtcNow,
                    FechaFin = DateTime.UtcNow.AddMonths(5),
                    PeriodoAcademico = new PeriodoAcademicoModel
                    {
                        Id = 1,
                        Nombre = Guid.NewGuid().ToString(),
                        FechaInicio = DateTime.UtcNow,
                        FechaFin = DateTime.UtcNow,
                        AnyoAcademico = new AnyoAcademicoModel
                        {
                            AnyoInicio = 2022,
                            AnyoFin = 2023
                        }
                    },
                    DuracionPeriodoLectivo = new DuracionPeriodoLectivoErpAcademicoModel
                    {
                        Id = 5,
                        Nombre = Guid.NewGuid().ToString(),
                        Simbolo = Guid.NewGuid().ToString()
                    }
                },
                AsignaturaMatriculadas = new List<AsignaturaMatriculadaMigracionModel>()
            };

            var mockIMediator = new Mock<IMediator> { CallBase = true };
            var mockIExpedientesGestorUnirServiceClient = new Mock<IExpedientesGestorUnirServiceClient> { CallBase = true };
            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient> { CallBase = true };
            var mockIStringLocalizer = new Mock<IStringLocalizer<MigrarCalificacionExpedienteCommandHandler>> { CallBase = true };

            var sut = new MigrarCalificacionExpedienteCommandHandler(Context, mockIMediator.Object,
                mockIExpedientesGestorUnirServiceClient.Object, mockIErpAcademicoServiceClient.Object,
                mockIStringLocalizer.Object);

            string mensajeEsperado = $"La asignatura ofertada con id: {asignaturaOfertadaModel.Id} no contiene asignaturas matriculadas";
            mockIStringLocalizer.Setup(s => s[It.Is<string>(msj => msj == mensajeEsperado)])
                .Returns(new LocalizedString(mensajeEsperado, mensajeEsperado));

            //ACT
            var ex = (BadRequestException)await Record.ExceptionAsync(async () =>
            {
                await sut.AssignAsignaturasCalificaciones(tiposConvocatorias, asignaturaGestor, asignaturaOfertadaModel, SituacionAsignatura.Matriculada);
            });

            //ASSERT
            Assert.NotNull(ex);
            Assert.IsType<BadRequestException>(ex);
            Assert.Equal(mensajeEsperado, ex.Message);
        }
        #endregion

        #region GetCicloPeriodoLectivo
        [Fact(DisplayName = "Cuando el ciclo periodo lectivo no tiene valor Retorna BadRequest")]
        public async Task GetCicloPeriodoLectivo_BadRequest()
        {
            //ARRANGE
            string mensajeEsperado = "El campo Ciclo de periodo lectivo es vacío o nulo";
            var mockIMediator = new Mock<IMediator> { CallBase = true };
            var mockIExpedientesGestorUnirServiceClient = new Mock<IExpedientesGestorUnirServiceClient> { CallBase = true };
            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient> { CallBase = true };
            var mockIStringLocalizer = new Mock<IStringLocalizer<MigrarCalificacionExpedienteCommandHandler>> { CallBase = true };
            var sut = new MigrarCalificacionExpedienteCommandHandler(Context, mockIMediator.Object, mockIExpedientesGestorUnirServiceClient.Object, mockIErpAcademicoServiceClient.Object, mockIStringLocalizer.Object);

            mockIStringLocalizer.Setup(s => s[It.Is<string>(msj => msj == mensajeEsperado)])
                .Returns(new LocalizedString(mensajeEsperado, mensajeEsperado));

            mockIMediator.Setup(s =>
                    s.Send(It.IsAny<GetCicloPeriodoLectivoQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((string)null);

            //ACT
            var ex = (BadRequestException)await Record.ExceptionAsync(async () =>
            {
                await sut.GetCicloPeriodoLectivo(new DateTime(2023, 1, 1), AsignaturaCalificacion.IdDuracionPeriodoLectivoSemestral);
            });

            //ASSERT
            Assert.NotNull(ex);
            Assert.IsType<BadRequestException>(ex);
            Assert.Equal(mensajeEsperado, ex.Message);
            mockIMediator.Verify(s =>
                    s.Send(It.IsAny<GetCicloPeriodoLectivoQuery>(), It.IsAny<CancellationToken>()), Times.Once);
        }
        [Fact(DisplayName = "Cuando la fecha inicio e idDuracion son correctos Retorna Ciclo periodo lectivo")]
        public async Task GetCicloPeriodoLectivo_Ok()
        {
            //ARRANGE
            var mockIMediator = new Mock<IMediator> { CallBase = true };
            var mockIExpedientesGestorUnirServiceClient = new Mock<IExpedientesGestorUnirServiceClient> { CallBase = true };
            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient> { CallBase = true };
            var mockIStringLocalizer = new Mock<IStringLocalizer<MigrarCalificacionExpedienteCommandHandler>> { CallBase = true };
            var sut = new MigrarCalificacionExpedienteCommandHandler(Context, mockIMediator.Object, mockIExpedientesGestorUnirServiceClient.Object, mockIErpAcademicoServiceClient.Object, mockIStringLocalizer.Object);
            mockIMediator.Setup(s =>
                    s.Send(It.IsAny<GetCicloPeriodoLectivoQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync("2023-1");

            //ACT
            var result = await sut.GetCicloPeriodoLectivo(new DateTime(2022, 2, 1), AsignaturaCalificacion.IdDuracionPeriodoLectivoMensual);

            //ASSERT
            Assert.NotNull(result);
            Assert.Equal("2023-1", result);
        }

        #endregion

        #region AssignAsignaturaOfertada
        [Fact(DisplayName = "Cuando se setea los datos de Asignatura Ofertada Migración a Ofertada Retorna Asignatura Ofertada")]
        public void AssignAsignaturaOfertada_Ok()
        {
            //ARRANGE
            var request = new AsignaturaOfertadaMigracionModel
            {
                Id = 1,
                Orden = 10,
                TipoAsignatura = new TipoAsignaturaErpAcademicoModel
                {
                    Id = 100,
                    Simbolo = "UNIR"
                },
                AsignaturaPlan = new AsignaturaPlanErpAcademicoModel
                {
                    Id = 1,
                    Asignatura = new AsignaturaErpAcademicoModel(),
                    DisplayName = Guid.NewGuid().ToString(),
                    Orden = 200
                },
                PeriodoLectivo = new PeriodoLectivoModel
                {
                    Id = 1,
                    Nombre = "Periodo",
                    FechaInicio = new DateTime(2023, 1, 1),
                    FechaFin = DateTime.UtcNow
                },
                Curso = new CursoErpAcademicoModel
                {
                    Id = 900,
                    Numero = 100
                }
            };

            var mockIMediator = new Mock<IMediator> { CallBase = true };
            var mockIExpedientesGestorUnirServiceClient = new Mock<IExpedientesGestorUnirServiceClient> { CallBase = true };
            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient> { CallBase = true };
            var mockIStringLocalizer = new Mock<IStringLocalizer<MigrarCalificacionExpedienteCommandHandler>> { CallBase = true };
            var sut = new MigrarCalificacionExpedienteCommandHandler(Context, mockIMediator.Object, mockIExpedientesGestorUnirServiceClient.Object,
                mockIErpAcademicoServiceClient.Object, mockIStringLocalizer.Object);

            //ACT
            var act = sut.AssignAsignaturaOfertada(request);

            //ASSERT
            Assert.NotNull(act);
            Assert.Equal(request.Id, act.Id);
            Assert.Equal(request.Orden, act.Orden);
            Assert.Equal(request.TipoAsignatura.Id, act.TipoAsignatura.Id);
            Assert.Equal(request.TipoAsignatura.Simbolo, act.TipoAsignatura.Simbolo);
            Assert.Equal(request.PeriodoLectivo.Nombre, act.PeriodoLectivo.Nombre);
            Assert.Equal(request.Curso.Id, act.Curso.Id);
            Assert.Equal(request.Curso.Numero, act.Curso.Numero);
        }

        #endregion

        #region GetRelacionExpedientesAsignaturas

        [Fact(DisplayName = "Cuando al relacionar las asignaturas y expedientes duvuelve lista de mensajes Retorna mensaje")]
        public async Task GetRelacionExpedientesAsignaturas_Error()
        {
            //ARRANGE
            var mockIMediator = new Mock<IMediator> { CallBase = true };
            var sut = new MigrarCalificacionExpedienteCommandHandler(Context, mockIMediator.Object, null, null, null);
            var expedienteAlumno = new ExpedienteAlumno();

            var messages = new List<string>
            {
                Guid.NewGuid().ToString(),
                Guid.NewGuid().ToString()
            };
            mockIMediator.Setup(s => s.Send(It.IsAny<RelateExpedientesAsignaturasCommand>(), 
                    It.IsAny<CancellationToken>())).ReturnsAsync(messages);

            //ACT
            var actual = await sut.GetRelacionExpedientesAsignaturas(expedienteAlumno, CancellationToken.None);

            //ASSERT
            Assert.Contains(messages.First(), actual);
            Assert.Contains(messages.Last(), actual);
            mockIMediator.Verify(s => s.Send(It.IsAny<RelateExpedientesAsignaturasCommand>(),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact(DisplayName = "Cuando al relacionar las asignaturas y expedientes no duvuelve lista de mensajes Retorna Ok")]
        public async Task GetRelacionExpedientesAsignaturas_Ok()
        {
            //ARRANGE
            var mockIMediator = new Mock<IMediator> { CallBase = true };
            var sut = new MigrarCalificacionExpedienteCommandHandler(Context, mockIMediator.Object, null, null, null);
            var expedienteAlumno = new ExpedienteAlumno();
            mockIMediator.Setup(s => s.Send(It.IsAny<RelateExpedientesAsignaturasCommand>(), 
                    It.IsAny<CancellationToken>())).ReturnsAsync(new List<string>());

            //ACT
            var actual = await sut.GetRelacionExpedientesAsignaturas(expedienteAlumno, CancellationToken.None);

            //ASSERT
            Assert.Null(actual);
            mockIMediator.Verify(s => s.Send(It.IsAny<RelateExpedientesAsignaturasCommand>(),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        #endregion

    }
}
