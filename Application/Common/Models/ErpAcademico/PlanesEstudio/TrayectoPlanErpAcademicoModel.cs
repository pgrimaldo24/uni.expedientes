namespace Unir.Expedientes.Application.Common.Models.ErpAcademico.PlanesEstudio
{
    public class TrayectoPlanErpAcademicoModel
    {
        public int Id { get; set; }
        public bool EsGenerico { get; set; }
        public int IdNodoInicial { get; set; }
        public int IdNodoFinal { get; set; }
        public int? IdRequerimientoPlan { get; set; }
    }
}
