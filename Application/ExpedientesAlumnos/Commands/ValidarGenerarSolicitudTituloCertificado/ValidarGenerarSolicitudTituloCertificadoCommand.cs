using MediatR;
using System.Collections.Generic;
using Unir.Expedientes.Application.Common.Queries.ExpedicionTitulos.GetTipoSolicitud;
using Unir.Expedientes.Application.ExpedientesAlumnos.Commands.GenerarSolicitudTituloCertificado;
using Unir.Expedientes.Domain.Entities;

namespace Unir.Expedientes.Application.ExpedientesAlumnos.Commands.ValidarGenerarSolicitudTituloCertificado
{
    public class ValidarGenerarSolicitudTituloCertificadoCommand : IRequest
    {
        public GenerarSolicitudTituloCertificadoCommand GenerarSolicitudTituloCertificadoCommand { get; set; }
        public List<ExpedienteAlumno> Expedientes { get; set; }
        public TipoSolicitudDto TipoSolicitud { get; set; }
        public string[] RolesUsuario { get; set; }

        public ValidarGenerarSolicitudTituloCertificadoCommand(
            GenerarSolicitudTituloCertificadoCommand generarSolicitudTituloCertificadoCommand, 
            List<ExpedienteAlumno> expedientes,
            TipoSolicitudDto tipoSolicitud,
            string[] rolesUsuario)
        {
            GenerarSolicitudTituloCertificadoCommand = generarSolicitudTituloCertificadoCommand;
            Expedientes = expedientes;
            TipoSolicitud = tipoSolicitud;
            RolesUsuario = rolesUsuario;
        }
    }
}
