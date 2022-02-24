using Assinador.Lib.Model;
using Xunit;

namespace Assinador.Lib.Test
{
    public class ModelDocumentoRequestTest
    {
        [Fact]
        public void Criar_DocumentoRequest_True()
        {
            var documentoRequest = new DocumentoRequest();
            Assert.True(documentoRequest != null);
        }
    }
}
