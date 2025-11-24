module InputHandling

open System
open System.IO
open System.Text

let loadFromFile (filePath: string) : Result<string, string> =
    if File.Exists(filePath) then
        try
            let bytes = File.ReadAllBytes(filePath)
            let utf8 = Encoding.UTF8
            let decoder = utf8.GetDecoder()
            decoder.Fallback <- DecoderFallback.ExceptionFallback
            let charCount = decoder.GetCharCount(bytes, 0, bytes.Length)
            let chars = Array.zeroCreate<char> charCount
            decoder.GetChars(bytes, 0, bytes.Length, chars, 0) |> ignore
            let content = String(chars)
            if String.IsNullOrWhiteSpace(content) then
                Error "File is empty."
            else
                Ok content
        with
        | :? DecoderFallbackException ->
            Error "File contains invalid UTF-8 encoding."
        | ex ->
            Error ("Error reading file: " + ex.Message)
    else
        Error "File does not exist."

let readFromConsole () : Result<string, string> =
    printfn "Enter/paste your text below (end with an empty line):"
    let lines =
        seq {
            let mutable input = Console.ReadLine()
            while not (String.IsNullOrWhiteSpace(input)) do
                yield input
                input <- Console.ReadLine()
        } |> Seq.toArray
    let text = String.Join("\n", lines)
    if String.IsNullOrWhiteSpace(text) then
        Error "No text entered."
    else
        Ok text