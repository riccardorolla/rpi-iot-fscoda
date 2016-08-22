[<CoDa.Code>]
module Fsc.App
 
open System
open System.IO
open CoDa.Runtime
open Fsc.Types
open FSharp.Data
open Newtonsoft.Json
//open Fsc.Facts


type Configuration(server:string,port:int) =
     member this.Server=server 
     member this.Port=port
 

let confjson =
 try  
  File.ReadAllText(@"fsc-rover.json")
 with
  | :? System.IO.FileNotFoundException  -> 
        File.WriteAllText(@"fsc-rover.json","""{"server":"localhost","port":8081}""")
        """{"server":"localhost","port":8081}"""
      
      
      
let conf = JsonConvert.DeserializeObject<Configuration>(confjson)

printfn "server:%s,port:%i" conf.Server conf.Port

let urlbuild server port command =  sprintf "http://%s:%i/%s" server port command


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


type execCommand<'T>(cmd :string , value, ?q0) =
 let q = defaultArg q0 []
 let out = 
     let resp=try 
               Http.RequestString((urlbuild conf.Server conf.Port (cmdbuild(cmd))), q,  silentHttpErrors = true)
              with 
              | :? System.Net.WebException -> value
     try 
      JsonConvert.DeserializeObject<'T>(sprintf "%s" (resp))       
     with 
     | :? Newtonsoft.Json.JsonReaderException -> 
      JsonConvert.DeserializeObject<'T>(JsonConvert.SerializeObject(resp)) 
 member this.output 
   with get() =  out
  
 
 


type Message(idmsg:string, txt:string) = 
  member this.Idmsg=idmsg
  member this.Txt=txt
  

let get_out  cmd = 
      try 
          let dr = ctx?out |- (rover_command(cmd,ctx?out))
          dr
      with e-> ""
let get_req idchat =
     try 
         let dr = ctx?cmd |- (rover_request(idchat,ctx?cmd))
         dr
     with e-> ""

let validate out = 
  try 
   let dr= ctx?status |- (rover_validate(out,ctx?status))
   dr
  with e-> false

 

let display_request idchat cmd =
  match ctx with
  | _ when !- rover_request(idchat, cmd) -> printfn " - %s" cmd
  | _ -> printfn " - %s (this chat not execute cmd)" cmd

let display_out cmd out =
  match ctx with
  | _ when !- rover_command(cmd, out) -> printfn " - %s" out
  | _ -> printfn " - %s (this cmd not output)" out
 
[<CoDa.ContextInit>]
let initFacts () =
 tell <| Fsc.Facts.rover_validate("OK",true)
 tell <| Fsc.Facts.rover_command("get distance","0")
 tell <| Fsc.Facts.rover_validate("0",true)
[<CoDa.Context("fsc-ctx")>]
[<CoDa.EntryPoint>]
let main () =
 initFacts ()
 display_out "led on 1" "OK"
 let mutable continueLooping = true
 while (continueLooping) do
 
  for _ in !-- rover_command("get distance",ctx?out) do
      retract <| Fsc.Facts.rover_command("get distance", ctx?out) 
  let distance = float (new execCommand<string>("get distance","0.0")).output
                 
  tell <| Fsc.Facts.rover_command("get distance",sprintf "%i" (int distance))

  let mutable o = "OK"
  for _ in !-- stop(ctx?status) do 
    o <- (new execCommand<string>("stop","")).output

  
  //if (is_stop) then printfn "%s" (new execCommand<string>("stop")).output
 //  let stop = (new execCommand<string>("stop")).output
  let listchat =  (new execCommand<List<int>>("telegram list","[]")).output
                

  for idchat in listchat do 
     let messages = (new execCommand<List<Message>>(sprintf "telegram message %i" idchat,"[]")).output
     let nMessages = messages |> Seq.length
   //  printfn "\t\tnMessages:%i" nMessages
     if (nMessages>0) then
      let msg = (new execCommand<Message>(sprintf "telegram pop %i" idchat,"{}")).output
      let cmd = msg.Txt.ToLower()
      tell <| Fsc.Facts.rover_request(idchat,cmd)
      let outcmd=((new execCommand<string>(cmd,"")).output).Trim()
      tell <| Fsc.Facts.rover_command(cmd,outcmd)
      let output =
       if (validate outcmd) then
        (new execCommand<string>((sprintf "telegram text %i" idchat),"",["text", (sprintf "%s" outcmd)])).output
       else
        (new execCommand<string>((sprintf "telegram text %i" idchat),"",["text", "not validate"])).output
      printfn "\t\t\tidmsg:%s,txt:%s,outcmd:%s,output:%s" msg.Idmsg  msg.Txt outcmd output

do debug ()