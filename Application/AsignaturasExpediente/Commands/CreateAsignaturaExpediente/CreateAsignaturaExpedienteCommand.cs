using MediatR;

namespace Unir.Expedientes.Application.AsignaturasExpediente.Commands.CreateAsignaturaExpediente
{
    public class CreateAsignaturaExpedienteCommand : IRequest<int>
    {

        public string IdRefAsignaturaPlan { get; set; }
        public string NombreAsignatura { get; set; }
        public string CodigoAsignatura { get; set; }
        public int OrdenAsignatura { get; set; }
        public double Ects { get; set; }
        public string IdRefTipoAsignatura { get; set; }
        public string SimboloTipoAsignatura { get; set; }
        public int OrdenTipoAsignatura { get; set; }
        public string NombreTipoAsignatura { get; set; }
        public string IdRefCurso { get; set; }
        public int NumeroCurso { get; set; }
        public int AnyoAcademicoInicio { get; set; }
        public int AnyoAcademicoFin { get; set; }
        public string PeriodoLectivo { get; set; }
        public string DuracionPeriodo { get; set; }
        public string SimboloDuracionPeriodo { get; set; }
        public string IdRefIdiomaImparticion { get; set; }
        public string SimboloIdiomaImparticion { get; set; }
        public bool Reconocida { get; set; }
        public int SituacionAsignaturaId { get; set; }
        public int ExpedienteAlumnoId { get; set; }
        public int? AsignaturaExpedienteId { get; set; }
    }
}
