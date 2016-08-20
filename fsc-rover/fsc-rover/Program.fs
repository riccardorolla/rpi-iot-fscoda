[<CoDa.Code>]
module Fsc.App

open CoDa.Runtime
open Fsc.Types
open FSharp.Data
open FSharp.Data.JsonExtensions

open System.Threading
open System

let urlbuild server port command =  sprintf "http://%s:%i/%s" server port command


let (|Prefix|_|) (p:string) (s:string) =
    if s.StartsWith(p) then
        Some(s.Substring(p.Length))
    else
        None
let cmdbuild (text : string)  =
 match text with
 | Prefix "go" rest -> sprintf "/rpi/motor/%s" ((rest).Trim()) 
 | Prefix "led" rest -> 
   match ((rest).Trim()) with
   | Prefix "on" rest -> sprintf "/rpi/led/%s/on" ((rest).Trim()) 
   | Prefix "off" rest -> sprintf "/rpi/led/%s/off" ((rest).Trim()) 
   | _ -> sprintf "neither"
 | Prefix "telegram" rest -> 
   match ((rest).Trim()) with
   | Prefix "photo" rest -> sprintf "/telegram/%s/photo" ((rest).Trim()) 
   | Prefix "video" rest -> sprintf "/telegram/%s/video" ((rest).Trim()) 
   | Prefix "text" rest -> sprintf "/telegram/%s/text" ((rest).Trim()) 
   | _ -> sprintf "neither"
 | Prefix "get" rest -> 
  match ((rest).Trim()) with
   | Prefix "distance" rest -> sprintf "/rpi/distance" 
   | Prefix "photo" rest -> sprintf "/rpi/photo" 
   | Prefix "video" rest -> sprintf "/rpi/video" 
   | _ -> sprintf "neither"
 | Prefix "whatdoyousee" rest -> sprintf "/whatdoyousee"
 | _ -> sprintf "neither"


printfn "cmdbuild:%s" (cmdbuild("led on 1"))
//let cmdbuild (text : string)  =
// let words = text.Split [|' '|]
// let nWords = words.Length
// match words with
//  | [] -> 
//       sprintf "/none"
//  | "go"::xs -> 
//       sprintf "/rpi/motor/%s" xs
 
type Jsonlistchat = JsonProvider<""" [1, 2, 3, 100] """>
type JsonMsgPop = JsonProvider<""" {"idmsg":"82c3b5b7-431a-4b7e-bcc7-70f3b9ba156d","txt":"Message"}""">
type JsonMsg = JsonProvider<""" [{"idmsg":"82c3b5b7-431a-4b7e-bcc7-70f3b9ba156d","txt":"Message"}]""">
let resp = Http.RequestString((urlbuild "localhost" 8081 "telegram/listchat"), silentHttpErrors = true)

if not (resp.Length = 0) then
 let listchat = Jsonlistchat.Parse(resp)
 let activechat = listchat |> Seq.length
 printfn "total active chat:%i" activechat
 for idchat in listchat do 
  printfn "\tidchat:%i" idchat
  let response = Http.RequestString((urlbuild "localhost" 8081 (sprintf "telegram/%i/msg" idchat)), silentHttpErrors = true)
  if not (response.Length = 0) then
   let messages = JsonMsg.Parse(response)
   let nMessages = messages |> Seq.length
   printfn "\t\tnMessages:%i" nMessages
   let response = Http.RequestString((urlbuild "localhost" 8081 (sprintf "telegram/%i/msg/pop" idchat)), silentHttpErrors = true)
   if not (response.Length = 0) then
    let msg=JsonMsgPop.Parse(response)
    printfn "\t\t\tidmsg:%A,txtmsg:%s" msg.Idmsg  msg.Txt
//[<CoDa.ContextInit>]
//let initFacts () =
// tell <| Rpi.Facts.gpio_device("led",pin1)
 
//[<CoDa.Context("fsc-ctx")>]
//[<CoDa.EntryPoint>]
//let main () =
//  initFacts ()

 
 
 // while (true) do
            
//  printfn "exit"
//  driver.Release(led)
//do
 // run ()
  //debug ()