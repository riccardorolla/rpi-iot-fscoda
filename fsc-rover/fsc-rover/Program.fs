[<CoDa.Code>]
module Fsc.App
 
open System
open System.IO
open CoDa.Runtime
open Fsc.Types
open Fsc.Utils
 


let get_out  cmd = 
                  try 
                      let dr = ctx?out |- (execute(cmd,ctx?out))
                      printfn "get_out cmd:%s out:%s" cmd dr
                      dr
                  with e-> ""
let new_execute cmd  =
                 printfn "new_execute %s" cmd 
                 match ctx with
                  | _ when !- execute(cmd,ctx?out) -> retract <| Fsc.Facts.execute(cmd, ctx?out) 
                                                      if (cmd="whatdoyousee") then 
                                                       tell <| Fsc.Facts.execute(cmd,sprintf "%s" (command cmd ["idphoto",get_out "get photo"]))
                                                      else
                                                       tell <| Fsc.Facts.execute(cmd,sprintf "%s" (command  cmd  []))
                  | _ -> if (cmd="whatdoyousee") then 
                          tell <| Fsc.Facts.execute(cmd,sprintf "%s" (command cmd ["idphoto",get_out "get photo"]))
                         else
                          tell <| Fsc.Facts.execute(cmd,sprintf "%s" (command  cmd  []))
let caption str =  
              try
               let imageinfo = str |> imagerecognition
               sprintf  "description %s" imageinfo.description.captions.[0].text 
              with e-> "no desc"             
        

let send_message idchat cmd text:string=
        printfn "send_message %i %s %s" idchat cmd text
        let snd= match (cmd) with
                 | Prefix "get photo" rest ->  command (sprintf "telegram photo %i" idchat)
                                                  ["idphoto",text;
                                                   "text",caption (get_out "whatdoyousee")]
                 | Prefix "get video" rest ->  command (sprintf "telegram video %i" idchat) 
                                                  ["idvideo",text;
                                                   "text",caption (get_out "whatdoyousee")]
                 | _   -> command  (sprintf "telegram text %i" idchat) [ "text", sprintf "%s -> %s" cmd text]
     
        snd
     
let get_response idchat =
    try
     let out = ctx?out |- (response(sprintf "%i" idchat,ctx?out))
     out
    with e-> "error"


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
        //do new_execute cmd
        let snd=send_message idchat cmd (get_response idchat)
        retract<|Fsc.Facts.request(sprintf "%i" idchat, cmd) 
        sprintf "%s" snd




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


 
let is_obstacle distance =
    printfn "is_osbtacle %f" distance
    if (distance<0.3) then "true"
     else "false" 
let is_confidence confidence =
    printfn "is_confidence %f" confidence
    if (confidence>0.9) then "true"
     else "false"
 



let new_observe obj status  =
    match ctx with
     | _ when !- observe(obj,ctx?status) -> retract <| Fsc.Facts.observe(obj,ctx?status)
 
                                            tell <| Fsc.Facts.observe(obj,status) 
     | _ ->  tell <| Fsc.Facts.observe(obj,status)
 



[<CoDa.ContextInit>]
let initFacts () =
 tell <| Fsc.Facts.request("0","nop")
 tell <| Fsc.Facts.observe("exit","false")
 tell <| Fsc.Facts.execute("get distance","0")
 tell <| Fsc.Facts.rule("obstacle","false","get photo")
 tell <| Fsc.Facts.rule("obstacle","false","whatdoyousee")
 tell <| Fsc.Facts.rule("obstacle","true","stop")
 tell <| Fsc.Facts.rule("obstacle","false","go forward")
 tell <| Fsc.Facts.rule("obstacle","false","get distance")
 tell <| Fsc.Facts.rule("obstacle","true","get distance")
 tell <| Fsc.Facts.rule("person","true","led on 1")
 tell <| Fsc.Facts.rule("person","false","led off 1")


[<CoDa.Context("fsc-ctx")>]
[<CoDa.EntryPoint>]
let main () =
 initFacts ()
 new_execute "get distance"
 new_execute "get photo"
 new_execute "whatdoyousee"
 printfn "caption:%s" (caption (get_out "whatdoyousee"))
 //while (true) do
 while (true) do
     for _ in !-- execute(ctx?cmd,ctx?out) do
      printfn "cmd(%s,%s)" ctx?cmd ctx?out
     
     match ctx with
      | _ when !- rule(ctx?obj,"true",ctx?cmd) -> retract<|Fsc.Facts.observe(ctx?obj,"true")
      | _ when !- rule(ctx?obj,"false",ctx?cmd) ->   new_observe ctx?obj "false"                                                                                                 
      | _ ->  printfn "no rule defined"

     new_observe "obstacle" (is_obstacle (
                                 try 
                                   (float(get_out "get distance"))
                                 with e-> float(0) ))
     let recognition = get_out "whatdoyousee" |> imagerecognition
     for tag in recognition.tags do 
         printfn "obj %s" tag.name
         new_observe tag.name (is_confidence tag.confidence)
     
     for _ in !-- observe(ctx?obj,ctx?status) do
      printfn "obj(%s,%s)" ctx?obj ctx?status
     for _ in !-- next(ctx?obj,ctx?cmd) do
      new_execute ctx?cmd     
     let listchat = command  "telegram list"     [] |> chats
     for idchat in listchat do
      let result =  process_chat idchat
      printfn "result %s" result
do if (conf.debug) then debug()
    else run()