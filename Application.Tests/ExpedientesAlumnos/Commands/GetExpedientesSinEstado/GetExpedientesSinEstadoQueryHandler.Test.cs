using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Unir.Expedientes.Application.ExpedientesAlumnos.Queries.GetExpedientesSinEstados;
using Unir.Expedientes.Application.Tests.Common;
using Unir.Expedientes.Domain.Entities;
using Xunit;

namespace Unir.Expedientes.Application.Tests.ExpedientesAlumnos.Commands.GetExpedientesSinEstado
{
    [Collection("CommonTestCollection")]
    public class GetExpedientesSinEstadoQueryHandlerTest : TestBase
    {
        #region handle

        [Fact(DisplayName = "Cuando encuentra expediente Retorna ok")]
        public async Task Handle_Expediente_Ok()
        {
            //ARRANGE
            await Context.ExpedientesAlumno.AddAsync(new ExpedienteAlumno
            {
                Id = 1,
                ExpedientesEspecializaciones = new List<ExpedienteEspecializacion>
                {
                    new ExpedienteEspecializacion
                    {
                        Id = 1,
                        IdRefEspecializacion = "1"
                    }
                },
                Estado = null
            });
            await Context.SaveChangesAsync();

            var sut = new GetExpedientesSinEstadoQueryHandler(Context);

            //ACT
            var actual = await sut.Handle(new GetExpedientesSinEstadosQuery
            {
                IdExpedienteAlumno = 1
            }, CancellationToken.None);

            //ASSERT
            Assert.NotNull(actual);
            Assert.IsType<List<ExpedienteAlumno>>(actual);
        }

        #endregion

        #region ApplyQuery

        [Fact(DisplayName = "Cuando no se envia ningun filtro Retorna los expedientes con estado en null")]
        public async Task ApplyQuery_ExpedienteEstadoNull_Ok()
        {
            //ARRANGE
            var estadoExpediente = new EstadoExpediente
            {
                 Id = 1
            };
            await Context.EstadosExpedientes.AddAsync(estadoExpediente);
            await Context.SaveChangesAsync();

            await Context.ExpedientesAlumno.AddRangeAsync(Enumerable.Range(1, 10).Select(c => new ExpedienteAlumno
            {
                Id = c,
                Estado = c % 2 == 0 ? null : estadoExpediente
            }));
            await Context.SaveChangesAsync();

            var sut = new GetExpedientesSinEstadoQueryHandler(Context);

            //ACT
            var actual = sut.ApplyQuery(Context.ExpedientesAlumno.AsQueryable(), new GetExpedientesSinEstadosQuery());

            //ASSERT
            Assert.NotNull(actual);
            Assert.IsType<List<ExpedienteAlumno>>(actual.ToList());
            Assert.Equal(5, actual.ToList().Count);
        }

        [Fact(DisplayName = "Cuando se envia id del expediente Retorna el expediente con estado en null")]
        public async Task ApplyQuery_ExpedienteByIdExpediente_Ok()
        {
            //ARRANGE
            var estadoExpediente = new EstadoExpediente
            {
                Id = 1
            };
            await Context.EstadosExpedientes.AddAsync(estadoExpediente);
            await Context.SaveChangesAsync();

            await Context.ExpedientesAlumno.AddRangeAsync(Enumerable.Range(1, 10).Select(c => new ExpedienteAlumno
            {
                Id = c,
                Estado = c % 2 == 0 ? null : estadoExpediente
            }));
            await Context.SaveChangesAsync();

            var sut = new GetExpedientesSinEstadoQueryHandler(Context);

            //ACT
            var actual = sut.ApplyQuery(Context.ExpedientesAlumno.AsQueryable(), new GetExpedientesSinEstadosQuery
            {
                IdExpedienteAlumno = 2
            });

            //ASSERT
            Assert.NotNull(actual);
            Assert.IsType<List<ExpedienteAlumno>>(actual.ToList());
            Assert.Single(actual.ToList());
        }

        [Fact(DisplayName = "Cuando se envia id ref de la universidad Retorna los expedientes que coincidan con el filtro y los expedientes con estados en null")]
        public async Task ApplyQuery_ExpedienteByIdRefUniversidad_Ok()
        {
            //ARRANGE
            var estadoExpediente = new EstadoExpediente
            {
                Id = 1
            };
            await Context.EstadosExpedientes.AddAsync(estadoExpediente);
            await Context.SaveChangesAsync();

            await Context.ExpedientesAlumno.AddRangeAsync(Enumerable.Range(1, 10).Select(c => new ExpedienteAlumno
            {
                Id = c,
                Estado = null,
                IdRefUniversidad = c % 3 == 0 ? "1" : "2"
            }));
            await Context.SaveChangesAsync();

            var sut = new GetExpedientesSinEstadoQueryHandler(Context);

            //ACT
            var actual = sut.ApplyQuery(Context.ExpedientesAlumno.AsQueryable(), new GetExpedientesSinEstadosQuery
            {
                IdRefUniversidad = "1"
            });

            //ASSERT
            Assert.NotNull(actual);
            Assert.IsType<List<ExpedienteAlumno>>(actual.ToList());
            Assert.Equal(3, actual.ToList().Count);
        }

        [Fact(DisplayName = "Cuando se envia id ref estudio Retorna los expedientes que coincidan con el filtro y los expedientes con estados en null")]
        public async Task ApplyQuery_ExpedienteByIdRefEstudio_Ok()
        {
            //ARRANGE
            var estadoExpediente = new EstadoExpediente
            {
                Id = 1
            };
            await Context.EstadosExpedientes.AddAsync(estadoExpediente);
            await Context.SaveChangesAsync();

            await Context.ExpedientesAlumno.AddRangeAsync(Enumerable.Range(1, 10).Select(c => new ExpedienteAlumno
            {
                Id = c,
                Estado = null,
                IdRefEstudio = c % 3 == 0 ? "1" : "2"
            }));
            await Context.SaveChangesAsync();

            var sut = new GetExpedientesSinEstadoQueryHandler(Context);

            //ACT
            var actual = sut.ApplyQuery(Context.ExpedientesAlumno.AsQueryable(), new GetExpedientesSinEstadosQuery
            {
                IdRefEstudio = "1"
            });

            //ASSERT
            Assert.NotNull(actual);
            Assert.IsType<List<ExpedienteAlumno>>(actual.ToList());
            Assert.Equal(3, actual.ToList().Count);
        }

        [Fact(DisplayName = "Cuando se envia fecha de apertura desde Retorna los expedientes que coincidan con el filtro y con estados en null")]
        public async Task ApplyQuery_ExpedienteByFechaAperturaDesde_Ok()
        {
            //ARRANGE
            var estadoExpediente = new EstadoExpediente
            {
                Id = 1
            };
            await Context.EstadosExpedientes.AddAsync(estadoExpediente);
            await Context.SaveChangesAsync();

            await Context.ExpedientesAlumno.AddRangeAsync(Enumerable.Range(1, 10).Select(c => new ExpedienteAlumno
            {
                Id = c,
                Estado = null,
                FechaApertura = DateTime.Now.AddDays(c)
            }));
            await Context.SaveChangesAsync();

            var sut = new GetExpedientesSinEstadoQueryHandler(Context);

            //ACT
            var actual = sut.ApplyQuery(Context.ExpedientesAlumno.AsQueryable(), new GetExpedientesSinEstadosQuery
            {
                FechaAperturaDesde = DateTime.Now.AddDays(2)
            });

            //ASSERT
            Assert.NotNull(actual);
            Assert.IsType<List<ExpedienteAlumno>>(actual.ToList());
            Assert.Equal(9, actual.ToList().Count);
        }

        [Fact(DisplayName = "Cuando se envia fecha de apertura hasta Retorna los expedientes que coincidan con el filtro y con estados en null")]
        public async Task ApplyQuery_ExpedienteByFechaAperturaHasta_Ok()
        {
            //ARRANGE
            var estadoExpediente = new EstadoExpediente
            {
                Id = 1
            };
            await Context.EstadosExpedientes.AddAsync(estadoExpediente);
            await Context.SaveChangesAsync();

            await Context.ExpedientesAlumno.AddRangeAsync(Enumerable.Range(1, 10).Select(c => new ExpedienteAlumno
            {
                Id = c,
                Estado = null,
                FechaApertura = DateTime.Now.AddDays(c)
            }));
            await Context.SaveChangesAsync();

            var sut = new GetExpedientesSinEstadoQueryHandler(Context);

            //ACT
            var actual = sut.ApplyQuery(Context.ExpedientesAlumno.AsQueryable(), new GetExpedientesSinEstadosQuery
            {
                FechaAperturaHasta = DateTime.Now.AddDays(4)
            });

            //ASSERT
            Assert.NotNull(actual);
            Assert.IsType<List<ExpedienteAlumno>>(actual.ToList());
            Assert.Equal(4, actual.ToList().Count);
        }

        [Fact(DisplayName = "Cuando se envia fechas desde y hasta Retorna los expedientes que coincidan con el filtro y con estados en null")]
        public async Task ApplyQuery_ExpedienteFiltroFechas_Ok()
        {
            //ARRANGE
            var estadoExpediente = new EstadoExpediente
            {
                Id = 1
            };
            await Context.EstadosExpedientes.AddAsync(estadoExpediente);
            await Context.SaveChangesAsync();

            await Context.ExpedientesAlumno.AddRangeAsync(Enumerable.Range(1, 10).Select(c => new ExpedienteAlumno
            {
                Id = c,
                Estado = null,
                FechaApertura = DateTime.Now.AddDays(c)
            }));
            await Context.SaveChangesAsync();

            var sut = new GetExpedientesSinEstadoQueryHandler(Context);

            //ACT
            var actual = sut.ApplyQuery(Context.ExpedientesAlumno.AsQueryable(), new GetExpedientesSinEstadosQuery
            {
                FechaAperturaDesde = DateTime.Now.AddDays(2),
                FechaAperturaHasta = DateTime.Now.AddDays(4)
            });

            //ASSERT
            Assert.NotNull(actual);
            Assert.IsType<List<ExpedienteAlumno>>(actual.ToList());
            Assert.Equal(3, actual.ToList().Count);
        }

        #endregion
    }
}
