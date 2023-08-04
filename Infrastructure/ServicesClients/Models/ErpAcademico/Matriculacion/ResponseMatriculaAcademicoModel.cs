using System;
using System.Collections.Generic;
using Unir.Expedientes.Infrastructure.ServicesClients.Models.ErpAcademico.PlanesEstudio;

namespace Unir.Expedientes.Infrastructure.ServicesClients.Models.ErpAcademico.Matriculacion
{
    public class ResponseMatriculaAcademicoModel
    {
        public int Id { get; set; }
        public string IdIntegracion { get; set; }
        public string DisplayName { get; set; }
        public string IdRefExpedienteAlumno { get; set; }
        public decimal TotalCreditosAsignaturasMatriculadasActivas { get; set; }
        public DateTime FechaCambioEstado { get; set; }
        public ResponseTipoMatriculaAcademicoModel Tipo { get; set; }
        public ResponsePlanOfertadoDtoAcademicoModel PlanOfertado { get; set; }
        public ResponseRegionEstudioAcademicoModel RegionEstudio { get; set; }
        public ResponseEstadoMatriculaAcademicoModel Estado { get; set; }
        public List<ResponseAsignaturaMatricula> AsignaturaMatriculadas { get; set; }
    }

    public class ResponseTipoMatriculaAcademicoModel
    {
        public string DisplayName { get; set; }
        public bool PermiteCrearPrimeraMatricula { get; set; }
    }

    public class ResponseRegionEstudioAcademicoModel
    {
        public string DisplayName { get; set; }
    }

    public class ResponseEstadoMatriculaAcademicoModel
    {
        public string DisplayName { get; set; }
        public bool EsDesestimada { get; set; }
        public bool EsAlta { get; set; }
        public bool EsBaja { get; set; }
        public bool EsRecibida { get; set; }
        public bool EsPrematricula { get; set; }
    }

    public class ResponsePlanOfertadoDtoAcademicoModel
    {
        public ResponsePeriodoAcademicoAcademicoModel PeriodoAcademico { get; set; }
        public ResponsePlanErpAcademico Plan { get; set; }
    }

    public class ResponsePeriodoAcademicoAcademicoModel
    {
        public string DisplayName { get; set; }
        public string FechaInicio { get; set; }
        public ResponseAnyoAcademicoAcademicoModel AnyoAcademico { get; set; }
    }

    public class ResponseAnyoAcademicoAcademicoModel
    {
        public string DisplayName { get; set; }
    }
}
