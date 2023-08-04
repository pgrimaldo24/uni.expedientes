using Microsoft.Extensions.Localization;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Unir.Expedientes.Application.RequisitosExpedientes.Commands.EditRequisito;
using Unir.Expedientes.Application.Tests.Common;
using Unir.Expedientes.Domain.Entities;
using Unir.Framework.ApplicationSuperTypes.Exceptions;
using Xunit;

namespace Unir.Expedientes.Application.Tests.RequisitosExpedientes.Commands.EditRequisito
{
    [Collection("CommonTestCollection")]
    public class EditRequisitoCommandHandlerTests : TestBase
    {
        #region Handle

        [Fact(DisplayName = "Cuando el requisito a crear no tiene todos los datos requeridos Devuelve una excepción")]
        public async Task Handle_DatosIncompletos()
        {
            //ARRANGE
            var request = new EditRequisitoCommand();
            var mockIStringLocalizer = new Mock<IStringLocalizer<EditRequisitoCommandHandler>>
            {
                CallBase = true
            };
            var sutMock = new Mock<EditRequisitoCommandHandler>(Context, mockIStringLocalizer.Object)
            {
                CallBase = true
            };
            const string mensajeError = "Mensaje de error";
            sutMock.Setup(x => x.ValidatePropiedadesRequeridas(
                It.IsAny<EditRequisitoCommand>(), It.IsAny<CancellationToken>()))
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
                It.IsAny<EditRequisitoCommand>(), It.IsAny<CancellationToken>()), Times.Once());
        }

        [Fact(DisplayName = "Cuando el requisito no existe Devuelve una excepción")]
        public async Task Handle_NotFoundException()
        {
            //ARRANGE
            var request = new EditRequisitoCommand
            {
                Id = 1
            };
            var mockIStringLocalizer = new Mock<IStringLocalizer<EditRequisitoCommandHandler>>
            {
                CallBase = true
            };
            var sutMock = new Mock<EditRequisitoCommandHandler>(Context, mockIStringLocalizer.Object)
            {
                CallBase = true
            };
            sutMock.Setup(s => s.ValidatePropiedadesRequeridas(It.IsAny<EditRequisitoCommand>(), 
                    It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

            //ACT
            var ex = await Record.ExceptionAsync(async () =>
            {
                await sutMock.Object.Handle(request, CancellationToken.None);
            });

            //ASSERT
            Assert.NotNull(ex);
            Assert.IsType<NotFoundException>(ex);
            sutMock.Verify(s => s.ValidatePropiedadesRequeridas(It.IsAny<EditRequisitoCommand>(), 
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact(DisplayName = "Cuando edita el requisito Devuelve ok")]
        public async Task Handle_Ok()
        {
            //ARRANGE
            var request = new EditRequisitoCommand
            {
                Id = 1
            };
            var requisito = new RequisitoExpediente
            {
                Id = 1,
                Nombre = Guid.NewGuid().ToString(),
                Orden = 5
            };
            await Context.RequisitosExpedientes.AddAsync(requisito);
            await Context.SaveChangesAsync();
            var mockIStringLocalizer = new Mock<IStringLocalizer<EditRequisitoCommandHandler>>
            {
                CallBase = true
            };
            var sutMock = new Mock<EditRequisitoCommandHandler>(Context, mockIStringLocalizer.Object)
            {
                CallBase = true
            };
            sutMock.Setup(s => s.ValidatePropiedadesRequeridas(It.IsAny<EditRequisitoCommand>(),
                It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
            sutMock.Setup(s => s.AssignEditRequisito(It.IsAny<EditRequisitoCommand>(), It.IsAny<RequisitoExpediente>()));
            sutMock.Setup(s => s.AssignRequerimientoTitulo(It.IsAny<EditRequisitoCommand>(), It.IsAny<RequisitoExpediente>(), 
                It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
            sutMock.Setup(s => s.AssignEstadoExpedienteTitulo(It.IsAny<EditRequisitoCommand>(), It.IsAny<RequisitoExpediente>(),
                It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
            sutMock.Setup(s => s.AssignRoles(It.IsAny<EditRequisitoCommand>(), It.IsAny<RequisitoExpediente>()));
            sutMock.Setup(s => s.AssignFilesTypes(It.IsAny<EditRequisitoCommand>(), It.IsAny<RequisitoExpediente>()));
            sutMock.Setup(s => s.UpdateDocumentosSecurizados(It.IsAny<bool>(), It.IsAny<ICollection<RequisitoExpedienteDocumento>>()));

            //ACT
            await sutMock.Object.Handle(request, CancellationToken.None);

            //ASSERT
            sutMock.Verify(s => s.ValidatePropiedadesRequeridas(It.IsAny<EditRequisitoCommand>(), It.IsAny<CancellationToken>()), Times.Once);
            sutMock.Verify(s => s.AssignEditRequisito(It.IsAny<EditRequisitoCommand>(), It.IsAny<RequisitoExpediente>()), Times.Once);
            sutMock.Verify(s => s.AssignRequerimientoTitulo(It.IsAny<EditRequisitoCommand>(), It.IsAny<RequisitoExpediente>(),
                It.IsAny<CancellationToken>()), Times.Once);
            sutMock.Verify(s => s.AssignEstadoExpedienteTitulo(It.IsAny<EditRequisitoCommand>(), It.IsAny<RequisitoExpediente>(),
                It.IsAny<CancellationToken>()), Times.Once);
            sutMock.Verify(s => s.AssignRoles(It.IsAny<EditRequisitoCommand>(), It.IsAny<RequisitoExpediente>()), Times.Once);
            sutMock.Verify(s => s.AssignFilesTypes(It.IsAny<EditRequisitoCommand>(), It.IsAny<RequisitoExpediente>()), Times.Once);
            sutMock.Verify(s => s.UpdateDocumentosSecurizados(It.IsAny<bool>(), It.IsAny<ICollection<RequisitoExpedienteDocumento>>()), 
                Times.Once);
        }

        #endregion

        #region UpdateDocumentosSecurizados

        [Fact(DisplayName = "Cuando existen requisitos Devuelve lista actualizada")]
        public void UpdateDocumentosSecurizados_Ok()
        {
            //ARRANGE
            var requisito = new RequisitoExpediente
            {
                RequisitosExpedientesDocumentos = new List<RequisitoExpedienteDocumento>
                {
                    new RequisitoExpedienteDocumento(),
                    new RequisitoExpedienteDocumento()
                }
            };
            var mockIStringLocalizer = new Mock<IStringLocalizer<EditRequisitoCommandHandler>>
            {
                CallBase = true
            };
            var sut = new EditRequisitoCommandHandler(Context, mockIStringLocalizer.Object);

            //ACT
            sut.UpdateDocumentosSecurizados(true, requisito.RequisitosExpedientesDocumentos);

            //ASSERT
            Assert.Equal(2, requisito.RequisitosExpedientesDocumentos.ToList().Where(red => red.DocumentoSecurizado).Count());
        }

        #endregion

        #region AssignEditRequisito

        [Fact(DisplayName = "Cuando se asignan propiedades Devuelve objeto requisito")]
        public void AssignEditRequisito_Ok()
        {
            //ARRANGE
            var request = new EditRequisitoCommand
            {
                Nombre = Guid.NewGuid().ToString(),
                Orden = 5,
                Descripcion = Guid.NewGuid().ToString(),
                EstaVigente = true,
                RequeridaParaTitulo = false,
                RequiereDocumentacion = true,
                EnviarEmailAlumno = true,
                RequeridaParaPago = false,
                EstaRestringida = true,
                EsCertificado = false,
                RequiereTextoAdicional = true,
                IdRefModoRequerimientoDocumentacion = 5
            };
            var requisito = new RequisitoExpediente();
            var mockIStringLocalizer = new Mock<IStringLocalizer<EditRequisitoCommandHandler>>
            {
                CallBase = true
            };
            var sut = new EditRequisitoCommandHandler(Context, mockIStringLocalizer.Object);

            //ACT
            sut.AssignEditRequisito(request, requisito);

            //ASSERT
            Assert.Equal(request.Nombre, requisito.Nombre);
            Assert.Equal(request.Orden, requisito.Orden);
            Assert.Equal(request.Descripcion, requisito.Descripcion);
            Assert.Equal(request.EstaVigente, requisito.EstaVigente);
            Assert.Equal(request.RequeridaParaTitulo, requisito.RequeridaParaTitulo);
            Assert.Equal(request.RequiereDocumentacion, requisito.RequiereDocumentacion);
            Assert.Equal(request.EnviarEmailAlumno, requisito.EnviarEmailAlumno);
            Assert.Equal(request.RequeridaParaPago, requisito.RequeridaParaPago);
            Assert.Equal(request.EstaRestringida, requisito.EstaRestringida);
            Assert.Equal(request.EsCertificado, requisito.EsCertificado);
            Assert.Equal(request.RequiereTextoAdicional, requisito.RequiereTextoAdicional);
            Assert.Equal(request.IdRefModoRequerimientoDocumentacion, requisito.IdRefModoRequerimientoDocumentacion);
        }

        #endregion

        #region AssignRequerimientoTitulo

        [Fact(DisplayName = "Cuando se envían requerimientos los asigna y Devuelve el objeto actualizado")]
        public async Task AssignRequerimientoTitulo_Ok()
        {
            //ARRANGE
            var request = new EditRequisitoCommand
            {
                Id = 1,
                RequisitosExpedientesRequerimientosTitulos = new RequisitoExpedienteRequerimientoTituloDto 
                {
                    IdTipoRelacionExpediente = 1, 
                    RequiereMatricularse = true
                }
            };

            var tipoRelacionExpediente = new TipoRelacionExpediente
            {
                Id = 1
            };
            await Context.TiposRelacionesExpediente.AddAsync(tipoRelacionExpediente);
            await Context.SaveChangesAsync();

            var requerimiento = new RequisitoExpedienteRequerimientoTitulo
            {
                Id = 1,
                TipoRelacionExpediente =
                        tipoRelacionExpediente,
                RequiereMatricularse = false
            };
            await Context.RequisitosExpedientesRequerimientosTitulos.AddAsync(requerimiento);
            await Context.SaveChangesAsync();

            var requisito = new RequisitoExpediente
            {
                Id = 1,
                RequisitosExpedientesRequerimientosTitulos = new List<RequisitoExpedienteRequerimientoTitulo>
                {
                    requerimiento
                }
            };
            await Context.RequisitosExpedientes.AddAsync(requisito);
            await Context.SaveChangesAsync();
            var mockIStringLocalizer = new Mock<IStringLocalizer<EditRequisitoCommandHandler>>
            {
                CallBase = true
            };
            var sut = new EditRequisitoCommandHandler(Context, mockIStringLocalizer.Object);

            //ACT
            await sut.AssignRequerimientoTitulo(request, requisito, CancellationToken.None);
            await Context.SaveChangesAsync();

            //ASSERT
            Assert.Null(requisito.RequisitosExpedientesRequerimientosTitulos.FirstOrDefault(rert => rert.RequiereMatricularse == false));
            Assert.NotNull(requisito.RequisitosExpedientesRequerimientosTitulos.FirstOrDefault(rert => rert.RequiereMatricularse == true));
        }

        [Fact(DisplayName = "Cuando no existe el tipo de relación Devuelve una excepción")]
        public async Task AssignRequerimientoTitulo_NotFoundException()
        {
            //ARRANGE
            var request = new EditRequisitoCommand
            {
                Id = 1,
                RequisitosExpedientesRequerimientosTitulos = new RequisitoExpedienteRequerimientoTituloDto
                {
                    IdTipoRelacionExpediente = 1,
                    RequiereMatricularse = true
                }
            };

            var requisito = new RequisitoExpediente
            {
                Id = 1,
                RequisitosExpedientesRequerimientosTitulos = new List<RequisitoExpedienteRequerimientoTitulo>()
            };
            await Context.RequisitosExpedientes.AddAsync(requisito);
            await Context.SaveChangesAsync();
            var mockIStringLocalizer = new Mock<IStringLocalizer<EditRequisitoCommandHandler>>
            {
                CallBase = true
            };
            var sut = new EditRequisitoCommandHandler(Context, mockIStringLocalizer.Object);

            //ACT
            var ex = await Record.ExceptionAsync(async () =>
            {
                await sut.AssignRequerimientoTitulo(request, requisito, CancellationToken.None);
            });

            //ASSERT
            Assert.NotNull(ex);
            Assert.IsType<NotFoundException>(ex);
        }

        [Fact(DisplayName = "Cuando no se envía el requisito Devuelve objeto vacío")]
        public async Task AssignRequerimientoTitulo_SinRequisito()
        {
            //ARRANGE
            var request = new EditRequisitoCommand
            {
                Id = 1,
                RequisitosExpedientesRequerimientosTitulos = new RequisitoExpedienteRequerimientoTituloDto
                {
                    RequiereMatricularse = false
                }
            };

            var requisito = new RequisitoExpediente
            {
                Id = 1,
                RequisitosExpedientesRequerimientosTitulos = new List<RequisitoExpedienteRequerimientoTitulo>()
            };
            await Context.RequisitosExpedientes.AddAsync(requisito);
            await Context.SaveChangesAsync();
            var mockIStringLocalizer = new Mock<IStringLocalizer<EditRequisitoCommandHandler>>
            {
                CallBase = true
            };
            var sut = new EditRequisitoCommandHandler(Context, mockIStringLocalizer.Object);

            //ACT
            await sut.AssignRequerimientoTitulo(request, requisito, CancellationToken.None);
            await Context.SaveChangesAsync();

            //ASSERT
            Assert.Null(requisito.RequisitosExpedientesRequerimientosTitulos.FirstOrDefault());
        }

        #endregion

        #region AssignEstadoExpedienteTitulo

        [Fact(DisplayName = "Cuando no se envía el requisito Devuelve objeto vacío")]
        public async Task AssignEstadoExpedienteTitulo_Ok()
        {
            //ARRANGE
            var request = new EditRequisitoCommand
            {
                Id = 1,
                IdEstadoExpediente = 1,
                RequisitosExpedientesRequerimientosTitulos = new RequisitoExpedienteRequerimientoTituloDto
                {
                    RequiereMatricularse = false
                }
            };

            var estado = new EstadoExpediente
            {
                Id = 1
            };
            await Context.EstadosExpedientes.AddAsync(estado);
            await Context.SaveChangesAsync();

            var requisito = new RequisitoExpediente
            {
                Id = 1,
                RequisitosExpedientesRequerimientosTitulos = new List<RequisitoExpedienteRequerimientoTitulo>()
            };
            await Context.RequisitosExpedientes.AddAsync(requisito);
            await Context.SaveChangesAsync();
            var mockIStringLocalizer = new Mock<IStringLocalizer<EditRequisitoCommandHandler>>
            {
                CallBase = true
            };
            var sut = new EditRequisitoCommandHandler(Context, mockIStringLocalizer.Object);

            //ACT
            await sut.AssignEstadoExpedienteTitulo(request, requisito, CancellationToken.None);
            await Context.SaveChangesAsync();

            //ASSERT
            Assert.NotNull(requisito.EstadoExpediente);
        }

        #endregion

        #region ValidatePropiedadesRequeridas

        [Fact(DisplayName = "Cuando se envían todas las propiedades requeridas Devuelve ok")]
        public void ValidatePropiedadesRequeridas_Ok()
        {
            //ARRANGE
            var request = new EditRequisitoCommand
            {
                Nombre = Guid.NewGuid().ToString(),
                RequisitosExpedientesRequerimientosTitulos = new RequisitoExpedienteRequerimientoTituloDto()
            };
            var mockIStringLocalizer = new Mock<IStringLocalizer<EditRequisitoCommandHandler>>
            {
                CallBase = true
            };
            var sut = new EditRequisitoCommandHandler(Context, mockIStringLocalizer.Object);

            //ACT
            var actual = sut.ValidatePropiedadesRequeridas(request, CancellationToken.None);

            //ASSERT
            Assert.True(actual.IsCompleted);
        }

        [Fact(DisplayName = "Cuando no se envía nombre Devuelve excepción")]
        public async Task ValidatePropiedadesRequeridas_Nombre()
        {
            //ARRANGE
            var request = new EditRequisitoCommand();
            var mockIStringLocalizer = new Mock<IStringLocalizer<EditRequisitoCommandHandler>>
            {
                CallBase = true
            };
            var sut = new EditRequisitoCommandHandler(Context, mockIStringLocalizer.Object);

            const string mensajeEsperado = "El campo Nombre es requerido para editar el Requisito.";
            mockIStringLocalizer.Setup(s => s[It.Is<string>(msj => msj == mensajeEsperado)])
                .Returns(new LocalizedString(mensajeEsperado, mensajeEsperado));

            //ACT
            var ex = await Record.ExceptionAsync(async () =>
            {
                await sut.ValidatePropiedadesRequeridas(request, CancellationToken.None);
            });

            //ASSERT
            Assert.NotNull(ex);
            Assert.IsType<BadRequestException>(ex);
            Assert.Equal(mensajeEsperado, ex.Message);
        }

        [Fact(DisplayName = "Cuando se marca enviar por email pero no se ingresa descripción Devuelve excepción")]
        public async Task ValidatePropiedadesRequeridas_EnviarEmailAlumno_Descripcion()
        {
            //ARRANGE
            var request = new EditRequisitoCommand
            {
                Nombre = Guid.NewGuid().ToString(),
                EnviarEmailAlumno = true
            };
            var mockIStringLocalizer = new Mock<IStringLocalizer<EditRequisitoCommandHandler>>
            {
                CallBase = true
            };
            var sut = new EditRequisitoCommandHandler(Context, mockIStringLocalizer.Object);

            const string mensajeEsperado = "El campo Descripción es requerido para editar el Requisito.";
            mockIStringLocalizer.Setup(s => s[It.Is<string>(msj => msj == mensajeEsperado)])
                .Returns(new LocalizedString(mensajeEsperado, mensajeEsperado));

            //ACT
            var ex = await Record.ExceptionAsync(async () =>
            {
                await sut.ValidatePropiedadesRequeridas(request, CancellationToken.None);
            });

            //ASSERT
            Assert.NotNull(ex);
            Assert.IsType<BadRequestException>(ex);
            Assert.Equal(mensajeEsperado, ex.Message);
        }

        [Fact(DisplayName = "Cuando se marca requiere matricularse pero no se selecciona tipo de relación Devuelve excepción")]
        public async Task ValidatePropiedadesRequeridas_RequiereMatricularse_TipoRelacion()
        {
            //ARRANGE
            var request = new EditRequisitoCommand
            {
                Nombre = Guid.NewGuid().ToString(),
                EnviarEmailAlumno = true,
                Descripcion = Guid.NewGuid().ToString(),
                RequisitosExpedientesRequerimientosTitulos = new RequisitoExpedienteRequerimientoTituloDto
                {
                    RequiereMatricularse = true
                }
            };
            var mockIStringLocalizer = new Mock<IStringLocalizer<EditRequisitoCommandHandler>>
            {
                CallBase = true
            };
            var sut = new EditRequisitoCommandHandler(Context, mockIStringLocalizer.Object);

            const string mensajeEsperado = "El campo Tipo Expediente es requerido para editar el Requisito.";
            mockIStringLocalizer.Setup(s => s[It.Is<string>(msj => msj == mensajeEsperado)])
                .Returns(new LocalizedString(mensajeEsperado, mensajeEsperado));

            //ACT
            var ex = await Record.ExceptionAsync(async () =>
            {
                await sut.ValidatePropiedadesRequeridas(request, CancellationToken.None);
            });

            //ASSERT
            Assert.NotNull(ex);
            Assert.IsType<BadRequestException>(ex);
            Assert.Equal(mensajeEsperado, ex.Message);
        }

        [Fact(DisplayName = "Cuando el nombre ya existe Devuelve excepción")]
        public async Task ValidatePropiedadesRequeridas_ExisteNombre()
        {
            //ARRANGE
            var request = new EditRequisitoCommand
            {
                Nombre = Guid.NewGuid().ToString(),
                EnviarEmailAlumno = true,
                Descripcion = Guid.NewGuid().ToString(),
                RequisitosExpedientesRequerimientosTitulos = new RequisitoExpedienteRequerimientoTituloDto
                {
                    RequiereMatricularse = true,
                    IdTipoRelacionExpediente = 1
                }
            };
            var mockIStringLocalizer = new Mock<IStringLocalizer<EditRequisitoCommandHandler>>
            {
                CallBase = true
            };
            var sut = new EditRequisitoCommandHandler(Context, mockIStringLocalizer.Object);

            var mensajeEsperado = $"Ya existe un Requisito con el nombre: {request.Nombre}";
            mockIStringLocalizer.Setup(s => s[It.Is<string>(msj => msj == mensajeEsperado)])
                .Returns(new LocalizedString(mensajeEsperado, mensajeEsperado));

            await Context.RequisitosExpedientes.AddAsync(new RequisitoExpediente
            {
                Id = 1,
                Nombre = request.Nombre
            });
            await Context.SaveChangesAsync();

            //ACT
            var ex = await Record.ExceptionAsync(async () =>
            {
                await sut.ValidatePropiedadesRequeridas(request, CancellationToken.None);
            });

            //ASSERT
            Assert.NotNull(ex);
            Assert.IsType<BadRequestException>(ex);
            Assert.Equal(mensajeEsperado, ex.Message);
        }

        #endregion

        #region AssignRoles

        [Fact(DisplayName = "Cuando se asignan roles Devuelve objeto requisito")]
        public void AssignRoles_Ok()
        {
            //ARRANGE
            var mockRequisito = new Mock<RequisitoExpediente> { CallBase = true };
            mockRequisito.SetupAllProperties();
            mockRequisito.Object.Id = 1;
            var rolExistente = "Rol A";
            mockRequisito.Object.RolesRequisitosExpedientes = new List<RolRequisitoExpediente>
            {
                new()
                {
                    Id = 1,
                    Rol = rolExistente
                }
            };
            var request = new EditRequisitoCommand
            {
                Roles = new[] { Guid.NewGuid().ToString(), rolExistente, Guid.NewGuid().ToString() }
            };
            var mockIStringLocalizer = new Mock<IStringLocalizer<EditRequisitoCommandHandler>>
            {
                CallBase = true
            };
            var sut = new EditRequisitoCommandHandler(Context, mockIStringLocalizer.Object);
            mockRequisito.Setup(a => a.DeleteRolesNoIncluidos(It.IsAny<string[]>()));

            //ACT
            sut.AssignRoles(request, mockRequisito.Object);

            //ASSERT
            Assert.Equal(request.Roles.Length, mockRequisito.Object.RolesRequisitosExpedientes.Count);
            mockRequisito.Verify(a => a.DeleteRolesNoIncluidos(It.IsAny<string[]>()), Times.Once);
        }

        #endregion

        #region AssignFilesTypes

        [Fact(DisplayName = "Cuando se asignan files types Devuelve objeto requisito")]
        public void AssignFilesTypes_Ok()
        {
            //ARRANGE
            var mockRequisito = new Mock<RequisitoExpediente> { CallBase = true };
            mockRequisito.SetupAllProperties();
            mockRequisito.Object.Id = 1;
            var fileTypeExistente = "File type A";
            mockRequisito.Object.RequisitosExpedientesFilesType = new List<RequisitoExpedienteFileType>
            {
                new()
                {
                    Id = 1,
                    IdRefFileType = fileTypeExistente
                }
            };
            var request = new EditRequisitoCommand
            {
                IdsFilesTypes = new[] { Guid.NewGuid().ToString(), fileTypeExistente, Guid.NewGuid().ToString() }
            };
            var mockIStringLocalizer = new Mock<IStringLocalizer<EditRequisitoCommandHandler>>
            {
                CallBase = true
            };
            var sut = new EditRequisitoCommandHandler(Context, mockIStringLocalizer.Object);
            mockRequisito.Setup(a => a.DeleteFilesTypesNoIncluidos(It.IsAny<string[]>()));

            //ACT
            sut.AssignFilesTypes(request, mockRequisito.Object);

            //ASSERT
            Assert.Equal(request.IdsFilesTypes.Length, mockRequisito.Object.RequisitosExpedientesFilesType.Count);
            mockRequisito.Verify(a => a.DeleteFilesTypesNoIncluidos(It.IsAny<string[]>()), Times.Once);
        }

        #endregion
    }
}
