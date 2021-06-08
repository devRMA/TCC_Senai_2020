using System.Threading.Tasks;
using GoogleReCaptcha.V3.Interface;
using Microsoft.AspNetCore.Mvc;
using TCC.Filters;
using TCC.Libs.EmailContato;
using TCC.Libs.Hash;
using TCC.Libs.Login;
using TCC.Models;
using TCC.Repositories.Interfaces;

namespace TCC.Controllers
{
    public class UsuarioController : Controller
    {
        private readonly IProdutoRepository _produtoRepository;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly LoginUsuario _loginUsuario;
        private readonly ICaptchaValidator _captchaValidator;
        private readonly ContatoEmail _contatoEmail;

        public UsuarioController(IUsuarioRepository usuarioRepository, LoginUsuario loginUsuario,
            ICaptchaValidator captchaValidator, ContatoEmail contatoEmail, IProdutoRepository produtoRepository)
        {
            _produtoRepository = produtoRepository;
            _usuarioRepository = usuarioRepository;
            _loginUsuario = loginUsuario;
            _captchaValidator = captchaValidator;
            _contatoEmail = contatoEmail;
        }

        // GET: Usuario/Login
        [HttpGet]
        public IActionResult Login()
        {
            // Se o usuário já estiver logado
            if (_loginUsuario.GetUsuario() != null)
            {
                return RedirectToAction(nameof(Index));
            }
            return View();
        }

        // POST: Usuario/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login([FromForm] Usuario usuario, [FromForm] string captcha)
        {
            ViewData["MSG_E"] = "Email ou senha inválidos!";
            if (!await _captchaValidator.IsCaptchaPassedAsync(captcha))
            {
                ViewData["MSG_E"] = "Verificação do Captcha falhou!";
            }
            if ((usuario.Senha == null) || (usuario.Email == null))
            {
                ViewData["MSG_E"] = "O campo do email e da senha, precisam ser preenchidos!";
                return View();
            }
            if (_loginUsuario.GetUsuario() != null)
            {
                return RedirectToAction("Index", "Produto");
            }
            if (_usuarioRepository.CanLogin(usuario))
            {
                usuario = await _usuarioRepository.Get(usuario.Email, usuario.Senha);
                _loginUsuario.ArmazenarSessao(usuario);
                return RedirectToAction("Index", "Produto");
            }
            return View();
        }

        // POST: Usuario/Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Logout()
        {
            _loginUsuario.RemoverSessao();
            return RedirectToAction("Index", "Home");
        }

        // GET: Usuario
        [HttpGet]
        [ServiceFilter(typeof(UsuarioFilter))]
        public IActionResult Index()
        {
            return View(_loginUsuario.GetUsuario());
        }

        // GET: Usuario/Signup
        [HttpGet]
        public IActionResult Signup()
        {
            return View();
        }

        // POST: Usuario/Signup
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Signup([FromForm] Usuario usuario, [FromForm] string captcha)
        {
            ViewData["MSG_E"] = "Este email já esta cadastrado! Você não quer fazer <a href=\"/Usuario/Login\">Login</a>?";
            if (!await _captchaValidator.IsCaptchaPassedAsync(captcha))
            {
                ViewData["MSG_E"] = "Verificação do Captcha falhou!";
            }
            else
            {
                if (await _usuarioRepository.Create(usuario))
                {
                    MensagemEmail mensagem = new MensagemEmail()
                    {
                        Destinatario = usuario.Email,
                        Titulo = "Confirmação de e-mail",
                        Conteudo = $"Olá {usuario.Nome} seja bem-vindo ao nosso TCC!",
                        Html = false
                    };
                    if (_contatoEmail.EnviarEmail(mensagem))
                    {
                        return View("SignupSuccess");
                    }
                    else
                    {
                        await _usuarioRepository.Delete(usuario.UsuarioId);
                        ViewData["MSG_E"] = "Ocorreu um erro interno, por favor, tente novamente!";
                        return View(usuario);
                    }
                }
            }
            return View(usuario);
        }

        // GET: Usuario/ChangeEmail
        [HttpGet]
        [ServiceFilter(typeof(UsuarioFilter))]
        public IActionResult ChangeEmail()
        {
            Usuario user = _loginUsuario.GetUsuario();
            return View(user);
        }

        // POST: Usuario/ChangeEmail
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ServiceFilter(typeof(UsuarioFilter))]
        public async Task<IActionResult> ChangeEmail([FromForm] Usuario usuario, [FromForm] string captcha)
        {
            ViewData["MSG_E"] = "Senha inválida!";

            if (!await _captchaValidator.IsCaptchaPassedAsync(captcha))
            {
                ViewData["MSG_E"] = "Verificação do Captcha falhou!";
                return View(usuario);
            }

            Usuario user_session = _loginUsuario.GetUsuario();

            if (user_session.UsuarioId != usuario.UsuarioId)
            {
                return NotFound();
            }

            var senha_hash_antigo = Hash.CreateHash(user_session.Email, usuario.Senha);

            Usuario usuario_antigo = new Usuario()
            {
                Email = user_session.Email,
                Senha = usuario.Senha
            };
            if ((!_usuarioRepository.CanLogin(usuario_antigo)) || (user_session.Senha != senha_hash_antigo))
            {
                return View(usuario);
            }
            if (ModelState.IsValid)
            {
                if (await _usuarioRepository.Update(usuario))
                {
                    usuario.Senha = Hash.CreateHash(usuario.Email, usuario.Senha);
                    _loginUsuario.AtualizarSessao(usuario);
                    return RedirectToAction(nameof(Index));
                }
            }
            return View(usuario);
        }

        // GET: Usuario/Edit
        [HttpGet]
        [ServiceFilter(typeof(UsuarioFilter))]
        public IActionResult ChangePassword()
        {
            Usuario user = _loginUsuario.GetUsuario();
            if (!_usuarioRepository.Exists(user.Email))
            {
                return NotFound();
            }
            return View(user);
        }

        // POST: Usuario/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ServiceFilter(typeof(UsuarioFilter))]
        public async Task<IActionResult> ChangePassword([FromForm] Usuario usuario, [FromForm] string senhaNova, [FromForm] string captcha)
        {
            ViewData["MSG_E"] = "Senha inválida!";

            if (!await _captchaValidator.IsCaptchaPassedAsync(captcha))
            {
                ViewData["MSG_E"] = "Verificação do Captcha falhou!";
                return View(usuario);
            }
            Usuario user_session = _loginUsuario.GetUsuario();

            if (user_session.UsuarioId != usuario.UsuarioId)
            {
                return NotFound();
            }

            var hash_senha = Hash.CreateHash(usuario.Email, usuario.Senha);

            usuario.Senha = hash_senha;

            if ((!_usuarioRepository.CanLogin(usuario)) || (user_session.Senha != hash_senha))
            {
                return View(usuario);
            }
            if (ModelState.IsValid)
            {
                usuario.Senha = senhaNova;
                if (await _usuarioRepository.Update(usuario))
                {
                    usuario.Senha = Hash.CreateHash(usuario.Email, senhaNova);
                    _loginUsuario.AtualizarSessao(usuario);
                    return RedirectToAction(nameof(Index));
                }
            }
            return View(usuario);
        }

        // GET: Usuario/Delete
        [HttpGet]
        [ServiceFilter(typeof(UsuarioFilter))]
        public IActionResult Delete()
        {
            return View(_loginUsuario.GetUsuario());
        }

        // POST: Usuario/Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ServiceFilter(typeof(UsuarioFilter))]
        public async Task<IActionResult> Delete([FromForm] Usuario userForm, [FromForm] string captcha)
        {
            ViewData["MSG_E"] = "Senha inválida!";

            if (!await _captchaValidator.IsCaptchaPassedAsync(captcha))
            {
                ViewData["ALERT_DANGER"] = "Verificação do Captcha falhou!";
                return View(userForm);
            }

            Usuario userSession = _loginUsuario.GetUsuario();
            if (userForm.UsuarioId != userSession.UsuarioId)
            {
                return NotFound();
            }

            var senhaHashDigitado = Hash.CreateHash(userSession.Email, userForm.Senha);
            if ((!_usuarioRepository.CanLogin(userForm)) || (userSession.Senha != senhaHashDigitado))
            {
                return View(userForm);
            }
            if (ModelState.IsValid)
            {
                if (await _usuarioRepository.Delete(userSession.UsuarioId))
                {
                    _loginUsuario.RemoverSessao();
                    return RedirectToAction(nameof(Index));
                }
            }
            return View(userForm);
        }
    }
}
