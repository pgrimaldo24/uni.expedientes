using MediatR;
using Microsoft.Extensions.Localization;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Unir.Expedientes.Application.ExpedientesAlumnos.Commands.EditTitulacionAccesoExpedienteAlumno;
using Unir.Expedientes.Application.SeguimientosExpedientes.Commands.AddSeguimientoTitulacionAccesoUncommit;
using Unir.Expedientes.Application.Tests.Common;
using Unir.Expedientes.Domain.Entities;
using Unir.Framework.ApplicationSuperTypes.Exceptions;
using Xunit;

namespace Unir.Expedientes.Application.Tests.ExpedientesAlumnos.Commands.EditTitulacionAccesoExpedienteAlumno
{
    [Collection("CommonTestCollection")]
    public class EditTitulacionAccesoExpedienteAlumnoCommandHandlerTest : TestBase
    {
        #region Handle

        [Fact(DisplayName = "Cuando no se ha especificado los valores requeridos No realiza ninguna acción")]
        public async Task Handle_IsRequestValidaParaProcederConEdicion()
        {
            //ARRANGE
            var request =
                new EditTitulacionAccesoExpedienteAlumnoCommand(1, Guid.NewGuid().ToString(), Guid.NewGuid().ToString(),
                    null, Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), DateTime.Now, DateTime.Now.AddDays(10),
                    Guid.NewGuid().ToString(), Guid.NewGuid().ToString());
            var mockIstringLocalizer = new Mock<IStringLocalizer<EditTitulacionAccesoExpedienteAlumnoCommandHandler>>
            {
                CallBase = true
            };
            var mockIMediator = new Mock<IMediator>
            {
                CallBase = true
            };
            var sut = new Mock<EditTitulacionAccesoExpedienteAlumnoCommandHandler>(Context, mockIstringLocalizer.Object, mockIMediator.Object)
            {
                CallBase = true
            };

            sut.Setup(s =>
                    s.IsRequestValidaParaProcederConEdicion(It.IsAny<EditTitulacionAccesoExpedienteAlumnoCommand>()))
                .Returns(false);

            //ACT
            await sut.Object.Handle(request, CancellationToken.None);

            //ASSERT
            sut.Verify(s =>
                s.IsRequestValidaParaProcederConEdicion(It.IsAny<EditTitulacionAccesoExpedienteAlumnoCommand>()), Times.Once);
        }

        [Fact(DisplayName =
            "Cuando se intenta editar la titulación de acceso de un expediente inexistente Devuelve error")]
        public async Task Handle_NotFoundException()
        {
            //ARRANGE
            var request =
                new EditTitulacionAccesoExpedienteAlumnoCommand(1, Guid.NewGuid().ToString(), Guid.NewGuid().ToString(),
                    null, Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), 
                    DateTime.Now, DateTime.Now.AddDays(10), Guid.NewGuid().ToString(), 
                    Guid.NewGuid().ToString());
            var mockIstringLocalizer = new Mock<IStringLocalizer<EditTitulacionAccesoExpedienteAlumnoCommandHandler>>
            {
                CallBase = true
            };
            var mockIMediator = new Mock<IMediator>
            {
                CallBase = true
            };
            var sut = new Mock<EditTitulacionAccesoExpedienteAlumnoCommandHandler>(Context, mockIstringLocalizer.Object, mockIMediator.Object)
            {
                CallBase = true
            };

            sut.Setup(s =>
                    s.IsRequestValidaParaProcederConEdicion(It.IsAny<EditTitulacionAccesoExpedienteAlumnoCommand>()))
                .Returns(true);
            sut.Setup(s => s.ValidateDatos(It.IsAny<EditTitulacionAccesoExpedienteAlumnoCommand>()));

            //ACT
            var exception = await Record.ExceptionAsync(async () =>
            {
                await sut.Object.Handle(request, CancellationToken.None);
            });

            //ASSERT
            Assert.IsType<NotFoundException>(exception);
            Assert.Equal(new NotFoundException(nameof(ExpedienteAlumno), request.IdExpedienteAlumno).Message,
                exception.Message);
            sut.Verify(s => s.IsRequestValidaParaProcederConEdicion(
                It.IsAny<EditTitulacionAccesoExpedienteAlumnoCommand>()), Times.Once);
            sut.Verify(s => s.ValidateDatos(
                It.IsAny<EditTitulacionAccesoExpedienteAlumnoCommand>()), Times.Once);
        }

        [Fact(DisplayName =
            "Cuando se intenta editar la titulación de acceso y no existen diferencias entonces no se asignan datos y Retorna ok")]
        public async Task Handle_AssignTitulacionAcceso_False()
        {
            //ARRANGE
            var request =
                new EditTitulacionAccesoExpedienteAlumnoCommand(1, Guid.NewGuid().ToString(), Guid.NewGuid().ToString(),
                    null, Guid.NewGuid().ToString(), Guid.NewGuid().ToString(),
                    DateTime.Now, DateTime.Now.AddDays(10), Guid.NewGuid().ToString(),
                    Guid.NewGuid().ToString());
            var mockIstringLocalizer = new Mock<IStringLocalizer<EditTitulacionAccesoExpedienteAlumnoCommandHandler>>
            {
                CallBase = true
            };
            var mockIMediator = new Mock<IMediator>
            {
                CallBase = true
            };
            var sut = new Mock<EditTitulacionAccesoExpedienteAlumnoCommandHandler>(Context, mockIstringLocalizer.Object, mockIMediator.Object)
            {
                CallBase = true
            };

            sut.Setup(s =>
                    s.IsRequestValidaParaProcederConEdicion(It.IsAny<EditTitulacionAccesoExpedienteAlumnoCommand>()))
                .Returns(true);
            sut.Setup(s => s.ValidateDatos(It.IsAny<EditTitulacionAccesoExpedienteAlumnoCommand>()));
            sut.Setup(s => s.AddSeguimientoTitulacionAcceso(It.IsAny<ExpedienteAlumno>(),
                It.IsAny<EditTitulacionAccesoExpedienteAlumnoCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(false);

            var expediente = new ExpedienteAlumno
            {
                Id = 1,
                TitulacionAcceso = new TitulacionAcceso()
            };
            await Context.ExpedientesAlumno.AddAsync(expediente, CancellationToken.None);
            await Context.SaveChangesAsync(CancellationToken.None);

            //ACT
            await sut.Object.Handle(request, CancellationToken.None);

            //ASSERT
            sut.Verify(s => s.IsRequestValidaParaProcederConEdicion(
                It.IsAny<EditTitulacionAccesoExpedienteAlumnoCommand>()), Times.Once);
            sut.Verify(s => s.ValidateDatos(
                    It.IsAny<EditTitulacionAccesoExpedienteAlumnoCommand>()), Times.Once);
            sut.Verify(s => s.AssignTitulacionAcceso(
                It.IsAny<EditTitulacionAccesoExpedienteAlumnoCommand>(), It.IsAny<ExpedienteAlumno>()), Times.Never);
        }

        [Fact(DisplayName =
            "Cuando se intenta editar la titulación de acceso y existen diferencias entonces se asignan datos y Retorna ok")]
        public async Task Handle_AssignTitulacionAcceso_True()
        {
            //ARRANGE
            var request =
                new EditTitulacionAccesoExpedienteAlumnoCommand(1, Guid.NewGuid().ToString(), Guid.NewGuid().ToString(),
                    null, Guid.NewGuid().ToString(), Guid.NewGuid().ToString(),
                    DateTime.Now, DateTime.Now.AddDays(10), Guid.NewGuid().ToString(),
                    Guid.NewGuid().ToString());
            var mockIstringLocalizer = new Mock<IStringLocalizer<EditTitulacionAccesoExpedienteAlumnoCommandHandler>>
            {
                CallBase = true
            };
            var mockIMediator = new Mock<IMediator>
            {
                CallBase = true
            };
            var sut = new Mock<EditTitulacionAccesoExpedienteAlumnoCommandHandler>(Context, mockIstringLocalizer.Object, mockIMediator.Object)
            {
                CallBase = true
            };

            sut.Setup(s =>
                    s.IsRequestValidaParaProcederConEdicion(It.IsAny<EditTitulacionAccesoExpedienteAlumnoCommand>()))
                .Returns(true);
            sut.Setup(s => s.ValidateDatos(It.IsAny<EditTitulacionAccesoExpedienteAlumnoCommand>()));
            sut.Setup(s => s.AddSeguimientoTitulacionAcceso(It.IsAny<ExpedienteAlumno>(),
                It.IsAny<EditTitulacionAccesoExpedienteAlumnoCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);
            sut.Setup(s => s.AssignTitulacionAcceso(
                It.IsAny<EditTitulacionAccesoExpedienteAlumnoCommand>(), It.IsAny<ExpedienteAlumno>()));

            var expediente = new ExpedienteAlumno
            {
                Id = 1,
                TitulacionAcceso = new TitulacionAcceso()
            };
            await Context.ExpedientesAlumno.AddAsync(expediente, CancellationToken.None);
            await Context.SaveChangesAsync(CancellationToken.None);

            //ACT
            await sut.Object.Handle(request, CancellationToken.None);

            //ASSERT
            sut.Verify(s => s.IsRequestValidaParaProcederConEdicion(
                It.IsAny<EditTitulacionAccesoExpedienteAlumnoCommand>()), Times.Once);
            sut.Verify(s => s.ValidateDatos(
                    It.IsAny<EditTitulacionAccesoExpedienteAlumnoCommand>()), Times.Once);
            sut.Verify(s => s.AssignTitulacionAcceso(
                It.IsAny<EditTitulacionAccesoExpedienteAlumnoCommand>(), It.IsAny<ExpedienteAlumno>()), Times.Once);
        }
        #endregion

        #region IsRequestValidaParaProcederConEdicion

        [Fact(DisplayName = "Cuando la request es válida para proceder con la edición Devuelve true")]
        public void IsRequestValidaParaProcederConEdicion_True()
        {
            //ARRANGE
            var request =
                new EditTitulacionAccesoExpedienteAlumnoCommand(1, Guid.NewGuid().ToString(), Guid.NewGuid().ToString(),
                    null, Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), DateTime.Now, DateTime.Now.AddDays(10),
                    Guid.NewGuid().ToString(), Guid.NewGuid().ToString());
            var mockIstringLocalizer = new Mock<IStringLocalizer<EditTitulacionAccesoExpedienteAlumnoCommandHandler>>
            {
                CallBase = true
            };
            var mockIMediator = new Mock<IMediator>
            {
                CallBase = true
            };
            var sut = new EditTitulacionAccesoExpedienteAlumnoCommandHandler(Context, mockIstringLocalizer.Object, mockIMediator.Object);

            //ACT
            var actual = sut.IsRequestValidaParaProcederConEdicion(request);

            //ASSERT
            Assert.True(actual);
        }

        [Fact(DisplayName = "Cuando la request no es válida para proceder con la edición Devuelve false")]
        public void IsRequestValidaParaProcederConEdicion_False()
        {
            //ARRANGE
            var request =
                new EditTitulacionAccesoExpedienteAlumnoCommand(1, null, null,
                    null, Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), DateTime.Now, DateTime.Now.AddDays(10),
                    Guid.NewGuid().ToString(), Guid.NewGuid().ToString());
            var mockIstringLocalizer = new Mock<IStringLocalizer<EditTitulacionAccesoExpedienteAlumnoCommandHandler>>
            {
                CallBase = true
            };
            var mockIMediator = new Mock<IMediator>
            {
                CallBase = true
            };
            var sut = new EditTitulacionAccesoExpedienteAlumnoCommandHandler(Context, mockIstringLocalizer.Object, mockIMediator.Object);

            //ACT
            var actual = sut.IsRequestValidaParaProcederConEdicion(request);

            //ASSERT
            Assert.False(actual);
        }

        #endregion

        #region ValidateDatos

        [Fact(DisplayName = "Cuando falta especificar el campo institución docente Devuelve error")]
        public void ValidateDatos_InstitucionDocente()
        {
            //ARRANGE
            var request =
                new EditTitulacionAccesoExpedienteAlumnoCommand(1, Guid.NewGuid().ToString(), "",
                    null, Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), DateTime.Now, DateTime.Now.AddDays(10),
                    Guid.NewGuid().ToString(), Guid.NewGuid().ToString());
            var mockIstringLocalizer = new Mock<IStringLocalizer<EditTitulacionAccesoExpedienteAlumnoCommandHandler>>
            {
                CallBase = true
            };
            const string mensajeEsperado = $"El campo {nameof(request.InstitucionDocente)} es requerido para editar la Titulación de Acceso.";
            mockIstringLocalizer.Setup(s => s[It.Is<string>(msj => msj == mensajeEsperado)])
                .Returns(new LocalizedString(mensajeEsperado, mensajeEsperado));
            var mockIMediator = new Mock<IMediator>
            {
                CallBase = true
            };
            var sut = new EditTitulacionAccesoExpedienteAlumnoCommandHandler(Context, mockIstringLocalizer.Object, mockIMediator.Object);

            //ACT
            var exception = Record.Exception(() =>
            {
                sut.ValidateDatos(request);
            });

            //ASSERT
            Assert.IsType<BadRequestException>(exception);
            Assert.Equal(mensajeEsperado, exception.Message);
            mockIstringLocalizer.Verify(s => s[It.Is<string>(msj => msj == mensajeEsperado)], Times.Once);
        }

        [Fact(DisplayName = "Cuando falta especificar el campo título  Devuelve error")]
        public void ValidateDatos_Titulo()
        {
            //ARRANGE
            var request =
                new EditTitulacionAccesoExpedienteAlumnoCommand(1, null, Guid.NewGuid().ToString(),
                    null, Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), DateTime.Now, DateTime.Now.AddDays(10),
                    Guid.NewGuid().ToString(), Guid.NewGuid().ToString());
            var mockIstringLocalizer = new Mock<IStringLocalizer<EditTitulacionAccesoExpedienteAlumnoCommandHandler>>
            {
                CallBase = true
            };
            const string mensajeEsperado = $"El campo {nameof(request.Titulo)} es requerido para editar la Titulación de Acceso.";
            mockIstringLocalizer.Setup(s => s[It.Is<string>(msj => msj == mensajeEsperado)])
                .Returns(new LocalizedString(mensajeEsperado, mensajeEsperado));
            var mockIMediator = new Mock<IMediator>
            {
                CallBase = true
            };
            var sut = new EditTitulacionAccesoExpedienteAlumnoCommandHandler(Context, mockIstringLocalizer.Object, mockIMediator.Object);

            //ACT
            var exception = Record.Exception(() =>
            {
                sut.ValidateDatos(request);
            });

            //ASSERT
            Assert.IsType<BadRequestException>(exception);
            Assert.Equal(mensajeEsperado, exception.Message);
            mockIstringLocalizer.Verify(s => s[It.Is<string>(msj => msj == mensajeEsperado)], Times.Once);
        }

        [Fact(DisplayName = "Cuando el campo de la ubicación y del territorio de la institución docente sin diferentes Devuelve error")]
        public void ValidateDatos_Ubicacion()
        {
            //ARRANGE
            var request =
                new EditTitulacionAccesoExpedienteAlumnoCommand(1, Guid.NewGuid().ToString(), Guid.NewGuid().ToString(),
                    null, Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), DateTime.Now, DateTime.Now.AddDays(10),
                    Guid.NewGuid().ToString(), Guid.NewGuid().ToString());
            var mockIstringLocalizer = new Mock<IStringLocalizer<EditTitulacionAccesoExpedienteAlumnoCommandHandler>>
            {
                CallBase = true
            };
            const string mensajeEsperado = $"El País de la Ubicación tiene que ser el mismo que el País de la Institución Docente.";
            mockIstringLocalizer.Setup(s => s[It.Is<string>(msj => msj == mensajeEsperado)])
                .Returns(new LocalizedString(mensajeEsperado, mensajeEsperado));
            var mockIMediator = new Mock<IMediator>
            {
                CallBase = true
            };
            var sut = new EditTitulacionAccesoExpedienteAlumnoCommandHandler(Context, mockIstringLocalizer.Object, mockIMediator.Object);

            //ACT
            var exception = Record.Exception(() =>
            {
                sut.ValidateDatos(request);
            });

            //ASSERT
            Assert.IsType<BadRequestException>(exception);
            Assert.Equal(mensajeEsperado, exception.Message);
            mockIstringLocalizer.Verify(s => s[It.Is<string>(msj => msj == mensajeEsperado)], Times.Once);
        }

        [Fact(DisplayName = "Cuando la fecha inicio es mayor a la fecha fin Devuelve error")]
        public void ValidateDatos_Fecha()
        {
            //ARRANGE
            var request =
                new EditTitulacionAccesoExpedienteAlumnoCommand(1, Guid.NewGuid().ToString(), Guid.NewGuid().ToString(),
                    null, Guid.NewGuid().ToString(), "Lolo", DateTime.Now.AddDays(10),DateTime.Now,
                    Guid.NewGuid().ToString(), "Lolo");
            var mockIstringLocalizer = new Mock<IStringLocalizer<EditTitulacionAccesoExpedienteAlumnoCommandHandler>>
            {
                CallBase = true
            };
            const string mensajeEsperado = $"El campo {nameof(request.FechaInicioTitulo)} no puede ser mayor al campo {nameof(request.FechafinTitulo)}.";
            mockIstringLocalizer.Setup(s => s[It.Is<string>(msj => msj == mensajeEsperado)])
                .Returns(new LocalizedString(mensajeEsperado, mensajeEsperado));
            var mockIMediator = new Mock<IMediator>
            {
                CallBase = true
            };
            var sut = new EditTitulacionAccesoExpedienteAlumnoCommandHandler(Context, mockIstringLocalizer.Object, mockIMediator.Object);

            //ACT
            var exception = Record.Exception(() =>
            {
                sut.ValidateDatos(request);
            });

            //ASSERT
            Assert.IsType<BadRequestException>(exception);
            Assert.Equal(mensajeEsperado, exception.Message);
            mockIstringLocalizer.Verify(s => s[It.Is<string>(msj => msj == mensajeEsperado)], Times.Once);
        }

        [Fact(DisplayName = "Cuando no falta especificar ningún campo requerido No devuelve nada")]
        public void ValidateDatos_Ok()
        {
            //ARRANGE
            var request =
                new EditTitulacionAccesoExpedienteAlumnoCommand(1, Guid.NewGuid().ToString(), Guid.NewGuid().ToString(),
                    null, Guid.NewGuid().ToString(), "ES", DateTime.Now, DateTime.Now.AddDays(10),
                    Guid.NewGuid().ToString(), "ES");
            var mockIstringLocalizer = new Mock<IStringLocalizer<EditTitulacionAccesoExpedienteAlumnoCommandHandler>>
            {
                CallBase = true
            };
            var mockIMediator = new Mock<IMediator>
            {
                CallBase = true
            };
            var sut = new EditTitulacionAccesoExpedienteAlumnoCommandHandler(Context, mockIstringLocalizer.Object, mockIMediator.Object);

            //ACT
            sut.ValidateDatos(request);

            //ASSERT
            Assert.NotNull(request.Titulo);
            Assert.NotNull(request.InstitucionDocente);
        }

        #endregion

        #region AssignTitulacionAcceso

        [Fact(DisplayName = "Cuando se asignan los valores a la titulacion de acceso Devuelve entidad seteada")]
        public void AssignTitulacionAcceso_Ok()
        {
            //ARRANGE
            var request =
                new EditTitulacionAccesoExpedienteAlumnoCommand(1, null, null,
                    null, Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), DateTime.Now, DateTime.Now.AddDays(10),
                    Guid.NewGuid().ToString(), Guid.NewGuid().ToString());
            var mockIstringLocalizer = new Mock<IStringLocalizer<EditTitulacionAccesoExpedienteAlumnoCommandHandler>>
            {
                CallBase = true
            };
            var mockIMediator = new Mock<IMediator>
            {
                CallBase = true
            };
            var sut = new EditTitulacionAccesoExpedienteAlumnoCommandHandler(Context, mockIstringLocalizer.Object, mockIMediator.Object);

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

        #region AddSeguimientoTitulacionAcceso

        [Fact(DisplayName = "Cuando se agregaa el seguimiento de titulación de acceso Retorna True")]
        public async Task AddSeguimientoTitulacionAcceso_AssignTitulacionAcceso_Ok()
        {
            //ARRANGE
            var mockIstringLocalizer = new Mock<IStringLocalizer<EditTitulacionAccesoExpedienteAlumnoCommandHandler>>
            {
                CallBase = true
            };
            var mockIMediator = new Mock<IMediator>
            {
                CallBase = true
            };
            var sutMock = new Mock<EditTitulacionAccesoExpedienteAlumnoCommandHandler>(Context, mockIstringLocalizer.Object, mockIMediator.Object)
            {
                CallBase = true
            };

            mockIMediator.Setup(s => s.Send(It.IsAny<AddSeguimientoTitulacionAccesoUncommitCommand>(),
                    It.IsAny<CancellationToken>())).ReturnsAsync(true);

            var expedienteAlumno = new ExpedienteAlumno
            {
                Id = 1
            };

            var request = new EditTitulacionAccesoExpedienteAlumnoCommand(1, null, null, null, 
                Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), DateTime.Now, 
                DateTime.Now.AddDays(10), Guid.NewGuid().ToString(), Guid.NewGuid().ToString());

            //ACT
            var actual = await sutMock.Object.AddSeguimientoTitulacionAcceso(expedienteAlumno, request, CancellationToken.None);

            //ASSERT
            Assert.True(actual);
            mockIMediator.Verify(s => s.Send(It.IsAny<AddSeguimientoTitulacionAccesoUncommitCommand>(),
                It.IsAny<CancellationToken>()), Times.Once);
        }
        
        [Fact(DisplayName = "Cuando no se agrega el seguimiento de titulación de acceso Retorna False")]
        public async Task AddSeguimientoTitulacionAcceso_No_AssignTitulacionAcceso_Ok()
        {
            //ARRANGE
            var mockIstringLocalizer = new Mock<IStringLocalizer<EditTitulacionAccesoExpedienteAlumnoCommandHandler>>
            {
                CallBase = true
            };
            var mockIMediator = new Mock<IMediator>
            {
                CallBase = true
            };
            var sutMock = new Mock<EditTitulacionAccesoExpedienteAlumnoCommandHandler>(Context, mockIstringLocalizer.Object, mockIMediator.Object)
            {
                CallBase = true
            };

            mockIMediator.Setup(s => s.Send(It.IsAny<AddSeguimientoTitulacionAccesoUncommitCommand>(),
                    It.IsAny<CancellationToken>())).ReturnsAsync(false);

            var expedienteAlumno = new ExpedienteAlumno
            {
                Id = 1
            };
            var request = new EditTitulacionAccesoExpedienteAlumnoCommand(1, null, null, null,
                Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), DateTime.Now,
                DateTime.Now.AddDays(10), Guid.NewGuid().ToString(), Guid.NewGuid().ToString());

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
