namespace Unir.Expedientes.Infrastructure.ServicesClients.Models.ErpAcademico.Matriculacion
{
    public class ResponseMatriculaErpAcademico
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
        public ResponseEstadoMatriculaErpAcademico Estado { get; set; }
        public ResponseAlumnoMatriculaErpAcademico Alumno { get; set; }
    }

    public class ResponseEstadoMatriculaErpAcademico
    {
        public string DisplayName { get; set; }
    }
    public class ResponseAlumnoMatriculaErpAcademico
    {
        public string IdIntegracion { get; set; }
    }
}
