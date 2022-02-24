namespace Assinador.Lib.Model
{
    public class DocumentoResponse
    {  
        public sbyte[] Imagem { get; set; }
        public sbyte[] ImagemValidacao { get; set; }
        public string DtInclusao { get; set; }
        public string DscNomeArquivoImagem { get; set; }
        public string DscTipoArquivoImagem { get; set; }
    }
}
