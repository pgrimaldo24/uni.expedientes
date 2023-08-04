using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Moq;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Unir.Expedientes.Application.ExpedientesAlumnos.Queries.GetDatosSolicitudTitulacionSupercap;
using Unir.Expedientes.Application.Tests.Common;
using Unir.Expedientes.Domain.Entities;
using Unir.Framework.ApplicationSuperTypes.Exceptions;
using Xunit;

namespace Unir.Expedientes.Application.Tests.ExpedientesAlumnos.Queries.GetDatosSolicitudTitulacionSupercap;

[Collection("CommonTestCollection")]
public class GetDatosSolicitudTitulacionSupercapQueryHandlerTest : TestBase
{
    private readonly IMapper _mapper;
    public GetDatosSolicitudTitulacionSupercapQueryHandlerTest(CommonTestFixture fixture)
    {
        _mapper = fixture.Mapper;
    }

    #region Handle

    [Fact(DisplayName = "Cuando se encuentra el expediente Devuelve el dto con los datos necesarios para solicitud supercap")]
    public async Task Handle_Ok()
    {
        //ARRANGE
        await Context.ExpedientesAlumno.AddAsync(new ExpedienteAlumno
        {
            Id = 1,
            FechaApertura = new DateTime(2020, 1, 1),
            IdRefViaAccesoPlan = "13",
            IdRefPlan = "10",
            IdRefIntegracionAlumno = "11",
            TitulacionAcceso = new TitulacionAcceso
            {
                Id = 10,
                Titulo = Guid.NewGuid().ToString(),
                InstitucionDocente = Guid.NewGuid().ToString()
            }
        });
        await Context.SaveChangesAsync(CancellationToken.None);

        var request = new GetDatosSolicitudTitulacionSupercapQuery("10", "11");
        var mockLocalizer = new Mock<IStringLocalizer<GetDatosSolicitudTitulacionSupercapQueryHandler>>
        {
            CallBase = true
        };
        var sut = new GetDatosSolicitudTitulacionSupercapQueryHandler(Context, mockLocalizer.Object, _mapper);

        //ACT
        var actual = await sut.Handle(request, CancellationToken.None);

        //ASSERT
        var expedientePersistido = await Context.ExpedientesAlumno.FirstAsync(CancellationToken.None);
        Assert.NotNull(actual);
        Assert.Equal(actual.Id, expedientePersistido.Id);
        Assert.Equal(actual.IdRefViaAccesoPlan, expedientePersistido.IdRefViaAccesoPlan);
        Assert.Equal(actual.FechaApertura, expedientePersistido.FechaApertura);
        Assert.Equal(actual.TitulacionAcceso.Id, expedientePersistido.TitulacionAcceso.Id);
        Assert.Equal(actual.TitulacionAcceso.Titulo, expedientePersistido.TitulacionAcceso.Titulo);
        Assert.Equal(actual.TitulacionAcceso.InstitucionDocente, expedientePersistido.TitulacionAcceso.InstitucionDocente);
    }

    [Fact(DisplayName = "Cuando no se encuentra un expediente para el plan y alumno Devuelve error")]
    public async Task Handle_Excepcion()
    {
        //ARRANGE
        //ARRANGE
        await Context.ExpedientesAlumno.AddAsync(new ExpedienteAlumno
        {
            Id = 1,
            FechaApertura = new DateTime(2020, 1, 1),
            IdRefViaAccesoPlan = "13",
            IdRefPlan = "10",
            IdRefIntegracionAlumno = "11",
            TitulacionAcceso = new TitulacionAcceso
            {
                Id = 10,
                Titulo = Guid.NewGuid().ToString(),
                InstitucionDocente = Guid.NewGuid().ToString()
            }
        });
        await Context.SaveChangesAsync(CancellationToken.None);

        var request = new GetDatosSolicitudTitulacionSupercapQuery("11", "12");
        var mockLocalizer = new Mock<IStringLocalizer<GetDatosSolicitudTitulacionSupercapQueryHandler>>
        {
            CallBase = true
        };
        var mensajeEsperado = $"No se ha encontrado un Expediente con el {nameof(request.IdRefPlan)}: '{request.IdRefPlan}' y {nameof(request.IdRefIntegracionAlumno)}: '{request.IdRefIntegracionAlumno}'.";
        mockLocalizer.Setup(s => s[It.Is<string>(msj => msj == mensajeEsperado)])
            .Returns(new LocalizedString(mensajeEsperado, mensajeEsperado));
        var sut = new GetDatosSolicitudTitulacionSupercapQueryHandler(Context, mockLocalizer.Object, _mapper);

        //ACT
        var ex = await Record.ExceptionAsync(async () =>
        {
            await sut.Handle(request, CancellationToken.None);
        });

        //ASSERT
        Assert.IsType<BadRequestException>(ex);
        Assert.Contains(mensajeEsperado, ex.Message);
    }
    #endregion

}