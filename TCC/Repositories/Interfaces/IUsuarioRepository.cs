using System.Threading.Tasks;
using TCC.Models;

namespace TCC.Repositories.Interfaces
{
    public interface IUsuarioRepository
    {
        Task<bool> Create(Usuario usuario);
        Task<bool> Delete(int? id);
        bool Exists(string email);
        Task<Usuario> Get(string email, string senha);
        bool CanLogin(Usuario usuario);
        Task<bool> Update(Usuario usuario);
        bool IsAdm(Usuario usuario);
    }
}
