using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Unir.Expedientes.Application.ComportamientosExpedientes.Commands.DeleteComportamientosExpedientesMasivo;
using Unir.Expedientes.Application.Tests.Common;
using Unir.Expedientes.Domain.Entities;
using Unir.Framework.ApplicationSuperTypes.Exceptions;
using Xunit;

namespace Unir.Expedientes.Application.Tests.ComportamientosExpedientes.Commands.DeleteComportamientosMasivo
{
    [Collection("CommonTestCollection")]
    public class DeleteComportamientosExpedientesMasivoCommandHandlerTests : TestBase
    {
        #region Handle

        [Fact(DisplayName = "Cuando los requisitos no existen Devuelve una excepción")]
        public async Task Handle_BadRequestException()
        {
            //ARRANGE
            var request = new DeleteComportamientosExpedientesMasivoCommand();
            var mockIStringLocalizer = new Mock<IStringLocalizer<DeleteComportamientosExpedientesMasivoCommandHandler>>
            {
                CallBase = true
            };
            const string mensajeEsperado = "Los comportamientos seleccionados no existen.";
            mockIStringLocalizer.Setup(s => s[It.Is<string>(msj => msj == mensajeEsperado)])
                .Returns(new LocalizedString(mensajeEsperado, mensajeEsperado));
            var sut = new Mock<DeleteComportamientosExpedientesMasivoCommandHandler>(Context, mockIStringLocalizer.Object)
            {
                CallBase = true
            };

            //ACT
            var ex = await Record.ExceptionAsync(async () =>
            {
                await sut.Object.Handle(request, CancellationToken.None);
            });

            //ASSERT
            Assert.NotNull(ex);
            Assert.IsType<BadRequestException>(ex);
            Assert.Equal(mensajeEsperado, ex.Message);
            mockIStringLocalizer.Verify(s => s[It.Is<string>(msj => msj == mensajeEsperado)], Times.Once);
        }

        [Fact(DisplayName = "Cuando se eliminan los comportamientos pero tienen algunos requisitos que tienen consolidaciones Devuelve Ok con mensajes")]
        public async Task Handle_Message_Ok()
        {
            //ARRANGE
            var request = new DeleteComportamientosExpedientesMasivoCommand
            {
                IdsComportamientos = new List<int>
                {
                    1, 2
                }
            };
            var mockIStringLocalizer = new Mock<IStringLocalizer<DeleteComportamientosExpedientesMasivoCommandHandler>>
            {
                CallBase = true
            };
            var sut = new DeleteComportamientosExpedientesMasivoCommandHandler(Context, mockIStringLocalizer.Object);
            var comportamiento = new ComportamientoExpediente
            {
                Id = 1
            };
            var comportamientoRequisitoConsolidacion = new ComportamientoExpediente
            {
                Id = 2,
                RequisitosComportamientosExpedientes = new List<RequisitoComportamientoExpediente>
                {
                    new()
                    {
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
            await Context.ComportamientosExpedientes.AddAsync(comportamientoRequisitoConsolidacion);
            await Context.SaveChangesAsync();

            //ACT
            var actual = await sut.Handle(request, CancellationToken.None);

            //ASSERT
            Assert.NotEmpty(actual);
            Assert.Single(actual);
        }

        [Fact(DisplayName = "Cuando se eliminan los comportamientos y no hay relación con otras tablas Devuelve Ok sin mensajes")]
        public async Task Handle_Ok()
        {
            //ARRANGE
            var request = new DeleteComportamientosExpedientesMasivoCommand
            {
                IdsComportamientos = new List<int>
                {
                    1, 2, 3
                }
            };
            var mockIStringLocalizer = new Mock<IStringLocalizer<DeleteComportamientosExpedientesMasivoCommandHandler>>
            {
                CallBase = true
            };
            var sutMock = new Mock<DeleteComportamientosExpedientesMasivoCommandHandler>(Context, mockIStringLocalizer.Object)
            {
                CallBase = true
            };
            await Context.ComportamientosExpedientes.AddRangeAsync(Enumerable.Range(1, 3).Select(r => new ComportamientoExpediente
            {
                Id = r
            }));
            await Context.SaveChangesAsync();

            //ACT
            var actual = await sutMock.Object.Handle(request, CancellationToken.None);

            //ASSERT
            Assert.Empty(actual);
            Assert.Empty(await Context.ComportamientosExpedientes.ToListAsync());
        }

        #endregion
    }
}
