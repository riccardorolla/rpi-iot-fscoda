var TelegramBot = require('node-telegram-bot-api');
var uuid = require('node-uuid');
var express = require('express');
var request = require('then-request');
var fs = require('fs');
var path = require('path');
var qs = require('qs');
var childprocess=require('child_process');
const execSync = childprocess.execSync;
const execAsync = childprocess.exec;
var app = express();
var telegram_key = '224831807:AAGNkaCtG-yML_yqw-ZEnU_fvTugyM3D5cM';
var lastmsg=[];
var vision_key='255ec2de41124b42a6ae6428f7f03b84'
 
var bot = new TelegramBot(telegram_key, {polling: true});
bot.on('message', function (msg) {
  var chatId = msg.chat.id;
  var idmsg=uuid.v4();
  var textmsg=msg.text;
  lastmsg.push({idmsg,chatId,textmsg});

   console.log("chatId:"+chatId);
});

function whatdoyousee(img,callback,fcallback){
     var response = request('POST','https://api.projectoxford.ai/vision/v1.0/analyze?visualFeatures=Description',
						{ 
							headers:{
								'Ocp-Apim-Subscription-Key': visionkey,
								'Content-type': ' application/octet-stream'
								},
							body:img

						}).then(callback).catch(fcallback);
	return response;
}
function translate(sourceText,sourceLang,targetLang,callback){
	var qst = qs.stringify({
		client : 'gtx',
		sl : sourceLang,
		tl : targetLang,
		dt : 't',
		q : sourceText
	});
	
	request('GET','http://translate.googleapis.com/translate_a/single?'+qst,
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

function photo(width,height,quality) {
	 var idphoto=uuid.v4();
	 console.log('width:'+width+',height'+height+',quality:'+quality);
	 if (undefined != width)  cmd = cmd + ' -w ' + width 
	 if (undefined != height) cmd = cmd + ' -h ' + height 
	 if (undefined != quality) cmd = cmd + ' -q ' + quality;
	 var filename = '/tmp/'+idphoto+'.jpg'
	 var cmd = 'raspistill -o ' + filename;
	 console.log(cmd);
	 
	 code = execSync(cmd);
	 var img = fs.readFileSync(filename);
	 return img;
}


app.get('/telegram/receive', function (req, res) {
   res.send(lastmsg);
 //  lastmsg='';
})

app.get('/telegram/pop',function(req,res) {
	res.send(lastmsg.pop());
})

app.get('/telegram/shift',function(req,res) {
	res.send(lastmsg.shift());
})
app.get('/telegram/:idchat/video',function(req,res) {
	var idchat=req.params.idchat
	var idvideo=uuid.v4();
	bot.sendMessage(idchat,"sending video...");
	var width = req.query.width;
	var height = req.query.height;
	var time = req.query.time;
	var msg = req.query.msg;
	
	var filename = '/tmp/'+idvideo+'.h264'
	var cmd = 'raspivid -o ' + filename;
	if (undefined != width)  cmd = cmd + ' -w ' + width;
	if (undefined != height) cmd = cmd + ' -h ' + height; 
	if (undefined != time) cmd = cmd + ' -t ' + time; 
	console.log(cmd);
	code = execSync(cmd);
	code2 = execSync('avconv -r 30 -i /tmp/'+idvideo+'.h264 -vcodec copy /tmp/'+idvideo+'.mp4');
	if (undefined != msg) 
		bot.sendVideo(idchat,'/tmp/'+idvideo+'.mp4', {caption: msg});
	else 
		bot.sendVideo(idchat,'/tmp/'+idvideo+'.mp4');
	res.send('send video');
 
	
})


app.get('/telegram/:idchat/photo',function(req,res) {
	var idchat=req.params.idchat
	 var img = photo(req.query.width,req.query.height,req.query.quality);
	 
	 var msg = req.query.msg;
	 
     bot.sendMessage(idchat,"sending photo...");
	 
     var resvision =  whatdoyousee(img,
							function(response) {
								console.log(response.getBody().toString('utf-8')); 
								translate(JSON.parse(response.getBody().toString('utf-8')).description.captions[0].text,'en','it',function(strout) {
										bot.sendPhoto(idchat, filename, {caption:strout});
										res.send('send photo:'+strout);
										res.end();
								});
							},
							function (err) {
									res.send(err);
									res.end();
								});
							  
    });
 
							

app.get('/telegram/:idchat/text',function(req,res) {
 var idchat = req.params['idchat'];
 var txt = req.query.text ;
 bot.sendMessage(idchat,txt);
 res.send('send msg');
});

app.get('/rpi/motor/:command',function(req,res) {
	code = execSync("../rpi-rover/bin/rpi-rover.exe motor "+ req.params.command);
	res.send(code.toString());
	res.end();
});
app.get('/rpi/led/:numled/:command',function(req,res) {
	code = execSync("../rpi-rover/bin/rpi-rover.exe led  " +req.params.numled + " " + req.params.command);
	res.send(code.toString());
	res.end();
	});
app.get('/rpi/distance/',function(req,res) {
		code = execSync("../rpi-rover/bin/rpi-rover.exe uds");
	res.send(code.toString());
	res.end();
});
app.get('/rpi/photo',function(req,res) {
	 var width = req.query.width;
	 var height = req.query.height;
	 var quality = req.query.quality;
	 var img = photo(width,height,quality);
     res.writeHead(200, {'Content-Type': 'image/jpeg' });
     res.end(img, 'binary');
  
							  
    });
app.get('/rpi/video',function(req,res) {
	var idchat=req.params.idchat
	 var idphoto=uuid.v4();
	 var width = req.query.width;
	 var height = req.query.height;
	 var quality = req.query.quality;
	 var msg = req.query.msg;
	 var filename = '/tmp/'+idphoto+'.jpg'
	 var cmd = 'raspistill -o ' + filename;
 
	 if (undefined != width)  cmd = cmd + ' -w ' + width 
	 if (undefined != height) cmd = cmd + ' -h ' + height 
	 if (undefined != quality) cmd = cmd + ' -q ' + quality;
	 //if (undefined != time) cmd = cmd + ' -t ' + time; 
	 console.log(cmd);
	 
	 code = execSync(cmd);
	 var img = fs.readFileSync(filename);
     res.writeHead(200, {'Content-Type': 'image/jpeg' });
     res.end(img, 'binary');
  
							  
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
app.get('/whatdoyousee',function(req,res) {
 
	 var idphoto=uuid.v4();
	 var width = 640;
	 var height = 480;
	 var quality = 95;
 
	 var lang = req.query.lang;
	 var filename = '/tmp/'+idphoto+'.jpg'
	 var cmd = 'raspistill -o ' + filename;
 
	 if (undefined != width)  cmd = cmd + ' -w ' + width 
	 if (undefined != height) cmd = cmd + ' -h ' + height 
	 if (undefined != quality) cmd = cmd + ' -q ' + quality;
	 
	 console.log(cmd);
	 
	 code = execSync(cmd)
	 var img = fs.readFileSync(filename);
     var resvision = whatdoyousee(img,function(response) {
										console.log(response.getBody().toString('utf-8')); 
										translate(JSON.parse(response.getBody().toString('utf-8')).description.captions[0].text,'en',lang,
											function(strout) {
												res.send(strout.toString());
												res.end();
											})},
								function (err) {
									res.send(err);
									res.end();
								});
							  
    });



var server = app.listen(8081, function () {

  var host = server.address().address
  var port = server.address().port

  console.log("Example app listening at http://%s:%s", host, port)

})