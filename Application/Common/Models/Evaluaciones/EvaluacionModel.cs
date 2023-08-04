using System.Collections.Generic;

namespace Unir.Expedientes.Application.Common.Models.Evaluaciones
{
    public class EvaluacionModel
    {
        public bool ConvocatoriaExtraordinaria { get; set; }
        public int IdTipoEvaluacion { get; set; }
        public string TipoEvaluacion { get; set; }
        public List<CategoriaEvaluacionModel> Categorias { get; set; }
    }
}
