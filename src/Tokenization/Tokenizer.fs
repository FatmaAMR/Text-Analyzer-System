module Tokenizer

open System
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

/// Splits text into words, removing punctuation
let tokenizeWords (text: string) : string list =
    // Split by whitespace and punctuation characters
    Regex.Split(text, @"[\s\p{Punct}]+")
    |> Array.toList
    |> List.map (fun w -> w.Trim().ToLowerInvariant()) // Normalize to lowercase
    |> List.filter (fun w -> not (String.IsNullOrWhiteSpace(w)))

/// Helper to count items in a list (wrapper for List.length)
let countItems list = List.length list
