using Unir.Expedientes.Application.Common.Models.ErpAcademico.Matriculacion;
using Unir.Expedientes.Application.Common.Models.ErpAcademico.OfertaEstudio;

namespace Unir.Expedientes.Infrastructure.ServicesClients.Models.ErpAcademico.Matriculacion
{
    public class ResponseAsignaturaMatricula
    {
        public string Id { get; set; }
        public bool Activa { get; set; }
        public string IdRefCurso { get; set; }
        public MatriculaModel Matricula { get; set; }
        public AsignaturaOfertadaModel AsignaturaOfertada { get; set; }
    }
}
