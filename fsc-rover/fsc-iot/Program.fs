[<CoDa.Code>]
module Fsc.App
 
open System
open System.IO
open CoDa.Runtime
  
open Fsc.Types
open Fsc.Utils

let get_detected obj  =
 match ctx with
 | _ when !- detected(obj) ->  true
 | _ -> false

let discovery obj value  =
  match ctx with
  | _ when !- recognition(obj,ctx?value) -> 
          retract <| Fsc.Facts.recognition(obj,ctx?value)
          tell <| Fsc.Facts.recognition(obj,value) 
  | _ ->  tell <| Fsc.Facts.recognition(obj,value) 

let get_out  syscmd =  
 try 
  let c = ctx?out |- (result(syscmd,ctx?out))
  c
 with e-> "not found"


[<CoDa.ContextInit>]
let initFacts () =
 tell <| Fsc.Facts.objcmd("button","/rpi/button/0")
 tell <| Fsc.Facts.action("button","false","/rpi/led/1/off")
 tell <| Fsc.Facts.action("button","false","/rpi/button/0")
 tell <| Fsc.Facts.action("button","true","/rpi/button/0")
 tell <| Fsc.Facts.action("button","true","/rpi/led/1/on")
 tell <| Fsc.Facts.confidence("button",1.0,1.0)

 //tell this fact because 'for _ !-- request(...) do ...' crash if request facts is empty into context
 tell <| Fsc.Facts.request(0,"0")
 tell <| Fsc.Facts.usrcmd("1","1")
 tell <| Fsc.Facts.recognition("button",0.0)
 tell <| Fsc.Facts.result("-","")
[<CoDa.Context("fsc-ctx")>]
[<CoDa.EntryPoint>]
let main () =
 initFacts ()
 let mutable listresult=   [||]
 let mutable array_cmd =  [||]
 while (true) do

  for _ in !-- next(ctx?syscmd) do 
   array_cmd <- array_cmd  |> Array.append [|(ctx?syscmd, [])|] 
  listresult <- Async.Parallel 
     [for syscmd,param in  Array.distinct array_cmd  ->   
         execute syscmd param
         ] 
      |> Async.RunSynchronously

  for r in listresult do        
   match r with
   |syscmd,res -> for _ in !-- result(syscmd,ctx?out) do 
                   retract <| Fsc.Facts.result(syscmd, ctx?out)   
                  tell <| Fsc.Facts.result(syscmd,res)
  for _ in !-- objcmd(ctx?obj,ctx?syscmd) do     
   discovery ctx?obj (try 
                         float(get_out ctx?syscmd)
                        with e-> 0.0) 
  array_cmd <-[||] 
  printfn "button:%b time:%s %d" (get_detected "button") (System.DateTime.Now.ToLongTimeString()) (System.DateTime.Now.Millisecond) 
     
do if (conf.debug) then debug()
    else run()