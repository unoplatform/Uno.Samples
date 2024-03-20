#!/bin/bash
export BUILD_SOURCESDIRECTORY=`pwd`/../..
export BUILD_ARTIFACTSTAGINGDIRECTORY=/tmp/uno-uitests-results

./ios-uitest-run.sh
