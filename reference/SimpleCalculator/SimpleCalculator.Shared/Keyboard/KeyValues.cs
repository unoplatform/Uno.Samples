using SimpleCalculator.Business;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleCalculator.Keyboard;

public static class KeyValues
{
    public static Dictionary<string, KeyInput> Keys = new()
    {
        { "Number0", KeyInput.Zero },
        { "NumberPad0", KeyInput.Zero },
        { "Number1", KeyInput.One },
        { "NumberPad1", KeyInput.One },
        { "Number2", KeyInput.Two },
        { "NumberPad2", KeyInput.Two },
        { "Number3", KeyInput.Three },
        { "NumberPad3", KeyInput.Three },
        { "Number4", KeyInput.Four },
        { "NumberPad4", KeyInput.Four },
        { "Number5", KeyInput.Five },
        { "NumberPad5", KeyInput.Five },
        { "Number6", KeyInput.Six },
        { "NumberPad6", KeyInput.Six },
        { "Number7", KeyInput.Seven },
        { "NumberPad7", KeyInput.Seven },
        { "Number8", KeyInput.Eight },
        { "NumberPad8", KeyInput.Eight },
        { "Number9", KeyInput.Nine },
        { "NumberPad9", KeyInput.Nine },
        { "Add", KeyInput.Addition },
        { "Divide", KeyInput.Division },
        { "Enter",KeyInput.Equal },
        { "Subtract",KeyInput.Subtraction },
        { "Multiply", KeyInput.Multiplication },
        { "*", KeyInput.Multiplication },
        { "X", KeyInput.Multiplication },
        { "Back", KeyInput.Back },
        { "Delete", KeyInput.Back },
        { "C", KeyInput.Clear },
        { "Decimal", KeyInput.Dot }
    };
}
