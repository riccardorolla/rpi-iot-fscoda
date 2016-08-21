[<CoDa.Code>]
module Fsc.App
 
open System
open System.IO
open CoDa.Runtime
open Fsc.Types
open FSharp.Data
open Fsc
open Newtonsoft.Json
 


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


type execCommand<'T>(cmd :string , ?q0) =
 let q = defaultArg q0 []
 let out = 
     let resp=try 
               Http.RequestString((urlbuild conf.Server conf.Port (cmdbuild(cmd))), q,  silentHttpErrors = true)
              with 
              | :? System.Net.WebException -> ""
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
  


 

[<CoDa.ContextInit>]
let initFacts () =
 tell <| Fsc.Facts.rover_distance("0",true)
 tell <| Fsc.Facts.rover_distance("1",true)
 tell <| Fsc.Facts.rover_distance("2",false)
 tell <| Fsc.Facts.rover_distance("3",false)
[<CoDa.Context("fsc-ctx")>]
[<CoDa.EntryPoint>]
let main () =
 initFacts ()
 let mutable continueLooping = true
 while (continueLooping) do
 
  for _ in !-- rover_command("get distance",ctx?out) do
      retract <| Fsc.Facts.rover_command("get distance", ctx?out) 
  let distance = float (new execCommand<string>("get distance")).output
  tell <| Fsc.Facts.rover_command("get distance",sprintf "%i" (int distance))
  let listchat = (new execCommand<List<int>>("telegram list")).output
  let activechat = listchat |> Seq.length
//  printfn "total active chat:%i" activechat
  for idchat in listchat do 
 //    printfn "\tidchat:%i" idchat

     let messages = (new execCommand<List<Message>>(sprintf "telegram message %i" idchat)).output
     let nMessages = messages |> Seq.length
   //  printfn "\t\tnMessages:%i" nMessages
     if (nMessages>0) then
      let msg = (new execCommand<Message>(sprintf "telegram pop %i" idchat)).output
      let cmd = msg.Txt.ToLower()
      tell <| Fsc.Facts.rover_request(idchat,cmd)
      let outcmd=(new execCommand<string>(cmd)).output
      tell <| Fsc.Facts.rover_command(cmd,outcmd)
  //    for _ in !-- FscContext.rover_response(idchat,ctx?out) do
      let output=(new execCommand<string>((sprintf "telegram text %i" idchat),["text", (sprintf "%s" outcmd)])).output
       
      printfn "\t\t\tidmsg:%s,txt:%s,outcmd:%s,output:%s" msg.Idmsg  msg.Txt outcmd output

do run ()