open System
open InputHandling

[<EntryPoint>]
let main argv =
    printfn "Choose input method: 1) File 2) Manual Input"
    let choice = Console.ReadLine()
    
    let result =
        match choice with
        | "1" ->
            printf "Enter file path: "
            let path = Console.ReadLine()
            loadFromFile path
        | "2" ->
            readFromConsole()
        | _ ->
            Error "Invalid choice."

    match result with
    | Ok text ->
        printfn "Text loaded successfully!"
        printfn "%s" text
    | Error msg ->
        printfn "Failed to load text: %s" msg

    0
