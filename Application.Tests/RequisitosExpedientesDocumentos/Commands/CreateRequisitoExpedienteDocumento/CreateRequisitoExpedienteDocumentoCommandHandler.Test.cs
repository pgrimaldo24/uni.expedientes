using Microsoft.Extensions.Localization;
using Moq;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Unir.Expedientes.Application.RequisitosExpedientesDocumentos.Commands.CreateRequisitoExpedienteDocumento;
using Unir.Expedientes.Application.Tests.Common;
using Unir.Expedientes.Domain.Entities;
using Unir.Framework.ApplicationSuperTypes.Exceptions;
using Xunit;
using static Unir.Expedientes.Application.RequisitosExpedientesDocumentos.Commands.CreateRequisitoExpedienteDocumento.CreateRequisitoExpedienteDocumentoCommandHandler;

namespace Unir.Expedientes.Application.Tests.RequisitosExpedientesDocumentos.Commands.CreateRequisitoExpedienteDocumento
{
    [Collection("CommonTestCollection")]
    public class CreateRequisitoExpedienteDocumentoCommandHandlerTests : TestBase
    {
        #region Handle

        [Fact(DisplayName = "Cuando los datos enviados en el request vienen vacíos Devuelve una excepción")]
        public async Task Handle_RequestInvalid()
        {
            //ARRANGE
            const string mensajeEsperado = "El campo NombreDocumento es requerido para crear el Documento.";
            var request = new CreateRequisitoExpedienteDocumentoCommand();

            var mockIStringLocalizer = new Mock<IStringLocalizer<CreateRequisitoExpedienteDocumentoCommandHandler>>
            {
                CallBase = true
            };
            mockIStringLocalizer.Setup(s => s[It.Is<string>(msj => msj == mensajeEsperado)])
                .Returns(new LocalizedString(mensajeEsperado, mensajeEsperado));
            var sut = new Mock<CreateRequisitoExpedienteDocumentoCommandHandler>(Context, 
                mockIStringLocalizer.Object)
            {
                CallBase = true
            };
            sut.Setup(s => s.ValidatePropiedadesRequeridas(It.IsAny<CreateRequisitoExpedienteDocumentoCommand>()))
                .Returns(PropiedadesRequeridas.NombreDocumento);

            //ACT
            var ex = (BadRequestException)await Record.ExceptionAsync(async () =>
            {
                await sut.Object.Handle(request, CancellationToken.None);
            });

            //ASSERT
            Assert.NotNull(ex);
            Assert.IsType<BadRequestException>(ex);
            Assert.Equal(mensajeEsperado, ex.Message);
            sut.Verify(s => s.ValidatePropiedadesRequeridas(It.IsAny<CreateRequisitoExpedienteDocumentoCommand>()), Times.Once);
            mockIStringLocalizer.Verify(s => s[It.Is<string>(msj => msj == mensajeEsperado)], Times.Once);
        }

        [Fact(DisplayName = "Cuando no existe el requisito expediente Devuelve una excepción")]
        public async Task Handle_NotFoundException()
        {
            //ARRANGE
            var request = new CreateRequisitoExpedienteDocumentoCommand();

            var mockIStringLocalizer = new Mock<IStringLocalizer<CreateRequisitoExpedienteDocumentoCommandHandler>>
            {
                CallBase = true
            };
            var sut = new Mock<CreateRequisitoExpedienteDocumentoCommandHandler>(Context,
                mockIStringLocalizer.Object)
            {
                CallBase = true
            };
            sut.Setup(s => s.ValidatePropiedadesRequeridas(It.IsAny<CreateRequisitoExpedienteDocumentoCommand>()))
                .Returns(PropiedadesRequeridas.Ninguno);

            //ACT
            var ex = (NotFoundException)await Record.ExceptionAsync(async () =>
            {
                await sut.Object.Handle(request, CancellationToken.None);
            });

            //ASSERT
            Assert.NotNull(ex);
            Assert.IsType<NotFoundException>(ex);
            sut.Verify(s => s.ValidatePropiedadesRequeridas(It.IsAny<CreateRequisitoExpedienteDocumentoCommand>()), Times.Once);
        }

        [Fact(DisplayName = "Cuando no existe el usuario Devuelve una excepción")]
        public async Task Handle_User_BadRequestException()
        {
            //ARRANGE
            var nombreDocumento = Guid.NewGuid().ToString();
            string mensajeEsperado = $"Ya existe un Documento con el nombre: {nombreDocumento}";
            var request = new CreateRequisitoExpedienteDocumentoCommand
            {
                IdRequisitoExpediente = 1,
                NombreDocumento = nombreDocumento
            };

            var mockIStringLocalizer = new Mock<IStringLocalizer<CreateRequisitoExpedienteDocumentoCommandHandler>>
            {
                CallBase = true
            };
            mockIStringLocalizer.Setup(s => s[It.Is<string>(msj => msj == mensajeEsperado)])
                .Returns(new LocalizedString(mensajeEsperado, mensajeEsperado));
            var sut = new Mock<CreateRequisitoExpedienteDocumentoCommandHandler>(Context,
                mockIStringLocalizer.Object)
            {
                CallBase = true
            };
            sut.Setup(s => s.ValidatePropiedadesRequeridas(It.IsAny<CreateRequisitoExpedienteDocumentoCommand>()))
                .Returns(PropiedadesRequeridas.Ninguno);

            var requisitoExpediente = new RequisitoExpediente
            {
                Id = 1
            };
            await Context.RequisitosExpedientes.AddAsync(requisitoExpediente);
            await Context.SaveChangesAsync();

            var requisitoExpedienteDocumento = new RequisitoExpedienteDocumento
            {
                Id = 1,
                NombreDocumento = nombreDocumento,
                RequisitoExpediente = requisitoExpediente
            };
            await Context.RequisitosExpedientesDocumentos.AddAsync(requisitoExpedienteDocumento);
            await Context.SaveChangesAsync();

            //ACT
            var ex = (BadRequestException)await Record.ExceptionAsync(async () =>
            {
                await sut.Object.Handle(request, CancellationToken.None);
            });

            //ASSERT
            Assert.NotNull(ex);
            Assert.IsType<BadRequestException>(ex);
            Assert.Equal(mensajeEsperado, ex.Message);
            sut.Verify(s => s.ValidatePropiedadesRequeridas(It.IsAny<CreateRequisitoExpedienteDocumentoCommand>()), Times.Once);
            sut.Verify(s => s.ValidatePropiedadesRequeridas(It.IsAny<CreateRequisitoExpedienteDocumentoCommand>()), Times.Once);
        }

        [Fact(DisplayName = "Cuando los datos enviados en el request están correctos Realiza el insert en bd")]
        public async Task Handle_Ok()
        {
            //ARRANGE
            var request = new CreateRequisitoExpedienteDocumentoCommand
            {
                IdRequisitoExpediente = 1
            };

            var mockIStringLocalizer = new Mock<IStringLocalizer<CreateRequisitoExpedienteDocumentoCommandHandler>>
            {
                CallBase = true
            };
            var sut = new Mock<CreateRequisitoExpedienteDocumentoCommandHandler>(Context,
                mockIStringLocalizer.Object)
            {
                CallBase = true
            };
            var requisitoExpediente = new RequisitoExpediente
            {
                Id = 1
            };
            await Context.RequisitosExpedientes.AddAsync(requisitoExpediente);
            await Context.SaveChangesAsync();

            var requisitoExpedienteDocumento = new RequisitoExpedienteDocumento
            {
                Id = 1,
                NombreDocumento = Guid.NewGuid().ToString(),
                RequisitoExpediente = requisitoExpediente
            };

            sut.Setup(s => s.ValidatePropiedadesRequeridas(It.IsAny<CreateRequisitoExpedienteDocumentoCommand>()))
                .Returns(PropiedadesRequeridas.Ninguno);
            sut.Setup(s => s.AssignNewRequisitoExpedienteDocumento(It.IsAny<CreateRequisitoExpedienteDocumentoCommand>())).Returns(requisitoExpedienteDocumento);

            //ACT
            await sut.Object.Handle(request, CancellationToken.None);

            //ASSERT
            sut.Verify(s => s.ValidatePropiedadesRequeridas(It.IsAny<CreateRequisitoExpedienteDocumentoCommand>()), Times.Once);
            sut.Verify(s => s.AssignNewRequisitoExpedienteDocumento(It.IsAny<CreateRequisitoExpedienteDocumentoCommand>()), Times.Once);
            Assert.Equal(1, Context.RequisitosExpedientesDocumentos.Count());
            Assert.Equal(requisitoExpediente, Context.RequisitosExpedientesDocumentos.FirstOrDefault()?.RequisitoExpediente);
        }

        #endregion

        #region AssignNewRequisitoExpedienteDocumento

        [Fact(DisplayName = "Cuando se asignan los datos enviados en el request Devuelve entidad")]
        public void AssignNewRequisitoExpedienteDocumento_Ok()
        {
            //ARRANGE
            var request = new CreateRequisitoExpedienteDocumentoCommand
            {
                NombreDocumento = Guid.NewGuid().ToString(),
                DocumentoEditable = true,
                DocumentoObligatorio = true,
                RequiereAceptacionAlumno = false,
                DocumentoSecurizado = false,
            };

            var mockIStringLocalizer = new Mock<IStringLocalizer<CreateRequisitoExpedienteDocumentoCommandHandler>>
            {
                CallBase = true
            };
            var sut = new CreateRequisitoExpedienteDocumentoCommandHandler(Context, mockIStringLocalizer.Object);
            const string documentoClasificacion = "J.03.01.01";

            //ACT
            var actual = sut.AssignNewRequisitoExpedienteDocumento(request);

            //ASSERT
            Assert.IsType<RequisitoExpedienteDocumento>(actual);
            Assert.Equal(request.NombreDocumento, actual.NombreDocumento);
            Assert.Equal(request.DocumentoEditable, actual.DocumentoEditable);
            Assert.Equal(request.DocumentoObligatorio, actual.DocumentoObligatorio);
            Assert.Equal(request.RequiereAceptacionAlumno, actual.RequiereAceptacionAlumno);
            Assert.Equal(request.DocumentoSecurizado, actual.DocumentoSecurizado);
            Assert.Equal(documentoClasificacion, actual.DocumentoClasificacion);
        }

        #endregion

        #region ValidatePropiedadesRequeridas

        [Fact(DisplayName = "Cuando las propiedades pasa todas las validaciones Retorna ok")]
        public void ValidatePropiedadesRequeridas_Ok()
        {
            //ARRANGE
            var mockIStringLocalizer = new Mock<IStringLocalizer<CreateRequisitoExpedienteDocumentoCommandHandler>>
            {
                CallBase = true
            };
            var request = new CreateRequisitoExpedienteDocumentoCommand
            {
                NombreDocumento = Guid.NewGuid().ToString()
            };
            var sut = new Mock<CreateRequisitoExpedienteDocumentoCommandHandler>(Context,
                mockIStringLocalizer.Object)
            {
                CallBase = true
            };

            //ACT
            var actual = sut.Object.ValidatePropiedadesRequeridas(request);

            //ASSERT
            Assert.Equal(PropiedadesRequeridas.Ninguno, actual);
        }

        [Fact(DisplayName = "Cuando la propiedad nombre documento no tiene valor Retorna propiedad requeridas")]
        public void ValidatePropiedadesRequeridas_NombreDocumento()
        {
            //ARRANGE
            var mockIStringLocalizer = new Mock<IStringLocalizer<CreateRequisitoExpedienteDocumentoCommandHandler>>
            {
                CallBase = true
            };
            var request = new CreateRequisitoExpedienteDocumentoCommand();
            var sut = new Mock<CreateRequisitoExpedienteDocumentoCommandHandler>(Context,
                mockIStringLocalizer.Object)
            {
                CallBase = true
            };

            //ACT
            var actual = sut.Object.ValidatePropiedadesRequeridas(request);

            //ASSERT
            Assert.Equal(PropiedadesRequeridas.NombreDocumento, actual);
        }

        #endregion
    }
}
