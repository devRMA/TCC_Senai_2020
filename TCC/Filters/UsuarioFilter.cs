using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TCC.Libs.Login;

namespace TCC.Filters
{
    public class UsuarioFilter : ActionFilterAttribute
    {
        private readonly LoginUsuario _loginUsuario;

        public UsuarioFilter(LoginUsuario loginUsuario)
        {
            _loginUsuario = loginUsuario;
        }
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (_loginUsuario.GetUsuario() == null)
            {
                context.Result = new RedirectToRouteResult(
                new RouteValueDictionary
                {
                    { "controller", "Usuario" },
                    { "action", "Login" }
                });
            }
        }

        public override void OnActionExecuted(ActionExecutedContext context) {}
    }
}
