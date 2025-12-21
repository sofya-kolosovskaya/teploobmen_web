// Models/CalculationResult.cs
using System.Text.Json.Serialization;

namespace HeatExchangeApp.Models
{
    public class CalculationResult
    {
        // Координаты и температуры
        public double[] YCoordinates { get; set; }
        public double[] MaterialTemp { get; set; }  // t
        public double[] GasTemp { get; set; }       // T
        public double[] TempDifference { get; set; }

        // Дополнительные параметры
        public double S { get; set; }
        public double Wm { get; set; }
        public double Wg { get; set; }
        public double M { get; set; }
        public double Y0 { get; set; }

        // Входные параметры
        [JsonIgnore]
        public CalculationInput Input { get; set; }

        // Название расчета
        public string Name { get; set; }

        // Конструктор
        public CalculationResult()
        {
            Name = "Новый расчет";
        }
    }
}