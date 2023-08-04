namespace Unir.Expedientes.Infrastructure.ServicesClients.Models.ErpAcademico.PlanesEstudio
{
    public class ResponseAsignaturaPlanTitulacionErpAcademico
    {
        public string IdAsignaturaPlan { get; set; }
        public string Nombre { get; set; }
        public string Codigo { get; set; }
        public double Creditos { get; set; }
        public bool Practicas { get; set; }
        public string CodigoOficialExterno { get; set; }
        public string IdentificadorOficialExterno { get; set; }
        public int? Curso { get; set; }
        public int Orden { get; set; }
        public string Periodicidad { get; set; }
        public string PeriodicidadCodigoOficialExterno { get; set; }
        public string PeriodoLectivo { get; set; }
        public string Idioma { get; set; }
        public ResponseTipoAsignaturaErpAcademico TipoAsignatura { get; set; }
    }
}
