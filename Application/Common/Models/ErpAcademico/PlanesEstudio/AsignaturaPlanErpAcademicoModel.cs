namespace Unir.Expedientes.Application.Common.Models.ErpAcademico.PlanesEstudio
{
    public class AsignaturaPlanErpAcademicoModel
    {
        public int Id { get; set; }
        public string DisplayName { get; set; }
        public string DisplayNameNivelUso { get; set; }
        public AsignaturaErpAcademicoModel Asignatura { get; set; }
        public PlanAcademicoModel Plan { get; set; }
        public int Orden { get; set; }
    }
}
