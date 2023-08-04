using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Unir.Expedientes.Application.Anotaciones.Commands.DeleteAnotacion;
using Unir.Expedientes.Application.Tests.Common;
using Unir.Expedientes.Domain.Entities;
using Unir.Framework.ApplicationSuperTypes.Exceptions;
using Xunit;

namespace Unir.Expedientes.Application.Tests.Anotaciones.Commands.DeleteAnotacion
{
    [Collection("CommonTestCollection")]
    public class DeleteAnotacionCommandHandlerTests : TestBase
    {
        #region Handle

        [Fact(DisplayName = "Cuando la anotación no existe Devuelve una excepción")]
        public async Task Handle_AnotacionInexistente()
        {
            //ARRANGE
            var request = new DeleteAnotacionCommand(1);
            var sut = new Mock<DeleteAnotacionCommandHandler>(Context)
            {
                CallBase = true
            };

            //ACT
            var ex = await Record.ExceptionAsync(async () =>
            {
                await sut.Object.Handle(request, CancellationToken.None);
            });

            //ASSERT
            Assert.IsType<NotFoundException>(ex);
            Assert.Contains("not found", ex.Message);
        }

        [Fact(DisplayName = "Cuando elimina una anotación Devuelve Ok")]
        public async Task Handle_EliminaAnotacion_Ok()
        {
            //ARRANGE
            var anotacion = new Anotacion()
            {
                Id = 1
            };
            await Context.Anotaciones.AddAsync(anotacion);
            await Context.SaveChangesAsync();

            var request = new DeleteAnotacionCommand(1);
            var sut = new Mock<DeleteAnotacionCommandHandler>(Context)
            {
                CallBase = true
            };

            //ACT
            await sut.Object.Handle(request, CancellationToken.None);

            //ASSERT
            Assert.Null(Context.Anotaciones.FirstOrDefault());
        }

        #endregion
    }
}
