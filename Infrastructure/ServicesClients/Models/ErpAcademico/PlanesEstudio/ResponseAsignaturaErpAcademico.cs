namespace Unir.Expedientes.Infrastructure.ServicesClients.Models.ErpAcademico.PlanesEstudio
{
    public class ResponseAsignaturaErpAcademico
    {
        public int Id { get; set; }
        public int IdAsignaturaPlan { get; set; }
        public string Nombre { get; set; }
        public string Codigo { get; set; }
        public double Creditos { get; set; }
        public string CodigoOficialExterno { get; set; }
        public string IdentificadorOficialExterno { get; set; }
        public string Curso { get; set; }
        public string Periodicidad { get; set; }
        public string PeriodicidadCodigoOficialExterno { get; set; }
        public string PeriodoLectivo { get; set; }
        public string Idioma { get; set; }
        public ResponseTipoAsignaturaErpAcademico TipoAsignatura { get; set; }
        public ResponseDatosGestorErpAcademico DatosGestor { get; set; }

    }
}
