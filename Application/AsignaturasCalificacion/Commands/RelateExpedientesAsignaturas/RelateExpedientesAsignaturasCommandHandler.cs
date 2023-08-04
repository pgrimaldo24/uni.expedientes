using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Unir.Expedientes.Application.Common.Interfaces;
using Unir.Expedientes.Application.Common.Models.ExpedientesGestorUnir;
using Unir.Expedientes.Application.Common.Models.GestorMapeos;
using Unir.Expedientes.Domain.Entities;

namespace Unir.Expedientes.Application.AsignaturasCalificacion.Commands.RelateExpedientesAsignaturas
{
    public class RelateExpedientesAsignaturasCommandHandler
        : IRequestHandler<RelateExpedientesAsignaturasCommand, List<string>>
    {
        private readonly IExpedientesContext _context;
        private readonly IGestorMapeosServiceClient _gestorMapeosServiceClient;
        private readonly IExpedientesGestorUnirServiceClient _expedientesGestorUnirServiceClient;
        private const int CodigoIncorrecto = 1;
        public const int EstadoSolicitudValido = 9;
        public const string TipoSolicitudValido = "Adaptación";
        public ExpedienteAlumno ExpedienteAlumno;
        public List<string> Mensajes;

        public RelateExpedientesAsignaturasCommandHandler(
            IExpedientesContext context,
            IGestorMapeosServiceClient gestorMapeosServiceClient,
            IExpedientesGestorUnirServiceClient expedientesGestorUnirServiceClient)
        {
            _context = context;
            _gestorMapeosServiceClient = gestorMapeosServiceClient;
            _expedientesGestorUnirServiceClient = expedientesGestorUnirServiceClient;
            Mensajes = new List<string>();
        }

        public async Task<List<string>> Handle(RelateExpedientesAsignaturasCommand request, CancellationToken cancellationToken)
        {
            ExpedienteAlumno = request.ExpedienteAlumno;
            var estudios = await _gestorMapeosServiceClient.GetEstudios(
                ExpedienteAlumno.IdRefPlan, ExpedienteAlumno.IdRefVersionPlan);
            if (estudios == null || !estudios.Any())
            {
                Mensajes.Add("No está mapeado el plan de estudio con ningún estudio del Gestor " +
                    "y no se pueden recuperar reconocimientos");
                return Mensajes;
            }

            ReconocimientoGestorModel reconocimiento = null;
            var idEstudioGestor = 0;
            foreach (var estudio in estudios)
            {
                var responseReconocimiento = await _expedientesGestorUnirServiceClient
                    .GetReconocimientos(ExpedienteAlumno.IdRefIntegracionAlumno, estudio.IdEstudioGestor);
                if (responseReconocimiento == null ||
                    responseReconocimiento.CodigoResultado == CodigoIncorrecto)
                    continue;

                idEstudioGestor = estudio.IdEstudioGestor;
                reconocimiento = responseReconocimiento.Reconocimiento;
                break;
            }
            if (reconocimiento == null)
            {
                Mensajes.Add("No se encontraron reconocimientos para el expediente");
                return Mensajes;
            }

            await GetSeminariosReconocimientos(reconocimiento.Seminarios, cancellationToken);
            await GetAsignaturasReconocimientoAdaptacion(reconocimiento.Asignaturas, 
                idEstudioGestor, cancellationToken);
            return Mensajes;
        }

        protected internal virtual async Task GetAsignaturasReconocimientoAdaptacion(
            List<AsignaturaGestorModel> asignaturas, int idEstudioGestor, CancellationToken cancellationToken)
        {
            if (asignaturas == null || !asignaturas.Any()) return;

            var asignaturasReconocimientos = asignaturas.Where(a => a.Reconocimientos
                .Any(r => r.IdEstadoSolicitud == EstadoSolicitudValido && r.TipoSolicitud == TipoSolicitudValido))
                .ToList();
            if (!asignaturasReconocimientos.Any()) return;

            var asignaturasGestor = await _gestorMapeosServiceClient.GetAsignaturas(idEstudioGestor);
            if (asignaturasGestor == null || !asignaturasGestor.Any()) return;

            List<AsignaturaGestorMapeoModel> asignaturasGestorOrigen = null;
            ExpedienteAlumno expedienteAlumnoARelacionar = null;
            var addRelacionAsignatura = false;
            foreach (var asignatura in asignaturasReconocimientos)
            {
                var asignaturaExpediente = GetAsignaturaExpediente(idEstudioGestor, asignaturasGestor, asignatura);
                if (asignaturaExpediente == null) continue;

                if (expedienteAlumnoARelacionar == null)
                {
                    var asignaturaOrigen = await GetAsignaturaOrigen(asignatura.Reconocimientos.First().IdAsignaturaOrigen);
                    if (asignaturaOrigen == null) continue;

                    asignaturasGestorOrigen = await GetAsignaturasGestorOrigen(asignaturaOrigen.AsignaturaUnir.EstudioUnir.Id);
                    if (asignaturasGestorOrigen == null) continue;

                    expedienteAlumnoARelacionar = await GetExpedienteAlumnoARelacionar(
                        asignaturaOrigen.AsignaturaPlan.Plan.Id.ToString(), cancellationToken);

                    if (expedienteAlumnoARelacionar == null) break;
                }

                var hasAsignaturasRelacionadas = await RelacionarAsignaturas(asignaturaExpediente, 
                    asignatura.Reconocimientos, expedienteAlumnoARelacionar, asignaturasGestorOrigen, cancellationToken);                

                    addRelacionAsignatura = hasAsignaturasRelacionadas || addRelacionAsignatura;
            }

            if (addRelacionAsignatura)
            {
                await AssignRelacionExpediente(TipoRelacionExpediente.CambioPlan,
                    expedienteAlumnoARelacionar.Id, cancellationToken);
            }                
        }

        protected internal virtual AsignaturaExpediente GetAsignaturaExpediente(int idEstudioGestor,
            List<AsignaturaGestorMapeoModel> asignaturasGestor, AsignaturaGestorModel asignatura)
        {
            var asignaturaGestor = asignaturasGestor
                    .FirstOrDefault(ag => ag.IdAsignaturaEstudioGestor == asignatura.IdAsignaturaUnir);
            if (asignaturaGestor == null)
            {
                Mensajes.Add($"No se encontró la asignatura con id {asignatura.IdAsignaturaUnir} " +
                    $"en el estudio {idEstudioGestor} del Gestor");
                return null;
            }

            return ExpedienteAlumno.AsignaturasExpedientes.FirstOrDefault(ae =>
                    ae.IdRefAsignaturaPlan == asignaturaGestor.IdAsignaturaPlanErp.ToString());
        }

        protected internal virtual async Task<AsignaturaIntegrationGestorMapeoModel> GetAsignaturaOrigen(int idAsignaturaOrigen)
        {
            var asignaturaOrigen = await _gestorMapeosServiceClient.GetAsignatura(idAsignaturaOrigen);
            if (asignaturaOrigen != null) return asignaturaOrigen;

            Mensajes.Add($"No se encontro la asignatura origen {idAsignaturaOrigen} del reconocimiento");
            return null;
        }

        protected internal virtual async Task<List<AsignaturaGestorMapeoModel>> GetAsignaturasGestorOrigen(
            int idEstudioGestorOrigen)
        {
            var asignaturasGestorOrigen = await _gestorMapeosServiceClient.GetAsignaturas(idEstudioGestorOrigen);
            if (asignaturasGestorOrigen != null && asignaturasGestorOrigen.Any()) return asignaturasGestorOrigen;

            Mensajes.Add($"No se encontraron las asignaturas origen en el estudio {idEstudioGestorOrigen} del Gestor");
            return null;

        }

        protected internal virtual async Task<ExpedienteAlumno> GetExpedienteAlumnoARelacionar(
            string idPlan, CancellationToken cancellationToken)
        {
            return await _context.ExpedientesAlumno
                .Include(ea => ea.AsignaturasExpedientes)
                .FirstOrDefaultAsync(ea
                    => ea.Id != ExpedienteAlumno.Id
                       && ea.IdRefIntegracionAlumno == ExpedienteAlumno.IdRefIntegracionAlumno
                       && ea.IdRefPlan == idPlan, cancellationToken);
        }

        protected internal virtual async Task<bool> RelacionarAsignaturas(AsignaturaExpediente asignaturaExpediente, 
            List<ReconocimientoCommonGestorModel> reconocimientos, ExpedienteAlumno expedienteAlumnoARelacionar, 
            List<AsignaturaGestorMapeoModel> asignaturasGestorOrigen, CancellationToken cancellationToken)
        {
            var hasAsignaturasRelacionadas = false;
            var idEstudioGestorOrigen = asignaturasGestorOrigen.First().IdEstudioGestor;
            foreach (var reconocimiento in reconocimientos)
            {
                var asignaturaGestorOrigen = asignaturasGestorOrigen
                    .FirstOrDefault(ag => ag.IdAsignaturaEstudioGestor == reconocimiento.IdAsignaturaOrigen);
                if (asignaturaGestorOrigen == null)
                {
                    Mensajes.Add($"No se encontró la asignatura origen con id {reconocimiento.IdAsignaturaOrigen} " +
                        $"en el estudio {idEstudioGestorOrigen} del Gestor");
                    continue;
                }

                var asignaturaExpedienteARelacionar = expedienteAlumnoARelacionar.AsignaturasExpedientes.FirstOrDefault(ae =>
                        ae.IdRefAsignaturaPlan == asignaturaGestorOrigen.IdAsignaturaPlanErp.ToString());
                if (asignaturaExpedienteARelacionar == null)
                    continue;

                await AssignAsignaturaExpedienteRelacionada(asignaturaExpediente, asignaturaExpedienteARelacionar, 
                    reconocimiento, cancellationToken);

                asignaturaExpediente.Reconocida = true;
                asignaturaExpediente.SituacionAsignaturaId = SituacionAsignatura.Superada;
                hasAsignaturasRelacionadas = true;
            }

            return hasAsignaturasRelacionadas;
        }

        protected internal virtual async Task AssignAsignaturaExpedienteRelacionada(
            AsignaturaExpediente asignaturaExpedienteOrigen, AsignaturaExpediente asignaturaExpedienteDestino,
            ReconocimientoCommonGestorModel reconocimiento, CancellationToken cancellationToken)
        {
            var asignaturaExpedienteRelacionada = await _context.AsignaturasExpedientesRelacionadas
                .FirstOrDefaultAsync(aer 
                    => aer.AsignaturaExpedienteOrigenId == asignaturaExpedienteOrigen.Id 
                    && aer.AsignaturaExpedienteDestinoId == asignaturaExpedienteDestino.Id, cancellationToken);

            if (asignaturaExpedienteRelacionada != null)
            {
                asignaturaExpedienteRelacionada.Reconocida = false;
                asignaturaExpedienteRelacionada.Adaptada = true;
                asignaturaExpedienteRelacionada.Fecha = Convert.ToDateTime(reconocimiento.FechaFinalizacion);
                return;
            }

            await _context.AsignaturasExpedientesRelacionadas.AddAsync(
                new AsignaturaExpedienteRelacionada
                {
                    AsignaturaExpedienteOrigenId = asignaturaExpedienteOrigen.Id,
                    AsignaturaExpedienteDestinoId = asignaturaExpedienteDestino.Id,
                    Adaptada = true,
                    Fecha = Convert.ToDateTime(reconocimiento.FechaFinalizacion)
                }, cancellationToken);
        }

        protected internal virtual async Task AssignRelacionExpediente(int idTipoRelacion,
             int idExpedienteAlumnoARelacionar, CancellationToken cancellationToken)
        {
            if (!await _context.RelacionesExpedientes.AnyAsync(re
                => re.ExpedienteAlumnoId == ExpedienteAlumno.Id 
                && re.ExpedienteAlumnoRelacionadoId == idExpedienteAlumnoARelacionar, cancellationToken))
            {
                await _context.RelacionesExpedientes.AddAsync(new RelacionExpediente
                {
                    ExpedienteAlumnoId = ExpedienteAlumno.Id,
                    ExpedienteAlumnoRelacionadoId = idExpedienteAlumnoARelacionar,
                    TipoRelacionId = idTipoRelacion
                }, cancellationToken);
            }
        }

        protected internal virtual async Task GetSeminariosReconocimientos(
            List<SeminarioGestorModel> seminarios, CancellationToken cancellationToken)
        {
            if (seminarios == null || !seminarios.Any())
                return;

            var idsExpedientesARelacionar = new List<int>();
            foreach (var seminario in seminarios)
            {
                var estudio = await _gestorMapeosServiceClient.GetEstudio(seminario.IdEstudioSeminario);
                if (estudio == null)
                {
                    idsExpedientesARelacionar.Clear();
                    Mensajes.Add($"No se encontró el estudio {seminario.IdEstudioSeminario} del seminario");
                    break;
                }

                var expedienteAlumnoARelacionar = await GetExpedienteAlumnoARelacionar(
                    estudio.PlantillaEstudioIntegracion.Plan.Id.ToString(), cancellationToken);
                if (expedienteAlumnoARelacionar == null)
                {
                    idsExpedientesARelacionar.Clear();
                    Mensajes.Add("El seminario no existe como expediente del alumno");
                    break;
                }

                idsExpedientesARelacionar.Add(expedienteAlumnoARelacionar.Id);
            }

            foreach (var id in idsExpedientesARelacionar)
            {
                await AssignRelacionExpediente(TipoRelacionExpediente.Seminario, id, cancellationToken);
            }
        }
    }
}
