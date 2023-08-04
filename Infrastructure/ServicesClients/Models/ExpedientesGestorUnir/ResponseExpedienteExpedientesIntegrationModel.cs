using System;
using System.Collections.Generic;

namespace Unir.Expedientes.Infrastructure.ServicesClients.Models.ExpedientesGestorUnir
{
    public class ResponseExpedienteExpedientesIntegrationModel
    {
        public int CodigoResultado { get; set; }
        public string ViaAcceso { get; set; }
        public ResponseViasAccesoIntegrationModel ViasAcceso { get; set; }
        public DateTime? FechaFinEstudio { get; set; }
        public DateTime? FechaExpedicion { get; set; }
        public string TituloTfmTfg { get; set; }
        public DateTime? FechaTfmTfg { get; set; }
        public double NotaMedia { get; set; }
        public bool Bloqueado { get; set; }
        public ResponseIdiomaAcreditacionIntegrationModel IdiomaAcreditacion { get; set; }
        public ResponsePracticaIntegrationModel Practica { get; set; }
        public List<ResponseBloqueosIntegrationModel> Bloqueos { get; set; }
        public List<ResponseItinerariosFinalizadosIntegrationModel> ItinerariosFinalizados { get; set; }
        public List<ResponseAsignaturaErpAcademicoExpedientesIntegrationModel> Asignaturas { get; set; }
    }
}
