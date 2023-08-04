namespace Unir.Expedientes.Application.Common.Models.ErpAcademico.Matriculacion
{
    public class MatriculaErpAcademicoModel
    {
        public string AlumnoDisplayName { get; set; }
        public int AlumnoId { get; set; }
        public string AnyoAcademicoDisplayName { get; set; }
        public string CentroEstudioDisplayName { get; set; }
        public int CountAmplicacionesMatriculas { get; set; }
        public int CountAsignaturaMatriculadas { get; set; }
        public int CountBajas { get; set; }
        public int CountConsolidacionesCondicionesMatriculas { get; set; }
        public int CountVariacionesMatriculas { get; set; }
        public string DisplayName { get; set; }
        public bool EsMatriculaAsignable { get; set; }
        public bool EsValidoDarAltaMatricula { get; set; }
        public bool EsValidoDarBajaMatricula { get; set; }
        public bool EsValidoRecuperarMatricula { get; set; }
        public string EstudioDisplayName { get; set; }
        public string FechaInicioPeriodoAcademicoMatricula { get; set; }
        public int Id { get; set; }
        public string IdIntegracion { get; set; }
        public int IdRefExpedienteAlumno { get; set; }
        public int IdPersonaLogged { get; set; }
        public int IdUniversidad { get; set; }
        public string PeriodoAcademicoDisplayName { get; set; }
        public string PlanDisplayName { get; set; }
        public string TipoEstudioDisplayName { get; set; }
        public string TipoMatriculaDisplayName { get; set; }
        public string TituloDisplayName { get; set; }
        public double? TotalCreditosAsignaturasMatriculadasActivas { get; set; }
        public string UniversidadDisplayName { get; set; }
        public string RegionEstudioDisplayName { get; set; }
        public EstadoMatriculaErpAcademicoModel Estado { get; set; }
    }

    public class EstadoMatriculaErpAcademicoModel
    {
        public string DisplayName { get; set; }
    }
}
