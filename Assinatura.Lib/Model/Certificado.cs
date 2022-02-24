using System.Security.Cryptography.X509Certificates;

namespace Assinador.Lib.Model
{
    public class Certificado
    {
        public string SubjectName { get; set; }
        public string Oid { get; set; }
        public string Location { get; set; }
        public string Issuer { get; set; }
        public string Owner { get; set; }
        public X509Certificate2 Certificate { get; set; }
        public bool HasPrivateKey { get; set; }
    }
}
