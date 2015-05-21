// Learn more about F# at http://fsharp.net
// See the 'F# Tutorial' project for more help.

open Client



[<EntryPoint>]
let main argv = 

    use cl = new Client()

    let t = cl.ConnectTo("127.0.0.1", 8081) 
    while true do
        printf "Msg: "
        let msg = System.Console.ReadLine()
        cl.Write(msg) |> ignore
        

    0 // return an integer exit code
