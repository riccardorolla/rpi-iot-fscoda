#!/bin/sh
cd rpi-rover
sh ./build.sh
cd ..
cd rpi-service
sh ./build.sh
cd ..
cd fsc-rover
sh ./build.sh
cd ..


