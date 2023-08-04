
using System.Collections.Generic;

namespace Unir.Expedientes.Infrastructure.ServicesClients.Models.Evaluaciones
{
    public class ResponseEvaluacion
    {
        public bool ConvocatoriaExtraordinaria { get; set; }
        public int IdTipoEvaluacion { get; set; }
        public string TipoEvaluacion { get; set; }
        public List<ResponseCategoriaEvaluacion> Categorias { get; set; }
    }
}
