namespace Unir.Expedientes.Infrastructure.ServicesClients.Models.ErpAcademico.PlanesEstudio
{
    public class ResponseTipoNodoErpAcademico
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public bool EsInicial { get; set; }
        public bool EsIntermedio { get; set; }
        public bool EsFinal { get; set; }
    }
}
