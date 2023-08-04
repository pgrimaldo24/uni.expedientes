using System.Collections.Generic;

namespace Unir.Expedientes.Infrastructure.ServicesClients.Models.ErpAcademico.Expedientes
{
    public class ResponseValidateAlumnoMatriculacionErpAcademico
    {
        public bool MatriculasOk { get; set; }
        public List<string> CausasFallosMatriculas { get; set; }
    }
}
