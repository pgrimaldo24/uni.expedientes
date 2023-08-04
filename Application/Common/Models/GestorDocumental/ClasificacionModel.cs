namespace Unir.Expedientes.Application.Common.Models.GestorDocumental
{
    public class ClasificacionModel
    {        
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Codigo { get; set; }
        public string Codificacion { get; set; }
        public string DisplayName { get; set; }
        public bool ConfirmarModificacion { get; set; }
    }
}
