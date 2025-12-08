open System.Windows.Forms
open TextAnalyzerSystem.UI

[<EntryPoint>]
let main argv =
    Database.initializeDatabase()
    Application.EnableVisualStyles()
    Application.Run(new MainForm())
    0
