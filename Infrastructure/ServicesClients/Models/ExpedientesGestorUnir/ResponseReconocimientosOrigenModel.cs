﻿namespace Unir.Expedientes.Infrastructure.ServicesClients.Models.ExpedientesGestorUnir
{
    public class ResponseReconocimientosOrigenModel
    {
        public string IdAsignaturaGestor { get; set; }
        public string TipoReconocimiento { get; set; }
        public string TipoAsignaturaExterna { get; set; }
        public string NombreAsignaturaExterna { get; set; }
        public string NombreEstudioExterno { get; set; }
        public double Nota { get; set; }
        public string Convocatoria { get; set; }
        public double Ects { get; set; }
        public string NivelAprobacion { get; set; }
        public string Anyo { get; set; }
    }
}
