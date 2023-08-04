using System.Collections.Generic;
using System.Linq;

namespace Unir.Expedientes.Application.Common.Models.ErpAcademico.PlanesEstudio
{
    public class RequerimientoPlanAcademicoModel
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public decimal CreditosRequeridos { get; set; }
        public bool EsGenerico => TrayectosPlanes != null && TrayectosPlanes.Any(tp => tp.EsGenerico);
        public List<TipoAsignaturaRequerimientoModel> TiposAsignaturasRequerimiento { get; set; }
        public List<TrayectoPlanModel> TrayectosPlanes { get; set; }
    }

    public class TipoAsignaturaRequerimientoModel
    {
        public int Id { get; set; }
        public decimal MinCreditos { get; set; }
        public decimal MaxCreditos { get; set; }
        public TipoAsignaturaExpedienteModel TipoAsignatura { get; set; }
    }

    public class TipoAsignaturaExpedienteModel
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
    }

    public class TrayectoPlanModel
    {
        public int Id { get; set; }
        public bool EsGenerico { get; set; }
        public NodoInicialModel NodoInicial { get; set; }
        public NodoFinalModel NodoFinal { get; set; }
    }

    public class NodoInicialModel
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
    }

    public class NodoFinalModel
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string NombreNodo => Hitos.First().HitoTitulo != null
            ? Hitos.First().HitoTitulo.Titulo.Nombre
            : Hitos.First().HitoEspecializacion.Especializacion.Nombre;
        public List<HitoModel> Hitos { get; set; }
    }

    public class HitoModel
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public HitoEspecializacionModel HitoEspecializacion { get; set; }
        public HitoTituloModel HitoTitulo { get; set; }
    }

    public class HitoEspecializacionModel
    {
        public int Id { get; set; }
        public EspecializacionModel Especializacion { get; set; }
    }

    public class EspecializacionModel
    {
        public int Id { get; set; }
        public string Codigo { get; set; }
        public string Nombre { get; set; }
    }

    public class HitoTituloModel
    {
        public int Id { get; set; }
        public TituloModel Titulo { get; set; }
    }

    public class TituloModel
    {
        public int Id { get; set; }
        public string Codigo { get; set; }
        public string Nombre { get; set; }
    }
}
