using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Unir.Expedientes.Application.RequisitosExpedientes.Commands.CreateRequisito;
using Unir.Expedientes.Application.Tests.Common;
using Unir.Expedientes.Domain.Entities;
using Unir.Framework.ApplicationSuperTypes.Exceptions;
using Xunit;
using static Unir.Expedientes.Application.RequisitosExpedientes.Commands.CreateRequisito.CreateRequisitoCommandHandler;

namespace Unir.Expedientes.Application.Tests.RequisitosExpedientes.Commands.CreateRequisito
{
    [Collection("CommonTestCollection")]
    public class CreateRequisitoCommandHandlerTests : TestBase
    {
        #region Handle

        [Fact(DisplayName = "Cuando el requisito a crear no tiene todos los datos requeridos Devuelve una excepción")]
        public async Task Handle_DatosIncompletos()
        {
            //ARRANGE
            var nombre = PropiedadesRequeridas.Nombre;
            string mensajeEsperado = $"El campo {nombre} es requerido para crear el Requisito.";
            var request = new CreateRequisitoCommand();
            var requisito = new RequisitoExpediente();
            var mockIStringLocalizer = new Mock<IStringLocalizer<CreateRequisitoCommandHandler>>
            {
                CallBase = true
            };
            mockIStringLocalizer.Setup(s => s[It.Is<string>(msj => msj == mensajeEsperado)])
                .Returns(new LocalizedString(mensajeEsperado, mensajeEsperado));
            var sutMock = new Mock<CreateRequisitoCommandHandler>(Context, mockIStringLocalizer.Object)
            {
                CallBase = true
            };
            sutMock.Setup(s => s.ValidatePropiedadesRequeridas(It.IsAny<CreateRequisitoCommand>()))
                .Returns(PropiedadesRequeridas.Nombre);
            sutMock.Setup(s => s.AssignNewRequisito(It.IsAny<CreateRequisitoCommand>()))
                .Returns(requisito);

            //ACT
            var ex = (BadRequestException)await Record.ExceptionAsync(async () =>
            {
                var actual = await sutMock.Object.Handle(request, CancellationToken.None);
            });

            //ASSERT
            Assert.NotNull(ex);
            Assert.IsType<BadRequestException>(ex);
            Assert.Equal(mensajeEsperado, ex.Message);
            sutMock.Verify(s => s.ValidatePropiedadesRequeridas(It.IsAny<CreateRequisitoCommand>()), Times.Once);
            sutMock.Verify(s => s.AssignNewRequisito(It.IsAny<CreateRequisitoCommand>()), Times.Never);
            mockIStringLocalizer.Verify(s => s[It.Is<string>(msj => msj == mensajeEsperado)], Times.Once);
        }

        [Fact(DisplayName = "Cuando el nombre del requisito ya existe Devuelve una excepción")]
        public async Task Handle_RequisitoExistente()
        {
            //ARRANGE
            var nombre = Guid.NewGuid().ToString();
            string mensajeEsperado = $"Ya existe un Requisito con el nombre: {nombre}";
            var request = new CreateRequisitoCommand
            {
                Nombre = nombre
            };
            var requisito = new RequisitoExpediente
            {
                Nombre = nombre,
                Orden = 5
            };
            await Context.RequisitosExpedientes.AddAsync(requisito);
            await Context.SaveChangesAsync();
            var mockIStringLocalizer = new Mock<IStringLocalizer<CreateRequisitoCommandHandler>>
            {
                CallBase = true
            };
            mockIStringLocalizer.Setup(s => s[It.Is<string>(msj => msj == mensajeEsperado)])
                .Returns(new LocalizedString(mensajeEsperado, mensajeEsperado));
            var sutMock = new Mock<CreateRequisitoCommandHandler>(Context, mockIStringLocalizer.Object)
            {
                CallBase = true
            };
            sutMock.Setup(s => s.ValidatePropiedadesRequeridas(It.IsAny<CreateRequisitoCommand>()))
                .Returns(PropiedadesRequeridas.Ninguno);
            sutMock.Setup(s => s.AssignNewRequisito(It.IsAny<CreateRequisitoCommand>()))
                .Returns(requisito);

            //ACT
            var ex = (BadRequestException)await Record.ExceptionAsync(async () =>
            {
                var actual = await sutMock.Object.Handle(request, CancellationToken.None);
            });

            //ASSERT
            Assert.NotNull(ex);
            Assert.IsType<BadRequestException>(ex);
            Assert.Equal(mensajeEsperado, ex.Message);
            sutMock.Verify(s => s.ValidatePropiedadesRequeridas(It.IsAny<CreateRequisitoCommand>()), Times.Once);
            sutMock.Verify(s => s.AssignNewRequisito(It.IsAny<CreateRequisitoCommand>()), Times.Never);
            mockIStringLocalizer.Verify(s => s[It.Is<string>(msj => msj == mensajeEsperado)], Times.Once);
        }

        [Fact(DisplayName = "Cuando se crear el requisito Devuelve id")]
        public async Task Handle_Ok()
        {
            //ARRANGE
            var request = new CreateRequisitoCommand();
            var requisito = new RequisitoExpediente
            {
                Nombre = Guid.NewGuid().ToString(),
                Orden = 5
            };
            var mockIStringLocalizer = new Mock<IStringLocalizer<CreateRequisitoCommandHandler>>
            {
                CallBase = true
            };
            var sutMock = new Mock<CreateRequisitoCommandHandler>(Context, mockIStringLocalizer.Object)
            {
                CallBase = true
            };
            sutMock.Setup(s => s.ValidatePropiedadesRequeridas(It.IsAny<CreateRequisitoCommand>()))
                .Returns(PropiedadesRequeridas.Ninguno);
            sutMock.Setup(s => s.AssignNewRequisito(It.IsAny<CreateRequisitoCommand>()))
                .Returns(requisito);

            //ACT
            var actual = await sutMock.Object.Handle(request, CancellationToken.None);

            //ASSERT
            Assert.IsType<int>(actual);
            Assert.Equal(requisito.Nombre, (await Context.RequisitosExpedientes.FirstOrDefaultAsync(CancellationToken.None)).Nombre);
            Assert.Equal(requisito.Orden, (await Context.RequisitosExpedientes.FirstOrDefaultAsync(CancellationToken.None)).Orden);
            sutMock.Verify(s => s.ValidatePropiedadesRequeridas(It.IsAny<CreateRequisitoCommand>()), Times.Once);
            sutMock.Verify(s => s.AssignNewRequisito(It.IsAny<CreateRequisitoCommand>()), Times.Once);
        }

        #endregion

        #region ValidatePropiedadesRequeridas

        [Fact(DisplayName = "Cuando se envían todas las propiedades requeridas Devuelve ninguno")]
        public void ValidatePropiedadesRequeridas_Ok()
        {
            //ARRANGE
            var request = new CreateRequisitoCommand
            {
                Nombre = Guid.NewGuid().ToString(),
                Orden = 5
            };
            var mockIStringLocalizer = new Mock<IStringLocalizer<CreateRequisitoCommandHandler>>
            {
                CallBase = true
            };
            var sut = new CreateRequisitoCommandHandler(Context, mockIStringLocalizer.Object);

            //ACT
            var actual = sut.ValidatePropiedadesRequeridas(request);

            //ASSERT
            Assert.Equal(PropiedadesRequeridas.Ninguno, actual);
        }

        [Fact(DisplayName = "Cuando no se envía orden Devuelve propiedad de orden")]
        public void ValidatePropiedadesRequeridas_Orden()
        {
            //ARRANGE
            var request = new CreateRequisitoCommand
            {
                Nombre = Guid.NewGuid().ToString()
            };
            var mockIStringLocalizer = new Mock<IStringLocalizer<CreateRequisitoCommandHandler>>
            {
                CallBase = true
            };
            var sut = new CreateRequisitoCommandHandler(Context, mockIStringLocalizer.Object);

            //ACT
            var actual = sut.ValidatePropiedadesRequeridas(request);

            //ASSERT
            Assert.Equal(PropiedadesRequeridas.Orden, actual);
        }

        [Fact(DisplayName = "Cuando no se envía orden Devuelve propiedad de nombre")]
        public void ValidatePropiedadesRequeridas_Nombre()
        {
            //ARRANGE
            var request = new CreateRequisitoCommand
            {
                Orden = 5
            };
            var mockIStringLocalizer = new Mock<IStringLocalizer<CreateRequisitoCommandHandler>>
            {
                CallBase = true
            };
            var sut = new CreateRequisitoCommandHandler(Context, mockIStringLocalizer.Object);

            //ACT
            var actual = sut.ValidatePropiedadesRequeridas(request);

            //ASSERT
            Assert.Equal(PropiedadesRequeridas.Nombre, actual);
        }

        #endregion

        #region AssignNewRequisito

        [Fact(DisplayName = "Cuando se asignan propiedades Devuelve objeto requisito")]
        public void AssignNewRequisito_Ok()
        {
            //ARRANGE
            var request = new CreateRequisitoCommand
            {
                Nombre = Guid.NewGuid().ToString(),
                Orden = 5
            };
            var mockIStringLocalizer = new Mock<IStringLocalizer<CreateRequisitoCommandHandler>>
            {
                CallBase = true
            };
            var sut = new CreateRequisitoCommandHandler(Context, mockIStringLocalizer.Object);

            //ACT
            var actual = sut.AssignNewRequisito(request);

            //ASSERT
            Assert.IsType<RequisitoExpediente>(actual);
            Assert.Equal(request.Nombre, actual.Nombre);
            Assert.Equal(request.Orden, actual.Orden);
        }

        #endregion
    }
}
