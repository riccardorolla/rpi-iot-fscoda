[<CoDa.Code>]
module Fsc.App
 
open System
open System.IO
open CoDa.Runtime
  
open Fsc.Types
open Fsc.Utils

let get_usercmd syscmd =  
 try
  let usercmd = 
   ctx?usercmd |- (usrcmd(ctx?usercmd,syscmd))
  usercmd
 with e-> syscmd

let discovery obj value  =
 match ctx with
 | _ when !- recognition(obj,ctx?value) -> 
         retract <| Fsc.Facts.recognition(obj,ctx?value)
         tell <| Fsc.Facts.recognition(obj,value) 
 | _ ->  tell <| Fsc.Facts.recognition(obj,value) 

let get_detected obj  =
 match ctx with
 | _ when !- detected(obj) ->  true
 | _ -> false

let get_out  syscmd =  
 try 
  let c = ctx?out |- (result(syscmd,ctx?out))
  c
 with e-> "not found"


let get_cmdrsp idchat syscmd =
 match (syscmd) with
 | "/rpi/photo" ->(sprintf "/telegram/%i/photo" idchat,
                   ["idphoto",get_out syscmd;
                  "text",caption (get_out "/whatdoyousee")])
 | "/rpi/video" ->(sprintf "/telegram/%i/video" idchat,
                   ["idvideo",get_out syscmd;
                   "text",caption (get_out "/whatdoyousee")])
 | "/whatdoyousee"-> let mutable result = "discovery->\n"
                     for _ in !-- recognition(ctx?obj,ctx?value) do
                      result <- sprintf "%s%s,\t%f,\t%b\n" 
                       result ctx?obj ctx?value (get_detected ctx?obj)
                     (sprintf "/telegram/%i/text" idchat,
                      ["text",result])
 | _ ->          (sprintf "/telegram/%i/text" idchat, 
                  ["text", sprintf "%s -> %s" 
                  (get_usercmd syscmd) (get_out syscmd)])
   
let get_cmd syscmd = 
 match syscmd with
 | "/whatdoyousee"  -> (syscmd,["idphoto",get_out "/rpi/photo"])
 | "help" ->          (get_out "help",[])
 | _ ->               (syscmd,[]) 
    
  
let get_rsp idchat res = 
 match ctx with
 | _ when !- 
     (request(idchat,ctx?usercmd),
      usrcmd(ctx?usercmd,ctx?syscmd),
      result(ctx?syscmd,res)) ->  
         retract<|Fsc.Facts.request(idchat,ctx?usercmd)    
         get_cmdrsp idchat ctx?syscmd 
 | _ ->  
     (sprintf "/telegram/%i/text" idchat ,
      ["text",
       sprintf "not found command per this result:%s " res])  
   

   
   
[<CoDa.ContextInit>]
let initFacts () =
 tell <| Fsc.Facts.objcmd("exit","/rpi/button/0")
 tell <| Fsc.Facts.objcmd("obstacle","/rpi/distance")
 tell <| Fsc.Facts.cmddesc("/rpi/photo","snapshot a photo with camera")
 tell <| Fsc.Facts.cmddesc("/rpi/video","shoot a movie with camera")
 tell <| Fsc.Facts.cmddesc("/rpi/distance","get the distance of the obstacle")
 tell <| Fsc.Facts.cmddesc("/rpi/motor/left","rover turn left")
 tell <| Fsc.Facts.cmddesc("/rpi/motor/right","rover turn right")
 tell <| Fsc.Facts.cmddesc("/rpi/motor/forward","rover move forward")
 tell <| Fsc.Facts.cmddesc("/rpi/motor/backward","rover move backward")
 tell <| Fsc.Facts.cmddesc("/rpi/motor/stop","stop the rover")
 tell <| Fsc.Facts.cmddesc("/rpi/led/0/on", "turn on led number 0")
 tell <| Fsc.Facts.cmddesc("/rpi/led/0/off", "turn off led number 0")
 tell <| Fsc.Facts.cmddesc("/rpi/led/1/on", "turn on led number 1")
 tell <| Fsc.Facts.cmddesc("/rpi/led/1/off", "turn off led number 1")
 tell <| Fsc.Facts.cmddesc("help", "command help")
 tell <| Fsc.Facts.cmddesc("/whatdoyousee","recognition objects in last snapshot")

 tell <| Fsc.Facts.usrcmd("photo","/rpi/photo")
 tell <| Fsc.Facts.usrcmd("video","/rpi/video")  
 tell <| Fsc.Facts.usrcmd("distance","/rpi/distance")
 tell <| Fsc.Facts.usrcmd("left","/rpi/motor/left")
 tell <| Fsc.Facts.usrcmd("right","/rpi/motor/right")
 tell <| Fsc.Facts.usrcmd("lon","/rpi/led/0/on")
 tell <| Fsc.Facts.usrcmd("loff","/rpi/led/0/off")
 tell <| Fsc.Facts.usrcmd("forward","/rpi/motor/forward")
 tell <| Fsc.Facts.usrcmd("backward","/rpi/motor/backward")
 tell <| Fsc.Facts.usrcmd("stop","/rpi/motor/stop")
 tell <| Fsc.Facts.usrcmd("discovery","/whatdoyousee")
 tell <| Fsc.Facts.usrcmd("help","help")

 let mutable help="?"
 for _ in !-- usrcmddesc(ctx?usrcmd,ctx?desc) do
  help <- sprintf "%s\n\t*%s*\t%s" help ctx?usrcmd ctx?desc
 tell <| Fsc.Facts.result("help",help)
 
 tell <| Fsc.Facts.confidence("obstacle",0.0,50.0)
 tell <| Fsc.Facts.confidence("person",0.9,1.0)
 tell <| Fsc.Facts.confidence("exit",1.0,1.0)

 tell <| Fsc.Facts.action("exit","true","/whatdoyousee")
 tell <| Fsc.Facts.action("exit","false","/rpi/distance")
 tell <| Fsc.Facts.action("exit","false","/telegram/listchat")
 tell <| Fsc.Facts.action("exit","true","/rpi/photo")
 tell <| Fsc.Facts.action("exit","false","/rpi/button/0")
 tell <| Fsc.Facts.action("person","false","/rpi/led/1/off")
 tell <| Fsc.Facts.action("person","true","/rpi/led/1/on")
 tell <| Fsc.Facts.action("obstacle","true","/rpi/motor/stop")

 tell <| Fsc.Facts.recognition("exit",0.0)


 for _ in !-- action(ctx?obj,ctx?status,ctx?syscmd) do
    discovery ctx?obj  (float(num.MinValue))  

 //tell this fact because 'for _ !-- request(...) do ...' crash if request facts is empty into context
 tell <| Fsc.Facts.request(0,"")

[<CoDa.Context("fsc-ctx")>]
[<CoDa.EntryPoint>]
let main () =
 initFacts ()
 let mutable listresult=[||]
 let mutable array_cmd =  
  [|"/telegram/broadcast",
    ["text","daemon fsc-rover start now"]|]

 while (not (get_detected "exit")) do

  for _ in !-- next(ctx?syscmd) do 
   array_cmd <- array_cmd  |> Array.append [|get_cmd ctx?syscmd|] 
  array_cmd <- array_cmd  |> Array.filter (fun syscmd -> match syscmd with 
                                                           |"help",[] -> false
                                                           |_ -> true
                                            ) 
  listresult <- Async.Parallel 
     [for syscmd,param in  Array.distinct array_cmd  ->   
         execute syscmd param
         ] 
      |> Async.RunSynchronously
  System.Console.Clear()
  array_cmd <-[||] 
  for r in listresult do        
   match r with
   |syscmd,res -> printfn "command:%s,\t%s" syscmd  (res.Trim())
                  for _ in !-- result(syscmd,ctx?out) do 
                   retract <| Fsc.Facts.result(syscmd, ctx?out)   
                  tell <| Fsc.Facts.result(syscmd,res)
  for _ in !-- action(ctx?obj,ctx?status,ctx?syscmd) do
     discovery ctx?obj  (float(num.MinValue))  

  for _ in !-- objcmd(ctx?obj,ctx?syscmd) do           
   discovery ctx?obj (try 
                         float(get_out ctx?syscmd)
                        with e-> 0.0) 
 
  let infoimage = get_out "/whatdoyousee" |> imagerecognition
  for tag in infoimage.tags do 
         discovery tag.name tag.confidence 
  for _ in !-- recognition(ctx?obj,ctx?value) do
      printfn "recognition:%s,\t%f,\t%b" 
       ctx?obj ctx?value (get_detected ctx?obj)

  for idchat in (get_out "/telegram/listchat" |> get_list) do
   for _ in !-- response(idchat,ctx?res) do 
          array_cmd <- array_cmd 
           |> Array.append [|get_rsp idchat ctx?res|]
   
   for _ in !-- request(idchat,ctx?usercmd) do
       printfn "not response request:%i,\t%s" idchat ctx?usercmd

   let mutable msg=get_message idchat               
   while (msg.Length >0) do  
       printfn "msg:%s" msg.[0]
       match ctx with
           | _ when !- usrcmd(msg.[0],ctx?syscmd) ->    
                  retract <| Fsc.Facts.request(idchat,msg.[0])
                  tell <| Fsc.Facts.request(idchat,msg.[0])
           | _ -> tell <| Fsc.Facts.request(idchat,"help")
       msg<- get_message idchat  
     
do if (conf.debug) then debug()
    else run()