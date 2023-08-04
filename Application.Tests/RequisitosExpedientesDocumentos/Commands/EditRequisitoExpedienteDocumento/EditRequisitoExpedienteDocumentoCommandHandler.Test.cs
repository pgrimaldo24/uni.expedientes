using Microsoft.Extensions.Localization;
using Moq;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Unir.Expedientes.Application.RequisitosExpedientesDocumentos.Commands.EditRequisitoExpedienteDocumento;
using Unir.Expedientes.Application.Tests.Common;
using Unir.Expedientes.Domain.Entities;
using Unir.Framework.ApplicationSuperTypes.Exceptions;
using Unir.Framework.Crosscutting.Security.Model;
using Xunit;
using static Unir.Expedientes.Application.RequisitosExpedientesDocumentos.Commands.EditRequisitoExpedienteDocumento.EditRequisitoExpedienteDocumentoCommandHandler;

namespace Unir.Expedientes.Application.Tests.RequisitosExpedientesDocumentos.Commands.EditRequisitoExpedienteDocumento
{
    [Collection("CommonTestCollection")]
    public class EditRequisitoExpedienteDocumentoCommandHandlerTests : TestBase
    {
        #region Handle

        [Fact(DisplayName = "Cuando los datos enviados en el request vienen vacíos Devuelve una excepción")]
        public async Task Handle_RequestInvalid()
        {
            //ARRANGE
            const string mensajeEsperado = "El campo NombreDocumento es requerido para editar el Documento.";
            var request = new EditRequisitoExpedienteDocumentoCommand();

            var mockIStringLocalizer = new Mock<IStringLocalizer<EditRequisitoExpedienteDocumentoCommandHandler>>
            {
                CallBase = true
            };
            mockIStringLocalizer.Setup(s => s[It.Is<string>(msj => msj == mensajeEsperado)])
                .Returns(new LocalizedString(mensajeEsperado, mensajeEsperado));
            var sut = new Mock<EditRequisitoExpedienteDocumentoCommandHandler>(Context,
                mockIStringLocalizer.Object)
            {
                CallBase = true
            };
            sut.Setup(s => s.ValidatePropiedadesRequeridas(It.IsAny<EditRequisitoExpedienteDocumentoCommand>()))
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
            sut.Verify(s => s.ValidatePropiedadesRequeridas(It.IsAny<EditRequisitoExpedienteDocumentoCommand>()), Times.Once);
        }

        [Fact(DisplayName = "Cuando no existe el documento Devuelve una excepción")]
        public async Task Handle_DocumentoInexistente_NotFoundException()
        {
            //ARRANGE
            var request = new EditRequisitoExpedienteDocumentoCommand();

            var mockIStringLocalizer = new Mock<IStringLocalizer<EditRequisitoExpedienteDocumentoCommandHandler>>
            {
                CallBase = true
            };
            var sut = new Mock<EditRequisitoExpedienteDocumentoCommandHandler>(Context,
                mockIStringLocalizer.Object)
            {
                CallBase = true
            };
            sut.Setup(s => s.ValidatePropiedadesRequeridas(It.IsAny<EditRequisitoExpedienteDocumentoCommand>()))
                .Returns(PropiedadesRequeridas.Ninguno);

            //ACT
            var ex = (NotFoundException)await Record.ExceptionAsync(async () =>
            {
                await sut.Object.Handle(request, CancellationToken.None);
            });

            //ASSERT
            Assert.NotNull(ex);
            Assert.IsType<NotFoundException>(ex);
            sut.Verify(s => s.ValidatePropiedadesRequeridas(It.IsAny<EditRequisitoExpedienteDocumentoCommand>()), Times.Once);
        }

        [Fact(DisplayName = "Cuando ya existe otro documento con el mismo nombre Devuelve una excepción")]
        public async Task Handle_User_BadRequestException()
        {
            //ARRANGE
            var nombreDocumento = Guid.NewGuid().ToString();
            string mensajeEsperado = $"Ya existe un Documento con el nombre: {nombreDocumento}";
            var request = new EditRequisitoExpedienteDocumentoCommand
            {
                Id = 1,
                IdRequisitoExpediente = 1,
                NombreDocumento = nombreDocumento
            };

            var mockIStringLocalizer = new Mock<IStringLocalizer<EditRequisitoExpedienteDocumentoCommandHandler>>
            {
                CallBase = true
            };
            mockIStringLocalizer.Setup(s => s[It.Is<string>(msj => msj == mensajeEsperado)])
                .Returns(new LocalizedString(mensajeEsperado, mensajeEsperado));
            var sut = new Mock<EditRequisitoExpedienteDocumentoCommandHandler>(Context,
                mockIStringLocalizer.Object)
            {
                CallBase = true
            };
            sut.Setup(s => s.ValidatePropiedadesRequeridas(It.IsAny<EditRequisitoExpedienteDocumentoCommand>()))
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
                NombreDocumento = Guid.NewGuid().ToString(),
                RequisitoExpediente = requisitoExpediente
            };
            await Context.RequisitosExpedientesDocumentos.AddAsync(requisitoExpedienteDocumento);
            await Context.SaveChangesAsync();

            var requisitoExpedienteDocumento2 = new RequisitoExpedienteDocumento
            {
                Id = 2,
                NombreDocumento = nombreDocumento,
                RequisitoExpediente = requisitoExpediente
            };
            await Context.RequisitosExpedientesDocumentos.AddAsync(requisitoExpedienteDocumento2);
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
            sut.Verify(s => s.ValidatePropiedadesRequeridas(It.IsAny<EditRequisitoExpedienteDocumentoCommand>()), Times.Once);
            mockIStringLocalizer.Verify(s => s[It.Is<string>(msj => msj == mensajeEsperado)], Times.Once);
        }


        [Fact(DisplayName = "Cuando los datos enviados en el request están correctos Realiza el insert en bd")]
        public async Task Handle_Ok()
        {
            //ARRANGE
            var request = new EditRequisitoExpedienteDocumentoCommand
            {
                IdRequisitoExpediente = 1,
                Id = 1
            };

            var mockIStringLocalizer = new Mock<IStringLocalizer<EditRequisitoExpedienteDocumentoCommandHandler>>
            {
                CallBase = true
            };
            var identityInfo = new IdentityModel
            {
                Id = Guid.NewGuid().ToString()
            };
            var sut = new Mock<EditRequisitoExpedienteDocumentoCommandHandler>(Context,
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

            var requisitoExpedienteDocumento2 = new RequisitoExpedienteDocumento
            {
                Id = 1,
                NombreDocumento = Guid.NewGuid().ToString(),
                RequisitoExpediente = requisitoExpediente
            };
            await Context.RequisitosExpedientesDocumentos.AddAsync(requisitoExpedienteDocumento2);
            await Context.SaveChangesAsync();

            sut.Setup(s => s.ValidatePropiedadesRequeridas(It.IsAny<EditRequisitoExpedienteDocumentoCommand>()))
                .Returns(PropiedadesRequeridas.Ninguno);

            sut.Setup(s => s.AssignEditRequisitoExpedienteDocumento(It.IsAny<EditRequisitoExpedienteDocumentoCommand>(),
                It.IsAny<RequisitoExpedienteDocumento>()));

            //ACT
            await sut.Object.Handle(request, CancellationToken.None);

            //ASSERT
            sut.Verify(s => s.ValidatePropiedadesRequeridas(It.IsAny<EditRequisitoExpedienteDocumentoCommand>()), Times.Once);
            Assert.Equal(1, Context.RequisitosExpedientesDocumentos.Count());
            Assert.Equal(requisitoExpediente, Context.RequisitosExpedientesDocumentos.FirstOrDefault()?.RequisitoExpediente);
            sut.Verify(s => s.AssignEditRequisitoExpedienteDocumento(It.IsAny<EditRequisitoExpedienteDocumentoCommand>(),
                It.IsAny<RequisitoExpedienteDocumento>()), Times.Once);
        }

        #endregion

        #region AssignEditRequisitoExpedienteDocumento

        [Fact(DisplayName = "Cuando se asignan los datos enviados en el request Devuelve entidad")]
        public void AssignEditRequisitoExpedienteDocumento_Ok()
        {
            //ARRANGE
            var request = new EditRequisitoExpedienteDocumentoCommand
            {
                NombreDocumento = Guid.NewGuid().ToString(),
                DocumentoEditable = true,
                DocumentoObligatorio = false,
                RequiereAceptacionAlumno = true,
                DocumentoSecurizado = false,
            };

            var mockIStringLocalizer = new Mock<IStringLocalizer<EditRequisitoExpedienteDocumentoCommandHandler>>
            {
                CallBase = true
            };
            var sut = new Mock<EditRequisitoExpedienteDocumentoCommandHandler>(Context,
                mockIStringLocalizer.Object)
            {
                CallBase = true
            };
            var requisitoExpedienteDocumento = new RequisitoExpedienteDocumento();

            //ACT
            sut.Object.AssignEditRequisitoExpedienteDocumento(request, requisitoExpedienteDocumento);

            //ASSERT
            Assert.Equal(request.NombreDocumento, requisitoExpedienteDocumento.NombreDocumento);
            Assert.Equal(request.DocumentoEditable, requisitoExpedienteDocumento.DocumentoEditable);
            Assert.Equal(request.DocumentoObligatorio, requisitoExpedienteDocumento.DocumentoObligatorio);
            Assert.Equal(request.RequiereAceptacionAlumno, requisitoExpedienteDocumento.RequiereAceptacionAlumno);
            Assert.Equal(request.DocumentoSecurizado, requisitoExpedienteDocumento.DocumentoSecurizado);
        }

        #endregion

        #region ValidatePropiedadesRequeridas

        [Fact(DisplayName = "Cuando las propiedades pasa todas las validaciones Retorna ok")]
        public void ValidatePropiedadesRequeridas_Ok()
        {
            //ARRANGE
            var mockIStringLocalizer = new Mock<IStringLocalizer<EditRequisitoExpedienteDocumentoCommandHandler>>
            {
                CallBase = true
            };
            var request = new EditRequisitoExpedienteDocumentoCommand
            {
                NombreDocumento = Guid.NewGuid().ToString(),
            };
            var sut = new Mock<EditRequisitoExpedienteDocumentoCommandHandler>(Context,
                mockIStringLocalizer.Object)
            {
                CallBase = true
            };

            //ACT
            var actual = sut.Object.ValidatePropiedadesRequeridas(request);

            //ASSERT
            Assert.Equal(PropiedadesRequeridas.Ninguno, actual);
        }

        [Fact(DisplayName = "Cuando la propiedad nombre documento no tiene valor Retorna propiedad requerida")]
        public void ValidatePropiedadesRequeridas_NombreDocumento()
        {
            //ARRANGE
            var mockIStringLocalizer = new Mock<IStringLocalizer<EditRequisitoExpedienteDocumentoCommandHandler>>
            {
                CallBase = true
            };
            var request = new EditRequisitoExpedienteDocumentoCommand();
            var sut = new Mock<EditRequisitoExpedienteDocumentoCommandHandler>(Context,
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
