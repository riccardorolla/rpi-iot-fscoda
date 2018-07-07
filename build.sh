#!/bin/sh
chmod o+x ${PWD}/rpi-rover
chmod o+x ${PWD}/rpi-service/
chmod o+x ${PWD}/fsc-rover 
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

