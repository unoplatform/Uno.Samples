using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleCalculator.Business
{
    public record Calculator
    {
        private string? Number { get; init; }
        private string? Operator { get; init; }
        private double? Number1 { get; init; }
        private double? Number2 { get; init; }
        private bool IsNumber2Percentage { get; init; }

        public string OutPut => $"{(Result != null ? Result.Value : HasNumber ? Number : "0")}";
        public string? Equation => $"{Number1} {Operator} {Number2}{(IsNumber2Percentage ? "%" : string.Empty)}{(Result != null ? " =" : string.Empty)}";
        private double? Result { get; init; }
        bool HasOperator => !string.IsNullOrEmpty(Operator);
        bool HasNumber => !string.IsNullOrEmpty(Number);
        bool HasNumber1 => Number1 != null;
        bool HasNumber2 => Number2 != null;

        public Calculator Input(string key)
            => Input(this, key);

        private static Calculator Input(Calculator calculator, string key)
        {
            if (calculator.Result != null)
            {
                if (key == "÷" || key == "×" || key == "+" || key == "-")
                {
                    calculator = calculator with
                    {
                        Number1 = calculator.Result,
                        Result = null,
                        Number2 = null,
                        Number = null,
                        Operator = key,
                        IsNumber2Percentage = false
                    };
                }
                else
                {
                    calculator = new();
                }
            }

            if (key == "back")
            {
                if (calculator.HasNumber)
                {
                    calculator = calculator with
                    {
                        Number = calculator.Number?.Substring(0, calculator.Number.Length - 1)
                    };
                }
            }
            else if (key == ".")
            {
                if (calculator.HasNumber)
                {
                    if (calculator.Number?.Contains(".") == false)
                    {
                        calculator = calculator with
                        {
                            Number = calculator.Number + key
                        };
                    }
                }
                else
                {
                    calculator = calculator with
                    {
                        Number = "0" + key
                    };
                }
            }
            else if (key == "0")
            {
                if (calculator.HasNumber)
                {
                    calculator = calculator with
                    {
                        Number = calculator.Number + key
                    };
                }
            }
            else if (key == "C")
            {
                calculator = calculator with
                {
                    Number = null,
                    Number1 = null,
                    Number2 = null,
                    Operator = null,
                    IsNumber2Percentage = false,
                    Result = null
                };
            }
            else if (key == "=")
            {
                if (calculator.HasOperator && calculator.HasNumber1)
                {
                    double? number2 = calculator.HasNumber ? GetNumber(calculator.Number) : 0.0;
                    double? result = null;
                    switch (calculator.Operator)
                    {
                        case "÷":
                            result = calculator.Number1!.Value / number2!.Value;
                            break;
                        case "×":
                            result = calculator.Number1!.Value * number2!.Value;
                            break;
                        case "+":
                            result = calculator.Number1!.Value + number2!.Value;
                            break;
                        case "-":
                            result = calculator.Number1!.Value - number2!.Value;
                            break;
                    }

                    calculator = calculator with
                    {
                        Number2 = number2,
                        Result = result
                    };
                }
            }
            else if (key == "%")
            {
                if (calculator.HasOperator && calculator.HasNumber1)
                {
                    double? number2 = calculator.HasNumber ? GetNumber(calculator.Number) : 0.0;
                    double? result = null;
                    switch (calculator.Operator)
                    {
                        case "÷":
                            result = calculator.Number1!.Value / (number2!.Value / 100) * calculator.Number1!.Value;
                            break;
                        case "×":
                            result = calculator.Number1!.Value * (number2!.Value / 100) * calculator.Number1!.Value;
                            break;
                        case "+":
                            result = calculator.Number1!.Value + (number2!.Value / 100) * calculator.Number1!.Value;
                            break;
                        case "-":
                            result = calculator.Number1!.Value - (number2!.Value / 100) * calculator.Number1!.Value;
                            break;
                    }

                    calculator = calculator with
                    {
                        Number2 = number2,
                        Result = result,
                        IsNumber2Percentage = true
                    };
                }
            }
            else if (key == "+-")
            {
                if (calculator.HasNumber)
                {
                    calculator = calculator with { Number = calculator.Number?.StartsWith("-") == true ? calculator.Number?.Substring(1) : "-" + calculator.Number };
                }
            }
            else if (key == "÷" || key == "×" || key == "+" || key == "-")
            {
                if (calculator.HasNumber && !calculator.HasOperator)
                {
                    calculator = calculator with
                    {
                        Operator = key,
                        Number1 = GetNumber(calculator.Number),
                        Number = null
                    };
                }
            }
            else
            {
                calculator = calculator with { Number = calculator.Number + key };
            }

            return calculator;
        }

        static double? GetNumber(string? number)
        {
            return Convert.ToDouble(number);
        }
    }
}
