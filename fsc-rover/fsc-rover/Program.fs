[<CoDa.Code>]
module Fsc.App
 
open System
open System.IO
open CoDa.Runtime
  
open Fsc.Types
open Fsc.Utils


let get_out  cmd =  try 
                      let c = ctx?out |- (execute(cmd,ctx?out))
                      c
                    with e-> ""

let new_execute cmd  =
                 match ctx with

                  | _ when !- execute(cmd,ctx?out) -> retract <| Fsc.Facts.execute(cmd, ctx?out)                                                    
                  | _ -> printfn "_ -> new_execute %s" cmd 
                 match cmd with
                         | "discovery" -> tell <| Fsc.Facts.execute(cmd,sprintf "%s" (command cmd ["idphoto",get_out "get photo"]))
                         | "help" -> tell <| Fsc.Facts.execute(cmd,"?")
                         | _ -> tell <| Fsc.Facts.execute(cmd,sprintf "%s" (command  cmd  []))

let get_command usercommand =  
            try
             let cmd = ctx?cmd |- (user_command(sprintf "%s" usercommand,ctx?cmd))
             cmd
            with e-> "help"
let get_usercommand cmd =
            try
             let usercmd = ctx?prompt |- (user_command(ctx?prompt,sprintf "%s" cmd))
             usercmd
            with e-> "unknown"

let get_response idchat =
    try
     let out = ctx?out |- (response(idchat,ctx?out))
     out
    with e-> "error"
 
let get_req idchat =
     try 
         let req = ctx?cmd |- (request(idchat,ctx?cmd))
         req
     with e-> ""


let get_observe obj  =
    try
        let s = ctx?status |- observe(obj,ctx?status)
        s
     with e-> "false"

let get_nextcmd obj = 
    try 
         let cmd = ctx?cmd |- next(obj,ctx?cmd)
         cmd
        
    with e -> "nop"



let new_observe obj status  =
    match ctx with
     | _ when !- observe(obj,ctx?status) -> retract <| Fsc.Facts.observe(obj,ctx?status)
                                            tell <| Fsc.Facts.observe(obj,status) 
     | _ ->  tell <| Fsc.Facts.observe(obj,status)
 



[<CoDa.ContextInit>]
let initFacts () =
 tell <| Fsc.Facts.request("0","nop")
 tell <| Fsc.Facts.observe("exit","false")
 
 tell <| Fsc.Facts.rule("obstacle","false","get photo")
 tell <| Fsc.Facts.rule("obstacle","false","discovery")
 tell <| Fsc.Facts.rule("obstacle","true","stop")
 tell <| Fsc.Facts.rule("obstacle","false","go forward")
 tell <| Fsc.Facts.rule("obstacle","false","get distance")
 tell <| Fsc.Facts.rule("obstacle","true","get distance")
 tell <| Fsc.Facts.rule("person","true","led on 0")
 tell <| Fsc.Facts.rule("person","false","led off 0")
 tell <| Fsc.Facts.user_command("photo","get photo")
 tell <| Fsc.Facts.user_command("video","get video")  
 tell <| Fsc.Facts.user_command("distance","get distance")
 tell <| Fsc.Facts.user_command("left","go left")
 tell <| Fsc.Facts.user_command("lon","led on 0")
 tell <| Fsc.Facts.user_command("loff","led off 0")
 tell <| Fsc.Facts.user_command("forward","go forward")
 tell <| Fsc.Facts.user_command("backward","go backward")
 tell <| Fsc.Facts.user_command("right","go right")
 tell <| Fsc.Facts.user_command("stop","stop")
 tell <| Fsc.Facts.user_command("help","help")
 let mutable help="?"
 for _ in !--  user_command(ctx?prompt,ctx?cmd) do
       help <- sprintf "%s\n\t%s:%s" help ctx?prompt ctx?cmd
 
 tell <| Fsc.Facts.execute("help",help)
  

[<CoDa.Context("fsc-ctx")>]
[<CoDa.EntryPoint>]
let main () =
 initFacts ()


 new_execute "get distance"
 new_execute "get photo"
 new_execute "discovery"
 printfn "caption:%s" (caption (get_out "discovery"))

 while (true) do
 
     for _ in !--  rule(ctx?obj,ctx?status,ctx?cmd) do
                 new_observe ctx?obj "false"

     new_observe "obstacle" (is_obstacle (
                                 try 
                                   (float(get_out "get distance"))
                                 with e-> float(0) ))
     let recognition = get_out "discovery" |> imagerecognition
     for tag in recognition.tags do 
         new_observe tag.name (is_confidence tag.confidence)
    
     for _ in !-- observe(ctx?obj,ctx?status) do
      printfn "observe(%s,%s)" ctx?obj ctx?status

     let listchat = command  "get channels" [] |> chats

     for idchat in listchat do
         let msg=get_message idchat
      
                     
         if (msg.Length >0) then // let param  =  msg  |> Seq.skip 1 |>  String.concat  " "  
                                  tell<|Fsc.Facts.request(sprintf "%i" idchat,  get_command msg.[0] ) 
                                  
       
         
     match ctx with
     | _ when !- request(ctx?idchat,"help") ->  let result=send_message ctx?idchat  "help" (get_out "help") (caption (get_out "discovery"))
                                                retract<|Fsc.Facts.request(ctx?idchat, "help")    
     | _ when !- request(ctx?idchat,ctx?cmd) -> do new_execute ctx?cmd
                                                printfn "request(%s,%s)" ctx?idchat ctx?cmd
                                                let result=send_message ctx?idchat  ctx?cmd (get_response ctx?idchat) (caption (get_out "discovery"))
                                                retract<|Fsc.Facts.request(ctx?idchat, ctx?cmd)     
     | _ -> for _ in !-- next(ctx?obj,ctx?cmd) do  
                                                  new_execute ctx?cmd     

 

do if (conf.debug) then debug()
    else run()