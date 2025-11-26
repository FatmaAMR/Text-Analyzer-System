module Tokenizer

open System
open System.Text
open System.Text.RegularExpressions

/// Splits text into paragraphs based on double newlines
let tokenizeParagraphs (text: string) : string list =
    Regex.Split(text, @"\r\n\r\n|\n\n")
    |> Array.toList
    |> List.map (fun p -> p.Trim())
    |> List.filter (fun p -> not (String.IsNullOrWhiteSpace(p)))

/// Splits text into sentences using standard punctuation delimiters
let tokenizeSentences (text: string) : string list =
    // Pattern matches ending punctuation (. ! ?) followed by whitespace or end of string
    Regex.Split(text, @"(?<=[.!?])\s+")
    |> Array.toList
    |> List.map (fun s -> s.Trim())
    |> List.filter (fun s -> not (String.IsNullOrWhiteSpace(s)))

let tokenizeWords (text: string) : string list =
    if String.IsNullOrWhiteSpace text then []
    else
        let sb = StringBuilder()
        let tokens = ResizeArray<string>()

        let flush() =
            if sb.Length > 0 then
                let tok = sb.ToString().Trim().ToLowerInvariant()
                if not (String.IsNullOrWhiteSpace tok) then tokens.Add(tok)
                sb.Clear() |> ignore

        for c in text do
            // treat letters and digits as part of tokens; allow apostrophes for contractions
            if Char.IsLetterOrDigit c || c = '\'' || c = '’' then
                sb.Append(c) |> ignore
            else
                // non token char => flush current token
                flush()

        // flush last token
        flush()
        tokens |> Seq.toList

/// Helper to count items in a list (wrapper for List.length)
let countItems list = List.length list
