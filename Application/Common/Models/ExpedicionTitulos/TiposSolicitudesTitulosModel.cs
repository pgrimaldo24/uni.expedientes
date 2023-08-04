namespace Unir.Expedientes.Application.Common.Models.ExpedicionTitulos
{
    public class TiposSolicitudesTitulosModel
    {
        public int Id { get; set; }
        public string RefCodigoTipoSolicitud { get; set; }
        public string Nombre { get; set; }
        public bool ConFechaPago { get; set; }
        public bool DestinoAlumno { get; set; }
        public FormatoTipoSolicitud FormatoTipoSolicitud { get; set; }
        public string DisplayName => $"({RefCodigoTipoSolicitud}) {Nombre}";
    }

    public class FormatoTipoSolicitud
    {
        public string Codigo { get; set; }
    }
}
