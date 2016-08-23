[<CoDa.Code>]
module Fsc.App
 
open System
open System.IO
open CoDa.Runtime
open Fsc.Types
open FSharp.Data
open Newtonsoft.Json
//open Fsc.Facts


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
   | Prefix "pop" rest -> sprintf "telegram/%s/msg/pop" ((rest).Trim()) 
   | _ -> sprintf "neither"
 | Prefix "get" rest -> 
  match ((rest).Trim()) with
   | Prefix "distance" rest -> sprintf "rpi/distance" 
   | Prefix "photo" rest -> sprintf "rpi/photo" 
   | Prefix "video" rest -> sprintf "rpi/video" 
   | _ -> sprintf "neither"
 | Prefix "whatdoyousee" rest -> sprintf "whatdoyousee"
 | Prefix "translate" rest -> sprintf "translate"
 | _ -> sprintf "neither"


type Command<'T>(cmd :string , value, ?q0) =
 let q = defaultArg q0 []
 let command = cmd
 let out = 
     let resp=try 
               Http.RequestString((urlbuild (cmdbuild(cmd))), q,  silentHttpErrors = true)
              with 
              | :? System.Net.WebException -> value
     try 
      JsonConvert.DeserializeObject<'T>(sprintf "%s" (resp))       
     with 
     | :? Newtonsoft.Json.JsonReaderException -> 
      JsonConvert.DeserializeObject<'T>(JsonConvert.SerializeObject(resp)) 

 member this.output 
   with get() =  

       out
  
 
type ImageRecognition =
 { 
   tags:List<Tag>;
   description: Descrition ;
   requestId: string;
   metadata: Meta
 }
and Tag = 
 {
   name:string;
   confidence:double
 }
and Descrition =
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

        


type Message = 
 {
   idmsg:string; 
   txt:string
 } 

let get_out  cmd = 
      try 
          let dr = ctx?out |- (execute(cmd,ctx?out))
          dr
      with e-> ""
let get_req idchat =
     try 
         let dr = ctx?cmd |- (execute(idchat,ctx?cmd))
         dr
     with e-> ""


let get_observe obj  =
    try
        let s = ctx?status |- observe(obj,ctx?status)
        bool.Parse(s)
     with e-> false


let display_request idchat cmd =
  match ctx with
  | _ when !- request(idchat, cmd) -> printfn " - %s" cmd
  | _ -> printfn " - %s (this chat not execute cmd)" cmd

let display_out cmd out =
  match ctx with
  | _ when !- execute(cmd, out) -> printfn " - %s" out
  | _ -> printfn " - %s (this cmd not output)" out
let is_obstacle distance =
    if (distance<0.3) then "true"
     else "false" 
let is_confidence confidence =
    if (confidence>0.9) then "true"
     else "false"


let new_execute cmd out =
    for _ in !-- execute(cmd,ctx?out) do
     retract <| Fsc.Facts.execute(cmd, ctx?out) 
    tell <| Fsc.Facts.execute(cmd,out)

let new_observe name status =
    for _ in !-- observe(name,ctx?status) do
     retract <| Fsc.Facts.observe(name, ctx?status) 
    tell <| Fsc.Facts.observe(name,status)

[<CoDa.ContextInit>]
let initFacts () =
 
 tell <| Fsc.Facts.execute("get distance","0")
 tell <| Fsc.Facts.observe("obstacle","true")
[<CoDa.Context("fsc-ctx")>]
[<CoDa.EntryPoint>]
let main () =
 initFacts ()
 display_out "led on 1" "OK"
 let mutable continueLooping = true
 while (continueLooping) do

  let  distance = float (new Command<string>("get distance","0.0")).output
  do new_execute "get distance"  (sprintf "%f" distance)
  do new_observe "obstacle"  (is_obstacle distance)
 
 
  let descimg = (new Command<ImageRecognition>("whatdoyousee","{\"tags\":[],\"description\":{\"tags\":[]}")).output
  let tags = descimg.tags

  for tag in  tags do
    do new_observe tag.name (is_confidence tag.confidence)
    
  
  let listchat = (new Command<List<int>>("telegram list","[]")).output
  for idchat in listchat do 
     let messages = (new Command<List<Message>>(sprintf "telegram message %i" idchat,"[]")).output
     let nMessages = messages |> Seq.length
   //  printfn "\t\tnMessages:%i" nMessages
     if (nMessages>0) then
      let msg = (new Command<Message>(sprintf "telegram pop %i" idchat,"{}")).output

      let cmd = msg.txt.ToLower()
      tell <| Fsc.Facts.request(sprintf "%i" idchat,cmd)
      do new_execute cmd ((new Command<string>(cmd,"")).output).Trim()
      let output =
     //  if (validate outcmd) then
        (new Command<string>((sprintf "telegram text %i" idchat),"",["text", (sprintf "%s" outcmd)])).output
      // else
      //  (new Command<string>((sprintf "telegram text %i" idchat),"",["text", "not validate"])).output
      printfn "\t\t\tidmsg:%s,txt:%s,outcmd:%s,output:%s" msg.idmsg  msg.txt outcmd output
do if (conf.debug) then debug()
    else run()