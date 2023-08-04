using Unir.Expedientes.Application.Common.Models.ErpAcademico.Expedientes;

namespace Unir.Expedientes.Application.Common.Models.ErpAcademico.Matriculacion
{
    public class DocumentoAcademicoModel
    {
        public int IdPlan { get; set; }
        public string DisplayNamePlan { get; set; }
        public PeriodoAcademicoMatriculaModel[] PeriodoAcademicoMatriculas { get; set; }
        public ExpedienteAcademicoModel Expediente { get; set; }
    }

    public class PeriodoAcademicoMatriculaModel
    {
        public int IdPeriodoAcademico { get; set; }
        public string DisplayNamePeriodoAcademico { get; set; }
        public MatriculaModel[] Matriculas { get; set; }
    }

    public class MatriculaModel
    {
        public int IdMatricula { get; set; }
        public string DisplayNameMatricula { get; set; }
        public CondicionConsolidadaModel[] CondicionesConsolidadas { get; set; }
        public AlumnoModel Alumno { get; set; }
    }

    public class AlumnoModel
    {
        public string IdIntegracion { get; set; }
    }

    public class CondicionConsolidadaModel
    {
        public int Id { get; set; }
        public bool IsEstadoNoProcesada { get; set; }
        public bool IsEstadoValidada { get; set; }
        public bool IsEstadoPendiente { get; set; }
        public bool IsEstadoRechazada { get; set; }
        public CondicionMatriculaModel CondicionMatricula { get; set; }
    }

    public class CondicionMatriculaModel
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string DisplayName { get; set; }
    }
}
