using MediatR;
using System.Collections.Generic;

namespace Unir.Expedientes.Application.HitosConseguidos.Queries.GetHitosConseguidosByIdExpediente
{
    public class GetHitosConseguidosByIdExpedienteQuery : IRequest<List<HitoConseguidoDto>>
    {
        public int IdExpedienteAlumno { get; set; }
        public GetHitosConseguidosByIdExpedienteQuery(int idExpedienteAlumno)
        {
            IdExpedienteAlumno = idExpedienteAlumno;
        }
    }
}
