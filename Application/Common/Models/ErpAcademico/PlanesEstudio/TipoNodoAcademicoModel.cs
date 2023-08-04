namespace Unir.Expedientes.Application.Common.Models.ErpAcademico.PlanesEstudio
{
    public class TipoNodoAcademicoModel
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public bool EsInicial { get; set; }
        public bool EsIntermedio { get; set; }
        public bool EsFinal { get; set; }
    }
}
