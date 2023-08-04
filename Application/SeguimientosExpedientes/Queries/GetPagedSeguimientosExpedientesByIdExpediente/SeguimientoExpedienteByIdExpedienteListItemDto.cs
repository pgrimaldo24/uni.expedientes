using System;
using Unir.Expedientes.Domain.Entities;
using Unir.Framework.ApplicationSuperTypes.Mappings;

namespace Unir.Expedientes.Application.SeguimientosExpedientes.Queries.GetPagedSeguimientosExpedientesByIdExpediente
{
    public class SeguimientoExpedienteByIdExpedienteListItemDto : IMapFrom<SeguimientoExpediente>
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; }
        public string IdRefTrabajador { get; set; }
        public string IdCuentaSeguridad { get; set; }
        public string Descripcion { get; set; }
        public string NombreTrabajador { get; set; }
        public virtual TipoSeguimientoExpedienteByExpedienteListItemDto TipoSeguimiento { get; set; }
    }

    public class TipoSeguimientoExpedienteByExpedienteListItemDto : IMapFrom<TipoSeguimientoExpediente>
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
    }
}
