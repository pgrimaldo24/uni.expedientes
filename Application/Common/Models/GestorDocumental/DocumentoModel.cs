namespace Unir.Expedientes.Application.Common.Models.GestorDocumental
{
    public class DocumentoModel
    {
        public string Id { get; set; }
        public string Nombre { get; set; }
        public string Codigo { get; set; }
        public string Error { get; set; }
        public bool HasError => !string.IsNullOrEmpty(Error);
        public int ElementoNegocio { get; set; }
        public int IdDocumentoAlumno { get; set; }
    }
}
