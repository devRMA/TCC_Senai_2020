using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TCC.Models
{
    public class MensagemEmail
    {
        public string Destinatario { get; set; }
        public string Titulo { get; set; }
        public string Conteudo { get; set; }
        public bool Html { get; set; }
    }
}
