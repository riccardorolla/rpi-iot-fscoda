[<CoDa.Code>]
module Rpi.Test
open Raspberry.IO.GeneralPurpose;
open CoDa.Runtime
open Rpi.Types
open System

let get_gpio_resistor pin = 
    try 
        let dr = ctx?resistor |- (gpio_resistor(pin,ctx?resistor))
        dr
    with e->
         PinResistor.None

let get_gpio_direction pin = 
    try 
        let dr = ctx?direction |- (gpio_direction(pin,ctx?direction))
        dr
    with e->
          PinDirection.Output

let get_gpio_digital pin = 
    try 
        let dg = ctx?status |- (gpio_digital(pin,ctx?status))
        dg
    with e->
          false

let pin1 = ConnectorPin.P1Pin11.ToProcessor()
let pin2 = ConnectorPin.P1Pin40.ToProcessor()
let driver = GpioConnectionSettings.DefaultDriver
let get_value (pin:ProcessorPin) = driver.Read(pin)

let updateGPIO  = async {
         while (true) do
                for _ in !-- gpio_direction(ctx?pin,PinDirection.Input) do
                        retract <| Rpi.Facts.gpio_digital(ctx?pin, not (get_value(ctx?pin)))                    
                        tell <| Rpi.Facts.gpio_digital(ctx?pin,get_value(ctx?pin))

                for _ in !-- gpio_direction(ctx?pin,PinDirection.Output) do
                        driver.Write(ctx?pin, (get_gpio_digital ctx?pin))                
                        tell<| Rpi.Facts.gpio_digital(ctx?pin,get_gpio_digital ctx?pin)
                        retract <| Rpi.Facts.gpio_digital(ctx?pin,not (get_gpio_digital ctx?pin))
                        }


[<CoDa.ContextInit>]
let initFacts () =
 tell <| Rpi.Facts.gpio_device("led",pin1)
 tell <| Rpi.Facts.gpio_direction(pin1,PinDirection.Output)
 
 tell <| Rpi.Facts.gpio_device("button",pin2)
 tell <| Rpi.Facts.gpio_direction(pin2,PinDirection.Input)
 tell <| Rpi.Facts.gpio_resistor(pin2,PinResistor.PullUp)
 
 for _ in !-- gpio_direction(ctx?pin,ctx?direction) do
         driver.Allocate (ctx?pin, ctx?direction)

 for _ in !-- gpio_resistor(ctx?pin,ctx?resistor) do
         driver.SetPinResistor(ctx?pin, ctx?resistor)

[<CoDa.Context("rpi-ctx")>]
[<CoDa.EntryPoint>]
let main () =
  initFacts ()
  Async.Start(updateGPIO)
  while (true) do
                match ctx with
                 | _ when !- (button(ctx?status), gpio_device("led",ctx?pin)) ->  
                                retract <| Rpi.Facts.gpio_digital(ctx?pin,not (ctx?status))
                                tell <| Rpi.Facts.gpio_digital(ctx?pin,ctx?status)
                 | _ -> printfn "no op"
                printfn "time:%s %d" (System.DateTime.Now.ToLongTimeString()) (System.DateTime.Now.Millisecond) 
do
  run ()
