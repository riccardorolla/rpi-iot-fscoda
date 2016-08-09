@ECHO OFF
set SUBKEY=255ec2de41124b42a6ae6428f7f03b84
curl -k -v -X POST "https://api.projectoxford.ai/vision/v1.0/analyze?visualFeatures=Description" -H "Content-Type: multipart/form-data" -H "Ocp-Apim-Subscription-Key: %SUBKEY%" -F "body=@./home-cat.jpg" 