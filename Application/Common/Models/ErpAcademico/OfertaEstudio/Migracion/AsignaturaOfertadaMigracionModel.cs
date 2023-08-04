using System.Collections.Generic;
using Unir.Expedientes.Application.Common.Models.ErpAcademico.PlanesEstudio;

namespace Unir.Expedientes.Application.Common.Models.ErpAcademico.OfertaEstudio.Migracion
{
    public class AsignaturaOfertadaMigracionModel
    {
        public AsignaturaOfertadaMigracionModel()
        {
            AsignaturaMatriculadas = new List<AsignaturaMatriculadaMigracionModel>();
        }
        public int Id { get; set; }
        public int Orden { get; set; }
        public TipoAsignaturaErpAcademicoModel TipoAsignatura { get; set; }
        public CursoErpAcademicoModel Curso { get; set; }
        public PeriodoLectivoModel PeriodoLectivo { get; set; }
        public AsignaturaPlanErpAcademicoModel AsignaturaPlan { get; set; }
        public List<AsignaturaMatriculadaMigracionModel> AsignaturaMatriculadas { get; set; }
    }
}
