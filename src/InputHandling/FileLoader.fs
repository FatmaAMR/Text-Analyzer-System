namespace TextAnalyzerSystem.InputHandling

open System
open System.IO
open System.Text


module FileLoader =
    let detectEncoding (filePath: string) =
        use reader = new StreamReader(filePath, true)
        reader.Peek() |> ignore
        reader.CurrentEncoding
    let loadFileAsync (filePath: string) =
        async {
            if not (File.Exists filePath) then
                return ()
            try
                let encoding = detectEncoding filePath
                let! content = File.ReadAllTextAsync(filePath, encoding) |> Async.AwaitTask

                if String.IsNullOrWhiteSpace content then
                    return Error "File is empty."
                else
                    return Ok content
            with ex ->
                return Error ("Error reading file: " + ex.Message)
        }
