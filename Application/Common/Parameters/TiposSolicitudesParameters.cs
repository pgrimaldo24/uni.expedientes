using System.Collections.Generic;
using System.Text.Json.Serialization;
using Unir.Expedientes.Application.Common.Models.Settings;

namespace Unir.Expedientes.Application.Common.Parameters
{
    public class TiposSolicitudesParameters
    {
        public TiposSolicitudesParameters()
        {
            IdsRefUniversidad = new List<string>();
        }
        public string FilterDisplayName { get; set; }
        public List<string> IdsRefUniversidad { get; set; }
        public bool? ConFechaPago { get; set; }
    }
}
