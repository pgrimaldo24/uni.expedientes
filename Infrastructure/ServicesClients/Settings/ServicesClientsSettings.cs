namespace Unir.Expedientes.Infrastructure.ServicesClients.Settings
{
    public class ServicesClientsSettings
    {
        public ErpAcademicoServices ErpAcademicoServices { get; set; }
        public CommonsServices CommonsServices { get; set; }
        public ExpedientesGestorUnirServices ExpedientesGestorUnirServices { get; set; }
        public ExpedicionTitulosServices ExpedicionTitulosServices { get; set; }
        public EvaluacionServices EvaluacionServices { get; set; }
        public FinancieroServices FinancieroServices { get; set; }
        public GestorDocumentalServices GestorDocumentalServices { get; set; }
        public GestorMapeosServices GestorMapeosServices { get; set; }
    }

    public class ErpAcademicoServices
    {
        public string Host { get; set; }
        public bool? Fake { get; set; }
    }

    public class CommonsServices
    {
        public string Host { get; set; }
    }

    public class ExpedientesGestorUnirServices
    {
        public string Host { get; set; }
        public bool? Fake { get; set; }
    }

    public class ExpedicionTitulosServices
    {
        public string Host { get; set; }
        public bool? Fake { get; set; }
        public string ApiKey { get; set; }
    }

    public class EvaluacionServices
    {
        public string Host { get; set; }
        public bool? Fake { get; set; }
        public string ApiKey { get; set; }
    }

    public class FinancieroServices
    {
        public string Host { get; set; }
        public string ApiKey { get; set; }
    }

    public class GestorDocumentalServices
    {
        public string Host { get; set; }
        public bool? Fake { get; set; }
    }

    public class GestorMapeosServices
    {
        public string Host { get; set; }
        public bool? Fake { get; set; }
    }
}
