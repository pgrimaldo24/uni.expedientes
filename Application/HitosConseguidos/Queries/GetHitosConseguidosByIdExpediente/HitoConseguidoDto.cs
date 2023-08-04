using System;
using Unir.Expedientes.Domain.Entities;
using Unir.Framework.ApplicationSuperTypes.Mappings;

namespace Unir.Expedientes.Application.HitosConseguidos.Queries.GetHitosConseguidosByIdExpediente
{
    public class HitoConseguidoDto : IMapFrom<HitoConseguido>
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public TipoHitoConseguidoDto TipoConseguido { get; set; }
    }

    public class TipoHitoConseguidoDto : IMapFrom<TipoHitoConseguido>
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Icono { get; set; }
        public bool EsPrimeraMatricula => Id == TipoHitoConseguido.PrimeraMatricula;
        public bool EsTrabajoFinEstudio => Id == TipoHitoConseguido.TrabajoFinEstudio;
    }
}
