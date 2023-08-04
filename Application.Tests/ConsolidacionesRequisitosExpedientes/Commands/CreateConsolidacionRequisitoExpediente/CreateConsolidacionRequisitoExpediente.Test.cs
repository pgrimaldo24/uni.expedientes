using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Moq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Unir.Expedientes.Application.ConsolidacionesRequisitosExpedientes.Commands.CreateConsolidacionRequisitoExpediente;
using Unir.Expedientes.Application.Tests.Common;
using Unir.Expedientes.Domain.Entities;
using Unir.Framework.ApplicationSuperTypes.Exceptions;
using Xunit;

namespace Unir.Expedientes.Application.Tests.ConsolidacionesRequisitosExpedientes.Commands.CreateConsolidacionRequisitoExpediente
{
    [Collection("CommonTestCollection")]
    public class CreateConsolidacionRequisitoExpedienteTests : TestBase
    {
        #region Handle

        [Fact(DisplayName = "Cuando no existe el expediente alumno Devuelve una excepción")]
        public async Task Handle_NotFoundException_ExpedienteAlumno()
        {
            //ARRANGE
            var request = new CreateConsolidacionRequisitoExpedienteCommand();
            var sut = new CreateConsolidacionRequisitoExpedienteCommandHandler(Context, null);

            //ACT
            var ex = await Record.ExceptionAsync(async () =>
            {
                await sut.Handle(request, CancellationToken.None);
            });

            //ASSERT
            Assert.NotNull(ex);
            Assert.IsType<NotFoundException>(ex);
        }

        [Fact(DisplayName = "Cuando no existe el requisito Devuelve una excepción")]
        public async Task Handle_NotFoundException_RequisitoExpediente()
        {
            //ARRANGE
            var request = new CreateConsolidacionRequisitoExpedienteCommand
            {
                IdExpedienteAlumno = 1
            };
            await Context.ExpedientesAlumno.AddAsync(new ExpedienteAlumno
            {
                Id = 1
            });
            await Context.SaveChangesAsync();
            var sut = new CreateConsolidacionRequisitoExpedienteCommandHandler(Context, null);

            //ACT
            var ex = await Record.ExceptionAsync(async () =>
            {
                await sut.Handle(request, CancellationToken.None);
            });

            //ASSERT
            Assert.NotNull(ex);
            Assert.IsType<NotFoundException>(ex);
        }

        [Fact(DisplayName = "Cuando ya existe una consolidación con el mismo requisito y expediente Devuelve una excepción")]
        public async Task Handle_BadRequestException_ConsolidacionExistente()
        {
            //ARRANGE
            var request = new CreateConsolidacionRequisitoExpedienteCommand
            {
                IdExpedienteAlumno = 1,
                IdRequisitoExpediente = 1
            };
            await Context.ExpedientesAlumno.AddAsync(new ExpedienteAlumno
            {
                Id = 1
            });
            await Context.RequisitosExpedientes.AddAsync(new RequisitoExpediente
            {
                Id = 1
            });
            await Context.SaveChangesAsync();
            await Context.ConsolidacionesRequisitosExpedientes.AddAsync(new ConsolidacionRequisitoExpediente
            {
                Id = 1,
                ExpedienteAlumno = await Context.ExpedientesAlumno.FirstAsync(),
                RequisitoExpediente = await Context.RequisitosExpedientes.FirstAsync()
            });
            await Context.SaveChangesAsync();
            var mockIStringLocalizer = new Mock<IStringLocalizer<CreateConsolidacionRequisitoExpedienteCommandHandler>>
            {
                CallBase = true
            };
            var sutMock = new Mock<CreateConsolidacionRequisitoExpedienteCommandHandler>(Context, mockIStringLocalizer.Object) 
                {CallBase = true };

            const string mensajeEsperado = "Ya existe una consolidación con el mismo requisito y expediente";
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

        [Fact(DisplayName = "Cuando no se encuentra el requisito comportamiento expediente Devuelve una excepción")]
        public async Task Handle_BadRequestException_RequisitoComportamientoExpediente()
        {
            //ARRANGE
            var request = new CreateConsolidacionRequisitoExpedienteCommand
            {
                IdExpedienteAlumno = 1,
                IdRequisitoExpediente = 1
            };
            await Context.ExpedientesAlumno.AddAsync(new ExpedienteAlumno
            {
                Id = 1
            });
            await Context.RequisitosExpedientes.AddAsync(new RequisitoExpediente
            {
                Id = 1
            });
            await Context.SaveChangesAsync();
            var mockIStringLocalizer = new Mock<IStringLocalizer<CreateConsolidacionRequisitoExpedienteCommandHandler>>
            {
                CallBase = true
            };
            var sutMock = new Mock<CreateConsolidacionRequisitoExpedienteCommandHandler>(Context, mockIStringLocalizer.Object)
            { CallBase = true };

            const string mensajeEsperado = "No se encontró el requisito comportamiento expediente";
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
            var request = new CreateConsolidacionRequisitoExpedienteCommand
            {
                IdExpedienteAlumno = 1,
                IdRequisitoExpediente = 1
            };
            await Context.ExpedientesAlumno.AddAsync(new ExpedienteAlumno
            {
                Id = 1,
                IdRefUniversidad = "1"
            });
            await Context.RequisitosExpedientes.AddAsync(new RequisitoExpediente
            {
                Id = 1
            });
            await Context.TiposRequisitosExpedientes.AddAsync(new TipoRequisitoExpediente
            {
                Id = 1
            });
            await Context.ComportamientosExpedientes.AddAsync(new ComportamientoExpediente
            {
                Id = 1,
                Bloqueado = true,
                NivelesUsoComportamientosExpedientes = new List<NivelUsoComportamientoExpediente>
                {
                    new()
                    {
                        IdRefUniversidad = "1",
                        TipoNivelUsoId = TipoNivelUso.Universidad
                    }
                }
            });
            await Context.SaveChangesAsync();
            await Context.RequisitosComportamientosExpedientes.AddAsync(new RequisitoComportamientoExpediente
            {
                Id = 1,
                RequisitoExpediente = await Context.RequisitosExpedientes.FirstAsync(),
                TipoRequisitoExpediente = await Context.TiposRequisitosExpedientes.FirstAsync(),
                ComportamientoExpediente = await Context.ComportamientosExpedientes.FirstAsync()
            });
            await Context.SaveChangesAsync();
            var mockIStringLocalizer = new Mock<IStringLocalizer<CreateConsolidacionRequisitoExpedienteCommandHandler>>
            {
                CallBase = true
            };
            var sutMock = new Mock<CreateConsolidacionRequisitoExpedienteCommandHandler>(Context, mockIStringLocalizer.Object)
                { CallBase = true };

            //ACT
            var actual = await sutMock.Object.Handle(request, CancellationToken.None);

            //ASSERT
            Assert.True(actual > 0);
            Assert.True(await Context.ConsolidacionesRequisitosExpedientes.AnyAsync());
        }

        #endregion
    }
}
