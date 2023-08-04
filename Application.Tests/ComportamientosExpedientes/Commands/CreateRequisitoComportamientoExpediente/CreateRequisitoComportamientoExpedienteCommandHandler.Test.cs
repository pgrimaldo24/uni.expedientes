using Microsoft.Extensions.Localization;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Unir.Expedientes.Application.ComportamientosExpedientes.Commands.CreateRequisitoComportamientoExpediente;
using Unir.Expedientes.Application.Tests.Common;
using Unir.Expedientes.Domain.Entities;
using Unir.Framework.ApplicationSuperTypes.Exceptions;
using Xunit;

namespace Unir.Expedientes.Application.Tests.ComportamientosExpedientes.Commands.CreateRequisitoComportamientoExpediente
{
    [Collection("CommonTestCollection")]
    public class CreateRequisitoComportamientoExpedienteCommandHandlerTests : TestBase
    {
        #region Handle

        [Fact(DisplayName = "Cuando el comportamiento no existe Devuelve una excepción")]
        public async Task Handle_NotFoundException()
        {
            //ARRANGE
            var request = new CreateRequisitoComportamientoExpedienteCommand();
            var mockIStringLocalizer = new Mock<IStringLocalizer<CreateRequisitoComportamientoExpedienteCommandHandler>>
            {
                CallBase = true
            };
            var sut = new CreateRequisitoComportamientoExpedienteCommandHandler(Context, mockIStringLocalizer.Object);

            //ACT
            var ex = await Record.ExceptionAsync(async () =>
            {
                await sut.Handle(request, CancellationToken.None);
            });

            //ASSERT
            Assert.NotNull(ex);
            Assert.IsType<NotFoundException>(ex);
        }

        [Fact(DisplayName = "Cuando al validar las propiedades da error Devuelve excepción")]
        public async Task Handle_ValidatePropiedades_BadRequestException()
        {
            //ARRANGE
            var request = new CreateRequisitoComportamientoExpedienteCommand
            {
                IdComportamiento = 1
            };
            var mockIStringLocalizer = new Mock<IStringLocalizer<CreateRequisitoComportamientoExpedienteCommandHandler>>
            {
                CallBase = true
            };
            var sutMock = new Mock<CreateRequisitoComportamientoExpedienteCommandHandler>(Context, 
                mockIStringLocalizer.Object) { CallBase = true };

            const string mensajeError = "Mensaje de error";
            sutMock.Setup(x => x.ValidatePropiedades(
                It.IsAny<CreateRequisitoComportamientoExpedienteCommand>(), It.IsAny<ComportamientoExpediente>()))
                .ThrowsAsync(new BadRequestException(mensajeError));

            var comportamientoExpediente = new ComportamientoExpediente
            {
                Id = 1,
                Nombre = Guid.NewGuid().ToString(),
                Descripcion = Guid.NewGuid().ToString(),
                EstaVigente = true,
                RequisitosComportamientosExpedientes = new List<RequisitoComportamientoExpediente>
                {
                    new(),
                    new()
                },
                NivelesUsoComportamientosExpedientes = new List<NivelUsoComportamientoExpediente>
                {
                    new(),
                    new()
                }
            };
            await Context.ComportamientosExpedientes.AddAsync(comportamientoExpediente);
            await Context.SaveChangesAsync();

            //ACT
            var ex = await Record.ExceptionAsync(async () =>
            {
                await sutMock.Object.Handle(request, CancellationToken.None);
            });

            //ASSERT
            Assert.NotNull(ex);
            Assert.IsType<BadRequestException>(ex);
            Assert.Equal(mensajeError, ex.Message);
            sutMock.Verify(x => x.ValidatePropiedades(
                It.IsAny<CreateRequisitoComportamientoExpedienteCommand>(), It.IsAny<ComportamientoExpediente>()), Times.Once());
        }

        [Fact(DisplayName = "Cuando el proceso es correcto Devuelve Ok")]
        public async Task Handle_oK()
        {
            //ARRANGE
            var request = new CreateRequisitoComportamientoExpedienteCommand
            {
                IdComportamiento = 1
            };
            var mockIStringLocalizer = new Mock<IStringLocalizer<CreateRequisitoComportamientoExpedienteCommandHandler>>
            {
                CallBase = true
            };
            var sutMock = new Mock<CreateRequisitoComportamientoExpedienteCommandHandler>(Context,
                mockIStringLocalizer.Object)
            { CallBase = true };

            sutMock.Setup(x => x.ValidatePropiedades(
                    It.IsAny<CreateRequisitoComportamientoExpedienteCommand>(), It.IsAny<ComportamientoExpediente>()))
                .Returns(Task.CompletedTask);
            sutMock.Setup(x => x.AssignNewRequisitoComportamientoExpediente(
                    It.IsAny<CreateRequisitoComportamientoExpedienteCommand>(), It.IsAny<ComportamientoExpediente>()))
                .ReturnsAsync(new RequisitoComportamientoExpediente());

            var comportamientoExpediente = new ComportamientoExpediente
            {
                Id = 1,
                Nombre = Guid.NewGuid().ToString(),
                Descripcion = Guid.NewGuid().ToString(),
                EstaVigente = true,
                RequisitosComportamientosExpedientes = new List<RequisitoComportamientoExpediente>
                {
                    new(),
                    new()
                },
                NivelesUsoComportamientosExpedientes = new List<NivelUsoComportamientoExpediente>
                {
                    new(),
                    new()
                }
            };
            await Context.ComportamientosExpedientes.AddAsync(comportamientoExpediente);
            await Context.SaveChangesAsync();

            //ACT
            var actual = await sutMock.Object.Handle(request, CancellationToken.None);

            //ASSERT
            Assert.True(actual > 0);
            sutMock.Verify(x => x.ValidatePropiedades(
                It.IsAny<CreateRequisitoComportamientoExpedienteCommand>(), It.IsAny<ComportamientoExpediente>()), Times.Once());
            sutMock.Verify(x => x.AssignNewRequisitoComportamientoExpediente(
                It.IsAny<CreateRequisitoComportamientoExpedienteCommand>(), It.IsAny<ComportamientoExpediente>()), Times.Once());
        }

        #endregion

        #region ValidatePropiedades

        [Fact(DisplayName = "Cuando no existe el requisito Devuelve una excepción")]
        public async Task ValidatePropiedades_Requisito_BadRequestException()
        {
            //ARRANGE
            var request = new CreateRequisitoComportamientoExpedienteCommand
            {
                RequisitoExpediente = new RequisitoExpedienteDto()
            };
            var mockIStringLocalizer = new Mock<IStringLocalizer<CreateRequisitoComportamientoExpedienteCommandHandler>>
            {
                CallBase = true
            };
            var sut = new CreateRequisitoComportamientoExpedienteCommandHandler(Context, mockIStringLocalizer.Object);
            const string mensajeEsperado = "No existe el requisito seleccionado.";
            mockIStringLocalizer.Setup(s => s[It.Is<string>(msj => msj == mensajeEsperado)])
                .Returns(new LocalizedString(mensajeEsperado, mensajeEsperado));

            //ACT
            var ex = await Record.ExceptionAsync(async () =>
            {
                await sut.ValidatePropiedades(request, new ComportamientoExpediente());
            });

            //ASSERT
            Assert.NotNull(ex);
            Assert.IsType<BadRequestException>(ex);
        }

        [Fact(DisplayName = "Cuando el requisito ya se encuentra registrado en el mismo comportamiento Devuelve una excepción")]
        public async Task ValidatePropiedades_RequisitoRepedito_BadRequestException()
        {
            //ARRANGE
            var request = new CreateRequisitoComportamientoExpedienteCommand
            {
                RequisitoExpediente = new RequisitoExpedienteDto
                {
                    Id = 1
                }
            };
            var mockIStringLocalizer = new Mock<IStringLocalizer<CreateRequisitoComportamientoExpedienteCommandHandler>>
            {
                CallBase = true
            };
            var sut = new CreateRequisitoComportamientoExpedienteCommandHandler(Context, mockIStringLocalizer.Object);
            const string mensajeEsperado = "El requisito seleccionado ya se encuentra registrado.";
            mockIStringLocalizer.Setup(s => s[It.Is<string>(msj => msj == mensajeEsperado)])
                .Returns(new LocalizedString(mensajeEsperado, mensajeEsperado));

            await Context.RequisitosExpedientes.AddAsync(new RequisitoExpediente
            {
                Id = 1
            });
            await Context.SaveChangesAsync();

            var comportamiento = new ComportamientoExpediente
            {
                RequisitosComportamientosExpedientes = new List<RequisitoComportamientoExpediente>
                {
                    new()
                    {
                        RequisitoExpediente = new RequisitoExpediente
                        {
                            Id = 1
                        }
                    }
                }
            };

            //ACT
            var ex = await Record.ExceptionAsync(async () =>
            {
                await sut.ValidatePropiedades(request, comportamiento);
            });

            //ASSERT
            Assert.NotNull(ex);
            Assert.IsType<BadRequestException>(ex);
        }

        #endregion

        #region AssignNewRequisitoComportamientoExpediente

        [Fact(DisplayName = "Cuando se asignan los datos Devuelve un objeto")]
        public async Task AssignNewRequisitoComportamientoExpediente_Ok()
        {
            //ARRANGE
            var request = new CreateRequisitoComportamientoExpedienteCommand
            {
                RequisitoExpediente = new RequisitoExpedienteDto
                {
                    Id = 1
                },
                TipoRequisitoExpediente = new TipoRequisitoExpedienteDto
                {
                    Id = 1
                }
            };
            var mockIStringLocalizer = new Mock<IStringLocalizer<CreateRequisitoComportamientoExpedienteCommandHandler>>
            {
                CallBase = true
            };
            var sut = new CreateRequisitoComportamientoExpedienteCommandHandler(Context, mockIStringLocalizer.Object);

            await Context.RequisitosExpedientes.AddAsync(new RequisitoExpediente
            {
                Id = 1
            });
            await Context.TiposRequisitosExpedientes.AddAsync(new TipoRequisitoExpediente
            {
                Id = 1
            });
            await Context.SaveChangesAsync();

            //ACT
            var actual = await sut.AssignNewRequisitoComportamientoExpediente(request, new ComportamientoExpediente());

            //ASSERT
            Assert.NotNull(actual);
            Assert.NotNull(actual.ComportamientoExpediente);
            Assert.NotNull(actual.RequisitoExpediente);
            Assert.NotNull(actual.TipoRequisitoExpediente);
        }

        #endregion
    }
}
