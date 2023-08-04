using System;
using Unir.Expedientes.Domain.Entities;
using Unir.Framework.ApplicationSuperTypes.Mappings;

namespace Unir.Expedientes.Application.ExpedientesAlumnos.Queries.GetDatosSolicitudTitulacionSupercap;

public class ExpedienteAlumnoSolicitudTitulacionSupercapDto : IMapFrom<ExpedienteAlumno>
{
    public int Id { get; set; }
    public DateTime? FechaApertura { get; set; }
    public string IdRefViaAccesoPlan { get; set; }
    public virtual TitulacionAccesoSolicitudTitulacionSuperCapDto TitulacionAcceso { get; set; }
}

public class TitulacionAccesoSolicitudTitulacionSuperCapDto : IMapFrom<TitulacionAcceso>
{
    public int Id { get; set; }
    public string Titulo { get; set; }
    public string InstitucionDocente { get; set; }
}