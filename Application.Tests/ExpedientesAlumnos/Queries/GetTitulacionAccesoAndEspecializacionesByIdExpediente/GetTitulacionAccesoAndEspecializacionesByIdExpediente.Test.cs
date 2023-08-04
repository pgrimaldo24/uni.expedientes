using AutoMapper;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Unir.Expedientes.Application.ExpedientesAlumnos.Queries.GetTitulacionAccesoAndEspecializacionesByIdExpediente;
using Unir.Expedientes.Application.Tests.Common;
using Unir.Expedientes.Domain.Entities;
using Unir.Framework.ApplicationSuperTypes.Exceptions;
using Xunit;

namespace Unir.Expedientes.Application.Tests.ExpedientesAlumnos.Queries.GetTitulacionAccesoAndEspecializacionesByIdExpediente;

[Collection("CommonTestCollection")]
public class GetTitulacionAccesoAndEspecializacionesByIdExpedienteTest : TestBase
{
    private readonly IMapper _mapper;

    public GetTitulacionAccesoAndEspecializacionesByIdExpedienteTest(CommonTestFixture fixture)
    {
        _mapper = fixture.Mapper;
    }

    #region Handle

    [Fact(DisplayName = "Cuando se obtiene el expediente Devuelve dto con titulación de acceso y especializaciones")]
    public async Task Handle_Ok()
    {
        //ARRANGE
        var titulacionAcceso = new TitulacionAcceso
        {
            Id = 10,
            Titulo = Guid.NewGuid().ToString(),
            InstitucionDocente = Guid.NewGuid().ToString(),
            NroSemestreRealizados = 10
        };
        await Context.TitulacionesAccesos.AddAsync(titulacionAcceso, CancellationToken.None);
        await Context.SaveChangesAsync(CancellationToken.None);

        var expedienteAlumno = new ExpedienteAlumno
        {
            Id = 1,
            TitulacionAcceso = titulacionAcceso
        };
        await Context.ExpedientesAlumno.AddAsync(expedienteAlumno, CancellationToken.None);
        await Context.SaveChangesAsync(CancellationToken.None);

        var expedientesEspecializaciones = Enumerable.Range(1, 5)
            .Select(i => new ExpedienteEspecializacion
            {
                Id = i,
                IdRefEspecializacion = i == 5 ? "Id inválido" : i.ToString(),
                ExpedienteAlumno = expedienteAlumno

            }).ToList();
        await Context.ExpedientesEspecializaciones.AddRangeAsync(expedientesEspecializaciones, CancellationToken.None);
        await Context.SaveChangesAsync(CancellationToken.None);

        var sut = new GetTitulacionAccesoAndEspecializacionesByIdExpedienteQueryHandler(Context, _mapper);

        //ACT
        var actual = await sut.Handle(new GetTitulacionAccesoAndEspecializacionesByIdExpedienteQuery(1),
            CancellationToken.None);

        //ASSERT
        Assert.NotNull(actual);
        Assert.NotNull(actual.TitulacionAcceso);
        Assert.Equal(actual.TitulacionAcceso.Titulo, titulacionAcceso.Titulo);
        Assert.Equal(actual.TitulacionAcceso.InstitucionDocente, titulacionAcceso.InstitucionDocente);
        Assert.Equal(actual.TitulacionAcceso.NroSemestreRealizados, titulacionAcceso.NroSemestreRealizados);
        Assert.NotNull(actual.ExpedientesEspecializaciones);
        Assert.NotEmpty(actual.ExpedientesEspecializaciones);
        var idsEspecializacionesValidos = expedientesEspecializaciones
            .Where(ee => int.TryParse(ee.IdRefEspecializacion, out _)).ToArray();
        Assert.Equal(expedientesEspecializaciones.Count, actual.ExpedientesEspecializaciones.Count);
        Assert.Equal(idsEspecializacionesValidos.Length, actual.IdsEspecializacionesExpediente.Count);
    }

    [Fact(DisplayName = "Cuando se obtiene el expediente Devuelve dto con titulación de acceso y sin especializaciones")]
    public async Task Handle_SinEspecializaciones_Ok()
    {
        //ARRANGE
        var titulacionAcceso = new TitulacionAcceso
        {
            Id = 10,
            Titulo = Guid.NewGuid().ToString(),
            InstitucionDocente = Guid.NewGuid().ToString(),
            NroSemestreRealizados = 10
        };
        await Context.TitulacionesAccesos.AddAsync(titulacionAcceso, CancellationToken.None);
        await Context.SaveChangesAsync(CancellationToken.None);

        var expedienteAlumno = new ExpedienteAlumno
        {
            Id = 1,
            TitulacionAcceso = titulacionAcceso
        };
        await Context.ExpedientesAlumno.AddAsync(expedienteAlumno, CancellationToken.None);
        await Context.SaveChangesAsync(CancellationToken.None);

        var sut = new GetTitulacionAccesoAndEspecializacionesByIdExpedienteQueryHandler(Context, _mapper);

        //ACT
        var actual = await sut.Handle(new GetTitulacionAccesoAndEspecializacionesByIdExpedienteQuery(1),
            CancellationToken.None);

        //ASSERT
        Assert.NotNull(actual);
        Assert.NotNull(actual.TitulacionAcceso);
        Assert.Equal(actual.TitulacionAcceso.Titulo, titulacionAcceso.Titulo);
        Assert.Equal(actual.TitulacionAcceso.InstitucionDocente, titulacionAcceso.InstitucionDocente);
        Assert.Equal(actual.TitulacionAcceso.NroSemestreRealizados, titulacionAcceso.NroSemestreRealizados);
        Assert.NotNull(actual.ExpedientesEspecializaciones);
        Assert.Empty(actual.ExpedientesEspecializaciones);
    }

    [Fact(DisplayName = "Cuando se obtiene el expediente Devuelve dto con especializaciones y sin titulación de acceso")]
    public async Task Handle_SinTitulacionAcceso_Ok()
    {
        //ARRANGE
        var expedienteAlumno = new ExpedienteAlumno
        {
            Id = 1
        };
        await Context.ExpedientesAlumno.AddAsync(expedienteAlumno, CancellationToken.None);
        await Context.SaveChangesAsync(CancellationToken.None);

        var expedientesEspecializaciones = Enumerable.Range(1, 5)
            .Select(i => new ExpedienteEspecializacion
            {
                Id = i,
                IdRefEspecializacion = i == 5 ? "Id inválido" : i.ToString(),
                ExpedienteAlumno = expedienteAlumno

            }).ToList();
        await Context.ExpedientesEspecializaciones.AddRangeAsync(expedientesEspecializaciones, CancellationToken.None);
        await Context.SaveChangesAsync(CancellationToken.None);

        var sut = new GetTitulacionAccesoAndEspecializacionesByIdExpedienteQueryHandler(Context, _mapper);

        //ACT
        var actual = await sut.Handle(new GetTitulacionAccesoAndEspecializacionesByIdExpedienteQuery(1),
            CancellationToken.None);

        //ASSERT
        Assert.NotNull(actual);
        Assert.Null(actual.TitulacionAcceso);
        Assert.NotNull(actual.ExpedientesEspecializaciones);
        Assert.NotEmpty(actual.ExpedientesEspecializaciones);
        var idsEspecializacionesValidos = expedientesEspecializaciones
            .Where(ee => int.TryParse(ee.IdRefEspecializacion, out _)).ToArray();
        Assert.Equal(expedientesEspecializaciones.Count, actual.ExpedientesEspecializaciones.Count);
        Assert.Equal(idsEspecializacionesValidos.Length, actual.IdsEspecializacionesExpediente.Count);
    }

    [Fact(DisplayName = "Cuando no se obtiene el expediente Devuelve excepción")]
    public async Task Handle_NotFound()
    {
        //ARRANGE
        var expedienteEsperado = new ExpedienteAlumno
        {
            Id = 1,
            IdRefIntegracionAlumno = "123456",
            IdRefPlan = "789456"
        };
        await Context.ExpedientesAlumno.AddAsync(expedienteEsperado);
        await Context.SaveChangesAsync(CancellationToken.None);

        var sut = new GetTitulacionAccesoAndEspecializacionesByIdExpedienteQueryHandler(Context, _mapper);

        //ACT
        var exception = await Record.ExceptionAsync(async () =>
        {
            await sut.Handle(new GetTitulacionAccesoAndEspecializacionesByIdExpedienteQuery(2), CancellationToken.None);
        });

        //ASSERT
        Assert.IsType<NotFoundException>(exception);
        Assert.Equal(new NotFoundException(nameof(ExpedienteAlumno), 2).Message,
            exception.Message);
    }

    #endregion
}