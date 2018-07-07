#!/bin/sh
cd ${PWD}/rpi-rover
nuget restore
chmod 777 -R packages
xbuild /p:Configuration=Release
cd ${PWD}/rpi-service/
npm install
cd ${PWD}/fsc-rover 
nuget restore
chmod 777 -R packages
xbuild /p:Configuration=Release

