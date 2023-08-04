using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Unir.Expedientes.Application.ComportamientosExpedientes.Commands.DeleteRequisitosComportamientosExpedientesMasivo;
using Unir.Expedientes.Application.Tests.Common;
using Unir.Expedientes.Domain.Entities;
using Unir.Framework.ApplicationSuperTypes.Exceptions;
using Xunit;

namespace Unir.Expedientes.Application.Tests.ComportamientosExpedientes.Commands.DeleteRequisitosComportamientosExpedientesMasivo
{
    [Collection("CommonTestCollection")]
    public class DeleteRequisitosComportamientosExpedientesMasivoCommandHandlerTests : TestBase
    {
        #region Handle

        [Fact(DisplayName = "Cuando los requisitos comportamientos no existen Devuelve una excepción")]
        public async Task Handle_BadRequestException()
        {
            //ARRANGE
            var request = new DeleteRequisitosComportamientosExpedientesMasivoCommand();
            var mockIStringLocalizer = new Mock<IStringLocalizer<DeleteRequisitosComportamientosExpedientesMasivoCommandHandler>>
            {
                CallBase = true
            };
            const string mensajeEsperado = "Los requisitos comportamientos seleccionados no existen.";
            mockIStringLocalizer.Setup(s => s[It.Is<string>(msj => msj == mensajeEsperado)])
                .Returns(new LocalizedString(mensajeEsperado, mensajeEsperado));
            var sut = new DeleteRequisitosComportamientosExpedientesMasivoCommandHandler(Context, mockIStringLocalizer.Object);

            //ACT
            var ex = await Record.ExceptionAsync(async () =>
            {
                await sut.Handle(request, CancellationToken.None);
            });

            //ASSERT
            Assert.NotNull(ex);
            Assert.IsType<BadRequestException>(ex);
            Assert.Equal(mensajeEsperado, ex.Message);
            mockIStringLocalizer.Verify(s => s[It.Is<string>(msj => msj == mensajeEsperado)], Times.Once);
        }

        [Fact(DisplayName = "Cuando se eliminan todos los requisitos Devuelve excepción")]
        public async Task Handle_BadRequestException_EliminarTodos()
        {
            //ARRANGE
            var request = new DeleteRequisitosComportamientosExpedientesMasivoCommand
            {
                IdsRequisitosComportamientos = new List<int>
                {
                    1, 2, 3
                }
            };
            var mockIStringLocalizer = new Mock<IStringLocalizer<DeleteRequisitosComportamientosExpedientesMasivoCommandHandler>>
            {
                CallBase = true
            };
            var sut = new DeleteRequisitosComportamientosExpedientesMasivoCommandHandler(Context, mockIStringLocalizer.Object);
            const string mensajeEsperado = "Como mínimo debe mantener un Requisito.";
            mockIStringLocalizer.Setup(s => s[It.Is<string>(msj => msj == mensajeEsperado)])
                .Returns(new LocalizedString(mensajeEsperado, mensajeEsperado));

            var comportamiento = new ComportamientoExpediente
            {
                RequisitosComportamientosExpedientes = Enumerable.Range(1, 3).Select(r => new RequisitoComportamientoExpediente
                {
                    Id = r
                }).ToList()
            };
            await Context.ComportamientosExpedientes.AddAsync(comportamiento);
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

        [Fact(DisplayName = "Cuando se eliminan los requisitos comportamientos Devuelve Ok")]
        public async Task Handle_Ok()
        {
            //ARRANGE
            var request = new DeleteRequisitosComportamientosExpedientesMasivoCommand
            {
                IdsRequisitosComportamientos = new List<int>
                {
                    1, 2, 3
                }
            };
            var mockIStringLocalizer = new Mock<IStringLocalizer<DeleteRequisitosComportamientosExpedientesMasivoCommandHandler>>
            {
                CallBase = true
            };
            var sut = new DeleteRequisitosComportamientosExpedientesMasivoCommandHandler(Context, mockIStringLocalizer.Object);
            var comportamiento = new ComportamientoExpediente
            {
                RequisitosComportamientosExpedientes = Enumerable.Range(1, 3).Select(r => new RequisitoComportamientoExpediente
                {
                    Id = r + 10
                }).ToList()
            };
            await Context.RequisitosComportamientosExpedientes.AddRangeAsync(Enumerable.Range(1, 3).Select(r => new RequisitoComportamientoExpediente
            {
                Id = r,
                ComportamientoExpediente = comportamiento
            }));
            await Context.SaveChangesAsync();

            //ACT
            await sut.Handle(request, CancellationToken.None);

            //ASSERT
            Assert.Empty(await Context.RequisitosComportamientosExpedientes.Where(a => 
                request.IdsRequisitosComportamientos.Contains(a.Id)).ToListAsync());
        }

        #endregion
    }
}
