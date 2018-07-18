var fs = require('fs');     //utilizzati per gestire i file
var path = require('path'); //e i nome dei file e directory

//Lettura del file di configurazione
var filename_conf='rpi-service.json';
var args = process.argv.slice(2); //ottiene i parametri a console  passati a node
console.log('rpi-service args: ', args);
switch (args[0]) {//se e' passato la il parametro "-dev" 
	case "-dev" : //viene  utilizzata la configurazione 'rpi-service-dev.json'
		filename_conf='rpi-service-dev.json';
		break
	 default: //altrimenti 'rpi-service.json'
		 filename_conf='rpi-service.json';
		 break;
}

var conf;
try { //legge il file di configurazione
	conf = fs.readFileSync(filename_conf);
} catch (e) {  //se non presente lo crea con dei valori di default
	conf = '{"telegram_key":"224831807:AAGNkaCtG-yML_yqw-ZEnU_fvTugyM3D5cM",'+
		'"vision_endpoint":"westcentralus.api.cognitive.microsoft.com",'+
		'"vision_key":"255ec2de41124b42a6ae6428f7f03b84",'+
		'"default_lang":"it",'+
		'"port":8081,'+
		'"rover_cmd":"../rpi-rover/bin/rpi-rover.exe",'+
		'"photo_cmd":"raspistill",'+
		'"video_cmd":"raspivid",'+
		'"vconv_cmd":"avconv",'+
		'"temp_path":"/tmp/"}';
	fs.writeFileSync(filename_conf, conf);
 
}//inserisce la configurazione in una struttura dati
var configuration = JSON.parse(conf);

console.log(configuration);
//se non presente la directory temporanea usate per la cache dei video e immagini
// viene creta
if (!fs.existsSync(configuration.temp_path)){
	fs.mkdirSync(configuration.temp_path);
}

var express = require('express'); //per il servizio http
var uuid = require('node-uuid'); //per generare UUID

var TelegramBot = require('node-telegram-bot-api'); //semplifica uso telegram


//Lista Chat
var listchat=[];

//funzione che data un identificativo chat restiutisce la chat
function getchat(idchat){
		  return listchat.find(function(chat){
						return chat.idchat==idchat;
						});
}
//instanzio il Bot telegram appasando la key 
var bot = new TelegramBot(configuration.telegram_key, {polling: true});
bot.on('message', function (msg) { //al momento di ricezione di un messagio
  var retchat = getchat(msg.chat.id); //ottengo la chat id  dalla Lista Chat
  if (retchat===undefined) { //se non presente la creo
				retchat= {
					idchat:msg.chat.id,
					lang:configuration.default_lang,
					msg:[]
				}; //la inserisco nella lista chat
				listchat.push(retchat);
  }  
  var newmsg={idmsg:uuid.v4(),txt:msg.text};//assegno al messagio un uuid e
  retchat.msg.push(newmsg);					//e lo metto nella chat
  console.log(retchat);
});

//servizio HTTP
var app = express();
//avvio il servizio sulla porta definita nella configurazione
var server = app.listen(configuration.port, function () {

  var host = server.address().address;
  var port = server.address().port;

  console.log("rpi-service.js at http://%s:%s", host, port)

})

//  API  RPI Hardware
var childprocess=require('child_process'); //per avviare processi esterni
//funzione utilzzata per eseguire i comandi
const exec = childprocess.exec;
var motorout="OK"
var motorrun = false;
app.get('/rpi/motor/:action',function(req,res) {
			 
	res.send(motorout);
	res.end;
	if (!motorrun) {
		motorun=true;
		exec(configuration.rover_cmd +" motor " + req.params.action,
			(error,stdout,stderr)=> {
				if (error) { 	motorout="error:'"+ error + "'";}
				else{  motorout=stdout;	 }
				motorrun=false;
			 });
		} 
	

});
var buttonout="0"
var buttonrun=false;
app.get('/rpi/button/:numbutton',function(req,res) {
	res.send(buttonout);
	res.end();
	if (!buttonrun)  {
		buttonrun=true;
		exec(configuration.rover_cmd +" button  " +req.params.numbutton ,
			(error,stdout,stderr)=> {
				if (error) { buttonout="0"}
				else{  buttonout=stdout}
				buttomrun=false;
		}); 
		
	} 

});
var ledout=["OK","OK"]
var runled=[false,false]
app.get('/rpi/led/:numled/:action',function(req,res) {
	var n = req.params.numled;
	res.send(ledout[n]);
	res.end();
	if (!runled[n]) {
			runled[n]=true;

    exec(configuration.rover_cmd +" led  " +req.params.numled + " " + req.params.action,
			(error,stdout,stderr)=> {
				if (error) { ledout[n]="error:'"+ error + "'";}
				else{ ledout[n]=stdout}
				runled[n]=false;
			});
	}
});
 
var distance="0.0";
var udsrun=false;
app.get('/rpi/distance/',function(req,res) {
	res.send(distance.replace(',','.'));
	res.end();
	if (!udsrun) {
			udsrun=true;
     exec(configuration.rover_cmd +" uds",
			(error,stdout,stderr)=> {
				if (error) { 
					distance="0.0";
				}else{
					distance=stdout;
				}
				udsrun=false;
		});
	} 

});

//  API  RPI Photo
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

//  API  RPI Video
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
});
app.get('/telegram/:idchat/next',function(req,res) {
	var idchat=req.params.idchat
	res.send(getchat(idchat).msg.shift());
	res.end();
});
app.get('/telegram/:idchat/text',function (req, res) {
	var idchat=req.params.idchat
	var msg = req.query.text ;
	var chat= getchat(idchat);
	if (undefined != msg)  { 
				bot.sendMessage(idchat,msg,{parse_mode: "Markdown"});
				res.send('send msg');
	} else { 	
		res.send('no send msg');
	}
 	res.end();
});

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
	
});
app.get('/telegram/:idchat/photo',function(req,res) {
	var idchat=req.params.idchat
	var idphoto=req.query.idphoto
	var filename = configuration.temp_path+idphoto+'.jpg'
	var msg = req.query.text;
	bot.sendPhoto(idchat, filename, {caption:msg})
	res.send('send photo')
	res.end();  
});
	

const cognitiveServices = require('cognitive-services');
const parameters = {
   "visualFeatures":"Description,Tags"
}
const headers = {
                'Content-type': 'application/octet-stream'
            };
const VisionClient = new cognitiveServices.computerVision({
    apiKey: configuration.vision_key,
    endpoint: configuration.vision_endpoint
})

app.get('/whatdoyousee',function(req,res) {
	 var idphoto=req.query.idphoto

	 var filename = configuration.temp_path+idphoto+'.jpg'  
	 var body = fs.readFileSync(filename);
 
		VisionClient.analyzeImage({
			parameters,
			headers,
			body
		}).then(response => {
			console.log(response);
			res.send(response);
			res.end();
		});
 
	
});

