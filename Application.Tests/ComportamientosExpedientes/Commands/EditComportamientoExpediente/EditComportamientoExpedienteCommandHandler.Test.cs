using Microsoft.Extensions.Localization;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Unir.Expedientes.Application.ComportamientosExpedientes.Commands.EditComportamientoExpediente;
using Unir.Expedientes.Application.Tests.Common;
using Unir.Expedientes.Domain.Entities;
using Unir.Framework.ApplicationSuperTypes.Exceptions;
using Xunit;

namespace Unir.Expedientes.Application.Tests.ComportamientosExpedientes.Commands.EditComportamientoExpediente
{
    [Collection("CommonTestCollection")]
    public class EditComportamientoExpedienteCommandHandlerTests : TestBase
    {
        #region Handle

        [Fact(DisplayName = "Cuando al validar las propiedades da error Devuelve excepción")]
        public async Task Handle_ValidatePropiedadesRequeridas_BadRequestException()
        {
            //ARRANGE
            var request = new EditComportamientoExpedienteCommand
            {
                IdComportamiento = 1
            };
            var mockIStringLocalizer = new Mock<IStringLocalizer<EditComportamientoExpedienteCommandHandler>>
            {
                CallBase = true
            };
            var sutMock = new Mock<EditComportamientoExpedienteCommandHandler>(Context,
                mockIStringLocalizer.Object)
            { CallBase = true };

            const string mensajeError = "Mensaje de error";
            sutMock.Setup(x => x.ValidatePropiedadesRequeridas(
                It.IsAny<EditComportamientoExpedienteCommand>()))
                .ThrowsAsync(new BadRequestException(mensajeError));


            //ACT
            var ex = await Record.ExceptionAsync(async () =>
            {
                await sutMock.Object.Handle(request, CancellationToken.None);
            });

            //ASSERT
            Assert.NotNull(ex);
            Assert.IsType<BadRequestException>(ex);
            Assert.Equal(mensajeError, ex.Message);
            sutMock.Verify(x => x.ValidatePropiedadesRequeridas(
                It.IsAny<EditComportamientoExpedienteCommand>()), Times.Once());
        }

        [Fact(DisplayName = "Cuando el comportamiento no existe Devuelve una excepción")]
        public async Task Handle_NotFoundException()
        {
            //ARRANGE
            var request = new EditComportamientoExpedienteCommand();
            var mockIStringLocalizer = new Mock<IStringLocalizer<EditComportamientoExpedienteCommandHandler>>
            {
                CallBase = true
            };
            var sutMock = new Mock<EditComportamientoExpedienteCommandHandler>(Context,
                mockIStringLocalizer.Object);
            sutMock.Setup(x => x.ValidatePropiedadesRequeridas(
                It.IsAny<EditComportamientoExpedienteCommand>())).Returns(Task.CompletedTask);

            //ACT
            var ex = await Record.ExceptionAsync(async () =>
            {
                await sutMock.Object.Handle(request, CancellationToken.None);
            });

            //ASSERT
            Assert.NotNull(ex);
            Assert.IsType<NotFoundException>(ex);
            sutMock.Verify(x => x.ValidatePropiedadesRequeridas(
                It.IsAny<EditComportamientoExpedienteCommand>()), Times.Once());
        }

        [Fact(DisplayName = "Cuando el proceso es correcto Devuelve Ok")]
        public async Task Handle_Ok()
        {
            //ARRANGE
            var request = new EditComportamientoExpedienteCommand
            {
                IdComportamiento = 1
            };
            var mockIStringLocalizer = new Mock<IStringLocalizer<EditComportamientoExpedienteCommandHandler>>
            {
                CallBase = true
            };
            var sutMock = new Mock<EditComportamientoExpedienteCommandHandler>(Context,
                mockIStringLocalizer.Object);
            sutMock.Setup(x => x.ValidatePropiedadesRequeridas(
                It.IsAny<EditComportamientoExpedienteCommand>())).Returns(Task.CompletedTask);
            sutMock.Setup(x => x.AssignComportamientoExpediente(
                It.IsAny<EditComportamientoExpedienteCommand>(), It.IsAny<ComportamientoExpediente>()));

            await Context.ComportamientosExpedientes.AddAsync(new ComportamientoExpediente
            {
                Id = 1
            });
            await Context.SaveChangesAsync();

            //ACT
            await sutMock.Object.Handle(request, CancellationToken.None);

            //ASSERT
            sutMock.Verify(x => x.ValidatePropiedadesRequeridas(
                It.IsAny<EditComportamientoExpedienteCommand>()), Times.Once());
            sutMock.Verify(x => x.AssignComportamientoExpediente(
                It.IsAny<EditComportamientoExpedienteCommand>(), 
                It.IsAny<ComportamientoExpediente>()), Times.Once());
        }

        #endregion

        #region ValidatePropiedadesRequeridas

        [Fact(DisplayName = "Cuando no se envía nombre Devuelve excepción")]
        public async Task ValidatePropiedadesRequeridas_Nombre_BadRequestException()
        {
            //ARRANGE
            var request = new EditComportamientoExpedienteCommand();
            var mockIStringLocalizer = new Mock<IStringLocalizer<EditComportamientoExpedienteCommandHandler>>
            {
                CallBase = true
            };
            var sutMock = new EditComportamientoExpedienteCommandHandler(Context, mockIStringLocalizer.Object);

            const string mensajeEsperado = "El campo Nombre es requerido para editar el Comportamiento.";
            mockIStringLocalizer.Setup(s => s[It.Is<string>(msj => msj == mensajeEsperado)])
                .Returns(new LocalizedString(mensajeEsperado, mensajeEsperado));

            //ACT
            var ex = await Record.ExceptionAsync(async () =>
            {
                await sutMock.ValidatePropiedadesRequeridas(request);
            });

            //ASSERT
            Assert.NotNull(ex);
            Assert.IsType<BadRequestException>(ex);
            Assert.Equal(mensajeEsperado, ex.Message);
        }

        [Fact(DisplayName = "Cuando el nombre ya existe Devuelve excepción")]
        public async Task ValidatePropiedadesRequeridas_ExisteNombre_BadRequestException()
        {
            //ARRANGE
            var mockIStringLocalizer = new Mock<IStringLocalizer<EditComportamientoExpedienteCommandHandler>>
            {
                CallBase = true
            };
            var sutMock = new EditComportamientoExpedienteCommandHandler(Context, mockIStringLocalizer.Object);

            const string mensajeEsperado = "El Nombre ingresado ya existe.";
            mockIStringLocalizer.Setup(s => s[It.Is<string>(msj => msj == mensajeEsperado)])
                .Returns(new LocalizedString(mensajeEsperado, mensajeEsperado));

            var nombre = Guid.NewGuid().ToString();
            await Context.ComportamientosExpedientes.AddAsync(new ComportamientoExpediente
            {
                Id = 1,
                Nombre = nombre
            });
            await Context.SaveChangesAsync();
            var request = new EditComportamientoExpedienteCommand
            {
                Nombre = nombre
            };

            //ACT
            var ex = await Record.ExceptionAsync(async () =>
            {
                await sutMock.ValidatePropiedadesRequeridas(request);
            });

            //ASSERT
            Assert.NotNull(ex);
            Assert.IsType<BadRequestException>(ex);
            Assert.Equal(mensajeEsperado, ex.Message);
        }

        #endregion

        #region AssignComportamientoExpediente

        [Fact(DisplayName = "Cuando se asignan los datos del comportamiento Devuelve un objeto")]
        public void AssignComportamientoExpediente_Ok()
        {
            //ARRANGE
            var mockIStringLocalizer = new Mock<IStringLocalizer<EditComportamientoExpedienteCommandHandler>>
            {
                CallBase = true
            };
            var sut = new EditComportamientoExpedienteCommandHandler(Context, mockIStringLocalizer.Object);
            var request = new EditComportamientoExpedienteCommand
            {
                Nombre = Guid.NewGuid().ToString(),
                Descripcion = Guid.NewGuid().ToString(),
                EstaVigente = true,
            };
            var comportamientoExpediente = new ComportamientoExpediente();

            //ACT
            sut.AssignComportamientoExpediente(request, comportamientoExpediente);

            //ASSERT
            Assert.Equal(request.Nombre, comportamientoExpediente.Nombre);
            Assert.Equal(request.Descripcion, comportamientoExpediente.Descripcion);
            Assert.Equal(request.EstaVigente, comportamientoExpediente.EstaVigente);
        }

        #endregion
    }
}
