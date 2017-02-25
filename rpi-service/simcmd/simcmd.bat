@echo off

set /A time=%RANDOM%/2000
set /A distance=%RANDOM%/1000
set /A decdis=%RANDOM%/100
set /A readbutton=%RANDOM%/32000
ping 127.0.0.1 -n %time% > NUL 
if "%1"=="video" copy /Y "%~dp0\example\file.h264" "%3"
if "%1"=="photo" copy /Y "%~dp0\example\file.%1" "%3"
if "%1"=="uds"  ( 
			echo %distance%.%decdis%
			goto end
		   ) 
if "%1"=="button"  ( 	
			echo %readbutton%
			) else (
			echo OK
			) 
 
:end