module Fsc.Facts

open CoDa

let inline private fact head (x:string, y: string) =
  Fact(head, [| x :> obj ; y :> obj |])


let request = fact "request_"
let observe = fact "observe_"
let execute = fact "execute_" 
let then_next = fact "then_next_"
let else_next = fact "else_next_"