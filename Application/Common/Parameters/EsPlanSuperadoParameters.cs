using System.Collections.Generic;

namespace Unir.Expedientes.Application.Common.Parameters
{
    public class EsPlanSuperadoParameters
    {
        public List<int> IdsAsignaturasPlanes { get; set; }
        public int IdNodo { get; set; }
        public int? IdVersionPlan { get; set; }
    }
}
