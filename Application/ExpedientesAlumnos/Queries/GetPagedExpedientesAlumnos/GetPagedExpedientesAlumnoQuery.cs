using MediatR;
using Unir.Expedientes.Application.ExpedientesAlumnos.Queries.GetApplyQueryExpedientesAlumnos;
using Unir.Framework.ApplicationSuperTypes.Models.Results;

namespace Unir.Expedientes.Application.ExpedientesAlumnos.Queries.GetPagedExpedientesAlumnos
{
    public class GetPagedExpedientesAlumnoQuery : ApplyQueryDto, 
        IRequest<ResultListDto<ExpedienteAlumnoPagedListItemDto>>
    {
    }
}
