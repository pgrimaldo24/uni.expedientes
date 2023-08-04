using System.Linq;
using Moq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Localization;
using Unir.Expedientes.Application.Common.Interfaces;
using Unir.Expedientes.Application.Common.Models.ExpedientesGestorUnir;
using Unir.Expedientes.Application.ExpedientesAlumnos.Commands.HasBloqueoExpedienteAlumno;
using Unir.Expedientes.Application.Tests.Common;
using Unir.Expedientes.Domain.Entities;
using Unir.Framework.ApplicationSuperTypes.Exceptions;
using Xunit;

namespace Unir.Expedientes.Application.Tests.ExpedientesAlumnos.Commands.HasBloqueoExpedienteAlumno
{
    [Collection("CommonTestCollection")]
    public class HasBloqueoExpedienteAlumnoCommandHandlerTest : TestBase
    {
        #region Handle

        [Fact(DisplayName = "Cuando el expediente no está bloqueado Devuelve false")]
        public async Task Handle_NoBloqueado_Ok()
        {
            //ARRANGE
            var expedienteAlumno = new ExpedienteAlumno
            {
                Id = 1,
                IdRefIntegracionAlumno = "123",
                IdRefPlan = "456"
            };
            await Context.ExpedientesAlumno.AddAsync(expedienteAlumno);
            await Context.SaveChangesAsync(CancellationToken.None);
            var request = new HasBloqueoExpedienteAlumnoCommand(expedienteAlumno.Id);
            var mockIStringLocalizer = new Mock<IStringLocalizer<HasBloqueoExpedienteAlumnoCommandHandler>>
            {
                CallBase = true
            };
            var mockIExpedientesGestorUnirServiceClient = new Mock<IExpedientesGestorUnirServiceClient>
            {
                CallBase = true
            };
            mockIExpedientesGestorUnirServiceClient.Setup(s =>
                s.GetBloqueoExpediente(It.Is<int>(i => i == int.Parse(expedienteAlumno.IdRefIntegracionAlumno)),
                    It.Is<int>(i => i == int.Parse(expedienteAlumno.IdRefPlan)))).ReturnsAsync(
                new ExpedienteBloqueoModel
                {
                    Bloqueado = false,
                    CodigoResultado = 0
                });
            var sut = new HasBloqueoExpedienteAlumnoCommandHandler(Context, mockIStringLocalizer.Object,
                mockIExpedientesGestorUnirServiceClient.Object);

            //ACT
            var actual = await sut.Handle(request, CancellationToken.None);

            //ASSERT
            Assert.False(actual);
            mockIExpedientesGestorUnirServiceClient.Verify(s =>
                s.GetBloqueoExpediente(It.Is<int>(i => i == int.Parse(expedienteAlumno.IdRefIntegracionAlumno)),
                    It.Is<int>(i => i == int.Parse(expedienteAlumno.IdRefPlan))));
        }

        [Fact(DisplayName = "Cuando no se ha recibido el código de resultado ok Devuelve false")]
        public async Task Handle_CodigoResultadoIncorrecto_Ok()
        {
            //ARRANGE
            var expedienteAlumno = new ExpedienteAlumno
            {
                Id = 1,
                IdRefIntegracionAlumno = "123",
                IdRefPlan = "456"
            };
            await Context.ExpedientesAlumno.AddAsync(expedienteAlumno);
            await Context.SaveChangesAsync(CancellationToken.None);
            var request = new HasBloqueoExpedienteAlumnoCommand(expedienteAlumno.Id);
            var mockIStringLocalizer = new Mock<IStringLocalizer<HasBloqueoExpedienteAlumnoCommandHandler>>
            {
                CallBase = true
            };
            var mockIExpedientesGestorUnirServiceClient = new Mock<IExpedientesGestorUnirServiceClient>
            {
                CallBase = true
            };
            mockIExpedientesGestorUnirServiceClient.Setup(s =>
                s.GetBloqueoExpediente(It.Is<int>(i => i == int.Parse(expedienteAlumno.IdRefIntegracionAlumno)),
                    It.Is<int>(i => i == int.Parse(expedienteAlumno.IdRefPlan)))).ReturnsAsync(
                new ExpedienteBloqueoModel
                {
                    CodigoResultado = 1
                });
            var sut = new HasBloqueoExpedienteAlumnoCommandHandler(Context, mockIStringLocalizer.Object,
                mockIExpedientesGestorUnirServiceClient.Object);

            //ACT
            var actual = await sut.Handle(request, CancellationToken.None);

            //ASSERT
            Assert.False(actual);
            mockIExpedientesGestorUnirServiceClient.Verify(s =>
                s.GetBloqueoExpediente(It.Is<int>(i => i == int.Parse(expedienteAlumno.IdRefIntegracionAlumno)),
                    It.Is<int>(i => i == int.Parse(expedienteAlumno.IdRefPlan))));
        }

        [Fact(DisplayName = "Cuando el expediente está bloqueado Devuelve true")]
        public async Task Handle_Bloqueado_Ok()
        {
            //ARRANGE
            var expedienteAlumno = new ExpedienteAlumno
            {
                Id = 1,
                IdRefIntegracionAlumno = "123",
                IdRefPlan = "456"
            };
            await Context.ExpedientesAlumno.AddAsync(expedienteAlumno);
            await Context.SaveChangesAsync(CancellationToken.None);
            var request = new HasBloqueoExpedienteAlumnoCommand(expedienteAlumno.Id);
            var mockIStringLocalizer = new Mock<IStringLocalizer<HasBloqueoExpedienteAlumnoCommandHandler>>
            {
                CallBase = true
            };
            var mockIExpedientesGestorUnirServiceClient = new Mock<IExpedientesGestorUnirServiceClient>
            {
                CallBase = true
            };
            mockIExpedientesGestorUnirServiceClient.Setup(s =>
                s.GetBloqueoExpediente(It.Is<int>(i => i == int.Parse(expedienteAlumno.IdRefIntegracionAlumno)),
                    It.Is<int>(i => i == int.Parse(expedienteAlumno.IdRefPlan)))).ReturnsAsync(
                new ExpedienteBloqueoModel
                {
                    Bloqueado = true,
                    CodigoResultado = 0
                });
            var sut = new HasBloqueoExpedienteAlumnoCommandHandler(Context, mockIStringLocalizer.Object,
                mockIExpedientesGestorUnirServiceClient.Object);

            //ACT
            var actual = await sut.Handle(request, CancellationToken.None);

            //ASSERT
            Assert.True(actual);
            mockIExpedientesGestorUnirServiceClient.Verify(s =>
                s.GetBloqueoExpediente(It.Is<int>(i => i == int.Parse(expedienteAlumno.IdRefIntegracionAlumno)),
                    It.Is<int>(i => i == int.Parse(expedienteAlumno.IdRefPlan))));
        }

        [Fact(DisplayName = "Cuando el expediente no existe Devuelve excepción")]
        public async Task Handle_NotFoundException()
        {
            //ARRANGE
            var expedienteAlumno = new ExpedienteAlumno
            {
                Id = 1,
                IdRefIntegracionAlumno = "123",
                IdRefPlan = "456"
            };
            await Context.ExpedientesAlumno.AddAsync(expedienteAlumno);
            await Context.SaveChangesAsync(CancellationToken.None);
            var request = new HasBloqueoExpedienteAlumnoCommand(2);
            var mockIStringLocalizer = new Mock<IStringLocalizer<HasBloqueoExpedienteAlumnoCommandHandler>>
            {
                CallBase = true
            };
            var mockIExpedientesGestorUnirServiceClient = new Mock<IExpedientesGestorUnirServiceClient>
            {
                CallBase = true
            };
            var sut = new HasBloqueoExpedienteAlumnoCommandHandler(Context, mockIStringLocalizer.Object,
                mockIExpedientesGestorUnirServiceClient.Object);

            //ACT
            var ex = await Record.ExceptionAsync(async () =>
            {
                await sut.Handle(request, CancellationToken.None);
            });

            //ASSERT
            Assert.IsType<NotFoundException>(ex);
            Assert.Equal( new NotFoundException(nameof(ExpedienteAlumno), request.IdExpedienteAlumno).Message, ex.Message);
        }

        [Fact(DisplayName = "Cuando el expediente tiene un id ref integración alumno inválido Devuelve excepción")]
        public async Task Handle_IdRefIntegracionAlumnoInvalido()
        {
            //ARRANGE
            var expedienteAlumno = new ExpedienteAlumno
            {
                Id = 1,
                IdRefIntegracionAlumno = "s1",
                IdRefPlan = "456"
            };
            await Context.ExpedientesAlumno.AddAsync(expedienteAlumno);
            await Context.SaveChangesAsync(CancellationToken.None);
            var request = new HasBloqueoExpedienteAlumnoCommand(expedienteAlumno.Id);
            var mockIStringLocalizer = new Mock<IStringLocalizer<HasBloqueoExpedienteAlumnoCommandHandler>>
            {
                CallBase = true
            };
            var mensajeEsperado = $"El valor {nameof(expedienteAlumno.IdRefIntegracionAlumno)} del Expediente es inválido";
            mockIStringLocalizer.Setup(s => s[It.Is<string>(msj => msj == mensajeEsperado)])
                .Returns(new LocalizedString(mensajeEsperado, mensajeEsperado));
            var mockIExpedientesGestorUnirServiceClient = new Mock<IExpedientesGestorUnirServiceClient>
            {
                CallBase = true
            };
            var sut = new HasBloqueoExpedienteAlumnoCommandHandler(Context, mockIStringLocalizer.Object,
                mockIExpedientesGestorUnirServiceClient.Object);

            //ACT
            var ex = await Record.ExceptionAsync(async () =>
            {
                await sut.Handle(request, CancellationToken.None);
            });

            //ASSERT
            Assert.IsType<BadRequestException>(ex);
            Assert.Equal(mensajeEsperado, ex.Message);
        }

        [Fact(DisplayName = "Cuando el expediente tiene un id ref plan inválido Devuelve excepción")]
        public async Task Handle_IdRefPlanInvalido()
        {
            //ARRANGE
            var expedienteAlumno = new ExpedienteAlumno
            {
                Id = 1,
                IdRefIntegracionAlumno = "1",
                IdRefPlan = "456s"
            };
            await Context.ExpedientesAlumno.AddAsync(expedienteAlumno);
            await Context.SaveChangesAsync(CancellationToken.None);
            var request = new HasBloqueoExpedienteAlumnoCommand(expedienteAlumno.Id);
            var mockIStringLocalizer = new Mock<IStringLocalizer<HasBloqueoExpedienteAlumnoCommandHandler>>
            {
                CallBase = true
            };
            var mensajeEsperado = $"El valor {nameof(expedienteAlumno.IdRefPlan)} del Expediente es inválido";
            mockIStringLocalizer.Setup(s => s[It.Is<string>(msj => msj == mensajeEsperado)])
                .Returns(new LocalizedString(mensajeEsperado, mensajeEsperado));
            var mockIExpedientesGestorUnirServiceClient = new Mock<IExpedientesGestorUnirServiceClient>
            {
                CallBase = true
            };
            var sut = new HasBloqueoExpedienteAlumnoCommandHandler(Context, mockIStringLocalizer.Object,
                mockIExpedientesGestorUnirServiceClient.Object);

            //ACT
            var ex = await Record.ExceptionAsync(async () =>
            {
                await sut.Handle(request, CancellationToken.None);
            });

            //ASSERT
            Assert.IsType<BadRequestException>(ex);
            Assert.Equal(mensajeEsperado, ex.Message);
        }

        #endregion
    }
}
