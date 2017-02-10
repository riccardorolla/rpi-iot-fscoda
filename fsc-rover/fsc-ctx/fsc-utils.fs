module Fsc.Utils

open System
open System.IO
open FSharp.Data
open Newtonsoft.Json

type Configuration =
 {
  server:string;
  port:int;
  debug:bool
 }


type Message = 
    {
      idmsg:string; 
      txt:string
    }

 
type ImageRecognition =
  { 
    tags:List<Tag>;
    description: Description ;
    requestId: string;
    metadata: Meta
    
  }
and Tag = 
  {
    name:string;
    confidence:double
  }
and Description =
  { 
    tags:List<string>;
    captions: List<Caption>
  }
and Caption =
  { 
    text:string;
    confidence:double
  }
and Meta =
  { 
    width:int;
    height:int;
    format:string
  }
type num=int

  
let confjson =
 try  
  File.ReadAllText(@"fsc-rover.json")
 with
  | :? System.IO.FileNotFoundException  -> 
        File.WriteAllText(@"fsc-rover.json","""{"server":"localhost","port":8081,"debug":false}""")
        """{"server":"localhost","port":8081,"debug":false}"""




let conf = JsonConvert.DeserializeObject<Configuration>(confjson)

printfn "server:%s,port:%i" conf.server conf.port

let get_list str =
      try
     
       JsonConvert.DeserializeObject<List<int>>(str)
      with e ->  [] 

let urlbuild command =  sprintf "http://%s:%i/%s" conf.server conf.port command


let imagerecognition str =
  try
   JsonConvert.DeserializeObject<ImageRecognition>(str)
  with e ->    JsonConvert.DeserializeObject<ImageRecognition>("{\"tags\":[],\"description\":{\"tags\":[],\"captions\":[]},requestId:\"\",metadata:{width:0,height:0,format:\"null\"}}")


let (|Prefix|_|) (p:string) (s:string) =
    if s.StartsWith(p) then
        Some(s.Substring(p.Length))
    else
        None

let command  cmd q =   
   // printfn "command %s" cmd
    if (cmd="nop") then sprintf "OK"
    else
     let resp=try 
               Http.RequestString((urlbuild  cmd ), q , headers = [ "Cache-Control","NoCache" ])
              with 
              | :? System.Net.WebException ->    "error"
     printf "command %s -> %s" cmd resp
     resp
let get_message idchat =
           try
            let msg=command (sprintf "telegram/%i/next" idchat) []
            let out=JsonConvert.DeserializeObject<Message>(msg)
            out.txt.ToLower().Split ' '
        
           with e-> [||]   


let caption str =  
                try
                 let imageinfo = str |> imagerecognition
                 sprintf  "%s" imageinfo.description.captions.[0].text 
                with e-> "no desc"             
          