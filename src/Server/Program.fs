// Learn more about F# at http://fsharp.net
// See the 'F# Tutorial' project for more help.
open Server

[<EntryPoint>]
let main argv = 
    
    let listenTask = Server.listen(8081)

    System.Console.ReadLine() |> ignore
    0 // return an integer exit code
