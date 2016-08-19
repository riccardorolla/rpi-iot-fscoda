module Fsc.Facts

open CoDa

let inline private fact head (x:string, y: string) =
  Fact(head, [| x :> obj ; y :> obj |])


let rover_motor(x:string) = Fact("rover_motor_", [| x:> obj |])
let rover_led(x:int,y:bool) = Fact("rover_led_", [| x:> obj ; y :>obj |])
let rover_request(x:int,y:string) = Fact("rover_request_", [| x:> obj ; y :>obj |])
let rover_command(x:string,y:string) = Fact("rover_command_", [| x:> obj ; y :>obj |])
let rover_distance(x:string,y:bool) = Fact("rover_distance_",[|x:> obj; y :>obj |])