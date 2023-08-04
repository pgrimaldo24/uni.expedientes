using System;
using System.Collections.Generic;
using System.Linq;
using Unir.Expedientes.Domain.Entities;
using Unir.Framework.ApplicationSuperTypes.Mappings;

namespace Unir.Expedientes.Application.ExpedientesAlumnos.Queries.GetTitulacionAccesoAndEspecializacionesByIdExpediente;

public class ExpedienteTitulacionAccesoEspecializacionesItemDto : IMapFrom<ExpedienteAlumno>
{
    public ExpedienteTitulacionAccesoDto TitulacionAcceso { get; set; }
    public List<ExpedienteEspecializacionesDto> ExpedientesEspecializaciones { get; set; }
    public List<int> IdsEspecializacionesExpediente => ExpedientesEspecializaciones?
        .Where(ee => int.TryParse(ee.IdRefEspecializacion, out _))
        .Select(ee => int.Parse(ee.IdRefEspecializacion))
        .Distinct()
        .ToList();
}

public class ExpedienteTitulacionAccesoDto : IMapFrom<TitulacionAcceso>
{
    public string Titulo { get; set; }
    public string InstitucionDocente { get; set; }
    public int? NroSemestreRealizados { get; set; }
    public string TipoEstudio { get; set; }
    public string IdRefTerritorioInstitucionDocente { get; set; }
    public DateTime? FechaInicioTitulo { get; set; }
    public DateTime? FechafinTitulo { get; set; }
    public string CodigoColegiadoProfesional { get; set; }
    public string IdRefInstitucionDocente { get; set; }
}

public class ExpedienteEspecializacionesDto : IMapFrom<ExpedienteEspecializacion>
{
    public string IdRefEspecializacion { get; set; }
}