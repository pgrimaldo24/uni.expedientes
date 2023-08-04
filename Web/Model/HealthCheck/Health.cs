using System;
using System.Collections.Generic;

namespace Unir.Expedientes.WebUi.Model.HealthCheck
{
    public class Health
    {
        public string Name { get; set; }
        public bool Healthy { get; set; }
        public string Version { get; set; }
        public string Descripcion { get; set; }
        public DateTime Timestamp { get; set; }
        public IList<HealthCheck> Checks { get; set; }
        public IList<KeyValuePair<string, string>> Metadata { get; set; }
    }
}
