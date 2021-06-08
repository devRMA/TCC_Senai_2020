using Newtonsoft.Json;
using TCC.Models;

namespace TCC.Libs.Login
{
    public class LoginUsuario
    {
        private readonly string key = "Login.Usuario";
        private readonly Sessao.Sessao _sessao;

        public LoginUsuario(Sessao.Sessao sessao)
        {
            _sessao = sessao;
        }

        public void ArmazenarSessao(Usuario usuario)
        {
            if (!_sessao.Exists(key))
            {
                string usuarioJSONStr = JsonConvert.SerializeObject(usuario);
                _sessao.Create(key, usuarioJSONStr);
            }
        }

        public void AtualizarSessao(Usuario usuario)
        {
            if (_sessao.Exists(key))
            {
                string usuarioJSONStr = JsonConvert.SerializeObject(usuario);
                _sessao.Update(key, usuarioJSONStr);
            }
        }

        public void RemoverSessao()
        {
            if (_sessao.Exists(key))
            {
                _sessao.Delete(key);
            }
        }

        public Usuario GetUsuario()
        {
            if (_sessao.Exists(key))
            {
                string usuarioJSONStr = _sessao.Get(key);
                return JsonConvert.DeserializeObject<Usuario>(usuarioJSONStr);
            }
            return null;
        }

    }
}
