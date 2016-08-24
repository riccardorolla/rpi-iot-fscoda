var TelegramBot = require('node-telegram-bot-api');
var uuid = require('node-uuid');
var express = require('express');
var request = require('then-request');
var fs = require('fs');
var path = require('path');
var qs = require('qs');
var childprocess=require('child_process');
var conf;

var filename_conf='rpi-service.json';

var args = process.argv.slice(2);
 console.log('rpi-service args: ', args);

 
switch (args[0]) {
	case "-dev" : 
		filename_conf='rpi-service-dev.json';
		break
	 default:
		 filename_conf='rpi-service.json';
		 break;
}
try {
	conf = fs.readFileSync(filename_conf);
} catch (e) {
	conf = '{"telegram_key":"224831807:AAGNkaCtG-yML_yqw-ZEnU_fvTugyM3D5cM",'+
			    '"vision_url":"https://api.projectoxford.ai/vision/v1.0/analyze",'+
			    '"vision_key":"255ec2de41124b42a6ae6428f7f03b84",'+
				'"translate_url":"http://translate.googleapis.com/translate_a/single",'+
				'"default_lang":"it",'+
				'"port":8081,'+
				'"rover_cmd":"../rpi-rover/bin/rpi-rover.exe",'+
				'"photo_cmd":"raspistill",'+
				'"video_cmd":"raspivid",'+
				'"vconv_cmd":"avconv",'+
				'"temp_path":"/tmp/"}';
	fs.writeFileSync(filename_conf, conf);
 
}
var configuration = JSON.parse(conf);

console.log(configuration);

if (!fs.existsSync(configuration.temp_path)){
    fs.mkdirSync(configuration.temp_path);
}

const execSync = childprocess.execSync;
const execAsync = childprocess.exec;
var app = express();
var lastmsg=[];
var listchat=[];
function getchat(idchat){
		  return listchat.find(function(chat){
						return chat.idchat==idchat;
						});
}
 
var bot = new TelegramBot(configuration.telegram_key, {polling: true});
bot.on('message', function (msg) {
  var retchat = getchat(msg.chat.id);
  if (retchat===undefined) {
				retchat= {
					idchat:msg.chat.id,
					lang:configuration.default_lang,
					msg:[]
				};
				listchat.push(retchat);
  }  
  var newmsg={idmsg:uuid.v4(),txt:msg.text};
  retchat.msg.push(newmsg);
   
    
  console.log(retchat);
});

function whatdoyousee(img,callback,fcallback){
     var response = request('POST',configuration.vision_url+'?visualFeatures=Description,Tags',
						{ 
							headers:{
								'Ocp-Apim-Subscription-Key': configuration.vision_key,
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

function photo(width,height,quality) {
	 var idphoto=uuid.v4(); 
	 var filename = configuration.temp_path+idphoto+'.jpg'
	 console.log('filename:'+filename + ',width:'+width+',height'+height+',quality:'+quality);
	 var cmd = configuration.photo_cmd +' -o ' + filename;
	 if (undefined != width)  cmd = cmd + ' -w ' + width 
		else cmd = cmd + ' -w 640'
	 if (undefined != height) cmd = cmd + ' -h ' + height 
		else cmd = cmd + ' -h 480'
	 if (undefined != quality) cmd = cmd + ' -q ' + quality;
		else cmd = cmd + ' -q 90'

	 console.log(cmd);
	 
	 code = execSync(cmd);
	
	 return idphoto;
}

function video(width,height,time) {
	
	var idvideo=uuid.v4();

	
	var filename = configuration.temp_path+idvideo+'.h264'
	var filenameconv = configuration.temp_path+idvideo+'.mp4'
	console.log('filename:'+filenameconv + ',width:'+width+',height'+height+',time:'+time);
	var cmd = configuration.video_cmd + ' -o ' + filename;
	 if (undefined != width)  cmd = cmd + ' -w ' + width 
		else cmd = cmd + ' -w 640'
	 if (undefined != height) cmd = cmd + ' -h ' + height 
		else cmd = cmd + ' -h 480'
	 if (undefined != time) cmd = cmd + ' -t ' + time;
		else cmd = cmd + ' -t 5000'
	console.log(cmd);
	code = execSync(cmd);
	var vconv_cmd = configuration.vconv_cmd + ' -r 30 -i '+ filename +' -vcodec copy '+ filenameconv;
	console.log(vconv_cmd);
	code2 = execSync(vconv_cmd);
 
	return idvideo;
 
	
}

app.get('/telegram/listchat',function (req,res) {
	var retchat=[];
	for (var i=0; i<listchat.length;i++) {
		retchat.push(listchat[i].idchat);
	}
	res.send(retchat);
	
});
app.get('/telegram/:idchat',function (req, res) {
	var idchat=req.params.idchat
	var chat= getchat(idchat);
	if (req.query.lang!=undefined) chat.lang=req.query.lang;
	 res.send(chat);
 
})
app.get('/telegram/:idchat/msg', function (req, res) {
	var idchat=req.params.idchat
   res.send(getchat(idchat).msg);

})

app.get('/telegram/:idchat/msg/pop',function(req,res) {
	var idchat=req.params.idchat
	res.send(getchat(idchat).msg.pop());
})

app.get('/telegram/:idchat/msg/shift',function(req,res) {
	var idchat=req.params.idchat
	res.send(getchat(idchat).msg.shift());
})
app.get('/telegram/:idchat/video',function(req,res) {
	var idchat=req.params.idchat
	var idvideo=req.query.idvideo
	var msg = req.query.msg;
	var filename = configuration.temp_path+idvideo+'.mp4';
	if (undefined != msg) 
		bot.sendVideo(idchat,filename, {caption: msg});
	else 
		bot.sendVideo(idchat,filename);
	res.send('send video');
 
	
})


app.get('/telegram/:idchat/photo',function(req,res) {
	var idchat=req.params.idchat
	var idphoto=req.query.idphoto
	var filename = configuration.temp_path+idphoto+'.jpg'
	var msg = req.query.msg;
	 
	 
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
	code = execSync(configuration.rover_cmd +" motor "+ req.params.command);
	res.send(code.toString());
	res.end();
});
app.get('/rpi/led/:numled/:command',function(req,res) {
	code = execSync(configuration.rover_cmd +" led  " +req.params.numled + " " + req.params.command);
	res.send(code.toString());
	res.end();
	});
app.get('/rpi/distance/',function(req,res) {
	code = execSync(configuration.rover_cmd +" uds");
	res.send(code.toString());
	res.end();
});
/*
app.get('/rpi/photo',function(req,res) {
	 var filename = photo(req.query.width,req.query.height,req.query.quality);
	 var img = fs.readFileSync(filename);
     res.writeHead(200, {'Content-Type': 'image/jpeg' });
     res.end(img, 'binary');
  
							  
    });
app.get('/rpi/video',function(req,res) {
 
	 var filename = video(req.query.width,req.query.height,req.query.time);
	 var vid = fs.readFileSync(filename);
     res.writeHead(200, {'Content-Type': 'video/mp4' });
     res.end(vid, 'binary');
  
							  
    });
*/
app.get('/rpi/photo',function(req,res) {
	 var idphoto = photo(req.query.width,req.query.height,req.query.quality);
	 res.send(idphoto);
	 res.end();
	  
  
							  
    });
app.get('/rpi/video',function(req,res) {
 
	 var idvideo = video(req.query.width,req.query.height,req.query.time);
	 res.send(idvideo);
	 res.end();
  
							  
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
     var idphoto=req.query.idphoto
	 var filename = configuration.temp_path+idphoto+'.jpg' //photo(req.query.width,req.query.height,req.query.quality);
	 var img = fs.readFileSync(filename);
	 var lang;
	 lang = req.query.lang;
	 if (lang===undefined) lang=configuration.default_lang;
     var resvision = whatdoyousee(img,function(response) {
								             console.log(response.getBody().toString('utf-8')); 
									//	translate(JSON.parse(response.getBody().toString('utf-8')).description.captions[0].text,'en',lang,
										//	function(strout) {
										//		res.send(strout.toString());
										//		res.end();
											//})
											res.send(response.getBody().toString('utf-8'));
											res.end();
											},
								function (err) {
									res.send(err);
									res.end();
								});
							  
    });



var server = app.listen(configuration.port, function () {

  var host = server.address().address
  var port = server.address().port

  console.log("Example app listening at http://%s:%s", host, port)

})