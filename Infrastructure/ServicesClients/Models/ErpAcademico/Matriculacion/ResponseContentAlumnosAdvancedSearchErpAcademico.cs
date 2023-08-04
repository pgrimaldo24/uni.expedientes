namespace Unir.Expedientes.Infrastructure.ServicesClients.Models.ErpAcademico.Matriculacion
{
    public class ResponseContentAlumnosAdvancedSearchErpAcademico
    {
        public Content Content { get; set; }
    }

    public class Content
    {
        public ResponseAlumnoErpAcademico[] Elements { get; set; }
    }
}
