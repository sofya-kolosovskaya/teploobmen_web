// Services/CalculationService.cs
using System;
using System.Text.Json;
using HeatExchangeApp.Models;

namespace HeatExchangeApp.Services
{
    public class CalculationService
    {
        public CalculationResult Calculate(CalculationInput input, string name = "Расчет")
        {
            Console.WriteLine("=== НАЧАЛО РАСЧЕТА ===");

            var result = new CalculationResult
            {
                Name = name,
                Input = input
            };

            try
            {
                // 1. Генерация координат y
                int n = input.PointsCount;
                result.YCoordinates = new double[n];

                Console.WriteLine($"Количество точек: {n}");

                if (input.H0 <= 0)
                {
                    Console.WriteLine("ОШИБКА: H0 должно быть > 0");
                    return result;
                }

                double step = input.H0 / (n - 1);
                for (int i = 0; i < n; i++)
                {
                    result.YCoordinates[i] = i * step;
                }

                Console.WriteLine($"Координаты y: от 0 до {input.H0}, шаг {step}");

                // 2. Расчет основных параметров
                result.S = Math.PI * Math.Pow(input.D / 2, 2);
                result.Wm = input.Gm * input.Cm * 1000;
                result.Wg = input.Wg * result.S * input.Cg * 1000;
                result.M = result.Wm / result.Wg;
                result.Y0 = (input.Av * input.H0) / (input.Wg * input.Cg * 1000);

                Console.WriteLine($"S={result.S}, Wm={result.Wm}, Wg={result.Wg}");
                Console.WriteLine($"m={result.M}, Y0={result.Y0}");

                // 3. Расчет температур
                double denominator = 1 - result.M * Math.Exp((result.M - 1) * result.Y0 / result.M);
                Console.WriteLine($"Знаменатель: {denominator}");

                result.MaterialTemp = new double[n];
                result.GasTemp = new double[n];
                result.TempDifference = new double[n];

                for (int i = 0; i < n; i++)
                {
                    double y = result.YCoordinates[i];
                    double Y = (input.Av * y) / (input.Wg * input.Cg * 1000);

                    double F1 = 1 - Math.Exp((result.M - 1) * Y / result.M);
                    double F2 = 1 - result.M * Math.Exp((result.M - 1) * Y / result.M);

                    double v = F1 / denominator;
                    double theta = F2 / denominator;

                    result.MaterialTemp[i] = input.TPrime + (input.TDoublePrime - input.TPrime) * v;
                    result.GasTemp[i] = input.TPrime + (input.TDoublePrime - input.TPrime) * theta;
                    result.TempDifference[i] = result.MaterialTemp[i] - result.GasTemp[i];

                    if (i < 3) // Выводим первые 3 точки для отладки
                    {
                        Console.WriteLine($"Точка {i}: y={y:F2}, Y={Y:F4}, t={result.MaterialTemp[i]:F1}, T={result.GasTemp[i]:F1}");
                    }
                }

                Console.WriteLine("=== РАСЧЕТ ЗАВЕРШЕН УСПЕШНО ===");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ОШИБКА В РАСЧЕТЕ: {ex.Message}");
                Console.WriteLine($"StackTrace: {ex.StackTrace}");
            }

            return result;
        }

        public CalculationHistory CreateHistory(CalculationInput input, CalculationResult result, string username = "")
        {
            return new CalculationHistory
            {
                CalculationName = result.Name,
                H0 = input.H0,
                TPrime = input.TPrime,
                TDoublePrime = input.TDoublePrime,
                Wg = input.Wg,
                Cg = input.Cg,
                Gm = input.Gm,
                Cm = input.Cm,
                Av = input.Av,
                D = input.D,
                ResultsJson = JsonSerializer.Serialize(result),
                Username = username
            };
        }

        public async Task<CalculationResult> CalculateAsync(CalculationInput input, string name = "Расчет")
        {
            return await Task.Run(() => Calculate(input, name));
        }

        public CalculationResult LoadFromHistory(CalculationHistory history)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            return JsonSerializer.Deserialize<CalculationResult>(history.ResultsJson, options);
        }
    }

}