using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GoogleReCaptcha.V3.Interface;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using TCC.Filters;
using TCC.Libs.EmailContato;
using TCC.Libs.Login;
using TCC.Models;
using TCC.Repositories.Interfaces;

namespace TCC.Controllers
{
    public class ProdutoController : Controller
    {
        private readonly IProdutoRepository _produtoRepository;
        private readonly LoginUsuario _loginUsuario;
        private readonly ICaptchaValidator _captchaValidator;
        private readonly ContatoEmail _contatoEmail;

        public ProdutoController(IProdutoRepository produtoRepository, LoginUsuario loginUsuario,
            ICaptchaValidator captchaValidator, ContatoEmail contatoEmail)
        {
            _produtoRepository = produtoRepository;
            _loginUsuario = loginUsuario;
            _captchaValidator = captchaValidator;
            _contatoEmail = contatoEmail;
        }

        // GET: Produto
        [HttpGet]
        [ServiceFilter(typeof(UsuarioFilter))]
        public async Task<IActionResult> Index()
        {
            return View(await _produtoRepository.GetAll(_loginUsuario.GetUsuario()));
        }

        // GET: Produto/LinkProduct
        [HttpGet]
        [ServiceFilter(typeof(UsuarioFilter))]
        public IActionResult LinkProduct()
        {
            return View();
        }

        // POST: Produto/LinkProduct/TOKEN ID
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ServiceFilter(typeof(UsuarioFilter))]
        public async Task<IActionResult> LinkProduct([FromForm] string key, [FromForm] string captcha)
        {
            if (!await _captchaValidator.IsCaptchaPassedAsync(captcha))
            {
                ViewData["MSG_E"] = "Verificação do Captcha falhou!";
                return View();
            }
            if ((key == null) || (key == "") || (key.Length <= 20))
            {
                ViewData["MSG_E"] = "Chave inválida!";
                return View();
            }
            string token = key.Substring(0, 20);
            int? id = Convert.ToInt32(key.Substring(20));
            if (_produtoRepository.Exists(token) && _produtoRepository.Exists(id))
            {
                Produto produto = await _produtoRepository.Get(id);
                if ((produto.Token == token) && (produto.ProdutoId == id))
                {
                    produto.UsuarioId = _loginUsuario.GetUsuario().UsuarioId;
                    if (await _produtoRepository.Update(produto))
                    {
                        ViewData["MSG_S"] = "Produto vinculado com sucesso!";
                        return RedirectToAction("Edit", new { id = id });
                    }
                }
            }
            ViewData["MSG_E"] = "Chave inválida!";
            return View();
        }


        // GET: Produto/Details/5
        [HttpGet]
        [ServiceFilter(typeof(UsuarioFilter))]
        public async Task<IActionResult> Details(int? id)
        {
            List<Produto> produtos = await _produtoRepository.GetAll(_loginUsuario.GetUsuario());
            if (produtos.Any(prod => prod.ProdutoId == id))
            {
                return View(produtos.Find(prod => prod.ProdutoId == id));
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: Produto/APIGetTemp/?id=5
        [HttpGet]
        [ServiceFilter(typeof(UsuarioFilter))]
        public async Task<ContentResult> APIGetTemp([FromQuery] int? id)
        {
            List<Produto> produtos = await _produtoRepository.GetAll(_loginUsuario.GetUsuario());
            if (produtos.Any(prod => prod.ProdutoId == id))
            {
                List<DataPoint> dataPoints = new List<DataPoint>();

                dataPoints.Add(new DataPoint(DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(), Convert.ToDouble(produtos.Find(prod => prod.ProdutoId == id).Temperatura)));

                JsonSerializerSettings _jsonSetting = new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore };
                return Content(JsonConvert.SerializeObject(dataPoints, _jsonSetting), "application/json");
            }
            return Content("404");
        }

        // GET: Produto/Create
        [HttpGet]
        [ServiceFilter(typeof(UsuarioFilter))]
        [ServiceFilter(typeof(AdminFilter))]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Produto/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ServiceFilter(typeof(UsuarioFilter))]
        [ServiceFilter(typeof(AdminFilter))]
        public async Task<IActionResult> Create([FromForm] string token)
        {
            if (ModelState.IsValid)
            {
                Produto produto = new Produto()
                {
                    Token = token,
                    Temperatura = 0,
                    UsuarioId = null,
                    Usuario = null,
                    Limite_max = 0,
                    Limite_min = 0
                };
                if (!_produtoRepository.Exists(token))
                {
                    if (await _produtoRepository.Create(produto))
                    {
                        ViewData["MSG_S"] = "Produto cadastrado com sucesso!";
                        return View();
                    }
                }
                else
                {
                    ViewData["MSG_E"] = "Já existe um produto com esse token!";
                    return View();
                }
            }
            ViewData["MSG_E"] = "Erro cadastrar produto!";
            return View();
        }

        // GET: Produto/Edit/5
        [HttpGet]
        [ServiceFilter(typeof(UsuarioFilter))]
        public async Task<IActionResult> Edit(int? id)
        {
            List<Produto> produtos = await _produtoRepository.GetAll(_loginUsuario.GetUsuario());

            if ((id != null) && produtos.Any(prod => prod.ProdutoId == id) && _produtoRepository.Exists(id))
            {
                return View(produtos.Find(prod => prod.ProdutoId == id));
            }
            return NotFound();
        }

        // POST: Produto/Edit/
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ServiceFilter(typeof(UsuarioFilter))]
        public async Task<IActionResult> Edit(int? id, [FromForm] float? limite_max, [FromForm] float? limite_min, [FromForm] string captcha)
        {
            if (!await _captchaValidator.IsCaptchaPassedAsync(captcha))
            {
                ViewData["MSG_E"] = "Verificação do Captcha falhou!";
                return View();
            }
            List<Produto> produtos = await _produtoRepository.GetAll(_loginUsuario.GetUsuario());

            if ((!produtos.Any(prod => prod.ProdutoId == id)))
            {
                return NotFound();
            }
            if (ModelState.IsValid && (limite_max != null) && (limite_min != null))
            {
                Produto produto = await _produtoRepository.Get(id);
                produto.Limite_min = limite_min;
                produto.Limite_max = limite_max;
                if (await _produtoRepository.Update(produto))
                {
                    return RedirectToAction(nameof(Details), new { id });
                }
            }
            return NotFound();
        }

        // GET: Produto/Delete/5
        [HttpGet]
        [ServiceFilter(typeof(UsuarioFilter))]
        public async Task<IActionResult> Delete(int? id)
        {
            List<Produto> produtos = await _produtoRepository.GetAll(_loginUsuario.GetUsuario());

            if (produtos.Any(prod => prod.ProdutoId == id))
            {
                return View(await _produtoRepository.Get(id));
            }
            return NotFound();
        }

        // POST: Produto/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ServiceFilter(typeof(UsuarioFilter))]
        public async Task<IActionResult> Delete(int id, [FromForm] string captcha)
        {
            if (!await _captchaValidator.IsCaptchaPassedAsync(captcha))
            {
                ViewData["MSG_E"] = "Verificação do Captcha falhou, tente novamente!";
                return View();
            }
            List<Produto> produtos = await _produtoRepository.GetAll(_loginUsuario.GetUsuario());

            if (produtos.Any(prod => prod.ProdutoId == id))
            {
                Produto produto = await _produtoRepository.Get(id);
                produto.Usuario = null;
                produto.UsuarioId = null;
                if (await _produtoRepository.Update(produto))
                {
                    return RedirectToAction(nameof(Index));
                }
            }
            return NotFound();
        }

        // POST: Produto/TempUpdate
        // END POINT que vai ser usado pelo arduino
        [HttpPost]
        public async Task<IActionResult> TempUpdate([FromForm] string key, [FromForm] float? value)
        {
            ViewData["RESULT"] = 404;
            if ((key.Length > 20) && (value != null))
            {
                string token = key.Substring(0, 20);
                int? id = Convert.ToInt32(key.Substring(20));
                if (_produtoRepository.Exists(token) && _produtoRepository.Exists(id))
                {
                    Produto produto = await _produtoRepository.Get(id);
                    if ((produto.Token == token) && (produto.ProdutoId == id))
                    {
                        if ((value >= produto.Limite_max) || (value <= produto.Limite_min))
                        {
                            MensagemEmail mensagem = new MensagemEmail()
                            {
                                Destinatario = produto.Usuario.Email,
                                Titulo = $"Atenção {produto.Usuario.Nome} a temperatura do ambiente passou do limite!",
                                Conteudo = $"Atenção {produto.Usuario.Nome}, a temperatura atual do ambiente está em {value}°C e os limites eram {produto.Limite_min}°C e {produto.Limite_max}°C!",
                                Html = false
                            };
                            // vai ficar tentando enviar o e-mail
                            while (true)
                            {
                                if (_contatoEmail.EnviarEmail(mensagem))
                                {
                                    // se conseguir, para o loop
                                    break;
                                }
                                // se não conseguir, continua tentando 
                            }
                        }
                        produto.Temperatura = value;
                        if (await _produtoRepository.Update(produto))
                        {
                            ViewData["RESULT"] = 202;
                            return View();
                        }
                        else
                        {
                            ViewData["RESULT"] = 500;
                            return View();
                        }
                    }
                }
            }
            return View();
        }
    }
}
