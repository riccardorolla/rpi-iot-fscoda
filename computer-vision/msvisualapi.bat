@ECHO OFF
set SUBKEY=d10808d16c594fd09a5519e5b03b4e7d
curl -k -v -X POST "https://westcentralus.api.cognitive.microsoft.com/vision/v1.0/analyze?visualFeatures=Description,Tags" -H "Content-Type: multipart/form-data" -H "Ocp-Apim-Subscription-Key: %SUBKEY%" -F "body=@./home-cat.jpg" 