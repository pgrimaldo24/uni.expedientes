using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Unir.Expedientes.Application.Common.Interfaces;
using Unir.Expedientes.Application.Common.Models.ErpAcademico.PlanesEstudio;
using Unir.Expedientes.Application.Common.Models.ExpedientesGestorUnir;
using Unir.Expedientes.Application.Common.Parameters;
using Unir.Expedientes.Application.ExpedientesAlumnos.Queries.CanQualifyAlumnoInPlanEstudio;
using Unir.Expedientes.Domain.Entities;
using Unir.Framework.ApplicationSuperTypes.Exceptions;

namespace Unir.Expedientes.Application.ExpedientesAlumnos.Commands.SetEstadoHitosEspecializacionesExpedienteAlumno
{
    public class SetEstadoHitosEspecializacionesExpedienteAlumnoCommandHandler : IRequestHandler<SetEstadoHitosEspecializacionesExpedienteAlumnoCommand>
    {
        private readonly IExpedientesContext _context;
        private readonly IExpedientesGestorUnirServiceClient _expedientesGestorUnirServiceClient;
        private readonly IStringLocalizer<SetEstadoHitosEspecializacionesExpedienteAlumnoCommandHandler> _localizer;
        private readonly IErpAcademicoServiceClient _erpAcademicoServiceClient;

        public SetEstadoHitosEspecializacionesExpedienteAlumnoCommandHandler(IExpedientesContext context,
            IExpedientesGestorUnirServiceClient expedientesGestorUnirServiceClient,
            IStringLocalizer<SetEstadoHitosEspecializacionesExpedienteAlumnoCommandHandler> localizer,
            IErpAcademicoServiceClient erpAcademicoServiceClient)
        {
            _context = context;
            _expedientesGestorUnirServiceClient = expedientesGestorUnirServiceClient;
            _localizer = localizer;
            _erpAcademicoServiceClient = erpAcademicoServiceClient;
        }

        public async Task<Unit> Handle(SetEstadoHitosEspecializacionesExpedienteAlumnoCommand request, CancellationToken cancellationToken)
        {
            var resultExpedienteGestor =
                await _expedientesGestorUnirServiceClient.GetExpedienteGestorFormatoErp(
                    request.ExpedienteAlumno.IdRefIntegracionAlumno, int.Parse(request.ExpedienteAlumno.IdRefPlan));
            if (resultExpedienteGestor.HasErrors)
                throw new BadRequestException(_localizer[$"[Expedientes Gestor]: {string.Join(", ", resultExpedienteGestor.Errors)}."]);

            var resultPlanSuperado = await GetPlanSurpassedErpAsync(request.ExpedienteAlumno, resultExpedienteGestor.Value);
            var estadoExpediente = await GetEstadoExpedienteAsync(resultExpedienteGestor.Value, cancellationToken);
            request.ExpedienteAlumno.Estado = estadoExpediente;
            const string descripcion = "Estado - Hitos";

            AssignExpedienteAlumno(request.ExpedienteAlumno, resultExpedienteGestor.Value);
            await AddHitosConseguidos(request.ExpedienteAlumno, cancellationToken);
            var descripcionEspecialización = await AddEspecializacionesHitosAsync(request.ExpedienteAlumno,
                resultPlanSuperado?.EsPlanSuperado?.ElementosSuperados?.HitosObtenidos, resultExpedienteGestor.Value,
                cancellationToken);
            AddSeguimientoExpediente(request.ExpedienteAlumno, $"{descripcion}{descripcionEspecialización}");

            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }

        protected internal virtual async Task AddHitosConseguidos(ExpedienteAlumno expedienteAlumno, CancellationToken cancellationToken)
        {
            var hitosConseguidos = new List<HitoConseguido>();

            var hitoPrematriculado = await GetTipoHitoByIdAsync(TipoHitoConseguido.PreMatriculado, cancellationToken);
            var fechaPreMatricula =
                await _erpAcademicoServiceClient.GetFirstFechaPrimerMatricula(expedienteAlumno.IdRefIntegracionAlumno,
                    expedienteAlumno.IdRefPlan);
            if (fechaPreMatricula != null)
                hitosConseguidos.Add(AssignNewHitoConseguido((DateTime)fechaPreMatricula, hitoPrematriculado));

            if (expedienteAlumno.FechaApertura != null)
            {
                var hitoPrimeraMatricula = await GetTipoHitoByIdAsync(TipoHitoConseguido.PrimeraMatricula, cancellationToken);
                hitosConseguidos.Add(AssignNewHitoConseguido((DateTime)expedienteAlumno.FechaApertura, hitoPrimeraMatricula));
            }

            if (expedienteAlumno.FechaTrabajoFinEstudio != null)
            {
                var hitoTrabajoFinEstudio = await GetTipoHitoByIdAsync(TipoHitoConseguido.TrabajoFinEstudio, cancellationToken);
                hitosConseguidos.Add(AssignNewHitoConseguido((DateTime)expedienteAlumno.FechaTrabajoFinEstudio, hitoTrabajoFinEstudio));
            }

            if (expedienteAlumno.FechaExpedicion != null)
            {
                var hitoTasasAbonadas = await GetTipoHitoByIdAsync(TipoHitoConseguido.TasasAbonadas, cancellationToken);
                hitosConseguidos.Add(AssignNewHitoConseguido((DateTime)expedienteAlumno.FechaExpedicion, hitoTasasAbonadas));
            }

            if (expedienteAlumno.Estado.Id == EstadoExpediente.Cerrado && expedienteAlumno.FechaFinalizacion != null)
            {
                var hitoCerrado = await GetTipoHitoByIdAsync(TipoHitoConseguido.Cerrado, cancellationToken);
                hitosConseguidos.Add(AssignNewHitoConseguido((DateTime)expedienteAlumno.FechaFinalizacion, hitoCerrado));
            }

            expedienteAlumno.HitosConseguidos = hitosConseguidos;
        }

        protected internal virtual async Task<string> AddEspecializacionesHitosAsync(ExpedienteAlumno expedienteAlumno,
            List<HitoErpAcademicoModel> hitosObtenidos, ExpedienteExpedientesIntegrationModel expedicion,
            CancellationToken cancellationToken)
        {
            if (hitosObtenidos == null) return string.Empty;
            
            var tieneEspecializacionesHitosObtenidos = false;

            var idsEspecializacionesHitosObtenidos = hitosObtenidos
                .Where(ho => ho.Especializacion != null)
                .Select(ho => ho.Especializacion.Id).Distinct().ToArray();

            var expedientesEspecializaciones = await _context.ExpedientesEspecializaciones.AsQueryable()
                .Where(ee =>
                    ee.ExpedienteAlumno.Id == expedienteAlumno.Id &&
                    idsEspecializacionesHitosObtenidos.Contains(Convert.ToInt32(ee.IdRefEspecializacion)))
                .ToListAsync(cancellationToken);

            var hitoEspecializacion = await GetTipoHitoByIdAsync(TipoHitoConseguido.Especializacion, cancellationToken);

            foreach (var hitoObtenido in hitosObtenidos)
            {
                var especializacion = hitoObtenido.Especializacion;
                if (especializacion == null) continue;

                var expedienteEspecializacion =
                    expedientesEspecializaciones.FirstOrDefault(ee =>
                        ee.IdRefEspecializacion == especializacion.Id.ToString());
                if (expedienteEspecializacion == null)
                {
                    expedienteEspecializacion = new ExpedienteEspecializacion
                    {
                        IdRefEspecializacion = especializacion.Id.ToString()
                    };
                    expedienteAlumno.ExpedientesEspecializaciones.Add(expedienteEspecializacion);
                    tieneEspecializacionesHitosObtenidos = true;
                }

                if (expedienteAlumno.FechaApertura == null) continue;

                var hitoConseguido = new HitoConseguido
                {
                    Nombre = especializacion.Nombre,
                    FechaInicio = (DateTime)expedienteAlumno.FechaApertura,
                    FechaFin = expedicion.ItinerariosFinalizados
                        .FirstOrDefault(itf => itf.IdEspecializacionErp == especializacion.Id.ToString())?.FechaFin,
                    TipoConseguido = hitoEspecializacion,
                    ExpedienteEspecializacion = expedienteEspecializacion
                };
                expedienteAlumno.HitosConseguidos.Add(hitoConseguido);
            }

            return tieneEspecializacionesHitosObtenidos ? " - Especialización" : string.Empty;
        }

        protected internal virtual HitoConseguido AssignNewHitoConseguido(DateTime fechaInicioHito, TipoHitoConseguido tipoHito)
        {
            return new HitoConseguido
            {
                TipoConseguido = tipoHito,
                Nombre = tipoHito.Nombre,
                FechaInicio = fechaInicioHito
            };
        }

        protected internal virtual async Task<TipoHitoConseguido> GetTipoHitoByIdAsync(int idTipoHito, CancellationToken cancellationToken)
        {
            return await _context.TiposHitoConseguidos.FirstOrDefaultAsync(thc => thc.Id == idTipoHito,
                cancellationToken);
        }

        protected internal virtual void AssignExpedienteAlumno(ExpedienteAlumno expedienteAlumno,
            ExpedienteExpedientesIntegrationModel expedicion)
        {
            if (expedicion == null) return;

            expedienteAlumno.FechaFinalizacion = expedicion.FechaFinEstudio;
            expedienteAlumno.FechaExpedicion = expedicion.FechaExpedicion;
            expedienteAlumno.FechaTrabajoFinEstudio = expedicion.FechaTfmTfg;
            expedienteAlumno.TituloTrabajoFinEstudio = expedicion.TituloTfmTfg;
            expedienteAlumno.NotaMedia = expedicion.NotaMedia;
        }

        protected internal virtual async Task<EstadoExpediente> GetEstadoExpedienteAsync(
            ExpedienteExpedientesIntegrationModel expedicion, CancellationToken cancellationToken)
        {
            var estado = EstadoExpediente.Abierto;
            if (expedicion != null && expedicion.FechaFinEstudio != null &&
                expedicion.FechaExpedicion != null && expedicion.FechaTfmTfg != null)
                estado = EstadoExpediente.Cerrado;

            return await _context.EstadosExpedientes.FirstOrDefaultAsync(ee => ee.Id == estado, cancellationToken);
        }

        protected internal virtual void AddSeguimientoExpediente(ExpedienteAlumno expedienteAlumno, string descripcion)
        {
            descripcion = $"Actualizado: {descripcion}.";
            expedienteAlumno.AddSeguimientoNoUser(TipoSeguimientoExpediente.ExpedienteActualizadoEnProcesoMasivo, descripcion);
        }

        protected internal virtual async Task<ExpedienteAlumnoTitulacionPlanDto> GetPlanSurpassedErpAsync(ExpedienteAlumno expedienteAlumno,
            ExpedienteExpedientesIntegrationModel expedienteExpedientesIntegrationModel)
        {
            var result = new ExpedienteAlumnoTitulacionPlanDto();

            if (expedienteExpedientesIntegrationModel.Asignaturas == null ||
                !expedienteExpedientesIntegrationModel.Asignaturas.Any()) return result;

            var asignaturasExpedientes = expedienteExpedientesIntegrationModel.Asignaturas.ToList();

            var filtersPlanSuperadoParameters = new EsPlanSuperadoParameters
            {
                IdNodo = int.Parse(expedienteAlumno.IdRefNodo),
                IdVersionPlan = !string.IsNullOrWhiteSpace(expedienteAlumno.IdRefVersionPlan)
                    ? int.Parse(expedienteAlumno.IdRefVersionPlan)
                    : null,
                IdsAsignaturasPlanes = asignaturasExpedientes.Select(a => a.IdAsignatura).Distinct().ToList()
            };

            var resultPlanSuperado = await
                _erpAcademicoServiceClient.ItIsPlanSurpassed(int.Parse(expedienteAlumno.IdRefPlan), filtersPlanSuperadoParameters);
            result.EsPlanSuperado = resultPlanSuperado;
            return result;
        }
    }
}
