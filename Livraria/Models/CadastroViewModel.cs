using System.ComponentModel.DataAnnotations;

namespace Livraria.Models
{
    public class CadastroViewModel
    {
        [Required(ErrorMessage = "Informe seu nome.")]
        public string Nome { get; set; } = string.Empty;

        [Required(ErrorMessage = "Informe seu e-mail.")]
        [EmailAddress(ErrorMessage = "E-mail inválido.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Informe sua senha.")]
        [DataType(DataType.Password)]
        public string Senha { get; set; } = string.Empty;

        [Required(ErrorMessage = "Confirme sua senha.")]
        [Compare("Senha", ErrorMessage = "As senhas não coincidem.")]
        [DataType(DataType.Password)]
        public string ConfirmarSenha { get; set; } = string.Empty;
    }
}