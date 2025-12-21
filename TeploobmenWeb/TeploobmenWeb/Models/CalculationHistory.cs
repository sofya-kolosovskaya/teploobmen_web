
using System;
using System.ComponentModel.DataAnnotations;

namespace HeatExchangeApp.Models
{
    public class CalculationHistory
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string CalculationName { get; set; }

        public DateTime CreatedAt { get; set; }

        // Основные параметры
        public double H0 { get; set; }
        public double TPrime { get; set; }
        public double TDoublePrime { get; set; }
        public double Wg { get; set; }
        public double Cg { get; set; }
        public double Gm { get; set; }
        public double Cm { get; set; }
        public double Av { get; set; }
        public double D { get; set; }

        // Результаты (сериализованные)
        public string ResultsJson { get; set; }

        public string Username { get; set; }

        // Связь с User (если реализуете полноценную авторизацию)
        public int? UserId { get; set; }
        public virtual User User { get; set; }

        // Конструктор
        public CalculationHistory()
        {
            CreatedAt = DateTime.Now;
            Username = "Гость"; // Значение по умолчанию
        }
    }
}