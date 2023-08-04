using MediatR;
using Microsoft.Extensions.Localization;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Unir.Expedientes.Application.Common.Models.Settings;
using Unir.Expedientes.Application.Common.Queries.ExpedicionTitulos.GetTipoSolicitud;
using Unir.Expedientes.Application.ExpedientesAlumnos.Commands.GenerarSolicitudTituloCertificado;
using Unir.Expedientes.Application.ExpedientesAlumnos.Commands.ValidarGenerarSolicitudTituloCertificado;
using Unir.Expedientes.Application.Tests.Common;
using Unir.Expedientes.Domain.Entities;
using Unir.Framework.ApplicationSuperTypes.Exceptions;
using Xunit;

namespace Unir.Expedientes.Application.Tests.ExpedientesAlumnos.Commands.ValidarGenerarSolicitudTituloCertificado
{
    [Collection("CommonTestCollection")]
    public class ValidarGenerarSolicitudTituloCertificadoCommandHandlerTest : TestBase
    {
        #region Handle

        [Fact(DisplayName = "Cuando los parámetros cumplen las validaciones Retorna Unit")]
        public async Task Handle_Ok()
        {
            //ARRANGE
            AppConfiguration.KeyAdminRole = "admin_expediente";
            var generarSolicitud = new GenerarSolicitudTituloCertificadoCommand
            {
                TipoSolicitud = 1,
                FechaSolicitud = DateTime.UtcNow,
                FechaPago = DateTime.UtcNow,
                IdsExpedientes = new List<int> { 1, 2 }
            };

            var expedientes = new List<ExpedienteAlumno>
            {
                new ()
                {
                    Id = 1,
                    IdRefUniversidad = "1"
                },
                new ()
                {
                    Id = 2,
                    IdRefUniversidad = "1"
                }
            };
            var tipoSolicitud = new TipoSolicitudDto();
            var roles = new[] { AppConfiguration.KeyAdminRole };

            var request = new ValidarGenerarSolicitudTituloCertificadoCommand(
                generarSolicitud, expedientes, tipoSolicitud, roles);

            var mockIStringLocalizer = new Mock<IStringLocalizer<ValidarGenerarSolicitudTituloCertificadoCommandHandler>>
            {
                CallBase = true
            };

            var sut = new ValidarGenerarSolicitudTituloCertificadoCommandHandler(mockIStringLocalizer.Object);

            //ACT
            var actual = await sut.Handle(request, CancellationToken.None);

            //ASSERT
            Assert.IsType<Unit>(actual);
        }

        [Fact(DisplayName = "Cuando la lista de idsExpedientes es vacío Retorna BadRequest")]
        public async Task Handle_BadRequest_Expedientes()
        {
            //ARRANGE
            const string mensajeEsperado = "Debe seleccionar expedientes";
            var generarSolicitud = new GenerarSolicitudTituloCertificadoCommand
            {
                IdsExpedientes = new List<int>()
            };
            var tipoSolicitud = new TipoSolicitudDto();
            var roles = Array.Empty<string>();

            var request = new ValidarGenerarSolicitudTituloCertificadoCommand(
                generarSolicitud, new List<ExpedienteAlumno>(), tipoSolicitud, roles);

            var mockIStringLocalizer = new Mock<IStringLocalizer<ValidarGenerarSolicitudTituloCertificadoCommandHandler>>
            {
                CallBase = true
            };
            mockIStringLocalizer.Setup(s => s[It.Is<string>(msj => msj == mensajeEsperado)])
                .Returns(new LocalizedString(mensajeEsperado, mensajeEsperado));

            var sut = new ValidarGenerarSolicitudTituloCertificadoCommandHandler(mockIStringLocalizer.Object);

            //ACT
            var ex = (BadRequestException)await Record.ExceptionAsync(async () =>
            {
                await sut.Handle(request, CancellationToken.None);
            });

            //ASSERT
            Assert.NotNull(ex);
            Assert.IsType<BadRequestException>(ex);
            Assert.Equal(mensajeEsperado, ex.Message);
        }

        [Fact(DisplayName = "Cuando hay expedientes diferentes IdRefUniversidad Retorna BadRequest")]
        public async Task Handle_BadRequest_UniversidadDuplicados()
        {
            //ARRANGE
            const string mensajeEsperado = "No se permiten expedientes de diferentes universidades";
            var generarSolicitud = new GenerarSolicitudTituloCertificadoCommand
            {
                IdsExpedientes = new List<int> { 1, 2 }
            };
            var expedientes = new List<ExpedienteAlumno>
            {
                new ()
                {
                    Id = 1,
                    IdRefUniversidad = "1"
                },
                new ()
                {
                    Id = 2,
                    IdRefUniversidad = "2"
                }
            };
            var tipoSolicitud = new TipoSolicitudDto();
            var roles = Array.Empty<string>();

            var request = new ValidarGenerarSolicitudTituloCertificadoCommand(generarSolicitud, expedientes, tipoSolicitud, roles);

            var mockIStringLocalizer = new Mock<IStringLocalizer<ValidarGenerarSolicitudTituloCertificadoCommandHandler>>
            {
                CallBase = true
            };
            mockIStringLocalizer.Setup(s => s[It.Is<string>(msj => msj == mensajeEsperado)])
                .Returns(new LocalizedString(mensajeEsperado, mensajeEsperado));

            var sut = new ValidarGenerarSolicitudTituloCertificadoCommandHandler(mockIStringLocalizer.Object);

            //ACT
            var ex = (BadRequestException)await Record.ExceptionAsync(async () =>
            {
                await sut.Handle(request, CancellationToken.None);
            });

            //ASSERT
            Assert.NotNull(ex);
            Assert.IsType<BadRequestException>(ex);
            Assert.Equal(mensajeEsperado, ex.Message);
        }

        [Fact(DisplayName = "Cuando la fecha de solicitud es menor a la actual Retorna BadRequest")]
        public async Task Handle_BadRequest_FechaSolicitud()
        {
            //ARRANGE
            const string mensajeEsperado = "La fecha solicitud no debe ser menor a la fecha actual";
            var generarSolicitud = new GenerarSolicitudTituloCertificadoCommand
            {
                IdsExpedientes = new List<int> { 1, 2 },
                FechaSolicitud = DateTime.UtcNow.AddDays(-2)
            };
            var expedientes = new List<ExpedienteAlumno>
            {
                new ()
                {
                    Id = 1,
                    IdRefUniversidad = "1"
                },
                new ()
                {
                    Id = 2,
                    IdRefUniversidad = "1"
                }
            };
            var tipoSolicitud = new TipoSolicitudDto();
            var roles = Array.Empty<string>();

            var request = new ValidarGenerarSolicitudTituloCertificadoCommand(
                generarSolicitud, expedientes, tipoSolicitud, roles);

            var mockIStringLocalizer = new Mock<IStringLocalizer<ValidarGenerarSolicitudTituloCertificadoCommandHandler>>
            {
                CallBase = true
            };
            mockIStringLocalizer.Setup(s => s[It.Is<string>(msj => msj == mensajeEsperado)])
                .Returns(new LocalizedString(mensajeEsperado, mensajeEsperado));

            var sut = new ValidarGenerarSolicitudTituloCertificadoCommandHandler(mockIStringLocalizer.Object);

            //ACT
            var ex = (BadRequestException)await Record.ExceptionAsync(async () =>
            {
                await sut.Handle(request, CancellationToken.None);
            });

            //ASSERT
            Assert.NotNull(ex);
            Assert.IsType<BadRequestException>(ex);
            Assert.Equal(mensajeEsperado, ex.Message);
        }

        [Fact(DisplayName = "Cuando la fecha de pago es menor a la actual Retorna BadRequest")]
        public async Task Handle_BadRequest_FechaPago()
        {
            //ARRANGE
            const string mensajeEsperado = "La fecha pago no debe ser menor a la fecha actual";
            var generarSolicitud = new GenerarSolicitudTituloCertificadoCommand
            {
                IdsExpedientes = new List<int> { 1, 2 },
                FechaSolicitud = DateTime.UtcNow,
                FechaPago = DateTime.UtcNow.AddDays(-2)
            };
            var expedientes = new List<ExpedienteAlumno>
            {
                new ()
                {
                    Id = 1,
                    IdRefUniversidad = "1"
                },
                new ()
                {
                    Id = 2,
                    IdRefUniversidad = "1"
                }
            };
            var tipoSolicitud = new TipoSolicitudDto();
            var roles = Array.Empty<string>();

            var request = new ValidarGenerarSolicitudTituloCertificadoCommand(
                generarSolicitud, expedientes, tipoSolicitud, roles);

            var mockIStringLocalizer = new Mock<IStringLocalizer<ValidarGenerarSolicitudTituloCertificadoCommandHandler>>
            {
                CallBase = true
            };
            mockIStringLocalizer.Setup(s => s[It.Is<string>(msj => msj == mensajeEsperado)])
                .Returns(new LocalizedString(mensajeEsperado, mensajeEsperado));

            var sut = new ValidarGenerarSolicitudTituloCertificadoCommandHandler(mockIStringLocalizer.Object);

            //ACT
            var ex = (BadRequestException)await Record.ExceptionAsync(async () =>
            {
                await sut.Handle(request, CancellationToken.None);
            });

            //ASSERT
            Assert.NotNull(ex);
            Assert.IsType<BadRequestException>(ex);
            Assert.Equal(mensajeEsperado, ex.Message);
        }

        [Fact(DisplayName = "Cuando la fecha de pago es requerido y el valor es nulo Retorna BadRequest")]
        public async Task Handle_BadRequest_FechaPagoRequerido()
        {
            //ARRANGE
            AppConfiguration.KeyAdminRole = "admin_expediente";
            const string mensajeEsperado = "Debe seleccionar fecha de pago";
            var generarSolicitud = new GenerarSolicitudTituloCertificadoCommand
            {
                IdsExpedientes = new List<int> { 1, 2 },
                FechaSolicitud = DateTime.UtcNow,
                FechaPago = null
            };
            var expedientes = new List<ExpedienteAlumno>
            {
                new ()
                {
                    Id = 1,
                    IdRefUniversidad = "1"
                },
                new ()
                {
                    Id = 2,
                    IdRefUniversidad = "1"
                }
            };
            var tipoSolicitud = new TipoSolicitudDto
            {
                Id = 1,
                ConFechaPago = true
            };
            var roles = new[] { AppConfiguration.KeyAdminRole };

            var request = new ValidarGenerarSolicitudTituloCertificadoCommand(
                generarSolicitud, expedientes, tipoSolicitud, roles);

            var mockIStringLocalizer = new Mock<IStringLocalizer<ValidarGenerarSolicitudTituloCertificadoCommandHandler>>
            {
                CallBase = true
            };
            mockIStringLocalizer.Setup(s => s[It.Is<string>(msj => msj == mensajeEsperado)])
                .Returns(new LocalizedString(mensajeEsperado, mensajeEsperado));

            var sut = new ValidarGenerarSolicitudTituloCertificadoCommandHandler(mockIStringLocalizer.Object);

            //ACT
            var ex = (BadRequestException)await Record.ExceptionAsync(async () =>
            {
                await sut.Handle(request, CancellationToken.None);
            });

            //ASSERT
            Assert.NotNull(ex);
            Assert.IsType<BadRequestException>(ex);
            Assert.Equal(mensajeEsperado, ex.Message);
        }

        #endregion
    }
}
