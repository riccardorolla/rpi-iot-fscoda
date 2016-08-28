[<CoDa.Code>]
module Fsc.App
 
open System
open System.IO
open CoDa.Runtime
  
open Fsc.Types
open Fsc.Utils


let get_out  cmd =  try 
                      let c = ctx?out |- (result(cmd,ctx?out))
                      c
                    with e-> ""

let get_confidence obj value= 
                
                           match ctx with
                             | _ when !- confidence(obj,ctx?min,ctx?max)
                                 ->  ((value>ctx?min) && (value<ctx?max))
                             | _ -> false
                       
let execute cmd  =
                 match ctx with

                  | _ when !- result(cmd,ctx?out) -> retract <| Fsc.Facts.result(cmd, ctx?out)                                                    
                  | _ -> printfn "_ -> new_execute %s" cmd 
                 match cmd with
                         | "discovery" -> tell <| Fsc.Facts.result(cmd,sprintf "%s" (command cmd ["idphoto",get_out "get photo"]))
                         | _ -> tell <| Fsc.Facts.result(cmd,sprintf "%s" (command  cmd  []))

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
         let req = ctx?cmd |- (request(idchat,ctx?cmd,ctx?param))
         req
     with e-> ""


let get_found obj  =
    try
        let s = ctx?status |- found(obj,ctx?status)
        s
     with e-> false

let get_nextcmd obj = 
    try 
         let cmd = ctx?cmd |- next(obj,ctx?cmd)
         cmd
        
    with e -> "nop"



let discovery obj value  =
    let status=
     match ctx with
       | _ when !- confidence(obj,ctx?min,ctx?max)
           ->  ((value>=ctx?min) && (value<=ctx?max))
       | _ -> false
    match ctx with
     | _ when !- found(obj,ctx?status) ->   retract <| Fsc.Facts.found(obj,ctx?status)
                                            tell <| Fsc.Facts.found(obj,status) 
     | _ ->  tell <| Fsc.Facts.found(obj,status)
 

let reset obj =

    discovery obj  (float(num.MinValue))

[<CoDa.ContextInit>]
let initFacts () =
 tell <| Fsc.Facts.request("0","nop","")
 tell <| Fsc.Facts.found("exit",false)
 tell <| Fsc.Facts.confidence("obstacle",0.0,50.0)
 tell <| Fsc.Facts.confidence("person",0.9,1.0)
 tell <| Fsc.Facts.confidence("wall",0.8,1.0)
 tell <| Fsc.Facts.confidence("exit",1.0,1.0)
 tell <| Fsc.Facts.rule("obstacle",false,"get photo")
 tell <| Fsc.Facts.rule("obstacle",true,"get photo")
 tell <| Fsc.Facts.rule("obstacle",true,"stop")
 tell <| Fsc.Facts.rule("indoor",true,"stop")
 tell <| Fsc.Facts.rule("obstacle",false,"get distance")
 tell <| Fsc.Facts.rule("obstacle",true,"get distance")
 tell <| Fsc.Facts.rule("person",true,"led on 0")
 tell <| Fsc.Facts.rule("person",true,"broadcast there is a person")
 tell <| Fsc.Facts.rule("person",false,"led off 0")
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
 //tell <| Fsc.Facts.user_command("exit","exit")
// tell <| Fsc.Facts.user_command("discovery","discovery")
 let mutable help="?"
 for _ in !--  user_command(ctx?prompt,ctx?cmd) do
       help <- sprintf "%s\n\t%s:%s" help ctx?prompt ctx?cmd
 
 tell <| Fsc.Facts.result("help",help)
  

[<CoDa.Context("fsc-ctx")>]
[<CoDa.EntryPoint>]
let main () =
 initFacts ()

 execute "broadcast start fsc-rover"
 execute "get distance"
 execute "get photo"
 execute "discovery"
 printfn "caption:%s" (caption (get_out "discovery"))

 while (not(get_found "exit")) do
      
     for _ in !-- rule(ctx?obj,ctx?status,ctx?cmd) do  reset ctx?obj  
    
     discovery "obstacle" (try 
                            float(get_out "get distance")
                           with e-> 0.0) 
 
     
     let recognition = get_out "discovery" |> imagerecognition
     for tag in recognition.tags do 
         discovery tag.name tag.confidence 
    
     for _ in !-- found(ctx?obj,ctx?status) do
      printfn "found(%s,%b)" ctx?obj ctx?status

     let channels = command  "get channels" [] |> get_list

     for idchat in channels do
         let msg=get_message idchat
      
                     
         if (msg.Length >0) then  let param  =  msg  |> Seq.skip 1 |>  String.concat  " "  
                                  tell<|Fsc.Facts.request(sprintf "%i" idchat,  get_command msg.[0],param ) 
                                  
       
        
     match ctx with
     | _ when !- request(ctx?idchat,"help",ctx?param) ->  let result=send_message ctx?idchat  "help" (get_out "help") (caption (get_out "discovery"))
                                                          retract<|Fsc.Facts.request(ctx?idchat, "help",ctx?param)

     | _ when !- request(ctx?idchat,ctx?cmd,ctx?param) -> do 
                                                execute ctx?cmd
                                                printfn "request(%s,%s,%s)" ctx?idchat ctx?cmd ctx?param
                                                let result=send_message ctx?idchat  ctx?cmd (get_response ctx?idchat) (caption (get_out "discovery"))
                                                retract<|Fsc.Facts.request(ctx?idchat, ctx?cmd,ctx?param)     
     | _ -> for _ in !-- next(ctx?obj,ctx?cmd) do  
                                                  execute ctx?cmd     

 

do if (conf.debug) then debug()
    else run()