using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Unir.Expedientes.Application.Common.Interfaces;
using Unir.Expedientes.Application.ConsolidacionesRequisitosExpedientes.Commands.AddConsolidacionesRequisitosExpedienteUncommit;
using Unir.Expedientes.Application.ExpedientesAlumnos.Commands.EditExpedienteAlumnoPorIntegracion;
using Unir.Expedientes.Application.ExpedientesAlumnos.Queries.HasPrimeraMatriculaExpediente;
using Unir.Expedientes.Application.SeguimientosExpedientes.Commands.AddSeguimientoTitulacionAccesoUncommit;
using Unir.Expedientes.Application.Tests.Common;
using Unir.Expedientes.Domain.Entities;
using Unir.Framework.ApplicationSuperTypes.Exceptions;
using Xunit;
using TitulacionAccesoParametersDto = Unir.Expedientes.Application.ExpedientesAlumnos.Commands.EditExpedienteAlumnoPorIntegracion.TitulacionAccesoParametersDto;

namespace Unir.Expedientes.Application.Tests.ExpedientesAlumnos.Commands.EditExpedienteAlumnoPorIntegracion
{
    [Collection("CommonTestCollection")]
    public class EditExpedienteAlumnoCommandHandlerTest : TestBase
    {
        #region Handle

        [Fact(DisplayName = "Cuando se edita pero la versión no es modificada Devuelve dto")]
        public async Task Handle_SinModificaciones_Ok()
        {
            //ARRANGE
            var expedienteAlumno = new ExpedienteAlumno
            {
                Id = 1,
                IdRefNodo = "456",
                IdRefVersionPlan = "123"
            };
            await Context.ExpedientesAlumno.AddAsync(expedienteAlumno);
            await Context.SaveChangesAsync(CancellationToken.None);
            var request = new EditExpedienteAlumnoPorIntegracionCommand
            {
                IdExpedienteAlumno = 1,
                IdRefNodo = "123",
                IdRefVersionPlan = "123",
                PorIntegracion = true
            };
            var mockIStringLocalizer = new Mock<IStringLocalizer<EditExpedienteAlumnoPorIntegracionCommandHandler>>
            {
                CallBase = true
            };
            var mockIMediator = new Mock<IMediator>
            {
                CallBase = true
            };
            mockIMediator.Setup(s => s.Send(It.IsAny<ExpedienteAlumno>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Unit.Value);
            mockIMediator.Setup(x => x.Send(It.IsAny<AddConsolidacionesRequisitosExpedienteUncommitCommand>(), 
                It.IsAny<CancellationToken>())).ReturnsAsync(Unit.Value);
            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };
            var sut = new Mock<EditExpedienteAlumnoPorIntegracionCommandHandler>(Context, mockIStringLocalizer.Object, 
                mockIMediator.Object, mockIErpAcademicoServiceClient.Object)
            {
                CallBase = true
            };
            sut.Setup(s => s.ValidatePropiedadesRequeridas(It.IsAny<EditExpedienteAlumnoPorIntegracionCommand>()));
            sut.Setup(s => s.AssignExpedienteAlumno(It.IsAny<EditExpedienteAlumnoPorIntegracionCommand>(),
                It.IsAny<ExpedienteAlumno>()));
            sut.Setup(s => s.AddEspecializaciones(It.IsAny<ExpedienteAlumno>(),
                It.IsAny<List<ExpedienteEspecializacionDto>>()));
            sut.Setup(s =>
                s.AddSeguimientoTitulacionAcceso(It.IsAny<ExpedienteAlumno>(), It.IsAny<EditExpedienteAlumnoPorIntegracionCommand>(), 
                    It.IsAny<CancellationToken>())).ReturnsAsync(false);
            sut.Setup(s => s.AssignVersionPlan(It.IsAny<ExpedienteAlumno>(),
                    It.IsAny<EditExpedienteAlumnoPorIntegracionCommand>()))
                .Returns(false);
            sut.Setup(s => s.AssignViaAcceso(It.IsAny<ExpedienteAlumno>(),
                    It.IsAny<EditExpedienteAlumnoPorIntegracionCommand>()))
                .Returns(false);
            sut.Setup(s => s.AddSeguimientoTitulacionAcceso(It.IsAny<ExpedienteAlumno>(),
                    It.IsAny<EditExpedienteAlumnoPorIntegracionCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);
            sut.Setup(x => x.AddSeguimientoExpediente(It.IsAny<ExpedienteAlumno>(),
                It.IsAny<EditExpedienteAlumnoPorIntegracionCommand>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            //ACT
            var actual = await sut.Object.Handle(request, CancellationToken.None);

            //ASSERT
            var expedientePersistido = await Context.ExpedientesAlumno.FirstAsync(CancellationToken.None);
            sut.Verify(s => s.AssignExpedienteAlumno(It.IsAny<EditExpedienteAlumnoPorIntegracionCommand>(),
                It.IsAny<ExpedienteAlumno>()), Times.Once);
            sut.Verify(s => s.AddEspecializaciones(It.IsAny<ExpedienteAlumno>(),
                It.IsAny<List<ExpedienteEspecializacionDto>>()), Times.Once);
            sut.Verify(s =>
                s.AddSeguimientoTitulacionAcceso(It.IsAny<ExpedienteAlumno>(), It.IsAny<EditExpedienteAlumnoPorIntegracionCommand>(),
                    It.IsAny<CancellationToken>()), Times.Once);
            sut.Verify(
                s => s.AddSeguimientoVersionPlan(It.IsAny<ExpedienteAlumno>(),
                    It.IsAny<EditExpedienteAlumnoPorIntegracionCommand>()), Times.Never);
            sut.Verify(
                s => s.AddSeguimientoViaAcceso(It.IsAny<ExpedienteAlumno>(),
                    It.IsAny<EditExpedienteAlumnoPorIntegracionCommand>()), Times.Never);
            sut.Verify(x => x.AddSeguimientoExpediente(It.IsAny<ExpedienteAlumno>(),
                It.IsAny<EditExpedienteAlumnoPorIntegracionCommand>(), It.IsAny<CancellationToken>()), Times.Once);
            mockIMediator.Verify(x => x.Send(It.IsAny<AddConsolidacionesRequisitosExpedienteUncommitCommand>(),
                It.IsAny<CancellationToken>()), Times.Once);
            Assert.NotEqual(expedientePersistido.IdRefNodo, request.IdRefNodo);
            Assert.Equal(expedientePersistido.IdRefVersionPlan, request.IdRefVersionPlan);
            Assert.Equal(expedientePersistido.Id, actual.Id);
            Assert.False(actual.HasAddedSeguimientos);
        }

        [Fact(DisplayName = "Cuando se edita la versión se añade un seguimiento y Devuelve dto")]
        public async Task Handle_VersionPlanMasSeguimiento_Ok()
        {
            //ARRANGE
            var expedienteAlumno = new ExpedienteAlumno
            {
                Id = 1,
                IdRefNodo = "456",
                IdRefVersionPlan = "123"
            };
            await Context.ExpedientesAlumno.AddAsync(expedienteAlumno);
            await Context.SaveChangesAsync(CancellationToken.None);
            var request = new EditExpedienteAlumnoPorIntegracionCommand
            {
                IdExpedienteAlumno = 1,
                IdRefNodo = "123",
                IdRefVersionPlan = "789"
            };
            var mockIStringLocalizer = new Mock<IStringLocalizer<EditExpedienteAlumnoPorIntegracionCommandHandler>>
            {
                CallBase = true
            };
            var mockIMediator = new Mock<IMediator>
            {
                CallBase = true
            };
            mockIMediator.Setup(s => s.Send(It.IsAny<ExpedienteAlumno>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Unit.Value);
            mockIMediator.Setup(x => x.Send(It.IsAny<AddConsolidacionesRequisitosExpedienteUncommitCommand>(),
               It.IsAny<CancellationToken>())).ReturnsAsync(Unit.Value);
            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };
            var sut = new Mock<EditExpedienteAlumnoPorIntegracionCommandHandler>(Context, mockIStringLocalizer.Object, 
                mockIMediator.Object, mockIErpAcademicoServiceClient.Object)
            {
                CallBase = true
            };
            sut.Setup(s => s.ValidatePropiedadesRequeridas(It.IsAny<EditExpedienteAlumnoPorIntegracionCommand>()));
            sut.Setup(s => s.AssignExpedienteAlumno(It.IsAny<EditExpedienteAlumnoPorIntegracionCommand>(),
                It.IsAny<ExpedienteAlumno>()));
            sut.Setup(s => s.AddEspecializaciones(It.IsAny<ExpedienteAlumno>(),
                It.IsAny<List<ExpedienteEspecializacionDto>>()));
            sut.Setup(s =>
                s.AddSeguimientoTitulacionAcceso(It.IsAny<ExpedienteAlumno>(), It.IsAny<EditExpedienteAlumnoPorIntegracionCommand>(),
                    It.IsAny<CancellationToken>())).ReturnsAsync(false);
            sut.Setup(s => s.AssignVersionPlan( It.IsAny<ExpedienteAlumno>(),
                    It.IsAny<EditExpedienteAlumnoPorIntegracionCommand>()))
                .Returns(true);
            sut.Setup(s => s.AddSeguimientoVersionPlan(It.IsAny<ExpedienteAlumno>(),
                It.IsAny<EditExpedienteAlumnoPorIntegracionCommand>()));
            sut.Setup(s => s.AssignViaAcceso(It.IsAny<ExpedienteAlumno>(),
                    It.IsAny<EditExpedienteAlumnoPorIntegracionCommand>()))
                .Returns(false);
            sut.Setup(x => x.AddSeguimientoExpediente(It.IsAny<ExpedienteAlumno>(),
                It.IsAny<EditExpedienteAlumnoPorIntegracionCommand>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            //ACT
            var actual = await sut.Object.Handle(request, CancellationToken.None);

            //ASSERT
            var expedientePersistido = await Context.ExpedientesAlumno.FirstAsync(CancellationToken.None);
            Assert.NotEqual(expedientePersistido.IdRefNodo, request.IdRefNodo);
            sut.Verify(s => s.AssignExpedienteAlumno(It.IsAny<EditExpedienteAlumnoPorIntegracionCommand>(),
                It.IsAny<ExpedienteAlumno>()), Times.Once);
            sut.Verify(s => s.AddEspecializaciones(It.IsAny<ExpedienteAlumno>(),
                It.IsAny<List<ExpedienteEspecializacionDto>>()), Times.Once);
            sut.Verify(s =>
                s.AddSeguimientoTitulacionAcceso(It.IsAny<ExpedienteAlumno>(), It.IsAny<EditExpedienteAlumnoPorIntegracionCommand>(),
                    It.IsAny<CancellationToken>()), Times.Once);
            sut.Verify(s => s.AssignVersionPlan(It.IsAny<ExpedienteAlumno>(),
                It.IsAny<EditExpedienteAlumnoPorIntegracionCommand>()), Times.Once);
            sut.Verify(s => s.AddSeguimientoVersionPlan(It.IsAny<ExpedienteAlumno>(),
                It.IsAny<EditExpedienteAlumnoPorIntegracionCommand>()), Times.Once);
            sut.Verify(s => s.AssignViaAcceso(It.IsAny<ExpedienteAlumno>(),
                It.IsAny<EditExpedienteAlumnoPorIntegracionCommand>()), Times.Once);
            sut.Verify(
                s => s.AddSeguimientoViaAcceso(It.IsAny<ExpedienteAlumno>(),
                    It.IsAny<EditExpedienteAlumnoPorIntegracionCommand>()), Times.Never);
            sut.Verify(x => x.AddSeguimientoExpediente(It.IsAny<ExpedienteAlumno>(),
                It.IsAny<EditExpedienteAlumnoPorIntegracionCommand>(), It.IsAny<CancellationToken>()), Times.Once);
            mockIMediator.Verify(x => x.Send(It.IsAny<AddConsolidacionesRequisitosExpedienteUncommitCommand>(),
                It.IsAny<CancellationToken>()), Times.Once);
            Assert.Equal(expedientePersistido.Id, actual.Id);
            Assert.True(actual.HasAddedSeguimientos);
        }

        [Fact(DisplayName = "Cuando se edita la vía de acceso se añade un seguimiento y Devuelve dto")]
        public async Task Handle_ViaAccesoMasSeguimiento_Ok()
        {
            //ARRANGE
            var expedienteAlumno = new ExpedienteAlumno
            {
                Id = 1,
                IdRefNodo = "456",
                IdRefViaAccesoPlan = "123"
            };
            await Context.ExpedientesAlumno.AddAsync(expedienteAlumno);
            await Context.SaveChangesAsync(CancellationToken.None);
            var request = new EditExpedienteAlumnoPorIntegracionCommand
            {
                IdExpedienteAlumno = 1,
                IdRefNodo = "123",
                IdRefViaAccesoPlan = "789"
            };
            var mockIStringLocalizer = new Mock<IStringLocalizer<EditExpedienteAlumnoPorIntegracionCommandHandler>>
            {
                CallBase = true
            };
            var mockIMediator = new Mock<IMediator>
            {
                CallBase = true
            };
            mockIMediator.Setup(s => s.Send(It.IsAny<ExpedienteAlumno>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Unit.Value);
            mockIMediator.Setup(x => x.Send(It.IsAny<AddConsolidacionesRequisitosExpedienteUncommitCommand>(),
               It.IsAny<CancellationToken>())).ReturnsAsync(Unit.Value);
            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };
            var sut = new Mock<EditExpedienteAlumnoPorIntegracionCommandHandler>(Context, mockIStringLocalizer.Object,
                mockIMediator.Object, mockIErpAcademicoServiceClient.Object)
            {
                CallBase = true
            };
            sut.Setup(s => s.ValidatePropiedadesRequeridas(It.IsAny<EditExpedienteAlumnoPorIntegracionCommand>()));
            sut.Setup(s => s.AssignExpedienteAlumno(It.IsAny<EditExpedienteAlumnoPorIntegracionCommand>(),
                It.IsAny<ExpedienteAlumno>()));
            sut.Setup(s => s.AddEspecializaciones(It.IsAny<ExpedienteAlumno>(),
                It.IsAny<List<ExpedienteEspecializacionDto>>()));
            sut.Setup(s =>
                s.AddSeguimientoTitulacionAcceso(It.IsAny<ExpedienteAlumno>(), It.IsAny<EditExpedienteAlumnoPorIntegracionCommand>(),
                    It.IsAny<CancellationToken>())).ReturnsAsync(false);
            sut.Setup(s => s.AssignVersionPlan(It.IsAny<ExpedienteAlumno>(),
                    It.IsAny<EditExpedienteAlumnoPorIntegracionCommand>()))
                .Returns(false);
            sut.Setup(s => s.AddSeguimientoViaAcceso(It.IsAny<ExpedienteAlumno>(),
                It.IsAny<EditExpedienteAlumnoPorIntegracionCommand>()));
            sut.Setup(s => s.AssignViaAcceso(It.IsAny<ExpedienteAlumno>(),
                    It.IsAny<EditExpedienteAlumnoPorIntegracionCommand>()))
                .Returns(true);
            sut.Setup(x => x.AddSeguimientoExpediente(It.IsAny<ExpedienteAlumno>(),
                It.IsAny<EditExpedienteAlumnoPorIntegracionCommand>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

            //ACT
            var actual = await sut.Object.Handle(request, CancellationToken.None);

            //ASSERT
            var expedientePersistido = await Context.ExpedientesAlumno.FirstAsync(CancellationToken.None);
            sut.Verify(s => s.AssignExpedienteAlumno(It.IsAny<EditExpedienteAlumnoPorIntegracionCommand>(),
                It.IsAny<ExpedienteAlumno>()), Times.Once);
            sut.Verify(s => s.AddEspecializaciones(It.IsAny<ExpedienteAlumno>(),
                It.IsAny<List<ExpedienteEspecializacionDto>>()), Times.Once);
            sut.Verify(s =>
                s.AddSeguimientoTitulacionAcceso(It.IsAny<ExpedienteAlumno>(), It.IsAny<EditExpedienteAlumnoPorIntegracionCommand>(),
                    It.IsAny<CancellationToken>()), Times.Once);
            sut.Verify(s => s.AssignVersionPlan(It.IsAny<ExpedienteAlumno>(),
                It.IsAny<EditExpedienteAlumnoPorIntegracionCommand>()), Times.Once);
            sut.Verify(s => s.AddSeguimientoVersionPlan(It.IsAny<ExpedienteAlumno>(),
                It.IsAny<EditExpedienteAlumnoPorIntegracionCommand>()), Times.Never);
            sut.Verify(s => s.AssignViaAcceso(It.IsAny<ExpedienteAlumno>(),
                It.IsAny<EditExpedienteAlumnoPorIntegracionCommand>()), Times.Once);
            sut.Verify(
                s => s.AddSeguimientoViaAcceso(It.IsAny<ExpedienteAlumno>(),
                    It.IsAny<EditExpedienteAlumnoPorIntegracionCommand>()), Times.Once);
            sut.Verify(x => x.AddSeguimientoExpediente(It.IsAny<ExpedienteAlumno>(),
                It.IsAny<EditExpedienteAlumnoPorIntegracionCommand>(), It.IsAny<CancellationToken>()), Times.Once);
            mockIMediator.Verify(x => x.Send(It.IsAny<AddConsolidacionesRequisitosExpedienteUncommitCommand>(),
                It.IsAny<CancellationToken>()), Times.Once);
            Assert.Equal(expedientePersistido.Id, actual.Id);
            Assert.True(actual.HasAddedSeguimientos);
        }

        [Fact(DisplayName = "Cuando el expediente no existe Devuelve excepción")]
        public async Task Handle_NotFoundExcepcion()
        {
            //ARRANGE
            var expedienteAlumno = new ExpedienteAlumno
            {
                Id = 1,
                IdRefNodo = "456",
                IdRefVersionPlan = "123"
            };
            await Context.ExpedientesAlumno.AddAsync(expedienteAlumno);
            await Context.SaveChangesAsync(CancellationToken.None);
            var request = new EditExpedienteAlumnoPorIntegracionCommand
            {
                IdExpedienteAlumno = 2,
                IdRefNodo = "456",
                AlumnoNombre = "123",
                AlumnoApellido1 = "123",
                IdRefTipoDocumentoIdentificacionPais = "123",
                AlumnoNroDocIdentificacion = "123",
                AlumnoEmail = "123"
            };
            var mockIStringLocalizer = new Mock<IStringLocalizer<EditExpedienteAlumnoPorIntegracionCommandHandler>>
            {
                CallBase = true
            };
            var mockIMediator = new Mock<IMediator>
            {
                CallBase = true
            };
            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };
            var sut = new EditExpedienteAlumnoPorIntegracionCommandHandler(Context, mockIStringLocalizer.Object, 
                mockIMediator.Object, mockIErpAcademicoServiceClient.Object);

            //ACT
            var exception = await Record.ExceptionAsync(async () =>
            {
                await sut.Handle(request, CancellationToken.None);
            });

            //ASSERT
            Assert.IsType<NotFoundException>(exception);
            Assert.Equal(new NotFoundException(nameof(ExpedienteAlumno), request.IdExpedienteAlumno).Message,
                exception.Message);
        }

        #endregion

        #region ValidatePropiedadesRequeridas

        [Fact(DisplayName = "Cuando el id ref versión plan es inválido Devuelve excepción")]
        public void ValidatePropiedadesRequeridas_IdRefVersionPlan()
        {
            //ARRANGE
            var request = new EditExpedienteAlumnoPorIntegracionCommand
            {
                IdRefVersionPlan = "asdas"
            };
            var mockIStringLocalizer = new Mock<IStringLocalizer<EditExpedienteAlumnoPorIntegracionCommandHandler>>
            {
                CallBase = true
            };
            var mensajeEsperado = $"El campo {nameof(request.IdRefVersionPlan)} tiene un valor inválido.";
            mockIStringLocalizer.Setup(s => s[It.Is<string>(msj => msj == mensajeEsperado)])
                .Returns(new LocalizedString(mensajeEsperado, mensajeEsperado));
            var mockIMediator = new Mock<IMediator>
            {
                CallBase = true
            };
            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };
            var sut = new EditExpedienteAlumnoPorIntegracionCommandHandler(Context, mockIStringLocalizer.Object,
                mockIMediator.Object, mockIErpAcademicoServiceClient.Object);

            //ACT
            var exception = Record.Exception(() =>
            {
                sut.ValidatePropiedadesRequeridas(request);
            });

            //ASSERT
            Assert.IsType<BadRequestException>(exception);
            Assert.Equal(mensajeEsperado, exception.Message);
        }

        [Fact(DisplayName = "Cuando el id ref versión plan es inválido Devuelve excepción")]
        public void ValidatePropiedadesRequeridas_IdRefVersionPlan_NoEsMayorQueCero()
        {
            //ARRANGE
            var request = new EditExpedienteAlumnoPorIntegracionCommand
            {
                IdRefVersionPlan = "0"
            };
            var mockIStringLocalizer = new Mock<IStringLocalizer<EditExpedienteAlumnoPorIntegracionCommandHandler>>
            {
                CallBase = true
            };
            const string mensajeEsperado = $"El campo {nameof(request.IdRefVersionPlan)} debe ser mayor a cero.";
            mockIStringLocalizer.Setup(s => s[It.Is<string>(msj => msj == mensajeEsperado)])
                .Returns(new LocalizedString(mensajeEsperado, mensajeEsperado));
            var mockIMediator = new Mock<IMediator>
            {
                CallBase = true
            };
            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };
            var sut = new EditExpedienteAlumnoPorIntegracionCommandHandler(Context, mockIStringLocalizer.Object,
                mockIMediator.Object, mockIErpAcademicoServiceClient.Object);

            //ACT
            var exception = Record.Exception(() =>
            {
                sut.ValidatePropiedadesRequeridas(request);
            });

            //ASSERT
            Assert.IsType<BadRequestException>(exception);
            Assert.Equal(mensajeEsperado, exception.Message);
        }

        [Fact(DisplayName = "Cuando no se ha especificado el nro versión Devuelve excepción")]
        public void ValidatePropiedadesRequeridas_NroVersion()
        {
            //ARRANGE
            var request = new EditExpedienteAlumnoPorIntegracionCommand
            {
                IdRefVersionPlan = "1"
            };
            var mockIStringLocalizer = new Mock<IStringLocalizer<EditExpedienteAlumnoPorIntegracionCommandHandler>>
            {
                CallBase = true
            };
            var mensajeEsperado = $"El campo {nameof(request.NroVersion)} es requerido para Editar el Expediente.";
            mockIStringLocalizer.Setup(s => s[It.Is<string>(msj => msj == mensajeEsperado)])
                .Returns(new LocalizedString(mensajeEsperado, mensajeEsperado));
            var mockIMediator = new Mock<IMediator>
            {
                CallBase = true
            };
            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };
            var sut = new EditExpedienteAlumnoPorIntegracionCommandHandler(Context, mockIStringLocalizer.Object,
                mockIMediator.Object, mockIErpAcademicoServiceClient.Object);

            //ACT
            var exception = Record.Exception(() =>
            {
                sut.ValidatePropiedadesRequeridas(request);
            });

            //ASSERT
            Assert.IsType<BadRequestException>(exception);
            Assert.Equal(mensajeEsperado, exception.Message);
        }

        [Fact(DisplayName = "Cuando el id ref nodo es inválido Devuelve excepción")]
        public void ValidatePropiedadesRequeridas_IdRefNodo()
        {
            //ARRANGE
            var request = new EditExpedienteAlumnoPorIntegracionCommand
            {
                IdRefVersionPlan = "1",
                NroVersion = 2,
                IdRefNodo = "2s"
            };
            var mockIStringLocalizer = new Mock<IStringLocalizer<EditExpedienteAlumnoPorIntegracionCommandHandler>>
            {
                CallBase = true
            };
            var mensajeEsperado = $"El campo {nameof(request.IdRefNodo)} tiene un valor inválido.";
            mockIStringLocalizer.Setup(s => s[It.Is<string>(msj => msj == mensajeEsperado)])
                .Returns(new LocalizedString(mensajeEsperado, mensajeEsperado));
            var mockIMediator = new Mock<IMediator>
            {
                CallBase = true
            };
            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };
            var sut = new EditExpedienteAlumnoPorIntegracionCommandHandler(Context, mockIStringLocalizer.Object,
                mockIMediator.Object, mockIErpAcademicoServiceClient.Object);

            //ACT
            var exception = Record.Exception(() =>
            {
                sut.ValidatePropiedadesRequeridas(request);
            });

            //ASSERT
            Assert.IsType<BadRequestException>(exception);
            Assert.Equal(mensajeEsperado, exception.Message);
        }

        [Fact(DisplayName = "Cuando el id ref nodo es menor que cero Devuelve excepción")]
        public void ValidatePropiedadesRequeridas_IdRefNodo_NoEsMayorQueCero()
        {
            //ARRANGE
            var request = new EditExpedienteAlumnoPorIntegracionCommand
            {
                IdRefVersionPlan = "1",
                NroVersion = 2,
                IdRefNodo = "0"
            };
            var mockIStringLocalizer = new Mock<IStringLocalizer<EditExpedienteAlumnoPorIntegracionCommandHandler>>
            {
                CallBase = true
            };
            const string mensajeEsperado = $"El campo {nameof(request.IdRefNodo)} debe ser mayor a cero.";
            mockIStringLocalizer.Setup(s => s[It.Is<string>(msj => msj == mensajeEsperado)])
                .Returns(new LocalizedString(mensajeEsperado, mensajeEsperado));
            var mockIMediator = new Mock<IMediator>
            {
                CallBase = true
            };
            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };
            var sut = new EditExpedienteAlumnoPorIntegracionCommandHandler(Context, mockIStringLocalizer.Object,
                mockIMediator.Object, mockIErpAcademicoServiceClient.Object);

            //ACT
            var exception = Record.Exception(() =>
            {
                sut.ValidatePropiedadesRequeridas(request);
            });

            //ASSERT
            Assert.IsType<BadRequestException>(exception);
            Assert.Equal(mensajeEsperado, exception.Message);
        }

        [Fact(DisplayName = "Cuando falta especificar el nombre del alumno Devuelve excepción")]
        public void ValidatePropiedadesRequeridas_AlumnoNombre_Invalido()
        {
            //ARRANGE
            var request = new EditExpedienteAlumnoPorIntegracionCommand
            {
                IdRefVersionPlan = "789",
                NroVersion = 1,
                IdRefNodo = "122"
            };
            var mockIStringLocalizer = new Mock<IStringLocalizer<EditExpedienteAlumnoPorIntegracionCommandHandler>>
            {
                CallBase = true
            };
            var mensajeEsperado = $"El campo {nameof(request.AlumnoNombre)} es requerido para Editar el Expediente.";
            mockIStringLocalizer.Setup(s => s[It.Is<string>(msj => msj == mensajeEsperado)])
                .Returns(new LocalizedString(mensajeEsperado, mensajeEsperado));
            var mockIMediator = new Mock<IMediator>
            {
                CallBase = true
            };
            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };
            var sut = new EditExpedienteAlumnoPorIntegracionCommandHandler(Context, mockIStringLocalizer.Object,
                mockIMediator.Object, mockIErpAcademicoServiceClient.Object);

            //ACT
            var ex = Record.Exception(() =>
            {
                sut.ValidatePropiedadesRequeridas(request);
            });

            //ASSERT
            Assert.IsType<BadRequestException>(ex);
            Assert.Equal(mensajeEsperado, ex.Message);
        }

        [Fact(DisplayName = "Cuando falta especificar el apellido1 del alumno Devuelve excepción")]
        public void ValidatePropiedadesRequeridas_AlumnoApellido1_Invalido()
        {
            //ARRANGE
            var request = new EditExpedienteAlumnoPorIntegracionCommand
            {
                IdRefVersionPlan = "789",
                NroVersion = 1,
                IdRefNodo = "122",
                AlumnoNombre = Guid.NewGuid().ToString()
            };
            var mockIStringLocalizer = new Mock<IStringLocalizer<EditExpedienteAlumnoPorIntegracionCommandHandler>>
            {
                CallBase = true
            };
            var mensajeEsperado = $"El campo {nameof(request.AlumnoApellido1)} es requerido para Editar el Expediente.";
            mockIStringLocalizer.Setup(s => s[It.Is<string>(msj => msj == mensajeEsperado)])
                .Returns(new LocalizedString(mensajeEsperado, mensajeEsperado));
            var mockIMediator = new Mock<IMediator>
            {
                CallBase = true
            };
            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };
            var sut = new EditExpedienteAlumnoPorIntegracionCommandHandler(Context, mockIStringLocalizer.Object,
                mockIMediator.Object, mockIErpAcademicoServiceClient.Object);

            //ACT
            var ex = Record.Exception(() =>
            {
                sut.ValidatePropiedadesRequeridas(request);
            });

            //ASSERT
            Assert.IsType<BadRequestException>(ex);
            Assert.Equal(mensajeEsperado, ex.Message);
        }

        [Fact(DisplayName = "Cuando falta especificar el id de referencia del tipo de identificación por país Devuelve excepción")]
        public void ValidatePropiedadesRequeridas_IdRefTipoDocumentoIdentificacionPais_Invalido()
        {
            //ARRANGE
            var request = new EditExpedienteAlumnoPorIntegracionCommand
            {
                IdRefVersionPlan = "789",
                NroVersion = 1,
                IdRefNodo = "122",
                AlumnoNombre = Guid.NewGuid().ToString(),
                AlumnoApellido1 = Guid.NewGuid().ToString()
            };
            var mockIStringLocalizer = new Mock<IStringLocalizer<EditExpedienteAlumnoPorIntegracionCommandHandler>>
            {
                CallBase = true
            };
            var mensajeEsperado = $"El campo {nameof(request.IdRefTipoDocumentoIdentificacionPais)} es requerido para Editar el Expediente.";
            mockIStringLocalizer.Setup(s => s[It.Is<string>(msj => msj == mensajeEsperado)])
                .Returns(new LocalizedString(mensajeEsperado, mensajeEsperado));
            var mockIMediator = new Mock<IMediator>
            {
                CallBase = true
            };
            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };
            var sut = new EditExpedienteAlumnoPorIntegracionCommandHandler(Context, mockIStringLocalizer.Object,
                mockIMediator.Object, mockIErpAcademicoServiceClient.Object);

            //ACT
            var ex = Record.Exception(() =>
            {
                sut.ValidatePropiedadesRequeridas(request);
            });

            //ASSERT
            Assert.IsType<BadRequestException>(ex);
            Assert.Equal(mensajeEsperado, ex.Message);
        }

        [Fact(DisplayName = "Cuando falta especificar el número de identificación del alumno Devuelve excepción")]
        public void ValidatePropiedadesRequeridas_AlumnoNroDocIdentificacion_Invalido()
        {
            //ARRANGE
            var request = new EditExpedienteAlumnoPorIntegracionCommand
            {
                IdRefVersionPlan = "789",
                NroVersion = 1,
                IdRefNodo = "122",
                AlumnoNombre = Guid.NewGuid().ToString(),
                AlumnoApellido1 = Guid.NewGuid().ToString(),
                IdRefTipoDocumentoIdentificacionPais = "654"
            };
            var mockIStringLocalizer = new Mock<IStringLocalizer<EditExpedienteAlumnoPorIntegracionCommandHandler>>
            {
                CallBase = true
            };
            var mockIMediator = new Mock<IMediator>
            {
                CallBase = true
            };
            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };
            var mensajeEsperado = $"El campo {nameof(request.AlumnoNroDocIdentificacion)} es requerido para Editar el Expediente.";
            mockIStringLocalizer.Setup(s => s[It.Is<string>(msj => msj == mensajeEsperado)])
                .Returns(new LocalizedString(mensajeEsperado, mensajeEsperado));
            var sut = new EditExpedienteAlumnoPorIntegracionCommandHandler(Context, mockIStringLocalizer.Object,
                mockIMediator.Object, mockIErpAcademicoServiceClient.Object);

            //ACT
            var ex = Record.Exception(() =>
            {
                sut.ValidatePropiedadesRequeridas(request);
            });

            //ASSERT
            Assert.IsType<BadRequestException>(ex);
            Assert.Equal(mensajeEsperado, ex.Message);
        }

        [Fact(DisplayName = "Cuando falta especificar el email del alumno Devuelve excepción")]
        public void ValidatePropiedadesRequeridas_AlumnoEmail_Invalido()
        {
            //ARRANGE
            var request = new EditExpedienteAlumnoPorIntegracionCommand
            {
                IdRefVersionPlan = "789",
                NroVersion = 1,
                IdRefNodo = "122",
                AlumnoNombre = Guid.NewGuid().ToString(),
                AlumnoApellido1 = Guid.NewGuid().ToString(),
                IdRefTipoDocumentoIdentificacionPais = "654",
                AlumnoNroDocIdentificacion = "46658645113"
            };
            var mockIStringLocalizer = new Mock<IStringLocalizer<EditExpedienteAlumnoPorIntegracionCommandHandler>>
            {
                CallBase = true
            };
            var mensajeEsperado = $"El campo {nameof(request.AlumnoEmail)} es requerido para Editar el Expediente.";
            mockIStringLocalizer.Setup(s => s[It.Is<string>(msj => msj == mensajeEsperado)])
                .Returns(new LocalizedString(mensajeEsperado, mensajeEsperado));
            var mockIMediator = new Mock<IMediator>
            {
                CallBase = true
            };
            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };
            var sut = new EditExpedienteAlumnoPorIntegracionCommandHandler(Context, mockIStringLocalizer.Object,
                mockIMediator.Object, mockIErpAcademicoServiceClient.Object);

            //ACT
            var ex = Record.Exception(() =>
            {
                sut.ValidatePropiedadesRequeridas(request);
            });

            //ASSERT
            Assert.IsType<BadRequestException>(ex);
            Assert.Equal(mensajeEsperado, ex.Message);
        }

        [Fact(DisplayName = "Cuando se envían todos los campos requeridos Continua con el proceso")]
        public void ValidatePropiedadesRequeridas_Ok()
        {
            //ARRANGE
            var request = new EditExpedienteAlumnoPorIntegracionCommand
            {
                IdRefVersionPlan = "1",
                NroVersion = 2,
                IdRefNodo = "2",
                AlumnoNombre = "123",
                AlumnoApellido1 = "123",
                IdRefTipoDocumentoIdentificacionPais = "123",
                AlumnoNroDocIdentificacion = "123",
                AlumnoEmail = "123"
            };
            var mockIStringLocalizer = new Mock<IStringLocalizer<EditExpedienteAlumnoPorIntegracionCommandHandler>>
            {
                CallBase = true
            };
            var mockIMediator = new Mock<IMediator>
            {
                CallBase = true
            };
            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };
            var sut = new EditExpedienteAlumnoPorIntegracionCommandHandler(Context, mockIStringLocalizer.Object,
                mockIMediator.Object, mockIErpAcademicoServiceClient.Object);

            //ACT
            sut.ValidatePropiedadesRequeridas(request);

            //ASSERT
            Assert.True(int.TryParse(request.IdRefVersionPlan, out _));
            Assert.True(int.TryParse(request.IdRefNodo, out _));
        }

        #endregion


        #region AssignVersionPlan

        [Trait("Category", "AssignVersionPlan")]
        [Fact(DisplayName = "Cuando se asignan los valores para la versión plan Devuelve true")]
        public void AssignVersionPlan_Ok_True()
        {
            //ARRANGE
            var mockIStringLocalizer = new Mock<IStringLocalizer<EditExpedienteAlumnoPorIntegracionCommandHandler>>
            {
                CallBase = true
            };
            var mockIMediator = new Mock<IMediator>
            {
                CallBase = true
            };
            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };
            var sut = new EditExpedienteAlumnoPorIntegracionCommandHandler(Context, mockIStringLocalizer.Object,
                mockIMediator.Object, mockIErpAcademicoServiceClient.Object);

            var expedienteAlumno = new ExpedienteAlumno
            {
                IdRefVersionPlan = Guid.NewGuid().ToString()
            };
            var request = new EditExpedienteAlumnoPorIntegracionCommand
            {
                PorIntegracion = true,
                IdRefVersionPlan = Guid.NewGuid().ToString()
            };

            //ACT
            var actual = sut.AssignVersionPlan(expedienteAlumno, request);

            //ASSERT
            Assert.True(actual);
        }

        [Trait("Category", "AssignVersionPlan")]
        [Fact(DisplayName = "Cuando no se asignan los valores para la versión plan Devuelve false")]
        public void AssignVersionPlan_Ok_False()
        {
            //ARRANGE
            var mockIStringLocalizer = new Mock<IStringLocalizer<EditExpedienteAlumnoPorIntegracionCommandHandler>>
            {
                CallBase = true
            };
            var mockIMediator = new Mock<IMediator>
            {
                CallBase = true
            };
            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };
            var sut = new EditExpedienteAlumnoPorIntegracionCommandHandler(Context, mockIStringLocalizer.Object,
                mockIMediator.Object, mockIErpAcademicoServiceClient.Object);

            var expedienteAlumno = new ExpedienteAlumno
            {
                IdRefVersionPlan = Guid.NewGuid().ToString()
            };
            var request = new EditExpedienteAlumnoPorIntegracionCommand
            {
                PorIntegracion = true,
                IdRefVersionPlan = expedienteAlumno.IdRefVersionPlan
            };

            //ACT
            var actual = sut.AssignVersionPlan(expedienteAlumno, request);

            //ASSERT
            Assert.False(actual);
            Assert.Equal(expedienteAlumno.IdRefVersionPlan, request.IdRefVersionPlan);
        }

        #endregion

        #region AssignViaAcceso

        [Trait("Category", "AssignViaAcceso")]
        [Fact(DisplayName = "Cuando se asignan los valores para la vía de acceso plan Devuelve true")]
        public void AssignViaAcceso_Ok_True()
        {
            //ARRANGE
            var mockIStringLocalizer = new Mock<IStringLocalizer<EditExpedienteAlumnoPorIntegracionCommandHandler>>
            {
                CallBase = true
            };
            var mockIMediator = new Mock<IMediator>
            {
                CallBase = true
            };
            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };
            var sut = new EditExpedienteAlumnoPorIntegracionCommandHandler(Context, mockIStringLocalizer.Object,
                mockIMediator.Object, mockIErpAcademicoServiceClient.Object);

            var expedienteAlumno = new ExpedienteAlumno
            {
                IdRefViaAccesoPlan = Guid.NewGuid().ToString()
            };
            var request = new EditExpedienteAlumnoPorIntegracionCommand
            {
                IdRefViaAccesoPlan = Guid.NewGuid().ToString()
            };

            //ACT
            var actual = sut.AssignViaAcceso(expedienteAlumno, request);

            //ASSERT
            Assert.True(actual);
        }

        [Trait("Category", "AssignViaAcceso")]
        [Fact(DisplayName = "Cuando no se asignan los valores para la para la vía de acceso plan Devuelve false")]
        public void AssignViaAcceso_Ok_False()
        {
            //ARRANGE
            var mockIStringLocalizer = new Mock<IStringLocalizer<EditExpedienteAlumnoPorIntegracionCommandHandler>>
            {
                CallBase = true
            };
            var mockIMediator = new Mock<IMediator>
            {
                CallBase = true
            };
            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };
            var sut = new EditExpedienteAlumnoPorIntegracionCommandHandler(Context, mockIStringLocalizer.Object,
                mockIMediator.Object, mockIErpAcademicoServiceClient.Object);

            var expedienteAlumno = new ExpedienteAlumno
            {
                IdRefViaAccesoPlan = Guid.NewGuid().ToString()
            };
            var request = new EditExpedienteAlumnoPorIntegracionCommand
            {
                IdRefViaAccesoPlan = expedienteAlumno.IdRefViaAccesoPlan
            };

            //ACT
            var actual = sut.AssignViaAcceso(expedienteAlumno, request);

            //ASSERT
            Assert.False(actual);
            Assert.Equal(expedienteAlumno.IdRefViaAccesoPlan, request.IdRefViaAccesoPlan);
        }

        #endregion

        #region AssignExpedienteAlumno

        [Fact(DisplayName = "Cuando se asignan los valores al expediente Devuelve entidad seteada")]
        public void AssignExpedienteAlumno_Ok()
        {
            //ARRANGE
            var request = new EditExpedienteAlumnoPorIntegracionCommand
            {
                IdRefVersionPlan = "789",
                IdRefNodo = "159",
                PorIntegracion = true,
                AlumnoNombre = Guid.NewGuid().ToString(),
                AlumnoApellido1 = Guid.NewGuid().ToString(),
                AlumnoApellido2 = Guid.NewGuid().ToString(),
                IdRefTipoDocumentoIdentificacionPais = Guid.NewGuid().ToString(),
                AlumnoNroDocIdentificacion = Guid.NewGuid().ToString(),
                AlumnoEmail = Guid.NewGuid().ToString(),
                IdRefViaAccesoPlan = Guid.NewGuid().ToString(),
                DocAcreditativoViaAcceso = Guid.NewGuid().ToString(),
                IdRefIntegracionDocViaAcceso = Guid.NewGuid().ToString(),
                FechaSubidaDocViaAcceso = Guid.NewGuid().ToString(),
                IdRefTipoVinculacion = Guid.NewGuid().ToString(),
                NombrePlan = Guid.NewGuid().ToString(),
                IdRefUniversidad = Guid.NewGuid().ToString(),
                AcronimoUniversidad = Guid.NewGuid().ToString(),
                IdRefCentro = Guid.NewGuid().ToString(),
                IdRefAreaAcademica = Guid.NewGuid().ToString(),
                IdRefTipoEstudio = Guid.NewGuid().ToString(),
                IdRefEstudio = Guid.NewGuid().ToString(),
                NombreEstudio = Guid.NewGuid().ToString(),
                IdRefTitulo = Guid.NewGuid().ToString(),
                FechaApertura = DateTime.Now,
                FechaFinalizacion = DateTime.Now,
                FechaTrabajoFinEstudio = DateTime.Now,
                TituloTrabajoFinEstudio = Guid.NewGuid().ToString(),
                FechaExpedicion = DateTime.Now,
                NotaMedia = Double.MaxValue,
                FechaPago = DateTime.Now,
            };
            var mockIStringLocalizer = new Mock<IStringLocalizer<EditExpedienteAlumnoPorIntegracionCommandHandler>>
            {
                CallBase = true
            };
            var mockIMediator = new Mock<IMediator>
            {
                CallBase = true
            };
            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };
            var sutMock = new Mock<EditExpedienteAlumnoPorIntegracionCommandHandler>(Context, mockIStringLocalizer.Object, 
                mockIMediator.Object, mockIErpAcademicoServiceClient.Object)
            {
                CallBase = true
            };

            var expedienteEsperado = new ExpedienteAlumno();

            //ACT
            sutMock.Object.AssignExpedienteAlumno(request, expedienteEsperado);

            //ASSERT
            Assert.Equal(expedienteEsperado.IdRefNodo, request.IdRefNodo);
            Assert.Equal(expedienteEsperado.AlumnoNombre, request.AlumnoNombre);
            Assert.Equal(expedienteEsperado.AlumnoApellido1, request.AlumnoApellido1);
            Assert.Equal(expedienteEsperado.AlumnoApellido2, request.AlumnoApellido2);
            Assert.Equal(expedienteEsperado.IdRefTipoDocumentoIdentificacionPais, request.IdRefTipoDocumentoIdentificacionPais);
            Assert.Equal(expedienteEsperado.AlumnoNroDocIdentificacion, request.AlumnoNroDocIdentificacion);
            Assert.Equal(expedienteEsperado.AlumnoEmail, request.AlumnoEmail);
            Assert.Equal(expedienteEsperado.DocAcreditativoViaAcceso, request.DocAcreditativoViaAcceso);
            Assert.Equal(expedienteEsperado.IdRefIntegracionDocViaAcceso, request.IdRefIntegracionDocViaAcceso);
            Assert.Equal(expedienteEsperado.FechaSubidaDocViaAcceso, request.FechaSubidaDocViaAcceso);
            Assert.Equal(expedienteEsperado.NombrePlan, request.NombrePlan);
            Assert.Equal(expedienteEsperado.IdRefUniversidad, request.IdRefUniversidad);
            Assert.Equal(expedienteEsperado.AcronimoUniversidad, request.AcronimoUniversidad);
            Assert.Equal(expedienteEsperado.IdRefCentro, request.IdRefCentro);
            Assert.Equal(expedienteEsperado.IdRefAreaAcademica, request.IdRefAreaAcademica);
            Assert.Equal(expedienteEsperado.IdRefTipoEstudio, request.IdRefTipoEstudio);
            Assert.Equal(expedienteEsperado.IdRefEstudio, request.IdRefEstudio);
            Assert.Equal(expedienteEsperado.NombreEstudio, request.NombreEstudio);
            Assert.Equal(expedienteEsperado.IdRefTitulo, request.IdRefTitulo);
            Assert.Equal(expedienteEsperado.FechaApertura, request.FechaApertura);
            Assert.Equal(expedienteEsperado.FechaFinalizacion, request.FechaFinalizacion);
            Assert.Equal(expedienteEsperado.FechaTrabajoFinEstudio, request.FechaTrabajoFinEstudio);
            Assert.Equal(expedienteEsperado.TituloTrabajoFinEstudio, request.TituloTrabajoFinEstudio);
            Assert.Equal(expedienteEsperado.FechaExpedicion, request.FechaExpedicion);
            Assert.Equal(expedienteEsperado.NotaMedia, request.NotaMedia);
            Assert.Equal(expedienteEsperado.FechaPago, request.FechaPago);
        }

        #endregion

        #region AssignTitulacionAcceso

        [Fact(DisplayName = "Cuando se asignan los valores a la titulacion de acceso Devuelve entidad seteada")]
        public void AssignTitulacionAcceso_Ok()
        {
            //ARRANGE
            var request = new TitulacionAccesoParametersDto
            {
                CodigoColegiadoProfesional = Guid.NewGuid().ToString(),
                FechaInicioTitulo = DateTime.Now,
                FechafinTitulo = DateTime.Now,
                IdRefTerritorioInstitucionDocente = "56468",
                InstitucionDocente = Guid.NewGuid().ToString(),
                NroSemestreRealizados = 5,
                TipoEstudio = Guid.NewGuid().ToString(),
                IdRefInstitucionDocente = "55421"
            };
            var mockIStringLocalizer = new Mock<IStringLocalizer<EditExpedienteAlumnoPorIntegracionCommandHandler>>
            {
                CallBase = true
            };
            var mockIMediator = new Mock<IMediator>
            {
                CallBase = true
            };
            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };
            var sut = new EditExpedienteAlumnoPorIntegracionCommandHandler(Context, mockIStringLocalizer.Object, 
                mockIMediator.Object, mockIErpAcademicoServiceClient.Object);

            var expedienteEsperado = new ExpedienteAlumno();

            //ACT
            sut.AssignTitulacionAcceso(request, expedienteEsperado);

            //ASSERT
            Assert.Equal(expedienteEsperado.TitulacionAcceso.CodigoColegiadoProfesional, request.CodigoColegiadoProfesional);
            Assert.Equal(expedienteEsperado.TitulacionAcceso.FechaInicioTitulo, request.FechaInicioTitulo);
            Assert.Equal(expedienteEsperado.TitulacionAcceso.FechafinTitulo, request.FechafinTitulo);
            Assert.Equal(expedienteEsperado.TitulacionAcceso.IdRefTerritorioInstitucionDocente, request.IdRefTerritorioInstitucionDocente);
            Assert.Equal(expedienteEsperado.TitulacionAcceso.InstitucionDocente, request.InstitucionDocente);
            Assert.Equal(expedienteEsperado.TitulacionAcceso.NroSemestreRealizados, request.NroSemestreRealizados);
            Assert.Equal(expedienteEsperado.TitulacionAcceso.CodigoColegiadoProfesional, request.CodigoColegiadoProfesional);
            Assert.Equal(expedienteEsperado.TitulacionAcceso.TipoEstudio, request.TipoEstudio);
            Assert.Equal(expedienteEsperado.TitulacionAcceso.IdRefInstitucionDocente, request.IdRefInstitucionDocente);
        }

        #endregion

        #region AddEspecializacionesHitosAsync

        [Fact(DisplayName = "Cuando se agregan especializaciones que no existen para el expediente Ejecuta metodo")]
        public async Task AddEspecializaciones_Ok()
        {
            //ARRANGE
            var mockExpedienteAlumno = new Mock<ExpedienteAlumno> { CallBase = true };
            mockExpedienteAlumno.SetupAllProperties();
            mockExpedienteAlumno.Object.Id = 1;
            mockExpedienteAlumno.Setup(s => s.HasEspecializacion(It.IsAny<string>())).Returns(false);
            mockExpedienteAlumno.Setup(s => s.AddEspecializacion(It.IsAny<string>()));
            mockExpedienteAlumno.Setup(s => s.DeleteEspecializacionesNoIncluidos(It.IsAny<string[]>()));

            var request = new List<ExpedienteEspecializacionDto>
            {
                new()
                {
                    IdRefEspecializacion = "123"
                },
                new()
                {
                    IdRefEspecializacion = "321"
                }
            };
            var mockIStringLocalizer = new Mock<IStringLocalizer<EditExpedienteAlumnoPorIntegracionCommandHandler>>
            {
                CallBase = true
            };
            var mockIMediator = new Mock<IMediator>
            {
                CallBase = true
            };
            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };
            var sut = new EditExpedienteAlumnoPorIntegracionCommandHandler(Context, mockIStringLocalizer.Object,
                mockIMediator.Object, mockIErpAcademicoServiceClient.Object);
            await Context.ExpedientesAlumno.AddAsync(mockExpedienteAlumno.Object);
            await Context.SaveChangesAsync(CancellationToken.None);

            //ACT
            sut.AddEspecializaciones(mockExpedienteAlumno.Object, request);

            //ASSERT
            mockExpedienteAlumno.Verify(s => s.HasEspecializacion(It.IsAny<string>()), Times.Exactly(2));
            mockExpedienteAlumno.Verify(s => s.AddEspecializacion(It.IsAny<string>()), Times.Exactly(2));
            mockExpedienteAlumno.Verify(s => s.DeleteEspecializacionesNoIncluidos(It.IsAny<string[]>()), Times.Once);
        }

        [Fact(DisplayName = "Cuando se agregan especializaciones que ya existen para el expediente no Ejecuta metodo")]
        public async Task AddEspecializaciones_NoAgregaEspecializaciones()
        {
            //ARRANGE
            var mockExpedienteAlumno = new Mock<ExpedienteAlumno> { CallBase = true };
            mockExpedienteAlumno.SetupAllProperties();
            mockExpedienteAlumno.Object.Id = 1;
            mockExpedienteAlumno.Setup(s => s.HasEspecializacion(It.IsAny<string>())).Returns(true);
            mockExpedienteAlumno.Setup(s => s.AddEspecializacion(It.IsAny<string>()));
            mockExpedienteAlumno.Setup(s => s.DeleteEspecializacionesNoIncluidos(It.IsAny<string[]>()));

            var request = new List<ExpedienteEspecializacionDto>
            {
                new()
                {
                    IdRefEspecializacion = "123"
                },
                new()
                {
                    IdRefEspecializacion = "321"
                }
            };
            var mockIStringLocalizer = new Mock<IStringLocalizer<EditExpedienteAlumnoPorIntegracionCommandHandler>>
            {
                CallBase = true
            };
            var mockIMediator = new Mock<IMediator>
            {
                CallBase = true
            };
            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };
            var sut = new EditExpedienteAlumnoPorIntegracionCommandHandler(Context, mockIStringLocalizer.Object,
                mockIMediator.Object, mockIErpAcademicoServiceClient.Object);
            await Context.ExpedientesAlumno.AddAsync(mockExpedienteAlumno.Object);
            await Context.SaveChangesAsync(CancellationToken.None);

            //ACT
            sut.AddEspecializaciones(mockExpedienteAlumno.Object, request);

            //ASSERT
            mockExpedienteAlumno.Verify(s => s.HasEspecializacion(It.IsAny<string>()), Times.Exactly(2));
            mockExpedienteAlumno.Verify(s => s.AddEspecializacion(It.IsAny<string>()), Times.Never);
            mockExpedienteAlumno.Verify(s => s.DeleteEspecializacionesNoIncluidos(It.IsAny<string[]>()), Times.Once);
        }

        #endregion

        #region AddSeguimientoVersionPlan

        [Trait("Category", "AddSeguimientoVersionPlan")]
        [Fact(DisplayName = "Cuando se agrega el seguimiento de versión de plan por integración")]
        public void AddSeguimientoVersionPlan_PorIntegracion_Ok()
        {
            //ARRANGE
            var mockIStringLocalizer = new Mock<IStringLocalizer<EditExpedienteAlumnoPorIntegracionCommandHandler>>
            {
                CallBase = true
            };
            var mockIMediator = new Mock<IMediator>
            {
                CallBase = true
            };
            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };
            var sut = new EditExpedienteAlumnoPorIntegracionCommandHandler(Context, mockIStringLocalizer.Object,
                mockIMediator.Object, mockIErpAcademicoServiceClient.Object);

            var mockExpedienteAlumno = new Mock<ExpedienteAlumno>
            {
                CallBase = true
            };
            mockExpedienteAlumno.SetupAllProperties();
            var expedienteAlumno = mockExpedienteAlumno.Object;
            mockExpedienteAlumno.Setup(x => x.AddSeguimientoNoUser(It.IsAny<int>(), It.IsAny<string>(),null,
                    It.IsAny<string>(), It.IsAny<string>()));
            var request = new EditExpedienteAlumnoPorIntegracionCommand
            {
                PorIntegracion = true
            };

            //ACT
            sut.AddSeguimientoVersionPlan(expedienteAlumno, request);

            //ASSERT
            mockExpedienteAlumno.Verify(x => x.AddSeguimientoNoUser(It.IsAny<int>(), It.IsAny<string>(),null,
                    It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        #endregion

        #region AddSeguimientoViaAcceso

        [Trait("Category", "AddSeguimientoViaAcceso")]
        [Fact(DisplayName = "Cuando se agrega el seguimiento de vía de acceso por integración")]
        public void AddSeguimientoViaAcceso_PorIntegracion_Ok()
        {
            //ARRANGE
            var mockIStringLocalizer = new Mock<IStringLocalizer<EditExpedienteAlumnoPorIntegracionCommandHandler>>
            {
                CallBase = true
            };
            var mockIMediator = new Mock<IMediator>
            {
                CallBase = true
            };
            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };
            var sut = new EditExpedienteAlumnoPorIntegracionCommandHandler(Context, mockIStringLocalizer.Object,
                mockIMediator.Object, mockIErpAcademicoServiceClient.Object);

            var mockExpedienteAlumno = new Mock<ExpedienteAlumno>
            {
                CallBase = true
            };
            mockExpedienteAlumno.SetupAllProperties();
            var expedienteAlumno = mockExpedienteAlumno.Object;
            mockExpedienteAlumno.Setup(x => x.AddSeguimientoNoUser(It.IsAny<int>(), It.IsAny<string>(), null,
                    It.IsAny<string>(), It.IsAny<string>()));
            var request = new EditExpedienteAlumnoPorIntegracionCommand
            {
                PorIntegracion = true,
                IdRefViaAccesoPlan = "1"
            };

            //ACT
            sut.AddSeguimientoViaAcceso(expedienteAlumno, request);

            //ASSERT
            mockExpedienteAlumno.Verify(x => x.AddSeguimientoNoUser(It.IsAny<int>(), It.IsAny<string>(), null,
                    It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }
        
        #endregion

        #region AddSeguimientoTitulacionAcceso

        [Trait("Category", "AddSeguimientoTitulacionAcceso")]
        [Fact(DisplayName = "Cuando la titulación de acceso es null No se agrega el seguimiento")]
        public async Task AddSeguimientoTitulacionAcceso_Null()
        {
            //ARRANGE
            var mockIStringLocalizer = new Mock<IStringLocalizer<EditExpedienteAlumnoPorIntegracionCommandHandler>>
            {
                CallBase = true
            };
            var mockIMediator = new Mock<IMediator>
            {
                CallBase = true
            };
            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };
            var sutMock = new EditExpedienteAlumnoPorIntegracionCommandHandler(Context, mockIStringLocalizer.Object,
                mockIMediator.Object, mockIErpAcademicoServiceClient.Object);

            var expedienteAlumno = new ExpedienteAlumno();
            var request = new EditExpedienteAlumnoPorIntegracionCommand();

            //ACT
            await sutMock.AddSeguimientoTitulacionAcceso(expedienteAlumno, request, CancellationToken.None);

            //ASSERT
            Assert.Null(request.TitulacionAcceso);
        }


        [Trait("Category", "AddSeguimientoTitulacionAcceso")]
        [Fact(DisplayName = "Cuando se agregaa el seguimiento de titulación de acceso Retorna True")]
        public async Task AddSeguimientoTitulacionAcceso_AssignTitulacionAcceso_Ok()
        {
            //ARRANGE
            var mockIStringLocalizer = new Mock<IStringLocalizer<EditExpedienteAlumnoPorIntegracionCommandHandler>>
            {
                CallBase = true
            };
            var mockIMediator = new Mock<IMediator>
            {
                CallBase = true
            };
            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };
            var sutMock = new Mock<EditExpedienteAlumnoPorIntegracionCommandHandler>(Context, mockIStringLocalizer.Object, 
                mockIMediator.Object, mockIErpAcademicoServiceClient.Object)
            {
                CallBase = true
            };

            mockIMediator.Setup(s => s.Send(It.IsAny<AddSeguimientoTitulacionAccesoUncommitCommand>(), 
                    It.IsAny<CancellationToken>())).ReturnsAsync(true);

            var expedienteAlumno = new ExpedienteAlumno
            {
                Id = 1
            };
            var request = new EditExpedienteAlumnoPorIntegracionCommand
            {
                PorIntegracion = true,
                TitulacionAcceso = new TitulacionAccesoParametersDto
                {
                    Titulo = Guid.NewGuid().ToString(),
                    InstitucionDocente = Guid.NewGuid().ToString(),
                    NroSemestreRealizados = 10
                }
            };

            //ACT
            var actual = await sutMock.Object.AddSeguimientoTitulacionAcceso(expedienteAlumno, request, CancellationToken.None);

            //ASSERT
            Assert.True(actual);
            mockIMediator.Verify(s => s.Send(It.IsAny<AddSeguimientoTitulacionAccesoUncommitCommand>(),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Trait("Category", "AddSeguimientoTitulacionAcceso")]
        [Fact(DisplayName = "Cuando no se agrega el seguimiento de titulación de acceso Retorna False")]
        public async Task AddSeguimientoTitulacionAcceso_No_AssignTitulacionAcceso_Ok()
        {
            //ARRANGE
            var mockIStringLocalizer = new Mock<IStringLocalizer<EditExpedienteAlumnoPorIntegracionCommandHandler>>
            {
                CallBase = true
            };
            var mockIMediator = new Mock<IMediator>
            {
                CallBase = true
            };
            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };
            var sutMock = new Mock<EditExpedienteAlumnoPorIntegracionCommandHandler>(Context, mockIStringLocalizer.Object,
                mockIMediator.Object, mockIErpAcademicoServiceClient.Object)
            {
                CallBase = true
            };

            mockIMediator.Setup(s => s.Send(It.IsAny<AddSeguimientoTitulacionAccesoUncommitCommand>(),
                    It.IsAny<CancellationToken>())).ReturnsAsync(false);
            sutMock.Setup(s => s.AssignTitulacionAcceso(
                It.IsAny<TitulacionAccesoParametersDto>(), It.IsAny<ExpedienteAlumno>()));

            var expedienteAlumno = new ExpedienteAlumno
            {
                Id = 1
            };
            var request = new EditExpedienteAlumnoPorIntegracionCommand
            {
                PorIntegracion = true,
                TitulacionAcceso = new TitulacionAccesoParametersDto
                {
                    Titulo = Guid.NewGuid().ToString(),
                    InstitucionDocente = Guid.NewGuid().ToString(),
                    NroSemestreRealizados = 10
                }
            };

            //ACT
            var actual = await sutMock.Object.AddSeguimientoTitulacionAcceso(expedienteAlumno, request, CancellationToken.None);

            //ASSERT
            Assert.False(actual);
            mockIMediator.Verify(s => s.Send(It.IsAny<AddSeguimientoTitulacionAccesoUncommitCommand>(),
                It.IsAny<CancellationToken>()), Times.Once);
            sutMock.Verify(s => s.AssignTitulacionAcceso(
                It.IsAny<TitulacionAccesoParametersDto>(), It.IsAny<ExpedienteAlumno>()), Times.Never);
        }

        #endregion

        #region AddSeguimientoExpediente

        [Fact(DisplayName = "Cuando es primera matrícula se guarda el seguimiento de expediente creado")]
        public async Task AddSeguimientoExpediente_ExpedienteCreado()
        {
            //ARRANGE
            var request = new EditExpedienteAlumnoPorIntegracionCommand
            {
                IdEstadoMatricula = 1
            };
            var mockExpedienteAlumno = new Mock<ExpedienteAlumno>
            {
                CallBase = true
            };
            mockExpedienteAlumno.Setup(s =>
                s.AddSeguimientoNoUser(It.IsAny<int>(), It.IsAny<string>(), null,
                    It.IsAny<string>(), It.IsAny<string>()));
            var mockIStringLocalizer = new Mock<IStringLocalizer<EditExpedienteAlumnoPorIntegracionCommandHandler>>
            {
                CallBase = true
            };
            var mockIMediator = new Mock<IMediator>
            {
                CallBase = true
            };
            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };
            var sut = new EditExpedienteAlumnoPorIntegracionCommandHandler(Context, mockIStringLocalizer.Object,
                mockIMediator.Object, mockIErpAcademicoServiceClient.Object);
            mockIMediator.Setup(x => x.Send(It.IsAny<HasPrimeraMatriculaExpedienteQuery>(),
                It.IsAny<CancellationToken>())).ReturnsAsync(true);

            //ACT
            await sut.AddSeguimientoExpediente(mockExpedienteAlumno.Object, request, CancellationToken.None);

            //ASSERT
            mockExpedienteAlumno.Verify(s =>
                    s.AddSeguimientoNoUser(It.IsAny<int>(), It.IsAny<string>(), null,
                    It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            mockIMediator.Verify(x => x.Send(It.IsAny<HasPrimeraMatriculaExpedienteQuery>(),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact(DisplayName = "Cuando no es primera matrícula se guarda el seguimiento de expediente actualizado")]
        public async Task AddSeguimientoExpediente_ExpedienteActualizadoo()
        {
            //ARRANGE
            var request = new EditExpedienteAlumnoPorIntegracionCommand
            {
                IdEstadoMatricula = 1
            };
            var mockExpedienteAlumno = new Mock<ExpedienteAlumno>
            {
                CallBase = true
            };
            mockExpedienteAlumno.Setup(s =>
                s.AddSeguimientoNoUser(It.IsAny<int>(), It.IsAny<string>(), null,
                    It.IsAny<string>(), It.IsAny<string>()));
            var mockIStringLocalizer = new Mock<IStringLocalizer<EditExpedienteAlumnoPorIntegracionCommandHandler>>
            {
                CallBase = true
            };
            var mockIMediator = new Mock<IMediator>
            {
                CallBase = true
            };
            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };
            var sut = new EditExpedienteAlumnoPorIntegracionCommandHandler(Context, mockIStringLocalizer.Object,
                mockIMediator.Object, mockIErpAcademicoServiceClient.Object);
            mockIMediator.Setup(x => x.Send(It.IsAny<HasPrimeraMatriculaExpedienteQuery>(),
                It.IsAny<CancellationToken>())).ReturnsAsync(false);

            //ACT
            await sut.AddSeguimientoExpediente(mockExpedienteAlumno.Object, request, CancellationToken.None);

            //ASSERT
            mockExpedienteAlumno.Verify(s =>
                    s.AddSeguimientoNoUser(It.IsAny<int>(), It.IsAny<string>(), null,
                    It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            mockIMediator.Verify(x => x.Send(It.IsAny<HasPrimeraMatriculaExpedienteQuery>(),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        #endregion
    }
}
