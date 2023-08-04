using Unir.Expedientes.Infrastructure.ServicesClients.Models.ErpAcademico.Expedientes;

namespace Unir.Expedientes.Infrastructure.ServicesClients.Models.ErpAcademico.Matriculacion
{
    public class ResponseDocumentoAcademicoErpAcademico
    {
        public int IdPlan { get; set; }
        public string DisplayNamePlan { get; set; }
        public ResponsePeriodoAcademicoMatriculaErpAcademico[] PeriodoAcademicoMatriculas { get; set; }
        public ResponseExpedienteErpAcademico Expediente { get; set; }
    }

    public class ResponsePeriodoAcademicoMatriculaErpAcademico
    {
        public int IdPeriodoAcademico { get; set; }
        public string DisplayNamePeriodoAcademico { get; set; }
        public ResponseMatriculaAcademico[] Matriculas { get; set; }
    }

    public class ResponseMatriculaAcademico
    {
        public int IdMatricula { get; set; }
        public string DisplayNameMatricula { get; set; }
        public ResponseCondicionConsolidadaErpAcademico[] CondicionesConsolidadas { get; set; }
    }

    public class ResponseCondicionConsolidadaErpAcademico
    {
        public int Id { get; set; }
        public bool IsEstadoNoProcesada { get; set; }
        public bool IsEstadoValidada { get; set; }
        public bool IsEstadoPendiente { get; set; }
        public bool IsEstadoRechazada { get; set; }
        public ResponseCondicionMatriculaErpAcademico CondicionMatricula { get; set; }
    }

    public class ResponseCondicionMatriculaErpAcademico
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string DisplayName { get; set; }
    }
}
