using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TCC.Data;
using TCC.Models;
using TCC.Repositories.Interfaces;

namespace TCC.Repositories
{
    public class ProdutoRepository : IProdutoRepository
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IUsuarioRepository _usuarioRepository;

        public ProdutoRepository(ApplicationDbContext dbContext, IUsuarioRepository usuarioRepository)
        {
            _dbContext = dbContext;
            _usuarioRepository = usuarioRepository;
        }

        public async Task<bool> Create(Produto produto)
        {
            try
            {
                produto.Limite_max = 256;
                produto.Limite_min = -256;
                _dbContext.Add(produto);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch (System.Exception)
            {
                return false;
            }
        }

        public async Task<bool> Delete(int? id)
        {
            try
            {
                Produto produto = await Get(id);
                _dbContext.Produtos.Remove(produto);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch (System.Exception)
            {
                return false;
            }
        }

        public bool Exists(int? id)
        {
            return _dbContext.Produtos.Any(e => e.ProdutoId == id);
        }

        public bool Exists(string token)
        {
            return _dbContext.Produtos.Any(e => e.Token == token);
        }

        public async Task<Produto> Get(int? id)
        {
            var produtos = _dbContext.Produtos.Include(p => p.Usuario);
            return await produtos.Where(p => p.ProdutoId == id).FirstAsync();
        }

        public Task<List<Produto>> GetAll(Usuario usuario)
        {
            var produtos = _dbContext.Produtos.Include(p => p.Usuario);
            if (_usuarioRepository.IsAdm(usuario))
            {
                return produtos.OrderBy(p => p.ProdutoId).ToListAsync();
            }
            return produtos.Where(p => p.UsuarioId == usuario.UsuarioId).ToListAsync();
        }

        public async Task<bool> Update(Produto produto)
        {
            try
            {
                _dbContext.Update(produto);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch (System.Exception)
            {
                return false;
            }
        }
    }
}
