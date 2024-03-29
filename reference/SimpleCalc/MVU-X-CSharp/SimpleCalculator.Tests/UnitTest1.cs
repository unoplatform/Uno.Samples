namespace SimpleCalculator.Tests;

public class Tests
{
    [TestCase("11×4 =", "44", "11 × 4 =")]
    [TestCase("6561÷9=", "729", "6561 ÷ 9 =")]
    [TestCase("1290+9521=", "10811", "1290 + 9521 =")]
    [TestCase("999−1000=", "-1", "999 − 1000 =")]
    [TestCase("100+50%", "150", "100 + 50% =")]
    [TestCase("1+4=×2=−1=÷3=", "3", "9 ÷ 3 =")]
    [TestCase("1++×+4÷×2−1=−−−÷3=", "3", "9 ÷ 3 =")]
    [TestCase("C%±+−÷×=", "0", "  ")]
    [TestCase("", "0", "  ")]
    public void OperationTest(string input, string output, string equation)
    {
        Calculator c = new();

        foreach (var value in input)
            c = c.Input(value.ToString());

        Assert.Multiple(() =>
        {
            Assert.That(c.Output, Is.EqualTo(output));
            Assert.That(c.Equation, Is.EqualTo(equation));
        });
    }
}
