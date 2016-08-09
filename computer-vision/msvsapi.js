var request = require('request');
var fs = require('fs');
var path = require('path');
 var qs = require('qs');
function translate(sourceText,sourceLang,targetLang,callback){
var qst = qs.stringify({
    client : 'gtx',
    sl : sourceLang,
    tl : targetLang,
    dt : 't',
    q : sourceText
});
var options = {
    uri: 'http://translate.googleapis.com/translate_a/single?'+qst,
    headers : { 
        'User-Agent': 'Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/49.0.2623.110 Safari/537.36'
    }
};
request.get(options).on('response',function(response){
    response.on('data',function(data){
        result=JSON.parse(JSON.stringify(data.toString().trim()));
		callback(result.split('"')[1])
		 
    });
});}
var req = request.post({url:'https://api.projectoxford.ai/vision/v1.0/analyze?visualFeatures=Description',headers:{
		'Ocp-Apim-Subscription-Key': '255ec2de41124b42a6ae6428f7f03b84'
		}}, requestCallback);
var form = req.form();
form.append('body', fs.createReadStream(path.join(__dirname, '87357.jpg')));
		
 
function requestCallback(err, httpResponse, body) {
  if (err) {
    return console.error('upload failed:', err);
  }
  console.log(JSON.parse(body).description.captions[0].text);
	translate(JSON.parse(body).description.captions[0].text,'en','it',function(res) {
		console.log(res);
	});
}
