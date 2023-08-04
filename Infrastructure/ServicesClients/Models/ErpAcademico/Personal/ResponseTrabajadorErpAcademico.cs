using Unir.Expedientes.Infrastructure.ServicesClients.Models.ErpAcademico.Global;

namespace Unir.Expedientes.Infrastructure.ServicesClients.Models.ErpAcademico.Personal
{
    public class ResponseTrabajadorErpAcademico
    {
        public int Id { get; set; }
        public ResponsePersonaErpAcademico Persona { get; set; }
        public string DisplayName { get; set; }
    }
}
