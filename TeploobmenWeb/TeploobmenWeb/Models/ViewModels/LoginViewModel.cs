
using System.ComponentModel.DataAnnotations;

namespace HeatExchangeApp.Models.ViewModels
{
    public class LoginViewModel
    {
        [Required]
        [Display(Name = "Имя пользователя или Email")]
        public string Username { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string Password { get; set; }

        [Display(Name = "Запомнить меня")]
        public bool RememberMe { get; set; }
    }
}