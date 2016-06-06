// Learn more about F# at http://fsharp.net
// See the 'F# Tutorial' project for more help.
open System.Threading

let testAsync() = 
    let counter = ref 0m
    let IncrGlobalCounter numberOfTimes = 
        for i in 1 .. numberOfTimes do
            lock counter (fun () -> counter := !counter + 1m)
            (* lock is a function in F# library. It automatically unlocks "counter" when 'fun () -> ...' completes *)

    let AsyncIncrGlobalCounter numberOfTimes =
        new Thread(fun () -> IncrGlobalCounter(numberOfTimes))

    let t1 = AsyncIncrGlobalCounter 1000000
    let t2 = AsyncIncrGlobalCounter 1000000
    t1.Start() // runs t1 asyncronously
    t2.Start() // runs t2 asyncronously
    t1.Join()  // waits until t1 finishes
    t2.Join()  // waits until t2 finishes
[<EntryPoint>]
let main argv = 
    Async.Start
    testAsync()
    printfn "%A" argv
    0 // return an integer exit code

