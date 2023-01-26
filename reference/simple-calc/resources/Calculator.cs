using System.Globalization;

namespace SimpleCalculator.Business;

#nullable enable
public record Calculator
{
    private string? Number { get; init; }
    private string? Operator { get; init; }
    private double? Number1 { get; init; }
    private double? Number2 { get; init; }
    private bool IsNumber2Percentage { get; init; }
    private double? Result { get; init; }
    private bool HasOperator => !string.IsNullOrEmpty(Operator);
    private bool HasNumber => !string.IsNullOrEmpty(Number);
    private bool HasNumber1 => Number1 != null;

    public string Output => $"{(Result != null ? Result.Value : HasNumber ? Number : "0")}";
    public string Equation => $"{Number1} {Operator} {Number2}{(IsNumber2Percentage ? "%" : string.Empty)}{(Result != null ? $" =" : string.Empty)}";

    public Calculator Input(string key)
    {
        var calculator = RestartOrClear(key);
        return key switch
        {
            "÷" or "×" or "+" or "−" => OperatorKey(calculator, key),
            "back" => BackKey(calculator),
            "." or "," => DotKey(calculator),
            "0" when !calculator.HasNumber => calculator,
            "C" => new(),
            "=" => EqualsKey(calculator),
            "%" => PercentageKey(calculator),
            "±" => PlusMinusKey(calculator),

            _ => calculator with { Number = calculator.Number + key }
        };
    }

    private Calculator RestartOrClear(string key)
    {
        if (Result != null)
        {
            if (key == "÷" || key == "×" || key == "+" || key == "−")
            {
                return this with
                {
                    Number1 = Result,
                    Result = null,
                    Number2 = null,
                    Number = null,
                    Operator = key,
                    IsNumber2Percentage = false
                };
            }
            else
            {
                return new();
            }
        }

        return this;
    }

    private static Calculator BackKey(Calculator calculator)
    {
        if (calculator.HasNumber)
        {
            calculator = calculator with
            {
                Number = calculator.Number?.Substring(0, calculator.Number.Length - 1)
            };
        }

        return calculator;
    }

    private static Calculator DotKey(Calculator calculator)
    {
        if (calculator.HasNumber)
        {
            if (calculator.Number?.Contains(NumberDecimalSeparator) == false)
            {
                calculator = calculator with
                {
                    Number = calculator.Number + NumberDecimalSeparator
                };
            }
        }
        else
        {
            calculator = calculator with
            {
                Number = "0" + NumberDecimalSeparator
            };
        }

        return calculator;
    }

    private static Calculator EqualsKey(Calculator calculator)
    {
        if (calculator.HasOperator && calculator.HasNumber1)
        {
            double? number2 = calculator.HasNumber ? GetNumber(calculator.Number) : 0.0;

            double result = calculator.Operator switch
            {
                "÷" => calculator.Number1!.Value / number2!.Value,
                "×" => calculator.Number1!.Value * number2!.Value,
                "+" => calculator.Number1!.Value + number2!.Value,
                "−" => calculator.Number1!.Value - number2!.Value,
                _ => throw new InvalidOperationException()
            };

            calculator = calculator with
            {
                Number2 = number2,
                Result = result
            };
        }

        return calculator;
    }

    private static Calculator PercentageKey(Calculator calculator)
    {
        if (calculator.HasOperator && calculator.HasNumber1)
        {
            double? number2 = calculator.HasNumber ? GetNumber(calculator.Number) : 0.0;

            double result = calculator.Operator switch
            {
                "÷" => calculator.Number1!.Value / (number2!.Value / 100) * calculator.Number1!.Value,
                "×" => calculator.Number1!.Value * (number2!.Value / 100) * calculator.Number1!.Value,
                "+" => calculator.Number1!.Value + (number2!.Value / 100) * calculator.Number1!.Value,
                "−" => calculator.Number1!.Value - (number2!.Value / 100) * calculator.Number1!.Value,
                _ => throw new InvalidOperationException()
            };

            calculator = calculator with
            {
                Number2 = number2,
                Result = result,
                IsNumber2Percentage = true
            };
        }

        return calculator;
    }

    private static Calculator PlusMinusKey(Calculator calculator)
    {
        if (calculator.HasNumber)
        {
            calculator = calculator with { Number = calculator.Number?.StartsWith("-") == true ? calculator.Number?.Substring(1) : "-" + calculator.Number };
        }
        return calculator;
    }

    private static Calculator OperatorKey(Calculator calculator, string key)
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

        return calculator;
    }

    private static double? GetNumber(string? number)
        => Convert.ToDouble(number);

    public static readonly string NumberDecimalSeparator = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
}
