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

 
     let resp=try 
               Http.RequestString((urlbuild (cmdbuild(cmd))), q , silentHttpErrors = true)
              with 
              | :? System.Net.WebException ->    "error"
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
        bool.Parse(s)
     with e-> false

let get_nextcmd obj = 
    try 
        if (get_observe obj) then
         let cmd = ctx?cmd |- then_next(obj,ctx?cmd)
         cmd
        else 
         let cmd = ctx?cmd |- else_next(obj,ctx?cmd)
         cmd
    with e -> "nop"
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


let new_execute cmd  =
    for _ in !-- execute(cmd,ctx?out) do
     retract <| Fsc.Facts.execute(cmd, ctx?out) 
    tell <| Fsc.Facts.execute(cmd,sprintf "%s" (command  cmd  []))

let new_observe name status =
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
let get_caption ir =
    ir.captions.[0].text
let get_distance =
    try 
        do new_execute "get distance"  
        float(get_out "get distance")
    with e-> float(0)
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

let send_message idchat text=
    let snd= match (get_message idchat) with
             | Prefix "get photo" rest -> command (sprintf "telegram photo %i" idchat)
                                              ["idphoto",get_out "get photo";
                                               "text",((command "whatdoyousee" ["idphoto",get_out "get photo"] |> imagerecognition).description.captions.[0].text)]
             | Prefix "get video" rest -> command (sprintf "telegram video %i" idchat) 
                                              ["idvideo",get_out "get video";
                                               "text",(command "whatdoyousee" ["idphoto",get_out "get photo"] |> imagerecognition).description.captions.[0].text ]
             | _   -> command  (sprintf "telegram text %i" idchat) [ "text",text]
 
    snd
 


let process_chat idchat =
     let cmd= match (get_message idchat) with
              | Prefix "photo" rest -> sprintf "get photo"  
              | Prefix "video" rest -> sprintf "get video"  
              | Prefix "distance" rest -> sprintf "get distance"
              | Prefix "left" rest -> sprintf "go left"
              | Prefix "forward" rest -> sprintf "go forward"
              | Prefix "backward" rest -> sprintf "go backward"
              | Prefix "right" rest -> sprintf "go right"
              | _ -> "nop"
     if not (cmd="nop") then 
       for _ in !-- request(sprintf "%i" idchat,ctx?cmd) do
        retract <| Fsc.Facts.request(sprintf "%i" idchat,ctx?cmd) 
       tell<|Fsc.Facts.request(sprintf "%i" idchat, cmd) 
       do new_execute cmd
       try
        let out = ctx?out |- response(sprintf "%i" idchat,ctx?out)
        send_message idchat out
       with e-> "error"
      else "nop"

[<CoDa.ContextInit>]
let initFacts () =
 tell <| Fsc.Facts.request("0","nop")
 tell <| Fsc.Facts.execute("get distance","0")
 tell <| Fsc.Facts.observe("obstacle","true")
 tell <| Fsc.Facts.then_next("obstacle","stop")
 tell <| Fsc.Facts.else_next("obstacle","go forward")
 tell <| Fsc.Facts.then_next("person","led on 1")
 tell <| Fsc.Facts.else_next("person","led off 1")

[<CoDa.Context("fsc-ctx")>]
[<CoDa.EntryPoint>]
let main () =
 initFacts ()

 let mutable continueLooping = true
 while (continueLooping) do
 
  do new_observe "obstacle"  (is_obstacle (get_distance))
  do new_execute "get photo"
  //let idphoto=
 // printfn "%s" idphoto
  let descimg =  command "whatdoyousee" ["idphoto",get_out "get photo"] |> imagerecognition
  for tag in descimg.tags 
    do new_observe tag.name (is_confidence tag.confidence)
       new_execute (get_nextcmd tag.name)
  let listchat = command  "telegram list"     [] |> chats
  for idchat in listchat do
    let result =  process_chat idchat 
    printfn "%s" result
do if (conf.debug) then debug()
    else run()