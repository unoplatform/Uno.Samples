using NUnit.Framework;
using Query = System.Func<Uno.UITest.IAppQuery, Uno.UITest.IAppQuery>;

namespace ToDo.UI.Tests
{
	public class Given_MainPage : TestBase
    {
        [Test]
        public void When_SmokeTest()
        {
			Query WelcomePage_GetStarted = q => q.All().Marked("WelcomePage_Button_Wide");

			// Make sure the ViewModelButton has rendered
			App.WaitForElement(WelcomePage_GetStarted, timeoutMessage: "Timeout waiting for WelcomePage_Button_Wide");

			// Take a screenshot and add it to the test results
			TakeScreenshot("GetStarted");

			App.Tap(WelcomePage_GetStarted);

			Query important = q => q.All().Marked("Important");
			App.WaitForElement(important, timeoutMessage: "Timeout waiting for Important");

			// Take a screenshot and add it to the test results
			TakeScreenshot("After important");

			
			App.Tap(important);

			Query payBills = q => q.All().Text("Pay bills");
			App.WaitForElement(payBills, timeoutMessage: "Timeout waiting for [Pay bills]");

			// Take a screenshot and add it to the test results
			TakeScreenshot("After pay bill");

			App.Tap(payBills);

			Query dueTime = q => q.All().Text("Due Wed, 13 April");
			App.WaitForElement(dueTime, timeoutMessage: "Timeout waiting for [Due Wed, 13 April]");
		}
	}
}
