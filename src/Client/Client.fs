module Client

open System.Net
open System.Net.Sockets
open System.IO
open System.Threading.Tasks

let awaitTaskVoid = Async.AwaitIAsyncResult >> Async.Ignore

type Client() = 

    let tcpClient = new TcpClient() // tcpClient is private since this is an internal let binding

    member private this.listenAsync() =
        async{
            let stream = tcpClient.GetStream()
            use streamReader = new StreamReader(stream)

            while true do
                let! line = streamReader.ReadLineAsync() |> Async.AwaitTask
                printf "\nINC MSG FROM %s: %s\n" ((tcpClient.Client.RemoteEndPoint :?> IPEndPoint).Address.ToString()) line
        }

    member this.ConnectTo(host, port) = 
        async { do! awaitTaskVoid (tcpClient.ConnectAsync(host = host, port = port))
                this.listenAsync () |> Async.StartAsTask |> ignore
        } |> Async.StartAsTask
    
    member this.Write msg = 
        match tcpClient.Connected with
        | true -> 
            async {
                let streamWriter = new StreamWriter(tcpClient.GetStream()) //use would close the baseStream --> dispose the stream elsewhere
                do! awaitTaskVoid (streamWriter.WriteLineAsync(msg.ToString()))
                do! awaitTaskVoid (streamWriter.FlushAsync())
            }
            |> Async.StartAsTask 
        | _ -> failwith "client not connected"
    
    interface System.IDisposable with
        member this.Dispose() = tcpClient.Close()
