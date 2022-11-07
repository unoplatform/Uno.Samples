using System;
using System.IO;
using NUnit.Framework;
using Uno.UITest;
using Uno.UITest.Helpers;
using Uno.UITest.Helpers.Queries;
using Uno.UITests.Helpers;

namespace Uno.Gallery.UITests;

[TestFixture]
public abstract class TestBase
{
	private IApp? _app;

	static TestBase()
	{
		AppInitializer.TestEnvironment.AndroidAppName = Constants.AndroidAppName;
		AppInitializer.TestEnvironment.WebAssemblyDefaultUri = Constants.WebAssemblyDefaultUri;
		AppInitializer.TestEnvironment.iOSAppName = Constants.iOSAppName;
		AppInitializer.TestEnvironment.AndroidAppName = Constants.AndroidAppName;
		AppInitializer.TestEnvironment.iOSDeviceNameOrId = Constants.iOSDeviceNameOrId;
		AppInitializer.TestEnvironment.CurrentPlatform = Constants.CurrentPlatform;

#if DEBUG
		AppInitializer.TestEnvironment.WebAssemblyHeadless = false;
#endif

		AppInitializer.ColdStartApp();
	}

	protected IApp App
	{
		get => _app!;
		private set
		{
			_app = value;
			Uno.UITest.Helpers.Queries.Helpers.App = value;
		}
	}

	[SetUp]
	public void SetUpTest()
	{

		App = AppInitializer.AttachToApp();
	}

	[TearDown]
	public void TearDownTest()
	{
		TakeScreenshot("teardown");
	}

	protected void NavigateToSample(string sample, string? design = null)
	{
		var backdoorPropVal = string.Join("-", sample, design);

		var shell = App.Marked("AppShell").WaitUntilExists();
		shell.SetDependencyPropertyValue("CurrentSampleBackdoor", backdoorPropVal);
	}

	protected void ShowMaterialTheme()
	{
		App.WaitThenTap("PART_MaterialRadioButton");
	}

	protected void ShowFluentTheme()
	{
		App.WaitThenTap("PART_FluentRadioButton");
	}

	protected void ShowCupertinoTheme()
	{
		App.WaitThenTap("PART_CupertinoRadioButton");
	}

	protected void ShowNativeTheme()
	{
		App.WaitThenTap("PART_NativeRadioButton");
	}

	protected void OpenNavView()
	{
		if (!IsNavViewOpen())
		{
			App.WaitThenTap("NavToggle");

			//Give the nav view time to open up
			App.Wait(TimeSpan.FromSeconds(2));
		}
	}

	protected void CloseNavView()
	{
		if (IsNavViewOpen())
		{
			App.WaitThenTap("NavToggle");
		}
	}

	private bool IsNavViewOpen()
	{
		App.WaitForElement("RootSplitView", timeout: TimeSpan.FromSeconds(60));
		return App
			.Marked("RootSplitView")
			.GetDependencyPropertyValue<bool>("IsPaneOpen");
	}

	public FileInfo TakeScreenshot(string stepName)
	{
		var title = $"{TestContext.CurrentContext.Test.Name}_{stepName}"
			.Replace(" ", "_")
			.Replace(".", "_");

		var fileInfo = App.Screenshot(title);

		var fileNameWithoutExt = Path.GetFileNameWithoutExtension(fileInfo.Name);
		if (fileNameWithoutExt != title)
		{
			var destFileName = Path
				.Combine(Path.GetDirectoryName(fileInfo.FullName), title + Path.GetExtension(fileInfo.Name));

			if (File.Exists(destFileName))
			{
				File.Delete(destFileName);
			}

			File.Move(fileInfo.FullName, destFileName);

			TestContext.AddTestAttachment(destFileName, stepName);

			fileInfo = new FileInfo(destFileName);
		}
		else
		{
			TestContext.AddTestAttachment(fileInfo.FullName, stepName);
		}

		return fileInfo;
	}

}
