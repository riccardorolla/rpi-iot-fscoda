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
    
 

//let command cmd default_out = command cmd default_out []
  //member this.output 
//   with get() =  

     
  
 
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
         let dr = ctx?cmd |- (request(idchat,ctx?cmd))
         dr
     with e-> ""


let get_observe obj  =
    try
        let s = ctx?status |- observe(obj,ctx?status)
        printfn "get_objerve %s -> %s" obj s
        s
     with e-> "false"

let get_nextcmd obj = 
    try 
         let cmd = ctx?cmd |- next(obj,ctx?cmd)
         cmd
        
    with e -> "nop"

let get_response idchat =
    try
     let out = ctx?out |- (response(sprintf "%i" idchat,ctx?out))
     out
    with e-> "error"

let display_request idchat cmd =
  match ctx with
  | _ when !- request(idchat, cmd) -> printfn " - %s" cmd
  | _ -> printfn " - %s (this chat not execute cmd)" cmd

let display_out cmd out =
  match ctx with
  | _ when !- execute(cmd, out) -> printfn " - %s" out
  | _ -> printfn " - %s (this cmd not output)" out
let is_obstacle distance =
    printfn "is_osbtacle %f" distance
    if (distance<0.3) then "true"
     else "false" 
let is_confidence confidence =
    printfn "is_confidence %f" confidence
    if (confidence>0.9) then "true"
     else "false"


let new_execute cmd  =
    printfn "new_execute %s" cmd 
    for _ in !-- execute(cmd,ctx?out) do
     retract <| Fsc.Facts.execute(cmd, ctx?out) 
    if (cmd="whatdoyousee") then 
     tell <| Fsc.Facts.execute(cmd,sprintf "%s" (command cmd ["idphoto",get_out "get photo"]))
    else
     tell <| Fsc.Facts.execute(cmd,sprintf "%s" (command  cmd  []))

let new_observe name status =
    printfn "new_observe %s %s" name status
    for _ in !-- observe(name,ctx?status) do
     retract <| Fsc.Facts.observe(name, ctx?status) 
    tell <| Fsc.Facts.observe(name,status)
 
let chats str =
    try
     JsonConvert.DeserializeObject<List<int>>(str)
    with e ->    JsonConvert.DeserializeObject<List<int>>("[]")
 
let imagerecognition str =
 try
  JsonConvert.DeserializeObject<ImageRecognition>(str)
 with e ->    JsonConvert.DeserializeObject<ImageRecognition>("{\"tags\":[],\"description\":{\"tags\":[],\"captions\":[]},requestId:\"\",metadata:{width:0,height:0,format:\"null\"}}")
 
 
let get_idphoto =
    try 
       get_out "get photo"
    with e -> ""
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
   
let send_message idchat cmd text:string=
    printfn "send_message %i %s %s" idchat cmd text
    let snd= match (cmd) with
             | Prefix "get photo" rest -> command (sprintf "telegram photo %i" idchat)
                                              ["idphoto",get_out "get photo";
                                               "text",((get_out "whatdoyousee") |> imagerecognition).description.captions.[0].text]
             | Prefix "get video" rest -> command (sprintf "telegram video %i" idchat) 
                                              ["idvideo",get_out "get video";
                                               "text",(get_out "whatdoyousee" |> imagerecognition).description.captions.[0].text ]
             | _   -> command  (sprintf "telegram text %i" idchat) [ "text", sprintf "%s -> %s" cmd text]
 
    snd
 


let process_chat idchat =
     printfn "process_chat %i" idchat 
     let cmd= match (get_message idchat) with
              | Prefix "photo" rest -> sprintf "get photo"  
              | Prefix "video" rest -> sprintf "get video"  
              | Prefix "distance" rest -> sprintf "get distance"
              | Prefix "left" rest -> sprintf "go left"
              | Prefix "forward" rest -> sprintf "go forward"
              | Prefix "backward" rest -> sprintf "go backward"
              | Prefix "right" rest -> sprintf "go right"
              | Prefix "nop" rest -> sprintf "nop"
              | _ -> "help"
     printfn "process_chat %i {%s}" idchat cmd
     match (cmd) with
       | "help" ->
        let snd=send_message idchat cmd (sprintf "you can use this command:\n\tphoto\n\tvideo\n\tdistance\n\tleft\n\tright\n\tforward\n\tbackward\n")
        sprintf "%s" snd
       | "nop" -> sprintf "nop"
       | _ -> 
        tell <| Fsc.Facts.request(sprintf "%i" idchat,cmd)       
        do new_execute cmd
        let snd=send_message idchat cmd (get_response idchat)
        retract<|Fsc.Facts.request(sprintf "%i" idchat, cmd) 
        sprintf "%s" snd

[<CoDa.ContextInit>]
let initFacts () =
 tell <| Fsc.Facts.request("0","nop")
 tell <| Fsc.Facts.execute("get distance","0")
 tell <| Fsc.Facts.rule("obstacle","false","get photo")
 tell <| Fsc.Facts.rule("obstacle","false","whatdoyousee")
 tell <| Fsc.Facts.rule("obstacle","true","stop")
 tell <| Fsc.Facts.rule("obstacle","false","go forward")
 tell <| Fsc.Facts.rule("obstacle","false","get distance")
 tell <| Fsc.Facts.rule("obstacle","true","get distance")
 tell <| Fsc.Facts.rule("person","true","led on 1")
 tell <| Fsc.Facts.rule("person","false","led off 1")
 for _ in !-- rule(ctx?obj,"false",ctx?cmd) do
   tell<|Fsc.Facts.observe(ctx?obj,"false")
[<CoDa.Context("fsc-ctx")>]
[<CoDa.EntryPoint>]
let main () =
 initFacts ()
 new_execute "get distance"
 new_execute "get photo"
 new_execute "whatdoyousee"
 let mutable continueLooping = true
 while (continueLooping) do
 
 
  new_observe "obstacle"  (is_obstacle
                              ( try 
                                (float(get_out "get distance"))
                                with e-> float(0) ))

  let recognition = get_out "whatdoyousee" |> imagerecognition
  for tag in recognition.tags do 
       new_observe tag.name (is_confidence tag.confidence)
  for _ in !-- next(ctx?obj,ctx?cmd) do
       new_execute ctx?cmd
  let listchat = command  "telegram list"     [] |> chats
  for idchat in listchat do
    let result =  process_chat idchat 
    printfn "result %s" result
do if (conf.debug) then debug()
    else run()