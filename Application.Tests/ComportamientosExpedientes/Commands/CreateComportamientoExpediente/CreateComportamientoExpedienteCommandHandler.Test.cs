using MediatR;
using Microsoft.Extensions.Localization;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Unir.Expedientes.Application.ComportamientosExpedientes.Commands.AddNivelUsoComportamientoExpedienteUncommit;
using Unir.Expedientes.Application.ComportamientosExpedientes.Commands.CreateComportamientoExpediente;
using Unir.Expedientes.Application.Tests.Common;
using Unir.Expedientes.Domain.Entities;
using Unir.Framework.ApplicationSuperTypes.Exceptions;
using Xunit;

namespace Unir.Expedientes.Application.Tests.ComportamientosExpedientes.Commands.CreateComportamientoExpediente
{
    [Collection("CommonTestCollection")]
    public class CreateComportamientoExpedienteCommandHandlerTests : TestBase
    {
        #region Handle

        [Fact(DisplayName = "Cuando al validar las propiedades da error Devuelve excepción")]
        public async Task Handle_ValidatePropiedadesRequeridas_BadRequestException()
        {
            //ARRANGE
            var request = new CreateComportamientoExpedienteCommand();
            var mockIStringLocalizer = new Mock<IStringLocalizer<CreateComportamientoExpedienteCommandHandler>>
            {
                CallBase = true
            };
            var mockIMediator = new Mock<IMediator>
            {
                CallBase = true
            };
            var sutMock = new Mock<CreateComportamientoExpedienteCommandHandler>(Context, mockIStringLocalizer.Object,
                mockIMediator.Object) {CallBase = true};

            const string mensajeError = "Mensaje de error";
            sutMock.Setup(x => x.ValidatePropiedadesRequeridas(
                    It.IsAny<CreateComportamientoExpedienteCommand>())).ThrowsAsync(new BadRequestException(mensajeError));

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
                    It.IsAny<CreateComportamientoExpedienteCommand>()), Times.Once());
        }

        [Fact(DisplayName = "Cuando el proceso de creacíón de comportamiento es correcto Devuelve Ok")]
        public async Task Handle_Ok()
        {
            //ARRANGE
            var mockIStringLocalizer = new Mock<IStringLocalizer<CreateComportamientoExpedienteCommandHandler>>
            {
                CallBase = true
            };
            var mockIMediator = new Mock<IMediator>
            {
                CallBase = true
            };
            var sutMock = new Mock<CreateComportamientoExpedienteCommandHandler>(Context, mockIStringLocalizer.Object,
                    mockIMediator.Object)
                { CallBase = true };

            var comportamientoExpediente = new ComportamientoExpediente
            {
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
            sutMock.Setup(x => x.ValidatePropiedadesRequeridas(
                    It.IsAny<CreateComportamientoExpedienteCommand>())).Returns(Task.CompletedTask);
            sutMock.Setup(x => x.AssignNewComportamientoExpediente(
                It.IsAny<CreateComportamientoExpedienteCommand>(), It.IsAny<List<RequisitoExpediente>>(), 
                It.IsAny<List<TipoRequisitoExpediente>>(), It.IsAny<CancellationToken>()))
                .Returns(comportamientoExpediente);

            var requisitoExpediente = new RequisitoExpediente
            {
                Id = 1
            };

            var request = new CreateComportamientoExpedienteCommand
            {
                RequisitosComportamientosExpedientes = new List<RequisitoComportamientoExpedienteDto>
                {
                    new()
                    {
                        RequisitoExpediente = new RequisitoExpedienteDto
                        {
                            Id = 1
                        }
                    }
                }
            };

            await Context.RequisitosExpedientes.AddAsync(requisitoExpediente);
            await Context.SaveChangesAsync();

            //ACT
            var actual = await sutMock.Object.Handle(request, CancellationToken.None);

            //ASSERT
            Assert.True(actual > 0);
            sutMock.Verify(x => x.ValidatePropiedadesRequeridas(
                It.IsAny<CreateComportamientoExpedienteCommand>()), Times.Once());
            sutMock.Verify(x => x.AssignNewComportamientoExpediente(
                It.IsAny<CreateComportamientoExpedienteCommand>(), It.IsAny<List<RequisitoExpediente>>(),
                It.IsAny<List<TipoRequisitoExpediente>>(), It.IsAny<CancellationToken>()), Times.Once());
        }

        #endregion

        #region ValidatePropiedadesRequeridas

        [Fact(DisplayName = "Cuando no se envía nombre Devuelve excepción")]
        public async Task ValidatePropiedadesRequeridas_Nombre_BadRequestException()
        {
            //ARRANGE
            var request = new CreateComportamientoExpedienteCommand();
            var mockIStringLocalizer = new Mock<IStringLocalizer<CreateComportamientoExpedienteCommandHandler>>
            {
                CallBase = true
            };
            var mockIMediator = new Mock<IMediator>
            {
                CallBase = true
            };
            var sutMock = new CreateComportamientoExpedienteCommandHandler(Context, mockIStringLocalizer.Object,
                    mockIMediator.Object);

            const string mensajeEsperado = "El campo Nombre es requerido para crear el Comportamiento.";
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
            var mockIStringLocalizer = new Mock<IStringLocalizer<CreateComportamientoExpedienteCommandHandler>>
            {
                CallBase = true
            };
            var mockIMediator = new Mock<IMediator>
            {
                CallBase = true
            };
            var sutMock = new CreateComportamientoExpedienteCommandHandler(Context, mockIStringLocalizer.Object,
                mockIMediator.Object);

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
            var request = new CreateComportamientoExpedienteCommand
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

        [Fact(DisplayName = "Cuando no se envían requisitos Devuelve excepción")]
        public async Task ValidatePropiedadesRequeridas_Requisitos_Empty_BadRequestException()
        {
            //ARRANGE
            var request = new CreateComportamientoExpedienteCommand
            {
                Nombre = Guid.NewGuid().ToString()
            };
            var mockIStringLocalizer = new Mock<IStringLocalizer<CreateComportamientoExpedienteCommandHandler>>
            {
                CallBase = true
            };
            var mockIMediator = new Mock<IMediator>
            {
                CallBase = true
            };
            var sutMock = new CreateComportamientoExpedienteCommandHandler(Context, mockIStringLocalizer.Object,
                mockIMediator.Object);

            const string mensajeEsperado = "Debe seleccionar como mínimo un Requisito.";
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

        [Fact(DisplayName = "Cuando no se envían niveles de uso Devuelve excepción")]
        public async Task ValidatePropiedadesRequeridas_NivelesUso_Empty_BadRequestException()
        {
            //ARRANGE
            var request = new CreateComportamientoExpedienteCommand
            {
                Nombre = Guid.NewGuid().ToString(),
                RequisitosComportamientosExpedientes = new List<RequisitoComportamientoExpedienteDto>
                {
                    new()
                }
            };
            var mockIStringLocalizer = new Mock<IStringLocalizer<CreateComportamientoExpedienteCommandHandler>>
            {
                CallBase = true
            };
            var mockIMediator = new Mock<IMediator>
            {
                CallBase = true
            };
            var sutMock = new CreateComportamientoExpedienteCommandHandler(Context, mockIStringLocalizer.Object,
                mockIMediator.Object);

            const string mensajeEsperado = "Debe seleccionar como mínimo un Nivel de Uso.";
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

        #endregion

        #region AssignNewComportamientoExpediente

        [Fact(DisplayName = "Cuando se asignan los datos del comportamiento Devuelve un objeto")]
        public void AssignNewComportamientoExpediente_Ok()
        {
            //ARRANGE
            var mockIStringLocalizer = new Mock<IStringLocalizer<CreateComportamientoExpedienteCommandHandler>>
            {
                CallBase = true
            };
            var mockIMediator = new Mock<IMediator>
            {
                CallBase = true
            };

            mockIMediator.Setup(s =>
                    s.Send(It.IsAny<AddNivelUsoComportamientoExpedienteUncommitCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new NivelUsoComportamientoExpediente());

            var sut = new CreateComportamientoExpedienteCommandHandler(Context, mockIStringLocalizer.Object,
                mockIMediator.Object);

            var request = new CreateComportamientoExpedienteCommand
            {
                Nombre = Guid.NewGuid().ToString(),
                Descripcion = Guid.NewGuid().ToString(),
                EstaVigente = true,
                RequisitosComportamientosExpedientes = new List<RequisitoComportamientoExpedienteDto>
                {
                    new()
                    {
                        RequisitoExpediente = new RequisitoExpedienteDto
                        {
                            Id = 1
                        },
                        TipoRequisitoExpediente = new TipoRequisitoExpedienteDto
                        {
                            Id = 1
                        }
                    }
                },
                NivelesUsoComportamientosExpedientes = new List<NivelUsoComportamientoExpedienteDto>
                {
                    new()
                }
            };
            var requisitos = new List<RequisitoExpediente>
            {
                new()
                {
                    Id = 1
                }
            };
            var tiposRequisitos = new List<TipoRequisitoExpediente>
            {
                new()
                {
                    Id = 1
                }
            };

            //ACT
            var actual = sut.AssignNewComportamientoExpediente(request,
                requisitos, tiposRequisitos, CancellationToken.None);

            //ASSERT
            Assert.Equal(request.Nombre, actual.Nombre);
            Assert.Equal(request.Descripcion, actual.Descripcion);
            Assert.Equal(request.EstaVigente, actual.EstaVigente);
            Assert.NotNull(actual.RequisitosComportamientosExpedientes);
            Assert.NotNull(actual.NivelesUsoComportamientosExpedientes);
            mockIMediator.Verify(s => s.Send(
                It.IsAny<AddNivelUsoComportamientoExpedienteUncommitCommand>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        #endregion
    }
}
