namespace Assinador.Lib.Model
{
    public class AssinaturaRequest
    {
        public string Alias { get; set; }
        public string Motivo { get; set; }
        public string Local { get; set; }
        public DocumentoRequest Documento { get; set; }
        public string Producao { get; set; }
    }
}
