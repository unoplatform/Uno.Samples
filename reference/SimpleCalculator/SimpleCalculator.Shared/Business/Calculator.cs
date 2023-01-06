using System;
using Windows.System;

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

    public string Output => $"{(Result != null ? Result.Value : HasNumber ? Number : "0")}";
    public string Equation => $"{Number1} {Operator} {Number2}{(IsNumber2Percentage ? "%" : string.Empty)}{(Result != null ? $" =" : string.Empty)}";

    public Calculator Input (KeyInput key)
        => Input(this, key);

    public Calculator Input (VirtualKey key)
    {
        return key switch
        {
            VirtualKey.Divide => Input(KeyInput.Division),
            VirtualKey.Multiply or VirtualKey.X => Input(KeyInput.Multiplication),
            VirtualKey.Add => Input(KeyInput.Addition),
            VirtualKey.Subtract => Input(KeyInput.Subtraction),
            VirtualKey.Back or VirtualKey.Delete => Input(KeyInput.Back),
            VirtualKey.Decimal => Input(KeyInput.Dot),
            VirtualKey.C => Input(KeyInput.Clear),
            VirtualKey.Enter => Input(KeyInput.Equal),
            VirtualKey.Number0 or VirtualKey.NumberPad0 => Input(KeyInput.Zero),
            VirtualKey.Number1 or VirtualKey.NumberPad1 => Input(KeyInput.One),
            VirtualKey.Number2 or VirtualKey.NumberPad2 => Input(KeyInput.Two),
            VirtualKey.Number3 or VirtualKey.NumberPad3 => Input(KeyInput.Three),
            VirtualKey.Number4 or VirtualKey.NumberPad4 => Input(KeyInput.Four),
            VirtualKey.Number5 or VirtualKey.NumberPad5 => Input(KeyInput.Five),
            VirtualKey.Number6 or VirtualKey.NumberPad6 => Input(KeyInput.Six),
            VirtualKey.Number7 or VirtualKey.NumberPad7 => Input(KeyInput.Seven),
            VirtualKey.Number8 or VirtualKey.NumberPad8 => Input(KeyInput.Eight),
            VirtualKey.Number9 or VirtualKey.NumberPad9 => Input(KeyInput.Nine),

            _ => Input(KeyInput.None)
        }; 
    }

    private Calculator Input (Calculator calculator, KeyInput key)
    {
        if (key == KeyInput.None)
            return calculator;

        calculator = RestartOrClear(calculator, key);

        return key switch
        {
            KeyInput.Division or KeyInput.Multiplication or KeyInput.Subtraction or KeyInput.Addition => ProcessOperatorKey(calculator, key),
            KeyInput.Back => ProcessBackKey(calculator),
            KeyInput.Dot => ProcessDotKey(calculator),
            KeyInput.Zero when !calculator.HasNumber => calculator,
            KeyInput.Clear => new(),
            KeyInput.Equal => ProcessEqualsKey(calculator),
            KeyInput.Percentage => ProcessPercentageKey(calculator),
            KeyInput.PlusMinus => ProcessPlusMinusKey(calculator),

            _ => calculator with { Number = calculator.Number + (int)key }
        };
    }

    private Calculator RestartOrClear(Calculator calculator, KeyInput key)
    {
        if (calculator.Result != null)
        {
            if (key == KeyInput.Division 
                || key == KeyInput.Multiplication 
                || key == KeyInput.Addition 
                || key == KeyInput.Subtraction)
            {
                calculator = calculator with
                {
                    Number1 = calculator.Result,
                    Result = null,
                    Number2 = null,
                    Number = null,
                    Operator = GetOperator(key),
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
                Number = "0."
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

    private Calculator ProcessOperatorKey(Calculator calculator, KeyInput key)
    {
        if (calculator.HasNumber && !calculator.HasOperator)
        {
            calculator = calculator with
            {
                Operator = GetOperator(key),
                Number1 = GetNumber(calculator.Number),
                Number = null
            };
        }

        return calculator;
    }

    double? GetNumber (string number)
        => Convert.ToDouble(number);

    string GetOperator (KeyInput op) => op switch
    {
        KeyInput.Division       => "÷",
        KeyInput.Multiplication => "×",
        KeyInput.Addition       => "+",
        KeyInput.Subtraction    => "−",

        _ => throw new InvalidOperationException()
    };
}

public enum KeyInput
{
    Zero = 0,
    One = 1,
    Two = 2,
    Three = 3,
    Four = 4,
    Five = 5,
    Six = 6,
    Seven = 7,
    Eight = 8,
    Nine = 9,
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
    None
}
