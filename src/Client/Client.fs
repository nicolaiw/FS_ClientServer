module Client

open System.Net.Sockets
open System.IO
open System.Threading.Tasks

let awaitTaskVoid : Task -> Async<unit> = Async.AwaitIAsyncResult >> Async.Ignore

type Client() = 
    let tcpClient = new TcpClient() // tcpClient is private since this is an internal let binding 
    
    member this.ConnectTo(host, port) = async { do! awaitTaskVoid (tcpClient.ConnectAsync(host = host, port = port)) } |> Async.RunSynchronously
    member this.Write msg = 
        let streamWriter = new StreamWriter(tcpClient.GetStream())
        match tcpClient.Connected with
        | true -> 
            async { 
                do! awaitTaskVoid (streamWriter.WriteLineAsync(msg.ToString()))
                do! awaitTaskVoid (streamWriter.FlushAsync())
            }
            |> Async.StartAsTask
        | _ ->  streamWriter.Dispose()
                failwith "client not connected"

