using Assinador.Lib.Model;
using Xunit;
namespace Assinador.Lib.Test
{
    public class ModelAssinaturaTest
    {
        [Fact]
        public void Criar_Assinatura_True()
        {
            var assinador = "NOME DO ASSINADOR";
            var cpf = "40466097026";
            var dataAssinatura = "27/02/2022";
            var subjectName = "SUBJECT_NAME;NOME DO ASSINADOR;40466097026;123456";

            var assinatura = new Assinatura(assinador, cpf, dataAssinatura, subjectName);
            Assert.True(!string.IsNullOrEmpty(assinatura.Assinador));
            Assert.True(!string.IsNullOrEmpty(assinatura.CPF));
            Assert.True(!string.IsNullOrEmpty(assinatura.DataAssinatura));
            Assert.True(!string.IsNullOrEmpty(assinatura.SubjectName));
        }

        [Fact]
        public void Criar_Assinatura_False() 
        {
            var assinador = "NOME DO ASSINADOR";
            var cpf = "";
            var dataAssinatura = "27/02/2022";
            var subjectName = "";

            var assinatura = new Assinatura(assinador, cpf, dataAssinatura, subjectName);

            Assert.True(string.IsNullOrEmpty(assinatura.Assinador)
                       ||string.IsNullOrEmpty(assinatura.CPF)
                       ||string.IsNullOrEmpty(assinatura.DataAssinatura)
                       ||string.IsNullOrEmpty(assinatura.SubjectName));
        }

    }
}
