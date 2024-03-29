﻿open System
open System.IO
open System.Linq
open System.Net.Http

open System.Threading
open System.Threading.Tasks
open Funogram.Api
open Funogram.Telegram
open Funogram.Telegram.Bot
open Funogram.Telegram.Types
open Newtonsoft.Json
open SedBot
open SedBot.ChatCommands
open SedBot.Utilities
open Microsoft.Extensions.Logging

module Api =
    let sendAnimationReply chatId animation replyToMessageId = Req.SendAnimation.Make(ChatId.Int chatId, animation, replyToMessageId = replyToMessageId)
    let sendVideoReply chatId videoFile replyToMessageId = Req.SendVideo.Make(ChatId.Int chatId, videoFile, replyToMessageId = replyToMessageId)
    let sendTextMarkupReply chatId text replyToMessageId parseMode = Req.SendMessage.Make(ChatId.Int chatId, text, replyToMessageId = replyToMessageId, parseMode = parseMode)
    let sendPhotoReply chatId photo replyToMessageId = Req.SendPhoto.Make(ChatId.Int chatId, photo, replyToMessageId = replyToMessageId)

let updateArrived (ctx: UpdateContext) =
    task {
        match CommandParser.parse ctx.Update.Message with
        | SedCommand (chatId, replyMsgId, srcMsgId, exp, text) ->
            let! res = Commands.sed text exp
            match res with
            | Some res ->
                do! Api.deleteMessage chatId srcMsgId |> api ctx.Config |> Async.Ignore
                do! Api.sendMessageReply chatId res replyMsgId |> api ctx.Config |> Async.Ignore
            | _ ->
                ()
        | JqCommand (chatId, msgId, data, exp) ->
            let! res = Commands.jq data exp
            match res with
            | Some res ->
                let res = $"```\n{res}\n```"
                do! Api.sendTextMarkupReply chatId res msgId ParseMode.Markdown |> api ctx.Config |> Async.Ignore
            | _ ->
                ()
        | ReverseCommand (chatId, msgId, fileId, fileType) ->
            let! file = Api.getFile fileId |> api ctx.Config
            match file with
            | Ok { FilePath = Some filePath } ->
                use hc = new HttpClient()
                let! srcStream = hc.GetStreamAsync($"https://api.telegram.org/file/bot{ctx.Config.Token}/{filePath}")
                match! Commands.reverse srcStream fileType with
                | ValueSome resStream ->
                    let extension = extension fileType
                    let synthName = Utilities.Path.getSynthName extension
                    let ms = new MemoryStream(resStream)
                    ms.Position <- 0
                    let ani = InputFile.File (synthName, ms)
                    match fileType with
                    | Gif | Sticker ->
                        do! Api.sendAnimationReply chatId ani msgId |> api ctx.Config |> Async.Ignore
                    | Video ->
                        do! Api.sendVideoReply chatId ani msgId |> api ctx.Config |> Async.Ignore
                    | _ ->
                        do! Api.sendPhotoReply chatId ani msgId |> api ctx.Config |> Async.Ignore
                | _ -> ()
            | _ -> ()
        | VflipCommand (chatId, msgId, fileId, fileType) ->
            let! file = Api.getFile fileId |> api ctx.Config
            match file with
            | Ok { FilePath = Some filePath } ->
                use hc = new HttpClient()
                let! srcStream = hc.GetStreamAsync($"https://api.telegram.org/file/bot{ctx.Config.Token}/{filePath}")
                match! Commands.vFlip srcStream fileType with
                | ValueSome resStream ->
                    let extension = extension fileType
                    let synthName = Utilities.Path.getSynthName extension
                    let ms = new MemoryStream(resStream)
                    ms.Position <- 0
                    let ani = InputFile.File (synthName, ms)
                    match fileType with
                    | Gif | Sticker ->
                        do! Api.sendAnimationReply chatId ani msgId |> api ctx.Config |> Async.Ignore
                    | Video ->
                        do! Api.sendVideoReply chatId ani msgId |> api ctx.Config |> Async.Ignore
                    | _ ->
                        do! Api.sendPhotoReply chatId ani msgId |> api ctx.Config |> Async.Ignore
                | _ -> ()
            | _ -> ()
        | HflipCommand (chatId, msgId, fileId, fileType) ->
            let! file = Api.getFile fileId |> api ctx.Config
            match file with
            | Ok { FilePath = Some filePath } ->
                use hc = new HttpClient()
                let! srcStream = hc.GetStreamAsync($"https://api.telegram.org/file/bot{ctx.Config.Token}/{filePath}")
                match! Commands.hFlip srcStream fileType with
                | ValueSome resStream ->
                    let extension = extension fileType
                    let synthName = Utilities.Path.getSynthName extension
                    let ms = new MemoryStream(resStream)
                    ms.Position <- 0
                    let ani = InputFile.File (synthName, ms)
                    match fileType with
                    | Gif | Sticker ->
                        do! Api.sendAnimationReply chatId ani msgId |> api ctx.Config |> Async.Ignore
                    | Video ->
                        do! Api.sendVideoReply chatId ani msgId |> api ctx.Config |> Async.Ignore
                    | _ ->
                        do! Api.sendPhotoReply chatId ani msgId |> api ctx.Config |> Async.Ignore
                | _ -> ()
            | _ -> ()
        | DistortCommand (chatId, msgId, fileId, fileType) ->
            let! file = Api.getFile fileId |> api ctx.Config
            match file with
            | Ok { FilePath = Some filePath } ->
                use hc = new HttpClient()
                let! srcStream = hc.GetStreamAsync($"https://api.telegram.org/file/bot{ctx.Config.Token}/{filePath}")
                match! Commands.distort srcStream fileType with
                | ValueSome resStream ->
                    let extension = extension fileType
                    let synthName = Utilities.Path.getSynthName extension
                    let ms = new MemoryStream(resStream)
                    ms.Position <- 0
                    let ani = InputFile.File (synthName, ms)
                    match fileType with
                    | Gif | Sticker ->
                        do! Api.sendAnimationReply chatId ani msgId |> api ctx.Config |> Async.Ignore
                    | Video ->
                        do! Api.sendVideoReply chatId ani msgId |> api ctx.Config |> Async.Ignore
                    | _ ->
                        do! Api.sendPhotoReply chatId ani msgId |> api ctx.Config |> Async.Ignore
                | _ -> ()
            | _ -> ()
            ()
        | ClownCommand(chatId, count) ->
            do! Api.sendMessage chatId (System.String.Concat(Enumerable.Repeat("🤡", count))) |> api ctx.Config |> Async.Ignore
        | InfoCommand(chatId, msgId, reply) ->
            do! Api.deleteMessage chatId msgId |> api ctx.Config |> Async.Ignore
            let res =  $"```\n{JsonConvert.SerializeObject(reply, Formatting.Indented, JsonSerializerSettings(NullValueHandling = NullValueHandling.Ignore))}\n```"
            let! x = Api.sendTextMarkupReply chatId res reply.MessageId ParseMode.Markdown |> api ctx.Config
            match x with
            | Ok { MessageId = msgId } ->
                do! Task.Delay(6000)
                do! Api.deleteMessage chatId msgId |> api ctx.Config |> Async.Ignore
            | _ -> ()
        | Nope ->
            ()
    } |> ignore

[<EntryPoint>]
let main args =
    let logger = Logger.get "EntryPoint"
    if args.Length = 0 then
        logger.LogCritical("Usage: {execName} yourtelegramtoken", AppDomain.CurrentDomain.FriendlyName)
        Environment.Exit(-1)
    let token = args[0]

    ProcessingChannels.start()
    while true do
        try
            task {
                let config = { Config.defaultConfig with Token = token }
                let! _ = Api.deleteWebhookBase () |> api config
                return! startBot config updateArrived None
            } |> fun x -> x.ConfigureAwait(false).GetAwaiter().GetResult()
        with
        | e ->
            logger.LogCritical("Something goes wrong: {error}", e)
            Thread.Sleep(5000)
    0
