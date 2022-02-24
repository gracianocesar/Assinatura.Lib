using Assinador.Lib.Model;
using Xunit;
namespace Assinador.Lib.Test
{
    public class ModelCarimboRequestTest
    {
        [Fact]
        public void Criar_CarimboRequest_True()
        {
            var carimbo = new CarimboRequest();
            Assert.True(carimbo != null);
        }
    }
}
