using MediatR;
using Unir.Expedientes.Application.ExpedientesAlumnos.SharedDtos;

namespace Unir.Expedientes.Application.ExpedientesAlumnos.Queries.GetProcesosEnSegundoPlano
{
    public class GetProcesosEnSegundoPlanoQuery : IRequest<ProcesoBackgroundDto[]>
    {

    }
}
