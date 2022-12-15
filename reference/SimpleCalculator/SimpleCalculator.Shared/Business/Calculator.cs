using System;

namespace SimpleCalculator.Business
{
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
        public string Equation => $"{Number1} {Operator} {Number2}{(IsNumber2Percentage ? "%" : string.Empty)}{(Result != null ? " =" : string.Empty)}";

        public Calculator Input (string key)
            => Input(this, key);

        private Calculator Input (Calculator calculator, string key)
        {
            calculator = RestartOrClear(calculator, key);

            return key switch
            {
                "÷" or "×" or "-" or "+" => OperatorKey(calculator, key),
                "back"  => BackKey(calculator),
                "."     => DotKey(calculator),
                "0"     => ZeroKey(calculator),
                "C"     => new(),
                "="     => EqualsKey(calculator),
                "%"     => PercentageKey(calculator),
                "+-"    => PlusMinusKey(calculator),

                _       => calculator with { Number = calculator.Number + key }
            };
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

        private Calculator BackKey (Calculator calculator)
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

        private Calculator DotKey (Calculator calculator)
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
        
        private Calculator ZeroKey (Calculator calculator)
        {
            if (calculator.HasNumber)
            {
                calculator = calculator with
                {
                    Number = calculator.Number + "0"
                };
            }

            return calculator;
        }
        
        private Calculator EqualsKey (Calculator calculator)
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

            return calculator;
        }

        private Calculator PercentageKey (Calculator calculator)
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

            return calculator;
        }

        private Calculator PlusMinusKey (Calculator calculator)
        {
            if (calculator.HasNumber)
            {
                calculator = calculator with { Number = calculator.Number?.StartsWith("-") == true ? calculator.Number?.Substring(1) : "-" + calculator.Number };
            }
            return calculator;
        }

        private Calculator OperatorKey (Calculator calculator, string key)
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
        {
            return Convert.ToDouble(number);
        }
    }
}
