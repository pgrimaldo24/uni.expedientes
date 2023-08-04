using System.Collections.Generic;

namespace Unir.Expedientes.Application.Common.Models.ErpAcademico.Expedientes
{
    public class ValidateAlumnoMatriculacionErpAcademicoModel
    {
        public bool MatriculasOk { get; set; }
        public List<string> CausasFallosMatriculas { get; set; }
    }
}
