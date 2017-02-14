var MsTranslator = require('mstranslator');
// Second parameter to constructor (true) indicates that
// the token should be auto-generated.
var client = new MsTranslator({
  client_id: "skqV8Yw06YdPuyBhthQqB5TDyEbmMNqf6OmKWNGwjnA"
  , client_secret: "skqV8Yw06YdPuyBhthQqB5TDyEbmMNqf6OmKWNGwjnA"
}, true);

var params = {
  text: 'How\'s it going?'
  , from: 'en'
  , to: 'es'
};

// Don't worry about access token, it will be auto-generated if needed.
client.translate(params, function(err, data) {
  console.log(data);
});


app.get('/translate',function(req,res) {
 
	 
 
	 var langsrc = req.query.src;
	 var langdst = req.query.dst;
	 var text = req.query.text;
	 
 
	  translate(text,langsrc,langdst,function(strout) {
												res.send(strout.toString());
												res.end();
											}) 
							  
	});
	




function translate(sourceText,sourceLang,targetLang,callback){
	var qst = qs.stringify({
		client : 'gtx',
		sl : sourceLang,
		tl : targetLang,
		dt : 't',
		q : sourceText
	});
	
	request('GET',configuration.translate_url+'?'+qst,
		{ 
			headers : { 
				'User-Agent': 'Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/49.0.2623.110 Safari/537.36'
			}		

		}).then(function(data){
							  
						 result=JSON.parse(JSON.stringify(data.getBody().toString().trim()));
						 callback(result.split('"')[1])
								 
						 
		 
				}).catch(
					function(err){
						callback (err);
					});
};