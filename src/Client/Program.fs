// Learn more about F# at http://fsharp.net
// See the 'F# Tutorial' project for more help.

open Client



[<EntryPoint>]
let main argv = 

    let cl = new Client()

    cl.ConnectTo("127.0.0.1", 8081) 

    while true do
        printf "MSG: "
        let msg = System.Console.ReadLine()
        cl.Write(msg)

    System.Console.ReadLine() |> ignore
    0 // return an integer exit code
