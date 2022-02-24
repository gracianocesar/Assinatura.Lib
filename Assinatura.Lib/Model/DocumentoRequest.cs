using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assinador.Lib.Model
{
    public class DocumentoRequest
    {
        public CarimboRequest Carimbo { get; set; }
        public string Challenge { get; set; }
        public string DisponibilizarNaUrl { get; set; }
        public string ObterNaUrl { get; set; }
        public string DscTipoArquivoImagem { get; set; }
        public string DscNomeArquivoImagem { get; set; }
        public string EnderecoArquivoLocal { get; set; }
    }
}
