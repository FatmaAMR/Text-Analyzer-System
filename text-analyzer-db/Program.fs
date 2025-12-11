open System
open System.Windows.Forms
open TextAnalyzerSystem.UI

[<STAThread>]
[<EntryPoint>]
let main argv =
    try
        Database.initializeDatabase()
        Application.EnableVisualStyles()
        Application.Run(new MainForm())
        0
    with
    | ex ->
        printfn "CRITICAL ERROR: %O" ex
        MessageBox.Show($"Application crashed: {ex.ToString()}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error) |> ignore
        1
