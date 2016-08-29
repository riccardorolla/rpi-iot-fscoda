module Fsc.Utils

open System
open System.IO
open FSharp.Data
open Newtonsoft.Json

type Configuration =
 {
  server:string;
  port:int;
  debug:bool
 }


type Message = 
    {
      idmsg:string; 
      txt:string
    }

 
type ImageRecognition =
  { 
    tags:List<Tag>;
    description: Description ;
    requestId: string;
    metadata: Meta
    
  }
and Tag = 
  {
    name:string;
    confidence:double
  }
and Description =
  { 
    tags:List<string>;
    captions: List<Caption>
  }
and Caption =
  { 
    text:string;
    confidence:double
  }
and Meta =
  { 
    width:int;
    height:int;
    format:string
  }
type num=int

  
let confjson =
 try  
  File.ReadAllText(@"fsc-rover.json")
 with
  | :? System.IO.FileNotFoundException  -> 
        File.WriteAllText(@"fsc-rover.json","""{"server":"localhost","port":8081,"debug":false}""")
        """{"server":"localhost","port":8081,"debug":false}"""




let conf = JsonConvert.DeserializeObject<Configuration>(confjson)

printfn "server:%s,port:%i" conf.server conf.port

let urlbuild command =  sprintf "http://%s:%i/%s" conf.server conf.port command


let (|Prefix|_|) (p:string) (s:string) =
    if s.StartsWith(p) then
        Some(s.Substring(p.Length))
    else
        None

let cmdbuild (text : string)  =
 match text with
 | Prefix "go" rest -> sprintf "rpi/motor/%s" ((rest).Trim())
 | Prefix "stop" rest -> sprintf "rpi/motor/stop"
 | Prefix "led" rest -> 
   match ((rest).Trim()) with
   | Prefix "on" rest -> sprintf "rpi/led/%s/on" ((rest).Trim()) 
   | Prefix "off" rest -> sprintf "rpi/led/%s/off" ((rest).Trim()) 
   | _ -> sprintf "nop"
 | Prefix "telegram" rest -> 
   match ((rest).Trim()) with
   | Prefix "photo" rest -> sprintf "telegram/%s/photo" ((rest).Trim()) 
   | Prefix "video" rest -> sprintf "telegram/%s/video" ((rest).Trim()) 
   | Prefix "text" rest -> sprintf "telegram/%s/text" ((rest).Trim()) 
   | Prefix "list" rest -> sprintf "telegram/listchat"
   | Prefix "messages" rest -> sprintf "telegram/%s/msg" ((rest).Trim()) 
   | Prefix "pop" rest -> sprintf "telegram/%s/msg/shift" ((rest).Trim()) 
   | _ -> sprintf "nop"
 | Prefix "get" rest -> 
  match ((rest).Trim()) with
   | Prefix "distance" rest -> sprintf "rpi/distance" 
   | Prefix "photo" rest -> sprintf "rpi/photo" 
   | Prefix "video" rest -> sprintf "rpi/video" 
   | Prefix "messages" rest -> sprintf "telegram/%s/msg" ((rest).Trim()) 
   | Prefix "channels" rest -> sprintf "telegram/listchat"
   | _ -> sprintf "nop"
 | Prefix "whatdoyousee" rest -> sprintf "whatdoyousee"
 | Prefix "discovery" rest -> sprintf "whatdoyousee"
 | Prefix "translate" rest -> sprintf "translate"
 | Prefix "broadcast" rest -> sprintf "telegram/broadcast/%s" ((rest).Trim()) 
 | _ -> sprintf "nop"


 
let command  cmd q =   
    printfn "command %s" cmd
    if (cmd="nop") then sprintf "OK"
    else
     let resp=try 
               Http.RequestString((urlbuild (cmdbuild(cmd))), q , headers = [ "Cache-Control","NoCache" ])
              with 
              | :? System.Net.WebException ->    "error"
     printfn "command %s -> %s" cmd resp
     resp
    
 




let get_messages idchat=
       try
        let msgs = command (sprintf "telegram message %i" idchat)   []
        JsonConvert.DeserializeObject<List<Message>>(msgs)
       with e-> JsonConvert.DeserializeObject<List<Message>>("[]")
   
let get_message idchat =
       try
        let msg=command (sprintf "telegram pop %i" idchat) []
        let out=JsonConvert.DeserializeObject<Message>(msg)
        out.txt.ToLower().Split ' '
    
       with e-> [||]
let get_list str =
      try
     
       JsonConvert.DeserializeObject<List<int>>(str)
      with e ->  [] 


let imagerecognition str =
  try
   JsonConvert.DeserializeObject<ImageRecognition>(str)
  with e ->    JsonConvert.DeserializeObject<ImageRecognition>("{\"tags\":[],\"description\":{\"tags\":[],\"captions\":[]},requestId:\"\",metadata:{width:0,height:0,format:\"null\"}}")

let caption str =  
                try
                 let imageinfo = str |> imagerecognition
                 sprintf  "%s" imageinfo.description.captions.[0].text 
                with e-> "no desc"             
          
  
let send_message idchat   cmd text cap=
          printfn "send_message %s %s %s" idchat cmd text
          let snd= match (cmd) with
                   | Prefix "get photo" rest ->  command (sprintf "telegram photo %s" idchat)
                                                    ["idphoto",text;
                                                     "text",cap]
                   | Prefix "get video" rest ->  command (sprintf "telegram video %s" idchat) 
                                                    ["idvideo",text;
                                                     "text",cap]
                   | Prefix "nop" rest -> sprintf "%s" "nop"
                   | _   -> command  (sprintf "telegram text %s" idchat) [ "text", sprintf "%s -> %s" cmd  text]
       
          snd