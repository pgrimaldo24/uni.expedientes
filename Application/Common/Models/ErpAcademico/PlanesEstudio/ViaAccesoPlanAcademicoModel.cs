namespace Unir.Expedientes.Application.Common.Models.ErpAcademico.PlanesEstudio
{
    public class ViaAccesoPlanAcademicoModel
    {
        public int Id { get; set; }
        public ViaAccesoAcademicoModel ViaAcceso { get; set; }
        public NodoErpAcademicoModel Nodo { get; set; }
    }
}
