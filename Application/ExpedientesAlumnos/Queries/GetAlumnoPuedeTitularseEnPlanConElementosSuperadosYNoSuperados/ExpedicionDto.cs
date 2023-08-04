using System;
using System.Collections.Generic;

namespace Unir.Expedientes.Application.ExpedientesAlumnos.Queries.GetAlumnoPuedeTitularseEnPlanConElementosSuperadosYNoSuperados
{
    public class ExpedicionDto
    {
        public ExpedicionDto()
        {
            ItinerariosFinalizados = new List<ItinerariosFinalizadosExpedicionDto>();
        }

        public string ViaAcceso { get; set; }
        public ViasAccesoExpedicionDto ViasAcceso { get; set; }
        public DateTime? FechaFinEstudio { get; set; }
        public DateTime? FechaExpedicion { get; set; }
        public string TituloTfmTfg { get; set; }
        public DateTime? FechaTfmTfg { get; set; }
        public double NotaMedia { get; set; }
        public IdiomaAcreditacionExpedicionDto IdiomaAcreditacion { get; set; }
        public List<ItinerariosFinalizadosExpedicionDto> ItinerariosFinalizados { get; set; }
        public TipoItinerarioExpedicionDto TipoItinerario { get; set; }
        public PracticaExpedicionDto Practica { get; set; }
    }

    public class ViasAccesoExpedicionDto
    {
        public string Generica { get; set; }
        public string Especifica { get; set; }
        public string GenericaIngles { get; set; }
        public string EspecificaIngles { get; set; }
    }

    public class IdiomaAcreditacionExpedicionDto
    {
        public string Idioma { get; set; }
        public string Acreditacion { get; set; }
        public DateTime FechaAcreditacion { get; set; }
    }

    public class ItinerariosFinalizadosExpedicionDto
    {
        public TipoItinerarioExpedicionDto TipoItinerario { get; set; }
        public string Nombre { get; set; }
        public DateTime? FechaFin { get; set; }
        public string IdEspecializacionErp { get; set; }
        public List<int> AsignaturasAsociadas { get; set; }
    }

    public class TipoItinerarioExpedicionDto
    {
        public string Id { get; set; }
        public string Nombre { get; set; }
    }

    public class PracticaExpedicionDto
    {
        public string CentroPractica { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
    }
}
