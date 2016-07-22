..\tools\nuget restore
call "%VS140COMNTOOLS%\vsvars32.bat"
msbuild /p:Configuration=Release
