namespace Unir.Expedientes.Application.Common.Models.ErpAcademico.PlanesEstudio
{
    public class AsignaturaPlanSubBloqueErpAcademicoModel
    {
        public int IdAsignaturaPlan { get; set; }
        public virtual TipoAsignaturaErpAcademicoModel TipoAsignatura { get; set; }
    }
}
