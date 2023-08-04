using MediatR;
using System.Globalization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Unir.Expedientes.Domain.Entities;
using Unir.Framework.Crosscutting.Security;

namespace Unir.Expedientes.Application.SeguimientosExpedientes.Commands.AddSeguimientoTitulacionAccesoUncommit
{
    public class AddSeguimientoTitulacionAccesoUncommitCommandHandler : IRequestHandler<AddSeguimientoTitulacionAccesoUncommitCommand, bool>
    {
        private readonly IIdentityService _identityService;
        public AddSeguimientoTitulacionAccesoUncommitCommandHandler(IIdentityService identityService)
        {
            _identityService = identityService;
        }

        public async Task<bool> Handle(AddSeguimientoTitulacionAccesoUncommitCommand request, CancellationToken cancellationToken)
        {
            var result = AddSeguimientoTitulacionAcceso(request);
            return await Task.FromResult(result);
        }

        protected internal virtual bool AddSeguimientoTitulacionAcceso(AddSeguimientoTitulacionAccesoUncommitCommand request)
        {
            string descripcionSeguimiento;
            var requestTitulacionAcceso = request.TitulacionAcceso;
            var expedienteAlumno = request.ExpedienteAlumno;
            var titulacionAcceso = expedienteAlumno.TitulacionAcceso;
            if (titulacionAcceso == null)
            {
                descripcionSeguimiento = "Titulación de Acceso anterior: (no disponible)";
                AgregarSeguimiento(expedienteAlumno, request.PorIntegracion, descripcionSeguimiento);
                return true;
            }

            if (requestTitulacionAcceso.Titulo == titulacionAcceso.Titulo &&
                requestTitulacionAcceso.TipoEstudio == titulacionAcceso.TipoEstudio &&
                requestTitulacionAcceso.InstitucionDocente == titulacionAcceso.InstitucionDocente &&
                requestTitulacionAcceso.IdRefTerritorioInstitucionDocente ==
                titulacionAcceso.IdRefTerritorioInstitucionDocente &&
                ((requestTitulacionAcceso.FechaInicioTitulo.HasValue && titulacionAcceso.FechaInicioTitulo.HasValue &&
                    requestTitulacionAcceso.FechaInicioTitulo.Value.Date == titulacionAcceso.FechaInicioTitulo.Value.Date) ||
                  !requestTitulacionAcceso.FechaInicioTitulo.HasValue && !titulacionAcceso.FechaInicioTitulo.HasValue) &&
                ((requestTitulacionAcceso.FechafinTitulo.HasValue && titulacionAcceso.FechafinTitulo.HasValue &&
                    requestTitulacionAcceso.FechafinTitulo.Value.Date == titulacionAcceso.FechafinTitulo.Value.Date) ||
                  !requestTitulacionAcceso.FechafinTitulo.HasValue && !titulacionAcceso.FechafinTitulo.HasValue) &&
                requestTitulacionAcceso.CodigoColegiadoProfesional == titulacionAcceso.CodigoColegiadoProfesional &&
                requestTitulacionAcceso.NroSemestreRealizados == titulacionAcceso.NroSemestreRealizados &&
                requestTitulacionAcceso.IdRefInstitucionDocente == titulacionAcceso.IdRefInstitucionDocente)
            {
                return false;
            }

            descripcionSeguimiento = SetDescripcionSeguimientoAnterior(titulacionAcceso);
            AgregarSeguimiento(expedienteAlumno, request.PorIntegracion, descripcionSeguimiento);
            return true;
        }

        protected internal virtual string SetDescripcionSeguimientoAnterior(TitulacionAcceso titulacionAcceso)
        {
            var noDisponible = "(no disponible)";
            var descripcionNroSemestresRealizados = titulacionAcceso.NroSemestreRealizados.HasValue
                    ? titulacionAcceso.NroSemestreRealizados.Value.ToString()
                    : noDisponible;
            var descripcionTipoEstudio = !string.IsNullOrWhiteSpace(titulacionAcceso.TipoEstudio)
                ? titulacionAcceso.TipoEstudio
                : noDisponible;
            var descripcionUbicacion = !string.IsNullOrWhiteSpace(titulacionAcceso.IdRefTerritorioInstitucionDocente)
                ? titulacionAcceso.IdRefTerritorioInstitucionDocente
                : noDisponible;
            var descripcionFechaInicio = titulacionAcceso.FechaInicioTitulo.HasValue
                ? titulacionAcceso.FechaInicioTitulo.Value.ToString(CultureInfo.InvariantCulture)
                : noDisponible;
            var descripcionFechaFin = titulacionAcceso.FechafinTitulo.HasValue
                ? titulacionAcceso.FechafinTitulo.Value.ToString(CultureInfo.InvariantCulture)
                : noDisponible;
            var descripcionCodigoColegiado = !string.IsNullOrWhiteSpace(titulacionAcceso.CodigoColegiadoProfesional)
                ? titulacionAcceso.CodigoColegiadoProfesional
                : noDisponible;
            var descripcionIdRefInstitucionDocente = !string.IsNullOrWhiteSpace(titulacionAcceso.IdRefInstitucionDocente)
                ? titulacionAcceso.IdRefInstitucionDocente
                : noDisponible;

            var valoresTitulacionAccesoParaDescripcion = new StringBuilder($"{nameof(titulacionAcceso.Titulo)}: {titulacionAcceso.Titulo}");
            valoresTitulacionAccesoParaDescripcion.Append($"#{nameof(titulacionAcceso.InstitucionDocente)}: {titulacionAcceso.InstitucionDocente}");
            valoresTitulacionAccesoParaDescripcion.Append($"#{nameof(titulacionAcceso.TipoEstudio)}: {descripcionTipoEstudio}");
            valoresTitulacionAccesoParaDescripcion.Append($"#{nameof(titulacionAcceso.FechaInicioTitulo)}: {descripcionFechaInicio}");
            valoresTitulacionAccesoParaDescripcion.Append($"#{nameof(titulacionAcceso.FechafinTitulo)}: {descripcionFechaFin}");
            valoresTitulacionAccesoParaDescripcion.Append($"#{nameof(titulacionAcceso.IdRefTerritorioInstitucionDocente)}: {descripcionUbicacion}");
            valoresTitulacionAccesoParaDescripcion.Append($"#{nameof(titulacionAcceso.CodigoColegiadoProfesional)}: {descripcionCodigoColegiado}");
            valoresTitulacionAccesoParaDescripcion.Append($"#{nameof(titulacionAcceso.NroSemestreRealizados)}: {descripcionNroSemestresRealizados}");
            valoresTitulacionAccesoParaDescripcion.Append($"#{nameof(titulacionAcceso.IdRefInstitucionDocente)}: {descripcionIdRefInstitucionDocente}");

            return $"Titulación de Acceso anterior: {valoresTitulacionAccesoParaDescripcion}";
        }

        protected internal virtual void AgregarSeguimiento(ExpedienteAlumno expedienteAlumno,
            bool porIntegracion, string descripcionSeguimiento)
        {
            if (porIntegracion)
            {
                descripcionSeguimiento += ". Por Integración.";
                expedienteAlumno.AddSeguimientoNoUser(TipoSeguimientoExpediente.ExpedienteModificadoTitulacionAcceso, descripcionSeguimiento);
                return;
            }

            expedienteAlumno.AddSeguimiento(TipoSeguimientoExpediente.ExpedienteModificadoTitulacionAcceso,
                _identityService.GetUserIdentityInfo().Id, descripcionSeguimiento);
        }
    }
}
