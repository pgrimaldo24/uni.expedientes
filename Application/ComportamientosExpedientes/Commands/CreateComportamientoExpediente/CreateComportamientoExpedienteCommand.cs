using MediatR;
using System.Collections.Generic;

namespace Unir.Expedientes.Application.ComportamientosExpedientes.Commands.CreateComportamientoExpediente
{
    public class CreateComportamientoExpedienteCommand : IRequest<int>
    {
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public bool EstaVigente { get; set; }
        public IList<RequisitoComportamientoExpedienteDto> RequisitosComportamientosExpedientes { get; set; }
        public IList<NivelUsoComportamientoExpedienteDto> NivelesUsoComportamientosExpedientes { get; set; }
    }

    public class RequisitoComportamientoExpedienteDto
    {
        public RequisitoExpedienteDto RequisitoExpediente { get; set; }
        public TipoRequisitoExpedienteDto TipoRequisitoExpediente { get; set; }
    }
    public class RequisitoExpedienteDto
    {
        public int Id { get; set; }
    }

    public class TipoRequisitoExpedienteDto
    {
        public int Id { get; set; }
    }

    public class NivelUsoComportamientoExpedienteDto
    {
        public string IdRefUniversidad { get; set; }
        public string AcronimoUniversidad { get; set; }
        public string IdRefTipoEstudio { get; set; }
        public string NombreTipoEstudio { get; set; }
        public string IdRefEstudio { get; set; }
        public string NombreEstudio { get; set; }
        public string IdRefPlan { get; set; }
        public string NombrePlan { get; set; }
        public string IdRefTipoAsignatura { get; set; }
        public string NombreTipoAsignatura { get; set; }
        public string IdRefAsignaturaPlan { get; set; }
        public string IdRefAsignatura { get; set; }
        public string NombreAsignatura { get; set; }
        public TipoNivelUsoDto TipoNivelUso { get; set; }
    }

    public class TipoNivelUsoDto
    {
        public int Id { get; set; }
    }
}