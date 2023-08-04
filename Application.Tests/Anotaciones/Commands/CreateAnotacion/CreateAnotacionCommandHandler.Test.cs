using Microsoft.Extensions.Localization;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Unir.Expedientes.Application.Anotaciones.Commands.CreateAnotacion;
using Unir.Expedientes.Application.Tests.Common;
using Unir.Expedientes.Domain.Entities;
using Unir.Framework.ApplicationSuperTypes.Exceptions;
using Unir.Framework.Crosscutting.Security;
using Unir.Framework.Crosscutting.Security.Model;
using Xunit;

namespace Unir.Expedientes.Application.Tests.Anotaciones.Commands.CreateAnotacion
{
    [Collection("CommonTestCollection")]
    public class CreateAnotacionCommandHandlerTests : TestBase
    {
        #region Handle

        [Fact(DisplayName = "Cuando los datos enviados en el request vienen vacíos Devuelve una excepción")]
        public async Task Handle_RequestInvalid()
        {
            //ARRANGE
            const string mensajeEsperado = "El campo Resumen es requerido para crear la Observación.";
            var request = new CreateAnotacionCommand();

            var mockIStringLocalizer = new Mock<IStringLocalizer<CreateAnotacionCommandHandler>>
            {
                CallBase = true
            };
            mockIStringLocalizer.Setup(s => s[It.Is<string>(msj => msj == mensajeEsperado)])
                .Returns(new LocalizedString(mensajeEsperado, mensajeEsperado));
            var mockIdentityService = new Mock<IIdentityService>
            {
                CallBase = true
            };
            var sut = new Mock<CreateAnotacionCommandHandler>(Context, 
                mockIStringLocalizer.Object, mockIdentityService.Object)
            {
                CallBase = true
            };
            sut.Setup(s => s.ValidatePropiedadesRequeridas(It.IsAny<CreateAnotacionCommand>()))
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
            sut.Verify(s => s.ValidatePropiedadesRequeridas(It.IsAny<CreateAnotacionCommand>()), Times.Once);
        }

        [Fact(DisplayName = "Cuando no existe el expediente Devuelve una excepción")]
        public async Task Handle_NotFoundException()
        {
            //ARRANGE
            var request = new CreateAnotacionCommand();

            var mockIStringLocalizer = new Mock<IStringLocalizer<CreateAnotacionCommandHandler>>
            {
                CallBase = true
            };
            var mockIdentityService = new Mock<IIdentityService>
            {
                CallBase = true
            };
            var sut = new Mock<CreateAnotacionCommandHandler>(Context,
                mockIStringLocalizer.Object, mockIdentityService.Object)
            {
                CallBase = true
            };
            sut.Setup(s => s.ValidatePropiedadesRequeridas(It.IsAny<CreateAnotacionCommand>()))
                .Returns(PropiedadesRequeridas.Ninguno);

            //ACT
            var ex = (NotFoundException)await Record.ExceptionAsync(async () =>
            {
                await sut.Object.Handle(request, CancellationToken.None);
            });

            //ASSERT
            Assert.NotNull(ex);
            Assert.IsType<NotFoundException>(ex);
            sut.Verify(s => s.ValidatePropiedadesRequeridas(It.IsAny<CreateAnotacionCommand>()), Times.Once);
        }

        [Fact(DisplayName = "Cuando no existe el usuario Devuelve una excepción")]
        public async Task Handle_User_BadRequestException()
        {
            //ARRANGE
            const string mensajeEsperado = "Usuario no encontrado.";
            var request = new CreateAnotacionCommand
            {
                IdExpedienteAlumno = 1
            };

            var mockIStringLocalizer = new Mock<IStringLocalizer<CreateAnotacionCommandHandler>>
            {
                CallBase = true
            };
            mockIStringLocalizer.Setup(s => s[It.Is<string>(msj => msj == mensajeEsperado)])
                .Returns(new LocalizedString(mensajeEsperado, mensajeEsperado));
            var mockIdentityService = new Mock<IIdentityService>
            {
                CallBase = true
            };
            var sut = new Mock<CreateAnotacionCommandHandler>(Context,
                mockIStringLocalizer.Object, mockIdentityService.Object)
            {
                CallBase = true
            };
            sut.Setup(s => s.ValidatePropiedadesRequeridas(It.IsAny<CreateAnotacionCommand>()))
                .Returns(PropiedadesRequeridas.Ninguno);
            mockIdentityService.Setup(x => x.GetUserIdentityInfo()).Returns(null as IdentityModel);

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
            sut.Verify(s => s.ValidatePropiedadesRequeridas(It.IsAny<CreateAnotacionCommand>()), Times.Once);
            mockIdentityService.Verify(x => x.GetUserIdentityInfo(), Times.Once);
        }

        [Fact(DisplayName = "Cuando los datos enviados en el request están correctos Realiza el insert en bd")]
        public async Task Handle_Ok()
        {
            //ARRANGE
            var request = new CreateAnotacionCommand
            {
                IdExpedienteAlumno = 1
            };

            var mockIStringLocalizer = new Mock<IStringLocalizer<CreateAnotacionCommandHandler>>
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
            var sut = new Mock<CreateAnotacionCommandHandler>(Context,
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
            sut.Setup(s => s.ValidatePropiedadesRequeridas(It.IsAny<CreateAnotacionCommand>()))
                .Returns(PropiedadesRequeridas.Ninguno);
            var anotacion = new Anotacion
            {
                Resumen = Guid.NewGuid().ToString(),
                Mensaje = Guid.NewGuid().ToString(),
                RolesAnotaciones = new List<RolAnotacion>
                {
                    new ()
                    {
                        Rol = Guid.NewGuid().ToString()
                    },
                    new ()
                    {
                        Rol = Guid.NewGuid().ToString()
                    }
                },
                EsPublica = false,
                EsRestringida = true,
                Fecha = DateTime.Now,
                FechaModificacion = DateTime.Now,
                IdRefCuentaSeguridad = Guid.NewGuid().ToString(),
                IdRefCuentaSeguridadModificacion = Guid.NewGuid().ToString()
            };
            sut.Setup(s => s.AssignNewAnotacion(It.IsAny<CreateAnotacionCommand>())).Returns(anotacion);

            //ACT
            await sut.Object.Handle(request, CancellationToken.None);

            //ASSERT
            sut.Verify(s => s.ValidatePropiedadesRequeridas(It.IsAny<CreateAnotacionCommand>()), Times.Once);
            sut.Verify(s => s.AssignNewAnotacion(It.IsAny<CreateAnotacionCommand>()), Times.Once);
            mockIdentityService.Verify(x => x.GetUserIdentityInfo(), Times.Once);
            Assert.Equal(1, Context.Anotaciones.Count());
            Assert.Equal(identityInfo.Id, Context.Anotaciones.FirstOrDefault()?.IdRefCuentaSeguridad);
            Assert.Equal(identityInfo.Id, Context.Anotaciones.FirstOrDefault()?.IdRefCuentaSeguridadModificacion);
            Assert.Equal(expedienteAlumno, Context.Anotaciones.FirstOrDefault()?.ExpedienteAlumno);
            Assert.Equal(anotacion.RolesAnotaciones.Count, Context.Anotaciones.First().RolesAnotaciones.Count);
        }

        #endregion

        #region AssignNewAnotacion

        [Fact(DisplayName = "Cuando se asignan los datos enviados en el request Devuelve entidad")]
        public async Task AssignNewAnotacion_Ok()
        {
            //ARRANGE
            var request = new CreateAnotacionCommand
            {
                Resumen = Guid.NewGuid().ToString(),
                Mensaje = Guid.NewGuid().ToString(),
                RolesAnotaciones = new[] { Guid.NewGuid().ToString() },
                EsPublica = false,
                EsRestringida = true
            };

            var mockIStringLocalizer = new Mock<IStringLocalizer<CreateAnotacionCommandHandler>>
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
            var sut = new Mock<CreateAnotacionCommandHandler>(Context,
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
            sut.Setup(s => s.ValidatePropiedadesRequeridas(It.IsAny<CreateAnotacionCommand>()))
                .Returns(PropiedadesRequeridas.Ninguno);

            //ACT
            var actual = sut.Object.AssignNewAnotacion(request);

            //ASSERT
            Assert.IsType<Anotacion>(actual);
            Assert.Equal(request.RolesAnotaciones.Length, actual.RolesAnotaciones.Count);
            Assert.Equal(request.Resumen, actual.Resumen);
            Assert.Equal(request.Mensaje, actual.Mensaje);
            Assert.Equal(request.EsRestringida, actual.EsRestringida);
            Assert.Equal(request.EsPublica, actual.EsPublica);
        }

        #endregion

        #region ValidatePropiedadesRequeridas

        [Fact(DisplayName = "Cuando las propiedades pasa todas las validaciones Retorna ok")]
        public void ValidatePropiedadesRequeridas_Ok()
        {
            //ARRANGE
            var mockIStringLocalizer = new Mock<IStringLocalizer<CreateAnotacionCommandHandler>>
            {
                CallBase = true
            };
            var request = new CreateAnotacionCommand
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
            var sut = new Mock<CreateAnotacionCommandHandler>(Context,
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
            var mockIStringLocalizer = new Mock<IStringLocalizer<CreateAnotacionCommandHandler>>
            {
                CallBase = true
            };
            var request = new CreateAnotacionCommand
            {
                Resumen = null,
                Mensaje = Guid.NewGuid().ToString(),
                EsPublica = false
            };
            var mockIdentityService = new Mock<IIdentityService>
            {
                CallBase = true
            };
            var sut = new Mock<CreateAnotacionCommandHandler>(Context,
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
            var mockIStringLocalizer = new Mock<IStringLocalizer<CreateAnotacionCommandHandler>>
            {
                CallBase = true
            };
            var request = new CreateAnotacionCommand
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
            var sut = new Mock<CreateAnotacionCommandHandler>(Context,
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
    }
}
