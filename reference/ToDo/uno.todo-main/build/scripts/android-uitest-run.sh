#!/usr/bin/env bash
set -euo pipefail
IFS=$'\n\t'

export UNO_UITEST_SCREENSHOT_PATH=$BUILD_ARTIFACTSTAGINGDIRECTORY/screenshots/android
export UNO_UITEST_PLATFORM=Android
export UNO_UITEST_ANDROIDAPK_PATH=$BUILD_SOURCESDIRECTORY/Uno.Gallery/Uno.Gallery.Droid/bin/Release/com.nventive.uno.ui.demo-Signed.apk
export UNO_UITEST_PROJECT=$BUILD_SOURCESDIRECTORY/Uno.Gallery/Uno.Gallery.UITest/Uno.Gallery.UITest.csproj
export UNO_UITEST_ANDROID_PROJECT=$BUILD_SOURCESDIRECTORY/Uno.Gallery/Uno.Gallery.Droid/Uno.Gallery.Droid.csproj
export UNO_UITEST_BINARY=$BUILD_SOURCESDIRECTORY/Uno.Gallery/Uno.Gallery.UITest/bin/Release/net47/Uno.Gallery.UITest.dll
export UNO_UITEST_NUNIT_VERSION=3.11.1
export UNO_UITEST_NUGET_URL=https://dist.nuget.org/win-x86-commandline/v5.7.0/nuget.exe
export UNO_EMULATOR_INSTALLED=$BUILD_SOURCESDIRECTORY/build/.emulator_started

if [ ! -f "$UNO_EMULATOR_INSTALLED" ];
then
	# Install AVD files
	echo "y" | $ANDROID_HOME/tools/bin/sdkmanager --install 'system-images;android-28;google_apis_playstore;x86'

	# Create emulator
	echo "no" | $ANDROID_HOME/tools/bin/avdmanager create avd -n xamarin_android_emulator -k 'system-images;android-28;google_apis_playstore;x86' --force

	echo $ANDROID_HOME/emulator/emulator -list-avds

	echo "Starting emulator"

	# Start emulator in background
	$ANDROID_HOME/emulator/emulator -avd xamarin_android_emulator -no-snapshot > /dev/null 2>&1 &

	touch "$UNO_EMULATOR_INSTALLED"
fi

# build the sample, while the emulator is starting
msbuild /r /p:Configuration=Release $UNO_UITEST_PROJECT
msbuild /r /p:Configuration=Release /p:IsUiAutomationMappingEnabled=True /p:AotAssemblies=false $UNO_UITEST_ANDROID_PROJECT

# Wait for the emulator to finish booting
$ANDROID_HOME/platform-tools/adb wait-for-device shell 'while [[ -z $(getprop sys.boot_completed | tr -d '\r') ]]; do sleep 1; done; input keyevent 82'

$ANDROID_HOME/platform-tools/adb devices

echo "Emulator started"

cd $BUILD_SOURCESDIRECTORY/build

wget $UNO_UITEST_NUGET_URL
mono nuget.exe install NUnit.ConsoleRunner -Version $UNO_UITEST_NUNIT_VERSION

mkdir -p $UNO_UITEST_SCREENSHOT_PATH

mono $BUILD_SOURCESDIRECTORY/build/NUnit.ConsoleRunner.$UNO_UITEST_NUNIT_VERSION/tools/nunit3-console.exe \
	$UNO_UITEST_BINARY \
	--timeout=2700000
