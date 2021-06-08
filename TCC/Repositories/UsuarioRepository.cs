using Microsoft.EntityFrameworkCore.Internal;
using System.Threading.Tasks;
using TCC.Data;
using TCC.Models;
using TCC.Repositories.Interfaces;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using TCC.Libs.Hash;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace TCC.Repositories
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IConfiguration _config;

        public UsuarioRepository(ApplicationDbContext dbContext, IConfiguration config)
        {
            _dbContext = dbContext;
            _config = config;
        }

        public async Task<bool> Create(Usuario usuario)
        {
            if (!Exists(usuario.Email))
            {
                usuario.Senha = Hash.CreateHash(usuario.Email, usuario.Senha);
                try
                {
                    _dbContext.Add(usuario);
                    await _dbContext.SaveChangesAsync();
                    return true;
                }
                catch (System.Exception)
                {}
            }
            return false;
        }

        public async Task<bool> Delete(int? id)
        {
            try
            {
                Usuario usuario = await Get(id);
                // removendo os produtos vinculados ao usuario
                var produtos = _dbContext.Produtos.Where(p => p.UsuarioId == id);
                if (produtos.Count() > 0)
                {
                    foreach (var produto in produtos)
                    {
                        produto.Usuario = null;
                        produto.UsuarioId = null;
                        _dbContext.Update(produto);
                    }
                }
                _dbContext.Usuarios.Remove(usuario);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch (System.Exception)
            {
                return false;
            }
        }

        public bool Exists(string email)
        {
            return _dbContext.Usuarios.Any(e => e.Email == email);
        }

        private async Task<Usuario> Get(int? id)
        {
            return await _dbContext.Usuarios.FindAsync(id);
        }

        public Task<Usuario> Get(string email, string senha)
        {
            return _dbContext.Usuarios.Where(u => u.Email == email &&
            u.Senha == senha).FirstOrDefaultAsync();
        }

        public bool CanLogin(Usuario usuario)
        {
            // Vamos ver se acha com o email e a senha passado
            if (_dbContext.Usuarios.Any(e => e.Email == usuario.Email &&
                                        e.Senha == usuario.Senha))
            {
                return true;
            }
            else  // se não achar, vamos ver se a senha não estava com o hash
            {
                usuario.Senha = Hash.CreateHash(usuario.Email, usuario.Senha);
                // depois de atribuir o hash da senha, vamos pesquisar de novo
                return _dbContext.Usuarios.Any(e => e.Email == usuario.Email &&
                                               e.Senha == usuario.Senha);
            }            
        }

        public async Task<bool> Update(Usuario usuario)
        {
            usuario.Senha = Hash.CreateHash(usuario.Email, usuario.Senha);
            try
            {
                _dbContext.Update(usuario);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch (System.Exception)
            {
                return false;
            }
        }

        public bool IsAdm(Usuario usuario)
        {
            // pega a lista de IDS da conta dos adm, se o usuário passado estiver nessa lista, é um adm
            return _config.GetSection("SUIds").Get<List<int?>>().Contains(usuario.UsuarioId);
        }
    }
}
