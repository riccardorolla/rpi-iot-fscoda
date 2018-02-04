@ECHO OFF
set SUBKEY=751245ca62bb403aaeaab26f4ed1cbf4
curl -k -v -X POST "https://westcentralus.api.cognitive.microsoft.com/vision/v1.0/analyze?visualFeatures=Description,Tags" -H "Content-Type: multipart/form-data" -H "Ocp-Apim-Subscription-Key: %SUBKEY%" -F "body=@./home-cat.jpg" 