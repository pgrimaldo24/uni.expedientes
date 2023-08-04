namespace Unir.Expedientes.Infrastructure.ServicesClients.Models.ErpAcademico.Matriculacion
{
    public class ResponseContentMatriculaAdvancedSearchErpAcademico
    {
        public ContentMatricula Content { get; set; }
    }

    public class ContentMatricula
    {
        public ResponseMatriculaErpAcademico[] Elements { get; set; }
    }
}
