using System;
using System.Linq;
using AutoMapper;
using Unir.Expedientes.Domain.Entities;
using Unir.Framework.ApplicationSuperTypes.Mappings;

namespace Unir.Expedientes.Application.ExpedientesAlumnos.Queries.GetPagedExpedientesAlumnos
{
    public class ExpedienteAlumnoPagedListItemDto : IMapFrom<ExpedienteAlumno>
    {
        // Referencia
        public int Id { get; set; }

        // Primitivas
        public string IdRefIntegracionAlumno { get; set; }
        public string IdRefPlan { get; set; }
        public string IdRefVersionPlan { get; set; }
        public string IdRefNodo { get; set; }
        public string AlumnoDisplayName { get; set; }
        public string PlanEstudioDisplayName { get; set; }
        public DateTime? FechaApertura { get; set; }
        public string UniversidadDisplayName { get; set; }
        public string CentroEstudioDisplayName { get; set; }
        public string TipoEstudioDisplayName { get; set; }
        public string EstudioDisplayName { get; set; }
        public string TituloDisplayName { get; set; }
        public string IdRefTipoDocumentoIdentificacionPais { get; set; }
        public string AlumnoNroDocIdentificacion { get; set; }
        public string AlumnoNombre { get; set; }
        public string AlumnoApellido1 { get; set; }
        public string AlumnoApellido2 { get; set; }
        public string NombreEstudio { get; set; }
        public string NombrePlan { get; set; }
        public DateTime? FechaFinalizacion { get; set; }
        public string AcronimoUniversidad { get; set; }
        public int? IdUniversidad { get; set; }
        public string EstadoDisplayName { get; set; }
        public string TipoSituacionDisplayName { get; set; }

        public string DisplayNameDocumentoIdentificacionAlumno =>
            $"{IdRefIntegracionAlumno} - {IdRefTipoDocumentoIdentificacionPais}/{AlumnoNroDocIdentificacion}".TrimEnd();

        public string DisplayNameNombreAlumno => $"{AlumnoNombre} {AlumnoApellido1} {AlumnoApellido2}".TrimEnd();

        // Utilidades Colecciones
        public int CountSeguimientos { get; set; }
        public int CountAnotaciones { get; set; }
        public void Mapping(Profile profile)
        {
            profile.CreateMap<ExpedienteAlumno, ExpedienteAlumnoPagedListItemDto>()
                .ForMember(e => e.CountSeguimientos, opt => opt.MapFrom(src => src.Seguimientos.Count))
                .ForMember(e => e.CountAnotaciones, opt => opt.MapFrom(src => src.Anotaciones.Count))
                .ForMember(e => e.EstadoDisplayName, opt => opt.MapFrom(src => src.Estado.Nombre.ToUpper()))
                .ForMember(e => e.TipoSituacionDisplayName, opt => opt.MapFrom(src => src.TiposSituacionEstadoExpedientes.OrderByDescending(tse => tse.Id).FirstOrDefault().TipoSituacionEstado.Nombre));
        }
    }
}
