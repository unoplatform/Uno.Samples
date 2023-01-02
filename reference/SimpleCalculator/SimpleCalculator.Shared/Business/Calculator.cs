using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleCalculator.Business;

public record Calculator
{
    public static Dictionary<KeyInput, string> KeyTranslator = new()
    {
        { KeyInput.Multiplication, "×" },
        { KeyInput.Division, "÷" },
        { KeyInput.Subtraction, "−" },
        { KeyInput.Addition, "+" },
        { KeyInput.Back, "⌫" },
        { KeyInput.Dot, "." },
        { KeyInput.Clear, "C" },
        { KeyInput.Equal, "=" },
        { KeyInput.Percentage, "%" },
        { KeyInput.PlusMinus, "±" },
        { KeyInput.Zero, "0" },
        { KeyInput.One, "1" },
        { KeyInput.Two, "2" },
        { KeyInput.Three, "3" },
        { KeyInput.Four, "4" },
        { KeyInput.Five, "5" },
        { KeyInput.Six, "6" },
        { KeyInput.Seven, "7" },
        { KeyInput.Eight, "8" },
        { KeyInput.Nine, "9" }
    };

    private string Number { get; init; }
    private string Operator { get; init; }
    private double? Number1 { get; init; }
    private double? Number2 { get; init; }
    private bool IsNumber2Percentage { get; init; }
    private double? Result { get; init; }
    private bool HasOperator => !string.IsNullOrEmpty(Operator);
    private bool HasNumber => !string.IsNullOrEmpty(Number);
    private bool HasNumber1 => Number1 != null;
    private bool HasNumber2 => Number2 != null;

    public string Output => $"{(Result != null ? Result.Value : HasNumber ? Number : "0")}";
    public string Equation => $"{Number1} {Operator} {Number2}{(IsNumber2Percentage ? "%" : string.Empty)}{(Result != null ? $" =" : string.Empty)}";

    public Calculator Input (KeyInput key)
        => Input(this, key);

    private Calculator Input (Calculator calculator, KeyInput key)
    {
        if (KeyTranslator.TryGetValue(key, out var keyValue))
        {
            calculator = RestartOrClear(calculator, keyValue);

            return key switch
            {
                KeyInput.Division or KeyInput.Multiplication or KeyInput.Subtraction or KeyInput.Addition => ProcessOperatorKey(calculator, keyValue),
                KeyInput.Back => ProcessBackKey(calculator),
                KeyInput.Dot => ProcessDotKey(calculator),
                KeyInput.Zero when !calculator.HasNumber => calculator,
                KeyInput.Clear => new(),
                KeyInput.Equal => ProcessEqualsKey(calculator),
                KeyInput.Percentage => ProcessPercentageKey(calculator),
                KeyInput.PlusMinus => ProcessPlusMinusKey(calculator),

                _ => calculator with { Number = calculator.Number + keyValue }
            };
        }
        else
        {
            throw new InvalidOperationException("Inputed key is not valid.");
        }
    }

    private Calculator RestartOrClear(Calculator calculator, string key)
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

        return calculator;
    }

    private Calculator ProcessBackKey(Calculator calculator)
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

    private Calculator ProcessDotKey(Calculator calculator)
    {
        if (calculator.HasNumber)
        {
            if (calculator.Number?.Contains(".") == false)
            {
                calculator = calculator with
                {
                    Number = calculator.Number + "."
                };
            }
        }
        else
        {
            calculator = calculator with
            {
                Number = "0" + "."
            };
        }

        return calculator;
    }
    
    private Calculator ProcessEqualsKey(Calculator calculator)
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
                _   => throw new InvalidOperationException()
            };

            calculator = calculator with
            {
                Number2 = number2,
                Result = result
            };
        }

        return calculator;
    }

    private Calculator ProcessPercentageKey(Calculator calculator)
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
                _   => throw new InvalidOperationException()
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

    private Calculator ProcessPlusMinusKey(Calculator calculator)
    {
        if (calculator.HasNumber)
        {
            calculator = calculator with { Number = calculator.Number?.StartsWith("-") == true ? calculator.Number?.Substring(1) : "-" + calculator.Number };
        }
        return calculator;
    }

    private Calculator ProcessOperatorKey(Calculator calculator, string key)
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

    double? GetNumber (string number)
        => Convert.ToDouble(number);
}

public enum KeyInput
{
    Multiplication,
    Division,
    Subtraction,
    Addition,
    Back,
    Dot,
    Clear,
    Equal,
    Percentage,
    PlusMinus,
    Zero,
    One,
    Two,
    Three,
    Four,
    Five,
    Six,
    Seven,
    Eight,
    Nine
}
