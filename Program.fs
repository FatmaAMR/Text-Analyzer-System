open System
open System.Windows.Forms
open TextAnalyzerSystem.UI

[<EntryPoint>]
let main argv =
    Application.EnableVisualStyles()
    Application.Run(new MainForm())

    0
