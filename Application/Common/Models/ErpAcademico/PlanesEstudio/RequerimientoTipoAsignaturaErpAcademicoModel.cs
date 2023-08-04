namespace Unir.Expedientes.Application.Common.Models.ErpAcademico.PlanesEstudio
{
    public class RequerimientoTipoAsignaturaErpAcademicoModel
    {
        public double CreditosMinimos { get; set; }
        public double CreditosMaximos { get; set; }
        public double? CreditosObtenidos { get; set; }
        public TipoAsignaturaErpAcademicoModel TipoAsignatura { get; set; }
    }
}
