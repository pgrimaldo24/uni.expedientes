using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Unir.Expedientes.Application.Common.Interfaces;
using Unir.Expedientes.Application.Common.Models.ErpAcademico.PlanesEstudio;
using Unir.Expedientes.Application.Common.Parameters;
using Unir.Expedientes.Application.ExpedientesAlumnos.Commands.EditExpedienteAlumno;
using Unir.Expedientes.Application.SeguimientosExpedientes.Commands.AddSeguimientoTitulacionAccesoUncommit;
using Unir.Expedientes.Application.Tests.Common;
using Unir.Expedientes.Domain.Entities;
using Unir.Framework.ApplicationSuperTypes.Exceptions;
using Unir.Framework.Crosscutting.Security;
using Unir.Framework.Crosscutting.Security.Model;
using Xunit;
using TitulacionAccesoParametersDto = Unir.Expedientes.Application.ExpedientesAlumnos.Commands.EditExpedienteAlumno.TitulacionAccesoParametersDto;

namespace Unir.Expedientes.Application.Tests.ExpedientesAlumnos.Commands.EditExpedienteAlumno
{
    [Collection("CommonTestCollection")]
    public class EditExpedienteAlumnoCommandHandlerTest : TestBase
    {
        #region Handle

        [Fact(DisplayName = "Cuando se edita pero la versión no es modificada Devuelve id")]
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
            var request = new EditExpedienteAlumnoCommand
            {
                IdExpedienteAlumno = 1,
                IdRefNodo = "123",
                IdRefVersionPlan = "123"
            };
            var mockIStringLocalizer = new Mock<IStringLocalizer<EditExpedienteAlumnoCommandHandler>>
            {
                CallBase = true
            };
            var mockIMediator = new Mock<IMediator>
            {
                CallBase = true
            };
            mockIMediator.Setup(s => s.Send(It.IsAny<ExpedienteAlumno>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Unit.Value);
            var mockIIdentityService = new Mock<IIdentityService>
            {
                CallBase = true
            };
            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };
            var sut = new Mock<EditExpedienteAlumnoCommandHandler>(Context, mockIStringLocalizer.Object, mockIMediator.Object,
                mockIIdentityService.Object, mockIErpAcademicoServiceClient.Object)
            {
                CallBase = true
            };
            sut.Setup(s => s.ValidatePropiedadesRequeridas(It.IsAny<EditExpedienteAlumnoCommand>()));
            sut.Setup(s => s.ModificarExpedienteAlumnoEnErp(It.Is<EditExpedienteAlumnoCommand>(e => e == request)))
                .Returns(Task.CompletedTask);
            sut.Setup(s =>
                s.AssignTitulacionAcceso(It.IsAny<TitulacionAccesoParametersDto>(), It.IsAny<ExpedienteAlumno>()));

            //ACT
            var actual = await sut.Object.Handle(request, CancellationToken.None);

            //ASSERT
            var expedientePersistido = await Context.ExpedientesAlumno.FirstAsync(CancellationToken.None);
            sut.Verify(s => s.ModificarExpedienteAlumnoEnErp(It.Is<EditExpedienteAlumnoCommand>(e => e == request)),
                Times.Once);
            sut.Setup(s =>
                s.AssignTitulacionAcceso(It.IsAny<TitulacionAccesoParametersDto>(), It.IsAny<ExpedienteAlumno>()));
            Assert.NotEqual(expedientePersistido.IdRefNodo, request.IdRefNodo);
            Assert.Equal(expedientePersistido.IdRefVersionPlan, request.IdRefVersionPlan);
            Assert.Equal(expedientePersistido.Id, actual);
        }

        [Fact(DisplayName = "Cuando se edita la versión se añade un seguimiento y Devuelve id")]
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
            var request = new EditExpedienteAlumnoCommand
            {
                IdExpedienteAlumno = 1,
                IdRefNodo = "123",
                IdRefVersionPlan = "789",
                TitulacionAcceso = new TitulacionAccesoParametersDto()
            };
            var mockIStringLocalizer = new Mock<IStringLocalizer<EditExpedienteAlumnoCommandHandler>>
            {
                CallBase = true
            };
            var mockIMediator = new Mock<IMediator>
            {
                CallBase = true
            };
            mockIMediator.Setup(s => s.Send(It.IsAny<ExpedienteAlumno>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Unit.Value);
            var mockIIdentityService = new Mock<IIdentityService>
            {
                CallBase = true
            };
            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };
            var sut = new Mock<EditExpedienteAlumnoCommandHandler>(Context, mockIStringLocalizer.Object, mockIMediator.Object,
                mockIIdentityService.Object, mockIErpAcademicoServiceClient.Object)
            {
                CallBase = true
            };
            sut.Setup(s => s.ValidatePropiedadesRequeridas(It.IsAny<EditExpedienteAlumnoCommand>()));
            sut.Setup(s =>
                    s.ModificarExpedienteAlumnoEnErp(It.Is<EditExpedienteAlumnoCommand>(e => e == request)))
                .Returns(Task.CompletedTask);
            sut.Setup(s => s.AssignExpedienteAlumno(It.IsAny<EditExpedienteAlumnoCommand>(),
                    It.IsAny<ExpedienteAlumno>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            //ACT
            var actual = await sut.Object.Handle(request, CancellationToken.None);

            //ASSERT
            var expedientePersistido = await Context.ExpedientesAlumno.FirstAsync(CancellationToken.None);
            sut.Verify(s => s.ValidatePropiedadesRequeridas(It.IsAny<EditExpedienteAlumnoCommand>()), Times.Once);
            sut.Verify(s => s.ModificarExpedienteAlumnoEnErp(It.Is<EditExpedienteAlumnoCommand>(e => e == request)),
                Times.Once);
            sut.Verify(s => s.AssignExpedienteAlumno(It.IsAny<EditExpedienteAlumnoCommand>(),
                It.IsAny<ExpedienteAlumno>(), It.IsAny<CancellationToken>()), Times.Once);
            Assert.NotEqual(expedientePersistido.IdRefNodo, request.IdRefNodo);
            Assert.Equal(expedientePersistido.Id, actual);
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
            var request = new EditExpedienteAlumnoCommand
            {
                IdExpedienteAlumno = 2,
                IdRefNodo = "456"
            };
            var mockIStringLocalizer = new Mock<IStringLocalizer<EditExpedienteAlumnoCommandHandler>>
            {
                CallBase = true
            };
            var mockIMediator = new Mock<IMediator>
            {
                CallBase = true
            };
            var mockIIdentityService = new Mock<IIdentityService>
            {
                CallBase = true
            };
            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };
            var sut = new EditExpedienteAlumnoCommandHandler(Context, mockIStringLocalizer.Object, mockIMediator.Object,
                mockIIdentityService.Object, mockIErpAcademicoServiceClient.Object);

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
            var request = new EditExpedienteAlumnoCommand
            {
                IdRefVersionPlan = "asdas"
            };
            var mockIStringLocalizer = new Mock<IStringLocalizer<EditExpedienteAlumnoCommandHandler>>
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
            var mockIIdentityService = new Mock<IIdentityService>
            {
                CallBase = true
            };
            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };
            var sut = new EditExpedienteAlumnoCommandHandler(Context, mockIStringLocalizer.Object, mockIMediator.Object,
                mockIIdentityService.Object, mockIErpAcademicoServiceClient.Object);

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
            var request = new EditExpedienteAlumnoCommand
            {
                IdRefVersionPlan = "1"
            };
            var mockIStringLocalizer = new Mock<IStringLocalizer<EditExpedienteAlumnoCommandHandler>>
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
            var mockIIdentityService = new Mock<IIdentityService>
            {
                CallBase = true
            };
            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };
            var sut = new EditExpedienteAlumnoCommandHandler(Context, mockIStringLocalizer.Object, mockIMediator.Object,
                mockIIdentityService.Object, mockIErpAcademicoServiceClient.Object);

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
            var request = new EditExpedienteAlumnoCommand
            {
                IdRefVersionPlan = "1",
                NroVersion = 2,
                IdRefNodo = "2s"
            };
            var mockIStringLocalizer = new Mock<IStringLocalizer<EditExpedienteAlumnoCommandHandler>>
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
            var mockIIdentityService = new Mock<IIdentityService>
            {
                CallBase = true
            };
            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };
            var sut = new EditExpedienteAlumnoCommandHandler(Context, mockIStringLocalizer.Object, mockIMediator.Object,
                mockIIdentityService.Object, mockIErpAcademicoServiceClient.Object);

            //ACT
            var exception = Record.Exception(() =>
            {
                sut.ValidatePropiedadesRequeridas(request);
            });

            //ASSERT
            Assert.IsType<BadRequestException>(exception);
            Assert.Equal(mensajeEsperado, exception.Message);
        }

        [Fact(DisplayName = "Cuando el título de la titulación de acceso es inválido Devuelve excepción")]
        public void ValidatePropiedadesRequeridas_Titulo()
        {
            //ARRANGE
            var request = new EditExpedienteAlumnoCommand
            {
                IdRefVersionPlan = "1",
                NroVersion = 2,
                IdRefNodo = "2",
                TitulacionAcceso = new TitulacionAccesoParametersDto
                {
                    InstitucionDocente = Guid.NewGuid().ToString()
                }
            };
            var mockIStringLocalizer = new Mock<IStringLocalizer<EditExpedienteAlumnoCommandHandler>>
            {
                CallBase = true
            };
            var mensajeEsperado = $"El campo Título es requerido para Editar el Expediente.";
            mockIStringLocalizer.Setup(s => s[It.Is<string>(msj => msj == mensajeEsperado)])
                .Returns(new LocalizedString(mensajeEsperado, mensajeEsperado));
            var mockIMediator = new Mock<IMediator>
            {
                CallBase = true
            };
            var mockIIdentityService = new Mock<IIdentityService>
            {
                CallBase = true
            };
            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };
            var sut = new EditExpedienteAlumnoCommandHandler(Context, mockIStringLocalizer.Object, mockIMediator.Object,
                mockIIdentityService.Object, mockIErpAcademicoServiceClient.Object);

            //ACT
            var exception = Record.Exception(() =>
            {
                sut.ValidatePropiedadesRequeridas(request);
            });

            //ASSERT
            Assert.IsType<BadRequestException>(exception);
            Assert.Equal(mensajeEsperado, exception.Message);
        }

        [Fact(DisplayName = "Cuando la institución docente de la titulación de acceso es inválido Devuelve excepción")]
        public void ValidatePropiedadesRequeridas_InstitucionDocente()
        {
            //ARRANGE
            var request = new EditExpedienteAlumnoCommand
            {
                IdRefVersionPlan = "1",
                NroVersion = 2,
                IdRefNodo = "2",
                TitulacionAcceso = new TitulacionAccesoParametersDto
                {
                    Titulo = Guid.NewGuid().ToString()
                }
            };
            var mockIStringLocalizer = new Mock<IStringLocalizer<EditExpedienteAlumnoCommandHandler>>
            {
                CallBase = true
            };
            var mensajeEsperado = $"El campo Institución Docente es requerido para Editar el Expediente.";
            mockIStringLocalizer.Setup(s => s[It.Is<string>(msj => msj == mensajeEsperado)])
                .Returns(new LocalizedString(mensajeEsperado, mensajeEsperado));
            var mockIMediator = new Mock<IMediator>
            {
                CallBase = true
            };
            var mockIIdentityService = new Mock<IIdentityService>
            {
                CallBase = true
            };
            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };
            var sut = new EditExpedienteAlumnoCommandHandler(Context, mockIStringLocalizer.Object, mockIMediator.Object,
                mockIIdentityService.Object, mockIErpAcademicoServiceClient.Object);

            //ACT
            var exception = Record.Exception(() =>
            {
                sut.ValidatePropiedadesRequeridas(request);
            });

            //ASSERT
            Assert.IsType<BadRequestException>(exception);
            Assert.Equal(mensajeEsperado, exception.Message);
        }

        [Fact(DisplayName = "Cuando el País de la Institución Docente difiere del País de la Ubicación Devuelve excepción")]
        public void ValidatePropiedadesRequeridas_Pais()
        {
            //ARRANGE
            var request = new EditExpedienteAlumnoCommand
            {
                IdRefVersionPlan = "1",
                NroVersion = 2,
                IdRefNodo = "2",
                TitulacionAcceso = new TitulacionAccesoParametersDto
                {
                    Titulo = Guid.NewGuid().ToString(),
                    InstitucionDocente = Guid.NewGuid().ToString(),
                    IdRefInstitucionDocente = "ES-05",
                    IdRefTerritorioInstitucionDocente = "FR-05-205-25"
                }
            };
            var mockIStringLocalizer = new Mock<IStringLocalizer<EditExpedienteAlumnoCommandHandler>>
            {
                CallBase = true
            };
            var mensajeEsperado = $"El País de la Ubicación tiene que ser el mismo que el País de la Institución Docente.";
            mockIStringLocalizer.Setup(s => s[It.Is<string>(msj => msj == mensajeEsperado)])
                .Returns(new LocalizedString(mensajeEsperado, mensajeEsperado));
            var mockIMediator = new Mock<IMediator>
            {
                CallBase = true
            };
            var mockIIdentityService = new Mock<IIdentityService>
            {
                CallBase = true
            };
            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };
            var sut = new EditExpedienteAlumnoCommandHandler(Context, mockIStringLocalizer.Object, mockIMediator.Object,
                mockIIdentityService.Object, mockIErpAcademicoServiceClient.Object);

            //ACT
            var exception = Record.Exception(() =>
            {
                sut.ValidatePropiedadesRequeridas(request);
            });

            //ASSERT
            Assert.IsType<BadRequestException>(exception);
            Assert.Equal(mensajeEsperado, exception.Message);
        }

        [Fact(DisplayName = "Cuando la Fecha de Inicio es mayor que la Fecha de Fin de la Titulación de Acceso Devuelve excepción")]
        public void ValidatePropiedadesRequeridas_Fechas()
        {
            //ARRANGE
            var request = new EditExpedienteAlumnoCommand
            {
                IdRefVersionPlan = "1",
                NroVersion = 2,
                IdRefNodo = "2",
                TitulacionAcceso = new TitulacionAccesoParametersDto
                {
                    Titulo = Guid.NewGuid().ToString(),
                    InstitucionDocente = Guid.NewGuid().ToString(),
                    IdRefInstitucionDocente = "ES-05",
                    IdRefTerritorioInstitucionDocente = "ES-05-205-25",
                    FechaInicioTitulo = DateTime.Now,
                    FechafinTitulo = DateTime.Now.AddDays(-4)
                }
            };
            var mockIStringLocalizer = new Mock<IStringLocalizer<EditExpedienteAlumnoCommandHandler>>
            {
                CallBase = true
            };
            var mensajeEsperado = $"El campo Fecha Inicio del Título no puede ser mayor al campo de Fecha Fin del Título.";
            mockIStringLocalizer.Setup(s => s[It.Is<string>(msj => msj == mensajeEsperado)])
                .Returns(new LocalizedString(mensajeEsperado, mensajeEsperado));
            var mockIMediator = new Mock<IMediator>
            {
                CallBase = true
            };
            var mockIIdentityService = new Mock<IIdentityService>
            {
                CallBase = true
            };
            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };
            var sut = new EditExpedienteAlumnoCommandHandler(Context, mockIStringLocalizer.Object, mockIMediator.Object,
                mockIIdentityService.Object, mockIErpAcademicoServiceClient.Object);

            //ACT
            var exception = Record.Exception(() =>
            {
                sut.ValidatePropiedadesRequeridas(request);
            });

            //ASSERT
            Assert.IsType<BadRequestException>(exception);
            Assert.Equal(mensajeEsperado, exception.Message);
        }

        [Fact(DisplayName = "Cuando se envían todos los campos requeridos Continua con el proceso")]
        public void ValidatePropiedadesRequeridas_Ok()
        {
            //ARRANGE
            var request = new EditExpedienteAlumnoCommand
            {
                IdRefVersionPlan = "1",
                NroVersion = 2,
                IdRefNodo = "2",
                TitulacionAcceso = new TitulacionAccesoParametersDto
                {
                    Titulo = Guid.NewGuid().ToString(),
                    InstitucionDocente = Guid.NewGuid().ToString(),
                    IdRefInstitucionDocente = "ES-05",
                    IdRefTerritorioInstitucionDocente = "ES-05-205-25",
                    FechaInicioTitulo = DateTime.Now.AddDays(-4),
                    FechafinTitulo = DateTime.Now
                }
            };
            var mockIStringLocalizer = new Mock<IStringLocalizer<EditExpedienteAlumnoCommandHandler>>
            {
                CallBase = true
            };
            var mockIMediator = new Mock<IMediator>
            {
                CallBase = true
            };
            var mockIIdentityService = new Mock<IIdentityService>
            {
                CallBase = true
            };
            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };
            var sut = new EditExpedienteAlumnoCommandHandler(Context, mockIStringLocalizer.Object, mockIMediator.Object,
                mockIIdentityService.Object, mockIErpAcademicoServiceClient.Object);

            //ACT
            sut.ValidatePropiedadesRequeridas(request);

            //ASSERT
            Assert.True(int.TryParse(request.IdRefVersionPlan, out _));
            Assert.True(int.TryParse(request.IdRefNodo, out _));
        }

        #endregion

        #region ModificarExpedienteAlumnoEnErp

        [Fact(DisplayName = "Cuando se modifica solo la vía de acceso en erp Devuelve ok")]
        public async Task ModificarExpedienteAlumnoEnErp_ViaAcceso_Ok()
        {
            //ARRANGE
            var request = new EditExpedienteAlumnoCommand
            {
                IdRefViaAcceso = "1",
                IdExpedienteAlumno = 10
            };
            var mockIStringLocalizer = new Mock<IStringLocalizer<EditExpedienteAlumnoCommandHandler>>
            {
                CallBase = true
            };
            var mockIMediator = new Mock<IMediator>
            {
                CallBase = true
            };
            var mockIIdentityService = new Mock<IIdentityService>
            {
                CallBase = true
            };
            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };
            mockIErpAcademicoServiceClient
                .Setup(s => s.ModifyExpedienteByIdIntegracion(
                    It.Is<string>(i => i == request.IdExpedienteAlumno.ToString()),
                    It.IsAny<ExpedienteEditParameters>()))
                .ReturnsAsync(Unit.Value);
            var sut = new EditExpedienteAlumnoCommandHandler(Context, mockIStringLocalizer.Object, mockIMediator.Object,
                mockIIdentityService.Object, mockIErpAcademicoServiceClient.Object);

            //ACT
            await sut.ModificarExpedienteAlumnoEnErp(request);

            //ASSERT
            mockIErpAcademicoServiceClient.Verify(s => s.ModifyExpedienteByIdIntegracion(
                It.Is<string>(i => i == request.IdExpedienteAlumno.ToString()),
                It.IsAny<ExpedienteEditParameters>()), Times.Once);
        }

        [Fact(DisplayName = "Cuando se modifica el expediente en erp Devuelve ok")]
        public async Task ModificarExpedienteAlumnoEnErp_Ok()
        {
            //ARRANGE
            var request = new EditExpedienteAlumnoCommand
            {
                IdRefViaAcceso = "1",
                TitulacionAcceso = new TitulacionAccesoParametersDto
                {
                    InstitucionDocente = Guid.NewGuid().ToString(),
                    NroSemestreRealizados = 10,
                    Titulo = Guid.NewGuid().ToString()
                },
                IdExpedienteAlumno = 10
            };
            var mockIStringLocalizer = new Mock<IStringLocalizer<EditExpedienteAlumnoCommandHandler>>
            {
                CallBase = true
            };
            var mockIMediator = new Mock<IMediator>
            {
                CallBase = true
            };
            var mockIIdentityService = new Mock<IIdentityService>
            {
                CallBase = true
            };
            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };
            mockIErpAcademicoServiceClient
                .Setup(s => s.ModifyExpedienteByIdIntegracion(
                    It.Is<string>(i => i == request.IdExpedienteAlumno.ToString()),
                    It.IsAny<ExpedienteEditParameters>()))
                .ReturnsAsync(Unit.Value);
            var sut = new EditExpedienteAlumnoCommandHandler(Context, mockIStringLocalizer.Object, mockIMediator.Object,
                mockIIdentityService.Object, mockIErpAcademicoServiceClient.Object);

            //ACT
            await sut.ModificarExpedienteAlumnoEnErp(request);

            //ASSERT
            mockIErpAcademicoServiceClient.Verify(s => s.ModifyExpedienteByIdIntegracion(
                It.Is<string>(i => i == request.IdExpedienteAlumno.ToString()),
                It.IsAny<ExpedienteEditParameters>()), Times.Once);
        }

        #endregion

        #region AssignExpedienteAlumno

        [Fact(DisplayName = "Cuando se asignan los valores al expediente Devuelve entidad seteada")]
        public async Task AssignExpedienteAlumno_Ok()
        {
            //ARRANGE
            var request = new EditExpedienteAlumnoCommand
            {
                IdRefVersionPlan = "789",
                IdRefNodo = "159"
            };
            var mockIStringLocalizer = new Mock<IStringLocalizer<EditExpedienteAlumnoCommandHandler>>
            {
                CallBase = true
            };
            var mockIMediator = new Mock<IMediator>
            {
                CallBase = true
            };
            var mockIIdentityService = new Mock<IIdentityService>
            {
                CallBase = true
            };
            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };
            var sutMock = new Mock<EditExpedienteAlumnoCommandHandler>(Context, mockIStringLocalizer.Object,
                mockIMediator.Object,
                mockIIdentityService.Object, mockIErpAcademicoServiceClient.Object)
            {
                CallBase = true
            };

            sutMock.Setup(s => s.AddSeguimientoVersionPlan(
                It.IsAny<ExpedienteAlumno>(), It.IsAny<EditExpedienteAlumnoCommand>())).Returns(true);
            var idRefViaAccesoPlan = Guid.NewGuid().ToString();
            sutMock.Setup(s => s.AddSeguimientoViaAcceso(
                It.IsAny<ExpedienteAlumno>(), It.IsAny<EditExpedienteAlumnoCommand>()))
                .ReturnsAsync(idRefViaAccesoPlan);
            sutMock.Setup(s => s.AddSeguimientoTitulacionAcceso(
                It.IsAny<ExpedienteAlumno>(), It.IsAny<EditExpedienteAlumnoCommand>(), 
                It.IsAny<CancellationToken>())).ReturnsAsync(true);
            sutMock.Setup(s => s.AssignTitulacionAcceso(
                It.IsAny<TitulacionAccesoParametersDto>(), It.IsAny<ExpedienteAlumno>()));

            var expedienteEsperado = new ExpedienteAlumno();

            //ACT
            await sutMock.Object.AssignExpedienteAlumno(request, expedienteEsperado, CancellationToken.None);

            //ASSERT
            Assert.Equal(expedienteEsperado.IdRefVersionPlan, request.IdRefVersionPlan);
            Assert.Equal(expedienteEsperado.IdRefViaAccesoPlan, idRefViaAccesoPlan);
            sutMock.Verify(s => s.AddSeguimientoVersionPlan(
                    It.IsAny<ExpedienteAlumno>(), It.IsAny<EditExpedienteAlumnoCommand>()),
                Times.Once);
            sutMock.Verify(s => s.AddSeguimientoViaAcceso(
                It.IsAny<ExpedienteAlumno>(), It.IsAny<EditExpedienteAlumnoCommand>()), Times.Once);
            sutMock.Verify(s => s.AddSeguimientoTitulacionAcceso(
                It.IsAny<ExpedienteAlumno>(), It.IsAny<EditExpedienteAlumnoCommand>(),
                It.IsAny<CancellationToken>()), Times.Once);
            sutMock.Verify(s => s.AssignTitulacionAcceso(
                It.IsAny<TitulacionAccesoParametersDto>(), It.IsAny<ExpedienteAlumno>()), Times.Once);
        }

        #endregion

        #region AssignTitulacionAcceso

        [Fact(DisplayName = "Cuando se asignan los valores a la titulacion de acceso Devuelve entidad seteada")]
        public void AssignTitulacionAcceso_Ok()
        {
            //ARRANGE
            var request = new TitulacionAccesoParametersDto
            {
                Titulo = Guid.NewGuid().ToString(),
                CodigoColegiadoProfesional = Guid.NewGuid().ToString(),
                FechaInicioTitulo = DateTime.Now,
                FechafinTitulo = DateTime.Now,
                IdRefTerritorioInstitucionDocente = "56468",
                InstitucionDocente = Guid.NewGuid().ToString(),
                NroSemestreRealizados = 5,
                TipoEstudio = Guid.NewGuid().ToString(),
                IdRefInstitucionDocente = Guid.NewGuid().ToString(),
            };

            var mockIStringLocalizer = new Mock<IStringLocalizer<EditExpedienteAlumnoCommandHandler>>
            {
                CallBase = true
            };
            var mockIMediator = new Mock<IMediator>
            {
                CallBase = true
            };
            var mockIIdentityService = new Mock<IIdentityService>
            {
                CallBase = true
            };
            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };
            var sut = new EditExpedienteAlumnoCommandHandler(Context, mockIStringLocalizer.Object, mockIMediator.Object,
                mockIIdentityService.Object, mockIErpAcademicoServiceClient.Object);

            var expedienteEsperado = new ExpedienteAlumno();

            //ACT
            sut.AssignTitulacionAcceso(request, expedienteEsperado);

            //ASSERT
            Assert.Equal(expedienteEsperado.TitulacionAcceso.Titulo, request.Titulo);
            Assert.Equal(expedienteEsperado.TitulacionAcceso.CodigoColegiadoProfesional, request.CodigoColegiadoProfesional);
            Assert.Equal(expedienteEsperado.TitulacionAcceso.FechaInicioTitulo, request.FechaInicioTitulo);
            Assert.Equal(expedienteEsperado.TitulacionAcceso.FechafinTitulo, request.FechafinTitulo);
            Assert.Equal(expedienteEsperado.TitulacionAcceso.IdRefTerritorioInstitucionDocente, request.IdRefTerritorioInstitucionDocente);
            Assert.Equal(expedienteEsperado.TitulacionAcceso.InstitucionDocente, request.InstitucionDocente);
            Assert.Equal(expedienteEsperado.TitulacionAcceso.NroSemestreRealizados, request.NroSemestreRealizados);
            Assert.Equal(expedienteEsperado.TitulacionAcceso.TipoEstudio, request.TipoEstudio);
            Assert.Equal(expedienteEsperado.TitulacionAcceso.IdRefInstitucionDocente, request.IdRefInstitucionDocente);
        }

        #endregion

        #region AddSeguimientoVersionPlan

        [Trait("Category", "AddSeguimientoVersionPlan")]
        [Fact(DisplayName = "Cuando no se agrega el seguimiento de versión de plan Retorna False")]
        public void AddSeguimientoVersionPlan_False()
        {
            //ARRANGE
            var mockIStringLocalizer = new Mock<IStringLocalizer<EditExpedienteAlumnoCommandHandler>>
            {
                CallBase = true
            };
            var mockIMediator = new Mock<IMediator>
            {
                CallBase = true
            };
            var mockIIdentityService = new Mock<IIdentityService>
            {
                CallBase = true
            };
            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };
            var sut = new EditExpedienteAlumnoCommandHandler(Context, mockIStringLocalizer.Object, mockIMediator.Object,
                mockIIdentityService.Object, mockIErpAcademicoServiceClient.Object);

            var mockExpedienteAlumno = new Mock<ExpedienteAlumno>
            {
                CallBase = true
            };
            mockExpedienteAlumno.SetupAllProperties();
            var expedienteAlumno = mockExpedienteAlumno.Object;
            expedienteAlumno.IdRefVersionPlan = "1";
            var request = new EditExpedienteAlumnoCommand
            {
                IdRefVersionPlan = "1"
            };

            //ACT
            var actual = sut.AddSeguimientoVersionPlan(expedienteAlumno, request);

            //ASSERT
            Assert.False(actual);
            mockExpedienteAlumno.Verify(x => x.AddSeguimiento(It.IsAny<int>(),
                It.IsAny<string>(), It.IsAny<string>(),null,
                    It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            mockIIdentityService.Verify(s => s.GetUserIdentityInfo(), Times.Never);
        }

        [Trait("Category", "AddSeguimientoVersionPlan")]
        [Fact(DisplayName = "Cuando se agrega el seguimiento de versión de plan Retorna True")]
        public void AddSeguimientoVersionPlan_True()
        {
            //ARRANGE
            var mockIStringLocalizer = new Mock<IStringLocalizer<EditExpedienteAlumnoCommandHandler>>
            {
                CallBase = true
            };
            var mockIMediator = new Mock<IMediator>
            {
                CallBase = true
            };
            var mockIIdentityService = new Mock<IIdentityService>
            {
                CallBase = true
            };
            mockIIdentityService.Setup(s => s.GetUserIdentityInfo()).Returns(new IdentityModel
            {
                Id = Guid.NewGuid().ToString()
            });
            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };
            var sut = new EditExpedienteAlumnoCommandHandler(Context, mockIStringLocalizer.Object, mockIMediator.Object,
                mockIIdentityService.Object, mockIErpAcademicoServiceClient.Object);

            var mockExpedienteAlumno = new Mock<ExpedienteAlumno>
            {
                CallBase = true
            };
            mockExpedienteAlumno.SetupAllProperties();
            var expedienteAlumno = mockExpedienteAlumno.Object;
            mockExpedienteAlumno.Setup(x => x.AddSeguimiento(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), null,
                    It.IsAny<string>(), It.IsAny<string>()));
            var request = new EditExpedienteAlumnoCommand
            {
                IdRefVersionPlan = Guid.NewGuid().ToString()
            };

            //ACT
            var actual = sut.AddSeguimientoVersionPlan(expedienteAlumno, request);

            //ASSERT
            Assert.True(actual);
            mockExpedienteAlumno.Verify(x => x.AddSeguimiento(It.IsAny<int>(), 
                It.IsAny<string>(), It.IsAny<string>(), null,
                    It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            mockIIdentityService.Verify(s => s.GetUserIdentityInfo(), Times.Once);
        }

        #endregion

        #region AddSeguimientoViaAcceso

        [Trait("Category", "AddSeguimientoViaAcceso")]
        [Fact(DisplayName = "Cuando se agrega el seguimiento de vía de acceso Retorna Id via acceso")]
        public async Task AddSeguimientoViaAcceso_Ok()
        {
            //ARRANGE
            var mockIStringLocalizer = new Mock<IStringLocalizer<EditExpedienteAlumnoCommandHandler>>
            {
                CallBase = true
            };
            var mockIMediator = new Mock<IMediator>
            {
                CallBase = true
            };
            var mockIIdentityService = new Mock<IIdentityService>
            {
                CallBase = true
            };
            mockIIdentityService.Setup(s => s.GetUserIdentityInfo()).Returns(new IdentityModel
            {
                Id = Guid.NewGuid().ToString()
            });
            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };
            mockIErpAcademicoServiceClient.Setup(x =>
                    x.GetViasAccesosPlanes(It.IsAny<ViaAccesoPlanListParameters>()))
                .ReturnsAsync(new List<ViaAccesoPlanAcademicoModel>
                {
                    new()
                    {
                        Id = 1
                    }
                });

            var sut = new EditExpedienteAlumnoCommandHandler(Context, mockIStringLocalizer.Object, mockIMediator.Object,
                mockIIdentityService.Object, mockIErpAcademicoServiceClient.Object);

            var mockExpedienteAlumno = new Mock<ExpedienteAlumno>
            {
                CallBase = true
            };
            mockExpedienteAlumno.SetupAllProperties();
            var expedienteAlumno = mockExpedienteAlumno.Object;
            mockExpedienteAlumno.Setup(x => x.AddSeguimiento(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), null,
                    It.IsAny<string>(), It.IsAny<string>()));
            var request = new EditExpedienteAlumnoCommand
            {
                IdRefViaAcceso = "1"
            };

            //ACT
            var actual = await sut.AddSeguimientoViaAcceso(expedienteAlumno, request);

            //ASSERT
            Assert.NotNull(actual);
            mockExpedienteAlumno.Verify(x => x.AddSeguimiento(It.IsAny<int>(),
                It.IsAny<string>(), It.IsAny<string>(), null,
                    It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            mockIIdentityService.Verify(s => s.GetUserIdentityInfo(), Times.Once);
            mockIErpAcademicoServiceClient.Verify(x =>
                x.GetViasAccesosPlanes(It.IsAny<ViaAccesoPlanListParameters>()), Times.Once);
        }

        #endregion

        #region AddSeguimientoTitulacionAcceso

        [Trait("Category", "AddSeguimientoTitulacionAcceso")]
        [Fact(DisplayName = "Cuando la titulación de acceso es null No se agrega el seguimiento")]
        public async Task AddSeguimientoTitulacionAcceso_Null()
        {
            //ARRANGE
            var mockIStringLocalizer = new Mock<IStringLocalizer<EditExpedienteAlumnoCommandHandler>>
            {
                CallBase = true
            };
            var mockIMediator = new Mock<IMediator>
            {
                CallBase = true
            };
            var mockIIdentityService = new Mock<IIdentityService>
            {
                CallBase = true
            };
            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };
            var sutMock = new EditExpedienteAlumnoCommandHandler(Context, mockIStringLocalizer.Object, mockIMediator.Object,
                mockIIdentityService.Object, mockIErpAcademicoServiceClient.Object);

            var expedienteAlumno = new ExpedienteAlumno();
            var request = new EditExpedienteAlumnoCommand();

            //ACT
            var actual = await sutMock.AddSeguimientoTitulacionAcceso(expedienteAlumno, request, CancellationToken.None);

            //ASSERT
            Assert.False(actual);
            Assert.Null(request.TitulacionAcceso);
        }


        [Trait("Category", "AddSeguimientoTitulacionAcceso")]
        [Fact(DisplayName = "Cuando se agrega el seguimiento de titulación de acceso Retorna True")]
        public async Task AddSeguimientoTitulacionAcceso_True()
        {
            //ARRANGE
            var mockIStringLocalizer = new Mock<IStringLocalizer<EditExpedienteAlumnoCommandHandler>>
            {
                CallBase = true
            };
            var mockIMediator = new Mock<IMediator>
            {
                CallBase = true
            };
            var mockIIdentityService = new Mock<IIdentityService>
            {
                CallBase = true
            };
            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };
            var sutMock = new Mock<EditExpedienteAlumnoCommandHandler>(Context, mockIStringLocalizer.Object, mockIMediator.Object,
                mockIIdentityService.Object, mockIErpAcademicoServiceClient.Object)
            {
                CallBase = true
            };

            mockIMediator.Setup(s => s.Send(It.IsAny<AddSeguimientoTitulacionAccesoUncommitCommand>(),
                    It.IsAny<CancellationToken>())).ReturnsAsync(true);

            var expedienteAlumno = new ExpedienteAlumno
            {
                Id = 1
            };
            var request = new EditExpedienteAlumnoCommand
            {
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
        public async Task AddSeguimientoTitulacionAcceso_False()
        {
            //ARRANGE
            var mockIStringLocalizer = new Mock<IStringLocalizer<EditExpedienteAlumnoCommandHandler>>
            {
                CallBase = true
            };
            var mockIMediator = new Mock<IMediator>
            {
                CallBase = true
            };
            var mockIIdentityService = new Mock<IIdentityService>
            {
                CallBase = true
            };
            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };
            var sutMock = new Mock<EditExpedienteAlumnoCommandHandler>(Context, mockIStringLocalizer.Object, mockIMediator.Object,
                mockIIdentityService.Object, mockIErpAcademicoServiceClient.Object)
            {
                CallBase = true
            };

            mockIMediator.Setup(s => s.Send(It.IsAny<AddSeguimientoTitulacionAccesoUncommitCommand>(),
                    It.IsAny<CancellationToken>())).ReturnsAsync(false);

            var expedienteAlumno = new ExpedienteAlumno
            {
                Id = 1
            };
            var request = new EditExpedienteAlumnoCommand
            {
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
        }

        #endregion
    }
}
