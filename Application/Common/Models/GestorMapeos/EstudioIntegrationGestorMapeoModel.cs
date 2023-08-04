namespace Unir.Expedientes.Application.Common.Models.GestorMapeos
{
    public class EstudioIntegrationGestorMapeoModel
    {
        public int Id { get; set; }
        public int PlantillaEstudioIntegracionId { get; set; }
        public int VersionPlanId { get; set; }
        public PlantillaEstudioIntegracionGestorMapeoModel PlantillaEstudioIntegracion { get; set; }
        public EstudioUnirIntegrationGestorMapeoModel EstudioUnir { get; set; }
    }

    public class PlantillaEstudioIntegracionGestorMapeoModel
    {
        public int Id { get; set; }
        public int PlantillaEstudioId { get; set; }
        public PlanIntegrationGestorMapeoModel Plan { get; set; }
    }
}
