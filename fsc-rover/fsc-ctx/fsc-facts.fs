module Fsc.Facts

open CoDa

let inline private fact head (x:string, y: string) =
  Fact(head, [| x :> obj ; y :> obj |])


let request = fact "request_"
let observe = fact "observe_"
let execute = fact "execute_" 
let rule (x:string,y:string,z:string) = Fact  ("rule_", [| x :> obj ; y :> obj ; z :> obj|])
 