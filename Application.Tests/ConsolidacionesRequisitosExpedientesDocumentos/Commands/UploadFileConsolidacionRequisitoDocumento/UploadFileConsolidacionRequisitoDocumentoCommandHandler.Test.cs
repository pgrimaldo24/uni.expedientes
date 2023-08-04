using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Unir.Expedientes.Application.Common.Interfaces;
using Unir.Expedientes.Application.Common.Models.GestorDocumental;
using Unir.Expedientes.Application.Common.Parameters;
using Unir.Expedientes.Application.ConsolidacionesRequisitosExpedientesDocumentos.Commands.UploadFileConsolidacionRequisitoDocumento;
using Unir.Expedientes.Application.Tests.Common;
using Unir.Expedientes.Domain.Entities;
using Unir.Framework.ApplicationSuperTypes.Exceptions;
using Unir.Framework.Crosscutting.Files;
using Xunit;

namespace Unir.Expedientes.Application.Tests.ConsolidacionesRequisitosExpedientesDocumentos.Commands.UploadFileConsolidacionRequisitoDocumento
{
    [Collection("CommonTestCollection")]
    public class UploadFileConsolidacionRequisitoDocumentoCommandHandlerTests : TestBase
    {
        #region Handle

        [Fact(DisplayName = "Cuando no existe la consolidación requisito expediente Devuelve una excepción")]
        public async Task Handle_NotFoundException_ConsolidacionRequisitoExpediente()
        {
            //ARRANGE
            var request = new UploadFileConsolidacionRequisitoDocumentoCommand
            {
                IdConsolidacionRequisito = 1
            };
            var sut = new UploadFileConsolidacionRequisitoDocumentoCommandHandler(Context, null, null, null);

            //ACT
            var ex = await Record.ExceptionAsync(async () =>
            {
                await sut.Handle(request, CancellationToken.None);
            });

            //ASSERT
            Assert.NotNull(ex);
            Assert.IsType<NotFoundException>(ex);
        }

        [Fact(DisplayName = "Cuando no existe el requisito expediente documento Devuelve una excepción")]
        public async Task Handle_NotFoundException_RequisitoExpedienteDocumento()
        {
            //ARRANGE
            var request = new UploadFileConsolidacionRequisitoDocumentoCommand
            {
                IdConsolidacionRequisito = 1,
                IdRequisitoDocumento = 1
            };
            var sut = new UploadFileConsolidacionRequisitoDocumentoCommandHandler(Context, null, null, null);

            await Context.ConsolidacionesRequisitosExpedientes.AddAsync(new ConsolidacionRequisitoExpediente
            {
                Id = 1,
                ExpedienteAlumno = new ExpedienteAlumno
                {
                    Id = 1
                },
                RequisitoExpediente = new RequisitoExpediente
                {
                    Id = 1
                }
            });
            await Context.SaveChangesAsync();

            //ACT
            var ex = await Record.ExceptionAsync(async () =>
            {
                await sut.Handle(request, CancellationToken.None);
            });

            //ASSERT
            Assert.NotNull(ex);
            Assert.IsType<NotFoundException>(ex);
        }

        [Fact(DisplayName = "Cuando no existe la configuración de la universidad Devuelve una excepción")]
        public async Task Handle_NotFoundException_ConfiguracionExpedienteUniversidad()
        {
            //ARRANGE
            var request = new UploadFileConsolidacionRequisitoDocumentoCommand
            {
                IdConsolidacionRequisito = 1,
                IdRequisitoDocumento = 1
            };
            var sut = new UploadFileConsolidacionRequisitoDocumentoCommandHandler(Context, null, null, null);

            await Context.ConsolidacionesRequisitosExpedientes.AddAsync(new ConsolidacionRequisitoExpediente
            {
                Id = 1,
                ExpedienteAlumno = new ExpedienteAlumno
                {
                    Id = 1
                },
                RequisitoExpediente = new RequisitoExpediente
                {
                    Id = 1
                }
            });
            await Context.RequisitosExpedientesDocumentos.AddAsync(new RequisitoExpedienteDocumento
            {
                Id = 1,
                NombreDocumento = Guid.NewGuid().ToString()
            });
            await Context.SaveChangesAsync();

            //ACT
            var ex = await Record.ExceptionAsync(async () =>
            {
                await sut.Handle(request, CancellationToken.None);
            });

            //ASSERT
            Assert.NotNull(ex);
            Assert.IsType<NotFoundException>(ex);
        }

        [Fact(DisplayName = "Cuando el proceso es correcto Devuelve Ok")]
        public async Task Handle_Ok()
        {
            //ARRANGE
            var bytes = Encoding.UTF8.GetBytes("Archivo de prueba");
            IFormFile file = new FormFile(new MemoryStream(bytes), 0, bytes.Length, "Data", "file.docx");
            var request = new UploadFileConsolidacionRequisitoDocumentoCommand
            {
                IdConsolidacionRequisito = 1,
                IdRequisitoDocumento = 1,
                File = file
            };
            var sutMock = new Mock<UploadFileConsolidacionRequisitoDocumentoCommandHandler>(Context, null, null, null)
            {
                CallBase = true
            };
            sutMock.Setup(x => x.ValidatePropiedades(
                It.IsAny<ConfiguracionExpedienteUniversidad>(),
                It.IsAny<UploadFileConsolidacionRequisitoDocumentoCommand>()));
            sutMock.Setup(x => x.GetDocumento(It.IsAny<ConsolidacionRequisitoExpediente>(),
                It.IsAny<ConsolidacionRequisitoExpedienteDocumento>(), It.IsAny<string>(),
                It.IsAny<string>())).ReturnsAsync(new DocumentoModel());
            sutMock.Setup(x => x.ConvertToBytes(It.IsAny<IFormFile>())).Returns(Array.Empty<byte>());
            sutMock.Setup(x => x.WriteFileDocumento(It.IsAny<byte[]>(),
                It.IsAny<ConsolidacionRequisitoExpedienteDocumento>(), It.IsAny<string>())).Returns(Task.CompletedTask);

            await Context.ConsolidacionesRequisitosExpedientes.AddAsync(new ConsolidacionRequisitoExpediente
            {
                Id = 1,
                ExpedienteAlumno = new ExpedienteAlumno
                {
                    Id = 1,
                    IdRefUniversidad = "1"
                },
                RequisitoExpediente = new RequisitoExpediente
                {
                    Id = 1
                }
            });
            await Context.RequisitosExpedientesDocumentos.AddAsync(new RequisitoExpedienteDocumento
            {
                Id = 1,
                NombreDocumento = Guid.NewGuid().ToString()
            });
            await Context.ConfiguracionesExpedientesUniversidades.AddAsync(new ConfiguracionExpedienteUniversidad
            {
                Id = 1,
                IdRefUniversidad = "1"
            });
            await Context.SaveChangesAsync();

            //ACT
            var actual = await sutMock.Object.Handle(request, CancellationToken.None);

            //ASSERT
            Assert.True(actual > 0);
            Assert.True(await Context.ConsolidacionesRequisitosExpedientesDocumentos.AnyAsync());
            sutMock.Verify(x => x.ValidatePropiedades(
                It.IsAny<ConfiguracionExpedienteUniversidad>(),
                It.IsAny<UploadFileConsolidacionRequisitoDocumentoCommand>()), Times.Once);
            sutMock.Verify(x => x.GetDocumento(It.IsAny<ConsolidacionRequisitoExpediente>(),
                It.IsAny<ConsolidacionRequisitoExpedienteDocumento>(), It.IsAny<string>(),
                It.IsAny<string>()), Times.Once);
            sutMock.Verify(x => x.ConvertToBytes(It.IsAny<IFormFile>()), Times.Once);
            sutMock.Verify(x => x.WriteFileDocumento(It.IsAny<byte[]>(),
                It.IsAny<ConsolidacionRequisitoExpedienteDocumento>(), It.IsAny<string>()), Times.Once);
        }

        #endregion

        #region ValidatePropiedades

        [Fact(DisplayName = "Cuando la configuración de la universidad no tiene código documental Devuelve excepción")]
        public void ValidatePropiedades_CodigoDocumental_BadRequestException()
        {
            //ARRANGE
            var request = new UploadFileConsolidacionRequisitoDocumentoCommand();
            var mockIStringLocalizer = new Mock<IStringLocalizer<UploadFileConsolidacionRequisitoDocumentoCommandHandler>>
            {
                CallBase = true
            };
            var sut = new UploadFileConsolidacionRequisitoDocumentoCommandHandler(Context, null, null, mockIStringLocalizer.Object);
            var configuracionExpedienteUniversidad = new ConfiguracionExpedienteUniversidad();
            const string mensajeEsperado = "No existe clasificación documental asociada a la universidad";
            mockIStringLocalizer.Setup(s => s[It.Is<string>(msj => msj == mensajeEsperado)])
                .Returns(new LocalizedString(mensajeEsperado, mensajeEsperado));

            //ACT
            var ex = Record.Exception( () =>
            {
                sut.ValidatePropiedades(configuracionExpedienteUniversidad, request);
            });

            //ASSERT
            Assert.NotNull(ex);
            Assert.IsType<BadRequestException>(ex);
            Assert.Equal(mensajeEsperado, ex.Message);
        }

        [Fact(DisplayName = "Cuando la extensión del archivo no es permitida Devuelve excepción")]
        public void ValidatePropiedades_ExtensionInvalida_BadRequestException()
        {
            //ARRANGE
            var bytes = Encoding.UTF8.GetBytes("Archivo de prueba");
            IFormFile file = new FormFile(new MemoryStream(bytes), 0, bytes.Length, "Data", "file.exe");
            var request = new UploadFileConsolidacionRequisitoDocumentoCommand
            {
                File = file
            };
            var mockIStringLocalizer = new Mock<IStringLocalizer<UploadFileConsolidacionRequisitoDocumentoCommandHandler>>
            {
                CallBase = true
            };
            var sut = new UploadFileConsolidacionRequisitoDocumentoCommandHandler(Context, null, null, mockIStringLocalizer.Object);
            var configuracionExpedienteUniversidad = new ConfiguracionExpedienteUniversidad
            {
                CodigoDocumental = Guid.NewGuid().ToString()
            };
            const string mensajeEsperado = "Extensión de archivo no permitido";
            mockIStringLocalizer.Setup(s => s[It.Is<string>(msj => msj == mensajeEsperado)])
                .Returns(new LocalizedString(mensajeEsperado, mensajeEsperado));

            //ACT
            var ex = Record.Exception(() =>
            {
                sut.ValidatePropiedades(configuracionExpedienteUniversidad, request);
            });

            //ASSERT
            Assert.NotNull(ex);
            Assert.IsType<BadRequestException>(ex);
            Assert.Equal(mensajeEsperado, ex.Message);
        }

        [Fact(DisplayName = "Cuando se supera el tamaño máximo configurado Devuelve excepción")]
        public void ValidatePropiedades_TamanyoMaximoFichero_BadRequestException()
        {
            //ARRANGE
            var bytes = new byte[10000000];
            IFormFile file = new FormFile(new MemoryStream(bytes), 0, bytes.Length, "Data", "file.docx");
            var request = new UploadFileConsolidacionRequisitoDocumentoCommand
            {
                File = file
            };
            var mockIStringLocalizer = new Mock<IStringLocalizer<UploadFileConsolidacionRequisitoDocumentoCommandHandler>>
            {
                CallBase = true
            };
            var sut = new UploadFileConsolidacionRequisitoDocumentoCommandHandler(Context, null, null, mockIStringLocalizer.Object);
            var configuracionExpedienteUniversidad = new ConfiguracionExpedienteUniversidad
            {
                CodigoDocumental = Guid.NewGuid().ToString(),
                TamanyoMaximoFichero = 1
            };
            var mensajeEsperado = $"El tamaño máximo es de {configuracionExpedienteUniversidad.TamanyoMaximoFichero} MB";
            mockIStringLocalizer.Setup(s => s[It.Is<string>(msj => msj == mensajeEsperado)])
                .Returns(new LocalizedString(mensajeEsperado, mensajeEsperado));

            //ACT
            var ex = Record.Exception(() =>
            {
                sut.ValidatePropiedades(configuracionExpedienteUniversidad, request);
            });

            //ASSERT
            Assert.NotNull(ex);
            Assert.IsType<BadRequestException>(ex);
            Assert.Equal(mensajeEsperado, ex.Message);
        }

        #endregion

        #region GetDocumento

        [Fact(DisplayName = "Cuando no se puede obtener las clasificaciones de gestor documental Devuelve una excepción")]
        public async Task GetDocumento_Clasificiones_BadRequestException()
        {
            //ARRANGE
            var mockIGestorDocumentalServiceClient = new Mock<IGestorDocumentalServiceClient> { CallBase = true };
            var mockIStringLocalizer = new Mock<IStringLocalizer<UploadFileConsolidacionRequisitoDocumentoCommandHandler>>
            {
                CallBase = true
            };
            var sut = new UploadFileConsolidacionRequisitoDocumentoCommandHandler(Context, null, mockIGestorDocumentalServiceClient.Object, mockIStringLocalizer.Object);
            mockIGestorDocumentalServiceClient.Setup(x => x.GetClasificaciones(It.IsAny<ClasificacionListParameters>()))
                .ReturnsAsync(new List<ClasificacionModel>());
            const string mensajeEsperado = "No se encontró la clasificación en Gestor Documental";
            mockIStringLocalizer.Setup(s => s[It.Is<string>(msj => msj == mensajeEsperado)])
                .Returns(new LocalizedString(mensajeEsperado, mensajeEsperado));

            var consolidacionRequisitoExpediente = new ConsolidacionRequisitoExpediente();
            var consolidacionRequisitoExpedienteDocumento = new ConsolidacionRequisitoExpedienteDocumento();
            var codigoClasificacion = Guid.NewGuid().ToString();
            var nombreArchivo = Guid.NewGuid().ToString();

            //ACT
            var ex = await Record.ExceptionAsync(async () =>
            {
                await sut.GetDocumento(consolidacionRequisitoExpediente, consolidacionRequisitoExpedienteDocumento, codigoClasificacion, nombreArchivo);
            });

            //ASSERT
            Assert.NotNull(ex);
            Assert.IsType<BadRequestException>(ex);
            Assert.Equal(mensajeEsperado, ex.Message);
            mockIGestorDocumentalServiceClient.Verify(x => x.GetClasificaciones(
                It.IsAny<ClasificacionListParameters>()), Times.Once);
        }

        [Fact(DisplayName = "Cuando no se puede guardar el documento en gestor documental Devuelve una excepción")]
        public async Task GetDocumento_Documento_BadRequestException()
        {
            //ARRANGE
            var mockIGestorDocumentalServiceClient = new Mock<IGestorDocumentalServiceClient> { CallBase = true };
            var mockIStringLocalizer = new Mock<IStringLocalizer<UploadFileConsolidacionRequisitoDocumentoCommandHandler>>
            {
                CallBase = true
            };
            var sut = new UploadFileConsolidacionRequisitoDocumentoCommandHandler(Context, null, mockIGestorDocumentalServiceClient.Object, mockIStringLocalizer.Object);

            var clasificacionModel = new ClasificacionModel
            {
                Id = 1
            };
            mockIGestorDocumentalServiceClient.Setup(x => x.GetClasificaciones(It.IsAny<ClasificacionListParameters>()))
                .ReturnsAsync(new List<ClasificacionModel>{ clasificacionModel });

            const string mensajeEsperado = "Error al guardar el documento";
            mockIGestorDocumentalServiceClient.Setup(x => x.SaveDocumento(It.IsAny<DocumentoParameters>()))
                .ReturnsAsync(new DocumentoModel
                {
                    Error = mensajeEsperado
                });
            
            mockIStringLocalizer.Setup(s => s[It.Is<string>(msj => msj == mensajeEsperado)])
                .Returns(new LocalizedString(mensajeEsperado, mensajeEsperado));

            var consolidacionRequisitoExpediente = new ConsolidacionRequisitoExpediente
            {
                ExpedienteAlumno = new ExpedienteAlumno(),
                RequisitoExpediente = new RequisitoExpediente()
            };
            var consolidacionRequisitoExpedienteDocumento = new ConsolidacionRequisitoExpedienteDocumento
            {
                RequisitoExpedienteDocumento = new RequisitoExpedienteDocumento()
            };
            var codigoClasificacion = Guid.NewGuid().ToString();
            var nombreArchivo = Guid.NewGuid().ToString();

            //ACT
            var ex = await Record.ExceptionAsync(async () =>
            {
                await sut.GetDocumento(consolidacionRequisitoExpediente, consolidacionRequisitoExpedienteDocumento, codigoClasificacion, nombreArchivo);
            });

            //ASSERT
            Assert.NotNull(ex);
            Assert.IsType<BadRequestException>(ex);
            Assert.Equal(mensajeEsperado, ex.Message);
            mockIGestorDocumentalServiceClient.Verify(x => x.GetClasificaciones(
                It.IsAny<ClasificacionListParameters>()), Times.Once);
            mockIGestorDocumentalServiceClient.Verify(x => x.SaveDocumento(It.IsAny<DocumentoParameters>()), Times.Once);
        }

        [Fact(DisplayName = "Cuando se guarda el documento en gestor documental Devuelve Ok")]
        public async Task GetDocumento_Ok()
        {
            //ARRANGE
            var mockIGestorDocumentalServiceClient = new Mock<IGestorDocumentalServiceClient> { CallBase = true };
            var mockIStringLocalizer = new Mock<IStringLocalizer<UploadFileConsolidacionRequisitoDocumentoCommandHandler>>
            {
                CallBase = true
            };
            var sut = new UploadFileConsolidacionRequisitoDocumentoCommandHandler(Context, null, mockIGestorDocumentalServiceClient.Object, mockIStringLocalizer.Object);

            var clasificacionModel = new ClasificacionModel
            {
                Id = 1
            };
            mockIGestorDocumentalServiceClient.Setup(x => x.GetClasificaciones(It.IsAny<ClasificacionListParameters>()))
                .ReturnsAsync(new List<ClasificacionModel> { clasificacionModel });
            var documento = new DocumentoModel
            {
                Id = "1",
                Nombre = Guid.NewGuid().ToString()
            };
            mockIGestorDocumentalServiceClient.Setup(x => x.SaveDocumento(It.IsAny<DocumentoParameters>()))
                .ReturnsAsync(documento);

            var consolidacionRequisitoExpediente = new ConsolidacionRequisitoExpediente
            {
                ExpedienteAlumno = new ExpedienteAlumno(),
                RequisitoExpediente = new RequisitoExpediente()
            };
            var consolidacionRequisitoExpedienteDocumento = new ConsolidacionRequisitoExpedienteDocumento
            {
                RequisitoExpedienteDocumento = new RequisitoExpedienteDocumento()
            };
            var codigoClasificacion = Guid.NewGuid().ToString();
            var nombreArchivo = Guid.NewGuid().ToString();

            //ACT
            var actual = await sut.GetDocumento(consolidacionRequisitoExpediente, consolidacionRequisitoExpedienteDocumento, codigoClasificacion, nombreArchivo);

            //ASSERT
            Assert.NotNull(actual);
            Assert.Equal(documento.Id, actual.Id);
            Assert.Equal(documento.Nombre, actual.Nombre);
            mockIGestorDocumentalServiceClient.Verify(x => x.GetClasificaciones(
                It.IsAny<ClasificacionListParameters>()), Times.Once);
            mockIGestorDocumentalServiceClient.Verify(x => x.SaveDocumento(It.IsAny<DocumentoParameters>()), Times.Once);
        }

        #endregion

        #region ConvertToBytes

        [Fact(DisplayName = "Cuando se convierte el archivo a un arreglo de bytes Devuelve Ok")]
        public void ConvertToBytes_Ok()
        {
            //ARRANGE
            var bytes = new byte[10000000];
            IFormFile file = new FormFile(new MemoryStream(bytes), 0, bytes.Length, "Data", "file.docx");
            var sut = new UploadFileConsolidacionRequisitoDocumentoCommandHandler(Context, null, null, null);

            //ACT
            var actual = sut.ConvertToBytes(file);

            //ASSERT
            Assert.NotNull(actual);
            Assert.Equal(bytes, actual);
        }

        #endregion

        #region WriteFileDocumento

        [Fact(DisplayName = "Cuando no se puede escribir el archivo Devuelve una excepción")]
        public async Task WriteFileDocumento_BadRequestException()
        {
            //ARRANGE
            var bytes = new byte[10000000];
            var consolidacionRequisitoExpedienteDocumento = new ConsolidacionRequisitoExpedienteDocumento
            {
                Id = 1
            };
            var nombreArchivo = Guid.NewGuid().ToString();
            var mockIFileManager = new Mock<IFileManager> {CallBase = true};
            var mockIStringLocalizer = new Mock<IStringLocalizer<UploadFileConsolidacionRequisitoDocumentoCommandHandler>>
            {
                CallBase = true
            };
            var sut = new UploadFileConsolidacionRequisitoDocumentoCommandHandler(Context, mockIFileManager.Object, null, mockIStringLocalizer.Object);
            mockIFileManager.Setup(x => x.WriteFileAsync(It.IsAny<string>(), It.IsAny<byte[]>())).ReturnsAsync(false);
            const string mensajeEsperado = "No se ha podido guardar el archivo";
            mockIStringLocalizer.Setup(s => s[It.Is<string>(msj => msj == mensajeEsperado)])
                .Returns(new LocalizedString(mensajeEsperado, mensajeEsperado));

            //ACT
            var ex = await Record.ExceptionAsync(async () =>
            {
                await sut.WriteFileDocumento(bytes, consolidacionRequisitoExpedienteDocumento, nombreArchivo);
            });

            //ASSERT
            Assert.NotNull(ex);
            Assert.IsType<BadRequestException>(ex);
            Assert.Equal(mensajeEsperado, ex.Message);
            mockIFileManager.Verify(x => x.WriteFileAsync(It.IsAny<string>(), It.IsAny<byte[]>()), Times.Once);
        }

        [Fact(DisplayName = "Cuando se escribe el archivo Devuelve Ok")]
        public async Task WriteFileDocumento_Ok()
        {
            //ARRANGE
            var bytes = new byte[10000000];
            var consolidacionRequisitoExpedienteDocumento = new ConsolidacionRequisitoExpedienteDocumento
            {
                Id = 1
            };
            var nombreArchivo = Guid.NewGuid().ToString();
            var mockIFileManager = new Mock<IFileManager> { CallBase = true };
            var sut = new UploadFileConsolidacionRequisitoDocumentoCommandHandler(Context, mockIFileManager.Object, null, null);
            mockIFileManager.Setup(x => x.WriteFileAsync(It.IsAny<string>(), 
                It.IsAny<byte[]>())).ReturnsAsync(true);
            //ACT
            await sut.WriteFileDocumento(bytes, consolidacionRequisitoExpedienteDocumento, nombreArchivo);

            //ASSERT
            mockIFileManager.Verify(x => x.WriteFileAsync(It.IsAny<string>(), It.IsAny<byte[]>()), Times.Once);
        }

        #endregion
    }
}
