using AutoMapper;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Unir.Expedientes.Application.ParametrosConfiguracionesExpedientes.Queries.GetFirstParametrosConfiguracionExpediente;
using Unir.Expedientes.Application.Tests.Common;
using Unir.Expedientes.Domain.Entities;
using Unir.Framework.ApplicationSuperTypes.Exceptions;
using Xunit;

namespace Unir.Expedientes.Application.Tests.ParametrosConfiguracionesExpedientes.Queries
{
    [Collection("CommonTestCollection")]
    public class GetFirstParametrosConfiguracionExpedienteQueryHandlerTest : TestBase
    {
        private readonly IMapper _mapper;

        public GetFirstParametrosConfiguracionExpedienteQueryHandlerTest(CommonTestFixture fixture)
        {
            _mapper = fixture.Mapper;
        }

        #region Handle

        [Fact(DisplayName = "Cuando se obtienen los parámetros de configuración Devuelve dto")]
        public async Task Handle_Ok()
        {
            //ARRANGE
            var parametrosEsperados = new ParametroConfiguracionExpediente
            {
                Id = 1,
                Nombre = Guid.NewGuid().ToString(),
                ParametrosConfiguracionesExpedientesFilesTypes = Enumerable.Range(1, 2).Select(i =>
                    new ParametroConfiguracionExpedienteFileType
                    {
                        Id = i,
                        IdRefFileType = i.ToString()
                    }).ToList()
            };
            await Context.ParametrosConfiguracionesExpedientes.AddAsync(parametrosEsperados);
            await Context.SaveChangesAsync(CancellationToken.None);

            var request = new GetFirstParametrosConfiguracionExpedienteQuery();
            var sut = new GetFirstParametrosConfiguracionExpedienteQueryHandler(Context, _mapper);

            //ACT
            var actual = await sut.Handle(request, CancellationToken.None);

            //ASSERT
            Assert.Equal(parametrosEsperados.Id, actual.Id);
            Assert.Equal(parametrosEsperados.Nombre, actual.Nombre);
            Assert.Equal(parametrosEsperados.ParametrosConfiguracionesExpedientesFilesTypes.Count,
                actual.ParametrosConfiguracionesExpedientesFilesTypes.Count);
            foreach (var fileType in parametrosEsperados.ParametrosConfiguracionesExpedientesFilesTypes)
            {
                var fileTypeActual =
                    actual.ParametrosConfiguracionesExpedientesFilesTypes.First(pc => pc.Id == fileType.Id);
                Assert.Equal(fileType.IdRefFileType, fileTypeActual.IdRefFileType);
            }
        }

        [Fact(DisplayName = "Cuando no se obtiene el parámetro Devuelve excepción")]
        public async Task Handle_NotFound()
        {
            //ARRANGE
            var request = new GetFirstParametrosConfiguracionExpedienteQuery();
            var sut = new GetFirstParametrosConfiguracionExpedienteQueryHandler(Context, _mapper);

            //ACT
            var exception = await Record.ExceptionAsync(async () =>
            {
                await sut.Handle(request, CancellationToken.None);
            });

            //ASSERT
            Assert.IsType<NotFoundException>(exception);
            Assert.Equal(
                new NotFoundException(nameof(ParametroConfiguracionExpediente),
                    "Condicion8_ParametrosConfiguracionExpediente").Message,
                exception.Message);
        }

        #endregion
    }
}
