using MediatR;
using Unir.Expedientes.Application.Common.Models.ErpAcademico.Expedientes;

namespace Unir.Expedientes.Application.ExpedientesAlumnos.Queries.GetExpedienteAlumnoErpById
{
    public class GetExpedienteAlumnoErpByIdQuery : IRequest<ExpedienteAcademicoModel>
    {
        public int IdExpedienteAlumno { get; set; }

        public GetExpedienteAlumnoErpByIdQuery(int idExpedienteAlumno)
        {
            IdExpedienteAlumno = idExpedienteAlumno;
        }
    }
}
