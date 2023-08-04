using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Localization;
using Moq;
using Unir.Expedientes.Application.ComportamientosExpedientes.Commands.DeleteNivelUsoComportamientoExpediente;
using Unir.Expedientes.Application.Tests.Common;
using Unir.Expedientes.Domain.Entities;
using Unir.Framework.ApplicationSuperTypes.Exceptions;
using Xunit;

namespace Unir.Expedientes.Application.Tests.ComportamientosExpedientes.Commands.DeleteNivelUsoComportamientoExpediente
{
    [Collection("CommonTestCollection")]
    public class DeleteNivelUsoComportamientoExpedienteCommandHandlerTests : TestBase
    {
        #region Handle

        [Fact(DisplayName = "Cuando el nivel de uso comportamiento no existe Devuelve una excepción")]
        public async Task Handle_NotFoundException()
        {
            //ARRANGE
            var request = new DeleteNivelUsoComportamientoExpedienteCommand(1);
            var mockIStringLocalizer = new Mock<IStringLocalizer<DeleteNivelUsoComportamientoExpedienteCommandHandler>>
            {
                CallBase = true
            };
            var sut = new DeleteNivelUsoComportamientoExpedienteCommandHandler(Context, mockIStringLocalizer.Object);

            //ACT
            var ex = await Record.ExceptionAsync(async () =>
            {
                await sut.Handle(request, CancellationToken.None);
            });

            //ASSERT
            Assert.NotNull(ex);
            Assert.IsType<NotFoundException>(ex);
        }

        [Fact(DisplayName = "Cuando se elimina el nivel de uso y solo existe un registro Devuelve excepción")]
        public async Task Handle_BadRequestException()
        {
            //ARRANGE
            var request = new DeleteNivelUsoComportamientoExpedienteCommand(1);
            var mockIStringLocalizer = new Mock<IStringLocalizer<DeleteNivelUsoComportamientoExpedienteCommandHandler>>
            {
                CallBase = true
            };
            var sut = new DeleteNivelUsoComportamientoExpedienteCommandHandler(Context, mockIStringLocalizer.Object);
            const string mensajeEsperado = "Debe existir como mínimo un Nivel de Uso.";
            mockIStringLocalizer.Setup(s => s[It.Is<string>(msj => msj == mensajeEsperado)])
                .Returns(new LocalizedString(mensajeEsperado, mensajeEsperado));

            var nivelUsoComportamiento = new NivelUsoComportamientoExpediente
            {
                Id = 1,
                ComportamientoExpediente = new ComportamientoExpediente
                {
                    RequisitosComportamientosExpedientes = new List<RequisitoComportamientoExpediente>()
                }
            };
            await Context.NivelesUsoComportamientosExpedientes.AddAsync(nivelUsoComportamiento);
            await Context.SaveChangesAsync();

            //ACT
            var ex = await Record.ExceptionAsync(async () =>
            {
                await sut.Handle(request, CancellationToken.None);
            });

            //ASSERT
            Assert.NotNull(ex);
            Assert.IsType<BadRequestException>(ex);
            Assert.Equal(mensajeEsperado, ex.Message);
        }

        [Fact(DisplayName = "Cuando se elimina el nivel de uso comportamiento Devuelve Ok")]
        public async Task Handle_Ok()
        {
            //ARRANGE
            var request = new DeleteNivelUsoComportamientoExpedienteCommand(1);
            var mockIStringLocalizer = new Mock<IStringLocalizer<DeleteNivelUsoComportamientoExpedienteCommandHandler>>
            {
                CallBase = true
            };
            var sut = new DeleteNivelUsoComportamientoExpedienteCommandHandler(Context, mockIStringLocalizer.Object);

            var nivelUsoComportamiento = new NivelUsoComportamientoExpediente
            {
                Id = 1,
                ComportamientoExpediente = new ComportamientoExpediente
                {
                    NivelesUsoComportamientosExpedientes = new List<NivelUsoComportamientoExpediente>
                    {
                        new()
                        {
                            Id = 2
                        }
                    }
                }
            };
            await Context.NivelesUsoComportamientosExpedientes.AddAsync(nivelUsoComportamiento);
            await Context.SaveChangesAsync();

            //ACT
            await sut.Handle(request, CancellationToken.None);

            //ASSERT
            Assert.Null(await Context.NivelesUsoComportamientosExpedientes.FirstOrDefaultAsync(r => r.Id == 1));
        }

        #endregion
    }
}
