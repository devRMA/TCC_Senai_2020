using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TCC.Libs.Login;
using TCC.Repositories.Interfaces;

namespace TCC.Filters
{
    public class AdminFilter : ActionFilterAttribute
    {
        private readonly LoginUsuario _loginUsuario;
        private readonly IUsuarioRepository _usuarioRepository;

        public AdminFilter(LoginUsuario loginUsuario, IUsuarioRepository usuarioRepository)
        {
            _loginUsuario = loginUsuario;
            _usuarioRepository = usuarioRepository;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            // Se o id da pessoa logada, não estiver na lista de IDs de administradores, retorna 404
            if (!_usuarioRepository.IsAdm(_loginUsuario.GetUsuario()))
            {
                context.Result = new RedirectToRouteResult(
                new RouteValueDictionary
                {
                    { "controller", "Home" },
                    { "action", "Error404" }
                });
            }
        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            // Do something after the action executes.
        }
    }
}
