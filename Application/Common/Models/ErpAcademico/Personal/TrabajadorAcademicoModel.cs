using Unir.Expedientes.Application.Common.Models.ErpAcademico.Global;

namespace Unir.Expedientes.Application.Common.Models.ErpAcademico.Personal
{
    public class TrabajadorAcademicoModel
    {
        public int Id { get; set; }
        public PersonaAcademicoModel Persona { get; set; }
        public string DisplayName { get; set; }
    }
}
