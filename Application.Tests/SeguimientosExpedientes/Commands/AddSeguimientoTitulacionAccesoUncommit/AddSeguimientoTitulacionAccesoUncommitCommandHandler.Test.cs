using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Unir.Expedientes.Application.SeguimientosExpedientes.Commands.AddSeguimientoTitulacionAccesoUncommit;
using Unir.Expedientes.Application.Tests.Common;
using Unir.Expedientes.Domain.Entities;
using Unir.Framework.Crosscutting.Security;
using Unir.Framework.Crosscutting.Security.Model;
using Xunit;

namespace Unir.Expedientes.Application.Tests.SeguimientosExpedientes.Commands.AddSeguimientoTitulacionAccesoUncommit
{
    [Collection("CommonTestCollection")]
    public class AddSeguimientoTitulacionAccesoUncommitCommandHandlerTest : TestBase
    {
        #region Handle

        [Fact(DisplayName = "Cuando existe diferencias en titulación de acceso Retorna true")]
        public async Task Handle_True()
        {
            //ARRANGE
            var expedienteAlumno = new ExpedienteAlumno
            {
                Id = 1,
                IdRefNodo = "456",
                IdRefVersionPlan = "123"
            };
            await Context.ExpedientesAlumno.AddAsync(expedienteAlumno);
            await Context.SaveChangesAsync(CancellationToken.None);
            var request = new AddSeguimientoTitulacionAccesoUncommitCommand(expedienteAlumno, false, null);
            var mockIIdentityService = new Mock<IIdentityService>
            {
                CallBase = true
            };
            var sut = new Mock<AddSeguimientoTitulacionAccesoUncommitCommandHandler>(mockIIdentityService.Object)
            {
                CallBase = true
            };
            sut.Setup(x => x.AddSeguimientoTitulacionAcceso(
                It.IsAny<AddSeguimientoTitulacionAccesoUncommitCommand>())).Returns(true);

            //ACT
            var actual = await sut.Object.Handle(request, CancellationToken.None);

            //ASSERT
            Assert.True(actual);
            sut.Verify(x => x.AddSeguimientoTitulacionAcceso(
                It.IsAny<AddSeguimientoTitulacionAccesoUncommitCommand>()), Times.Once);
        }

        [Fact(DisplayName = "Cuando no existe diferencias en titulación de acceso Retorna false")]
        public async Task Handle_False()
        {
            //ARRANGE
            var expedienteAlumno = new ExpedienteAlumno
            {
                Id = 1,
                IdRefNodo = "456",
                IdRefVersionPlan = "123"
            };
            await Context.ExpedientesAlumno.AddAsync(expedienteAlumno);
            await Context.SaveChangesAsync(CancellationToken.None);
            var request = new AddSeguimientoTitulacionAccesoUncommitCommand(expedienteAlumno, false, null);
            var mockIIdentityService = new Mock<IIdentityService>
            {
                CallBase = true
            };
            var sut = new Mock<AddSeguimientoTitulacionAccesoUncommitCommandHandler>(mockIIdentityService.Object)
            {
                CallBase = true
            };
            sut.Setup(x => x.AddSeguimientoTitulacionAcceso(
                It.IsAny<AddSeguimientoTitulacionAccesoUncommitCommand>())).Returns(false);

            //ACT
            var actual = await sut.Object.Handle(request, CancellationToken.None);

            //ASSERT
            Assert.False(actual);
            sut.Verify(x => x.AddSeguimientoTitulacionAcceso(
                It.IsAny<AddSeguimientoTitulacionAccesoUncommitCommand>()), Times.Once);
        }

        #endregion

        #region AddSeguimientoTitulacionAcceso

        [Fact(DisplayName = "Cuando la titulación de acceso del expediente es null Agrega el seguimiento")]
        public void AddSeguimientoTitulacionAcceso_Expediente_TitulacionAcceso_Null()
        {
            //ARRANGE
            var mockIIdentityService = new Mock<IIdentityService>
            {
                CallBase = true
            };
            var sut = new Mock<AddSeguimientoTitulacionAccesoUncommitCommandHandler>(mockIIdentityService.Object)
            {
                CallBase = true
            };
            sut.Setup(x => x.AgregarSeguimiento(It.IsAny<ExpedienteAlumno>(),
                It.IsAny<bool>(), It.IsAny<string>()));

            var expedienteAlumno = new ExpedienteAlumno();
            var request = new AddSeguimientoTitulacionAccesoUncommitCommand(expedienteAlumno, false, new TitulacionAccesoParametersDto());

            //ACT
            sut.Object.AddSeguimientoTitulacionAcceso(request);

            //ASSERT
            Assert.Null(expedienteAlumno.TitulacionAcceso);
            sut.Verify(x => x.AgregarSeguimiento(It.IsAny<ExpedienteAlumno>(),
                It.IsAny<bool>(), It.IsAny<string>()), Times.Once);
        }

        [Fact(DisplayName = "Cuando la titulación de acceso no ha sido modificada Termina el proceso")]
        public void AddSeguimientoTitulacionAcceso_NoModifico_TitulacionAcceso()
        {
            //ARRANGE
            var mockIIdentityService = new Mock<IIdentityService>
            {
                CallBase = true
            };
            var sut = new Mock<AddSeguimientoTitulacionAccesoUncommitCommandHandler>(mockIIdentityService.Object)
            {
                CallBase = true
            };
            var expedienteAlumno = new ExpedienteAlumno
            {
                TitulacionAcceso = new TitulacionAcceso()
            };
            var request = new AddSeguimientoTitulacionAccesoUncommitCommand(expedienteAlumno, false, new TitulacionAccesoParametersDto());

            //ACT
            sut.Object.AddSeguimientoTitulacionAcceso(request);

            //ASSERT
            sut.Verify(x => x.AgregarSeguimiento(It.IsAny<ExpedienteAlumno>(),
                It.IsAny<bool>(), It.IsAny<string>()), Times.Never);
        }

        [Fact(DisplayName = "Cuando la titulación de acceso se ha modificado Retorna Ok")]
        public void AddSeguimientoTitulacionAcceso_Modifico_TitulacionAcceso_Ok()
        {
            //ARRANGE
            var mockIIdentityService = new Mock<IIdentityService>
            {
                CallBase = true
            };
            var sut = new Mock<AddSeguimientoTitulacionAccesoUncommitCommandHandler>(mockIIdentityService.Object)
            {
                CallBase = true
            };
            sut.Setup(x => x.AgregarSeguimiento(It.IsAny<ExpedienteAlumno>(),
                It.IsAny<bool>(), It.IsAny<string>()));

            var expedienteAlumno = new ExpedienteAlumno
            {
                TitulacionAcceso = new TitulacionAcceso
                {
                    Titulo = Guid.NewGuid().ToString(),
                    InstitucionDocente = Guid.NewGuid().ToString(),
                    NroSemestreRealizados = 5
                }
            };
            var request = new AddSeguimientoTitulacionAccesoUncommitCommand(expedienteAlumno, false, new TitulacionAccesoParametersDto
            {
                Titulo = Guid.NewGuid().ToString(),
                InstitucionDocente = Guid.NewGuid().ToString(),
                NroSemestreRealizados = 10
            });

            //ACT
            sut.Object.AddSeguimientoTitulacionAcceso(request);

            //ASSERT
            sut.Verify(x => x.AgregarSeguimiento(It.IsAny<ExpedienteAlumno>(),
                It.IsAny<bool>(), It.IsAny<string>()), Times.Once);
        }

        #endregion

        #region AgregarSeguimiento

        [Fact(DisplayName = "Cuando se agrega el seguimiento por integración Retorna Ok")]
        public void AgregarSeguimiento_PorIntegracionTrue_Ok()
        {
            //ARRANGE
            var mockExpedienteAlumno = new Mock<ExpedienteAlumno>
            {
                CallBase = true
            };
            mockExpedienteAlumno.SetupAllProperties();
            mockExpedienteAlumno.Setup(x => x.AddSeguimientoNoUser(It.IsAny<int>(), It.IsAny<string>(), null,
                    It.IsAny<string>(), It.IsAny<string>()));
            var expedienteAlumno = mockExpedienteAlumno.Object;

            var mockIIdentityService = new Mock<IIdentityService>
            {
                CallBase = true
            };
            var sut = new Mock<AddSeguimientoTitulacionAccesoUncommitCommandHandler>(mockIIdentityService.Object)
            {
                CallBase = true
            };
            var descripcionSeguimiento = Guid.NewGuid().ToString();

            //ACT
            sut.Object.AgregarSeguimiento(expedienteAlumno, true, descripcionSeguimiento);

            //ASSERT
            mockExpedienteAlumno.Verify(x => x.AddSeguimientoNoUser(It.IsAny<int>(), It.IsAny<string>(), null,
                    It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [Fact(DisplayName = "Cuando se agrega el seguimiento y no es por integración Retorna Ok")]
        public void AgregarSeguimiento_PorIntegracionFalse_Ok()
        {
            //ARRANGE
            var mockExpedienteAlumno = new Mock<ExpedienteAlumno>
            {
                CallBase = true
            };
            mockExpedienteAlumno.SetupAllProperties();
            mockExpedienteAlumno.Setup(x => x.AddSeguimiento(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), null,
                    It.IsAny<string>(), It.IsAny<string>()));
            var expedienteAlumno = mockExpedienteAlumno.Object;

            var mockIIdentityService = new Mock<IIdentityService>
            {
                CallBase = true
            };
            mockIIdentityService.Setup(s => s.GetUserIdentityInfo()).Returns(new IdentityModel
            {
                Id = Guid.NewGuid().ToString()
            });
            var sut = new Mock<AddSeguimientoTitulacionAccesoUncommitCommandHandler>(mockIIdentityService.Object)
            {
                CallBase = true
            };
            var descripcionSeguimiento = Guid.NewGuid().ToString();

            //ACT
            sut.Object.AgregarSeguimiento(expedienteAlumno, false, descripcionSeguimiento);

            //ASSERT
            mockExpedienteAlumno.Verify(x => x.AddSeguimiento(It.IsAny<int>(),
                It.IsAny<string>(), It.IsAny<string>(), null,
                    It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            mockIIdentityService.Verify(s => s.GetUserIdentityInfo(), Times.Once);
        }

        #endregion
    }
}
