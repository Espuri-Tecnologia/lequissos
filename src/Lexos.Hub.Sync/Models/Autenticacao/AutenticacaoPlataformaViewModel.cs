using System;

namespace Lexos.Hub.Sync.Models.Autenticacao
{
    public class AutenticacaoPlataformaViewModel
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public DateTime DataExpiracao { get; set; }
    }
}
