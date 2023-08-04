using System;
using System.Collections.Generic;

namespace Unir.Expedientes.Application.Common.Models.ExpedientesGestorUnir
{
    public class ExpedienteErpComercialIntegrationModel
    {
        /*CODIGOS DE EXPEDIENTES */
        private const int CodigoExpedienteCorrecto = 0;
        private const int CodigoExpedienteNoEncontrado = 1;
        private const int CodigoExpedientePlanSinTraduccionPlantilla = 2;
        private const int CodigoExpedienteFalloLlamadaWs = 3;
        private const int CodigoExpedienteRestoErrores = 4;

        public int Codigo { get; set; }
        public int IdAlumno { get; set; }
        public int IdPlan { get; set; }
        public List<AsignaturaErpComercialExpedientesIntegrationModel> Asignaturas { get; set; }

        public string ViaAcceso { get; set; }
        public ViasAccesoIntegrationModel ViasAcceso { get; set; }
        public DateTime? FechaFinEstudio { get; set; }
        public DateTime? FechaExpedicion { get; set; }
        public string TituloTfmTfg { get; set; }
        public DateTime? FechaTfmTfg { get; set; }
        public double NotaMedia { get; set; }
        public IdiomaAcreditacionIntegrationModel IdiomaAcreditacion { get; set; }
        public List<ItinerariosFinalizadosIntegrationModel> ItinerariosFinalizados { get; set; }
        public TipoItinerarioIntegrationModel TipoItinerario { get; set; }
        public PracticaIntegrationModel Practica { get; set; }
        public bool Bloqueado { get; set; }
        public List<BloqueosIntegrationModel> Bloqueos { get; set; }

        public bool EsCodigoCorrecto => Codigo is CodigoExpedienteCorrecto or CodigoExpedienteNoEncontrado;

        public bool EsCodigoInCorrecto =>
            Codigo is CodigoExpedientePlanSinTraduccionPlantilla or CodigoExpedienteFalloLlamadaWs or
                CodigoExpedienteRestoErrores;
    }
}
