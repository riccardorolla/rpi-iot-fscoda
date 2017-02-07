module Fsc.Facts

open CoDa

let inline private fact head (x:string, y: string) =
  Fact(head, [| x :> obj ; y :> obj |])


let request (x:string,y:string,z:string)  = Fact  ( "request_", [| x :> obj ; y :> obj ; z :> obj|])
let found (x:string,y:bool) =  Fact  ( "found_", [| x :> obj ; y :> obj|])
let result = fact "result_" 
let synopsis = fact "synopsis_" 
let confidence(x:string,y:float,z:float) = Fact  ( "confidence_", [| x :> obj; y:>obj; z:> obj |])
let action (x:string,y:bool,z:string) = Fact  ("action_", [| x :> obj ; y :> obj ; z :> obj|])
 