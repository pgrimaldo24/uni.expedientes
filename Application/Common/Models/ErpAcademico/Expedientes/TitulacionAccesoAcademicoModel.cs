using System;
namespace Unir.Expedientes.Application.Common.Models.ErpAcademico.Expedientes
{
    public class TitulacionAccesoAcademicoModel
    {
        public int Id { get; set; }
        public string Titulo { get; set; }
        public string TipoEstudio { get; set; }
        public string IdRefInstitucionDocente { get; set; }
        public string CodeCountryInstitucionDocente { get; set; }
        public string InstitucionDocente { get; set; }
        public string IdRefTerritorioInstitucionDocente { get; set; }
        public DateTime? FechaInicioTitulo { get; set; }
        public DateTime? FechafinTitulo { get; set; }
        public int? NroSemestreRealizados { get; set; }
        public string CodigoColegiadoProfesional { get; set; }
    }
}
