// Learn more about F# at http://fsharp.net
// See the 'F# Tutorial' project for more help.
open FSharp.Data
open FSharp.Data.JsonExtensions
type Json = JsonProvider<"pop.json">
 
[<EntryPoint>]
let main argv = 
    printfn "%A" argv
    let response = Http.RequestString("http://localhost:8081/pop", silentHttpErrors = true)
    if not (response.Length = 0) then
        let info = JsonValue.Parse(response)

        let chatid =  info?chatId.AsString()
        
        Http.RequestString(sprintf "http://localhost:8081/text/%s" chatid , query = ["text","prova"], silentHttpErrors = true) |> ignore
       
    0 // return an integer exit code

