using System;
using System.Runtime.CompilerServices;
using Unir.Expedientes.Application.Common.Models.ErpAcademico.OfertaEstudio;
using Unir.Expedientes.Application.Common.Models.Evaluaciones;

namespace Unir.Expedientes.Application.Common.Models.ErpAcademico.Matriculacion
{
    public class AsignaturaMatriculadaModel
    {
        public int Id { get; set; }
        public bool Activa { get; set; }
        public string IdRefCurso { get; set; }  
        public MatriculaModel Matricula { get; set; }
        public AsignaturaOfertadaModel AsignaturaOfertada { get; set; }
        public ConfiguracionVersionEscalaModel ConfiguracionVersionEscala { get; set; }
    }
}
