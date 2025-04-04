using System.ComponentModel.DataAnnotations;

namespace GloboClima.Server.Models.Requests;

public class RegisterRequest
{
    [Required(ErrorMessage = "Email é obrigatório")]
    [EmailAddress(ErrorMessage = "Formato de email inválido")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Senha é obrigatória")]
    [MinLength(5, ErrorMessage = "Mínimo de 5 caracteres")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Confirmação de senha é obrigatória")]
    [Compare(nameof(Password), ErrorMessage = "Senhas não coincidem")]
    public string ConfirmPassword { get; set; } = string.Empty;
}
