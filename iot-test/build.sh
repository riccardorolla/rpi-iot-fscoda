#!/bin/sh
nuget restore
chmod 777 -R packages
xbuild /p:Configuration=Release