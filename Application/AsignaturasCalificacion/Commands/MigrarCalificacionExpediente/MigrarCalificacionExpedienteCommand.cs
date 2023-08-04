using MediatR;
using System.Collections.Generic;
using Unir.Expedientes.Application.Common.Models.ErpAcademico.OfertaEstudio.Migracion;
using Unir.Expedientes.Domain.Entities;

namespace Unir.Expedientes.Application.AsignaturasCalificacion.Commands.MigrarCalificacionExpediente
{
    public class MigrarCalificacionExpedienteCommand : IRequest<string>
    {
        public ExpedienteAlumno ExpedienteAlumno { get; set; }
        public List<AsignaturaOfertadaMigracionModel> AsignaturasOfertadas { get; set; }
        public MigrarCalificacionExpedienteCommand(ExpedienteAlumno expedientesAlumno, List<AsignaturaOfertadaMigracionModel> asignaturasOfertadas)
        {
            ExpedienteAlumno = expedientesAlumno;
            AsignaturasOfertadas = asignaturasOfertadas;
        }
    }
}
