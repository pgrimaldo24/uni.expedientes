using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Unir.Expedientes.Application.Common.Interfaces;
using Unir.Expedientes.Application.ConsolidacionesRequisitosExpedientes.Commands.AddConsolidacionesRequisitosExpedienteUncommit;
using Unir.Expedientes.Application.ExpedientesAlumnos.Commands.CreateExpedienteAlumno;
using Unir.Expedientes.Application.ExpedientesAlumnos.Commands.ValidateNodoExpedienteAlumno;
using Unir.Expedientes.Application.ExpedientesAlumnos.Queries.HasPrimeraMatriculaExpediente;
using Unir.Expedientes.Application.Tests.Common;
using Unir.Expedientes.Domain.Entities;
using Unir.Framework.ApplicationSuperTypes.Exceptions;
using Xunit;

namespace Unir.Expedientes.Application.Tests.ExpedientesAlumnos.Commands.CreateExpedienteAlumno;

[Collection("CommonTestCollection")]
public class CreateExpedienteAlumnoCommandHandlerTest : TestBase
{
    #region Handle

    [Fact(DisplayName = "Cuando se crea un expediente de alumno Devuelve id")]
    public async Task Handle_Ok()
    {
        //ARRANGE
        await Context.TitulacionesAccesos.AddAsync(new TitulacionAcceso
        {
            Id = 1
        });
        await Context.EstadosExpedientes.AddAsync(new EstadoExpediente
        {
            Id = EstadoExpediente.Inicial
        });
        await Context.SaveChangesAsync(CancellationToken.None);
        var mockIStringLocalizer = new Mock<IStringLocalizer<CreateExpedienteAlumnoCommandHandler>>
        {
            CallBase = true
        };
        var mockIMediator = new Mock<IMediator>
        {
            CallBase = true
        };
        mockIMediator.Setup(s =>
            s.Send(It.IsAny<ValidateNodoExpedienteAlumnoCommand>(), It.IsAny<CancellationToken>()));
        mockIMediator.Setup(s =>
                s.Send(It.IsAny<AddConsolidacionesRequisitosExpedienteUncommitCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Unit.Value);
        var mockILogger = new Mock<ILogger<CreateExpedienteAlumnoCommandHandler>>
        {
            CallBase = true
        };
        var sut = new Mock<CreateExpedienteAlumnoCommandHandler>(Context, mockIStringLocalizer.Object,
            mockIMediator.Object, mockILogger.Object, null)
        {
            CallBase = true
        };
        sut.Setup(s => s.ValidatePropiedadesRequeridas(It.IsAny<CreateExpedienteAlumnoCommand>()));
        sut.Setup(s =>
            s.AssignExpedienteAlumno(It.IsAny<CreateExpedienteAlumnoCommand>(), It.IsAny<ExpedienteAlumno>()));
        sut.Setup(s =>
                s.AddNewExpediente(It.IsAny<ExpedienteAlumno>(),
                    It.Is<int>(ts => ts == TipoSeguimientoExpediente.ExpedienteCreado),
                    It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        sut.Setup(s => s.AddSeguimientoExpediente(It.IsAny<ExpedienteAlumno>(),
            It.IsAny<CreateExpedienteAlumnoCommand>(), It.Is<int>(ts => ts == TipoSeguimientoExpediente.ExpedienteCreado),
            It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        sut.Setup(s => s.AssignTitulacionAcceso(It.IsAny<TitulacionAccesoDto>(), It.IsAny<ExpedienteAlumno>()));
        sut.Setup(s => s.AddEspecializaciones(It.IsAny<ExpedienteAlumno>(),
            It.IsAny<List<ExpedienteEspecializacionDto>>()));
        sut.Setup(s => s.AssignExpedienteAlumnoRelacionado(It.IsAny<CreateExpedienteAlumnoCommand>(),
                It.IsAny<ExpedienteAlumno>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        sut.Setup(s => s.AddHitosConseguidos(It.IsAny<DateTime?>(),
                It.IsAny<ExpedienteAlumno>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        var request = new CreateExpedienteAlumnoCommand
        {
            IdRefIntegracionAlumno = "123",
            IdRefPlan = "456",
            PorIntegracion = false,
            TitulacionAcceso = new TitulacionAccesoDto()
        };

        //ACT
        var actual = await sut.Object.Handle(request, CancellationToken.None);

        //ASSERT
        sut.Verify(s => s.ValidatePropiedadesRequeridas(It.IsAny<CreateExpedienteAlumnoCommand>()), Times.Once);
        sut.Verify(
            s => s.AssignExpedienteAlumno(It.IsAny<CreateExpedienteAlumnoCommand>(), It.IsAny<ExpedienteAlumno>()),
            Times.Once);
        sut.Verify(
            s => s.AddNewExpediente(It.IsAny<ExpedienteAlumno>(),
                It.Is<int>(ts => ts == TipoSeguimientoExpediente.ExpedienteCreado),
                It.IsAny<CancellationToken>()),
            Times.Once);
        sut.Verify(
            s => s.AddSeguimientoExpediente(It.IsAny<ExpedienteAlumno>(), It.IsAny<CreateExpedienteAlumnoCommand>(),
                It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Once);
        sut.Verify(s => s.AssignTitulacionAcceso(It.IsAny<TitulacionAccesoDto>(), It.IsAny<ExpedienteAlumno>()), Times.Once);
        sut.Verify(s => s.AddEspecializaciones(It.IsAny<ExpedienteAlumno>(),
            It.IsAny<List<ExpedienteEspecializacionDto>>()), Times.Once);
        sut.Verify(s => s.AssignExpedienteAlumnoRelacionado(It.IsAny<CreateExpedienteAlumnoCommand>(),
            It.IsAny<ExpedienteAlumno>(), It.IsAny<CancellationToken>()), Times.Once);
        sut.Verify(s => s.AddHitosConseguidos(It.IsAny<DateTime?>(),
            It.IsAny<ExpedienteAlumno>(), It.IsAny<CancellationToken>()), Times.Once);
        mockIMediator.Verify(x => x.Send(It.IsAny<AddConsolidacionesRequisitosExpedienteUncommitCommand>(),
            It.IsAny<CancellationToken>()), Times.Once);
        Assert.True(actual >= 0);
    }

    [Fact(DisplayName = "Cuando un expediente ya existe por alumno y plan se edita sus valores y Devuelve id")]
    public async Task Handle_EditOk()
    {
        //ARRANGE
        var request = new CreateExpedienteAlumnoCommand
        {
            IdRefIntegracionAlumno = "123",
            IdRefPlan = "456",
            PorIntegracion = true
        };
        await Context.EstadosExpedientes.AddAsync(new EstadoExpediente
        {
            Id = EstadoExpediente.Inicial
        });
        await Context.ExpedientesAlumno.AddAsync(new ExpedienteAlumno
        {
            Id = 1,
            IdRefIntegracionAlumno = request.IdRefIntegracionAlumno,
            IdRefPlan = request.IdRefPlan
        });
        await Context.SaveChangesAsync(CancellationToken.None);
        var mockIStringLocalizer = new Mock<IStringLocalizer<CreateExpedienteAlumnoCommandHandler>>
        {
            CallBase = true
        };
        var mockIMediator = new Mock<IMediator>
        {
            CallBase = true
        };
        mockIMediator.Setup(s =>
            s.Send(It.IsAny<ValidateNodoExpedienteAlumnoCommand>(), It.IsAny<CancellationToken>()));
        mockIMediator.Setup(s =>
                s.Send(It.IsAny<AddConsolidacionesRequisitosExpedienteUncommitCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Unit.Value);
        var mockILogger = new Mock<ILogger<CreateExpedienteAlumnoCommandHandler>>
        {
            CallBase = true
        };
        var sut = new Mock<CreateExpedienteAlumnoCommandHandler>(Context, mockIStringLocalizer.Object,
            mockIMediator.Object, mockILogger.Object, null)
        {
            CallBase = true
        };
        sut.Setup(s => s.ValidatePropiedadesRequeridas(It.IsAny<CreateExpedienteAlumnoCommand>()));
        sut.Setup(s =>
            s.AssignExpedienteAlumno(It.IsAny<CreateExpedienteAlumnoCommand>(), It.IsAny<ExpedienteAlumno>()));
        sut.Setup(s =>
                s.AddNewExpediente(It.IsAny<ExpedienteAlumno>(),
                    It.Is<int>(ts => ts == TipoSeguimientoExpediente.ExpedienteModificadoVersionPlan),
                    It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        sut.Setup(s => s.AddSeguimientoExpediente(It.IsAny<ExpedienteAlumno>(),
            It.IsAny<CreateExpedienteAlumnoCommand>(), It.Is<int>(ts => ts == TipoSeguimientoExpediente.ExpedienteModificadoVersionPlan),
            It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        sut.Setup(s => s.AssignTitulacionAcceso(It.IsAny<TitulacionAccesoDto>(), It.IsAny<ExpedienteAlumno>()));
        sut.Setup(s => s.AddEspecializaciones(It.IsAny<ExpedienteAlumno>(),
            It.IsAny<List<ExpedienteEspecializacionDto>>()));
        sut.Setup(s => s.AssignExpedienteAlumnoRelacionado(It.IsAny<CreateExpedienteAlumnoCommand>(),
                It.IsAny<ExpedienteAlumno>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        sut.Setup(s => s.AddHitosConseguidos(It.IsAny<DateTime?>(),
                It.IsAny<ExpedienteAlumno>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        //ACT
        var actual = await sut.Object.Handle(request, CancellationToken.None);

        //ASSERT
        sut.Verify(s => s.ValidatePropiedadesRequeridas(It.IsAny<CreateExpedienteAlumnoCommand>()), Times.Once);
        sut.Verify(
            s => s.AssignExpedienteAlumno(It.IsAny<CreateExpedienteAlumnoCommand>(), It.IsAny<ExpedienteAlumno>()),
            Times.Once);
        sut.Verify(
            s => s.AddNewExpediente(It.IsAny<ExpedienteAlumno>(),
                It.Is<int>(ts => ts == TipoSeguimientoExpediente.ExpedienteModificadoVersionPlan),
                It.IsAny<CancellationToken>()),
            Times.Once);
        sut.Verify(
            s => s.AddSeguimientoExpediente(It.IsAny<ExpedienteAlumno>(), It.IsAny<CreateExpedienteAlumnoCommand>(),
                It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Once);
        sut.Verify(s => s.AssignTitulacionAcceso(It.IsAny<TitulacionAccesoDto>(), It.IsAny<ExpedienteAlumno>()), Times.Once);
        sut.Verify(s => s.AddEspecializaciones(It.IsAny<ExpedienteAlumno>(),
            It.IsAny<List<ExpedienteEspecializacionDto>>()), Times.Once);
        sut.Verify(s => s.AssignExpedienteAlumnoRelacionado(It.IsAny<CreateExpedienteAlumnoCommand>(),
            It.IsAny<ExpedienteAlumno>(), It.IsAny<CancellationToken>()), Times.Once);
        sut.Verify(s => s.AddHitosConseguidos(It.IsAny<DateTime?>(),
            It.IsAny<ExpedienteAlumno>(), It.IsAny<CancellationToken>()), Times.Once);
        mockIMediator.Verify(x => x.Send(It.IsAny<AddConsolidacionesRequisitosExpedienteUncommitCommand>(),
            It.IsAny<CancellationToken>()), Times.Once);
        Assert.True(actual >= 0);
    }

    #endregion

    #region ValidatePropiedadesRequeridas

    [Fact(DisplayName = "Cuando falta especificar el id integración alumno Devuelve excepción")]
    public void ValidatePropiedadesRequeridas_IdRefIntegracionAlumno()
    {
        //ARRANGE
        var request = new CreateExpedienteAlumnoCommand
        {
            IdRefPlan = "456"
        };
        var mockIStringLocalizer = new Mock<IStringLocalizer<CreateExpedienteAlumnoCommandHandler>>
        {
            CallBase = true
        };
        var mensajeEsperado = $"El campo {nameof(request.IdRefIntegracionAlumno)} es requerido para Crear el Expediente.";
        mockIStringLocalizer.Setup(s => s[It.Is<string>(msj => msj == mensajeEsperado)])
            .Returns(new LocalizedString(mensajeEsperado, mensajeEsperado));
        var mockIMediator = new Mock<IMediator>
        {
            CallBase = true
        };
        var mockILogger = new Mock<ILogger<CreateExpedienteAlumnoCommandHandler>>
        {
            CallBase = true
        };
        var sut = new CreateExpedienteAlumnoCommandHandler(Context, mockIStringLocalizer.Object,
            mockIMediator.Object, mockILogger.Object, null);

        //ACT
        var ex = Record.Exception(() =>
        {
            sut.ValidatePropiedadesRequeridas(request);
        });

        //ASSERT
        Assert.IsType<BadRequestException>(ex);
        Assert.Equal(mensajeEsperado, ex.Message);
    }

    [Fact(DisplayName = "Cuando el id integración alumno es inválido Devuelve excepción")]
    public void ValidatePropiedadesRequeridas_IdRefIntegracionAlumnoInvalido()
    {
        //ARRANGE
        var request = new CreateExpedienteAlumnoCommand
        {
            IdRefIntegracionAlumno = "456s"
        };
        var mockIStringLocalizer = new Mock<IStringLocalizer<CreateExpedienteAlumnoCommandHandler>>
        {
            CallBase = true
        };
        const string mensajeEsperado = $"El campo {nameof(request.IdRefIntegracionAlumno)} tiene un valor inválido.";
        mockIStringLocalizer.Setup(s => s[It.Is<string>(msj => msj == mensajeEsperado)])
            .Returns(new LocalizedString(mensajeEsperado, mensajeEsperado));
        var mockIMediator = new Mock<IMediator>
        {
            CallBase = true
        };
        var mockILogger = new Mock<ILogger<CreateExpedienteAlumnoCommandHandler>>
        {
            CallBase = true
        };
        var sut = new CreateExpedienteAlumnoCommandHandler(Context, mockIStringLocalizer.Object,
            mockIMediator.Object, mockILogger.Object, null);

        //ACT
        var ex = Record.Exception(() =>
        {
            sut.ValidatePropiedadesRequeridas(request);
        });

        //ASSERT
        Assert.IsType<BadRequestException>(ex);
        Assert.Equal(mensajeEsperado, ex.Message);
    }

    [Fact(DisplayName = "Cuando el id integración alumno no es mayor que cero Devuelve excepción")]
    public void ValidatePropiedadesRequeridas_IdRefIntegracionAlumnoNoEsMayorCero()
    {
        //ARRANGE
        var request = new CreateExpedienteAlumnoCommand
        {
            IdRefIntegracionAlumno = "0"
        };
        var mockIStringLocalizer = new Mock<IStringLocalizer<CreateExpedienteAlumnoCommandHandler>>
        {
            CallBase = true
        };
        const string mensajeEsperado = $"El campo {nameof(request.IdRefIntegracionAlumno)} debe ser mayor a cero.";
        mockIStringLocalizer.Setup(s => s[It.Is<string>(msj => msj == mensajeEsperado)])
            .Returns(new LocalizedString(mensajeEsperado, mensajeEsperado));
        var mockIMediator = new Mock<IMediator>
        {
            CallBase = true
        };
        var mockILogger = new Mock<ILogger<CreateExpedienteAlumnoCommandHandler>>
        {
            CallBase = true
        };
        var sut = new CreateExpedienteAlumnoCommandHandler(Context, mockIStringLocalizer.Object,
            mockIMediator.Object, mockILogger.Object, null);

        //ACT
        var ex = Record.Exception(() =>
        {
            sut.ValidatePropiedadesRequeridas(request);
        });

        //ASSERT
        Assert.IsType<BadRequestException>(ex);
        Assert.Equal(mensajeEsperado, ex.Message);
    }

    [Fact(DisplayName = "Cuando falta especificar el id ref plan Devuelve excepción")]
    public void ValidatePropiedadesRequeridas_IdRefPlan()
    {
        //ARRANGE
        var request = new CreateExpedienteAlumnoCommand
        {
            IdRefIntegracionAlumno = "123"
        };
        var mockIStringLocalizer = new Mock<IStringLocalizer<CreateExpedienteAlumnoCommandHandler>>
        {
            CallBase = true
        };
        var mensajeEsperado = $"El campo {nameof(request.IdRefPlan)} es requerido para Crear el Expediente.";
        mockIStringLocalizer.Setup(s => s[It.Is<string>(msj => msj == mensajeEsperado)])
            .Returns(new LocalizedString(mensajeEsperado, mensajeEsperado));
        var mockIMediator = new Mock<IMediator>
        {
            CallBase = true
        };
        var mockILogger = new Mock<ILogger<CreateExpedienteAlumnoCommandHandler>>
        {
            CallBase = true
        };
        var sut = new CreateExpedienteAlumnoCommandHandler(Context, mockIStringLocalizer.Object,
            mockIMediator.Object, mockILogger.Object, null);

        //ACT
        var ex = Record.Exception(() =>
        {
            sut.ValidatePropiedadesRequeridas(request);
        });

        //ASSERT
        Assert.IsType<BadRequestException>(ex);
        Assert.Equal(mensajeEsperado, ex.Message);
    }

    [Fact(DisplayName = "Cuando falta el id ref plan es inválido Devuelve excepción")]
    public void ValidatePropiedadesRequeridas_IdRefPlanInvalido()
    {
        //ARRANGE
        var request = new CreateExpedienteAlumnoCommand
        {
            IdRefIntegracionAlumno = "321",
            IdRefPlan = "123s"
        };
        var mockIStringLocalizer = new Mock<IStringLocalizer<CreateExpedienteAlumnoCommandHandler>>
        {
            CallBase = true
        };
        const string mensajeEsperado = $"El campo {nameof(request.IdRefPlan)} tiene un valor inválido.";
        mockIStringLocalizer.Setup(s => s[It.Is<string>(msj => msj == mensajeEsperado)])
            .Returns(new LocalizedString(mensajeEsperado, mensajeEsperado));
        var mockIMediator = new Mock<IMediator>
        {
            CallBase = true
        };
        var mockILogger = new Mock<ILogger<CreateExpedienteAlumnoCommandHandler>>
        {
            CallBase = true
        };
        var sut = new CreateExpedienteAlumnoCommandHandler(Context, mockIStringLocalizer.Object,
            mockIMediator.Object, mockILogger.Object, null);

        //ACT
        var ex = Record.Exception(() =>
        {
            sut.ValidatePropiedadesRequeridas(request);
        });

        //ASSERT
        Assert.IsType<BadRequestException>(ex);
        Assert.Equal(mensajeEsperado, ex.Message);
    }

    [Fact(DisplayName = "Cuando falta el id ref plan no es mayor que cero Devuelve excepción")]
    public void ValidatePropiedadesRequeridas_IdRefPlanNoEsMayorQueCero()
    {
        //ARRANGE
        var request = new CreateExpedienteAlumnoCommand
        {
            IdRefIntegracionAlumno = "321",
            IdRefPlan = "0"
        };
        var mockIStringLocalizer = new Mock<IStringLocalizer<CreateExpedienteAlumnoCommandHandler>>
        {
            CallBase = true
        };
        const string mensajeEsperado = $"El campo {nameof(request.IdRefPlan)} debe ser mayor a cero.";
        mockIStringLocalizer.Setup(s => s[It.Is<string>(msj => msj == mensajeEsperado)])
            .Returns(new LocalizedString(mensajeEsperado, mensajeEsperado));
        var mockIMediator = new Mock<IMediator>
        {
            CallBase = true
        };
        var mockILogger = new Mock<ILogger<CreateExpedienteAlumnoCommandHandler>>
        {
            CallBase = true
        };
        var sut = new CreateExpedienteAlumnoCommandHandler(Context, mockIStringLocalizer.Object,
            mockIMediator.Object, mockILogger.Object, null);

        //ACT
        var ex = Record.Exception(() =>
        {
            sut.ValidatePropiedadesRequeridas(request);
        });

        //ASSERT
        Assert.IsType<BadRequestException>(ex);
        Assert.Equal(mensajeEsperado, ex.Message);
    }

    [Fact(DisplayName = "Cuando falta especificar el id ref versión plan es inválido Devuelve excepción")]
    public void ValidatePropiedadesRequeridas_IdRefVersionPlan_Invalido()
    {
        //ARRANGE
        var request = new CreateExpedienteAlumnoCommand
        {
            IdRefIntegracionAlumno = "123",
            IdRefPlan = "456",
            IdRefVersionPlan = "as"
        };
        var mockIStringLocalizer = new Mock<IStringLocalizer<CreateExpedienteAlumnoCommandHandler>>
        {
            CallBase = true
        };
        var mensajeEsperado = $"El campo {nameof(request.IdRefVersionPlan)} tiene un valor inválido.";
        mockIStringLocalizer.Setup(s => s[It.Is<string>(msj => msj == mensajeEsperado)])
            .Returns(new LocalizedString(mensajeEsperado, mensajeEsperado));
        var mockIMediator = new Mock<IMediator>
        {
            CallBase = true
        };
        var mockILogger = new Mock<ILogger<CreateExpedienteAlumnoCommandHandler>>
        {
            CallBase = true
        };
        var sut = new CreateExpedienteAlumnoCommandHandler(Context, mockIStringLocalizer.Object,
            mockIMediator.Object, mockILogger.Object, null);

        //ACT
        var ex = Record.Exception(() =>
        {
            sut.ValidatePropiedadesRequeridas(request);
        });

        //ASSERT
        Assert.IsType<BadRequestException>(ex);
        Assert.Equal(mensajeEsperado, ex.Message);
    }

    [Fact(DisplayName = "Cuando el id ref versión plan no es mayor que cero Devuelve excepción")]
    public void ValidatePropiedadesRequeridas_IdRefVersionPlan_NoEsMayorQueCero()
    {
        //ARRANGE
        var request = new CreateExpedienteAlumnoCommand
        {
            IdRefIntegracionAlumno = "123",
            IdRefPlan = "456",
            IdRefVersionPlan = "0"
        };
        var mockIStringLocalizer = new Mock<IStringLocalizer<CreateExpedienteAlumnoCommandHandler>>
        {
            CallBase = true
        };
        const string mensajeEsperado = $"El campo {nameof(request.IdRefVersionPlan)} debe ser mayor a cero.";
        mockIStringLocalizer.Setup(s => s[It.Is<string>(msj => msj == mensajeEsperado)])
            .Returns(new LocalizedString(mensajeEsperado, mensajeEsperado));
        var mockIMediator = new Mock<IMediator>
        {
            CallBase = true
        };
        var mockILogger = new Mock<ILogger<CreateExpedienteAlumnoCommandHandler>>
        {
            CallBase = true
        };
        var sut = new CreateExpedienteAlumnoCommandHandler(Context, mockIStringLocalizer.Object,
            mockIMediator.Object, mockILogger.Object, null);

        //ACT
        var ex = Record.Exception(() =>
        {
            sut.ValidatePropiedadesRequeridas(request);
        });

        //ASSERT
        Assert.IsType<BadRequestException>(ex);
        Assert.Equal(mensajeEsperado, ex.Message);
    }

    [Fact(DisplayName = "Cuando falta especificar el número de versión plan Devuelve excepción")]
    public void ValidatePropiedadesRequeridas_NroVersion()
    {
        //ARRANGE
        var request = new CreateExpedienteAlumnoCommand
        {
            IdRefIntegracionAlumno = "123",
            IdRefPlan = "456",
            IdRefVersionPlan = "789"
        };
        var mockIStringLocalizer = new Mock<IStringLocalizer<CreateExpedienteAlumnoCommandHandler>>
        {
            CallBase = true
        };
        var mensajeEsperado = $"El campo {nameof(request.NroVersion)} es requerido para Crear el Expediente.";
        mockIStringLocalizer.Setup(s => s[It.Is<string>(msj => msj == mensajeEsperado)])
            .Returns(new LocalizedString(mensajeEsperado, mensajeEsperado));
        var mockIMediator = new Mock<IMediator>
        {
            CallBase = true
        };
        var mockILogger = new Mock<ILogger<CreateExpedienteAlumnoCommandHandler>>
        {
            CallBase = true
        };
        var sut = new CreateExpedienteAlumnoCommandHandler(Context, mockIStringLocalizer.Object,
            mockIMediator.Object, mockILogger.Object, null);

        //ACT
        var ex = Record.Exception(() =>
        {
            sut.ValidatePropiedadesRequeridas(request);
        });

        //ASSERT
        Assert.IsType<BadRequestException>(ex);
        Assert.Equal(mensajeEsperado, ex.Message);
    }

    [Fact(DisplayName = "Cuando falta especificar el id ref nodo Devuelve excepción")]
    public void ValidatePropiedadesRequeridas_IdRefNodo_Invalido()
    {
        //ARRANGE
        var request = new CreateExpedienteAlumnoCommand
        {
            IdRefIntegracionAlumno = "123",
            IdRefPlan = "456",
            IdRefVersionPlan = "789",
            NroVersion = 1,
            IdRefNodo = "as1"
        };
        var mockIStringLocalizer = new Mock<IStringLocalizer<CreateExpedienteAlumnoCommandHandler>>
        {
            CallBase = true
        };
        var mensajeEsperado = $"El campo {nameof(request.IdRefNodo)} tiene un valor inválido.";
        mockIStringLocalizer.Setup(s => s[It.Is<string>(msj => msj == mensajeEsperado)])
            .Returns(new LocalizedString(mensajeEsperado, mensajeEsperado));
        var mockIMediator = new Mock<IMediator>
        {
            CallBase = true
        };
        var mockILogger = new Mock<ILogger<CreateExpedienteAlumnoCommandHandler>>
        {
            CallBase = true
        };
        var sut = new CreateExpedienteAlumnoCommandHandler(Context, mockIStringLocalizer.Object,
            mockIMediator.Object, mockILogger.Object, null);

        //ACT
        var ex = Record.Exception(() =>
        {
            sut.ValidatePropiedadesRequeridas(request);
        });

        //ASSERT
        Assert.IsType<BadRequestException>(ex);
        Assert.Equal(mensajeEsperado, ex.Message);
    }

    [Fact(DisplayName = "Cuando el id ref nodo no es mayor que cero Devuelve excepción")]
    public void ValidatePropiedadesRequeridas_IdRefNodo_NoEsMayorQueCero()
    {
        //ARRANGE
        var request = new CreateExpedienteAlumnoCommand
        {
            IdRefIntegracionAlumno = "123",
            IdRefPlan = "456",
            IdRefVersionPlan = "789",
            NroVersion = 1,
            IdRefNodo = "0"
        };
        var mockIStringLocalizer = new Mock<IStringLocalizer<CreateExpedienteAlumnoCommandHandler>>
        {
            CallBase = true
        };
        const string mensajeEsperado = $"El campo {nameof(request.IdRefNodo)} debe ser mayor a cero.";
        mockIStringLocalizer.Setup(s => s[It.Is<string>(msj => msj == mensajeEsperado)])
            .Returns(new LocalizedString(mensajeEsperado, mensajeEsperado));
        var mockIMediator = new Mock<IMediator>
        {
            CallBase = true
        };
        var mockILogger = new Mock<ILogger<CreateExpedienteAlumnoCommandHandler>>
        {
            CallBase = true
        };
        var sut = new CreateExpedienteAlumnoCommandHandler(Context, mockIStringLocalizer.Object,
            mockIMediator.Object, mockILogger.Object, null);

        //ACT
        var ex = Record.Exception(() =>
        {
            sut.ValidatePropiedadesRequeridas(request);
        });

        //ASSERT
        Assert.IsType<BadRequestException>(ex);
        Assert.Equal(mensajeEsperado, ex.Message);
    }

    [Fact(DisplayName = "Cuando falta especificar el nombre del alumno Devuelve excepción")]
    public void ValidatePropiedadesRequeridas_AlumnoNombre_Invalido()
    {
        //ARRANGE
        var request = new CreateExpedienteAlumnoCommand
        {
            IdRefIntegracionAlumno = "123",
            IdRefPlan = "456",
            IdRefVersionPlan = "789",
            NroVersion = 1,
            IdRefNodo = "122"
        };
        var mockIStringLocalizer = new Mock<IStringLocalizer<CreateExpedienteAlumnoCommandHandler>>
        {
            CallBase = true
        };
        var mensajeEsperado = $"El campo {nameof(request.AlumnoNombre)} es requerido para Crear el Expediente.";
        mockIStringLocalizer.Setup(s => s[It.Is<string>(msj => msj == mensajeEsperado)])
            .Returns(new LocalizedString(mensajeEsperado, mensajeEsperado));
        var mockIMediator = new Mock<IMediator>
        {
            CallBase = true
        };
        var mockILogger = new Mock<ILogger<CreateExpedienteAlumnoCommandHandler>>
        {
            CallBase = true
        };
        var sut = new CreateExpedienteAlumnoCommandHandler(Context, mockIStringLocalizer.Object,
            mockIMediator.Object, mockILogger.Object, null);

        //ACT
        var ex = Record.Exception(() =>
        {
            sut.ValidatePropiedadesRequeridas(request);
        });

        //ASSERT
        Assert.IsType<BadRequestException>(ex);
        Assert.Equal(mensajeEsperado, ex.Message);
    }

    [Fact(DisplayName = "Cuando falta especificar el apellido1 del alumno Devuelve excepción")]
    public void ValidatePropiedadesRequeridas_AlumnoApellido1_Invalido()
    {
        //ARRANGE
        var request = new CreateExpedienteAlumnoCommand
        {
            IdRefIntegracionAlumno = "123",
            IdRefPlan = "456",
            IdRefVersionPlan = "789",
            NroVersion = 1,
            IdRefNodo = "122",
            AlumnoNombre = Guid.NewGuid().ToString()
        };
        var mockIStringLocalizer = new Mock<IStringLocalizer<CreateExpedienteAlumnoCommandHandler>>
        {
            CallBase = true
        };
        var mensajeEsperado = $"El campo {nameof(request.AlumnoApellido1)} es requerido para Crear el Expediente.";
        mockIStringLocalizer.Setup(s => s[It.Is<string>(msj => msj == mensajeEsperado)])
            .Returns(new LocalizedString(mensajeEsperado, mensajeEsperado));
        var mockIMediator = new Mock<IMediator>
        {
            CallBase = true
        };
        var mockILogger = new Mock<ILogger<CreateExpedienteAlumnoCommandHandler>>
        {
            CallBase = true
        };
        var sut = new CreateExpedienteAlumnoCommandHandler(Context, mockIStringLocalizer.Object,
            mockIMediator.Object, mockILogger.Object, null);

        //ACT
        var ex = Record.Exception(() =>
        {
            sut.ValidatePropiedadesRequeridas(request);
        });

        //ASSERT
        Assert.IsType<BadRequestException>(ex);
        Assert.Equal(mensajeEsperado, ex.Message);
    }

    [Fact(DisplayName = "Cuando falta especificar el id de referencia del tipo de identificación por país Devuelve excepción")]
    public void ValidatePropiedadesRequeridas_IdRefTipoDocumentoIdentificacionPais_Invalido()
    {
        //ARRANGE
        var request = new CreateExpedienteAlumnoCommand
        {
            IdRefIntegracionAlumno = "123",
            IdRefPlan = "456",
            IdRefVersionPlan = "789",
            NroVersion = 1,
            IdRefNodo = "122",
            AlumnoNombre = Guid.NewGuid().ToString(),
            AlumnoApellido1 = Guid.NewGuid().ToString()
        };
        var mockIStringLocalizer = new Mock<IStringLocalizer<CreateExpedienteAlumnoCommandHandler>>
        {
            CallBase = true
        };
        var mensajeEsperado = $"El campo {nameof(request.IdRefTipoDocumentoIdentificacionPais)} es requerido para Crear el Expediente.";
        mockIStringLocalizer.Setup(s => s[It.Is<string>(msj => msj == mensajeEsperado)])
            .Returns(new LocalizedString(mensajeEsperado, mensajeEsperado));
        var mockIMediator = new Mock<IMediator>
        {
            CallBase = true
        };
        var mockILogger = new Mock<ILogger<CreateExpedienteAlumnoCommandHandler>>
        {
            CallBase = true
        };
        var sut = new CreateExpedienteAlumnoCommandHandler(Context, mockIStringLocalizer.Object,
            mockIMediator.Object, mockILogger.Object, null);

        //ACT
        var ex = Record.Exception(() =>
        {
            sut.ValidatePropiedadesRequeridas(request);
        });

        //ASSERT
        Assert.IsType<BadRequestException>(ex);
        Assert.Equal(mensajeEsperado, ex.Message);
    }

    [Fact(DisplayName = "Cuando falta especificar el número de identificación del alumno Devuelve excepción")]
    public void ValidatePropiedadesRequeridas_AlumnoNroDocIdentificacion_Invalido()
    {
        //ARRANGE
        var request = new CreateExpedienteAlumnoCommand
        {
            IdRefIntegracionAlumno = "123",
            IdRefPlan = "456",
            IdRefVersionPlan = "789",
            NroVersion = 1,
            IdRefNodo = "122",
            AlumnoNombre = Guid.NewGuid().ToString(),
            AlumnoApellido1 = Guid.NewGuid().ToString(),
            IdRefTipoDocumentoIdentificacionPais = "654"
        };
        var mockIStringLocalizer = new Mock<IStringLocalizer<CreateExpedienteAlumnoCommandHandler>>
        {
            CallBase = true
        };
        var mensajeEsperado = $"El campo {nameof(request.AlumnoNroDocIdentificacion)} es requerido para Crear el Expediente.";
        mockIStringLocalizer.Setup(s => s[It.Is<string>(msj => msj == mensajeEsperado)])
            .Returns(new LocalizedString(mensajeEsperado, mensajeEsperado));
        var mockIMediator = new Mock<IMediator>
        {
            CallBase = true
        };
        var mockILogger = new Mock<ILogger<CreateExpedienteAlumnoCommandHandler>>
        {
            CallBase = true
        };
        var sut = new CreateExpedienteAlumnoCommandHandler(Context, mockIStringLocalizer.Object,
            mockIMediator.Object, mockILogger.Object, null);

        //ACT
        var ex = Record.Exception(() =>
        {
            sut.ValidatePropiedadesRequeridas(request);
        });

        //ASSERT
        Assert.IsType<BadRequestException>(ex);
        Assert.Equal(mensajeEsperado, ex.Message);
    }

    [Fact(DisplayName = "Cuando falta especificar el email del alumno Devuelve excepción")]
    public void ValidatePropiedadesRequeridas_AlumnoEmail_Invalido()
    {
        //ARRANGE
        var request = new CreateExpedienteAlumnoCommand
        {
            IdRefIntegracionAlumno = "123",
            IdRefPlan = "456",
            IdRefVersionPlan = "789",
            NroVersion = 1,
            IdRefNodo = "122",
            AlumnoNombre = Guid.NewGuid().ToString(),
            AlumnoApellido1 = Guid.NewGuid().ToString(),
            IdRefTipoDocumentoIdentificacionPais = "654",
            AlumnoNroDocIdentificacion = "46658645113"
        };
        var mockIStringLocalizer = new Mock<IStringLocalizer<CreateExpedienteAlumnoCommandHandler>>
        {
            CallBase = true
        };
        var mensajeEsperado = $"El campo {nameof(request.AlumnoEmail)} es requerido para Crear el Expediente.";
        mockIStringLocalizer.Setup(s => s[It.Is<string>(msj => msj == mensajeEsperado)])
            .Returns(new LocalizedString(mensajeEsperado, mensajeEsperado));
        var mockIMediator = new Mock<IMediator>
        {
            CallBase = true
        };
        var mockILogger = new Mock<ILogger<CreateExpedienteAlumnoCommandHandler>>
        {
            CallBase = true
        };
        var sut = new CreateExpedienteAlumnoCommandHandler(Context, mockIStringLocalizer.Object,
            mockIMediator.Object, mockILogger.Object, null);

        //ACT
        var ex = Record.Exception(() =>
        {
            sut.ValidatePropiedadesRequeridas(request);
        });

        //ASSERT
        Assert.IsType<BadRequestException>(ex);
        Assert.Equal(mensajeEsperado, ex.Message);
    }

    [Fact(DisplayName = "Cuando falta especificar el número de versión plan Devuelve excepción")]
    public void ValidatePropiedadesRequeridas_Ok()
    {
        //ARRANGE
        var request = new CreateExpedienteAlumnoCommand
        {
            IdRefIntegracionAlumno = "123",
            IdRefPlan = "456",
            IdRefNodo = "789",
            AlumnoNombre = "789",
            AlumnoApellido1 = "789",
            IdRefTipoDocumentoIdentificacionPais = "789",
            AlumnoNroDocIdentificacion = "789",
            AlumnoEmail = "789"
        };
        var mockIStringLocalizer = new Mock<IStringLocalizer<CreateExpedienteAlumnoCommandHandler>>
        {
            CallBase = true
        };
        var mockIMediator = new Mock<IMediator>
        {
            CallBase = true
        };
        var mockILogger = new Mock<ILogger<CreateExpedienteAlumnoCommandHandler>>
        {
            CallBase = true
        };
        var sut = new CreateExpedienteAlumnoCommandHandler(Context, mockIStringLocalizer.Object,
            mockIMediator.Object, mockILogger.Object, null);

        var mensajeValidacionVerificacion = $"El campo {nameof(request.IdRefNodo)} tiene un valor inválido.";

        //ACT
        sut.ValidatePropiedadesRequeridas(request);

        //ASSERT
        Assert.True(int.TryParse(request.IdRefNodo, out _));
        Assert.NotNull(request.IdRefPlan);
        Assert.NotNull(request.IdRefIntegracionAlumno);
        mockIStringLocalizer.Verify(s => s[It.Is<string>(msj => msj == mensajeValidacionVerificacion)],
            Times.Never);
    }

    #endregion

    #region AssignExpedienteAlumno

    [Fact(DisplayName = "Cuando se asignan los valores al expediente Devuelve entidad seteada")]
    public void AssignExpedienteAlumno_Ok()
    {
        //ARRANGE
        var request = new CreateExpedienteAlumnoCommand
        {
            IdRefPlan = "456",
            IdRefIntegracionAlumno = "123",
            IdRefVersionPlan = "789",
            IdRefNodo = "159"
        };
        var mockIStringLocalizer = new Mock<IStringLocalizer<CreateExpedienteAlumnoCommandHandler>>
        {
            CallBase = true
        };
        var mockIMediator = new Mock<IMediator>
        {
            CallBase = true
        };
        var mockILogger = new Mock<ILogger<CreateExpedienteAlumnoCommandHandler>>
        {
            CallBase = true
        };
        var sut = new CreateExpedienteAlumnoCommandHandler(Context, mockIStringLocalizer.Object,
            mockIMediator.Object, mockILogger.Object, null);

        var expedienteEsperado = new ExpedienteAlumno();

        //ACT
        sut.AssignExpedienteAlumno(request, expedienteEsperado);

        //ASSERT
        Assert.Equal(expedienteEsperado.IdRefIntegracionAlumno, request.IdRefIntegracionAlumno);
        Assert.Equal(expedienteEsperado.IdRefPlan, request.IdRefPlan);
        Assert.Equal(expedienteEsperado.IdRefVersionPlan, request.IdRefVersionPlan);
        Assert.Equal(expedienteEsperado.IdRefNodo, request.IdRefNodo);
    }

    [Fact(DisplayName = "Cuando se asignan los valores al expediente sin versión ni nodo Devuelve entidad seteada")]
    public void AssignExpedienteAlumno_SinVersionNiNodo()
    {
        //ARRANGE
        var request = new CreateExpedienteAlumnoCommand
        {
            IdRefPlan = "456",
            IdRefIntegracionAlumno = "123"
        };
        var mockIStringLocalizer = new Mock<IStringLocalizer<CreateExpedienteAlumnoCommandHandler>>
        {
            CallBase = true
        };
        var mockIMediator = new Mock<IMediator>
        {
            CallBase = true
        };
        var mockILogger = new Mock<ILogger<CreateExpedienteAlumnoCommandHandler>>
        {
            CallBase = true
        };
        var sut = new CreateExpedienteAlumnoCommandHandler(Context, mockIStringLocalizer.Object,
            mockIMediator.Object, mockILogger.Object, null);

        var expedienteEsperado = new ExpedienteAlumno();

        //ACT
        sut.AssignExpedienteAlumno(request, expedienteEsperado);

        //ASSERT
        Assert.Equal(expedienteEsperado.IdRefIntegracionAlumno, request.IdRefIntegracionAlumno);
        Assert.Equal(expedienteEsperado.IdRefPlan, request.IdRefPlan);
        Assert.Null(expedienteEsperado.IdRefVersionPlan);
        Assert.Null(expedienteEsperado.IdRefNodo);
    }

    #endregion

    #region AddSeguimientoExpediente

    [Fact(DisplayName = "Cuando no se envía el nro versión Se añade el seguimiento")]
    public async Task AddSeguimientoExpediente_SinNroVersion()
    {
        //ARRANGE
        const int idTipoSeguimientoEsperado = TipoSeguimientoExpediente.ExpedienteCreado;
        var request = new CreateExpedienteAlumnoCommand();
        var mockExpedienteAlumno = new Mock<ExpedienteAlumno>
        {
            CallBase = true
        };
        mockExpedienteAlumno.Setup(s =>
            s.AddSeguimientoNoUser(It.Is<int>(i => i == idTipoSeguimientoEsperado), It.IsAny<string>(), null,
                    It.IsAny<string>(), It.IsAny<string>()));
        var mockIStringLocalizer = new Mock<IStringLocalizer<CreateExpedienteAlumnoCommandHandler>>
        {
            CallBase = true
        };
        var mockIMediator = new Mock<IMediator>
        {
            CallBase = true
        };
        var mockILogger = new Mock<ILogger<CreateExpedienteAlumnoCommandHandler>>
        {
            CallBase = true
        };
        var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
        {
            CallBase = true
        };
        var sut = new CreateExpedienteAlumnoCommandHandler(Context, mockIStringLocalizer.Object,
            mockIMediator.Object, mockILogger.Object, mockIErpAcademicoServiceClient.Object);
        mockIMediator.Setup(x => x.Send(It.IsAny<HasPrimeraMatriculaExpedienteQuery>(),
            It.IsAny<CancellationToken>())).ReturnsAsync(false);

        //ACT
        await sut.AddSeguimientoExpediente(mockExpedienteAlumno.Object,
            request, idTipoSeguimientoEsperado, CancellationToken.None);

        //ASSERT
        mockExpedienteAlumno.Verify(s =>
                s.AddSeguimientoNoUser(It.Is<int>(i => i == idTipoSeguimientoEsperado), It.IsAny<string>(), null,
                    It.IsAny<string>(), It.IsAny<string>()),
            Times.Once);
        mockIMediator.Verify(x => x.Send(It.IsAny<HasPrimeraMatriculaExpedienteQuery>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact(DisplayName = "Cuando se envía el nro versión Se añade el seguimiento")]
    public async Task AddSeguimientoExpediente_ConNroVersion()
    {
        //ARRANGE
        const int idTipoSeguimientoEsperado = TipoSeguimientoExpediente.ExpedienteCreado;
        var request = new CreateExpedienteAlumnoCommand
        {
            NroVersion = 1
        };
        var mockExpedienteAlumno = new Mock<ExpedienteAlumno>
        {
            CallBase = true
        };
        mockExpedienteAlumno.Setup(s =>
            s.AddSeguimientoNoUser(It.Is<int>(i => i == idTipoSeguimientoEsperado), It.IsAny<string>(), null,
                    It.IsAny<string>(), It.IsAny<string>()));
        var mockIStringLocalizer = new Mock<IStringLocalizer<CreateExpedienteAlumnoCommandHandler>>
        {
            CallBase = true
        };
        var mockIMediator = new Mock<IMediator>
        {
            CallBase = true
        };
        var mockILogger = new Mock<ILogger<CreateExpedienteAlumnoCommandHandler>>
        {
            CallBase = true
        };
        var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
        {
            CallBase = true
        };
        var sut = new CreateExpedienteAlumnoCommandHandler(Context, mockIStringLocalizer.Object,
            mockIMediator.Object, mockILogger.Object, mockIErpAcademicoServiceClient.Object);
        mockIMediator.Setup(x => x.Send(It.IsAny<HasPrimeraMatriculaExpedienteQuery>(),
            It.IsAny<CancellationToken>())).ReturnsAsync(false);

        //ACT
        await sut.AddSeguimientoExpediente(mockExpedienteAlumno.Object,
            request, idTipoSeguimientoEsperado, CancellationToken.None);

        //ASSERT
        mockExpedienteAlumno.Verify(s =>
                s.AddSeguimientoNoUser(It.Is<int>(i => i == idTipoSeguimientoEsperado), It.IsAny<string>(), null,
                    It.IsAny<string>(), It.IsAny<string>()),
            Times.Once);
        mockIMediator.Verify(x => x.Send(It.IsAny<HasPrimeraMatriculaExpedienteQuery>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact(DisplayName = "Cuando es primera matrícula se guarda el seguimiento de expediente creado")]
    public async Task AddSeguimientoExpediente_ExpedienteCreado()
    {
        //ARRANGE
        const int idTipoSeguimientoEsperado = TipoSeguimientoExpediente.ExpedienteCreado;
        var request = new CreateExpedienteAlumnoCommand
        {
            NroVersion = 1,
            IdEstadoMatricula = 1
        };
        var mockExpedienteAlumno = new Mock<ExpedienteAlumno>
        {
            CallBase = true
        };
        mockExpedienteAlumno.Setup(s =>
            s.AddSeguimientoNoUser(It.IsAny<int>(), It.IsAny<string>(), null,
                    It.IsAny<string>(), It.IsAny<string>()));
        var mockIStringLocalizer = new Mock<IStringLocalizer<CreateExpedienteAlumnoCommandHandler>>
        {
            CallBase = true
        };
        var mockIMediator = new Mock<IMediator>
        {
            CallBase = true
        };
        var mockILogger = new Mock<ILogger<CreateExpedienteAlumnoCommandHandler>>
        {
            CallBase = true
        };
        var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
        {
            CallBase = true
        };
        var sut = new CreateExpedienteAlumnoCommandHandler(Context, mockIStringLocalizer.Object,
            mockIMediator.Object, mockILogger.Object, mockIErpAcademicoServiceClient.Object);
        mockIMediator.Setup(x => x.Send(It.IsAny<HasPrimeraMatriculaExpedienteQuery>(),
                It.IsAny<CancellationToken>())).ReturnsAsync(true);

        //ACT
        await sut.AddSeguimientoExpediente(mockExpedienteAlumno.Object,
            request, idTipoSeguimientoEsperado, CancellationToken.None);

        //ASSERT
        mockExpedienteAlumno.Verify(s =>
                s.AddSeguimientoNoUser(It.IsAny<int>(), It.IsAny<string>(), null,
                    It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(2));
        mockIMediator.Verify(x => x.Send(It.IsAny<HasPrimeraMatriculaExpedienteQuery>(),
                It.IsAny<CancellationToken>()), Times.Once);
    }

    #endregion

    #region AddNewExpediente

    [Fact(DisplayName = "Cuando el tipo de seguimiento es modificación No realiza ninguna acción")]
    public async Task AddNewExpediente_SinAccion()
    {
        //ARRANGE
        var mockIStringLocalizer = new Mock<IStringLocalizer<CreateExpedienteAlumnoCommandHandler>>
        {
            CallBase = true
        };
        var mockIMediator = new Mock<IMediator>
        {
            CallBase = true
        };
        var mockILogger = new Mock<ILogger<CreateExpedienteAlumnoCommandHandler>>
        {
            CallBase = true
        };
        var sut = new CreateExpedienteAlumnoCommandHandler(Context, mockIStringLocalizer.Object,
            mockIMediator.Object, mockILogger.Object, null);

        //ACT
        await sut.AddNewExpediente(new ExpedienteAlumno(), TipoSeguimientoExpediente.ExpedienteModificadoVersionPlan, CancellationToken.None);

        //ASSERT
        Assert.False(await Context.ExpedientesAlumno.AnyAsync(CancellationToken.None));
    }

    [Fact(DisplayName = "Cuando el tipo de seguimiento es nuevo Añade el expediente")]
    public async Task AddNewExpediente_Ok()
    {
        //ARRANGE
        var mockIStringLocalizer = new Mock<IStringLocalizer<CreateExpedienteAlumnoCommandHandler>>
        {
            CallBase = true
        };
        var mockIMediator = new Mock<IMediator>
        {
            CallBase = true
        };
        var mockILogger = new Mock<ILogger<CreateExpedienteAlumnoCommandHandler>>
        {
            CallBase = true
        };
        var sut = new CreateExpedienteAlumnoCommandHandler(Context, mockIStringLocalizer.Object,
            mockIMediator.Object, mockILogger.Object, null);

        var expedienteEsperado = new ExpedienteAlumno
        {
            Id = 1
        };
        await Context.ExpedientesAlumno.AddAsync(expedienteEsperado);
        await Context.SaveChangesAsync(CancellationToken.None);

        //ACT
        await sut.AddNewExpediente(new ExpedienteAlumno(), TipoSeguimientoExpediente.ExpedienteCreado,
            CancellationToken.None);

        //ASSERT
        var expedientePersistido = await Context.ExpedientesAlumno.FirstOrDefaultAsync(CancellationToken.None);
        Assert.True(await Context.ExpedientesAlumno.AnyAsync(CancellationToken.None));
        if (expedientePersistido != null) Assert.Equal(expedientePersistido.Id, expedienteEsperado.Id);
    }

    #endregion

    #region AssignTitulacionAcceso

    [Fact(DisplayName = "Cuando se asignan los valores a la titulacion de acceso Devuelve entidad seteada")]
    public void AssignTitulacionAcceso_Ok()
    {
        //ARRANGE
        var request = new TitulacionAccesoDto
        {
            CodigoColegiadoProfesional = Guid.NewGuid().ToString(),
            FechaInicioTitulo = DateTime.Now,
            FechafinTitulo = DateTime.Now,
            IdRefTerritorioInstitucionDocente = "56468",
            InstitucionDocente = Guid.NewGuid().ToString(),
            NroSemestreRealizados = 5,
            TipoEstudio = Guid.NewGuid().ToString(),
            IdRefInstitucionDocente = "55421"
        };
        var mockIStringLocalizer = new Mock<IStringLocalizer<CreateExpedienteAlumnoCommandHandler>>
        {
            CallBase = true
        };
        var mockIMediator = new Mock<IMediator>
        {
            CallBase = true
        };
        var mockILogger = new Mock<ILogger<CreateExpedienteAlumnoCommandHandler>>
        {
            CallBase = true
        };
        var sut = new CreateExpedienteAlumnoCommandHandler(Context, mockIStringLocalizer.Object,
            mockIMediator.Object, mockILogger.Object, null);

        var expedienteEsperado = new ExpedienteAlumno();

        //ACT
        sut.AssignTitulacionAcceso(request, expedienteEsperado);

        //ASSERT
        Assert.Equal(expedienteEsperado.TitulacionAcceso.CodigoColegiadoProfesional, request.CodigoColegiadoProfesional);
        Assert.Equal(expedienteEsperado.TitulacionAcceso.FechaInicioTitulo, request.FechaInicioTitulo);
        Assert.Equal(expedienteEsperado.TitulacionAcceso.FechafinTitulo, request.FechafinTitulo);
        Assert.Equal(expedienteEsperado.TitulacionAcceso.IdRefTerritorioInstitucionDocente, request.IdRefTerritorioInstitucionDocente);
        Assert.Equal(expedienteEsperado.TitulacionAcceso.InstitucionDocente, request.InstitucionDocente);
        Assert.Equal(expedienteEsperado.TitulacionAcceso.NroSemestreRealizados, request.NroSemestreRealizados);
        Assert.Equal(expedienteEsperado.TitulacionAcceso.CodigoColegiadoProfesional, request.CodigoColegiadoProfesional);
        Assert.Equal(expedienteEsperado.TitulacionAcceso.TipoEstudio, request.TipoEstudio);
        Assert.Equal(expedienteEsperado.TitulacionAcceso.IdRefInstitucionDocente, request.IdRefInstitucionDocente);
    }

    #endregion

    #region AddEspecializacionesHitosAsync

    [Fact(DisplayName = "Cuando se agregan especializaciones que no existen para el expediente Ejecuta metodo")]
    public async Task AddEspecializaciones_Ok()
    {
        //ARRANGE
        var mockExpedienteAlumno = new Mock<ExpedienteAlumno> { CallBase = true };
        mockExpedienteAlumno.SetupAllProperties();
        mockExpedienteAlumno.Object.Id = 1;
        mockExpedienteAlumno.Setup(s => s.HasEspecializacion(It.IsAny<string>())).Returns(false);
        mockExpedienteAlumno.Setup(s => s.AddEspecializacion(It.IsAny<string>()));

        var request = new List<ExpedienteEspecializacionDto>
        {
            new()
            {
                IdRefEspecializacion = "123"
            },
            new()
            {
                IdRefEspecializacion = "321"
            }
        };
        var mockIStringLocalizer = new Mock<IStringLocalizer<CreateExpedienteAlumnoCommandHandler>>
        {
            CallBase = true
        };
        var mockIMediator = new Mock<IMediator>
        {
            CallBase = true
        };
        var mockILogger = new Mock<ILogger<CreateExpedienteAlumnoCommandHandler>>
        {
            CallBase = true
        };
        var sut = new CreateExpedienteAlumnoCommandHandler(Context, mockIStringLocalizer.Object,
            mockIMediator.Object, mockILogger.Object, null);
        await Context.ExpedientesAlumno.AddAsync(mockExpedienteAlumno.Object);
        await Context.SaveChangesAsync(CancellationToken.None);

        //ACT
        sut.AddEspecializaciones(mockExpedienteAlumno.Object, request);

        //ASSERT
        mockExpedienteAlumno.Verify(s => s.HasEspecializacion(It.IsAny<string>()), Times.Exactly(2));
        mockExpedienteAlumno.Verify(s => s.AddEspecializacion(It.IsAny<string>()), Times.Exactly(2));
    }

    [Fact(DisplayName = "Cuando se agregan especializaciones que ya existen para el expediente no Ejecuta metodo")]
    public async Task AddEspecializaciones_NoAgregaEspecializaciones()
    {
        //ARRANGE
        var mockExpedienteAlumno = new Mock<ExpedienteAlumno> { CallBase = true };
        mockExpedienteAlumno.SetupAllProperties();
        mockExpedienteAlumno.Object.Id = 1;
        mockExpedienteAlumno.Setup(s => s.HasEspecializacion(It.IsAny<string>())).Returns(true);
        mockExpedienteAlumno.Setup(s => s.AddEspecializacion(It.IsAny<string>()));

        var request = new List<ExpedienteEspecializacionDto>
        {
            new()
            {
                IdRefEspecializacion = "123"
            },
            new()
            {
                IdRefEspecializacion = "321"
            }
        };
        var mockIStringLocalizer = new Mock<IStringLocalizer<CreateExpedienteAlumnoCommandHandler>>
        {
            CallBase = true
        };
        var mockIMediator = new Mock<IMediator>
        {
            CallBase = true
        };
        var mockILogger = new Mock<ILogger<CreateExpedienteAlumnoCommandHandler>>
        {
            CallBase = true
        };
        var sut = new CreateExpedienteAlumnoCommandHandler(Context, mockIStringLocalizer.Object,
            mockIMediator.Object, mockILogger.Object, null);
        await Context.ExpedientesAlumno.AddAsync(mockExpedienteAlumno.Object);
        await Context.SaveChangesAsync(CancellationToken.None);

        //ACT
        sut.AddEspecializaciones(mockExpedienteAlumno.Object, request);

        //ASSERT
        mockExpedienteAlumno.Verify(s => s.HasEspecializacion(It.IsAny<string>()), Times.Exactly(2));
        mockExpedienteAlumno.Verify(s => s.AddEspecializacion(It.IsAny<string>()), Times.Never);
    }

    #endregion

    #region AssignExpedienteAlumnoRelacionado

    [Fact(DisplayName = "Cuando no existen IdsPlanes para relacionar el expediente Termina el proceso")]

    public async Task AssignExpedienteAlumnoRelacionado_Null()
    {
        //ARRANGE
        var request = new CreateExpedienteAlumnoCommand
        {
            IdRefIntegracionAlumno = "100"
        };

        var expedienteAlumno = new ExpedienteAlumno();
        var mockIStringLocalizer = new Mock<IStringLocalizer<CreateExpedienteAlumnoCommandHandler>>
        {
            CallBase = true
        };
        var mockIMediator = new Mock<IMediator>
        {
            CallBase = true
        };
        var mockILogger = new Mock<ILogger<CreateExpedienteAlumnoCommandHandler>>
        {
            CallBase = true
        };
        var sut = new CreateExpedienteAlumnoCommandHandler(Context, mockIStringLocalizer.Object,
            mockIMediator.Object, mockILogger.Object, null);

        //ACT
        await sut.AssignExpedienteAlumnoRelacionado(request, expedienteAlumno, CancellationToken.None);

        //ASSERT
        Assert.Null(request.IdsPlanes);
    }

    [Fact(DisplayName = "Cuando existe un Expediente relacionado por Alumno y Plan lo inserta en la Tabla de RelacionesExpedientes")]

    public async Task AssignExpedienteAlumnoRelacionado_Ok()
    {
        //ARRANGE
        var request = new CreateExpedienteAlumnoCommand
        {
            IdsPlanes = new List<int> { 20, 10, 5 },
            IdRefIntegracionAlumno = "100"
        };

        var tipoRelacionExpediente = new TipoRelacionExpediente
        {
            Id = TipoRelacionExpediente.CambioPlan
        };
        await Context.TiposRelacionesExpediente.AddAsync(tipoRelacionExpediente);

        var expedienteRelacionado = new ExpedienteAlumno
        {
            IdRefIntegracionAlumno = "100",
            IdRefPlan = "10"
        };
        var expedienteRelacionado2 = new ExpedienteAlumno
        {
            IdRefIntegracionAlumno = "100",
            IdRefPlan = "20"
        };
        var expedientesRelacionados = new List<ExpedienteAlumno> { expedienteRelacionado, expedienteRelacionado2 };
        await Context.ExpedientesAlumno.AddRangeAsync(expedientesRelacionados);

        await Context.SaveChangesAsync(CancellationToken.None);

        var expedienteAlumno = new ExpedienteAlumno
        {
            IdRefIntegracionAlumno = "100",
            IdRefPlan = "150"
        };
        var mockIStringLocalizer = new Mock<IStringLocalizer<CreateExpedienteAlumnoCommandHandler>>
        {
            CallBase = true
        };
        var mockIMediator = new Mock<IMediator>
        {
            CallBase = true
        };
        var mockILogger = new Mock<ILogger<CreateExpedienteAlumnoCommandHandler>>
        {
            CallBase = true
        };
        var sut = new CreateExpedienteAlumnoCommandHandler(Context, mockIStringLocalizer.Object,
            mockIMediator.Object, mockILogger.Object, null);

        //ACT
        await sut.AssignExpedienteAlumnoRelacionado(request, expedienteAlumno, CancellationToken.None);
        await Context.SaveChangesAsync(CancellationToken.None);

        //ASSERT
        Assert.True(await Context.RelacionesExpedientes.AnyAsync(CancellationToken.None));

        var relacionExpedientePersistido = await Context.RelacionesExpedientes.FirstOrDefaultAsync(CancellationToken.None);
        if (relacionExpedientePersistido != null)
        {
            Assert.Equal(relacionExpedientePersistido.TipoRelacion, tipoRelacionExpediente);
            Assert.Equal(relacionExpedientePersistido.ExpedienteAlumnoRelacionado, expedienteRelacionado2);
            Assert.Equal(relacionExpedientePersistido.ExpedienteAlumno, expedienteAlumno);
        }
    }

    #endregion

    #region AddHitosConseguidos

    [Fact(DisplayName = "Cuando se agregan Hitos Conseguidos para el expediente Ejecuta método")]
    public async Task AddHitosConseguidos_Ok()
    {
        //ARRANGE
        await Context.TiposHitoConseguidos.AddAsync(new TipoHitoConseguido
        {
            Id = TipoHitoConseguido.PreMatriculado,
            Nombre = "Prematriculado"
        });
        var fechaInicio = DateTime.Now;
        await Context.SaveChangesAsync(CancellationToken.None);

        var mockExpedienteAlumno = new Mock<ExpedienteAlumno> { CallBase = true };
        mockExpedienteAlumno.SetupAllProperties();
        mockExpedienteAlumno.Setup(s => s.AddHitosConseguidos(It.IsAny<TipoHitoConseguido>(), It.IsAny<DateTime>(), It.IsAny<ExpedienteEspecializacion>()));

        var mockIStringLocalizer = new Mock<IStringLocalizer<CreateExpedienteAlumnoCommandHandler>>
        {
            CallBase = true
        };
        var mockIMediator = new Mock<IMediator>
        {
            CallBase = true
        };
        var mockILogger = new Mock<ILogger<CreateExpedienteAlumnoCommandHandler>>
        {
            CallBase = true
        };
        var sut = new CreateExpedienteAlumnoCommandHandler(Context, mockIStringLocalizer.Object,
            mockIMediator.Object, mockILogger.Object, null);

        //ACT
        await sut.AddHitosConseguidos(fechaInicio, mockExpedienteAlumno.Object, CancellationToken.None);

        //ASSERT
        mockExpedienteAlumno.Verify(s => s.AddHitosConseguidos(It.IsAny<TipoHitoConseguido>(), It.IsAny<DateTime>(), It.IsAny<ExpedienteEspecializacion>()), Times.Once);
    }

    [Fact(DisplayName = "Cuando se envía la fecha inicio en Null no inserta Hitos Conseguidos")]
    public async Task HitosConseguidos_NoAgregaSiFechaInicioEsNull_Return()
    {
        //ARRANGE
        var mockExpedienteAlumno = new Mock<ExpedienteAlumno> { CallBase = true };
        mockExpedienteAlumno.Object.Id = 1;
        mockExpedienteAlumno.SetupAllProperties();
        mockExpedienteAlumno.Setup(s => s.AddHitosConseguidos(It.IsAny<TipoHitoConseguido>(), It.IsAny<DateTime>(), It.IsAny<ExpedienteEspecializacion>()));

        var mockIStringLocalizer = new Mock<IStringLocalizer<CreateExpedienteAlumnoCommandHandler>>
        {
            CallBase = true
        };
        var mockIMediator = new Mock<IMediator>
        {
            CallBase = true
        };
        var mockILogger = new Mock<ILogger<CreateExpedienteAlumnoCommandHandler>>
        {
            CallBase = true
        };
        var sut = new CreateExpedienteAlumnoCommandHandler(Context, mockIStringLocalizer.Object,
            mockIMediator.Object, mockILogger.Object, null);

        //ACT
        await sut.AddHitosConseguidos(null, mockExpedienteAlumno.Object, CancellationToken.None);

        //ASSERT
        mockExpedienteAlumno.Verify(s => s.AddHitosConseguidos(It.IsAny<TipoHitoConseguido>(), It.IsAny<DateTime>(), It.IsAny<ExpedienteEspecializacion>()), Times.Never);
    }
    #endregion
}