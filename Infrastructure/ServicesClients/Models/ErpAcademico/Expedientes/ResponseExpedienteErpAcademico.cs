using System;
using System.Collections.Generic;
using Unir.Expedientes.Infrastructure.ServicesClients.Models.ErpAcademico.Matriculacion;
using Unir.Expedientes.Infrastructure.ServicesClients.Models.ErpAcademico.PlanesEstudio;
using Unir.Expedientes.Infrastructure.ServicesClients.Models.ErpAcademico.Titulos;

namespace Unir.Expedientes.Infrastructure.ServicesClients.Models.ErpAcademico.Expedientes
{
    public class ResponseExpedienteErpAcademico
    {
        public int Id { get; set; }
        public string IdIntegracion { get; set; }
        public DateTime? FechaApertura { get; set; }
        public string DisplayName { get; set; }
        public string IdRefTipoVinculacion { get; set; }
        public string IdRefVersionPlan { get; set; }
        public string IdRefIntegracionAlumno { get; set; }
        public string IdRefPlan { get; set; }
        public ResponseAlumnoErpAcademico Alumno { get; set; }
        public ResponsePlanErpAcademico Plan { get; set; }
        public ResponseTitulacionAccesoErpAcademico TitulacionAcceso { get; set; }
        public ResponseViaAccesoPlanErpAcademico ViaAccesoPlan { get; set; }
        public ICollection<ResponseEspecializacionErpAcademico> Especializaciones { get; set; }
    }
}
