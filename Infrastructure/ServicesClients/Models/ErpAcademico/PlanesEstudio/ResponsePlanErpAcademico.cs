using System.Collections.Generic;
using Unir.Expedientes.Infrastructure.ServicesClients.Models.ErpAcademico.Titulos;

namespace Unir.Expedientes.Infrastructure.ServicesClients.Models.ErpAcademico.PlanesEstudio
{
    public class ResponsePlanErpAcademico
    {
        public int Id { get; set; }
        public string Codigo { get; set; }
        public string Nombre { get; set; }
        public string Version { get; set; }
        public string DisplayName { get; set; }
        public ResponseEstudioErpAcademico Estudio { get; set; }
        public ResponseTituloErpAcademico Titulo { get; set; }
        public ICollection<ResponseVersionPlanErpAcademico> VersionesPlanes { get; set; }
    }
}
