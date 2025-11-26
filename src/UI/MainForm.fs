namespace TextAnalyzerSystem.UI


open System.Drawing
open System.Windows.Forms
open Events

type MainForm() as this =
    inherit Form()

    let createButton (text: string) =
        let btn =
            new Button(
                Text = text,
                Height = 42,
                Dock = DockStyle.Top
            )
        btn.Font <- new Font("Segoe UI Semibold", 10.0f)
        btn.BackColor <- Color.FromArgb(25,25,112)
        btn.ForeColor <- Color.White
        btn.FlatStyle <- FlatStyle.Flat
        btn.FlatAppearance.BorderSize <- 0
        btn.Margin <- Padding(0, 0, 0, 18)
        btn.UseCompatibleTextRendering <- true
        btn

    let placeholder = "Enter your text manually or paste your text file path here..."
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
            BackColor = Color.FromArgb(220,220,220)
        )

    let resultsBox =
        new TextBox(
            Multiline = true,
            Dock = DockStyle.Fill,
            // ReadOnly = true,
            BackColor = Color.White,
            ForeColor = Color.FromArgb(25,25,112),
            Font = new Font("Segoe UI", 13.0f, FontStyle.Italic),
            ScrollBars = ScrollBars.Vertical
        )

    // Buttons
    // let loadTextButton = createButton "Input Text (Manually)"
    let loadTxtFileButton = createButton "Load Text File"
    let analyzeButton = createButton "Analyze Text"
    let exportButton = createButton "Export JSON Report"

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


        // tooltip.SetToolTip(loadTextButton, "Input text manually using the text editor")
        tooltip.SetToolTip(loadTxtFileButton, "Load a .txt file into the text editor")
        tooltip.SetToolTip(analyzeButton, "Analyze the entered or loaded text")
        tooltip.SetToolTip(exportButton, "Export analysis results to JSON")

   
        sidePanel.Controls.Add(exportButton)
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


        loadTxtFileButton.Click.Add(fun _ -> loadSelectedFile inputBox inputBox.Text )
        analyzeButton.Click.Add(fun _ -> analyzeText inputBox  resultsBox )
        exportButton.Click.Add(fun _ -> exportJSONReport inputBox  resultsBox)
