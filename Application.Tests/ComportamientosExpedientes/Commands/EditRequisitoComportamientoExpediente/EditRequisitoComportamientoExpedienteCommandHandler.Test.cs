using Microsoft.Extensions.Localization;
using Moq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Unir.Expedientes.Application.ComportamientosExpedientes.Commands.EditRequisitoComportamientoExpediente;
using Unir.Expedientes.Application.Tests.Common;
using Unir.Expedientes.Domain.Entities;
using Unir.Framework.ApplicationSuperTypes.Exceptions;
using Xunit;

namespace Unir.Expedientes.Application.Tests.ComportamientosExpedientes.Commands.EditRequisitoComportamientoExpediente
{
    [Collection("CommonTestCollection")]
    public class EditRequisitoComportamientoExpedienteCommandHandlerTests : TestBase
    {
        #region Handle

        [Fact(DisplayName = "Cuando el requisito comportamiento no existe Devuelve una excepción")]
        public async Task Handle_NotFoundException()
        {
            //ARRANGE
            var request = new EditRequisitoComportamientoExpedienteCommand();
            var mockIStringLocalizer = new Mock<IStringLocalizer<EditRequisitoComportamientoExpedienteCommandHandler>>
            {
                CallBase = true
            };
            var sut = new EditRequisitoComportamientoExpedienteCommandHandler(Context, mockIStringLocalizer.Object);

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
            var request = new EditRequisitoComportamientoExpedienteCommand
            {
                IdRequisitoComportamiento = 1
            };
            var mockIStringLocalizer = new Mock<IStringLocalizer<EditRequisitoComportamientoExpedienteCommandHandler>>
            {
                CallBase = true
            };
            var sutMock = new Mock<EditRequisitoComportamientoExpedienteCommandHandler>(Context,
                mockIStringLocalizer.Object)
            { CallBase = true };

            const string mensajeError = "Mensaje de error";
            sutMock.Setup(x => x.ValidatePropiedades(
                It.IsAny<EditRequisitoComportamientoExpedienteCommand>(), It.IsAny<ComportamientoExpediente>()))
                .ThrowsAsync(new BadRequestException(mensajeError));

            await Context.RequisitosComportamientosExpedientes.AddAsync(new RequisitoComportamientoExpediente
            {
                Id = 1,
                ComportamientoExpediente = new ComportamientoExpediente
                {
                    RequisitosComportamientosExpedientes = new List<RequisitoComportamientoExpediente>()
                },
                RequisitoExpediente = new RequisitoExpediente(),
                TipoRequisitoExpediente = new TipoRequisitoExpediente()
            });
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
                It.IsAny<EditRequisitoComportamientoExpedienteCommand>(), It.IsAny<ComportamientoExpediente>()), Times.Once());
        }

        [Fact(DisplayName = "Cuando el proceso es correcto Devuelve Ok")]
        public async Task Handle_Ok()
        {
            //ARRANGE
            var request = new EditRequisitoComportamientoExpedienteCommand
            {
                IdRequisitoComportamiento = 1
            };
            var mockIStringLocalizer = new Mock<IStringLocalizer<EditRequisitoComportamientoExpedienteCommandHandler>>
            {
                CallBase = true
            };
            var sutMock = new Mock<EditRequisitoComportamientoExpedienteCommandHandler>(Context,
                mockIStringLocalizer.Object)
            { CallBase = true };
            
            sutMock.Setup(x => x.ValidatePropiedades(
                It.IsAny<EditRequisitoComportamientoExpedienteCommand>(), It.IsAny<ComportamientoExpediente>()))
                .Returns(Task.CompletedTask);
            sutMock.Setup(x => x.AssignRequisitoComportamientoExpediente(
                It.IsAny<EditRequisitoComportamientoExpedienteCommand>(),
                It.IsAny<RequisitoComportamientoExpediente>())).Returns(Task.CompletedTask);

            await Context.RequisitosComportamientosExpedientes.AddAsync(new RequisitoComportamientoExpediente
            {
                Id = 1,
                ComportamientoExpediente = new ComportamientoExpediente
                {
                    RequisitosComportamientosExpedientes = new List<RequisitoComportamientoExpediente>()
                },
                RequisitoExpediente = new RequisitoExpediente(),
                TipoRequisitoExpediente = new TipoRequisitoExpediente()
            });
            await Context.SaveChangesAsync();

            //ACT
            await sutMock.Object.Handle(request, CancellationToken.None);

            //ASSERT
            sutMock.Verify(x => x.ValidatePropiedades(
                It.IsAny<EditRequisitoComportamientoExpedienteCommand>(), 
                It.IsAny<ComportamientoExpediente>()), Times.Once());
            sutMock.Verify(x => x.AssignRequisitoComportamientoExpediente(
                It.IsAny<EditRequisitoComportamientoExpedienteCommand>(),
                It.IsAny<RequisitoComportamientoExpediente>()), Times.Once());
        }

        #endregion

        #region ValidatePropiedades

        [Fact(DisplayName = "Cuando no existe el requisito Devuelve una excepción")]
        public async Task ValidatePropiedades_Requisito_BadRequestException()
        {
            //ARRANGE
            var request = new EditRequisitoComportamientoExpedienteCommand
            {
                RequisitoExpediente = new RequisitoExpedienteDto()
            };
            var mockIStringLocalizer = new Mock<IStringLocalizer<EditRequisitoComportamientoExpedienteCommandHandler>>
            {
                CallBase = true
            };
            var sut = new EditRequisitoComportamientoExpedienteCommandHandler(Context, mockIStringLocalizer.Object);
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

        [Fact(DisplayName = "Cuando el requisito ya se encuentra registrado en el mismo requisito comportamiento Devuelve una excepción")]
        public async Task ValidatePropiedades_RequisitoRepedito_BadRequestException()
        {
            //ARRANGE
            var request = new EditRequisitoComportamientoExpedienteCommand
            {
                IdRequisitoComportamiento = 1,
                RequisitoExpediente = new RequisitoExpedienteDto
                {
                    Id = 1
                }
            };
            var mockIStringLocalizer = new Mock<IStringLocalizer<EditRequisitoComportamientoExpedienteCommandHandler>>
            {
                CallBase = true
            };
            var sut = new EditRequisitoComportamientoExpedienteCommandHandler(Context, mockIStringLocalizer.Object);
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

        #region AssignRequisitoComportamientoExpediente

        [Fact(DisplayName = "Cuando se asignan los datos Devuelve un objeto")]
        public async Task AssignRequisitoComportamientoExpediente_Ok()
        {
            //ARRANGE
            var request = new EditRequisitoComportamientoExpedienteCommand
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
            var mockIStringLocalizer = new Mock<IStringLocalizer<EditRequisitoComportamientoExpedienteCommandHandler>>
            {
                CallBase = true
            };
            var sut = new EditRequisitoComportamientoExpedienteCommandHandler(Context, mockIStringLocalizer.Object);

            await Context.RequisitosExpedientes.AddAsync(new RequisitoExpediente
            {
                Id = 1
            });
            await Context.TiposRequisitosExpedientes.AddAsync(new TipoRequisitoExpediente
            {
                Id = 1
            });
            await Context.SaveChangesAsync();
            var requisitoComportamiento = new RequisitoComportamientoExpediente();

            //ACT
            await sut.AssignRequisitoComportamientoExpediente(request, requisitoComportamiento);

            //ASSERT
            Assert.NotNull(requisitoComportamiento);
            Assert.NotNull(requisitoComportamiento.RequisitoExpediente);
            Assert.NotNull(requisitoComportamiento.TipoRequisitoExpediente);
        }

        #endregion
    }
}
