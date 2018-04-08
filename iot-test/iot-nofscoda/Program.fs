module Rpi.Test
open Raspberry.IO.GeneralPurpose;
open System

let led = ConnectorPin.P1Pin11.ToProcessor()
let button = ConnectorPin.P1Pin40.ToProcessor()

let driver = GpioConnectionSettings.DefaultDriver

let timeNow = System.DateTime.Now.ToLongTimeString()
let main () =
  driver.Allocate(led,PinDirection.Output)
  driver.Allocate(button,PinDirection.Input)
  driver.SetPinResistor(button,PinResistor.PullUp)
  while true do
     
    driver.Write(led, driver.Read(button))

    printfn "time:%s %d" (System.DateTime.Now.ToLongTimeString()) (System.DateTime.Now.Millisecond)
main ()