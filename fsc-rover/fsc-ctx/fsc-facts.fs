module Fsc.Facts

open CoDa

let inline private fact head (x:string, y: string) =
  Fact(head, [| x :> obj ; y :> obj |])


let request (x:string,y:string,z:string)  = Fact  ( "request_", [| x :> obj ; y :> obj ; z :> obj|])
let found (x:string,y:bool) =  Fact  ( "found_", [| x :> obj ; y :> obj|])
let result = fact "result_" 
let user_command = fact "user_command_" 
let confidence(x:string,y:float,z:float) = Fact  ( "confidence_", [| x :> obj; y:>obj; z:> obj |])
let rule (x:string,y:bool,z:string) = Fact  ("rule_", [| x :> obj ; y :> obj ; z :> obj|])
 