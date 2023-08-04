using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Localization;
using Moq;
using Unir.Expedientes.Application.ComportamientosExpedientes.Commands.DeleteRequisitoComportamientoExpediente;
using Unir.Expedientes.Application.Tests.Common;
using Unir.Expedientes.Domain.Entities;
using Unir.Framework.ApplicationSuperTypes.Exceptions;
using Xunit;

namespace Unir.Expedientes.Application.Tests.ComportamientosExpedientes.Commands.DeleteRequisitoComportamientoExpediente
{
    [Collection("CommonTestCollection")]
    public class DeleteRequisitoComportamientoExpedienteCommandHandlerTests : TestBase
    {
        #region Handle

        [Fact(DisplayName = "Cuando el requisito comportamiento no existe Devuelve una excepción")]
        public async Task Handle_NotFoundException()
        {
            //ARRANGE
            var request = new DeleteRequisitoComportamientoExpedienteCommand(1);
            var mockIStringLocalizer = new Mock<IStringLocalizer<DeleteRequisitoComportamientoExpedienteCommandHandler>>
            {
                CallBase = true
            };
            var sut = new DeleteRequisitoComportamientoExpedienteCommandHandler(Context, mockIStringLocalizer.Object);

            //ACT
            var ex = await Record.ExceptionAsync(async () =>
            {
                await sut.Handle(request, CancellationToken.None);
            });

            //ASSERT
            Assert.NotNull(ex);
            Assert.IsType<NotFoundException>(ex);
        }

        [Fact(DisplayName = "Cuando se elimina el requisito y solo existe un registro Devuelve excepción")]
        public async Task Handle_BadRequestException()
        {
            //ARRANGE
            var request = new DeleteRequisitoComportamientoExpedienteCommand(1);
            var mockIStringLocalizer = new Mock<IStringLocalizer<DeleteRequisitoComportamientoExpedienteCommandHandler>>
            {
                CallBase = true
            };
            var sut = new DeleteRequisitoComportamientoExpedienteCommandHandler(Context, mockIStringLocalizer.Object);
            const string mensajeEsperado = "Debe existir como mínimo un Requisito.";
            mockIStringLocalizer.Setup(s => s[It.Is<string>(msj => msj == mensajeEsperado)])
                .Returns(new LocalizedString(mensajeEsperado, mensajeEsperado));

            var requisitoComportamiento = new RequisitoComportamientoExpediente
            {
                Id = 1,
                ComportamientoExpediente = new ComportamientoExpediente
                {
                    RequisitosComportamientosExpedientes = new List<RequisitoComportamientoExpediente>()
                }
            };
            await Context.RequisitosComportamientosExpedientes.AddAsync(requisitoComportamiento);
            await Context.SaveChangesAsync();

            //ACT
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

        [Fact(DisplayName = "Cuando se elimina el requisito comportamiento Devuelve Ok")]
        public async Task Handle_Ok()
        {
            //ARRANGE
            var request = new DeleteRequisitoComportamientoExpedienteCommand(1);
            var mockIStringLocalizer = new Mock<IStringLocalizer<DeleteRequisitoComportamientoExpedienteCommandHandler>>
            {
                CallBase = true
            };
            var sut = new DeleteRequisitoComportamientoExpedienteCommandHandler(Context, mockIStringLocalizer.Object);

            var requisitoComportamiento = new RequisitoComportamientoExpediente
            {
                Id = 1,
                ComportamientoExpediente = new ComportamientoExpediente
                {
                    RequisitosComportamientosExpedientes = new List<RequisitoComportamientoExpediente>
                    {
                        new()
                        {
                            Id = 2,
                            RequisitoExpediente = new RequisitoExpediente
                            {
                                Id = 2
                            }
                        }
                    }
                }
            };
            await Context.RequisitosComportamientosExpedientes.AddAsync(requisitoComportamiento);
            await Context.SaveChangesAsync();

            //ACT
            await sut.Handle(request, CancellationToken.None);

            //ASSERT
            Assert.Null(await Context.RequisitosComportamientosExpedientes.FirstOrDefaultAsync(r => r.Id == 1));
        }

        #endregion
    }
}
