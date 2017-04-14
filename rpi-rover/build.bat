cd /D "%~dp0"
..\tools\nuget restore
set PATH=%ProgramFiles%\MSBuild\14.0\Bin;%ProgramFiles(x86)%\MSBuild\14.0\Bin;%PATH%
msbuild /p:Configuration=Release