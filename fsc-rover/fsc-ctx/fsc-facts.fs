module Fsc.Facts

open CoDa

let inline private fact head (x:string, y: string) =
  Fact(head, [| x :> obj ; y :> obj |])


let request (x:int,y:string)  = Fact  ( "request_", [| x :> obj ; y :> obj |])
let recognition (x:string,y:float) =  Fact  ( "recognition_", [| x :> obj ; y :> obj|])
let result (x:string,y:string)  = Fact  ( "result_", [| x :> obj ; y :> obj |])
let cmddesc(x:string,y:string) = Fact  ( "cmddesc_", [| x :> obj; y:>obj |])
let usrcmd(x:string,y:string) = Fact  ( "usrcmd_", [| x :> obj; y:>obj |])
let confidence(x:string,y:float,z:float) = Fact  ( "confidence_", [| x :> obj; y:>obj; z:> obj |])
let action (x:string,y:string,z:string) = Fact  ("action_", [| x :> obj ; y :> obj ; z :> obj|])
let objcmd (x:string,y:string)  = Fact  ( "objcmd_", [| x :> obj ; y :> obj |])
 