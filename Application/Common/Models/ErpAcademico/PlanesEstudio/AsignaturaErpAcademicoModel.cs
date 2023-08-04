using Unir.Expedientes.Application.Common.Models.ErpAcademico.Global;

namespace Unir.Expedientes.Application.Common.Models.ErpAcademico.PlanesEstudio
{
    public class AsignaturaErpAcademicoModel
    {
        public int Id { get; set; }
        public int IdAsignaturaPlan { get; set; }
        public string Nombre { get; set; }
        public string Codigo { get; set; }
        public double Creditos { get; set; }
        public string CodigoOficialExterno { get; set; }
        public string IdentificadorOficialExterno { get; set; }
        public string Curso { get; set; }
        public string Periodicidad { get; set; }
        public string PeriodicidadCodigoOficialExterno { get; set; }
        public string PeriodoLectivo { get; set; }
        public string Idioma { get; set; }
        public TipoAsignaturaErpAcademicoModel TipoAsignatura { get; set; }
        public DatosGestorErpAcademicoModel DatosGestor { get; set; }
        public IdiomaAcademicoModel IdiomaImparticion { get; set; }
    }
}
