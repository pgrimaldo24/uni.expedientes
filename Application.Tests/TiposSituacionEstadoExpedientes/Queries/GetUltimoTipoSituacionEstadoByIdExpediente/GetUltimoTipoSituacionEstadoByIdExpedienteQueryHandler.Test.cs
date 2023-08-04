using AutoMapper;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Unir.Expedientes.Application.Tests.Common;
using Unir.Expedientes.Application.TiposSituacionEstadoExpedientes.Queries.GetUltimoTipoSituacionEstadoByIdExpediente;
using Unir.Expedientes.Domain.Entities;
using Xunit;

namespace Unir.Expedientes.Application.Tests.TiposSituacionEstadoExpedientes.Queries.GetUltimoTipoSituacionEstadoByIdExpediente
{
    [Collection("CommonTestCollection")]
    public class GetUltimoTipoSituacionEstadoByIdExpedienteQueryHandlerTest : TestBase
    {
        private readonly IMapper _mapper;

        public GetUltimoTipoSituacionEstadoByIdExpedienteQueryHandlerTest(
            CommonTestFixture fixture)
        {
            _mapper = fixture.Mapper;
        }

        #region Handle

        [Fact(DisplayName = "Cuando no existe tipo situación estado del expediente Devuelve Null")]
        public async Task Handle_Null()
        {
            //ARRANGE
            const int idExpedienteAlumno = 1;
            await Context.TiposSituacionEstadoExpedientes.AddRangeAsync(
                Enumerable.Range(1, 3).Select(i =>
                    new TipoSituacionEstadoExpediente
                    {
                        Id = i,
                        Descripcion = $"{i} - {Guid.NewGuid()}"
                    }));
            await Context.SaveChangesAsync();
            
            var request = new GetUltimoTipoSituacionEstadoByIdExpedienteQuery(idExpedienteAlumno);
            var sut = new GetUltimoTipoSituacionEstadoByIdExpedienteQueryHandler(Context, _mapper);

            //ACT
            var actual = await sut.Handle(request, CancellationToken.None);

            //ASSERT
            Assert.Null(actual);
        }

        [Fact(DisplayName = "Cuando existen tipos situaciones estado del expediente Devuelve el último registro")]
        public async Task Handle_Ok()
        {
            //ARRANGE
            const int idExpedienteAlumno = 1;
            const int lastId = 3;
            await Context.TiposSituacionEstadoExpedientes.AddRangeAsync(
                Enumerable.Range(1, lastId).Select(i =>
                    new TipoSituacionEstadoExpediente
                    {
                        Id = i,
                        Descripcion = $"{i} - {Guid.NewGuid()}",
                        ExpedienteAlumnoId = idExpedienteAlumno
                    }));
            await Context.SaveChangesAsync();

            var request = new GetUltimoTipoSituacionEstadoByIdExpedienteQuery(idExpedienteAlumno);
            var sut = new GetUltimoTipoSituacionEstadoByIdExpedienteQueryHandler(Context, _mapper);

            //ACT
            var actual = await sut.Handle(request, CancellationToken.None);

            //ASSERT
            Assert.NotNull(actual);
            Assert.Equal(lastId, actual.Id);
        }

        #endregion
    }
}
