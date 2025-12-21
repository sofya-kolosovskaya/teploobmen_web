
using System.ComponentModel.DataAnnotations;

namespace HeatExchangeApp.Models
{
    public class CalculationInput
    {
        [Required(ErrorMessage = "Поле обязательно")]
        [Range(0.1, 100, ErrorMessage = "Значение от 0.1 до 100")]
        [Display(Name = "Высота слоя H₀, м")]
        public double H0 { get; set; } = 5;

        [Required]
        [Range(0, 2000, ErrorMessage = "Значение от 0 до 2000")]
        [Display(Name = "Температура газа t', °C")]
        public double TPrime { get; set; } = 700;

        [Required]
        [Range(-50, 100, ErrorMessage = "Значение от -50 до 100")]
        [Display(Name = "Температура материала T', °C")]
        public double TDoublePrime { get; set; } = 20;

        [Required]
        [Range(0.01, 10, ErrorMessage = "Значение от 0.01 до 10")]
        [Display(Name = "Скорость газа wг, м/с")]
        public double Wg { get; set; } = 0.78;

        [Required]
        [Range(0.1, 10, ErrorMessage = "Значение от 0.1 до 10")]
        [Display(Name = "Теплоемкость газа Cг, кДж/(кг·К)")]
        public double Cg { get; set; } = 1.32;

        [Required]
        [Range(0.1, 10, ErrorMessage = "Значение от 0.1 до 10")]
        [Display(Name = "Расход материала Gм, кг/с")]
        public double Gm { get; set; } = 1.72;

        [Required]
        [Range(0.1, 10, ErrorMessage = "Значение от 0.1 до 10")]
        [Display(Name = "Теплоемкость материала Cм, кДж/(кг·К)")]
        public double Cm { get; set; } = 1.49;

        [Required]
        [Range(100, 10000, ErrorMessage = "Значение от 100 до 10000")]
        [Display(Name = "Коэффициент теплоотдачи αv, Вт/(м³·К)")]
        public double Av { get; set; } = 2460;

        [Required]
        [Range(0.1, 10, ErrorMessage = "Значение от 0.1 до 10")]
        [Display(Name = "Диаметр аппарата D, м")]
        public double D { get; set; } = 2.2;

        [Required]
        [Range(2, 100, ErrorMessage = "От 2 до 100 точек")]
        [Display(Name = "Количество точек расчета")]
        public int PointsCount { get; set; } = 11;
    }
}
