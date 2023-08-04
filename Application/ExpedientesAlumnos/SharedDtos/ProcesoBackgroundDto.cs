using System;
using System.Collections.Generic;

namespace Unir.Expedientes.Application.ExpedientesAlumnos.SharedDtos
{
    public class ProcesoBackgroundDto
    {
        public ProcesoBackgroundDto()
        {
            Warnings = new List<string>();
            Errors = new List<string>();
            CreatedDateTime = DateTime.UtcNow;
        }
        public string Id { get; set; }
        public string Descripcion { get; set; }
        public string CreadoPor { get; set; }
        public int TotalRegistros { get; set; }
        public int Ok { get; set; }
        public int Fallidos { get; set; }
        public List<string> Errors { get; set; }
        public List<string> Warnings { get; set; }
        public int[] IdsExpedientes { get; set; }
        public DateTime CreatedDateTime { get; set; }
    }

    public class BackgroundDetailDto
    {
        public bool HasError { get; set; }
        public bool HasWarning { get; set; }
        public string Message { get; set; }
    }
}
