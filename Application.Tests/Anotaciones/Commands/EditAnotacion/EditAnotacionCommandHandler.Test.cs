using Microsoft.Extensions.Localization;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Unir.Expedientes.Application.Anotaciones.Commands.EditAnotacion;
using Unir.Expedientes.Application.Tests.Common;
using Unir.Expedientes.Domain.Entities;
using Unir.Framework.ApplicationSuperTypes.Exceptions;
using Unir.Framework.Crosscutting.Security;
using Unir.Framework.Crosscutting.Security.Model;
using Xunit;

namespace Unir.Expedientes.Application.Tests.Anotaciones.Commands.EditAnotacion
{
    [Collection("CommonTestCollection")]
    public class EditAnotacionCommandHandlerTests : TestBase
    {
        #region Handle

        [Fact(DisplayName = "Cuando los datos enviados en el request vienen vacíos Devuelve una excepción")]
        public async Task Handle_RequestInvalid()
        {
            //ARRANGE
            const string mensajeEsperado = "El campo Resumen es requerido para crear la Observación.";
            var request = new EditAnotacionCommand();

            var mockIStringLocalizer = new Mock<IStringLocalizer<EditAnotacionCommandHandler>>
            {
                CallBase = true
            };
            mockIStringLocalizer.Setup(s => s[It.Is<string>(msj => msj == mensajeEsperado)])
                .Returns(new LocalizedString(mensajeEsperado, mensajeEsperado));

            var mockIdentityService = new Mock<IIdentityService>
            {
                CallBase = true
            };
            var sut = new Mock<EditAnotacionCommandHandler>(Context,
                mockIStringLocalizer.Object, mockIdentityService.Object)
            {
                CallBase = true
            };
            sut.Setup(s => s.ValidatePropiedadesRequeridas(It.IsAny<EditAnotacionCommand>()))
                .Returns(PropiedadesRequeridas.Resumen);

            //ACT
            var ex = (BadRequestException)await Record.ExceptionAsync(async () =>
            {
                await sut.Object.Handle(request, CancellationToken.None);
            });

            //ASSERT
            Assert.NotNull(ex);
            Assert.IsType<BadRequestException>(ex);
            Assert.Equal(mensajeEsperado, ex.Message);
            sut.Verify(s => s.ValidatePropiedadesRequeridas(It.IsAny<EditAnotacionCommand>()), Times.Once);
        }

        [Fact(DisplayName = "Cuando no existe la anotación Devuelve una excepción")]
        public async Task Handle_Anotacion_NotFoundException()
        {
            //ARRANGE
            var request = new EditAnotacionCommand();

            var mockIStringLocalizer = new Mock<IStringLocalizer<EditAnotacionCommandHandler>>
            {
                CallBase = true
            };
            var mockIdentityService = new Mock<IIdentityService>
            {
                CallBase = true
            };
            var sut = new Mock<EditAnotacionCommandHandler>(Context,
                mockIStringLocalizer.Object, mockIdentityService.Object)
            {
                CallBase = true
            };
            sut.Setup(s => s.ValidatePropiedadesRequeridas(It.IsAny<EditAnotacionCommand>()))
                .Returns(PropiedadesRequeridas.Ninguno);

            //ACT
            var ex = (NotFoundException)await Record.ExceptionAsync(async () =>
            {
                await sut.Object.Handle(request, CancellationToken.None);
            });

            //ASSERT
            Assert.NotNull(ex);
            Assert.IsType<NotFoundException>(ex);
            sut.Verify(s => s.ValidatePropiedadesRequeridas(It.IsAny<EditAnotacionCommand>()), Times.Once);
        }

        [Fact(DisplayName = "Cuando no existe el expediente Devuelve una excepción")]
        public async Task Handle_NotFoundException()
        {
            //ARRANGE
            const string mensajeEsperado = "No existe el expediente.";
            var request = new EditAnotacionCommand
            {
                Id = 1
            };

            var mockIStringLocalizer = new Mock<IStringLocalizer<EditAnotacionCommandHandler>>
            {
                CallBase = true
            };
            mockIStringLocalizer.Setup(s => s[It.Is<string>(msj => msj == mensajeEsperado)])
                .Returns(new LocalizedString(mensajeEsperado, mensajeEsperado));
            var mockIdentityService = new Mock<IIdentityService>
            {
                CallBase = true
            };
            var sut = new Mock<EditAnotacionCommandHandler>(Context,
                mockIStringLocalizer.Object, mockIdentityService.Object)
            {
                CallBase = true
            };
            sut.Setup(s => s.ValidatePropiedadesRequeridas(It.IsAny<EditAnotacionCommand>()))
                .Returns(PropiedadesRequeridas.Ninguno);
            sut.Setup(x => x.AssignEditAnotacion(
                It.IsAny<EditAnotacionCommand>(), It.IsAny<Anotacion>()));

            var anotacion = new Anotacion
            {
                Id = 1
            };
            await Context.Anotaciones.AddAsync(anotacion);
            await Context.SaveChangesAsync();

            //ACT
            var ex = (NotFoundException)await Record.ExceptionAsync(async () =>
            {
                await sut.Object.Handle(request, CancellationToken.None);
            });

            //ASSERT
            Assert.NotNull(ex);
            Assert.IsType<NotFoundException>(ex);
            sut.Verify(s => s.ValidatePropiedadesRequeridas(It.IsAny<EditAnotacionCommand>()), Times.Once);
            sut.Verify(x => x.AssignEditAnotacion(
                It.IsAny<EditAnotacionCommand>(), It.IsAny<Anotacion>()), Times.Once);
        }

        [Fact(DisplayName = "Cuando no existe el usuario Devuelve una excepción")]
        public async Task Handle_User_BadRequestException()
        {
            //ARRANGE
            const string mensajeEsperado = "Usuario no encontrado.";
            var request = new EditAnotacionCommand
            {
                Id = 1,
                IdExpedienteAlumno = 1
            };

            var mockIStringLocalizer = new Mock<IStringLocalizer<EditAnotacionCommandHandler>>
            {
                CallBase = true
            };
            mockIStringLocalizer.Setup(s => s[It.Is<string>(msj => msj == mensajeEsperado)])
                .Returns(new LocalizedString(mensajeEsperado, mensajeEsperado));
            var mockIdentityService = new Mock<IIdentityService>
            {
                CallBase = true
            };
            var sut = new Mock<EditAnotacionCommandHandler>(Context,
                mockIStringLocalizer.Object, mockIdentityService.Object)
            {
                CallBase = true
            };
            sut.Setup(s => s.ValidatePropiedadesRequeridas(It.IsAny<EditAnotacionCommand>()))
                .Returns(PropiedadesRequeridas.Ninguno);
            mockIdentityService.Setup(x => x.GetUserIdentityInfo()).Returns(null as IdentityModel);
            var anotacion = new Anotacion
            {
                Id = 1
            };
            await Context.Anotaciones.AddAsync(anotacion);
            var expedienteAlumno = new ExpedienteAlumno
            {
                Id = 1
            };
            await Context.ExpedientesAlumno.AddAsync(expedienteAlumno);
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
            sut.Verify(s => s.ValidatePropiedadesRequeridas(It.IsAny<EditAnotacionCommand>()), Times.Once);
            mockIdentityService.Verify(x => x.GetUserIdentityInfo(), Times.Once);
        }


        [Fact(DisplayName = "Cuando los datos enviados en el request están correctos Realiza el insert en bd")]
        public async Task Handle_Ok()
        {
            //ARRANGE
            var request = new EditAnotacionCommand
            {
                IdExpedienteAlumno = 1,
                Id = 1
            };

            var mockIStringLocalizer = new Mock<IStringLocalizer<EditAnotacionCommandHandler>>
            {
                CallBase = true
            };
            var mockIdentityService = new Mock<IIdentityService>
            {
                CallBase = true
            };
            var identityInfo = new IdentityModel
            {
                Id = Guid.NewGuid().ToString()
            };
            mockIdentityService.Setup(s => s.GetUserIdentityInfo()).Returns(identityInfo);
            var sut = new Mock<EditAnotacionCommandHandler>(Context,
                mockIStringLocalizer.Object, mockIdentityService.Object)
            {
                CallBase = true
            };
            var expedienteAlumno = new ExpedienteAlumno
            {
                Id = 1
            };
            await Context.ExpedientesAlumno.AddAsync(expedienteAlumno);
            await Context.SaveChangesAsync();

            var anotacion = new Anotacion
            {
                Id = 1
            };
            await Context.Anotaciones.AddAsync(anotacion);
            await Context.SaveChangesAsync();

            sut.Setup(s => s.ValidatePropiedadesRequeridas(It.IsAny<EditAnotacionCommand>()))
                .Returns(PropiedadesRequeridas.Ninguno);

            //ACT
            await sut.Object.Handle(request, CancellationToken.None);

            //ASSERT
            sut.Verify(s => s.ValidatePropiedadesRequeridas(It.IsAny<EditAnotacionCommand>()), Times.Once);
            Assert.Equal(1, Context.Anotaciones.Count());
            Assert.Equal(identityInfo.Id, Context.Anotaciones.FirstOrDefault()?.IdRefCuentaSeguridadModificacion);
            Assert.Equal(expedienteAlumno, Context.Anotaciones.FirstOrDefault()?.ExpedienteAlumno);
        }

        [Fact(DisplayName = "Cuando la anotación no existe Devuelve una excepción")]
        public async Task Handle_AnotacionInexistente()
        {
            //ARRANGE
            var request = new EditAnotacionCommand { Id = 1 };

            var mockIStringLocalizer = new Mock<IStringLocalizer<EditAnotacionCommandHandler>>
            {
                CallBase = true
            };
            var mockIdentityService = new Mock<IIdentityService>
            {
                CallBase = true
            };
            var sut = new Mock<EditAnotacionCommandHandler>(Context,
                mockIStringLocalizer.Object, mockIdentityService.Object)
            {
                CallBase = true
            };
            sut.Setup(s => s.ValidatePropiedadesRequeridas(It.IsAny<EditAnotacionCommand>()))
                .Returns(PropiedadesRequeridas.Ninguno);

            //ACT
            var ex = await Record.ExceptionAsync(async () =>
            {
                await sut.Object.Handle(request, CancellationToken.None);
            });

            //ASSERT
            Assert.IsType<NotFoundException>(ex);
            Assert.Contains("not found", ex.Message);
        }

        #endregion

        #region AssignNewAnotacion

        [Fact(DisplayName = "Cuando se asignan los datos enviados en el request Devuelve entidad")]
        public void AssignNewAnotacion_Ok()
        {
            //ARRANGE
            var request = new EditAnotacionCommand
            {
                Resumen = Guid.NewGuid().ToString(),
                Mensaje = Guid.NewGuid().ToString(),
                RolesAnotaciones = new[] { Guid.NewGuid().ToString(), Guid.NewGuid().ToString() },
                EsPublica = false,
                EsRestringida = true
            };

            var mockIStringLocalizer = new Mock<IStringLocalizer<EditAnotacionCommandHandler>>
            {
                CallBase = true
            };
            var mockIdentityService = new Mock<IIdentityService>
            {
                CallBase = true
            };
            var sut = new Mock<EditAnotacionCommandHandler>(Context,
                mockIStringLocalizer.Object, mockIdentityService.Object)
            {
                CallBase = true
            };
            var anotacion = new Anotacion
            {
                RolesAnotaciones = new List<RolAnotacion>
                {
                    new ()
                    {
                        Id = 1,
                        Rol = Guid.NewGuid().ToString()
                    }
                },
                EsPublica = false,
                EsRestringida = true,
                Mensaje = Guid.NewGuid().ToString(),
                Resumen = Guid.NewGuid().ToString()
            };
            sut.Setup(s => s.AddRoles(It.IsAny<Anotacion>(), It.IsAny<string[]>()));

            //ACT
            sut.Object.AssignEditAnotacion(request, anotacion);

            //ASSERT
            Assert.Equal(request.Resumen, anotacion.Resumen);
            Assert.Equal(request.Mensaje, anotacion.Mensaje);
            Assert.Equal(request.EsRestringida, anotacion.EsRestringida);
            Assert.Equal(request.EsPublica, anotacion.EsPublica);
            sut.Verify(s =>
                s.AddRoles(It.IsAny<Anotacion>(), It.IsAny<string[]>()), Times.Once);
        }

        #endregion

        #region ValidatePropiedadesRequeridas

        [Fact(DisplayName = "Cuando las propiedades pasa todas las validaciones Retorna ok")]
        public void ValidatePropiedadesRequeridas_Ok()
        {
            //ARRANGE
            var mockIStringLocalizer = new Mock<IStringLocalizer<EditAnotacionCommandHandler>>
            {
                CallBase = true
            };
            var request = new EditAnotacionCommand
            {
                Resumen = Guid.NewGuid().ToString(),
                Mensaje = Guid.NewGuid().ToString(),
                RolesAnotaciones = new[] { Guid.NewGuid().ToString() },
                EsPublica = false,
                EsRestringida = true
            };
            var mockIdentityService = new Mock<IIdentityService>
            {
                CallBase = true
            };
            var sut = new Mock<EditAnotacionCommandHandler>(Context,
                mockIStringLocalizer.Object, mockIdentityService.Object)
            {
                CallBase = true
            };

            //ACT
            var actual = sut.Object.ValidatePropiedadesRequeridas(request);

            //ASSERT
            Assert.Equal(PropiedadesRequeridas.Ninguno, actual);
        }

        [Fact(DisplayName = "Cuando la propiedad Resumen no tiene valor Retorna PropiedadesRequeridas")]
        public void ValidatePropiedadesRequeridas_Resumen()
        {
            //ARRANGE
            var mockIStringLocalizer = new Mock<IStringLocalizer<EditAnotacionCommandHandler>>
            {
                CallBase = true
            };
            var request = new EditAnotacionCommand
            {
                Resumen = null,
                Mensaje = Guid.NewGuid().ToString(),
                EsPublica = false
            };
            var mockIdentityService = new Mock<IIdentityService>
            {
                CallBase = true
            };
            var sut = new Mock<EditAnotacionCommandHandler>(Context,
                mockIStringLocalizer.Object, mockIdentityService.Object)
            {
                CallBase = true
            };

            //ACT
            var actual = sut.Object.ValidatePropiedadesRequeridas(request);

            //ASSERT
            Assert.Equal(PropiedadesRequeridas.Resumen, actual);
        }

        [Fact(DisplayName = "Cuando la propiedad RolesAnotaciones no tiene valor Retorna PropiedadesRequeridas")]
        public void ValidatePropiedadesRequeridas_ListaRoles()
        {
            //ARRANGE
            var mockIStringLocalizer = new Mock<IStringLocalizer<EditAnotacionCommandHandler>>
            {
                CallBase = true
            };
            var request = new EditAnotacionCommand
            {
                Resumen = Guid.NewGuid().ToString(),
                Mensaje = Guid.NewGuid().ToString(),
                RolesAnotaciones = null,
                EsPublica = false,
                EsRestringida = true
            };
            var mockIdentityService = new Mock<IIdentityService>
            {
                CallBase = true
            };
            var sut = new Mock<EditAnotacionCommandHandler>(Context,
                mockIStringLocalizer.Object, mockIdentityService.Object)
            {
                CallBase = true
            };

            //ACT
            var actual = sut.Object.ValidatePropiedadesRequeridas(request);

            //ASSERT
            Assert.Equal(PropiedadesRequeridas.ListaRoles, actual);
        }

        #endregion

        #region AddRoles

        [Fact(DisplayName = "Cuando no hay roles a guardar Retorna Ok")]
        public void AddRoles_RolesEmpty_Ok()
        {
            //ARRANGE
            var mockIStringLocalizer = new Mock<IStringLocalizer<EditAnotacionCommandHandler>>
            {
                CallBase = true
            };
            var mockIdentityService = new Mock<IIdentityService>
            {
                CallBase = true
            };
            var sut = new Mock<EditAnotacionCommandHandler>(Context,
                mockIStringLocalizer.Object, mockIdentityService.Object)
            {
                CallBase = true
            };
            var mockAnotacion = new Mock<Anotacion> { CallBase = true };
            mockAnotacion.SetupAllProperties();
            mockAnotacion.Object.Id = 1;
            mockAnotacion.Object.RolesAnotaciones = new List<RolAnotacion>
            {
                new()
                {
                    Id = 1,
                    Rol = Guid.NewGuid().ToString()
                }
            };
            mockAnotacion.Setup(a => a.DeleteRolesNoIncluidos(It.IsAny<string[]>()));

            //ACT
            sut.Object.AddRoles(mockAnotacion.Object, null);

            //ASSERT
            mockAnotacion.Verify(a => a.DeleteRolesNoIncluidos(It.IsAny<string[]>()), Times.Once);
        }

        [Fact(DisplayName = "Cuando se envían roles a guardar Retorna Ok")]
        public void AddRoles_Ok()
        {
            //ARRANGE
            var mockIStringLocalizer = new Mock<IStringLocalizer<EditAnotacionCommandHandler>>
            {
                CallBase = true
            };
            var mockIdentityService = new Mock<IIdentityService>
            {
                CallBase = true
            };
            var sut = new Mock<EditAnotacionCommandHandler>(Context,
                mockIStringLocalizer.Object, mockIdentityService.Object)
            {
                CallBase = true
            };
            var mockAnotacion = new Mock<Anotacion> { CallBase = true };
            mockAnotacion.SetupAllProperties();
            mockAnotacion.Object.Id = 1;
            var rolExistene = "Rol A";
            mockAnotacion.Object.RolesAnotaciones = new List<RolAnotacion>
            {
                new()
                {
                    Id = 1,
                    Rol = rolExistene
                }
            };
            mockAnotacion.Setup(a => a.DeleteRolesNoIncluidos(It.IsAny<string[]>()));
            var roles = new[] { Guid.NewGuid().ToString(), rolExistene, Guid.NewGuid().ToString() };

            //ACT
            sut.Object.AddRoles(mockAnotacion.Object, roles);

            //ASSERT
            Assert.Equal(roles.Length, mockAnotacion.Object.RolesAnotaciones.Count);
            mockAnotacion.Verify(a => a.DeleteRolesNoIncluidos(It.IsAny<string[]>()), Times.Once);
        }

        #endregion
    }
}
