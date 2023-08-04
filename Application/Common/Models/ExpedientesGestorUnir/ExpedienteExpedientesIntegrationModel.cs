using System;
using System.Collections.Generic;

namespace Unir.Expedientes.Application.Common.Models.ExpedientesGestorUnir
{
    public class ExpedienteExpedientesIntegrationModel
    {
        public const int SinResultados = 1;
        public ExpedienteExpedientesIntegrationModel()
        {
            ItinerariosFinalizados = new List<ItinerariosFinalizadosIntegrationModel>();
            Asignaturas = new List<AsignaturaErpAcademicoExpedientesIntegrationModel>();
            Bloqueos = new List<BloqueosIntegrationModel>();
        }

        public int CodigoResultado { get; set; }
        public string ViaAcceso { get; set; }
        public ViasAccesoIntegrationModel ViasAcceso { get; set; }
        public DateTime? FechaFinEstudio { get; set; }
        public DateTime? FechaExpedicion { get; set; }
        public string TituloTfmTfg { get; set; }
        public DateTime? FechaTfmTfg { get; set; }
        public double NotaMedia { get; set; }
        public bool Bloqueado { get; set; }
        public IdiomaAcreditacionIntegrationModel IdiomaAcreditacion { get; set; }
        public PracticaIntegrationModel Practica { get; set; }
        public List<BloqueosIntegrationModel> Bloqueos { get; set; }
        public List<ItinerariosFinalizadosIntegrationModel> ItinerariosFinalizados { get; set; }
        public List<AsignaturaErpAcademicoExpedientesIntegrationModel> Asignaturas { get; set; }
    }
}
