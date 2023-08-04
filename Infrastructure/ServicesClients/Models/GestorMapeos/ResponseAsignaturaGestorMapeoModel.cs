namespace Unir.Expedientes.Infrastructure.ServicesClients.Models.GestorMapeos
{
    public class ResponseAsignaturaGestorMapeoModel
    {
        public int Id { get; set; }
        public int AsignaturaPlanIntegracionId { get; set; }
        public ResponseAsignaturaUnirGestorMapeoModel AsignaturaUnir { get; set; }
        public ResponseAsignaturaPlanGestorMapeoModel AsignaturaPlan { get; set; }
    }

    public class ResponseAsignaturaUnirGestorMapeoModel
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string TipoAsignatura { get; set; }
        public int Creditos { get; set; }
        public string PeriodoLectivo { get; set; }
        public int Curso { get; set; }
        public string Activo { get; set; }
        public int EstudioUnirId { get; set; }
        public ResponseEstudioUnirGestorMapeoModel EstudioUnir { get; set; }
    }

    public class ResponseAsignaturaPlanGestorMapeoModel
    {
        public int Id { get; set; }
        public int UbicacionPeriodoLectivo { get; set; }
        public int PlanId { get; set; }
        public int AsignaturaId { get; set; }
        public int DuracionPeriodoLectivoId { get; set; }
        public ResponsePlanGestorMapeoModel Plan { get; set; }
        public string DisplayName { get; set; }
    }

    public class ResponseEstudioUnirGestorMapeoModel
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string PlanEstudio { get; set; }
        public string RamaEstudio { get; set; }
        public string Activo { get; set; }
    }

    public class ResponsePlanGestorMapeoModel
    {
        public int Id { get; set; }
        public int Anyo { get; set; }
        public string Codigo { get; set; }
        public string Nombre { get; set; }
        public bool EsOficial { get; set; }
        public int EstudioId { get; set; }
        public int TituloId { get; set; }
        public string DisplayName { get; set; }
    }
}
