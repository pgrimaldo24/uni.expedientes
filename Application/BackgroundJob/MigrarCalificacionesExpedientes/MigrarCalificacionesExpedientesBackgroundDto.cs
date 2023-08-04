using System.Collections.Generic;

namespace Unir.Expedientes.Application.BackgroundJob.MigrarCalificacionesExpedientes
{
    public class MigrarCalificacionesExpedientesBackgroundDto
    {
        public MigrarCalificacionesExpedientesBackgroundDto()
        {
            IdsExpedientesCorrectos = new List<int>();
            IdsExpedientesFallidos = new List<MigracionCalificacionExpedientesFallidos>();
        }
        public int Fallidos { get; set; }
        public int Correctos { get; set; }
        public int TotalExpedientesEncontrados { get; set; }
        public List<int> IdsExpedientesCorrectos { get; set; }
        public List<MigracionCalificacionExpedientesFallidos> IdsExpedientesFallidos { get; set; }
    }

    public class MigracionCalificacionExpedientesFallidos
    {
        public int IdExpediente { get; set; }
        public string Mensaje { get; set; }
    }
}
