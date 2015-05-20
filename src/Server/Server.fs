module Server

open System.Net
open System.Net.Sockets
open System.Threading.Tasks
open System.IO

//•Make it run.
//•Make it right.
//•Make it fast.

let mutable clientTaskList : Task list = []

let addClientTask cl = clientTaskList <- cl :: clientTaskList

let listenForMessages endpoint cl = 
    let listenWorkflow = 
        async { 
            use reader = new System.IO.StreamReader(stream = cl)
            try 
                while true do
                    let! line = reader.ReadLineAsync() |> Async.AwaitTask
                    printfn "MSG from %s: %s" endpoint line
            with ex -> printfn "%s disconnected" endpoint
        }
    listenWorkflow

let listen port = 
    let listenWorkflow = 
        async { 
            let listener = new TcpListener(IPAddress.Any, port)
            listener.Start()
            printfn "start listening on port %i" port
            while true do
                let! client = listener.AcceptTcpClientAsync() |> Async.AwaitTask
                let endpoint = (client.Client.RemoteEndPoint :?> IPEndPoint).Address.ToString()
                printfn "client connected: %A" endpoint
                let clientListenTask = listenForMessages endpoint (client.GetStream()) |> Async.StartAsTask
                addClientTask clientListenTask
        }
    Async.StartAsTask listenWorkflow
