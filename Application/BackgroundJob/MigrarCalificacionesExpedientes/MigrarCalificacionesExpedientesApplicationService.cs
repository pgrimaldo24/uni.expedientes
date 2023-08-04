using MediatR;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Unir.Expedientes.Application.AsignaturasCalificacion.Commands.MigrarCalificacionExpediente;
using Unir.Expedientes.Application.Common.Interfaces;
using Unir.Expedientes.Application.Common.Models.ErpAcademico.OfertaEstudio.Migracion;
using Unir.Expedientes.Application.ExpedientesAlumnos.Queries.GetExpedientesMigrarCalificaciones;
using Unir.Expedientes.Application.ExpedientesAlumnos.SharedDtos;
using Unir.Expedientes.Application.Tareas.Commands.CreateTarea;
using Unir.Expedientes.Application.Tareas.Commands.EditTarea;
using Unir.Expedientes.Application.TareasDetalle.Commands.CreateTareaDetalle;
using Unir.Expedientes.Domain.Entities;
using Unir.Framework.ApplicationSuperTypes.Exceptions;
using Unir.Framework.Crosscutting.BackgroundJob;
namespace Unir.Expedientes.Application.BackgroundJob.MigrarCalificacionesExpedientes
{
    public class MigrarCalificacionesExpedientesApplicationService : IMigrarCalificacionesExpedientesApplicationService
    {
        private const string MigrarCalificaciones = "Migrar calificaciones";
        private readonly IMediator _mediator;
        private readonly IBackgroundJobService _backgroundJobService;
        private readonly IErpAcademicoServiceClient _erpAcademicoServiceClient;
        private readonly IExpedientesContext _context;
        private readonly ILogger<MigrarCalificacionesExpedientesApplicationService> _logger;

        public MigrarCalificacionesExpedientesApplicationService(IMediator mediator,
            IBackgroundJobService backgroundJobService,
            IErpAcademicoServiceClient erpAcademicoServiceClient,
            ILogger<MigrarCalificacionesExpedientesApplicationService> logger,
            IExpedientesContext context)
        {
            _mediator = mediator;
            _backgroundJobService = backgroundJobService;
            _erpAcademicoServiceClient = erpAcademicoServiceClient;
            _context = context;
            _logger = logger;
        }

        public string MigrarCalificacionesExpedientes(string identityInfoId, string creadoPor,
            MigrarCalificacionesExpedientesParameters parameters)
        {
            return _backgroundJobService.EnqueueJob(identityInfoId,
                () => ExecuteBackgroundJobMigrarCalificaciones(creadoPor, parameters));
        }
        protected internal virtual void ExecuteBackgroundJobMigrarCalificaciones(string creadoPor,
            MigrarCalificacionesExpedientesParameters parameters)
        {
            var expedientes = _mediator.Send(new GetExpedientesMigrarCalificacionesQuery
            {
                IdRefUniversidad = parameters.IdRefUniversidad,
                IdRefEstudio = parameters.IdRefEstudio
            }).Result;

            if (!expedientes.Any())
            {
                _logger.LogError("En el proceso de migración no se encontraron expedientes " +
                    $"con el IdRefUniversidad:{parameters.IdRefUniversidad} y IdRefEstudio: {parameters.IdRefEstudio}");
                return;
            }
            var tarea = StartMigracionCalificacion(parameters).Result;
            var procesoBackgroundData = new ProcesoBackgroundDto
            {
                CreadoPor = creadoPor,
                Descripcion = MigrarCalificaciones,
                Fallidos = 0,
                Ok = 0,
                Id = _backgroundJobService.GetCurrentJobId(),
                TotalRegistros = expedientes.Count
            };

            _backgroundJobService.SetJobCustomData(procesoBackgroundData);
            ProcesarExpedientes(expedientes, tarea, procesoBackgroundData);
        }

        protected internal virtual async Task<BackgroundDetailDto> ExecuteMigrarCalificacionesCommand(
            ExpedienteAlumno expediente, List<AsignaturaOfertadaMigracionModel> asignaturaOfertadaModel)
        {
            var response = new BackgroundDetailDto();
            try
            {
                var message = await _mediator.Send(new MigrarCalificacionExpedienteCommand(expediente, asignaturaOfertadaModel));
                response.HasWarning = !string.IsNullOrEmpty(message);
                response.Message = message;
                return response;
            }
            catch (BadRequestException ex)
            {
                response.HasError = true;
                response.Message = ex.Message;
                return response;
            }
        }

        private async Task<Tarea> StartMigracionCalificacion(MigrarCalificacionesExpedientesParameters migrarCalificacionesExpedientesParameters)
        {
            var result = await _mediator.Send(new CreateTareaCommand
            {
                JobId = Convert.ToInt32(_backgroundJobService.GetCurrentJobId()),
                FechaInicio = DateTime.UtcNow,
                Total = 0,
                Completadas = 0,
                Fallidas = 0,
                IdRefEstudio = Convert.ToInt32(migrarCalificacionesExpedientesParameters.IdRefEstudio),
                IdRefUniversidad = Convert.ToInt32(migrarCalificacionesExpedientesParameters.IdRefUniversidad)
            }, CancellationToken.None); ;
            return result;
        }

        private void ProcesarExpedientes(List<ExpedienteAlumno> expedientes,
            Tarea tarea, ProcesoBackgroundDto procesoBackgroundData)
        {
            var asignaturasOfertadas = _erpAcademicoServiceClient.GetAsignaturasOfertadasByExpediente(
                expedientes.Select(e => e.Id.ToString()).ToList()).Result;

            if (asignaturasOfertadas is null || !asignaturasOfertadas.Any())
            {
                _logger.LogError("En el proceso de migración no se encontraron asignaturas ofertadas " +
                $"con ids expediente {string.Join(",", expedientes.Select(e => e.Id))}");
                return;
            }

            foreach (var expediente in expedientes)
            {
                var fechaInicio = DateTime.UtcNow;
                var asignaturasOfertadasFiltrado = FiltrarAsignaturasOfertadasPorExpediente(asignaturasOfertadas, expediente.Id);
                var resultMigration = ExecuteMigrarCalificacionesCommand(expediente, asignaturasOfertadasFiltrado).Result;
                var _ = _mediator.Send(new CreateTareaDetalleCommand
                {
                    IdTarea = tarea.Id,
                    IdExpediente = expediente.Id,
                    FechaInicio = fechaInicio,
                    FechaFin = DateTime.UtcNow,
                    CompletadaOk = !resultMigration.HasError,
                    Mensaje = resultMigration.Message
                }, CancellationToken.None).Result;

                if (resultMigration.HasError)
                {
                    procesoBackgroundData.Fallidos++;
                    procesoBackgroundData.Errors.Add(resultMigration.Message);
                    _backgroundJobService.SetJobCustomData(procesoBackgroundData);
                    _logger.LogError(resultMigration.Message);
                    continue;
                }

                if (resultMigration.HasWarning)
                    procesoBackgroundData.Warnings.Add(resultMigration.Message);

                procesoBackgroundData.Ok++;
                _backgroundJobService.SetJobCustomData(procesoBackgroundData);
            }

            var unused = _mediator.Send(new EditTareaCommand
            {
                Completadas = procesoBackgroundData.Ok,
                Fallidas = procesoBackgroundData.Fallidos,
                FechaFin = DateTime.UtcNow,
                Total = procesoBackgroundData.TotalRegistros,
                IdTarea = tarea.Id
            }, CancellationToken.None).Result;

            procesoBackgroundData.IdsExpedientes = expedientes.Select(x => x.Id).ToArray();
        }

        protected internal virtual List<AsignaturaOfertadaMigracionModel> FiltrarAsignaturasOfertadasPorExpediente(
            List<AsignaturaOfertadaMigracionModel> asignaturasOfertadas, int idExpediente)
        {
            var asignaturasMatriculadasPorExpediente = asignaturasOfertadas.SelectMany(ao => ao.AsignaturaMatriculadas
                .Where(am => am.IdRefExpedienteAlumno == idExpediente.ToString() && am.MatriculaActiva)).ToList();

            var idsAsignaturasOfertadas = asignaturasMatriculadasPorExpediente.Select(am => am.IdAsignaturaOfertada).ToList();
            var asignaturasOfertadasFiltrado = asignaturasOfertadas.Where(aof => idsAsignaturasOfertadas.Contains(aof.Id)).ToList();

            foreach (var ofertada in asignaturasOfertadasFiltrado)
            {
                var asignaturaMatriculada = asignaturasMatriculadasPorExpediente.First(x => x.IdAsignaturaOfertada == ofertada.Id);
                ofertada.AsignaturaMatriculadas = new List<AsignaturaMatriculadaMigracionModel>();
                ofertada.AsignaturaMatriculadas.Add(asignaturaMatriculada);
            }
            return asignaturasOfertadasFiltrado;
        }

        public JobDetails GetJobDetails(string procesoId)
        {
            return _backgroundJobService.GetJobDetails(procesoId);
        }
    }
}
