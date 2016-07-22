
module Rpi.Types

open CoDa



[<TypedPred>]
[<Code>]
let gpio_device(s:string,m:obj ) =
  if true then
    failwith "Solved by JIT"
  else
    Rpi.RpiContext.gpio_device(s,m)

[<TypedPred>]
[<Code>]
let gpio_digital(m:obj, e:obj) =
  if true then
    failwith "Solved by JIT"
  else
    Rpi.RpiContext.gpio_digital(m, e)

[<TypedPred>]
[<Code>]
let gpio_resistor(m:obj, e:obj) =
  if true then
    failwith "Solved by JIT"
  else
    Rpi.RpiContext.gpio_resistor(m, e)

[<TypedPred>]
[<Code>]
let gpio_direction(m:obj, e:obj) =
  if true then
    failwith "Solved by JIT"
  else
    Rpi.RpiContext.gpio_direction(m, e)

