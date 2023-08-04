using Unir.Expedientes.Domain.Entities;
using Unir.Framework.ApplicationSuperTypes.Mappings;

namespace Unir.Expedientes.Application.ExpedientesAlumnos.Queries.GetConfiguracionExpedienteUniversidadByIdExpediente
{
    public class ConfiguracionExpedienteUniversidadDto : IMapFrom<ConfiguracionExpedienteUniversidad>
    {
        public int Id { get; set; }
        public string IdRefUniversidad { get; set; }
        public string AcronimoUniversidad { get; set; }
        public string NombreUniversidad { get; set; }
        public string IdIntegracionUniversidad { get; set; }
        public string CodigoDocumental { get; set; }
        public int TamanyoMaximoFichero { get; set; }
        public int TiempoMaximoInactividad { get; set; }
    }
}
