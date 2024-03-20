#!/bin/bash
set -euo pipefail
IFS=$'\n\t'

export UNO_UITEST_PLATFORM=iOS
export UNO_UITEST_IOSBUNDLE_PATH=$BUILD_SOURCESDIRECTORY/Uno.Gallery/Uno.Gallery.iOS/bin/iPhoneSimulator/Release/Uno.Gallery.app
export UNO_UITEST_SCREENSHOT_PATH=$BUILD_ARTIFACTSTAGINGDIRECTORY/screenshots/ios
export UNO_UITEST_PROJECT=$BUILD_SOURCESDIRECTORY/Uno.Gallery/Uno.Gallery.UITest/Uno.Gallery.UITest.csproj
export UNO_UITEST_BINARY=$BUILD_SOURCESDIRECTORY/Uno.Gallery/Uno.Gallery.UITest/bin/Release/net47/Uno.Gallery.UITest.dll
export UNO_UITEST_LOGFILE=$BUILD_ARTIFACTSTAGINGDIRECTORY/screenshots/ios/nunit-log.txt
export UNO_UITEST_IOS_PROJECT=$BUILD_SOURCESDIRECTORY/Uno.Gallery/Uno.Gallery.iOS/Uno.Gallery.iOS.csproj
export UNO_UITEST_NUNIT_VERSION=3.11.1
export UNO_UITEST_NUGET_URL=https://dist.nuget.org/win-x86-commandline/v5.7.0/nuget.exe

echo "Lising iOS simulators"
xcrun simctl list devices --json

/Applications/Xcode.app/Contents/Developer/Applications/Simulator.app/Contents/MacOS/Simulator &

cd $BUILD_SOURCESDIRECTORY

msbuild /r /p:Configuration=Release $UNO_UITEST_PROJECT
msbuild /r /p:Configuration=Release /p:Platform=iPhoneSimulator /p:IsUiAutomationMappingEnabled=True $UNO_UITEST_IOS_PROJECT 

cd $BUILD_SOURCESDIRECTORY/build

wget $UNO_UITEST_NUGET_URL
mono nuget.exe install NUnit.ConsoleRunner -Version $UNO_UITEST_NUNIT_VERSION

mkdir -p $UNO_UITEST_SCREENSHOT_PATH

mono $BUILD_SOURCESDIRECTORY/build/NUnit.ConsoleRunner.$UNO_UITEST_NUNIT_VERSION/tools/nunit3-console.exe \
   --inprocess --agents=1 --workers=1 \
   $UNO_UITEST_BINARY