using System.Collections.Generic;

namespace Unir.Expedientes.Application.Common.Models.ExpedientesGestorUnir
{
    public class ReconocimientoIntegrationGestorModel
    {
        public int CodigoResultado { get; set; }
        public ReconocimientoGestorModel Reconocimiento { get; set; }
    }

    public class ReconocimientoGestorModel
    {
        public int IdMatricula { get; set; }
        public TransversalGestorModel Transversal { get; set; }
        public List<AsignaturaGestorModel> Asignaturas { get; set; }
        public List<SeminarioGestorModel> Seminarios { get; set; }
        public ExtensionUniversitariaGestorModel ExtensionUniversitaria { get; set; }
    }

    public class TransversalGestorModel
    {
        public double NotaMedia { get; set; }
        public double Ects { get; set; }
        public List<ReconocimientoCommonGestorModel> Reconocimientos { get; set; }
    }

    public class AsignaturaGestorModel
    {
        public int IdAsignaturaUnir { get; set; }
        public int IdTipoAsignatura { get; set; }
        public string TipoAsignaturaDescripcion { get; set; }
        public double NotaMedia { get; set; }
        public List<ReconocimientoCommonGestorModel> Reconocimientos { get; set; }
    }

    public class SeminarioGestorModel
    {
        public int IdEstudioSeminario { get; set; }
        public string NombreSeminario { get; set; }
        public double Ects { get; set; }
        public int IdMatriculaSeminario { get; set; }
    }

    public class ExtensionUniversitariaGestorModel
    {
        public double Ects { get; set; }
        public List<ReconocimientoCommonGestorModel> Reconocimientos { get; set; }
    }

    public class ReconocimientoCommonGestorModel
    {
        public int IdTipoConvalidacion { get; set; }
        public int IdAsignaturaExterna { get; set; }
        public string AsignaturaExterna { get; set; }
        public double EctsExterna { get; set; }
        public string TipoAsignaturaExternaDescripcion { get; set; }
        public string EstudioExterno { get; set; }
        public double Nota { get; set; }
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
