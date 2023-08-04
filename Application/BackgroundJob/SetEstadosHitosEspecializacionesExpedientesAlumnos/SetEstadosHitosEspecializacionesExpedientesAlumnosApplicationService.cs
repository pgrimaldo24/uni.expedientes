using MediatR;
using System;
using System.Linq;
using Unir.Expedientes.Application.ExpedientesAlumnos.Commands.SetEstadoHitosEspecializacionesExpedienteAlumno;
using Unir.Expedientes.Application.ExpedientesAlumnos.Queries.GetExpedientesSinEstados;
using Unir.Expedientes.Domain.Entities;
using Unir.Framework.ApplicationSuperTypes.Exceptions;
using Unir.Framework.Crosscutting.BackgroundJob;

namespace Unir.Expedientes.Application.BackgroundJob.SetEstadosHitosEspecializacionesExpedientesAlumnos;

public class SetEstadosHitosEspecializacionesExpedientesAlumnosApplicationService :
    ISetEstadosHitosEspecializacionesExpedientesAlumnosApplicationService
{
    private readonly IMediator _mediator;
    private readonly IBackgroundJobService _backgroundJobService;

    public SetEstadosHitosEspecializacionesExpedientesAlumnosApplicationService(IMediator mediator, IBackgroundJobService backgroundJobService)
    {
        _mediator = mediator;
        _backgroundJobService = backgroundJobService;
    }

    public string SetEstadosHitosEspecializacionesDeExpedientesAlumnos(
        SetEstadosHitosEspecializacionesExpedientesAlumnosParameters parameters)
    {
        return _backgroundJobService.EnqueueJob(parameters.AccountId,
            () => ExecuteBackgroundJobSetEstadosHitosEspecializacionesDeExpedientesAlumnos(parameters));
    }

    protected internal virtual void ExecuteBackgroundJobSetEstadosHitosEspecializacionesDeExpedientesAlumnos(
        SetEstadosHitosEspecializacionesExpedientesAlumnosParameters parameters)
    {
        var procesoBackgroundData = new EstadoHitosEspecializacionesExpedienteAlumnoBackgroundDto
        {
            Fallidos = 0,
            Correctos = 0,
            TotalExpedientesEncontrados = 0
        };
        _backgroundJobService.SetJobCustomData(procesoBackgroundData);

        var expedientes = _mediator.Send(new GetExpedientesSinEstadosQuery
        {
            IdRefUniversidad = parameters.IdUniversidad?.ToString(),
            IdRefEstudio = parameters.IdEstudio?.ToString(),
            FechaAperturaDesde = parameters.FechaAperturaDesde,
            FechaAperturaHasta = parameters.FechaAperturaHasta
        }).Result;

        procesoBackgroundData.TotalExpedientesEncontrados = expedientes.Count;
        if (!expedientes.Any())
        {
            _backgroundJobService.SetJobCustomData(procesoBackgroundData);
            return;
        }

        foreach (var expediente in expedientes)
        {
            if (string.IsNullOrWhiteSpace(ExecuteSetEstadoHitosEspecializacionesExpedienteAlumnoCommand(expediente)))
            {
                procesoBackgroundData.Correctos++;
            }
            else
            {
                procesoBackgroundData.Fallidos++;
                procesoBackgroundData.IdsExpedientesFallidos.Add(expediente.Id);
            }
            _backgroundJobService.SetJobCustomData(procesoBackgroundData);
        }
    }

    protected internal virtual string ExecuteSetEstadoHitosEspecializacionesExpedienteAlumnoCommand(ExpedienteAlumno expediente)
    {
        try
        {
            var unused = _mediator.Send(new SetEstadoHitosEspecializacionesExpedienteAlumnoCommand(expediente)).Result;
            return null;
        }
        catch (BadRequestException ex)
        {
            return ex.Message;
        }
        catch (NotFoundException ex)
        {
            return ex.Message;
        }
        catch (ValidationErrorsException ex)
        {
            return ex.Message;
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }
}