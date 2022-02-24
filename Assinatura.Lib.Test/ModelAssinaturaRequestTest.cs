using Assinador.Lib.Model;
using Xunit;

namespace Assinador.Lib.Test
{
    public class ModelAssinaturaRequestTest
    {
        [Fact]
        public void Criar_AssinaturaRequest_True()
        {
            var assinaturaRequest = new AssinaturaRequest();
            Assert.True(assinaturaRequest != null);
        }
    }
}
