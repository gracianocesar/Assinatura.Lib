using Assinador.Lib.Model;
using Xunit;

namespace Assinador.Lib.Test
{
    public class ModelDocumentoResponseTest
    {
        [Fact]
        public void Criar_DocumentoResponse_True()
        {
            var documentoResponse = new DocumentoResponse();
            Assert.True(documentoResponse != null);
        }
    }
}
