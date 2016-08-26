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
   | _ -> sprintf "neither"
 | Prefix "telegram" rest -> 
   match ((rest).Trim()) with
   | Prefix "photo" rest -> sprintf "telegram/%s/photo" ((rest).Trim()) 
   | Prefix "video" rest -> sprintf "telegram/%s/video" ((rest).Trim()) 
   | Prefix "text" rest -> sprintf "telegram/%s/text" ((rest).Trim()) 
   | Prefix "list" rest -> sprintf "telegram/listchat"
   | Prefix "message" rest -> sprintf "telegram/%s/msg" ((rest).Trim()) 
   | Prefix "pop" rest -> sprintf "telegram/%s/msg/shift" ((rest).Trim()) 
   | _ -> sprintf "neither"
 | Prefix "get" rest -> 
  match ((rest).Trim()) with
   | Prefix "distance" rest -> sprintf "rpi/distance" 
   | Prefix "photo" rest -> sprintf "rpi/photo" 
   | Prefix "video" rest -> sprintf "rpi/video" 
   | _ -> sprintf "neither"
 | Prefix "whatdoyousee" rest -> sprintf "whatdoyousee"
 | Prefix "translate" rest -> sprintf "translate"
 | _ -> sprintf "nop"


let command  cmd q   =
    printfn "command %s" cmd
    if (cmd="nop") then sprintf "OK"
    else
     let resp=try 
               Http.RequestString((urlbuild (cmdbuild(cmd))), q , headers = [ "Cache-Control","NoCache" ])
              with 
              | :? System.Net.WebException ->    "error"
     printfn "command %s -> %s" cmd resp
     resp
    
 



type Message = 
   {
     idmsg:string; 
     txt:string
   }
let get_messages idchat=
      try
       let msgs = command (sprintf "telegram message %i" idchat)   []
       JsonConvert.DeserializeObject<List<Message>>(msgs)
      with e-> JsonConvert.DeserializeObject<List<Message>>("[]")
  
let get_message idchat =
      try
       let msg=command (sprintf "telegram pop %i" idchat) []
       let out=JsonConvert.DeserializeObject<Message>(msg)
       out.txt.ToLower()
  
      with e-> "nop"
let chats str =
     try
      JsonConvert.DeserializeObject<List<int>>(str)
     with e ->    JsonConvert.DeserializeObject<List<int>>("[]")

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


let imagerecognition str =
  try
   JsonConvert.DeserializeObject<ImageRecognition>(str)
  with e ->    JsonConvert.DeserializeObject<ImageRecognition>("{\"tags\":[],\"description\":{\"tags\":[],\"captions\":[]},requestId:\"\",metadata:{width:0,height:0,format:\"null\"}}")
  