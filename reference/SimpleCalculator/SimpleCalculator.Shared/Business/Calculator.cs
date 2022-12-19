using System;
using System.Linq;

namespace SimpleCalculator.Business;

public record Calculator
{
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

    public string Output => $"{(Result != null ? Result.Value : HasNumber ? Number : ZeroKey)}";
    public string Equation => $"{Number1} {Operator} {Number2}{(IsNumber2Percentage ? PercentageKey : string.Empty)}{(Result != null ? $" {EqualsKey}" : string.Empty)}";

    public const string MultiplicationKey = "×";
    public const string DivisionKey = "÷";
    public const string SubtractionKey = "-";
    public const string AdditionKey = "+";
    public const string BackKey = "back";
    public const string DotKey = ".";
    public const string CleanKey = "C";
    public const string EqualsKey = "=";
    public const string PercentageKey = "%";
    public const string PlusMinusKey = "+-";
    public const string ZeroKey = "0";
    public const string OneKey = "1";
    public const string TwoKey = "2";
    public const string ThreeKey = "3";
    public const string FourKey = "4";
    public const string FiveKey = "5";
    public const string SixKey = "6";
    public const string SevenKey = "7";
    public const string EightKey = "8";
    public const string NineKey = "9";

    private readonly string[] AllKeys = new[]
    {
        MultiplicationKey, DivisionKey, SubtractionKey, AdditionKey, BackKey, DotKey, ZeroKey, CleanKey, EqualsKey, PercentageKey, PlusMinusKey, OneKey, TwoKey, ThreeKey, FourKey, FiveKey, SixKey, SevenKey, EightKey, NineKey
    };
    
    private readonly string[] OperationKeys = new[]
    {
        MultiplicationKey, DivisionKey, SubtractionKey, AdditionKey
    };

    public Calculator Input (string key)
        => Input(this, key);

    private Calculator Input (Calculator calculator, string key)
    {
        if (!AllKeys.Any(x => x == key))
            throw new InvalidOperationException("Inputed key is not valid.");

        calculator = RestartOrClear(calculator, key);

        return key switch
        {
            DivisionKey or MultiplicationKey or SubtractionKey or AdditionKey => ProcessOperatorKey(calculator, key),
            BackKey => ProcessBackKey(calculator),
            DotKey => ProcessDotKey(calculator),
            ZeroKey when !calculator.HasNumber => calculator,
            CleanKey => new(),
            EqualsKey => ProcessEqualsKey(calculator),
            PercentageKey => ProcessPercentageKey(calculator),
            PlusMinusKey => ProcessPlusMinusKey(calculator),

            _       => calculator with { Number = calculator.Number + key }
        };
    }

    private Calculator RestartOrClear(Calculator calculator, string key)
    {
        if (calculator.Result != null)
        {
            if (OperationKeys.Any(x => x == key))
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
            if (calculator.Number?.Contains(DotKey) == false)
            {
                calculator = calculator with
                {
                    Number = calculator.Number + DotKey
                };
            }
        }
        else
        {
            calculator = calculator with
            {
                Number = ZeroKey + DotKey
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
                DivisionKey => calculator.Number1!.Value / number2!.Value,
                MultiplicationKey => calculator.Number1!.Value * number2!.Value,
                AdditionKey => calculator.Number1!.Value + number2!.Value,
                SubtractionKey => calculator.Number1!.Value - number2!.Value,
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
                DivisionKey => calculator.Number1!.Value / (number2!.Value / 100) * calculator.Number1!.Value,
                MultiplicationKey => calculator.Number1!.Value * (number2!.Value / 100) * calculator.Number1!.Value,
                AdditionKey => calculator.Number1!.Value + (number2!.Value / 100) * calculator.Number1!.Value,
                SubtractionKey => calculator.Number1!.Value - (number2!.Value / 100) * calculator.Number1!.Value,
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
            calculator = calculator with { Number = calculator.Number?.StartsWith(SubtractionKey) == true ? calculator.Number?.Substring(1) : SubtractionKey + calculator.Number };
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
