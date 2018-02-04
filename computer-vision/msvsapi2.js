var request = require('then-request');
var fs = require('fs');
var path = require('path');
 const cognitiveServices = require('cognitive-services');
var body = fs.readFileSync(path.join(__dirname, '87357.jpg'));
 
/*var res = request('POST','https://westcentralus.api.cognitive.microsoft.com/vision/v1.0/analize?visualFeatures=Description,Tags',
		{ 
			headers:{
				'Ocp-Apim-Subscription-Key': '751245ca62bb403aaeaab26f4ed1cbf4',
				'Content-type': 'multipart/form-data'
				},
			body: img

	 
		}).then(function(res1) { console.log(res1.body.toString('utf-8'))}); 
		 */
 const parameters = {
   "visualFeatures":"Description,Tags"
}
  const headers = {
                'Content-type': 'application/octet-stream'
            };

const VisionClient = new cognitiveServices.computerVision({
    apiKey: "751245ca62bb403aaeaab26f4ed1cbf4",
    endpoint: "westcentralus.api.cognitive.microsoft.com"
})

VisionClient.analyzeImage({
    parameters,
	headers,
    body
}).then(response => {
    console.log(response);
})