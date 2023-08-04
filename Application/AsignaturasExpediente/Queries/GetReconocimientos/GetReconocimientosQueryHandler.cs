using MediatR;
using Microsoft.Extensions.Localization;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Drawing;
using Unir.Expedientes.Application.Common.Interfaces;
using Unir.Expedientes.Application.Common.Models.Evaluaciones;
using Unir.Expedientes.Application.Common.Models.ExpedientesGestorUnir;
using Unir.Expedientes.Application.Resources.Globalization;
using Unir.Framework.ApplicationSuperTypes.Exceptions;

namespace Unir.Expedientes.Application.AsignaturasExpediente.Queries.GetReconocimientos
{
    public class GetReconocimientosQueryHandler :
        IRequestHandler<GetReconocimientosQuery, ReconocimientoClasificacionDto>
    {
        private readonly IGestorMapeosServiceClient _gestorMapeosServiceClient;
        private readonly IExpedientesGestorUnirServiceClient _expedientesGestorUnirServiceClient;
        private readonly IEvaluacionesServiceClient _evaluacionesServiceClient;
        private readonly IStringLocalizer<GetReconocimientosQueryHandler> _localizer;
        private const int CodigoIncorrecto = 1;
        private const int EstadoSolicitudValido = 9;
        private const string IdTipoAsignaturaTransversal = "-1";
        private const string IdTipoAsignaturaSeminario = "-2";
        private const string IdTipoAsignaturaExtensionUniversitaria = "-3";
        private const string EnTramite = "(en trámite)";
        private const string Reconocimiento = "Reconocimiento";
        private const string Adaptacion = "Adaptación";
        private const string Movilidad = "Movilidad";
        private const string Apto = "apto";
        private const string NoApto = "no apto";

        public GetReconocimientosQueryHandler(
            IGestorMapeosServiceClient gestorMapeosServiceClient,
            IExpedientesGestorUnirServiceClient expedientesGestorUnirServiceClient,
            IEvaluacionesServiceClient evaluacionesServiceClient,
            IStringLocalizer<GetReconocimientosQueryHandler> localizer)
        {
            _gestorMapeosServiceClient = gestorMapeosServiceClient;
            _expedientesGestorUnirServiceClient = expedientesGestorUnirServiceClient;
            _evaluacionesServiceClient = evaluacionesServiceClient;
            _localizer = localizer;
        }

        public async Task<ReconocimientoClasificacionDto> Handle(
            GetReconocimientosQuery request, CancellationToken cancellationToken)
        {
            var estudios = await _gestorMapeosServiceClient.GetEstudios(request.IdRefPlan, request.IdRefVersionPlan);
            if (estudios == null || !estudios.Any())
                throw new BadRequestException(_localizer[
                    "No está mapeado el plan de estudio con ningún estudio del Gestor y no se pueden recuperar reconocimientos"]);

            ReconocimientoGestorModel reconocimiento = null;
            var idEstudioGestor = 0;
            foreach (var estudio in estudios)
            {
                var responseReconocimiento = await _expedientesGestorUnirServiceClient
                    .GetReconocimientos(request.IdIntegracionAlumno, estudio.IdEstudioGestor);
                if (responseReconocimiento == null ||
                    responseReconocimiento.CodigoResultado == CodigoIncorrecto)
                    continue;

                idEstudioGestor = estudio.IdEstudioGestor;
                reconocimiento = responseReconocimiento.Reconocimiento;
                break;
            }
            if (reconocimiento == null) return null;

            var reconocimientoClasificacion = new ReconocimientoClasificacionDto();
            await GetAsignaturasReconocimiento(reconocimiento.Asignaturas, 
                idEstudioGestor, reconocimientoClasificacion);
            GetAsignaturasReconocimientoTransversal(reconocimiento.Transversal, 
                reconocimientoClasificacion.AsignaturasReconocimientos);
            GetAsignaturasReconocimientoSeminario(reconocimiento.Seminarios, 
                reconocimientoClasificacion.AsignaturasReconocimientos);
            GetAsignaturasReconocimientoUniversitaria(reconocimiento.ExtensionUniversitaria, 
                reconocimientoClasificacion.AsignaturasReconocimientos);

            return reconocimientoClasificacion;
        }

        protected internal virtual void GetAsignaturasReconocimientoTransversal(
            TransversalGestorModel reconocimientoTransversal,
            List<AsignaturaReconocimientoDto> reconocimientosAsignaturas)
        {
            if (reconocimientoTransversal == null || !reconocimientoTransversal.Reconocimientos.Any())
                return;

            reconocimientosAsignaturas.Add(AssignAsignaturaReconocimientoTransversal(null, 
                reconocimientoTransversal.Ects, $"{reconocimientoTransversal.NotaMedia} Reconocido*", true));

            var reconocimientosTransversal = reconocimientoTransversal.Reconocimientos
                .Where(r => r.IdEstadoSolicitud <= EstadoSolicitudValido).ToList();
            foreach (var reconocimiento in reconocimientosTransversal)
            {
                var asignatura = AssignAsignaturaReconocimientoTransversal(reconocimiento);
                GetDescripcionCalificacion(reconocimiento, asignatura);
                reconocimientosAsignaturas.Add(asignatura);
            }
        }

        protected internal virtual async Task GetAsignaturasReconocimiento(
            ICollection<AsignaturaGestorModel> asignaturas, int idEstudioGestor,
            ReconocimientoClasificacionDto reconocimientoClasificacion)
        {
            if (asignaturas == null || !asignaturas.Any())
                return;

            var asignaturasGestor = await _gestorMapeosServiceClient.GetAsignaturas(idEstudioGestor);
            if (asignaturasGestor == null || !asignaturasGestor.Any())
                return;

            foreach (var asignatura in asignaturas)
            {
                if (asignatura.Reconocimientos == null || !asignatura.Reconocimientos.Any()) 
                    continue;

                var asignaturaGestor = asignaturasGestor
                    .FirstOrDefault(ag => ag.IdAsignaturaEstudioGestor == asignatura.IdAsignaturaUnir);
                if (asignaturaGestor == null)
                {
                    reconocimientoClasificacion.MensajesError
                        .Add($"No se encontró la asignatura con id {asignatura.IdAsignaturaUnir} en el estudio {idEstudioGestor} del Gestor");
                    continue;
                }

                string calificacion;
                if (asignatura.NotaMedia > 0)
                {
                    calificacion = await GetCalificacionEvaluacionConfiguracionEscala(asignatura,
                        asignaturaGestor.IdAsignaturaPlanErp);
                }
                else
                {
                    var primerReconocimiento = asignatura.Reconocimientos
                        .FirstOrDefault(ar => ar.IdEstadoSolicitud == EstadoSolicitudValido);
                    calificacion = primerReconocimiento?.NivelAprobacionDescripcion ?? string.Empty;
                }

                SetAsignaturasReconocimientos(asignaturaGestor.IdAsignaturaPlanErp, calificacion,
                    asignatura.Reconocimientos, reconocimientoClasificacion.AsignaturasReconocimientos);
            }
        }

        protected internal virtual void SetAsignaturasReconocimientos(int idAsignaturaPlanErp, 
            string calificacion, ICollection<ReconocimientoCommonGestorModel> reconocimientos,
            List<AsignaturaReconocimientoDto> reconocimientosAsignaturas)
        {
            foreach (var reconocimiento in reconocimientos)
            {
                var asignaturaReconocimientoDto = AssignAsignaturaReconocimiento(idAsignaturaPlanErp, calificacion, reconocimiento);
                GetDescripcionCalificacion(reconocimiento, asignaturaReconocimientoDto);
                asignaturaReconocimientoDto.Calificacion += $" {GetDescripcionTipoSolicitud(reconocimiento.TipoSolicitud)}";
                reconocimientosAsignaturas.Add(asignaturaReconocimientoDto);
            }
        }

        protected internal virtual void GetAsignaturasReconocimientoSeminario(
            ICollection<SeminarioGestorModel> reconocimientoSeminarios,
            List<AsignaturaReconocimientoDto> reconocimientosAsignaturas)
        {
            if (reconocimientoSeminarios == null || !reconocimientoSeminarios.Any())
                return;

            foreach (var reconocimiento in reconocimientoSeminarios)
            {
                reconocimientosAsignaturas.Add(AssignAsignaturaReconocimientoSeminario(reconocimiento));
            }
        }

        protected internal virtual void GetAsignaturasReconocimientoUniversitaria(
            ExtensionUniversitariaGestorModel reconocimientoExtensionUniversitaria,
            List<AsignaturaReconocimientoDto> reconocimientosAsignaturas)
        {
            if (reconocimientoExtensionUniversitaria == null || !reconocimientoExtensionUniversitaria.Reconocimientos.Any())
                return;

            var reconocimientosUniversitaria = reconocimientoExtensionUniversitaria.Reconocimientos
                    .Where(r => r.IdEstadoSolicitud <= EstadoSolicitudValido).ToList();
            foreach (var reconocimiento in reconocimientosUniversitaria)
            {
                var asignatura = AssignAsignaturaReconocimientoUniversitaria(reconocimiento);
                GetDescripcionCalificacion(reconocimiento, asignatura);
                reconocimientosAsignaturas.Add(asignatura);
            }
        }

        protected internal virtual void GetDescripcionCalificacion(
            ReconocimientoCommonGestorModel reconocimiento, AsignaturaReconocimientoDto asignatura)
        {
            asignatura.Calificacion = reconocimiento.Nota <= 0 && 
                (reconocimiento.NivelAprobacionDescripcion?.ToLower() == Apto || 
                 reconocimiento.NivelAprobacionDescripcion?.ToLower() == NoApto) 
                ? string.Empty : reconocimiento.Nota.ToString(CultureInfo.InvariantCulture);
            asignatura.Calificacion = $"{asignatura.Calificacion} {reconocimiento.NivelAprobacionDescripcion}".Trim();
            if (reconocimiento.IdEstadoSolicitud < EstadoSolicitudValido)
            {
                asignatura.Calificacion += $" {EnTramite}";
            }
        }

        protected internal virtual async Task<string> GetCalificacionEvaluacionConfiguracionEscala(
            AsignaturaGestorModel asignatura, int idAsignaturaPlanErp)
        {
            var calificacion = asignatura.NotaMedia.ToString();
            var configuracionEscala = await _evaluacionesServiceClient
                .GetConfiguracionEscalaFromNivelesAsociadosEscalas(null, idAsignaturaPlanErp);
            if (configuracionEscala == null) return calificacion;
            var calificaciones = configuracionEscala.Configuracion.Calificacion.Calificaciones
                .Where(c => !c.EsNoPresentado).ToList();

            var esAfectaNotaNumerica = configuracionEscala.Configuracion.Calificacion.AfectaNotaNumerica;

            calificaciones = esAfectaNotaNumerica
                ? calificaciones.OrderBy(c => c.NotaMinima).ToList()
                : calificaciones.OrderBy(c => c.PorcentajeMinimo).ToList();

            CalificacionListModel calificacionActual = null;
            foreach (var item in calificaciones)
            {
                var notaCalificacion = esAfectaNotaNumerica ? item.NotaMinima : item.PorcentajeMinimo;
                if (asignatura.NotaMedia >= notaCalificacion)
                {
                    calificacionActual = item;
                    continue;
                }
                break;
            }
            calificacion += $" {calificacionActual?.Nombre}";

            return calificacion;
        }
        
        protected internal virtual string GetDescripcionTipoSolicitud(string tipoSolicitud)
        {
            var output = string.Empty;
            var mapTipoSolicitud = new Dictionary<string, string>
            {
                { Reconocimiento, CommonStrings.TipoSolicitudReconocimiento },
                { Adaptacion, CommonStrings.TipoSolicitudAdaptacion },
                { Movilidad, CommonStrings.TipoSolicitudMovilidad }
            };

            return mapTipoSolicitud.TryGetValue(tipoSolicitud, out output) ? output : string.Empty; 
        }

        protected internal virtual string GetNombreAsignaturaReconocimiento(
            string asignaturaExterna, string estudioExterno)
        {
            var nombreAsignatura =
                !string.IsNullOrWhiteSpace(asignaturaExterna) && !string.IsNullOrWhiteSpace(estudioExterno)
                    ? $"{asignaturaExterna} ({estudioExterno})"
                : string.IsNullOrWhiteSpace(asignaturaExterna) && !string.IsNullOrWhiteSpace(estudioExterno)
                    ? estudioExterno
                : !string.IsNullOrWhiteSpace(asignaturaExterna) && string.IsNullOrWhiteSpace(estudioExterno)
                    ? asignaturaExterna : string.Empty;
            return nombreAsignatura;
        }

        protected internal virtual AsignaturaReconocimientoDto AssignAsignaturaReconocimientoTransversal(
            ReconocimientoCommonGestorModel reconocimiento, double ects = 0, string calificacion = null,
            bool esPrincipal = false)
        {
            return new AsignaturaReconocimientoDto
            {
                NombreAsignatura = esPrincipal ? CommonStrings.NombreAsignaturaTransversal
                                    : $"{reconocimiento.AsignaturaExterna} ({reconocimiento.EstudioExterno})",
                Ects = esPrincipal ? ects : reconocimiento.EctsExterna,
                Calificacion = calificacion,
                EsTransversal = true,
                EsTransversalPrincipal = esPrincipal,
                IdRefTipoAsignatura = IdTipoAsignaturaTransversal,
                NombreTipoAsignatura = esPrincipal ? CommonStrings.NombreTipoAsignaturaTransversal
                                        : reconocimiento.TipoAsignaturaExternaDescripcion
            };
        }

        protected internal virtual AsignaturaReconocimientoDto AssignAsignaturaReconocimiento(
            int idAsignaturaPlanErp, string calificacion, ReconocimientoCommonGestorModel reconocimiento)
        {
            return new AsignaturaReconocimientoDto
            {
                IdAsignaturaPlanErp = idAsignaturaPlanErp,
                CalificacionAsignaturaErp = calificacion,
                NombreAsignatura = GetNombreAsignaturaReconocimiento(reconocimiento.AsignaturaExterna,
                                        reconocimiento.EstudioExterno),
                Ects = reconocimiento.EctsExterna,
                NombreTipoAsignatura = reconocimiento.TipoAsignaturaExternaDescripcion,
                EsAsignatura = true
            };
        }

        protected internal virtual AsignaturaReconocimientoDto AssignAsignaturaReconocimientoSeminario(
            SeminarioGestorModel reconocimiento)
        {
            return new AsignaturaReconocimientoDto
            {
                NombreAsignatura = reconocimiento.NombreSeminario,
                Ects = reconocimiento.Ects,
                Calificacion = CommonStrings.NotaPorDefectoSeminario,
                EsSeminario = true,
                IdRefTipoAsignatura = IdTipoAsignaturaSeminario,
                NombreTipoAsignatura = CommonStrings.NombreTipoAsignaturaSeminario
            };
        }

        protected internal virtual AsignaturaReconocimientoDto AssignAsignaturaReconocimientoUniversitaria(
            ReconocimientoCommonGestorModel reconocimiento)
        {
            return new AsignaturaReconocimientoDto
            {
                NombreAsignatura = reconocimiento.AsignaturaExterna,
                Ects = reconocimiento.EctsExterna,
                EsExtensionUniversitaria = true,
                IdRefTipoAsignatura = IdTipoAsignaturaExtensionUniversitaria,
                NombreTipoAsignatura = CommonStrings.NombreTipoAsignaturaUniversitaria
            };
        }
    }
}
