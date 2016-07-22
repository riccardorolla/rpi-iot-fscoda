#!/bin/sh
nuget restore
chmod 777 -R *.exe 
xbuild /p:Configuration=Release