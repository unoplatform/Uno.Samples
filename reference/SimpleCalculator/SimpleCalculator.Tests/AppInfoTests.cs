using SimpleCalculator.Business;

namespace SimpleCalculator.Tests;

public class AppInfoTests
{
	[SetUp]
	public void Setup()
	{
	}

    [Test]
    public void Test1()
    {
        Calculator c = new();
        c = c.Input("1")
             .Input("+")
             .Input("2")
             .Input("=");

        Assert.AreEqual("4", c.Output);
    }

    [Test]
    public void MultiplicationTest()
    {
		Calculator c = new();
		c = c.Input("1")
			 .Input("1")
			 .Input("×")
			 .Input("4")
			 .Input("=");

		Assert.AreEqual("44", c.Output);
        Assert.AreEqual("11 × 4 =", c.Equation);
    }

    [Test]
    public void DivisionTest()
    {
        Calculator c = new();
        c = c.Input("6")
             .Input("5")
             .Input("6")
             .Input("1")
             .Input("÷")
             .Input("9")
             .Input("=");

        Assert.AreEqual("729", c.Output);
        Assert.AreEqual("6561 ÷ 9 =", c.Equation);
    }

    [Test]
    public void AdditionTest()
    {
        Calculator c = new();
        c = c.Input("1")
             .Input("2")
             .Input("9")
             .Input("0")
             .Input("+")
             .Input("9")
             .Input("5")
             .Input("2")
             .Input("1")
             .Input("=");

        Assert.AreEqual("10811", c.Output);
        Assert.AreEqual("1290 + 9521 =", c.Equation);
    }

    [Test]
    public void SubtractionTest()
    {
        Calculator c = new();
        c = c.Input("9")
             .Input("9")
             .Input("9")
             .Input("-")
             .Input("1")
             .Input("0")
             .Input("0")
             .Input("0")
             .Input("=");

        Assert.AreEqual("-1", c.Output);
        Assert.AreEqual("999 - 1000 =", c.Equation);
    }

    [Test]
    public void MultiOperationTest()
    {
        Calculator c = new();
        c = c.Input("1")
             .Input("+")
             .Input("4")
             .Input("=")
             .Input("×")
             .Input("2")
             .Input("=")
             .Input("-")
             .Input("1")
             .Input("=")
             .Input("÷")
             .Input("3")
             .Input("=");

        Assert.AreEqual("3", c.Output);
        Assert.AreEqual("9 ÷ 3 =", c.Equation);
    }
}
