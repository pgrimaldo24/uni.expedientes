using MediatR;
using Unir.Framework.ApplicationSuperTypes.Models.RequestParameters;

namespace Unir.Expedientes.Application.ExpedientesAlumnos.Queries.GetDatosSolicitudTitulacionSupercap;

public class GetDatosSolicitudTitulacionSupercapQuery : QueryParameters, IRequest<ExpedienteAlumnoSolicitudTitulacionSupercapDto>
{
    public string IdRefPlan { get; set; }
    public string IdRefIntegracionAlumno { get; set; }

    public GetDatosSolicitudTitulacionSupercapQuery(string idRefPlan, string idRefIntegracionAlumno)
    {
        IdRefPlan = idRefPlan;
        IdRefIntegracionAlumno = idRefIntegracionAlumno;
    }
}