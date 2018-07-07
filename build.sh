#!/bin/bash
chmod o+x ${PWD}/rpi-rover
chmod o+x ${PWD}/rpi-service
chmod o+x ${PWD}/fsc-rover 
cd ../rpi-rover
nuget restore
chmod 777 -R packages
xbuild /p:Configuration=Release
cd ../rpi-service
npm install
cd ../fsc-rover 
nuget restore
chmod 777 -R packages
xbuild /p:Configuration=Release

