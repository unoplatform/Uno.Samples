


namespace SimpleCalculator.Tests;

public class AppInfoTests
{
	[SetUp]
	public void Setup()
	{
	}

	[Test]
	public void AppInfoCreation()
	{
		var appInfo = new AppConfig { Title = "Test" };

		appInfo.Should().NotBeNull();
		appInfo.Title.Should().Be("Test");
	}
}
