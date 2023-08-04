using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Unir.Expedientes.Application.AsignaturasCalificacion.Commands.RelateExpedientesAsignaturas;
using Unir.Expedientes.Application.AsignaturasCalificacion.Queries.GetCalificacionByNota;
using Unir.Expedientes.Application.AsignaturasCalificacion.Queries.GetCicloPeriodoLectivo;
using Unir.Expedientes.Application.Common.Interfaces;
using Unir.Expedientes.Application.Common.Models.ErpAcademico.OfertaEstudio;
using Unir.Expedientes.Application.Common.Models.ErpAcademico.OfertaEstudio.Migracion;
using Unir.Expedientes.Application.Common.Models.ExpedientesGestorUnir;
using Unir.Expedientes.Application.Matriculacion.Common.Queries.GetAsignaturasAsociadasExpediente;
using Unir.Expedientes.Domain.Entities;
using Unir.Framework.ApplicationSuperTypes.Exceptions;

namespace Unir.Expedientes.Application.AsignaturasCalificacion.Commands.MigrarCalificacionExpediente
{
    public class MigrarCalificacionExpedienteCommandHandler : IRequestHandler<MigrarCalificacionExpedienteCommand, string>
    {
        private readonly IExpedientesContext _context;
        private readonly IMediator _mediator;
        private readonly IExpedientesGestorUnirServiceClient _expedientesGestorUnirServiceClient;
        private readonly IErpAcademicoServiceClient _erpAcademicoServiceClient;
        private readonly IStringLocalizer<MigrarCalificacionExpedienteCommandHandler> _localizer;

        public MigrarCalificacionExpedienteCommandHandler(
            IExpedientesContext context,
            IMediator mediator,
            IExpedientesGestorUnirServiceClient expedientesGestorUnirServiceClient,
            IErpAcademicoServiceClient erpAcademicoServiceClient,
            IStringLocalizer<MigrarCalificacionExpedienteCommandHandler> localizer)
        {
            _context = context;
            _mediator = mediator;
            _expedientesGestorUnirServiceClient = expedientesGestorUnirServiceClient;
            _erpAcademicoServiceClient = erpAcademicoServiceClient;
            _localizer = localizer;
        }

        public async Task<string> Handle(MigrarCalificacionExpedienteCommand request, CancellationToken cancellationToken)
        {
            var expediente = request.ExpedienteAlumno;

            if (!request.AsignaturasOfertadas.Any())
                throw new BadRequestException(_localizer[$"El expediente con id : {expediente.Id}, no cuenta con asignaturas ofertadas"]);

            var expedienteGestor = await _expedientesGestorUnirServiceClient
                .GetExpedienteGestorFormatoErp(expediente.IdRefIntegracionAlumno, int.Parse(expediente.IdRefPlan));

            if (expedienteGestor is null || expedienteGestor.Value.CodigoResultado == ExpedienteExpedientesIntegrationModel.SinResultados)
                throw new BadRequestException(_localizer[$"[Expediente id:{expediente.Id}] - El alumno con id {expediente.IdRefIntegracionAlumno} no fue encontrado en gestor Erp"]);
            var gestor = expedienteGestor.Value;

            expediente.FechaFinalizacion = gestor.FechaFinEstudio;
            expediente.NotaMedia = gestor.NotaMedia;
            await AssignTrabajoFinEstudio(expediente, gestor, cancellationToken);
            await AssignEspecializacion(expediente, gestor);
            await AssignTitulacion(expediente, gestor, cancellationToken);
            await AssignAsignaturasExpediente(expediente, gestor, request.AsignaturasOfertadas, cancellationToken);
            expediente.AddSeguimientoNoUser(TipoSeguimientoExpediente.ActualizadoPorMigracion,
                    "Actualizadas calificaciones por migración desde el Gestor", DateTime.UtcNow);
            expediente.Migrado = true;
            var messages = await GetRelacionExpedientesAsignaturas(expediente, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return messages;
        }

        protected internal virtual async Task AssignTrabajoFinEstudio(ExpedienteAlumno expediente,
            ExpedienteExpedientesIntegrationModel gestor, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(gestor.TituloTfmTfg) || gestor.FechaTfmTfg is null) return;

            expediente.TituloTrabajoFinEstudio = gestor.TituloTfmTfg;
            expediente.FechaTrabajoFinEstudio = gestor.FechaTfmTfg;

            var hitoConseguidoFinEstudio = expediente.HitosConseguidos.FirstOrDefault(hc => hc.TipoConseguidoId == TipoHitoConseguido.TrabajoFinEstudio);
            if (hitoConseguidoFinEstudio is null)
            {
                var hitoFinEstudio = await _context.TiposHitoConseguidos.FirstAsync(hc => hc.Id == TipoHitoConseguido.TrabajoFinEstudio, cancellationToken);
                expediente.AddHitosConseguidos(hitoFinEstudio, expediente.FechaTrabajoFinEstudio.Value);
            }
            else
            {
                hitoConseguidoFinEstudio.Nombre = gestor.TituloTfmTfg;
                hitoConseguidoFinEstudio.FechaInicio = gestor.FechaTfmTfg.Value;
            }
        }

        protected internal virtual async Task AssignEspecializacion(ExpedienteAlumno expediente, ExpedienteExpedientesIntegrationModel gestor)
        {
            if (!gestor.ItinerariosFinalizados.Any()) return;

            foreach (var itinerarioGestor in gestor.ItinerariosFinalizados)
            {
                ExpedienteEspecializacion nuevaEspecializacion = null;
                var expedienteEspecializacion = expediente.ExpedientesEspecializaciones.FirstOrDefault(ee => ee.IdRefEspecializacion == itinerarioGestor.IdEspecializacionErp);
                if (expedienteEspecializacion is null)
                {
                    nuevaEspecializacion = new ExpedienteEspecializacion
                    {
                        ExpedienteAlumnoId = expediente.Id,
                        IdRefEspecializacion = itinerarioGestor.IdEspecializacionErp
                    };
                    await _context.ExpedientesEspecializaciones.AddAsync(nuevaEspecializacion);
                }

                var especializacion = await _erpAcademicoServiceClient.GetEspecializacion(int.Parse(itinerarioGestor.IdEspecializacionErp));
                if (especializacion is null)
                    throw new BadRequestException(_localizer[$"[Expediente id:{expediente.Id}] - La especialización con id:{itinerarioGestor.IdEspecializacionErp} no fue encontrado"]);
                var tipoHitoConseguido = new TipoHitoConseguido { Id = TipoHitoConseguido.Especializacion, Nombre = especializacion.Nombre };
                if (nuevaEspecializacion is not null)
                    expediente.AddHitosConseguidos(tipoHitoConseguido, itinerarioGestor.FechaFin, nuevaEspecializacion);
            }
        }

        protected internal virtual async Task AssignTitulacion(ExpedienteAlumno expediente,
            ExpedienteExpedientesIntegrationModel gestor, CancellationToken cancellationToken)
        {
            if (gestor.FechaExpedicion is null) return;

            var hitoTitulacion = await _context.TiposHitoConseguidos.FirstAsync(hc => hc.Id == TipoHitoConseguido.Titulacion, cancellationToken);
            var tipoSituacionExpedido = await _context.TiposSituacionEstado.FirstAsync(ts => ts.Id == TipoSituacionEstado.TituloExpedido, cancellationToken);
            var existeHitoTitulacion = expediente.HitosConseguidos.FirstOrDefault(hc => hc.TipoConseguidoId == TipoHitoConseguido.Titulacion);
            if (existeHitoTitulacion is null)
            {
                expediente.AddHitosConseguidos(hitoTitulacion, gestor.FechaExpedicion.Value);
            }
            else
            {
                existeHitoTitulacion.Nombre = hitoTitulacion.Nombre;
                existeHitoTitulacion.FechaInicio = gestor.FechaExpedicion.Value;
            }
            expediente.EstadoId = EstadoExpediente.Cerrado;
            expediente.AddTipoSituacionEstadoExpediente(tipoSituacionExpedido, gestor.FechaExpedicion.Value);
        }

        protected internal virtual async Task AssignAsignaturasExpediente(ExpedienteAlumno expediente,
            ExpedienteExpedientesIntegrationModel gestor, List<AsignaturaOfertadaMigracionModel> asignaturasOfertadas, CancellationToken cancellationToken)
        {
            var asignaturasGestorValidas = gestor.Asignaturas.Where(x => x.IdAsignatura > 0).ToList();
            if (!asignaturasGestorValidas.Any())
                throw new BadRequestException(_localizer[$"[Expediente id:{expediente.Id}] - El gestor no contiene asignaturas con id mayor a cero"]);

            var tiposConvocatorias = await _context.TiposConvocatorias.ToListAsync(cancellationToken);
            foreach (var asignaturaGestor in asignaturasGestorValidas)
            {
                var asignaturaOfertada = asignaturasOfertadas.FirstOrDefault(am => am.AsignaturaPlan.Id == asignaturaGestor.IdAsignatura);
                if (asignaturaOfertada is null) continue;

                int situacionAsignaturaId =
                    asignaturaGestor.NotaNumerica == AsignaturaErpAcademicoExpedientesIntegrationModel.NotaNumericaMatriculada ? SituacionAsignatura.Matriculada :
                    asignaturaGestor.NotaNumerica == AsignaturaErpAcademicoExpedientesIntegrationModel.NotaNumericaNoPresentada ? SituacionAsignatura.NoPresentada :
                    asignaturaGestor.Superado && asignaturaGestor.NotaAlfanumerica?.Trim() == AsignaturaErpAcademicoExpedientesIntegrationModel.NotaAlfanumericaMatriculaHonor ?
                    SituacionAsignatura.MatriculaHonor :
                    asignaturaGestor.Superado ? SituacionAsignatura.Superada : SituacionAsignatura.NoSuperada;

                bool reconocida = asignaturaGestor.ReconocimientosOrigen != null && asignaturaGestor.ReconocimientosOrigen.Any();
                var asignaturaCalificacion = await AssignAsignaturasCalificaciones(tiposConvocatorias, asignaturaGestor, asignaturaOfertada, situacionAsignaturaId);

                if (expediente.AsignaturasExpedientes.Any())
                {
                    var expedienteActualizar = expediente.AsignaturasExpedientes.FirstOrDefault(ae => ae.Id != 0 &&
                        ae.IdRefAsignaturaPlan == asignaturaGestor.IdAsignatura.ToString() &&
                        !ae.AsignaturasCalificaciones.Any());
                    if (expedienteActualizar is not null)
                        expedienteActualizar.AsignaturasCalificaciones.Add(asignaturaCalificacion);
                }
                var asignaturasAgregar = await _mediator.Send(new GetAsignaturasAsociadasQuery(expediente.AsignaturasExpedientes,
                    new List<AsignaturaOfertadaModel> { AssignAsignaturaOfertada(asignaturaOfertada) }, situacionAsignaturaId, reconocida), cancellationToken);

                if (asignaturasAgregar.Any())
                {
                    var asignaturaAgregar = asignaturasAgregar.First();
                    asignaturaAgregar.AsignaturasCalificaciones.Add(asignaturaCalificacion);
                    expediente.AsignaturasExpedientes.Add(asignaturaAgregar);
                }
            }
        }

        protected internal virtual async Task<AsignaturaCalificacion> AssignAsignaturasCalificaciones(
            List<TipoConvocatoria> tipoConvocatorias, AsignaturaErpAcademicoExpedientesIntegrationModel asignaturaGestor,
            AsignaturaOfertadaMigracionModel asignaturaOfertadaErp, int situacionAsignaturaId)
        {
            if (!asignaturaOfertadaErp.AsignaturaMatriculadas.Any())
                throw new BadRequestException(_localizer[$"La asignatura ofertada con id: {asignaturaOfertadaErp.Id} no contiene asignaturas matriculadas"]);

            int? tipoConvocatoriaId = tipoConvocatorias.FirstOrDefault(tc => tc.Codigo == asignaturaGestor.Convocatoria?.ToUpper())?.Id;
            if (tipoConvocatoriaId is null)
                throw new BadRequestException(_localizer[$"El campo Convocatoria con valor: {asignaturaGestor.Convocatoria} , no fue encontrado"]);

            string nombreCalificacion = asignaturaGestor.NotaAlfanumerica;
            if (string.IsNullOrEmpty(asignaturaGestor.NotaAlfanumerica) && asignaturaGestor.NotaNumerica >= 0)
            {
                var calificacion = await _mediator.Send(new GetCalificacionByNotaQuery(asignaturaOfertadaErp.Id, asignaturaGestor.NotaNumerica));
                if (calificacion is null)
                    throw new BadRequestException(_localizer[$"La configuración escala o calificación no fue encontrado para asignatura ofertada con id : {asignaturaOfertadaErp.Id}"]);
                nombreCalificacion = calificacion.Nombre;
            }

            var cicloPeriodo = await GetCicloPeriodoLectivo(asignaturaOfertadaErp.PeriodoLectivo.FechaInicio,
                asignaturaOfertadaErp.PeriodoLectivo.DuracionPeriodoLectivo.Id);

            var asignaturaCalificacion = new AsignaturaCalificacion
            {
                IdRefGrupoCurso = Convert.ToInt32(asignaturaOfertadaErp.AsignaturaMatriculadas.First().IdRefCurso),
                EstadoCalificacionId = EstadoCalificacion.EstadoDefinitiva,
                TipoConvocatoriaId = tipoConvocatoriaId.Value,
                FechaPublicado = tipoConvocatoriaId == TipoConvocatoria.Ordinaria ? asignaturaOfertadaErp.PeriodoLectivo.FechaFin :
                                                       asignaturaOfertadaErp.PeriodoLectivo.PeriodoAcademico.FechaFin,
                IdRefPeriodoLectivo = asignaturaOfertadaErp.PeriodoLectivo.Id,
                AnyoAcademico = $"[{asignaturaOfertadaErp.PeriodoLectivo.PeriodoAcademico.AnyoAcademico.AnyoInicio}-" +
                                $"{asignaturaOfertadaErp.PeriodoLectivo.PeriodoAcademico.AnyoAcademico.AnyoFin}]",
                Ciclo = cicloPeriodo,
                EsMatriculaHonor = situacionAsignaturaId == SituacionAsignatura.MatriculaHonor,
                EsNoPresentado = situacionAsignaturaId == SituacionAsignatura.NoPresentada,
                Superada = situacionAsignaturaId == SituacionAsignatura.Superada,
                Calificacion = Convert.ToDecimal(asignaturaGestor.NotaNumerica),
                NombreCalificacion = nombreCalificacion
            };
            return asignaturaCalificacion;
        }

        protected internal virtual async Task<string> GetCicloPeriodoLectivo(DateTime fechaInicioPeriodo, int idDuracionPeriodo)
        {
            var cicloPeriodoLectivo = await _mediator.Send(new GetCicloPeriodoLectivoQuery(fechaInicioPeriodo, idDuracionPeriodo));
            if (cicloPeriodoLectivo is null)
                throw new BadRequestException(_localizer["El campo Ciclo de periodo lectivo es vacío o nulo"]);
            return cicloPeriodoLectivo;
        }

        protected internal virtual AsignaturaOfertadaModel AssignAsignaturaOfertada(AsignaturaOfertadaMigracionModel asignaturaOfertadaMigracion)
        {
            return new AsignaturaOfertadaModel
            {
                Id = asignaturaOfertadaMigracion.Id,
                Orden = asignaturaOfertadaMigracion.Orden,
                TipoAsignatura = asignaturaOfertadaMigracion.TipoAsignatura,
                AsignaturaPlan = asignaturaOfertadaMigracion.AsignaturaPlan,
                PeriodoLectivo = asignaturaOfertadaMigracion.PeriodoLectivo,
                Curso = asignaturaOfertadaMigracion.Curso
            };
        }

        protected internal virtual async Task<string> GetRelacionExpedientesAsignaturas(
            ExpedienteAlumno expedienteAlumno, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(
                new RelateExpedientesAsignaturasCommand(expedienteAlumno), cancellationToken);
            var messages = !result.Any() ? null : 
                $"El expediente con id : {expedienteAlumno.Id}, tiene las siguientes observaciones. {string.Join(" | ", result)}";
            return messages;
        }
    }
}