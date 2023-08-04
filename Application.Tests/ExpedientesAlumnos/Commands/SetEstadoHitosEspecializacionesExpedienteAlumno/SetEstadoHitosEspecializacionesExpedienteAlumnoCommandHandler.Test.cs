using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Localization;
using Unir.Expedientes.Application.Common.Interfaces;
using Unir.Expedientes.Application.Common.Models.ErpAcademico.PlanesEstudio;
using Unir.Expedientes.Application.Common.Models.ErpAcademico.Titulos;
using Unir.Expedientes.Application.Common.Models.ExpedientesGestorUnir;
using Unir.Expedientes.Application.Common.Models.Results;
using Unir.Expedientes.Application.Common.Parameters;
using Unir.Expedientes.Application.ExpedientesAlumnos.Commands.SetEstadoHitosEspecializacionesExpedienteAlumno;
using Unir.Expedientes.Application.ExpedientesAlumnos.Queries.CanQualifyAlumnoInPlanEstudio;
using Unir.Expedientes.Application.Tests.Common;
using Unir.Expedientes.Domain.Entities;
using Unir.Framework.ApplicationSuperTypes.Exceptions;
using Xunit;

namespace Unir.Expedientes.Application.Tests.ExpedientesAlumnos.Commands.SetEstadoHitosEspecializacionesExpedienteAlumno
{
    [Collection("CommonTestCollection")]
    public class SetEstadoHitosEspecializacionesExpedienteAlumnoCommandHandlerTest : TestBase
    {
        #region Handle

        [Fact(DisplayName = "Cuando actualiza el estado de hitos Retorna Ok")]
        public async Task Handle_ActualizaEstado_Ok()
        {
            //ARRANGE
            var expedienteAlumno = new ExpedienteAlumno
            {
                IdRefIntegracionAlumno = "123456",
                IdRefPlan = "44",
                HitosConseguidos = new List<HitoConseguido>(),
                Seguimientos = new List<SeguimientoExpediente>()
            };

            var request = new SetEstadoHitosEspecializacionesExpedienteAlumnoCommand(expedienteAlumno);
            
            var mockExpedientesGestorUnirServiceClient = new Mock<IExpedientesGestorUnirServiceClient> { CallBase = true };
            var mockLocalizer = new Mock<IStringLocalizer<SetEstadoHitosEspecializacionesExpedienteAlumnoCommandHandler>>() { CallBase = true };
            var mockErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient> { CallBase = true };

            var expedienteGestor = new ResultPrimitiveCustomValue<ExpedienteExpedientesIntegrationModel>
            {
                Errors = new List<string>(),
                Value = new ExpedienteExpedientesIntegrationModel()
            };
            mockExpedientesGestorUnirServiceClient.Setup(s =>
                    s.GetExpedienteGestorFormatoErp(It.Is<string>(i => i == request.ExpedienteAlumno.IdRefIntegracionAlumno),
                        It.Is<int>(i => i == int.Parse(request.ExpedienteAlumno.IdRefPlan))))
                .ReturnsAsync(expedienteGestor);

            var sut = new Mock<SetEstadoHitosEspecializacionesExpedienteAlumnoCommandHandler>(Context,
                mockExpedientesGestorUnirServiceClient.Object, mockLocalizer.Object,
                mockErpAcademicoServiceClient.Object)
            {
                CallBase = true
            };
            sut.Setup(s => s.GetEstadoExpedienteAsync(It.IsAny<ExpedienteExpedientesIntegrationModel>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new EstadoExpediente
                {
                    Id = 1,
                    Nombre = Guid.NewGuid().ToString()
                });
            sut.Setup(s => s.AssignExpedienteAlumno(It.IsAny<ExpedienteAlumno>(), It.IsAny<ExpedienteExpedientesIntegrationModel>()));
            sut.Setup(s => s.AddHitosConseguidos(It.IsAny<ExpedienteAlumno>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
            sut.Setup(s => s.AddEspecializacionesHitosAsync(It.IsAny<ExpedienteAlumno>(),
                    It.IsAny<List<HitoErpAcademicoModel>>(), It.IsAny<ExpedienteExpedientesIntegrationModel>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync("- Especialización");
            sut.Setup(s => s.AddSeguimientoExpediente(expedienteAlumno, "- Especialización"));

            //ACT
            await sut.Object.Handle(request, CancellationToken.None);

            //ASSERT
            mockExpedientesGestorUnirServiceClient.Verify(s =>
                s.GetExpedienteGestorFormatoErp(
                    It.Is<string>(i => i == request.ExpedienteAlumno.IdRefIntegracionAlumno),
                    It.Is<int>(i => i == int.Parse(request.ExpedienteAlumno.IdRefPlan))), Times.Once);
            sut.Verify(s => s.GetEstadoExpedienteAsync(It.IsAny<ExpedienteExpedientesIntegrationModel>(), It.IsAny<CancellationToken>()), Times.Once);
            sut.Verify(s => s.AssignExpedienteAlumno(It.IsAny<ExpedienteAlumno>(), It.IsAny<ExpedienteExpedientesIntegrationModel>()), Times.Once);
            sut.Verify(s => s.AddHitosConseguidos(It.IsAny<ExpedienteAlumno>(), It.IsAny<CancellationToken>()), Times.Once);
            sut.Verify(s => s.AddEspecializacionesHitosAsync(It.IsAny<ExpedienteAlumno>(),
                It.IsAny<List<HitoErpAcademicoModel>>(), It.IsAny<ExpedienteExpedientesIntegrationModel>(),
                It.IsAny<CancellationToken>()), Times.Once);
            sut.Verify(s => s.AddSeguimientoExpediente(It.IsAny<ExpedienteAlumno>(), It.IsAny<string>()), Times.Once);
            Assert.Single(expedienteAlumno.Seguimientos);
        }

        [Fact(DisplayName = "Cuando no encuentra el expediente gestor para el idRefAlumno y el idRefPlan Devuelve Error")]
        public async Task Handle_SinExpedienteGestor()
        {
            //ARRANGE
            var expedienteAlumno = new ExpedienteAlumno
            {
                IdRefIntegracionAlumno = "123456",
                IdRefPlan = "44",
                HitosConseguidos = new List<HitoConseguido>(),
                Seguimientos = new List<SeguimientoExpediente>()
            };

            var request = new SetEstadoHitosEspecializacionesExpedienteAlumnoCommand(expedienteAlumno);

            var erroresExpedienteGestor = Enumerable.Range(1, 2).Select(i => $"{Guid.NewGuid()}{i}").ToList();
            var mensajeEsperado =
                $"[Expedientes Gestor]: { string.Join(", ", erroresExpedienteGestor)}.";

            var mockExpedientesGestorUnirServiceClient = new Mock<IExpedientesGestorUnirServiceClient> { CallBase = true };
            var mockLocalizer = new Mock<IStringLocalizer<SetEstadoHitosEspecializacionesExpedienteAlumnoCommandHandler>>() { CallBase = true };
            mockLocalizer.Setup(s => s[It.Is<string>(msj => msj == mensajeEsperado)])
                .Returns(new LocalizedString(mensajeEsperado, mensajeEsperado));
            var mockErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient> { CallBase = true };

            var expedienteGestor = new ResultPrimitiveCustomValue<ExpedienteExpedientesIntegrationModel>
            {
                Errors = erroresExpedienteGestor,
                Value = new ExpedienteExpedientesIntegrationModel()
            };
            mockExpedientesGestorUnirServiceClient.Setup(s =>
                    s.GetExpedienteGestorFormatoErp(It.Is<string>(i => i == request.ExpedienteAlumno.IdRefIntegracionAlumno),
                        It.Is<int>(i => i == int.Parse(request.ExpedienteAlumno.IdRefPlan))))
                .ReturnsAsync(expedienteGestor);

            var sut = new Mock<SetEstadoHitosEspecializacionesExpedienteAlumnoCommandHandler>(Context,
                mockExpedientesGestorUnirServiceClient.Object, mockLocalizer.Object,
                mockErpAcademicoServiceClient.Object)
            {
                CallBase = true
            };
            sut.Setup(s => s.GetEstadoExpedienteAsync(It.IsAny<ExpedienteExpedientesIntegrationModel>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new EstadoExpediente
                {
                    Id = 1,
                    Nombre = Guid.NewGuid().ToString()
                });
            sut.Setup(s => s.AssignExpedienteAlumno(It.IsAny<ExpedienteAlumno>(), It.IsAny<ExpedienteExpedientesIntegrationModel>()));
            sut.Setup(s => s.GetTipoHitoByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new TipoHitoConseguido
                {
                    Nombre = Guid.NewGuid().ToString(),
                    HitosConseguidos = new List<HitoConseguido>()
                });
            sut.Setup(s => s.AddHitosConseguidos(It.IsAny<ExpedienteAlumno>(), It.IsAny<CancellationToken>()));
            sut.Setup(s => s.AddEspecializacionesHitosAsync(It.IsAny<ExpedienteAlumno>(),
                    It.IsAny<List<HitoErpAcademicoModel>>(), It.IsAny<ExpedienteExpedientesIntegrationModel>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync("- Especialización");
            sut.Setup(s => s.AddSeguimientoExpediente(expedienteAlumno, "- Especialización"));

            //ACT
            var ex = await Record.ExceptionAsync(async () =>
            {
                await sut.Object.Handle(request, CancellationToken.None);
            });

            //ASSERT
            mockExpedientesGestorUnirServiceClient.Verify(s =>
                s.GetExpedienteGestorFormatoErp(
                    It.Is<string>(i => i == request.ExpedienteAlumno.IdRefIntegracionAlumno),
                    It.Is<int>(i => i == int.Parse(request.ExpedienteAlumno.IdRefPlan))), Times.Once);
            sut.Verify(
                s => s.GetEstadoExpedienteAsync(It.IsAny<ExpedienteExpedientesIntegrationModel>(),
                    It.IsAny<CancellationToken>()), Times.Never);
            sut.Verify(
                s => s.AssignExpedienteAlumno(It.IsAny<ExpedienteAlumno>(),
                    It.IsAny<ExpedienteExpedientesIntegrationModel>()), Times.Never);
            sut.Verify(s => s.GetTipoHitoByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
            sut.Verify(s => s.AddHitosConseguidos(It.IsAny<ExpedienteAlumno>(), It.IsAny<CancellationToken>()),
                Times.Never);
            sut.Verify(s => s.AddEspecializacionesHitosAsync(It.IsAny<ExpedienteAlumno>(),
                It.IsAny<List<HitoErpAcademicoModel>>(), It.IsAny<ExpedienteExpedientesIntegrationModel>(),
                It.IsAny<CancellationToken>()), Times.Never);
            sut.Verify(s => s.AddSeguimientoExpediente(It.IsAny<ExpedienteAlumno>(), It.IsAny<string>()), Times.Never);
            Assert.IsType<BadRequestException>(ex);
            Assert.Contains(mensajeEsperado, ex.Message);
        }

        #endregion

        #region AddHitosConseguidos

        [Fact(DisplayName = "Cuando es prematriculado Retorna Ok")]
        public async Task AddHitosConseguidos_Prematriculado_Ok()
        {
            //ARRANGE
            var mockExpedientesGestorUnirServiceClient = new Mock<IExpedientesGestorUnirServiceClient> { CallBase = true };
            var mockLocalizer = new Mock<IStringLocalizer<SetEstadoHitosEspecializacionesExpedienteAlumnoCommandHandler>>();
            var mockErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient> { CallBase = true };
            mockErpAcademicoServiceClient
                .Setup(s => s.GetFirstFechaPrimerMatricula(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(DateTime.Now);

            var expedienteAlumno = new ExpedienteAlumno
            {
                Id = 1,
                HitosConseguidos = new List<HitoConseguido>(),
                Estado = new EstadoExpediente()
            };
            var tipoHito = new TipoHitoConseguido
            {
                Id = 1,
                HitosConseguidos = new List<HitoConseguido>(),
                Nombre = Guid.NewGuid().ToString()
            };
            var hitoConseguido = new HitoConseguido
            {
                Id = 1,
                ExpedienteEspecializacion = new ExpedienteEspecializacion(),
                TipoConseguido = tipoHito,
                Nombre = Guid.NewGuid().ToString(),
                FechaInicio = DateTime.MinValue
            };

            var sut = new Mock<SetEstadoHitosEspecializacionesExpedienteAlumnoCommandHandler>(Context,
                mockExpedientesGestorUnirServiceClient.Object, mockLocalizer.Object,
                mockErpAcademicoServiceClient.Object)
            {
                CallBase = true
            };
            sut.Setup(s => s.GetTipoHitoByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(tipoHito);
            sut.Setup(s => s.AssignNewHitoConseguido(It.IsAny<DateTime>(), It.IsAny<TipoHitoConseguido>()))
                .Returns(hitoConseguido);

            //ACT
            await sut.Object.AddHitosConseguidos(expedienteAlumno, CancellationToken.None);

            //ASSERT
            sut.Verify(s => s.GetTipoHitoByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Once);
            sut.Verify(s => s.AssignNewHitoConseguido(It.IsAny<DateTime>(), It.IsAny<TipoHitoConseguido>()), Times.Once);
            mockErpAcademicoServiceClient
                .Verify(s => s.GetFirstFechaPrimerMatricula(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            Assert.Equal(hitoConseguido, expedienteAlumno.HitosConseguidos.FirstOrDefault());
        }
        [Theory(DisplayName = "Cuando es fecha de expediente alumno Retorna Ok")]
        [InlineData(2)]
        [InlineData(8)]
        [InlineData(10)]
        public async Task AddHitosConseguidos_PrimeraMatricula_Ok(int idTipo)
        {
            //ARRANGE
            var mockExpedientesGestorUnirServiceClient = new Mock<IExpedientesGestorUnirServiceClient> { CallBase = true };
            var mockLocalizer = new Mock<IStringLocalizer<SetEstadoHitosEspecializacionesExpedienteAlumnoCommandHandler>>();
            var mockErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient> { CallBase = true };
            mockErpAcademicoServiceClient
                .Setup(s => s.GetFirstFechaPrimerMatricula(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(DateTime.Now);

            var expedienteAlumno = new ExpedienteAlumno
            {
                Id = 1,
                HitosConseguidos = new List<HitoConseguido>(),
                Estado = new EstadoExpediente(),
                FechaApertura = DateTime.MinValue
            };
            var tipoHito = new TipoHitoConseguido
            {
                Id = idTipo,
                HitosConseguidos = new List<HitoConseguido>(),
                Nombre = Guid.NewGuid().ToString()
            };
            var hitoConseguido = new HitoConseguido
            {
                Id = 1,
                ExpedienteEspecializacion = new ExpedienteEspecializacion(),
                TipoConseguido = tipoHito,
                Nombre = Guid.NewGuid().ToString(),
                FechaInicio = DateTime.MinValue
            };

            var sut = new Mock<SetEstadoHitosEspecializacionesExpedienteAlumnoCommandHandler>(Context,
                mockExpedientesGestorUnirServiceClient.Object, mockLocalizer.Object,
                mockErpAcademicoServiceClient.Object)
            {
                CallBase = true
            };
            sut.Setup(s => s.GetTipoHitoByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(tipoHito);
            sut.Setup(s => s.AssignNewHitoConseguido(It.IsAny<DateTime>(), It.IsAny<TipoHitoConseguido>()))
                .Returns(hitoConseguido);

            //ACT
            await sut.Object.AddHitosConseguidos(expedienteAlumno, CancellationToken.None);

            //ASSERT
            sut.Verify(s => s.GetTipoHitoByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
            sut.Verify(s => s.AssignNewHitoConseguido(It.IsAny<DateTime>(), It.IsAny<TipoHitoConseguido>()), Times.Exactly(2));
            mockErpAcademicoServiceClient
                .Verify(s => s.GetFirstFechaPrimerMatricula(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            Assert.Equal(hitoConseguido, expedienteAlumno.HitosConseguidos.First());
        }

        [Fact(DisplayName = "Cuando es expediente cerrado con fecha de finalización Retorna Ok")]
        public async Task AddHitosConseguidos_Cerrado_Ok()
        {
            //ARRANGE
            var mockExpedientesGestorUnirServiceClient = new Mock<IExpedientesGestorUnirServiceClient> { CallBase = true };
            var mockLocalizer = new Mock<IStringLocalizer<SetEstadoHitosEspecializacionesExpedienteAlumnoCommandHandler>>();
            var mockErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient> { CallBase = true };
            mockErpAcademicoServiceClient
                .Setup(s => s.GetFirstFechaPrimerMatricula(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync((DateTime?)null);

            var expedienteAlumno = new ExpedienteAlumno
            {
                Id = 1,
                HitosConseguidos = new List<HitoConseguido>(),
                Estado = new EstadoExpediente
                {
                    Id = 3
                },
                FechaFinalizacion = DateTime.MaxValue
            };
            var tipoHito = new TipoHitoConseguido
            {
                Id = 1,
                HitosConseguidos = new List<HitoConseguido>(),
                Nombre = Guid.NewGuid().ToString()
            };
            var hitoConseguido = new HitoConseguido
            {
                Id = 1,
                ExpedienteEspecializacion = new ExpedienteEspecializacion(),
                TipoConseguido = tipoHito,
                Nombre = Guid.NewGuid().ToString(),
                FechaInicio = DateTime.MinValue
            };

            var sut = new Mock<SetEstadoHitosEspecializacionesExpedienteAlumnoCommandHandler>(Context,
                mockExpedientesGestorUnirServiceClient.Object, mockLocalizer.Object,
                mockErpAcademicoServiceClient.Object)
            {
                CallBase = true
            };
            sut.Setup(s => s.GetTipoHitoByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(tipoHito);
            sut.Setup(s => s.AssignNewHitoConseguido(It.IsAny<DateTime>(), It.IsAny<TipoHitoConseguido>()))
                .Returns(hitoConseguido);

            //ACT
            await sut.Object.AddHitosConseguidos(expedienteAlumno, CancellationToken.None);

            //ASSERT
            sut.Verify(s => s.GetTipoHitoByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
            sut.Verify(s => s.AssignNewHitoConseguido(It.IsAny<DateTime>(), It.IsAny<TipoHitoConseguido>()), Times.Exactly(1));
            mockErpAcademicoServiceClient
                .Verify(s => s.GetFirstFechaPrimerMatricula(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            Assert.Equal(hitoConseguido, expedienteAlumno.HitosConseguidos.FirstOrDefault());
        }

        [Fact(DisplayName = "Cuando es expediente con trabajo fin de estudio Retorna Ok")]
        public async Task AddHitosConseguidos_TrabajajoFinEstudio_Ok()
        {
            //ARRANGE
            var mockExpedientesGestorUnirServiceClient = new Mock<IExpedientesGestorUnirServiceClient> { CallBase = true };
            var mockLocalizer = new Mock<IStringLocalizer<SetEstadoHitosEspecializacionesExpedienteAlumnoCommandHandler>>();
            var mockErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient> { CallBase = true };
            mockErpAcademicoServiceClient
                .Setup(s => s.GetFirstFechaPrimerMatricula(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(DateTime.Now);

            var expedienteAlumno = new ExpedienteAlumno
            {
                Id = 1,
                HitosConseguidos = new List<HitoConseguido>(),
                Estado = new EstadoExpediente
                {
                    Id = 3
                },
                FechaTrabajoFinEstudio = DateTime.MaxValue,
            };
            var tipoHito = new TipoHitoConseguido
            {
                Id = 1,
                HitosConseguidos = new List<HitoConseguido>(),
                Nombre = Guid.NewGuid().ToString()
            };
            var hitoConseguido = new HitoConseguido
            {
                Id = 1,
                ExpedienteEspecializacion = new ExpedienteEspecializacion(),
                TipoConseguido = tipoHito,
                Nombre = Guid.NewGuid().ToString(),
                FechaInicio = DateTime.MinValue
            };

            var sut = new Mock<SetEstadoHitosEspecializacionesExpedienteAlumnoCommandHandler>(Context,
                mockExpedientesGestorUnirServiceClient.Object, mockLocalizer.Object,
                mockErpAcademicoServiceClient.Object)
            {
                CallBase = true
            };
            sut.Setup(s => s.GetTipoHitoByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(tipoHito);
            sut.Setup(s => s.AssignNewHitoConseguido(It.IsAny<DateTime>(), It.IsAny<TipoHitoConseguido>()))
                .Returns(hitoConseguido);

            //ACT
            await sut.Object.AddHitosConseguidos(expedienteAlumno, CancellationToken.None);

            //ASSERT
            sut.Verify(s => s.GetTipoHitoByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
            sut.Verify(s => s.AssignNewHitoConseguido(It.IsAny<DateTime>(), It.IsAny<TipoHitoConseguido>()), Times.Exactly(2));
            mockErpAcademicoServiceClient
                .Verify(s => s.GetFirstFechaPrimerMatricula(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            Assert.Equal(hitoConseguido, expedienteAlumno.HitosConseguidos.FirstOrDefault());
        }

        [Fact(DisplayName = "Cuando es expediente con tasas abonadas Retorna Ok")]
        public async Task AddHitosConseguidos_TasasAbonadas_Ok()
        {
            //ARRANGE
            var mockExpedientesGestorUnirServiceClient = new Mock<IExpedientesGestorUnirServiceClient> { CallBase = true };
            var mockLocalizer = new Mock<IStringLocalizer<SetEstadoHitosEspecializacionesExpedienteAlumnoCommandHandler>>();
            var mockErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient> { CallBase = true };
            mockErpAcademicoServiceClient
                .Setup(s => s.GetFirstFechaPrimerMatricula(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(DateTime.Now);

            var expedienteAlumno = new ExpedienteAlumno
            {
                Id = 1,
                HitosConseguidos = new List<HitoConseguido>(),
                Estado = new EstadoExpediente
                {
                    Id = 3
                },
                FechaExpedicion = DateTime.MaxValue,
                IdRefIntegracionAlumno = "25",
                IdRefPlan = "25"
            };
            var tipoHito = new TipoHitoConseguido
            {
                Id = 1,
                HitosConseguidos = new List<HitoConseguido>(),
                Nombre = Guid.NewGuid().ToString()
            };
            var hitoConseguido = new HitoConseguido
            {
                Id = 1,
                ExpedienteEspecializacion = new ExpedienteEspecializacion(),
                TipoConseguido = tipoHito,
                Nombre = Guid.NewGuid().ToString(),
                FechaInicio = DateTime.MinValue
            };

            var sut = new Mock<SetEstadoHitosEspecializacionesExpedienteAlumnoCommandHandler>(Context,
                mockExpedientesGestorUnirServiceClient.Object, mockLocalizer.Object,
                mockErpAcademicoServiceClient.Object)
            {
                CallBase = true
            };
            sut.Setup(s => s.GetTipoHitoByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(tipoHito);
            sut.Setup(s => s.AssignNewHitoConseguido(It.IsAny<DateTime>(), It.IsAny<TipoHitoConseguido>()))
                .Returns(hitoConseguido);

            //ACT
            await sut.Object.AddHitosConseguidos(expedienteAlumno, CancellationToken.None);

            //ASSERT
            sut.Verify(s => s.GetTipoHitoByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
            sut.Verify(s => s.AssignNewHitoConseguido(It.IsAny<DateTime>(), It.IsAny<TipoHitoConseguido>()), Times.Exactly(2));
            mockErpAcademicoServiceClient
                .Verify(s => s.GetFirstFechaPrimerMatricula(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            Assert.Equal(hitoConseguido, expedienteAlumno.HitosConseguidos.FirstOrDefault());
        }

        #endregion

        #region AddEspecializacionesHitosAsync

        [Fact(DisplayName = "Cuando no contiene hitos obtenidos Retorna Ok")]
        public async Task AddEspecializaciones_SinHitos_Ok()
        {
            //ARRANGE
            var mockExpedientesGestorUnirServiceClient = new Mock<IExpedientesGestorUnirServiceClient> { CallBase = true };
            var mockLocalizer = new Mock<IStringLocalizer<SetEstadoHitosEspecializacionesExpedienteAlumnoCommandHandler>>();
            var mockErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient> { CallBase = true };

            var expedienteAlumno = new ExpedienteAlumno();
            var hitosConseguidos = new List<HitoErpAcademicoModel>();

            var sut = new SetEstadoHitosEspecializacionesExpedienteAlumnoCommandHandler(Context,
                mockExpedientesGestorUnirServiceClient.Object, mockLocalizer.Object,
                mockErpAcademicoServiceClient.Object);

            //ACT
            var actual = await sut.AddEspecializacionesHitosAsync(expedienteAlumno, hitosConseguidos,
                new ExpedienteExpedientesIntegrationModel(),
                CancellationToken.None);

            //ASSERT
            Assert.False(expedienteAlumno.HitosConseguidos.Any());
            Assert.False(expedienteAlumno.ExpedientesEspecializaciones.Any());
            Assert.Equal(string.Empty, actual);
        }

        [Fact(DisplayName = "Cuando no contiene especialización Retorna Ok")]
        public async Task AddEspecializaciones_SinEspecializacion_Ok()
        {
            //ARRANGE
            var mockExpedientesGestorUnirServiceClient = new Mock<IExpedientesGestorUnirServiceClient> { CallBase = true };
            var mockLocalizer = new Mock<IStringLocalizer<SetEstadoHitosEspecializacionesExpedienteAlumnoCommandHandler>>();
            var mockErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient> { CallBase = true };
            
            var hitosConseguidos = new List<HitoErpAcademicoModel>
            {
                new HitoErpAcademicoModel()
            };
            
            var sut = new SetEstadoHitosEspecializacionesExpedienteAlumnoCommandHandler(Context,
                mockExpedientesGestorUnirServiceClient.Object, mockLocalizer.Object,
                mockErpAcademicoServiceClient.Object);

            //ACT
            await sut.AddEspecializacionesHitosAsync(new ExpedienteAlumno(), hitosConseguidos,
                new ExpedienteExpedientesIntegrationModel(),
                CancellationToken.None);

            //ASSERT
            Assert.NotNull(hitosConseguidos);
        }

        [Fact(DisplayName = "Cuando no contiene expediente especialización Retorna Ok")]
        public async Task AddEspecializaciones_SinExpedienteEspecializacion_Ok()
        {
            //ARRANGE
            var mockExpedientesGestorUnirServiceClient = new Mock<IExpedientesGestorUnirServiceClient> { CallBase = true };
            var mockLocalizer = new Mock<IStringLocalizer<SetEstadoHitosEspecializacionesExpedienteAlumnoCommandHandler>>();
            var mockErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient> { CallBase = true };
            var expedienteAlumno = new ExpedienteAlumno
            {
                ExpedientesEspecializaciones = new List<ExpedienteEspecializacion>(),
                FechaApertura = DateTime.MaxValue,
                HitosConseguidos = new List<HitoConseguido>()
            };
            
            var sut = new SetEstadoHitosEspecializacionesExpedienteAlumnoCommandHandler(Context,
                mockExpedientesGestorUnirServiceClient.Object, mockLocalizer.Object,
                mockErpAcademicoServiceClient.Object);

            //ACT
            var actual = await sut.AddEspecializacionesHitosAsync(expedienteAlumno, new List<HitoErpAcademicoModel>
                {
                    new HitoErpAcademicoModel
                    {
                        Especializacion = new EspecializacionAcademicoModel()
                    }
                },
                new ExpedienteExpedientesIntegrationModel(),
                CancellationToken.None);

            //ASSERT
            Assert.Single(expedienteAlumno.ExpedientesEspecializaciones);
            Assert.Single(expedienteAlumno.HitosConseguidos);
            Assert.Equal(" - Especialización", actual);

        }
        [Fact(DisplayName = "Cuando no contiene fecha de apertura Retorna Ok")]
        public async Task AddEspecializaciones_SinFechaApertura_Ok()
        {
            //ARRANGE
            var mockExpedientesGestorUnirServiceClient = new Mock<IExpedientesGestorUnirServiceClient> { CallBase = true };
            var mockLocalizer = new Mock<IStringLocalizer<SetEstadoHitosEspecializacionesExpedienteAlumnoCommandHandler>>();
            var mockErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient> { CallBase = true };

            var expedienteAlumno = new ExpedienteAlumno
            {
                ExpedientesEspecializaciones = new List<ExpedienteEspecializacion>(),
            };

            var sut = new SetEstadoHitosEspecializacionesExpedienteAlumnoCommandHandler(Context,
                mockExpedientesGestorUnirServiceClient.Object, mockLocalizer.Object,
                mockErpAcademicoServiceClient.Object);

            //ACT
            var actual = await sut.AddEspecializacionesHitosAsync(expedienteAlumno, new List<HitoErpAcademicoModel>
                {
                    new HitoErpAcademicoModel
                    {
                        Especializacion = new EspecializacionAcademicoModel()
                    }
                }, new ExpedienteExpedientesIntegrationModel(),
                CancellationToken.None);

            //ASSERT
            Assert.Single(expedienteAlumno.ExpedientesEspecializaciones);
            Assert.False(expedienteAlumno.HitosConseguidos.Any());
            Assert.Equal(" - Especialización", actual);
        }
        [Fact(DisplayName = "Cuando contiene fecha de apertura Retorna Ok")]
        public async Task AddEspecializaciones_FechaApertura_Ok()
        {
            //ARRANGE
            var mockExpedientesGestorUnirServiceClient = new Mock<IExpedientesGestorUnirServiceClient> { CallBase = true };
            var mockLocalizer = new Mock<IStringLocalizer<SetEstadoHitosEspecializacionesExpedienteAlumnoCommandHandler>>();
            var mockErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient> { CallBase = true };
            var hitosConseguidos = new List<HitoErpAcademicoModel>
            {
                new HitoErpAcademicoModel
                {
                    Especializacion = new EspecializacionAcademicoModel
                    {
                        Id = 1
                    }
                }
            };
            var expedienteAlumno = new ExpedienteAlumno
            {
                Id = 1,
                ExpedientesEspecializaciones = new List<ExpedienteEspecializacion>
                {
                    new ExpedienteEspecializacion()
                },
                FechaApertura = DateTime.MaxValue,
                HitosConseguidos = new List<HitoConseguido>()
            };

            await Context.AddAsync(new ExpedienteEspecializacion
            {
                ExpedienteAlumno = new ExpedienteAlumno
                {
                    Id = 1
                },
                Id = 2,
                HitosConseguidos = new List<HitoConseguido>(),
                IdRefEspecializacion = "1",
                ExpedienteAlumnoId = 1
            });
            await Context.SaveChangesAsync();

            var sut = new Mock<SetEstadoHitosEspecializacionesExpedienteAlumnoCommandHandler>(Context,
                mockExpedientesGestorUnirServiceClient.Object, mockLocalizer.Object,
                mockErpAcademicoServiceClient.Object)
            {
                CallBase = true
            };
            sut.Setup(s => s.GetTipoHitoByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new TipoHitoConseguido
                {
                    Nombre = Guid.NewGuid().ToString(),
                    HitosConseguidos = new List<HitoConseguido>()
                });

            //ACT
            var actual = await sut.Object.AddEspecializacionesHitosAsync(expedienteAlumno, hitosConseguidos, new ExpedienteExpedientesIntegrationModel(),
                CancellationToken.None);

            //ASSERT
            Assert.Single(expedienteAlumno.HitosConseguidos);
            Assert.Single(expedienteAlumno.ExpedientesEspecializaciones);
            Assert.Equal(string.Empty, actual);
            sut.Verify(s => s.GetTipoHitoByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        #endregion

        #region AssignNewHitoConseguido

        [Fact(DisplayName = "Cuando devuelve hito conseguido Retorna Ok")]
        public void GetHitoConseguido_HitoConseguido_ok()
        {
            //ARRANGE
            var mockExpedientesGestorUnirServiceClient = new Mock<IExpedientesGestorUnirServiceClient> { CallBase = true };
            var mockLocalizer = new Mock<IStringLocalizer<SetEstadoHitosEspecializacionesExpedienteAlumnoCommandHandler>>();
            var mockErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient> { CallBase = true };

            var tipoHito = new TipoHitoConseguido { Nombre = Guid.NewGuid().ToString() };
            var fechaFinalizacion = DateTime.MaxValue;
            
            var sut = new SetEstadoHitosEspecializacionesExpedienteAlumnoCommandHandler(Context,
                mockExpedientesGestorUnirServiceClient.Object, mockLocalizer.Object,
                mockErpAcademicoServiceClient.Object);

            //ACT
            var actual = sut.AssignNewHitoConseguido(fechaFinalizacion, tipoHito);

            //ASSERT
            Assert.NotNull(actual);
            Assert.IsType<HitoConseguido>(actual);
            Assert.Equal(tipoHito, actual.TipoConseguido);
            Assert.Equal(tipoHito.Nombre, actual.Nombre);
            Assert.Equal(fechaFinalizacion, actual.FechaInicio);
        }

        #endregion

        #region GetTipoHitoByIdAsync

        [Fact(DisplayName = "Cuando no contiene tipos hito conseguidos Retorna ok")]
        public async Task GetTipoHitoById_SinTiposHito_Ok()
        {
            //ARRANGE
            var mockExpedientesGestorUnirServiceClient = new Mock<IExpedientesGestorUnirServiceClient> { CallBase = true };
            var mockLocalizer = new Mock<IStringLocalizer<SetEstadoHitosEspecializacionesExpedienteAlumnoCommandHandler>>();
            var mockErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient> { CallBase = true };

            var sut = new SetEstadoHitosEspecializacionesExpedienteAlumnoCommandHandler(Context,
                mockExpedientesGestorUnirServiceClient.Object, mockLocalizer.Object,
                mockErpAcademicoServiceClient.Object);

            //ACT
            var actual = await sut.GetTipoHitoByIdAsync(1, CancellationToken.None);

            //ASSERT
            Assert.Null(actual);
        }
        [Fact(DisplayName = "Cuando contiene tipos hito conseguidos Retorna ok")]
        public async Task GetTipoHitoById_TiposHito_Ok()
        {
            //ARRANGE
            var mockExpedientesGestorUnirServiceClient = new Mock<IExpedientesGestorUnirServiceClient> { CallBase = true };
            var mockLocalizer = new Mock<IStringLocalizer<SetEstadoHitosEspecializacionesExpedienteAlumnoCommandHandler>>();
            var mockErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient> { CallBase = true };

            await Context.AddRangeAsync(Enumerable.Range(1, 3).Select(s => new TipoHitoConseguido { Id = s }));
            await Context.SaveChangesAsync();
            
            var sut = new SetEstadoHitosEspecializacionesExpedienteAlumnoCommandHandler(Context,
                mockExpedientesGestorUnirServiceClient.Object, mockLocalizer.Object,
                mockErpAcademicoServiceClient.Object);

            //ACT
            var actual = await sut.GetTipoHitoByIdAsync(2, CancellationToken.None);

            //ASSERT
            Assert.NotNull(actual);
            Assert.IsType<TipoHitoConseguido>(actual);
        }

        #endregion

        #region AssignExpedienteAlumno

        [Fact(DisplayName = "Cuando expedición es null Retorna ok")]
        public void AssignExpedienteAlumno_SinExpedicion_Ok()
        {
            //ARRANGE
            var expedienteAlumno = new ExpedienteAlumno();

            var mockExpedientesGestorUnirServiceClient = new Mock<IExpedientesGestorUnirServiceClient> { CallBase = true };
            var mockLocalizer = new Mock<IStringLocalizer<SetEstadoHitosEspecializacionesExpedienteAlumnoCommandHandler>>();
            var mockErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient> { CallBase = true };

            var sut = new SetEstadoHitosEspecializacionesExpedienteAlumnoCommandHandler(Context,
                mockExpedientesGestorUnirServiceClient.Object, mockLocalizer.Object,
                mockErpAcademicoServiceClient.Object);

            //ACT
            sut.AssignExpedienteAlumno(expedienteAlumno, null);

            //ASSERT
            Assert.Null(expedienteAlumno.FechaFinalizacion);
            Assert.Null(expedienteAlumno.FechaExpedicion);
            Assert.Null(expedienteAlumno.FechaTrabajoFinEstudio);
            Assert.Null(expedienteAlumno.TituloTrabajoFinEstudio);
            Assert.Null(expedienteAlumno.NotaMedia);
        }
        [Fact(DisplayName = "Cuando contiene expedición Retorna ok")]
        public void AssignExpedienteAlumno_Expedicion_Ok()
        {
            //ARRANGE
            var expedienteAlumno = new ExpedienteAlumno();

            var expedicion = new ExpedienteExpedientesIntegrationModel
            {
                FechaFinEstudio = DateTime.MaxValue,
                FechaExpedicion = DateTime.MaxValue,
                FechaTfmTfg = DateTime.MaxValue,
                TituloTfmTfg = Guid.NewGuid().ToString(),
                NotaMedia = 5.5
            };

            var mockExpedientesGestorUnirServiceClient = new Mock<IExpedientesGestorUnirServiceClient> { CallBase = true };
            var mockLocalizer = new Mock<IStringLocalizer<SetEstadoHitosEspecializacionesExpedienteAlumnoCommandHandler>>();
            var mockErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient> { CallBase = true };
            
            var sut = new SetEstadoHitosEspecializacionesExpedienteAlumnoCommandHandler(Context,
                mockExpedientesGestorUnirServiceClient.Object, mockLocalizer.Object,
                mockErpAcademicoServiceClient.Object);

            //ACT
            sut.AssignExpedienteAlumno(expedienteAlumno, expedicion);

            //ASSERT
            Assert.Equal(expedicion.FechaFinEstudio, expedienteAlumno.FechaFinalizacion);
            Assert.Equal(expedicion.FechaExpedicion, expedienteAlumno.FechaExpedicion);
            Assert.Equal(expedicion.FechaTfmTfg, expedienteAlumno.FechaTrabajoFinEstudio);
            Assert.Equal(expedicion.TituloTfmTfg, expedienteAlumno.TituloTrabajoFinEstudio);
            Assert.Equal(expedicion.NotaMedia, expedienteAlumno.NotaMedia);
        }
        #endregion

        #region GetEstadoExpedienteAsync

        [Fact(DisplayName = "Cuando no contiene estado expediente Retorna ok")]
        public async Task GetEstadoExpediente_SinEstado_Ok()
        {
            //ARRANGE
            var mockExpedientesGestorUnirServiceClient = new Mock<IExpedientesGestorUnirServiceClient> { CallBase = true };
            var mockLocalizer = new Mock<IStringLocalizer<SetEstadoHitosEspecializacionesExpedienteAlumnoCommandHandler>>();
            var mockErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient> { CallBase = true };
            var sut = new SetEstadoHitosEspecializacionesExpedienteAlumnoCommandHandler(Context,
                mockExpedientesGestorUnirServiceClient.Object, mockLocalizer.Object,
                mockErpAcademicoServiceClient.Object);

            //ACT
            var actual = await sut.GetEstadoExpedienteAsync(new ExpedienteExpedientesIntegrationModel(), CancellationToken.None);

            //ASSERT
            Assert.Null(actual);
        }
        [Fact(DisplayName = "Cuando no contiene expedición Retorna estado abierto")]
        public async Task GetEstadoExpediente_SinExpedicion_Abierto()
        {
            //ARRANGE
            var mockExpedientesGestorUnirServiceClient = new Mock<IExpedientesGestorUnirServiceClient> { CallBase = true };
            var mockLocalizer = new Mock<IStringLocalizer<SetEstadoHitosEspecializacionesExpedienteAlumnoCommandHandler>>();
            var mockErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient> { CallBase = true };

            await Context.AddAsync(new EstadoExpediente
            {
                Id = EstadoExpediente.Abierto
            });
            await Context.AddAsync(new EstadoExpediente
            {
                Id = EstadoExpediente.Cerrado
            });
            await Context.SaveChangesAsync();
            
            var sut = new SetEstadoHitosEspecializacionesExpedienteAlumnoCommandHandler(Context,
                mockExpedientesGestorUnirServiceClient.Object, mockLocalizer.Object,
                mockErpAcademicoServiceClient.Object);

            //ACT
            var actual = await sut.GetEstadoExpedienteAsync(new ExpedienteExpedientesIntegrationModel(), CancellationToken.None);

            //ASSERT
            Assert.IsType<EstadoExpediente>(actual);
            Assert.True(actual.Id == EstadoExpediente.Abierto);
        }
        [Fact(DisplayName = "Cuando contiene expedición Retorna estado cerrado")]
        public async Task GetEstadoExpediente_Expedicion_Cerrado()
        {
            //ARRANGE
            var expedicion = new ExpedienteExpedientesIntegrationModel
            {
                FechaFinEstudio = DateTime.MaxValue,
                FechaExpedicion = DateTime.MaxValue,
                FechaTfmTfg = DateTime.MaxValue
            };
            
            var mockExpedientesGestorUnirServiceClient = new Mock<IExpedientesGestorUnirServiceClient> { CallBase = true };
            var mockLocalizer = new Mock<IStringLocalizer<SetEstadoHitosEspecializacionesExpedienteAlumnoCommandHandler>>();
            var mockErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient> { CallBase = true };

            await Context.AddAsync(new EstadoExpediente
            {
                Id = EstadoExpediente.Abierto
            });
            await Context.AddAsync(new EstadoExpediente
            {
                Id = EstadoExpediente.Cerrado
            });
            await Context.SaveChangesAsync();
            
            var sut = new SetEstadoHitosEspecializacionesExpedienteAlumnoCommandHandler(Context,
                mockExpedientesGestorUnirServiceClient.Object, mockLocalizer.Object,
                mockErpAcademicoServiceClient.Object);

            //ACT
            var actual = await sut.GetEstadoExpedienteAsync(expedicion, CancellationToken.None);

            //ASSERT
            Assert.IsType<EstadoExpediente>(actual);
            Assert.True(actual.Id == EstadoExpediente.Cerrado);
        }

        #endregion

        #region AddSeguimientoExpediente

        [Fact(DisplayName = "Cuando se adiciona seguimiento Retorna ok")]
        public void AddSeguimientoExpediente_Add_Ok()
        {
            //ARRANGE
            var descripcion = "test";
            var expediente = new ExpedienteAlumno();
            
            var mockExpedientesGestorUnirServiceClient = new Mock<IExpedientesGestorUnirServiceClient> { CallBase = true };
            var mockLocalizer = new Mock<IStringLocalizer<SetEstadoHitosEspecializacionesExpedienteAlumnoCommandHandler>>();
            var mockErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient> { CallBase = true };

            var sut = new SetEstadoHitosEspecializacionesExpedienteAlumnoCommandHandler(Context,
                mockExpedientesGestorUnirServiceClient.Object, mockLocalizer.Object,
                mockErpAcademicoServiceClient.Object);

            // ACT
            sut.AddSeguimientoExpediente(expediente, descripcion);

            //ASSERT
            Assert.Single(expediente.Seguimientos);
        }

        #endregion

        #region GetPlanSurpassedErpAsync

        [Fact(DisplayName = "Cuando se adiciona seguimiento Retorna ok")]
        public async Task GetPlanSurpassedErpAsync_SinAsignaturas()
        {
            //ARRANGE
            var expediente = new ExpedienteAlumno();

            var mockExpedientesGestorUnirServiceClient = new Mock<IExpedientesGestorUnirServiceClient> { CallBase = true };
            var mockLocalizer = new Mock<IStringLocalizer<SetEstadoHitosEspecializacionesExpedienteAlumnoCommandHandler>>();
            var mockErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient> { CallBase = true };
            mockErpAcademicoServiceClient
                .Setup(s => s.ItIsPlanSurpassed(It.IsAny<int>(), It.IsAny<EsPlanSuperadoParameters>()))
                .ReturnsAsync(new PlanSuperadoErpAcademicoModel());

            var sut = new Mock<SetEstadoHitosEspecializacionesExpedienteAlumnoCommandHandler>(Context,
                    mockExpedientesGestorUnirServiceClient.Object, mockLocalizer.Object,
                    mockErpAcademicoServiceClient.Object)
                { CallBase = true };

            // ACT
            var result = await sut.Object.GetPlanSurpassedErpAsync(expediente, new ExpedienteExpedientesIntegrationModel
            {
                Asignaturas = new List<AsignaturaErpAcademicoExpedientesIntegrationModel>()
            });

            //ASSERT
            Assert.NotNull(result);
            Assert.IsType<ExpedienteAlumnoTitulacionPlanDto>(result);
            mockErpAcademicoServiceClient
                .Verify(s => s.ItIsPlanSurpassed(It.IsAny<int>(), It.IsAny<EsPlanSuperadoParameters>()), Times.Never);
        }

        [Fact(DisplayName = "Cuando se adiciona seguimiento Retorna ok")]
        public async Task GetPlanSurpassedErpAsync_ConAsignaturas()
        {
            //ARRANGE
            var expediente = new ExpedienteAlumno
            {
                IdRefNodo = "1",
                IdRefVersionPlan = "1",
                IdRefPlan = "1"
            };

            var mockExpedientesGestorUnirServiceClient = new Mock<IExpedientesGestorUnirServiceClient> { CallBase = true };
            var mockLocalizer = new Mock<IStringLocalizer<SetEstadoHitosEspecializacionesExpedienteAlumnoCommandHandler>>();
            var mockErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient> { CallBase = true };
            mockErpAcademicoServiceClient
                .Setup(s => s.ItIsPlanSurpassed(It.IsAny<int>(), It.IsAny<EsPlanSuperadoParameters>()))
                .ReturnsAsync(new PlanSuperadoErpAcademicoModel());

            var sut = new Mock<SetEstadoHitosEspecializacionesExpedienteAlumnoCommandHandler>(Context,
                    mockExpedientesGestorUnirServiceClient.Object, mockLocalizer.Object,
                    mockErpAcademicoServiceClient.Object)
                { CallBase = true };

            // ACT
            var result = await sut.Object.GetPlanSurpassedErpAsync(expediente, new ExpedienteExpedientesIntegrationModel
            {
                Asignaturas = new List<AsignaturaErpAcademicoExpedientesIntegrationModel>
                {
                    new AsignaturaErpAcademicoExpedientesIntegrationModel
                    {
                        IdAsignatura = 1
                    }
                }
            });

            //ASSERT
            Assert.NotNull(result);
            Assert.IsType<ExpedienteAlumnoTitulacionPlanDto>(result);
            mockErpAcademicoServiceClient
                .Verify(s => s.ItIsPlanSurpassed(It.IsAny<int>(), It.IsAny<EsPlanSuperadoParameters>()), Times.Once);
        }

        #endregion
    }
}
