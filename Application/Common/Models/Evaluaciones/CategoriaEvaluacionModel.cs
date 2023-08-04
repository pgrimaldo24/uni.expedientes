namespace Unir.Expedientes.Application.Common.Models.Evaluaciones
{
    public class CategoriaEvaluacionModel
    {
        public int Orden { get; set; }
        public int IdTipoCategoria { get; set; }
        public string TipoCategoria { get; set; }
        public double? Peso { get; set; }
        public double? NotaTruncada { get; set; }
        public bool NecesarioAprobar { get; set; }
        public double? PorcentajeAprobar { get; set; }
        public bool OcultarSubtotal { get; set; }
        public bool GuardarNotaOrdinaria { get; set; }
        public bool ConActividades { get; set; }
        public double? PorcentajeActividades { get; set; }
        public bool MostrarActividadesEnActa { get; set; }
        public bool MostrarNombresActividadesEnActa { get; set; }
        public bool NoCalificarConActividadesNoPuntuadas { get; set; }
        public string CodigoIntegracion { get; set; }
    }
}
