using Microsoft.Extensions.Localization;
using Moq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Unir.Expedientes.Application.ComportamientosExpedientes.Commands.DeleteComportamientoExpediente;
using Unir.Expedientes.Application.Tests.Common;
using Unir.Expedientes.Domain.Entities;
using Unir.Framework.ApplicationSuperTypes.Exceptions;
using Xunit;

namespace Unir.Expedientes.Application.Tests.ComportamientosExpedientes.Commands.DeleteComportamientoExpediente
{
    [Collection("CommonTestCollection")]
    public class DeleteComportamientoExpedienteCommandHandlerTests : TestBase
    {
        #region Handle

        [Fact(DisplayName = "Cuando el comportamiento no existe Devuelve una excepción")]
        public async Task Handle_NotFoundException()
        {
            //ARRANGE
            var request = new DeleteComportamientoExpedienteCommand(1);
            var mockIStringLocalizer = new Mock<IStringLocalizer<DeleteComportamientoExpedienteCommandHandler>>
            {
                CallBase = true
            };
            var sut = new DeleteComportamientoExpedienteCommandHandler(Context, mockIStringLocalizer.Object);

            //ACT
            var ex = await Record.ExceptionAsync(async () =>
            {
                await sut.Handle(request, CancellationToken.None);
            });

            //ASSERT
            Assert.NotNull(ex);
            Assert.IsType<NotFoundException>(ex);
        }

        [Fact(DisplayName = "Cuando el comportamiento tiene requisitos que tienen consolidaciones en los expedientes Devuelve una excepción")]
        public async Task Handle_consolidaciones_BadRequestException()
        {
            //ARRANGE
            var request = new DeleteComportamientoExpedienteCommand(1);
            var mockIStringLocalizer = new Mock<IStringLocalizer<DeleteComportamientoExpedienteCommandHandler>>
            {
                CallBase = true
            };
            const string mensajeEsperado = "Alguno de los requisitos asociados tienen consolidaciones en los expedientes.";
            mockIStringLocalizer.Setup(s => s[It.Is<string>(msj => msj == mensajeEsperado)])
                .Returns(new LocalizedString(mensajeEsperado, mensajeEsperado));
            var sut = new DeleteComportamientoExpedienteCommandHandler(Context, mockIStringLocalizer.Object);

            var comportamiento = new ComportamientoExpediente
            {
                RequisitosComportamientosExpedientes = new List<RequisitoComportamientoExpediente>
                {
                    new()
                    {
                        Id = 1,
                        RequisitoExpediente = new RequisitoExpediente
                        {
                            Id = 1,
                            ConsolidacionesRequisitosExpedientes = new List<ConsolidacionRequisitoExpediente>
                            {
                                new()
                                {
                                    Id = 1
                                }
                            }
                        }
                    }
                }
            };
            await Context.ComportamientosExpedientes.AddAsync(comportamiento);
            await Context.SaveChangesAsync();

            //ACT
            var ex = (BadRequestException)await Record.ExceptionAsync(async () =>
            {
                await sut.Handle(request, CancellationToken.None);
            });

            //ASSERT
            Assert.NotNull(ex);
            Assert.IsType<BadRequestException>(ex);
            Assert.Equal(mensajeEsperado, ex.Message);
            mockIStringLocalizer.Verify(s => s[It.Is<string>(msj => msj == mensajeEsperado)], Times.Once);
        }

        [Fact(DisplayName = "Cuando se elimina el comportamiento Devuelve Ok")]
        public async Task Handle_Ok()
        {
            //ARRANGE
            var request = new DeleteComportamientoExpedienteCommand(1);
            var mockIStringLocalizer = new Mock<IStringLocalizer<DeleteComportamientoExpedienteCommandHandler>>
            {
                CallBase = true
            };
            var sut = new DeleteComportamientoExpedienteCommandHandler(Context, mockIStringLocalizer.Object);

            var comportamiento = new ComportamientoExpediente
            {
                Id = 1
            };
            await Context.ComportamientosExpedientes.AddAsync(comportamiento);
            await Context.SaveChangesAsync();

            //ACT
            await sut.Handle(request, CancellationToken.None);

            //ASSERT
            Assert.Null(await Context.RequisitosExpedientes.FirstOrDefaultAsync(r => r.Id == 1));
        }

        #endregion
    }
}
