namespace Unir.Expedientes.Application.Common.Models.GestorMapeos
{
    public class AsignaturaIntegrationGestorMapeoModel
    {
        public int Id { get; set; }
        public int AsignaturaPlanIntegracionId { get; set; }
        public AsignaturaUnirIntegrationGestorMapeoModel AsignaturaUnir { get; set; }
        public AsignaturaPlanIntegrationGestorMapeoModel AsignaturaPlan { get; set; }
    }

    public class AsignaturaUnirIntegrationGestorMapeoModel
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string TipoAsignatura { get; set; }
        public int Creditos { get; set; }
        public string PeriodoLectivo { get; set; }
        public int Curso { get; set; }
        public string Activo { get; set; }
        public int EstudioUnirId { get; set; }
        public EstudioUnirIntegrationGestorMapeoModel EstudioUnir { get; set; }
    }

    public class AsignaturaPlanIntegrationGestorMapeoModel
    {
        public int Id { get; set; }
        public int UbicacionPeriodoLectivo { get; set; }
        public int PlanId { get; set; }
        public int AsignaturaId { get; set; }
        public int DuracionPeriodoLectivoId { get; set; }
        public PlanIntegrationGestorMapeoModel Plan { get; set; }
        public string DisplayName { get; set; }
    }

    public class EstudioUnirIntegrationGestorMapeoModel
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public int TipoEstudioSegunUnirId { get; set; }
    }

    public class PlanIntegrationGestorMapeoModel
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
