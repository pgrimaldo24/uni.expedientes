using System;
using Unir.Expedientes.Domain.Entities;
using Unir.Framework.ApplicationSuperTypes.Mappings;

namespace Unir.Expedientes.Application.TiposSituacionEstadoExpedientes.Queries.GetUltimoTipoSituacionEstadoByIdExpediente
{
    public class TipoSituacionEstadoExpedienteDto: IMapFrom<TipoSituacionEstadoExpediente>
    {
        public int Id { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public string Descripcion { get; set; }
        public TipoSituacionEstadoDto TipoSituacionEstado { get; set; }
    }

    public class TipoSituacionEstadoDto : IMapFrom<TipoSituacionEstadoExpediente>
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
    }
}
