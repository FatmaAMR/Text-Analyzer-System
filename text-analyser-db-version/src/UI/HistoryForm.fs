namespace TextAnalyzerSystem.UI

open System
open System.Drawing
open System.Windows.Forms
open ReportRepository

type HistoryForm() as this =
    inherit Form()

    let grid = new DataGridView()

    let loadData () =
        let reports = ReportRepository.getAllReports()
        // Convert to a specific list for binding, or just bind directly if possible.
        // DataGridView lists need to be wrapped or property-based.
        // AnalysisReport is a record, so it exposes properties.
        let bindingSource = new BindingSource()
        bindingSource.DataSource <- reports
        grid.DataSource <- bindingSource

    do
        this.Text <- "Analysis History"
        this.Size <- new Size(800, 600)
        this.StartPosition <- FormStartPosition.CenterScreen

        grid.Dock <- DockStyle.Fill
        grid.AutoSizeColumnsMode <- DataGridViewAutoSizeColumnsMode.Fill
        grid.ReadOnly <- true
        grid.AllowUserToAddRows <- false
        
        this.Controls.Add(grid)

        this.Load.Add(fun _ -> loadData())
