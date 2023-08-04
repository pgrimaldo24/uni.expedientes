namespace Unir.Expedientes.Application.ExpedientesAlumnos.Commands.EditExpedientesAlumnosByIntegracion
{
    public class EditExpedienteAlumnoByIdIntegracionParametersDto
    {
        public int Id { get; set; }
        public string IdRefVersionPlan { get; set; }
        public int? NroVersion { get; set; }
        public string IdRefNodo { get; set; }
    }
}
