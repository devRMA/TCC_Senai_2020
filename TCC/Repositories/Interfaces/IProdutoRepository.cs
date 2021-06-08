using System.Collections.Generic;
using System.Threading.Tasks;
using TCC.Models;

namespace TCC.Repositories.Interfaces
{
    public interface IProdutoRepository
    {
        Task<bool> Create(Produto produto);
        bool Exists(int? id);
        bool Exists(string token);
        Task<Produto> Get(int? id);
        Task<List<Produto>> GetAll(Usuario usuario);
        Task<bool> Update(Produto produto);
        Task<bool> Delete(int? id);
    }
}
