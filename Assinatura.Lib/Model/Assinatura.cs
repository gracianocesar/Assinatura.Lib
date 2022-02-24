namespace Assinador.Lib.Model
{
    public class Assinatura
    {
        public string Assinador { get; set; }
        public string CPF { get; set; }
        public string DataAssinatura { get; set; }
        public string SubjectName { get; set; }

        public Assinatura(string assinador, string cpf, string dataAssinatura, string subjectName)
        {
            Assinador = assinador;
            CPF = cpf;
            DataAssinatura = dataAssinatura;
            SubjectName = subjectName;
        }
    }
}
