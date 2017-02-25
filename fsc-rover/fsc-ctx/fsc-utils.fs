module Fsc.Utils

open System
open System.IO
open FSharp.Data
open Newtonsoft.Json

type Configuration ={
  http_server:string;
  debug:bool
 }



let confjson =
  let defaultconf =
        "{\"http_server\":\"http://localhost:8081\","+
         "\"debug\":false}"
  try  
   File.ReadAllText(@"fsc-rover.json")
  with
   | :? System.IO.FileNotFoundException  -> 
         File.WriteAllText(@"fsc-rover.json",
          defaultconf)
         defaultconf

let conf = 
  JsonConvert.DeserializeObject<Configuration>(confjson)
printfn "Required rpi_service.js:%s " conf.http_server  

type ImageRecognition =  { 
    tags:List<Tag>;
    description: Description;
    requestId: string;
    metadata: Meta       } 
and Tag = {
    name:string;
    confidence:double    } 
and Description = { 
    tags:List<string>;
    captions: List<Caption> }
and Caption =  { 
    text:string;
    confidence:double }
and Meta = { 
    width:int;
    height:int;
    format:string
}

let imagerecognition str =
  try
   JsonConvert.DeserializeObject<ImageRecognition>(str)
  with e -> JsonConvert.DeserializeObject<ImageRecognition>(
             "{\"tags\":[],\"description\":"+
             "{\"tags\":[],\"captions\":[]},"+
              "requestId:\"\",metadata:"+
              "{width:0,height:0,format:\"null\"}}")


type num=int


let caption str =  
 try
  let imageinfo = str |> imagerecognition
  sprintf  "%s" imageinfo.description.captions.[0].text 
 with e-> "no desc"   



let command  cmd q =   
  try 
    Http.RequestString(
     (sprintf "%s%s" conf.http_server  cmd), 
     q , headers = [ "Cache-Control","NoCache" ])
  with  | :? System.Net.WebException ->    "error"

let execute syscmd param  = 
  async {
   let result =   sprintf "%s" (command  syscmd param) 
   return syscmd,result
  }

type Message = {
      idmsg:string; 
      txt:string
      } 
let get_message idchat =
 try
  let msg=command (sprintf "/telegram/%i/next" idchat) []
  let out=JsonConvert.DeserializeObject<Message>(msg)
  out.txt.ToLower().Split ' '
 with e-> [||]   

let get_list str =
 try
  JsonConvert.DeserializeObject<List<int>>(str)
 with e ->  [] 

          
          