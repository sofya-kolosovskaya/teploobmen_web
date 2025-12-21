
using System;
using System.ComponentModel.DataAnnotations;

namespace HeatExchangeApp.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        [Display(Name = "Имя пользователя")]
        public string Username { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string PasswordHash { get; set; } 

        public DateTime CreatedAt { get; set; }

        //Навигационное свойство для расчетов пользователя
        public virtual ICollection<CalculationHistory> Calculations { get; set; }

        public User()
        {
            CreatedAt = DateTime.Now;
        }
    }
}