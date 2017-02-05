var express = require('express');
var fs = require('fs');
var path = require('path');
var uuid = require('node-uuid');
var childprocess=require('child_process');
var TelegramBot = require('node-telegram-bot-api');
var request = require('then-request');




var app = express();

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

var conf;
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

/*  API  RPI Hardware
*/


 
const exec = childprocess.exec;

app.get('/rpi/motor/:action',function(req,res) {
	 exec(configuration.rover_cmd +" motor " + req.params.action,
			(error,stdout,stderr)=> {
				if (error) {
   					res.send("{error:'"+ error + "'");	
				}else{
					res.send(stdout);
				}
			res.end();
  });
});

app.get('/rpi/led/:numled/:action',function(req,res) {
	 exec(configuration.rover_cmd +" led  " +req.params.numled + " " + req.params.action,
			(error,stdout,stderr)=> {
				if (error) {
   					res.send("{error:'"+ error + "'");
				}else{
					res.send(stdout);
				}
			res.end();
  });
});
app.get('/rpi/distance/',function(req,res) {
     exec(configuration.rover_cmd +" uds",
			(error,stdout,stderr)=> {
				if (error) {
   					res.send("{error:'"+ error + "'");
				}else{
					res.send(stdout);
				}
			res.end();
  });
});


app.get('/rpi/photo',function(req,res) {
	 var idphoto=uuid.v4(); 
	 exec(photo_cmd(idphoto,req.query.width,req.query.height,req.query.quality),
			(error,stdout,stderr)=> {
				if (error) {
 				   res.send("{error:'"+ error + "'");
				}else{
					res.send(idphoto);
				}
			res.end();
  });
});

app.get('/rpi/video',function(req,res) {
	 var idvideo=uuid.v4(); 
	 exec(video_cmd(idvideo,req.query.width,req.query.height,req.query.quality),
			(error,stdout,stderr)=> {
				if (error) {
   						res.send("{error:'"+ error + "'");
					res.end();
				}else{
					exec(vconv_cmd(idvideo),
						(verror,vstdout,vstderr)=> {
							if (verror) {
  								 res.send("{error:'"+ error + "'");
							}else{
								res.send(idvideo);
							}
							res.end();
 			 			});
				}
			  });
});


function photo_cmd(idphoto,width,height,quality) {
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
	 
	  
	
	 return cmd;
}

function video_cmd(idvideo,width,height,time) {
	
 
	
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

 
	return cmd;
	
}

function vconv_cmd(idvideo){
	
 
	
	var filename = configuration.temp_path+idvideo+'.h264'
	var filenameconv = configuration.temp_path+idvideo+'.mp4'

	var  cmd = configuration.vconv_cmd + ' -r 30 -i '+ filename +' -vcodec copy '+ filenameconv;
	console.log(cmd);
 
	return cmd;
	
}

// API e Feature Telegram

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

var listchat=[];
function getchat(idchat){
		  return listchat.find(function(chat){
						return chat.idchat==idchat;
						});
}
 
app.get('/telegram/listchat',function (req,res) {
	var retchat=[];
	for (var i=0; i<listchat.length;i++) {
		retchat.push(listchat[i].idchat);
	}
	res.send(retchat);
	res.end();
});
app.get('/telegram/broadcast',function(req,res) {
  
 var txt = req.query.text ;
	for (var i=0; i<listchat.length;i++) {
		bot.sendMessage(listchat[i].idchat,txt);
	}
 
 res.send('send broadcast msg');
 res.end();
});

app.get('/telegram/:idchat',function (req, res) {
	var idchat=req.params.idchat
	var chat= getchat(idchat);
    res.send(chat);
	
 	res.end();
})
app.get('/telegram/:idchat/next',function(req,res) {
	var idchat=req.params.idchat
	res.send(getchat(idchat).msg.shift());
	res.end();
})

app.get('/telegram/:idchat/text',function (req, res) {
	var idchat=req.params.idchat
	 var msg = req.query.text ;
	var chat= getchat(idchat);
	if (undefined != msg)  { 
				bot.sendMessage(idchat,msg)
				res.send('send msg')
		}
	else { 	
		res.send('no send msg');
	}
 	res.end();
})


app.get('/telegram/:idchat/video',function(req,res) {
	var idchat=req.params.idchat
	var idvideo=req.query.idvideo
	var msg = req.query.text;
	var filename = configuration.temp_path+idvideo+'.mp4';
	if (undefined != msg) 
		bot.sendVideo(idchat,filename, {caption: msg});
	else 
		bot.sendVideo(idchat,filename);
	res.send('send video');
   res.end();
	
})


app.get('/telegram/:idchat/photo',function(req,res) {
	var idchat=req.params.idchat
	var idphoto=req.query.idphoto
 
	var filename = configuration.temp_path+idphoto+'.jpg'
	var msg = req.query.text;
	bot.sendPhoto(idchat, filename, {caption:msg})
	res.send('send photo')
	 res.end();
							  
	});
 
// Proxy - Computer Vision
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

app.get('/whatdoyousee',function(req,res) {
	 var idphoto=req.query.idphoto
	 var filename = configuration.temp_path+idphoto+'.jpg'  
	 var img = fs.readFileSync(filename);
 
	 var resvision = whatdoyousee(img,function(response) {
											console.log(response.getBody().toString('utf-8')); 
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
