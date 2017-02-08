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
                    with e-> "not found"

let get_confidence obj value= 
                
                           match ctx with
                             | _ when !- confidence(obj,ctx?min,ctx?max)
                                 ->  ((value>ctx?min) && (value<ctx?max))
                             | _ -> false
             

let execute cmd = async {
          let result = match cmd with
                          | Prefix "discovery" rest -> sprintf "%s"  (command cmd ["idphoto",get_out "get photo"])
                          | Prefix "broadcast" rest -> sprintf "%s"  (command "telegram broadcast" ["text",sprintf "%s" ((rest).Trim())])
                          | Prefix "help" rest -> get_out "help"
                          | _ ->  sprintf "%s" (command  cmd  []) 
          return cmd,result
          }

let get_command usercommand =  
            try
             let cmd = ctx?cmd |- (usrcmd(sprintf "%s" usercommand,ctx?cmd))
             cmd
            with e-> "help"
let get_cmddesc cmd =
            try
             let desccmd = ctx?description |- (cmddesc(sprintf "%s" cmd,ctx?description))
             desccmd
            with e-> "unknown"

let get_usrcmddesc usrcmd =
    try
     let desccmd = ctx?description |- (usrcmddesc(sprintf "%s" usrcmd,ctx?description))
     desccmd
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
     with e-> "not found"


let get_detected obj  =
    match ctx with
     | _ when !- detected(obj) ->  true
     | _ -> false
//    try
//        let s = ctx?status |- detected(obj,ctx?status)
//        s
//     with e-> false

//let get_nextcmd obj = 
//   try 
//         let cmd = ctx?cmd |- next(obj,ctx?cmd)
//         cmd
//        
//    with e -> "nop"



let discovery obj value  =
   // printfn "discovery %s %f" obj value
    match ctx with
     | _ when !- recognition(obj,ctx?value) ->   retract <| Fsc.Facts.recognition(obj,ctx?value)
                                                 tell <| Fsc.Facts.recognition(obj,value) 
     | _ -> tell <| Fsc.Facts.recognition(obj,value) 
 

let reset obj =

    discovery obj  (float(num.MinValue))

[<CoDa.ContextInit>]
let initFacts () =
 tell <| Fsc.Facts.request("0","nop")
 tell <| Fsc.Facts.recognition("exit",0.0)
 tell <| Fsc.Facts.confidence("obstacle",0.1,50.0)
 tell <| Fsc.Facts.confidence("person",0.9,1.0)
 tell <| Fsc.Facts.confidence("exit",1.0,1.0)
 tell <| Fsc.Facts.confidence("never",0.0,0.0)
 tell <| Fsc.Facts.action("never","true","discovery")
 tell <| Fsc.Facts.action("never","false","get distance")
 tell <| Fsc.Facts.action("never","false","get channels")
 tell <| Fsc.Facts.action("never","true","get photo")
 tell <| Fsc.Facts.action("indoor","true","led on 0")
 tell <| Fsc.Facts.action("person","false","led off 0")
 tell <| Fsc.Facts.action("obstacle","true","stop")
 tell <| Fsc.Facts.action("never","false","led off 0")
 tell <| Fsc.Facts.usrcmd("photo","get photo")
 tell <| Fsc.Facts.cmddesc("get photo","snapshot a photo with camera")
 tell <| Fsc.Facts.usrcmd("video","get video")  
 tell <| Fsc.Facts.cmddesc("get video","shoot a movie with camera")
 tell <| Fsc.Facts.usrcmd("distance","get distance")
 tell <| Fsc.Facts.cmddesc("get distance","get the distance of the obstacle")
 tell <| Fsc.Facts.usrcmd("left","go left")
 tell <| Fsc.Facts.cmddesc("left","rover turn left")
 tell <| Fsc.Facts.usrcmd("right","go right")
 tell <| Fsc.Facts.cmddesc("right","rover turn right")
 tell <| Fsc.Facts.usrcmd("lon","led on 0")
 tell <| Fsc.Facts.usrcmd("loff","led off 0")
 tell <| Fsc.Facts.usrcmd("forward","go forward")
 tell <| Fsc.Facts.usrcmd("backward","go backward")

 tell <| Fsc.Facts.usrcmd("stop","stop")
 tell <| Fsc.Facts.cmddesc("stop","stop the rover")
 tell <| Fsc.Facts.usrcmd("discovery","discovery")
 tell <| Fsc.Facts.usrcmd("help","help")

 let mutable help="?"
 for _ in !-- usrcmddesc(ctx?usrcmd,ctx?desc) do
       help <- sprintf "%s\n\t*%s*\t%s" help ctx?usrcmd ctx?desc
 
 tell <| Fsc.Facts.result("help",help)
 for _ in !-- action(ctx?obj,ctx?status,ctx?cmd) do  reset ctx?obj  

[<CoDa.Context("fsc-ctx")>]
[<CoDa.EntryPoint>]
let main () =
 initFacts ()
 let mutable listresult=[||]
 let mutable array_cmd =  [|"broadcast start"|]

 //Async.Start( async{
 while (not (get_detected "exit")) do

     for _ in !-- request(ctx?idchat,ctx?cmd) do array_cmd <- array_cmd |> Array.append [|ctx?cmd|]
                                
     for _ in !-- next(ctx?cmd) do array_cmd <- array_cmd |> Array.append [|ctx?cmd|]  

     listresult <- Async.Parallel [for c in  array_cmd -> execute c] |> Async.RunSynchronously

     for r in listresult do
         match r with
          |cmd,res -> for _ in !-- result(cmd,ctx?out) do retract <| Fsc.Facts.result(cmd, ctx?out)   

                      tell <| Fsc.Facts.result(cmd, res)

     discovery "obstacle" (try 
                            float(get_out "get distance")
                           with e-> 0.0) 
 
     
     let infoimage = get_out "discovery" |> imagerecognition
     for tag in infoimage.tags do 
         discovery tag.name tag.confidence 
    // printfn "discovery:"
     for _ in !-- recognition(ctx?obj,ctx?value) do
      printfn "recognition:%s,\t%f,\t%b" ctx?obj ctx?value (get_detected ctx?obj)
  
 
     for idchat in (get_out "get channels" |> get_list) do
         let msg=get_message idchat               
         if (msg.Length >0) then  
          tell<|Fsc.Facts.request(sprintf "%i" idchat,  get_command msg.[0] ) 
                            
 
     match ctx with
      | _ when !- (request(ctx?idchat,ctx?cmd),result(ctx?cmd,ctx?out)) -> do 
                                                 printfn "request(%s,%s)" ctx?idchat ctx?cmd 
                                                 let result=send_message ctx?idchat ctx?cmd (get_response ctx?idchat) 
                                                                                            (caption (get_out "discovery"))
                                                 retract<|Fsc.Facts.request(ctx?idchat, ctx?cmd)     
      | _ ->  printfn "no request"

   

     array_cmd <-[||]
do if (conf.debug) then debug()
    else run()