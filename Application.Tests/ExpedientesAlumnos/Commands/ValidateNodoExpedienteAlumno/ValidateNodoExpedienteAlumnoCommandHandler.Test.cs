using System.Collections.Generic;
using System.Linq;
using Moq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Localization;
using Unir.Expedientes.Application.Common.Interfaces;
using Unir.Expedientes.Application.Common.Models.ErpAcademico.PlanesEstudio;
using Unir.Expedientes.Application.ExpedientesAlumnos.Commands.ValidateNodoExpedienteAlumno;
using Unir.Expedientes.Application.Tests.Common;
using Unir.Expedientes.Domain.Entities;
using Unir.Framework.ApplicationSuperTypes.Exceptions;
using Xunit;

namespace Unir.Expedientes.Application.Tests.ExpedientesAlumnos.Commands.ValidateNodoExpedienteAlumno
{
    [Collection("CommonTestCollection")]
    public class ValidateNodoExpedienteAlumnoCommandHandlerTest : TestBase
    {
        #region Handle

        [Fact(DisplayName = "Cuando no se envía el expediente No realiza ninguna acción")]
        public async Task Handle_SinExpediente()
        {
            //ARRANGE
            var request = new ValidateNodoExpedienteAlumnoCommand(null);
            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };
            var mockIStringLocalizer = new Mock<IStringLocalizer<ValidateNodoExpedienteAlumnoCommandHandler>>
            {
                CallBase = true
            };
            var sut = new ValidateNodoExpedienteAlumnoCommandHandler(mockIErpAcademicoServiceClient.Object,
                mockIStringLocalizer.Object);

            //ACT
            await sut.Handle(request, CancellationToken.None);

            //ASSERT
            Assert.Null(request.ExpedienteAlumno);
        }

        [Fact(DisplayName = "Cuando no se envía el id ref versión plan ni nodo No realiza ninguna acción")]
        public async Task Handle_SinVersionNiNodo()
        {
            //ARRANGE
            var request = new ValidateNodoExpedienteAlumnoCommand(new ExpedienteAlumno());
            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };
            var mockIStringLocalizer = new Mock<IStringLocalizer<ValidateNodoExpedienteAlumnoCommandHandler>>
            {
                CallBase = true
            };
            var sut = new ValidateNodoExpedienteAlumnoCommandHandler(mockIErpAcademicoServiceClient.Object,
                mockIStringLocalizer.Object);

            //ACT
            await sut.Handle(request, CancellationToken.None);

            //ASSERT
            Assert.Null(request.ExpedienteAlumno.IdRefVersionPlan);
            Assert.Null(request.ExpedienteAlumno.IdRefNodo);
        }

        [Fact(DisplayName = "Cuando el nodo no existe Devuelve excepción")]
        public async Task Handle_NotFoundExcepcion()
        {
            //ARRANGE
            var expedienteAlumno = new ExpedienteAlumno
            {
                IdRefNodo = "12",
                IdRefVersionPlan = "45"
            };
            var request = new ValidateNodoExpedienteAlumnoCommand(expedienteAlumno);
            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };
            mockIErpAcademicoServiceClient
                .Setup(s => s.GetNodo(It.Is<int>(i => i == int.Parse(expedienteAlumno.IdRefNodo))))
                .ReturnsAsync(null as NodoErpAcademicoModel);
            var mockIStringLocalizer = new Mock<IStringLocalizer<ValidateNodoExpedienteAlumnoCommandHandler>>
            {
                CallBase = true
            };
            var sut = new ValidateNodoExpedienteAlumnoCommandHandler(mockIErpAcademicoServiceClient.Object,
                mockIStringLocalizer.Object);

            //ACT
            var ex = await Record.ExceptionAsync(async () =>
            {
                await sut.Handle(request, CancellationToken.None);
            });

            //ASSERT
            Assert.IsType<NotFoundException>(ex);
            Assert.Equal(new NotFoundException(nameof(NodoErpAcademicoModel), request.ExpedienteAlumno.IdRefNodo).Message,
                ex.Message);
        }

        [Fact(DisplayName = "Cuando el nodo no tiene versiones Devuelve excepción")]
        public async Task Handle_NodoSinVersiones()
        {
            //ARRANGE
            var expedienteAlumno = new ExpedienteAlumno
            {
                IdRefNodo = "12",
                IdRefVersionPlan = "45"
            };
            var request = new ValidateNodoExpedienteAlumnoCommand(expedienteAlumno);
            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };
            mockIErpAcademicoServiceClient
                .Setup(s => s.GetNodo(It.Is<int>(i => i == int.Parse(expedienteAlumno.IdRefNodo))))
                .ReturnsAsync(new NodoErpAcademicoModel());
            var mockIStringLocalizer = new Mock<IStringLocalizer<ValidateNodoExpedienteAlumnoCommandHandler>>
            {
                CallBase = true
            };
            const string mensajeEsperado =
                "La Versión del Plan vinculada al Expediente no está asociada al Nodo de Inicio.";
            mockIStringLocalizer.Setup(s => s[It.Is<string>(msj => msj == mensajeEsperado)])
                .Returns(new LocalizedString(mensajeEsperado, mensajeEsperado));
            var sut = new ValidateNodoExpedienteAlumnoCommandHandler(mockIErpAcademicoServiceClient.Object,
                mockIStringLocalizer.Object);

            //ACT
            var ex = await Record.ExceptionAsync(async () =>
            {
                await sut.Handle(request, CancellationToken.None);
            });

            //ASSERT
            Assert.IsType<BadRequestException>(ex);
            Assert.Equal(mensajeEsperado, ex.Message);
        }

        [Fact(DisplayName = "Cuando la versión no pertenece al nodo Devuelve excepción")]
        public async Task Handle_VersionInvalida()
        {
            //ARRANGE
            var expedienteAlumno = new ExpedienteAlumno
            {
                IdRefNodo = "12",
                IdRefVersionPlan = "45"
            };
            var request = new ValidateNodoExpedienteAlumnoCommand(expedienteAlumno);
            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };
            mockIErpAcademicoServiceClient
                .Setup(s => s.GetNodo(It.Is<int>(i => i == int.Parse(expedienteAlumno.IdRefNodo))))
                .ReturnsAsync(new NodoErpAcademicoModel
                {
                    VersionesPlanes = new List<VersionPlanAcademicoModel>
                    {
                        new()
                        {
                            Id = 1
                        }
                    }
                });
            var mockIStringLocalizer = new Mock<IStringLocalizer<ValidateNodoExpedienteAlumnoCommandHandler>>
            {
                CallBase = true
            };
            const string mensajeEsperado =
                "La Versión del Plan vinculada al Expediente no está asociada al Nodo de Inicio.";
            mockIStringLocalizer.Setup(s => s[It.Is<string>(msj => msj == mensajeEsperado)])
                .Returns(new LocalizedString(mensajeEsperado, mensajeEsperado));
            var sut = new ValidateNodoExpedienteAlumnoCommandHandler(mockIErpAcademicoServiceClient.Object,
                mockIStringLocalizer.Object);

            //ACT
            var ex = await Record.ExceptionAsync(async () =>
            {
                await sut.Handle(request, CancellationToken.None);
            });

            //ASSERT
            Assert.IsType<BadRequestException>(ex);
            Assert.Equal(mensajeEsperado, ex.Message);
        }

        [Fact(DisplayName = "Cuando la versión pertenece al nodo Devuelve ok")]
        public async Task Handle_Ok()
        {
            //ARRANGE
            var expedienteAlumno = new ExpedienteAlumno
            {
                IdRefNodo = "12",
                IdRefVersionPlan = "45"
            };
            var request = new ValidateNodoExpedienteAlumnoCommand(expedienteAlumno);
            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };
            mockIErpAcademicoServiceClient
                .Setup(s => s.GetNodo(It.Is<int>(i => i == int.Parse(expedienteAlumno.IdRefNodo))))
                .ReturnsAsync(new NodoErpAcademicoModel
                {
                    VersionesPlanes = new List<VersionPlanAcademicoModel>
                    {
                        new()
                        {
                            Id = 45
                        },
                        new()
                        {
                            Id = 1
                        }
                    }
                });
            var mockIStringLocalizer = new Mock<IStringLocalizer<ValidateNodoExpedienteAlumnoCommandHandler>>
            {
                CallBase = true
            };
            var sut = new ValidateNodoExpedienteAlumnoCommandHandler(mockIErpAcademicoServiceClient.Object,
                mockIStringLocalizer.Object);

            //ACT
            await sut.Handle(request, CancellationToken.None);

            //ASSERT
            mockIErpAcademicoServiceClient.Verify(
                s => s.GetNodo(It.Is<int>(i => i == int.Parse(expedienteAlumno.IdRefNodo))), Times.Once);
        }
        #endregion
    }
}
