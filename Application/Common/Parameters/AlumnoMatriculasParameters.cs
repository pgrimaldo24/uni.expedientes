using System.Collections.Generic;

namespace Unir.Expedientes.Application.Common.Parameters
{
    public class AlumnoMatriculasParameters
    {
        public int Index { get; set; }
        public int Count { get; set; }
        public string IdIntegracionAlumno { get; set; }
        public List<string> IdsRefExpedientesAlumnos { get; set; }
    }
}
