using System.Collections.Generic;

namespace Unir.Expedientes.Application.AsignaturasExpediente.Queries.GetReconocimientos
{
    public class ReconocimientoClasificacionDto
    {
        public List<AsignaturaReconocimientoDto> AsignaturasReconocimientos { get; set; }
        public List<string> MensajesError { get; set; }
        public ReconocimientoClasificacionDto()
        {
            AsignaturasReconocimientos = new List<AsignaturaReconocimientoDto>();
            MensajesError = new List<string>();
        }
    }

    public class AsignaturaReconocimientoDto
    {
        public int? IdAsignaturaPlanErp { get; set; }
        public string NombreAsignatura { get; set; }
        public string CodigoAsignatura { get; set; }
        public string IdRefTipoAsignatura { get; set; }
        public string NombreTipoAsignatura { get; set; }
        public string Calificacion { get; set; }
        public string CalificacionAsignaturaErp { get; set; }
        public double Ects { get; set; }
        public bool EsExtensionUniversitaria { get; set; }
        public bool EsTransversal { get; set; }
        public bool EsAsignatura { get; set; }
        public bool EsSeminario { get; set; }
        public bool EsTransversalPrincipal { get; set; }
    }
}
