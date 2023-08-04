using System;
using System.Collections.Generic;
using System.Linq;
using Unir.Expedientes.Application.Common.Models.ErpAcademico.Matriculacion;
using Unir.Expedientes.Application.Common.Models.ErpAcademico.PlanesEstudio;
using Unir.Expedientes.Application.Common.Models.ErpAcademico.Titulos;

namespace Unir.Expedientes.Application.Common.Models.ErpAcademico.Expedientes
{
    public class ExpedienteAcademicoModel
    {
        public int Id { get; set; }
        public string IdIntegracion { get; set; }
        public DateTime? FechaApertura { get; set; }
        public string DisplayName { get; set; }
        public string IdRefTipoVinculacion { get; set; }
        public string IdRefVersionPlan { get; set; }
        public string IdRefIntegracionAlumno { get; set; }
        public string IdRefPlan { get; set; }
        public AlumnoAcademicoModel Alumno { get; set; }
        public PlanAcademicoModel Plan { get; set; }
        public TitulacionAccesoAcademicoModel TitulacionAcceso { get; set; }
        public ViaAccesoPlanAcademicoModel ViaAccesoPlan { get; set; }
        public ICollection<EspecializacionAcademicoModel> Especializaciones { get; set; }
        
        public string VersionPlan => !string.IsNullOrWhiteSpace(IdRefVersionPlan) && Plan != null
            ? Plan.VersionesPlanes.FirstOrDefault(vp => vp.Id.ToString() == IdRefVersionPlan)?.Nro.ToString()
            : "-";
    }
}
