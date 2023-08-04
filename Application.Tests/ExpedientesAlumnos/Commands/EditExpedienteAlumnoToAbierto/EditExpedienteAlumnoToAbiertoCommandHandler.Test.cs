using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Unir.Expedientes.Application.ExpedientesAlumnos.Commands.EditExpedienteAlumnoToAbierto;
using Unir.Expedientes.Application.Tests.Common;
using Unir.Expedientes.Domain.Entities;
using Unir.Framework.ApplicationSuperTypes.Exceptions;
using Xunit;

namespace Unir.Expedientes.Application.Tests.ExpedientesAlumnos.Commands.EditExpedienteAlumnoToAbierto;

[Collection("CommonTestCollection")]
public class EditExpedienteAlumnoToAbiertoCommandHandlerTest : TestBase
{
    #region Handle

    [Fact(DisplayName = "Cuando se intenta editar el expediente para ponerlo en abierto y no existe Devuelve error")]
    public async Task Handle_NotFoundException()
    {
        //ARRANGE
        await Context.ExpedientesAlumno.AddAsync(new ExpedienteAlumno
        {
            Id = 1,
            FechaApertura = new DateTime(2020, 1, 1)
        }, CancellationToken.None);
        await Context.SaveChangesAsync(CancellationToken.None);

        var request = new EditExpedienteAlumnoToAbiertoCommand
        {
            Id = 2,
            FechaApertura = DateTime.Now
        };

        var sut = new EditExpedienteAlumnoToAbiertoCommandHandler(Context);

        //ACT
        var exception = await Record.ExceptionAsync(async () =>
        {
            await sut.Handle(request, CancellationToken.None);
        });

        //ASSERT
        Assert.IsType<NotFoundException>(exception);
        Assert.Equal(new NotFoundException(nameof(ExpedienteAlumno), request.Id).Message,
            exception.Message);
    }

    [Fact(DisplayName = "Cuando se edita el expediente para ponerlo en abierto pero no se envía la fecha apertura Devuelve ok")]
    public async Task Handle_NoActualizaFecha_Ok()
    {
        //ARRANGE
        var fechaEsperada = new DateTime(2020, 1, 1);
        await Context.EstadosExpedientes.AddAsync(new EstadoExpediente
        {
            Id = EstadoExpediente.Abierto
        });
        await Context.ExpedientesAlumno.AddAsync(new ExpedienteAlumno
        {
            Id = 1,
            FechaApertura = fechaEsperada
        }, CancellationToken.None);
        await Context.SaveChangesAsync(CancellationToken.None);

        var request = new EditExpedienteAlumnoToAbiertoCommand
        {
            Id = 1
        };

        var sut = new Mock<EditExpedienteAlumnoToAbiertoCommandHandler>(Context)
        {
            CallBase = true
        };
        sut.Setup(s => s.AddHitosConseguidos(It.IsAny<DateTime?>(), It.IsAny<ExpedienteAlumno>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        //ACT
        await sut.Object.Handle(request, CancellationToken.None);

        //ASSERT
        var expedientePersistido = await Context.ExpedientesAlumno.FirstAsync(CancellationToken.None);
        Assert.NotNull(expedientePersistido.FechaApertura);
        Assert.Equal(fechaEsperada, expedientePersistido.FechaApertura);
        sut.Verify(s => s.AddHitosConseguidos(It.IsAny<DateTime?>(), It.IsAny<ExpedienteAlumno>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact(DisplayName = "Cuando se edita el expediente para ponerlo en abierto y se actualiza la fecha apertura Devuelve ok")]
    public async Task Handle__ActualizaFecha_Ok()
    {
        //ARRANGE
        await Context.EstadosExpedientes.AddAsync(new EstadoExpediente
        {
            Id = EstadoExpediente.Abierto
        });
        await Context.ExpedientesAlumno.AddAsync(new ExpedienteAlumno
        {
            Id = 1,
            FechaApertura = new DateTime(2022, 5, 5)
        }, CancellationToken.None);
        await Context.SaveChangesAsync(CancellationToken.None);

        var fechaEsperada = new DateTime(2020, 1, 1);
        var request = new EditExpedienteAlumnoToAbiertoCommand
        {
            Id = 1,
            FechaApertura = fechaEsperada
        };

        var sut = new Mock<EditExpedienteAlumnoToAbiertoCommandHandler>(Context)
        {
            CallBase = true
        };
        sut.Setup(s => s.AddHitosConseguidos(It.IsAny<DateTime?>(), It.IsAny<ExpedienteAlumno>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        //ACT
        await sut.Object.Handle(request, CancellationToken.None);

        //ASSERT
        var expedientePersistido = await Context.ExpedientesAlumno.FirstAsync(CancellationToken.None);
        Assert.NotNull(expedientePersistido.FechaApertura);
        Assert.Equal(fechaEsperada, expedientePersistido.FechaApertura);
    }

    #endregion

    #region AddHitosConseguidos

    [Fact(DisplayName = "Cuando se agregan Hitos Conseguidos Tipo Primera Matrícula para el expediente si no tiene un tipo similar")]
    public async Task AddHitosConseguidos_Tipo_PrimeraMatricula_Ok()
    {
        //ARRANGE
        var idExpedienteAlumno = 1;
        await Context.TiposHitoConseguidos.AddAsync(new TipoHitoConseguido
        {
            Id = TipoHitoConseguido.PrimeraMatricula,
            Nombre = "Primera Matrícula"
        });
        await Context.HitosConseguidos.AddAsync(new HitoConseguido
        {
            TipoConseguido = new TipoHitoConseguido
            {
                Id = TipoHitoConseguido.PreMatriculado,
            },
            ExpedienteAlumno = new ExpedienteAlumno
            {
                Id = idExpedienteAlumno
            }
        });
        var fechaRecepcionMensaje = DateTime.Now;
        await Context.SaveChangesAsync(CancellationToken.None);

        var mockExpedienteAlumno = new Mock<ExpedienteAlumno> { CallBase = true };
        mockExpedienteAlumno.Object.Id = idExpedienteAlumno;
        mockExpedienteAlumno.SetupAllProperties();
        mockExpedienteAlumno.Setup(s => s.AddHitosConseguidos(It.IsAny<TipoHitoConseguido>(), It.IsAny<DateTime>(), It.IsAny<ExpedienteEspecializacion>()));


        var sut = new EditExpedienteAlumnoToAbiertoCommandHandler(Context);

        //ACT
        await sut.AddHitosConseguidos(fechaRecepcionMensaje, mockExpedienteAlumno.Object, CancellationToken.None);

        //ASSERT
        mockExpedienteAlumno.Verify(s => s.AddHitosConseguidos(It.IsAny<TipoHitoConseguido>(), It.IsAny<DateTime>(), It.IsAny<ExpedienteEspecializacion>()), Times.Once);
    }

    [Fact(DisplayName = "Cuando se agregan Hitos Conseguidos Tipo Matriculado si existiera un Tipo de Primera Matrícula para el expediente")]
    public async Task AddHitosConseguidos_Tipo_Matriculado_Ok()
    {
        //ARRANGE
        var idExpedienteAlumno = 1;
        await Context.TiposHitoConseguidos.AddAsync(new TipoHitoConseguido
        {
            Id = TipoHitoConseguido.Matriculado,
            Nombre = "Primera Matrícula"
        });
        await Context.HitosConseguidos.AddAsync(new HitoConseguido
        {
            TipoConseguido = new TipoHitoConseguido
            {
                Id = TipoHitoConseguido.PrimeraMatricula,
            },
            ExpedienteAlumno = new ExpedienteAlumno
            {
                Id = idExpedienteAlumno
            }
        });
        var fechaRecepcionMensaje = DateTime.Now;
        await Context.SaveChangesAsync(CancellationToken.None);

        var mockExpedienteAlumno = new Mock<ExpedienteAlumno> { CallBase = true };
        mockExpedienteAlumno.Object.Id = idExpedienteAlumno;
        mockExpedienteAlumno.SetupAllProperties();
        mockExpedienteAlumno.Setup(s => s.AddHitosConseguidos(It.IsAny<TipoHitoConseguido>(), It.IsAny<DateTime>(), It.IsAny<ExpedienteEspecializacion>()));


        var sut = new EditExpedienteAlumnoToAbiertoCommandHandler(Context);

        //ACT
        await sut.AddHitosConseguidos(fechaRecepcionMensaje, mockExpedienteAlumno.Object, CancellationToken.None);

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

        var sut = new EditExpedienteAlumnoToAbiertoCommandHandler(Context);

        //ACT
        await sut.AddHitosConseguidos(null, mockExpedienteAlumno.Object, CancellationToken.None);

        //ASSERT
        mockExpedienteAlumno.Verify(s => s.AddHitosConseguidos(It.IsAny<TipoHitoConseguido>(), It.IsAny<DateTime>(), It.IsAny<ExpedienteEspecializacion>()), Times.Never);
    }
    #endregion
}