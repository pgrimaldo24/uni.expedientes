using System;
using System.Collections.Generic;
using System.Linq;
using Unir.Expedientes.Application.Tests.Common;
using Unir.Expedientes.Domain.Entities;
using Xunit;

namespace Unir.Expedientes.Domain.Tests.Entities
{
    [Collection("CommonTestCollection")]
    public class ExpedienteAlumnoQueryHandlerTests : TestBase
    {

        #region AddEspecializacion

        [Fact(DisplayName = "Cuando se envía la especialización se Agrega a la lista")]
        public void AddEspecializacion_AgregaALista()
        {
            //ARRANGE
            var idRefEspecializacion = "56464";
            var expedienteAlumno = new ExpedienteAlumno
            {
                Id = 1
            };
            //ACT

            expedienteAlumno.AddEspecializacion(idRefEspecializacion);

            //ASSERT
            Assert.Equal(1, expedienteAlumno.ExpedientesEspecializaciones.Count);
            Assert.Equal(idRefEspecializacion, expedienteAlumno.ExpedientesEspecializaciones.ToList()[0].IdRefEspecializacion);
            Assert.Equal(expedienteAlumno.Id, expedienteAlumno.ExpedientesEspecializaciones.ToList()[0].ExpedienteAlumnoId);
        }

        #endregion

        #region HasEspecializacion

        [Fact(DisplayName = "Cuando tiene especialización Devuelve true")]
        public void HasEspecializacion_True()
        {
            //ARRANGE
            var idRefEspecializacion = "56464";
            var idExpedienteAlumno = 1;
            var expedienteAlumno = new ExpedienteAlumno()
            {
                Id = idExpedienteAlumno,
                ExpedientesEspecializaciones = new List<ExpedienteEspecializacion>
                {
                    new()
                    {
                        Id = 1,
                        IdRefEspecializacion = idRefEspecializacion,
                        ExpedienteAlumnoId = idExpedienteAlumno
                    }
                }
            };
            //ACT

            var actual = expedienteAlumno.HasEspecializacion(idRefEspecializacion);

            //ASSERT
            Assert.True(actual);
        }

        [Fact(DisplayName = "Cuando no tiene especialización Devuelve false")]
        public void HasEspecializacion_False()
        {
            //ARRANGE
            var idRefEspecializacion = "56464";
            var idExpedienteAlumno = 1;
            var expedienteAlumno = new ExpedienteAlumno()
            {
                Id = idExpedienteAlumno,
                ExpedientesEspecializaciones = new List<ExpedienteEspecializacion>
                {
                    new()
                    {
                        Id = 1,
                        IdRefEspecializacion = "9876",
                        ExpedienteAlumnoId = idExpedienteAlumno
                    }
                }
            };
            //ACT

            var actual = expedienteAlumno.HasEspecializacion(idRefEspecializacion);

            //ASSERT
            Assert.False(actual);
        }

        #endregion

        #region DeleteEspecializacionesNoIncluidos

        [Fact(DisplayName = "Cuando no se envían las especializaciones existentes Elimina los registros")]
        public void DeleteEspecializacionesNoIncluidos_Ok()
        {
            //ARRANGE
            var expedienteAlumno = new ExpedienteAlumno()
            {
                Id = 1,
                ExpedientesEspecializaciones = new List<ExpedienteEspecializacion>
                {
                    new ExpedienteEspecializacion{IdRefEspecializacion = "3", Id = 1}
                }
            };

            //ACT
            expedienteAlumno.DeleteEspecializacionesNoIncluidos(new[] { "1", "2" });

            //ASSERT
            Assert.Equal(0, expedienteAlumno.ExpedientesEspecializaciones.Count);
        }

        #endregion

        #region AddHitosConseguidos

        [Fact(DisplayName = "Cuando se envía una lista Especializaciones se Agrega a la lista un Hito Conseguido por cada Especialización")]
        public void AddHitosConseguidos_AgregaALista()
        {
            var tipoHitoConseguido = new TipoHitoConseguido
            {
                Id = TipoHitoConseguido.PreMatriculado,
                Nombre = "Prematriculado"
            };
            var fechaInicio = DateTime.Now;
            var especializacion1 = new ExpedienteEspecializacion
            {
                Id = 1,
                IdRefEspecializacion = "10"
            };
            var especializacion2 = new ExpedienteEspecializacion
            {
                Id = 2,
                IdRefEspecializacion = "20"
            };
            var expedienteAlumno = new ExpedienteAlumno()
            {
                Id = 1,
                ExpedientesEspecializaciones = new List<ExpedienteEspecializacion>
                {
                    especializacion1, especializacion2
                }
            };

            //ACT
            expedienteAlumno.AddHitosConseguidos(tipoHitoConseguido, fechaInicio);

            //ASSERT
            Assert.Single(expedienteAlumno.HitosConseguidos);
            Assert.Equal(tipoHitoConseguido.Id, expedienteAlumno.HitosConseguidos.First().TipoConseguidoId);
            Assert.Equal(tipoHitoConseguido.Nombre, expedienteAlumno.HitosConseguidos.First().Nombre);
            Assert.Equal(fechaInicio, expedienteAlumno.HitosConseguidos.First().FechaInicio);
            Assert.Null(expedienteAlumno.HitosConseguidos.First().ExpedienteEspecializacion);
        }
        #endregion

        #region CambiarSituacionAsignaturas

        [Fact(DisplayName = "Cuando se cambia de situación a las asignaturas Retorna Ok")]
        public void CambiarSituacionAsignaturas()
        {
            //ARRANGE
            var asignaturasModificar = new List<int> { 1, 2, 3 };
            var expedienteAlumno = new ExpedienteAlumno()
            {
                Id = 1,
                AsignaturasExpedientes = new List<AsignaturaExpediente>()
                {
                    new ()
                    {
                        Id = 1,
                        IdRefAsignaturaPlan = "1",
                        SituacionAsignaturaId = 1
                    },
                    new ()
                    {
                        Id = 2,
                        IdRefAsignaturaPlan = "2",
                        SituacionAsignaturaId = 1
                    },
                    new ()
                    {
                        Id = 3,
                        IdRefAsignaturaPlan = "3",
                        SituacionAsignaturaId = 1
                    }
                }
            };

            //ACT
            expedienteAlumno.CambiarSituacionAsignaturas(asignaturasModificar, 6);

            //ASSERT
            Assert.True(expedienteAlumno.AsignaturasExpedientes.All(x=>x.SituacionAsignaturaId == 6));
        }

        #endregion

    }
}
