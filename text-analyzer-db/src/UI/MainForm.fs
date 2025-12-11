namespace TextAnalyzerSystem.UI


open System.Drawing
open System.Windows.Forms
open System.Text.Json
open System.IO
open Events
open ReportRepository

type MainForm() as this =
    inherit Form()

    let createButton (text: string) (normalColor: Color) (hoverColor: Color) =
        let btn =
            new Button(
                Text = text,
                Height = 45,
                Dock = DockStyle.Top
            )
        btn.Font <- new Font("Segoe UI", 11.0f, FontStyle.Bold)
        btn.BackColor <- normalColor
        btn.ForeColor <- Color.White
        btn.FlatStyle <- FlatStyle.Flat
        btn.FlatAppearance.BorderSize <- 0
        btn.Cursor <- Cursors.Hand
        btn.Margin <- Padding(0, 0, 0, 15)
        btn.UseCompatibleTextRendering <- true

        // Hover Effects
        btn.MouseEnter.Add(fun _ -> btn.BackColor <- hoverColor)
        btn.MouseLeave.Add(fun _ -> btn.BackColor <- normalColor)
        btn

    let placeholder = "Enter your text manually..."
    let inputBox =
        new TextBox(
            Multiline = true,
            Dock = DockStyle.Fill,
            Font = new Font("Segoe UI", 12.0f),
            BackColor = Color.White,
            ForeColor = Color.FromArgb(25,25,112),
            ScrollBars = ScrollBars.Vertical, 
            Text = placeholder 
        )
        
    do inputBox.GotFocus.Add(fun _ ->
        if inputBox.Text = placeholder then
            inputBox.Text <- ""
            inputBox.ForeColor <- Color.Black
    )
    do inputBox.LostFocus.Add(fun _ ->
        if inputBox.Text.Trim() = "" then
            inputBox.Text <- placeholder
            inputBox.ForeColor <- Color.Gray
    )
    let sidePanel =
        new Panel(
            Dock = DockStyle.Fill,
            Padding = new Padding(20),
            BackColor = Color.FromArgb(245, 245, 250) // Very light gray/blue
        )

    let resultsBox =
        new TextBox(
            Multiline = true,
            Dock = DockStyle.Fill,
            ReadOnly = true,
            BackColor = Color.White,
            ForeColor = Color.FromArgb(25,25,112),
            Font = new Font("Segoe UI", 13.0f, FontStyle.Italic),
            ScrollBars = ScrollBars.Vertical
        )


    let headerPanel =
        new Panel(
            Dock = DockStyle.Top,
            Height = 80,
            BackColor = Color.FromArgb(30, 30, 35)
        )
    
    let titleLabel =
        new Label(
            Text = "Text Analyzer Pro",
            Font = new Font("Segoe UI", 24.0f, FontStyle.Bold),
            ForeColor = Color.White,
            AutoSize = true,
            Location = Point(20, 20)
        )


    // Colors
    let blueNormal = Color.FromArgb(65, 105, 225)
    let blueHover = Color.FromArgb(100, 149, 237)
    
    let greenNormal = Color.FromArgb(46, 139, 87)
    let greenHover = Color.FromArgb(60, 179, 113)
    
    let purpleNormal = Color.FromArgb(148, 0, 211)
    let purpleHover = Color.FromArgb(186, 85, 211)

    let orangeNormal = Color.FromArgb(255, 140, 0)
    let orangeHover = Color.FromArgb(255, 165, 0)

    let loadTxtFileButton = createButton "Load Text File" blueNormal blueHover
    let analyzeButton = createButton "Analyze Text" greenNormal greenHover
    let historyButton = createButton "View History" purpleNormal purpleHover
    let exportJsonButton = createButton "Export to JSON" orangeNormal orangeHover

    let tooltip = new ToolTip()

    // Split container: left = text input, right = menu
    let splitMain =
        new SplitContainer(
            Dock = DockStyle.Fill,
            Orientation = Orientation.Vertical,
            SplitterDistance = 120
        )

    // Results area
    let splitBottom =
        new SplitContainer(
            Dock = DockStyle.Bottom,
            Orientation = Orientation.Horizontal,
            Height = 300,
            SplitterDistance = 10
        )
    let loadingLabel =
        new Label(
            Text = "",
            Dock = DockStyle.Top,
            Height = 25,
            Font = new Font("Segoe UI", 10.0f),
            ForeColor = Color.DarkGray
        )

    let spacer height =
        new Panel(Height = height, Dock = DockStyle.Top)


    // Form Initialization
    do
        this.Text <- "Text Analyzer System — F# WinForms"
        this.WindowState <- FormWindowState.Maximized
        this.FormBorderStyle <- FormBorderStyle.Sizable
        this.Font <- new Font("Segoe UI", 10.0f)
        this.StartPosition <- FormStartPosition.CenterScreen
        
        headerPanel.Controls.Add(titleLabel)


        tooltip.SetToolTip(loadTxtFileButton, "Load a .txt file into the text editor")
        tooltip.SetToolTip(analyzeButton, "Analyze the entered or loaded text")
        tooltip.SetToolTip(historyButton, "View past analysis reports")
        tooltip.SetToolTip(exportJsonButton, "Export analysis history to a JSON file")

   
        sidePanel.Controls.Add(exportJsonButton)
        sidePanel.Controls.Add(spacer 12)
        sidePanel.Controls.Add(historyButton)
        sidePanel.Controls.Add(spacer 12)
        sidePanel.Controls.Add(analyzeButton)
        sidePanel.Controls.Add(spacer 12)
        sidePanel.Controls.Add(loadTxtFileButton)
        sidePanel.Controls.Add loadingLabel

        splitMain.Panel1.Controls.Add(inputBox)
        splitMain.Panel2.Controls.Add(sidePanel)
        splitBottom.Panel2.Controls.Add(resultsBox)

        // Add everything to the form
        this.Controls.Add(splitMain)
        this.Controls.Add(splitBottom)
        this.Controls.Add(headerPanel) // Add header at the top


        loadTxtFileButton.Click.Add(fun _ -> 
            use openFileDialog = new OpenFileDialog()
            openFileDialog.Filter <- "Text Files (*.txt)|*.txt|All Files (*.*)|*.*"
            openFileDialog.Title <- "Select a Text File"
            if openFileDialog.ShowDialog() = DialogResult.OK then
                loadSelectedFile inputBox openFileDialog.FileName
        )
        analyzeButton.Click.Add(fun _ -> analyzeText inputBox  resultsBox )
        historyButton.Click.Add(fun _ -> 
            let historyForm = new HistoryForm()
            historyForm.Show()
        )

        exportJsonButton.Click.Add(fun _ ->
            try
                let reports = ReportRepository.getAllReports()
                let json = JsonSerializer.Serialize(reports, new JsonSerializerOptions(WriteIndented = true))
                
                use saveFileDialog = new SaveFileDialog()
                saveFileDialog.Filter <- "JSON Files (*.json)|*.json|All Files (*.*)|*.*"
                saveFileDialog.Title <- "Export Analysis History"
                saveFileDialog.FileName <- "analysis_history.json"
                
                if saveFileDialog.ShowDialog() = DialogResult.OK then
                   File.WriteAllText(saveFileDialog.FileName, json)
                   MessageBox.Show("Export successful!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information) |> ignore
            with
            | ex -> MessageBox.Show($"Error exporting data: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error) |> ignore
        )
