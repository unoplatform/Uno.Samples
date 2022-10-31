

namespace SimpleCalculator.UITests;

public class Given_MainPage : TestBase
{
	[Test]
	public void When_SmokeTest()
	{
		// Query for the SecondPageButton and then tap it
		Query xamlButton = q => q.All().Marked("SecondPageButton");
		App.WaitForElement(xamlButton);
		App.Tap(xamlButton);

		// Take a screenshot and add it to the test results
		TakeScreenshot("After tapped");
	}
}
