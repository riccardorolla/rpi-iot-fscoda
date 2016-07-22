// Learn more about F# at http://fsharp.net
[<CoDa.Code>]
module Rpi.Test
open Raspberry.IO.GeneralPurpose;
open CoDa.Runtime
open Rpi.Types

open System.Threading
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
//let led = ConnectorPin.P1Pin11.ToProcessor()
//let button = ConnectorPin.P1Pin12.ToProcessor()
let pin2 = ConnectorPin.P1Pin12.ToProcessor()


let driver = GpioConnectionSettings.DefaultDriver

let get_value (pin:ProcessorPin) = driver.Read(pin)


[<CoDa.ContextInit>]
let initFacts () =
 tell <| Rpi.Facts.gpio_device("led",pin1)
 tell <| Rpi.Facts.gpio_direction(pin1,PinDirection.Output)
 
 tell <| Rpi.Facts.gpio_device("button",pin2)
 tell <| Rpi.Facts.gpio_direction(pin2,PinDirection.Input)
 tell <| Rpi.Facts.gpio_resistor(pin2,PinResistor.PullUp)     
           
[<CoDa.Context("rpi-ctx")>]
[<CoDa.EntryPoint>]
let main () =
  initFacts ()

 // for _ in !-- gpio_direction(ctx?pin,PinDirection.Output) do
   //  
 //        driver.Allocate (ctx?pin, (get_gpio_direction ctx?pin))
  for _ in !-- gpio_direction(ctx?pin,ctx?direction) do
         driver.Allocate (ctx?pin, ctx?direction)

  for _ in !-- gpio_resistor(ctx?pin,ctx?resistor) do
         driver.SetPinResistor(ctx?pin, ctx?resistor)
  

  

  let mutable light = true;
  while (true) do
                for _ in !-- gpio_direction(ctx?pin,PinDirection.Input) do
                        retract <| Rpi.Facts.gpio_digital(ctx?pin, get_gpio_digital ctx?pin)                    
                        tell <| Rpi.Facts.gpio_digital(ctx?pin,get_value(ctx?pin))

                for _ in !-- gpio_direction(ctx?pin,PinDirection.Output) do
                        driver.Write(ctx?pin, (get_gpio_digital ctx?pin))
                        retract<| Rpi.Facts.gpio_digital(ctx?pin,get_gpio_digital ctx?pin)
                
                tell <| Rpi.Facts.gpio_digital(pin1,get_gpio_digital pin2)

               // driver.Write(pin1, (get_gpio_digital pin1))
               // light <- not light
              //  Thread.Sleep(1000)
   
   
  
  //printfn "%s" (get_gpio "1")

//  driver.Allocate(led, PinDirection.Output)
//  driver.Allocate(button,PinDirection.Input)
//  driver.SetPinResistor(button,PinResistor.PullUp)
//  while true do
//    if driver.Read(button)  then
 //               driver.Write(led, true)
               // printf "led on...n" 
             //   tell <| button_press("1") 
   //          else
       //         tell <| button_press("0") 
     //           driver.Write(led, false)
               // printf "...led off\n" 
         
  printfn "exit"
//  driver.Release(led)
do
  run ()
  //debug ()