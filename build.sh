#!/bin/sh
pushd rpi-rover
sh ./build.sh
popd
pushd  rpi-service
sh ./build.sh
popd
pushd fsc-rover
sh ./build.sh
popd


