namespace Unir.Expedientes.Application.Common.Models.ErpAcademico.Global
{
    public class PersonaAcademicoModel
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Apellido1 { get; set; }
        public string Apellido2 { get; set; }
        public string Email { get; set; }
        public string IdGestorCepal { get; set; }
        public string IdSeguridad { get; set; }
        public virtual string DisplayName { get; set; }
    }
}
