namespace Unir.Expedientes.Infrastructure.ServicesClients.Models.ErpAcademico.PlanesEstudio
{
    public class ResponseRequerimientoTipoAsignaturaErpAcademico
    {
        public double CreditosMinimos { get; set; }
        public double CreditosMaximos { get; set; }
        public double? CreditosObtenidos { get; set; }
        public ResponseTipoAsignaturaErpAcademico TipoAsignatura { get; set; }
    }
}
