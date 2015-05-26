module Server

open System.Net
open System.Net.Sockets
open System.Threading.Tasks
open System.IO
open System.Threading
//•Make it run.
//•Make it right.
//•Make it fast.

let mutable clientTaskList : (string * CancellationTokenSource * Task * NetworkStream) list = []

let addClientTask cl = clientTaskList <- cl :: clientTaskList

let writeToClient cl msg =
    let streamWriter = new StreamWriter(stream= cl)
    async{
            do! streamWriter.WriteLineAsync(msg.ToString()) |> Async.AwaitIAsyncResult |> Async.Ignore
            do! streamWriter.FlushAsync() |> Async.AwaitIAsyncResult |> Async.Ignore
    }

let forwadMessageToConnectedClients msg =
        clientTaskList |> List.map(fun (_, _, _, stream) -> writeToClient stream msg)

let removeFirst pred list = 
    let rec removeFirstTailRec p l acc =
        match l with
        | [] -> acc |> List.rev
        | h::t when p h -> (acc |> List.rev) @ t
        | h::t -> removeFirstTailRec p t (h::acc)
    removeFirstTailRec pred list []


let listenForMessages clientId endpoint cl = 
    let listenWorkflow = 
        async { 
            use reader = new System.IO.StreamReader(stream = cl)
            try 
                while true do
                    let! line = reader.ReadLineAsync() |> Async.AwaitTask
                    printfn "MSG from %s: %s" endpoint line
                    do! forwadMessageToConnectedClients (endpoint + ": " + line) |> Async.Parallel |> Async.Ignore
            with _ -> let client = clientTaskList |> List.find (fun (id, _, _, _) -> id = clientId)
                      let  _, cts, task, stream = client
                      cts.Cancel()
                      task.Dispose()
                      stream.Dispose()
                      clientTaskList <- clientTaskList |> removeFirst (fun (id, _, _, _) -> id = clientId)
                      printfn "%s disconnected" endpoint
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
                let id = System.Guid.NewGuid().ToString()
                let cts = new CancellationTokenSource();
                let clientListenTask = Async.StartAsTask (listenForMessages id endpoint (client.GetStream()), cancellationToken = cts.Token)
               
                addClientTask (id, cts, clientListenTask, client.GetStream())
        }
    Async.StartAsTask listenWorkflow
