using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Moq;
using Unir.Expedientes.Application.Anotaciones.Queries.GetAnotacionById;
using Unir.Expedientes.Application.RequisitosExpedientesDocumentos.Queries.GetAllRequisitosExpedientesDocumentosByIdRequisito;
using Unir.Expedientes.Application.Tests.Common;
using Unir.Expedientes.Domain.Entities;
using Unir.Framework.ApplicationSuperTypes.Exceptions;
using Unir.Framework.Crosscutting.Security;
using Unir.Framework.Crosscutting.Security.Model;
using Xunit;

namespace Unir.Expedientes.Application.Tests.RequisitosExpedientesDocumentos.Queries.GetAllRequisitosExpedientesDocumentosByIdRequisito
{
    [Collection("CommonTestCollection")]
    public class GetAnotacionByIdQueryHandlerTests : TestBase
    {
        private readonly IMapper _mapper;

        public GetAnotacionByIdQueryHandlerTests(CommonTestFixture fixture)
        {
            _mapper = fixture.Mapper;
        }

        #region Handle

        [Fact(DisplayName = "Cuando no tiene registros devuelve una lista vacía")]
        public async Task Handle_NoExistesElementos()
        {
            //ARRANGE
            var cantidadEsperada = 0;
            var sut = new GetAllRequisitosExpedientesDocumentosByIdRequisitoQueryHandler(Context, _mapper);

            //ACT
            var actual = await sut.Handle(new GetAllRequisitosExpedientesDocumentosByIdRequisitoQuery(1),
                CancellationToken.None);

            //ASSERT
            Assert.Equal(cantidadEsperada, actual.Count);
        }

        [Fact(DisplayName = "Cuando existe por lo menos un registro devuelve una lista con datos")]
        public async Task Handle_ExisteElementos()
        {
            //ARRANGE
            var cantidadEsperada = 1;
            var requisitoExpediente = new RequisitoExpediente
            {
                Id = 1
            };
            await Context.RequisitosExpedientes.AddAsync(requisitoExpediente);
            await Context.SaveChangesAsync();

            await Context.RequisitosExpedientesDocumentos.AddAsync(new RequisitoExpedienteDocumento
            {
                Id = 1,
                RequisitoExpediente = requisitoExpediente
            });
            await Context.SaveChangesAsync();

            var sut = new Mock<GetAllRequisitosExpedientesDocumentosByIdRequisitoQueryHandler>(Context, _mapper);

            //ACT
            var actual = await sut.Object.Handle(new GetAllRequisitosExpedientesDocumentosByIdRequisitoQuery(1),
                CancellationToken.None);

            //ASSERT
            Assert.NotNull(actual);
            Assert.Equal(cantidadEsperada, actual.Count);
        }

        #endregion
    }
}
