using System;
using MediatR;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Unir.Expedientes.Application.ExpedientesAlumnos.Commands.ProcesarRelacionExpedienteAlumnoById;
using Unir.Expedientes.Application.ExpedientesAlumnos.Commands.RelacionarExpedienteAlumno;
using Unir.Expedientes.Application.ExpedientesAlumnos.Queries.GetExpedientesAlumnosARelacionar;
using Unir.Expedientes.Application.Tests.Common;
using Unir.Expedientes.Domain.Entities;
using Unir.Framework.ApplicationSuperTypes.Exceptions;
using Xunit;

namespace Unir.Expedientes.Application.Tests.ExpedientesAlumnos.Commands.ProcesarRelacionExpedienteAlumnoById
{
    [Collection("CommonTestCollection")]
    public class ProcesarRelacionExpedienteAlumnoByIdCommandHandlerTest : TestBase
    {
        #region Handle

        [Fact(DisplayName = "Cuando no encuentra expediente Retorna excepción")]
        public async Task Handle_NotFoundException()
        {
            //ARRANGE
            var mockIMediator = new Mock<IMediator>
            {
                CallBase = true
            };
            mockIMediator.Setup(m => m.Send(It.IsAny<GetExpedientesAlumnosARelacionarQuery>(), 
                    It.IsAny<CancellationToken>())).ReturnsAsync(new List<ExpedienteAlumno>());
            mockIMediator.Setup(m => m.Send(It.IsAny<RelacionarExpedienteAlumnoCommand>(), 
                    It.IsAny<CancellationToken>())).ReturnsAsync(Unit.Value);

            var sut = new ProcesarRelacionExpedienteAlumnoByIdCommandHandler(Context, mockIMediator.Object);
            var request = new ProcesarRelacionExpedienteAlumnoByIdCommand
            {
                IdExpedienteAlumno = 1
            };

            //ACT
            var exception = await Record.ExceptionAsync(async () =>
            {
                await sut.Handle(request, CancellationToken.None);
            });

            //ASSERT
            Assert.IsType<NotFoundException>(exception);
            Assert.Equal(new NotFoundException(nameof(ExpedienteAlumno), request.IdExpedienteAlumno).Message,
                exception.Message);
            mockIMediator.Verify(m => m.Send(It.IsAny<GetExpedientesAlumnosARelacionarQuery>(),
                It.IsAny<CancellationToken>()), Times.Once);
            mockIMediator.Verify(m => m.Send(It.IsAny<RelacionarExpedienteAlumnoCommand>(),
                It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact(DisplayName = "Cuando encuentra expediente Retorna ok")]
        public async Task Handle_Ok()
        {
            //ARRANGE
            var mockIMediator = new Mock<IMediator>
            {
                CallBase = true
            };
            mockIMediator.Setup(m => m.Send(It.IsAny<GetExpedientesAlumnosARelacionarQuery>(),
                It.IsAny<CancellationToken>())).ReturnsAsync(new List<ExpedienteAlumno>
            {
                new()
                {
                    Id = 1
                },
                new()
                {
                    Id = 2
                }
            });

            var tiposRelacionExpediente = Enumerable.Range(1, 3).Select(tre => new TipoRelacionExpediente
            {
                Id = tre,
                Nombre = Guid.NewGuid().ToString()
            });
            await Context.TiposRelacionesExpediente.AddRangeAsync(tiposRelacionExpediente);
            await Context.SaveChangesAsync();

            mockIMediator.Setup(m => m.Send(It.IsAny<RelacionarExpedienteAlumnoCommand>(),
                It.IsAny<CancellationToken>())).ReturnsAsync(Unit.Value);

            var sut = new ProcesarRelacionExpedienteAlumnoByIdCommandHandler(Context, mockIMediator.Object);

            //ACT
            await sut.Handle(new ProcesarRelacionExpedienteAlumnoByIdCommand
            {
                IdExpedienteAlumno = 1
            }, CancellationToken.None);

            //ASSERT
            mockIMediator.Verify(m => m.Send(It.IsAny<GetExpedientesAlumnosARelacionarQuery>(),
                It.IsAny<CancellationToken>()), Times.Once);
            mockIMediator.Verify(m => m.Send(It.IsAny<RelacionarExpedienteAlumnoCommand>(),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        #endregion
    }
}
