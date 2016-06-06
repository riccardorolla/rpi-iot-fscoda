module Rpi.Facts

open CoDa

let inline private fact head (x:string, y: string) =
  Fact(head, [| x :> obj ; y :> obj |])
let gpio_direction (x:obj,y:obj) = Fact("gpio_direction_", [| x  ; y |])
let gpio_digital (x:obj,y:bool) = Fact("gpio_digital_", [| x ; y :>obj |])
let gpio_resistor (x:obj,y:obj) = Fact("gpio_resistor_", [|x ; y |])
let gpio_device (x:string,y:obj) =Fact("gpio_device_", [| x :> obj ; y |])

