using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Moq;
using System.Threading;
using System.Threading.Tasks;
using Unir.Expedientes.Application.ConsolidacionesRequisitosExpedientes.Commands.DeleteConsolidacionRequisitoExpediente;
using Unir.Expedientes.Application.Tests.Common;
using Unir.Expedientes.Domain.Entities;
using Unir.Framework.ApplicationSuperTypes.Exceptions;
using Xunit;

namespace Unir.Expedientes.Application.Tests.ConsolidacionesRequisitosExpedientes.Commands.DeleteConsolidacionRequisitoExpediente
{
    [Collection("CommonTestCollection")]
    public class DeleteConsolidacionRequisitoExpedienteCommandHandlerTests : TestBase
    {
        #region Handle

        [Fact(DisplayName = "Cuando no existe la consolidación requisito expediente Devuelve una excepción")]
        public async Task Handle_NotFoundException()
        {
            //ARRANGE
            var request = new DeleteConsolidacionRequisitoExpedienteCommand(1);
            var sut = new DeleteConsolidacionRequisitoExpedienteCommandHandler(Context, null);

            //ACT
            var ex = await Record.ExceptionAsync(async () =>
            {
                await sut.Handle(request, CancellationToken.None);
            });

            //ASSERT
            Assert.NotNull(ex);
            Assert.IsType<NotFoundException>(ex);
        }

        [Fact(DisplayName = "Cuando el estado de la consolidación es validado Devuelve una excepción")]
        public async Task Handle_BadRequestException()
        {
            //ARRANGE
            var request = new DeleteConsolidacionRequisitoExpedienteCommand(1);
            await Context.ConsolidacionesRequisitosExpedientes.AddAsync(new ConsolidacionRequisitoExpediente
            {
                Id = 1,
                EstadoRequisitoExpediente = new EstadoRequisitoExpediente
                {
                    Id = EstadoRequisitoExpediente.Validado
                },
                RequisitoExpediente = new RequisitoExpediente
                {
                    Id = 1
                }
            });
            await Context.SaveChangesAsync();
            var mockIStringLocalizer = new Mock<IStringLocalizer<DeleteConsolidacionRequisitoExpedienteCommandHandler>>
            {
                CallBase = true
            };
            var sutMock = new Mock<DeleteConsolidacionRequisitoExpedienteCommandHandler>(Context,
                mockIStringLocalizer.Object) { CallBase = true };
            const string mensajeEsperado = "La consolidación no debe estar en estado Validado.";
            mockIStringLocalizer.Setup(s => s[It.Is<string>(msj => msj == mensajeEsperado)])
                .Returns(new LocalizedString(mensajeEsperado, mensajeEsperado));

            //ACT
            var ex = await Record.ExceptionAsync(async () =>
            {
                await sutMock.Object.Handle(request, CancellationToken.None);
            });

            //ASSERT
            Assert.NotNull(ex);
            Assert.IsType<BadRequestException>(ex);
            Assert.Equal(mensajeEsperado, ex.Message);
        }

        [Fact(DisplayName = "Cuando el proceso es correcto Devuelve Ok")]
        public async Task Handle_Ok()
        {
            //ARRANGE
            var request = new DeleteConsolidacionRequisitoExpedienteCommand(1);
            await Context.ConsolidacionesRequisitosExpedientes.AddAsync(new ConsolidacionRequisitoExpediente
            {
                Id = 1,
                EstadoRequisitoExpediente = new EstadoRequisitoExpediente
                {
                    Id = EstadoRequisitoExpediente.Pendiente
                },
                RequisitoExpediente = new RequisitoExpediente
                {
                    Id = 1,
                    Bloqueado = true
                }
            });
            await Context.SaveChangesAsync();
            var mockIStringLocalizer = new Mock<IStringLocalizer<DeleteConsolidacionRequisitoExpedienteCommandHandler>>
            {
                CallBase = true
            };
            var sutMock = new Mock<DeleteConsolidacionRequisitoExpedienteCommandHandler>(Context,
                    mockIStringLocalizer.Object)
                { CallBase = true };

            //ACT
            await sutMock.Object.Handle(request, CancellationToken.None);

            //ASSERT
            Assert.False(await Context.ConsolidacionesRequisitosExpedientes.AnyAsync());
        }

        #endregion
    }
}
