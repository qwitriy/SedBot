module SedBot.Commands

open System.IO
open System.Threading.Tasks

open SedBot.ProcessingChannels

let sed data expression =
    Utilities.runTextProcess "sed" [| "-E"; expression |] data

let jq data expression =
    Utilities.runTextProcess "jq" [| "-M"; expression |] data

let reverse (stream: Stream) =
    task {
        if stream.CanRead then
            use ms = new MemoryStream()
            do! stream.CopyToAsync(ms)
            let tcs = TaskCompletionSource<byte[] voption>()
            do! ffmpegChannel.Writer.WriteAsync({ Stream = ms; Tcs = tcs })
            return! tcs.Task
        else
            return ValueNone
    }

let hFlip (stream: Stream) =
    task {
        if stream.CanRead then
            use ms = new MemoryStream()
            do! stream.CopyToAsync(ms)
            let tcs = TaskCompletionSource<byte[] voption>()
            do! ffmpegHflipChannel.Writer.WriteAsync({ Stream = ms; Tcs = tcs })
            return! tcs.Task
        else
            return ValueNone
    }

let vFlip (stream: Stream) =
    task {
        if stream.CanRead then
            use ms = new MemoryStream()
            do! stream.CopyToAsync(ms)
            let tcs = TaskCompletionSource<byte[] voption>()
            do! ffmpegVflipChannel.Writer.WriteAsync({ Stream = ms; Tcs = tcs })
            return! tcs.Task
        else
            return ValueNone
    }

let distort (stream: Stream) =
    task {
        if stream.CanRead then
            use ms = new MemoryStream()
            do! stream.CopyToAsync(ms)
            let tcs = TaskCompletionSource<byte[] voption>()
            do! magicChannel.Writer.WriteAsync({ Stream = ms; Tcs = tcs })
            return! tcs.Task
        else
            return ValueNone
    }