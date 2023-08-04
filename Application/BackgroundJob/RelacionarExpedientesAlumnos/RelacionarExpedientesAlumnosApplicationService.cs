using MediatR;
using System.Linq;
using Unir.Expedientes.Application.Common.Interfaces;
using Unir.Expedientes.Application.ExpedientesAlumnos.Commands.RelacionarExpedienteAlumno;
using Unir.Expedientes.Application.ExpedientesAlumnos.Queries.GetExpedientesAlumnosARelacionar;
using Unir.Expedientes.Domain.Entities;
using Unir.Framework.ApplicationSuperTypes.Exceptions;
using Unir.Framework.Crosscutting.BackgroundJob;

namespace Unir.Expedientes.Application.BackgroundJob.RelacionarExpedientesAlumnos
{
    public class RelacionarExpedientesAlumnosApplicationService : IRelacionarExpedientesAlumnosApplicationService
    {
        private readonly IExpedientesContext _context;
        private readonly IMediator _mediator;
        private readonly IBackgroundJobService _backgroundJobService;

        public RelacionarExpedientesAlumnosApplicationService(
            IExpedientesContext context, 
            IMediator mediator, 
            IBackgroundJobService backgroundJobService)
        {
            _context = context;
            _mediator = mediator;
            _backgroundJobService = backgroundJobService;
        }

        public string RelacionarExpedientesAlumnos(RelacionarExpedientesAlumnosParameters parameters)
        {
            return _backgroundJobService.EnqueueJob(parameters.AccountId,
                () => ExecuteBackgroundJobRelacionarExpedientesAlumnos(parameters));
        }

        protected internal virtual void ExecuteBackgroundJobRelacionarExpedientesAlumnos(
            RelacionarExpedientesAlumnosParameters parameters)
        {
            var procesoBackgroundData = new RelacionarExpedienteAlumnoBackgroundDto
            {
                Fallidos = 0,
                Correctos = 0,
                TotalExpedientesEncontrados = 0
            };
            _backgroundJobService.SetJobCustomData(procesoBackgroundData);

            var expedientes = _mediator.Send(new GetExpedientesAlumnosARelacionarQuery
            {
                IdRefUniversidad = parameters.IdRefUniversidad,
                IdRefEstudio = parameters.IdRefEstudio,
                FechaAperturaDesde = parameters.FechaAperturaDesde,
                FechaAperturaHasta = parameters.FechaAperturaHasta
            }).Result;

            procesoBackgroundData.TotalExpedientesEncontrados = expedientes.Count;
            if (!expedientes.Any())
            {
                _backgroundJobService.SetJobCustomData(procesoBackgroundData);
                return;
            }

            var tipoRelacionExpediente = _context.TiposRelacionesExpediente.First(e =>
                                            e.Id == TipoRelacionExpediente.CambioPlan);
            foreach (var expediente in expedientes)
            {
                if (string.IsNullOrWhiteSpace(ExecuteRelacionarExpedienteAlumnoCommand(expediente, tipoRelacionExpediente)))
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

        protected internal virtual string ExecuteRelacionarExpedienteAlumnoCommand(
            ExpedienteAlumno expediente, TipoRelacionExpediente tipoRelacionExpediente)
        {
            try
            {
                var unused = _mediator.Send(new RelacionarExpedienteAlumnoCommand(expediente, tipoRelacionExpediente)).Result;
                return null;
            }
            catch (BadRequestException ex)
            {
                return ex.Message;
            }
        }
    }
}
