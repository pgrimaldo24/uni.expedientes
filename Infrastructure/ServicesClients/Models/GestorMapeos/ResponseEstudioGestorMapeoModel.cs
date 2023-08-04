namespace Unir.Expedientes.Infrastructure.ServicesClients.Models.GestorMapeos
{
    public class ResponseEstudioGestorMapeoModel
    {
        public int Id { get; set; }
        public int PlantillaEstudioIntegracionId { get; set; }
        public int VersionPlanId { get; set; }
        public ResponsePlantillaEstudioIntegracionGestorMapeoModel PlantillaEstudioIntegracion { get; set; }
        public ResponseEstudioUnirGestorMapeoModel EstudioUnir { get; set; }
    }

    public class ResponsePlantillaEstudioIntegracionGestorMapeoModel
    {
        public int Id { get; set; }
        public int PlantillaEstudioId { get; set; }
        public ResponsePlanGestorMapeoModel Plan { get; set; }
    }
}
