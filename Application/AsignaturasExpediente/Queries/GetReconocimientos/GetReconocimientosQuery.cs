using MediatR;

namespace Unir.Expedientes.Application.AsignaturasExpediente.Queries.GetReconocimientos
{
    public class GetReconocimientosQuery : IRequest<ReconocimientoClasificacionDto>
    {
        public string IdIntegracionAlumno { get; set; }
        public string IdRefPlan { get; set; }
        public string IdRefVersionPlan { get; set; }

        public GetReconocimientosQuery(string idIntegracionAlumno, 
            string idRefPlan, string idRefVersionPlan)
        {
            IdIntegracionAlumno = idIntegracionAlumno;
            IdRefPlan = idRefPlan;
            IdRefVersionPlan = idRefVersionPlan;
        }
    }
}
