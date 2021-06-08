using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TCC.Models
{
    public class Usuario
    {
        [Key]
        [Display(Name = "Código Usuário")]
        public int? UsuarioId { get; set; }

        [Required(ErrorMessage = "O campo do nome não pode estar vazio!")]
        [Display(Name = "Nome")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "O campo do email não pode estar vazio!")]
        [EmailAddress(ErrorMessage = "E-mail inválido!")]
        [Display(Name = "E-mail")]
        public string Email { get; set; }

        [Required(ErrorMessage = "O campo da senha não pode estar vazio!")]
        public string Senha { get; set; }

        public virtual ICollection<Produto> Produtos { get; set; }
    }
}
