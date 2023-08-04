using Unir.Expedientes.Infrastructure.ServicesClients.Models.ErpAcademico.Titulos;

namespace Unir.Expedientes.Infrastructure.ServicesClients.Models.ErpAcademico.PlanesEstudio
{
    public class ResponseHitoErpAcademico
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public ResponseTituloErpAcademico Titulo { get; set; }
        public ResponseEspecializacionErpAcademico Especializacion { get; set; }
    }
}
