using SimpleCalculator.Business;

namespace SimpleCalculator.Tests;

public class AppInfoTests
{
    [TestCase(new KeyInput[] { KeyInput.One, KeyInput.One, KeyInput.Multiplication, KeyInput.Four, KeyInput.Equal }, "44", "11 × 4 =")]
    [TestCase(new KeyInput[] { KeyInput.Six, KeyInput.Five, KeyInput.Six, KeyInput.One, KeyInput.Division, KeyInput.Nine, KeyInput.Equal }, "729", "6561 ÷ 9 =")]
    [TestCase(new KeyInput[] { KeyInput.One, KeyInput.Two, KeyInput.Nine, KeyInput.Zero, KeyInput.Addition, KeyInput.Nine, KeyInput.Five, KeyInput.Two, KeyInput.One, KeyInput.Equal }, "10811", "1290 + 9521 =")]
    [TestCase(new KeyInput[] { KeyInput.Nine, KeyInput.Nine, KeyInput.Nine, KeyInput.Subtraction, KeyInput.One, KeyInput.Zero, KeyInput.Zero, KeyInput.Zero, KeyInput.Equal }, "-1", "999 − 1000 =")]
    [TestCase(new KeyInput[] { KeyInput.One, KeyInput.Addition, KeyInput.Four, KeyInput.Equal, KeyInput.Multiplication, KeyInput.Two, KeyInput.Equal, KeyInput.Subtraction, KeyInput.One, KeyInput.Equal, KeyInput.Division, KeyInput.Three, KeyInput.Equal }, "3", "9 ÷ 3 =")]
    public void OperationTest(KeyInput[] input, string output, string equation)
    {
        Calculator c = new();

        foreach (var value in input)
            c = c.Input(value);

        Assert.AreEqual(output, c.Output);
        Assert.AreEqual(equation, c.Equation);
    }
}
