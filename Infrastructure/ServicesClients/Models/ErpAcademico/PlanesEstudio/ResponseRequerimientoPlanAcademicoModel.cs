using System.Collections.Generic;

namespace Unir.Expedientes.Infrastructure.ServicesClients.Models.ErpAcademico.PlanesEstudio
{
    public class ResponseRequerimientoPlanAcademico
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public decimal CreditosRequeridos { get; set; }
        public List<ResponseTipoAsignaturaRequerimiento> TiposAsignaturasRequerimiento { get; set; }
        public List<ResponseTrayectoPlan> TrayectosPlanes { get; set; }
    }

    public class ResponseTipoAsignaturaRequerimiento
    {
        public int Id { get; set; }
        public decimal MinCreditos { get; set; }
        public decimal MaxCreditos { get; set; }
        public ResponseTipoAsignaturaExpediente TipoAsignatura { get; set; }
    }

    public class ResponseTipoAsignaturaExpediente
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
    }

    public class ResponseTrayectoPlan
    {
        public int Id { get; set; }
        public bool EsGenerico { get; set; }
        public ResponseNodoInicial NodoInicial { get; set; }
        public ResponseNodoFinal NodoFinal { get; set; }
    }

    public class ResponseNodoInicial
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
    }

    public class ResponseNodoFinal
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public List<ResponseHito> Hitos { get; set; }
    }

    public class ResponseHito
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public ResponseHitoEspecializacion HitoEspecializacion { get; set; }
        public ResponseHitoTitulo HitoTitulo { get; set; }
    }

    public class ResponseHitoEspecializacion
    {
        public int Id { get; set; }
        public ResponseEspecializacion Especializacion { get; set; }
    }

    public class ResponseEspecializacion
    {
        public int Id { get; set; }
        public string Codigo { get; set; }
        public string Nombre { get; set; }
    }

    public class ResponseHitoTitulo
    {
        public int Id { get; set; }
        public ResponseTitulo Titulo { get; set; }
    }

    public class ResponseTitulo
    {
        public int Id { get; set; }
        public string Codigo { get; set; }
        public string Nombre { get; set; }
    }
}
