module Fsc.Facts

open CoDa

let inline private fact head (x:string, y: string) =
  Fact(head, [| x :> obj ; y :> obj |])



let rover_request(x:int,y:string) = Fact("rover_request_", [| x:> obj ; y :>obj |])
let rover_command = fact "rover_command_" 
let rover_validate(x:string,y:bool) = Fact("rover_validate_",[|x:> obj; y :>obj |])