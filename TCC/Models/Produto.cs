using System.ComponentModel.DataAnnotations;

namespace TCC.Models
{
    public class Produto
    {
        [Key]
        [Display(Name = "Código Produto")]
        public int? ProdutoId { get; set; }

        [Display(Name = "Temperatura atual")]
        public float? Temperatura { get; set; }

        public int? UsuarioId { get; set; }
        public virtual Usuario Usuario { get; set; }

        [Display(Name = "Limite máximo")]
        public float? Limite_max { get; set; }

        [Display(Name = "Limite Minimo")]
        public float? Limite_min { get; set; }

        public string Token { get; set; }
        // 20 caracteres
    }
}
