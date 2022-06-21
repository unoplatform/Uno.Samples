using NUnit.Framework;

namespace Uno.Gallery.UITests;

public class Given_MainPage : TestBase
{
	[Test]
	public void When_SmokeTest()
	{
		App.WaitForElement(q => q.Marked("UserName"));

		App.EnterText("UserName", "test@test.com");
		App.EnterText("Password", "passwordpassword");

		App.Tap("Login");

		App.WaitForElement(q => q.Marked("NavView"));
	}
}
