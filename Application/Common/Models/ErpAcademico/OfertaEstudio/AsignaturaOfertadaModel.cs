using System.Collections.Generic;
using Unir.Expedientes.Application.Common.Models.ErpAcademico.Matriculacion;
using Unir.Expedientes.Application.Common.Models.ErpAcademico.PlanesEstudio;

namespace Unir.Expedientes.Application.Common.Models.ErpAcademico.OfertaEstudio
{
    public class AsignaturaOfertadaModel
    {
        public int Id { get; set; }
        public int Orden { get; set; }
        public TipoAsignaturaErpAcademicoModel TipoAsignatura { get; set; }
        public CursoErpAcademicoModel Curso { get; set; }
        public PeriodoLectivoModel PeriodoLectivo { get; set; }
        public AsignaturaPlanErpAcademicoModel AsignaturaPlan { get; set; }
        public List<AsignaturaMatriculadaModel> AsignaturaMatriculadas { get; set; }
    }
}
