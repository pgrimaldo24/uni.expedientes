using System.Collections.Generic;

namespace Unir.Expedientes.Application.Common.Parameters
{
    public class AsignaturaMatriculadaCursoAlumnosParameters
    {
        public int FilterIdRefCurso { get; set; }
        public List<string> FilterIdRefsAlumno { get; set; }
    }
}
