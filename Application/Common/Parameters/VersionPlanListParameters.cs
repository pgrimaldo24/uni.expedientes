namespace Unir.Expedientes.Application.Common.Parameters
{
    public class VersionPlanListParameters
    {
        public string Search { get; set; }
        public int Index { get; set; }
        public int Count { get; set; }
        public int? FilterIdPlan { get; set; }
        public int? FilterNroFrom { get; set; }
        public int? FilterNroTo { get; set; }
    }
}
