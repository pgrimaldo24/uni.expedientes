namespace Unir.Expedientes.Infrastructure.ServicesClients.Models.Evaluaciones
{
    public class ResponseActa
    {
        public bool PublicarNotasPorGrupo { get; set; }
        public bool GenerarActasPorGrupo { get; set; }
        public int MaximoDecimales { get; set; }
        public string CargoProfesoresFirma { get; set; }
        public bool IncluirDirector { get; set; }
        public string NombreDirector { get; set; }
        public string CargoDirector { get; set; }
        public int IdTipoImpresionCabeceraActa { get; set; }
        public string TipoImpresionCabeceraActa { get; set; }
        public bool ActivarFlujoFirmaActa { get; set; }
        public bool PermitirDescargarActa { get; set; }
    }
}
