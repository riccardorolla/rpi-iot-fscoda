module Fsc.Facts

open CoDa

let inline private fact head (x:string, y: string) =
  Fact(head, [| x :> obj ; y :> obj |])


let request (x:string,y:string,z:string)  = Fact  ( "request_", [| x :> obj ; y :> obj ; z :> obj|])
let found = fact "found_"
let result = fact "result_" 
let user_command = fact "user_command_" 
let rule (x:string,y:string,z:string) = Fact  ("rule_", [| x :> obj ; y :> obj ; z :> obj|])
 