using System.Collections.Generic;

namespace Unir.Expedientes.Infrastructure.ServicesClients.Models.ExpedientesGestorUnir
{
    public class ResponseReconocimientoServicioGestorUnir
    {
        public int CodigoResultado { get; set; }
        public ResponseReconocimientoGestorUnir Reconocimiento { get; set; }
    }

    public class ResponseReconocimientoGestorUnir
    {
        public int IdMatricula { get; set; }
        public ResponseTransversalGestorUnir Transversal { get; set; }
        public ICollection<ResponseAsignaturaGestorUnir> Asignaturas { get; set; }
        public ICollection<ResponseSeminarioGestorUnir> Seminarios { get; set; }
        public ResponseExtensionUniversitariaGestorUnir ExtensionUniversitaria { get; set; }
    }

    public class ResponseTransversalGestorUnir
    {
        public decimal NotaMedia { get; set; }
        public decimal Ects { get; set; }
        public ICollection<ResponseReconocimientoCommonGestorUnir> Reconocimientos { get; set; }
    }

    public class ResponseAsignaturaGestorUnir
    {
        public int IdAsignaturaUnir { get; set; }
        public int IdTipoAsignatura { get; set; }
        public string TipoAsignaturaDescripcion { get; set; }
        public decimal NotaMedia { get; set; }
        public ICollection<ResponseReconocimientoCommonGestorUnir> Reconocimientos { get; set; }
    }

    public class ResponseSeminarioGestorUnir
    {
        public int IdEstudioSeminario { get; set; }
        public string NombreSeminario { get; set; }
        public decimal Ects { get; set; }
        public int IdMatriculaSeminario { get; set; }
    }

    public class ResponseExtensionUniversitariaGestorUnir
    {
        public decimal Ects { get; set; }
        public ICollection<ResponseReconocimientoCommonGestorUnir> Reconocimientos { get; set; }
    }

    public class ResponseReconocimientoCommonGestorUnir
    {
        public int IdTipoConvalidacion { get; set; }
        public int IdAsignaturaExterna { get; set; }
        public string AsignaturaExterna { get; set; }
        public decimal EctsExterna { get; set; }
        public string TipoAsignaturaExternaDescripcion { get; set; }
        public string EstudioExterno { get; set; }
        public decimal Nota { get; set; }
        public string NivelAprobacionDescripcion { get; set; }
        public int IdEstadoSolicitud { get; set; }
        public string TipoSolicitud { get; set; }
        public int IdAsignaturaOrigen { get; set; }
        public string ConvocatoriaOrigen { get; set; }
        public string CicloOrigen { get; set; }
        public string FechaFinalizacion { get; set; }
        public string TextoDgair { get; set; }
    }
}
