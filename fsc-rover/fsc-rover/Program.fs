[<CoDa.Code>]
module Fsc.App
 
open System
open System.IO
open CoDa.Runtime
open Fsc.Types
open FSharp.Data

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


type execCommand (cmd :string , ?q0) =
 let q = defaultArg q0 []
 let resp = Http.RequestString((urlbuild conf.Server conf.Port (cmdbuild(cmd))), q,  silentHttpErrors = true)
 member this.output 
  with get() = sprintf "%s" (resp)

//printfn "%s" (new execCommand ("translate",["src","it";"dst","en";"text","ciao ciao"])).output
//printfn "%s" (new execCommand ("telegram text 189202739",["text",(new execCommand ("translate",["src","en";"dst","it";"text",(new execCommand ("whatdoyousee")).output])).output])).output


type Message(idmsg:string, txt:string) = 
  member this.Idmsg=idmsg
  member this.Txt=txt
  


 

[<CoDa.ContextInit>]
let initFacts () =
 tell <| Fsc.Facts.rover_distance("1",true)
 

[<CoDa.Context("fsc-ctx")>]
[<CoDa.EntryPoint>]
let main () =
 initFacts ()
 let resp = Http.RequestString((urlbuild conf.Server conf.Port "telegram/listchat"), silentHttpErrors = true)
 printfn "%s" resp
 if not (resp.Length = 0) then
  let listchat = JsonConvert.DeserializeObject<List<int>>(resp)
  let activechat = listchat |> Seq.length
  printfn "total active chat:%i" activechat
  for idchat in listchat do 
   printfn "\tidchat:%i" idchat
   let response = Http.RequestString((urlbuild conf.Server conf.Port (sprintf "telegram/%i/msg" idchat)), silentHttpErrors = true)
   if not (response.Length = 0) then
    let messages = JsonConvert.DeserializeObject<List<Message>>(response)
    let nMessages = messages |> Seq.length
    printfn "\t\tnMessages:%i" nMessages
    let response = Http.RequestString((urlbuild conf.Server conf.Port (sprintf "telegram/%i/msg/pop" idchat)), silentHttpErrors = true)
    if not (response.Length = 0) then
     let msg=JsonConvert.DeserializeObject<Message>(response)
     let cmd=cmdbuild (msg.Txt)
     tell <| Fsc.Facts.rover_request(idchat,cmd)
     let outcmd=(new execCommand(cmd)).output
     tell <| Fsc.Facts.rover_command(cmd,outcmd)
     printfn "\t\t\tidmsg:%s,txt:%s" msg.Idmsg  msg.Txt

do run ()
 // debug ()