using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TCC.Libs.Sessao
{
    public class Sessao
    {
        IHttpContextAccessor _context;

        public Sessao(IHttpContextAccessor context)
        {
            _context = context;
        }

        public void Create(string key, string value)
        {
            _context.HttpContext.Session.SetString(key, value);
        }

        public void Delete(string key)
        {
            _context.HttpContext.Session.Remove(key);
        }

        public bool Exists(string key)
        {
            if (Get(key) == null)
            {
                return false;
            }
            return true;
        }

        public string Get(string key)
        {
            return _context.HttpContext.Session.GetString(key);
        }

        public void Update(string key, string value)
        {
            if (Exists(key))
            {
                _context.HttpContext.Session.Remove(key);
            }
            _context.HttpContext.Session.SetString(key, value);
        }
    }
}
