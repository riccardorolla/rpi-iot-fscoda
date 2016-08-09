var request = require('sync-request');
var fs = require('fs');
var path = require('path');
 
var img = fs.readFileSync(path.join(__dirname, '87357.jpg'));
 
var res = request('POST','https://api.projectoxford.ai/vision/v1.0/analyze?visualFeatures=Description',
		{ 
			headers:{
				'Ocp-Apim-Subscription-Key': '255ec2de41124b42a6ae6428f7f03b84',
				'Content-type': ' application/octet-stream'
				},
			body: img

	 
		}); 
console.log(JSON.parse(res.body.toString('utf-8')).description.captions[0].text);