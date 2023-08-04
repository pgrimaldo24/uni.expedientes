using MediatR;
using System.Linq;
using Unir.Expedientes.Domain.Entities;

namespace Unir.Expedientes.Application.ExpedientesAlumnos.Queries.GetApplyQueryExpedientesAlumnos
{
    public class GetApplyQueryExpedientesAlumnosQuery : IRequest<IQueryable<ExpedienteAlumno>>
    {
        public ApplyQueryDto Filter { get; set; }
        public IQueryable<ExpedienteAlumno> Queryable { get; set; }

        public GetApplyQueryExpedientesAlumnosQuery(IQueryable<ExpedienteAlumno> queryable, ApplyQueryDto filter)
        {
            Queryable = queryable;
            Filter = filter;
        }
    }
}
