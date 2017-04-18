cd /D "%~dp0"
..\tools\nuget restore
set PATH=C:\Program Files (x86)\Microsoft Visual Studio\2017\Community\MSBuild\15.0\Bin;%PATH%
msbuild /p:Configuration=Release