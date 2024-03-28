using NUnit.Framework;
using NUnit.Framework.Interfaces;
using System;
using System.IO;
using Uno.UITest;
using Uno.UITest.Helpers.Queries;
using Uno.UITest.Selenium;
using Uno.UITests.Helpers;

namespace ToDo.UI.Tests
{
    public class TestBase
	{
		private readonly string _screenShotPath = Environment.GetEnvironmentVariable("UNO_UITEST_SCREENSHOT_PATH");

		private IApp? _app;
		private DateTime _startTime;

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


			// Start the app only once, so the tests runs don't restart it
			// and gain some time for the tests.
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
			_startTime = DateTime.Now;
			App = AppInitializer.AttachToApp();
        }

		[TearDown]
		public void AfterEachTest()
		{
			if (
				TestContext.CurrentContext.Result.Outcome != ResultState.Success
				&& TestContext.CurrentContext.Result.Outcome != ResultState.Skipped
				&& TestContext.CurrentContext.Result.Outcome != ResultState.Ignored
			)
			{
				TakeScreenshot($"{TestContext.CurrentContext.Test.Name} - Tear down on error");
			}

			WriteSystemLogs(GetCurrentStepTitle("log"));
		}

		private void WriteSystemLogs(string fileName)
		{
			if (_app != null && AppInitializer.GetLocalPlatform() == Platform.Browser)
			{
				var outputPath = string.IsNullOrEmpty(_screenShotPath)
					? Environment.CurrentDirectory
					: _screenShotPath;

				using (var logOutput = new StreamWriter(Path.Combine(outputPath, $"{fileName}_{DateTime.Now:yyyy-MM-dd-HH-mm-ss.fff}.txt")))
				{
					foreach (var log in _app.GetSystemLogs(_startTime.ToUniversalTime()))
					{
						logOutput.WriteLine($"{log.Timestamp}/{log.Level}: {log.Message}");
					}
				}
			}
		}

		private static string GetCurrentStepTitle(string stepName) =>
					$"{TestContext.CurrentContext.Test.Name}_{stepName}"
						.Replace(" ", "_")
						.Replace(".", "_")
						.Replace("__", "_");

		public FileInfo TakeScreenshot(string stepName)
        {
            if(_app == null)
            {
                throw new InvalidOperationException("App is not yet created");
            }

            var title = $"{TestContext.CurrentContext.Test.Name}_{stepName}"
                .Replace(" ", "_")
                .Replace(".", "_");

            var fileInfo = _app.Screenshot(title);

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
}
