cd /D "%~dp0"
call npm install

node rpi-service.js -dev
 