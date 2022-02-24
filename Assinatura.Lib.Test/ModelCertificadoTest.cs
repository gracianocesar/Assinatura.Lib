using Xunit;
using Assinador.Lib.Model;
namespace Assinador.Lib.Test
{
    public class ModelCertificadoTest
    {
        [Fact]
        public void Criar_Certificado_True()
        {
            var certificado = new Certificado();
            Assert.True(certificado != null);
        }
    }
}
