module TextAnalyzerSystem.UI.Events

open System
open System.Windows.Forms
open TextAnalyzerSystem.InputHandling.FileLoader
open TextAnalyzerSystem.Utils.AsyncUI
open Tokenizer
open FrequencyAnalyzer
open MetricsCalculator
open ReportExporter

let loadSelectedFile (inputBox:TextBox) (path: string) =

    runAsync
        (fun () ->
            loadFileAsync path |> Async.RunSynchronously
        )
        (fun result ->

            match result with
            | Ok text ->
                inputBox.Text <- text
            | Error msg ->
                MessageBox.Show(msg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                |> ignore
        )


let analyzeText (inputBox:TextBox) (resultsBox:TextBox) =
    let text = inputBox.Text.Trim()

    if text = "" then
        resultsBox.Text <- "⚠ No text to analyze."

    else
    
        let text = inputBox.Text
        let paragraphs = tokenizeParagraphs text
        let sentences = tokenizeSentences text
        let words = tokenizeWords text

        let totalParagraphs = paragraphs.Length
        let totalSentences = sentences.Length
        let totalWords = words.Length
        let avgSentenceLength = calculateAvgSentenceLength totalWords totalSentences
        let readingEase = calculateReadingEase words totalSentences
        let topWords: (string * int) list = getTopWords 10 words

        let topWordsText =
            topWords
            |> List.map (fun (w,c) -> $"{w}: {c} \n")
            |> String.concat "\n"

        resultsBox.Text <- $"""
    --- TEXT ANALYSIS REPORT ---

    Paragraph Count: {totalParagraphs}
    Sentence Count: {totalSentences}
    Word Count: {totalWords}

    Top 10 Words:
    {topWordsText + "\n"}

    Average Sentence Length: {avgSentenceLength}
    Reading Ease Score: {readingEase:F2}
        """



let exportJSONReport (inputBox:TextBox) (resultsBox:TextBox) =
    let text = inputBox.Text.Trim()

    if text = "" then
        MessageBox.Show("No text to export", "Error") |> ignore

    else
        // Recompute analysis
        let paragraphs = tokenizeParagraphs text
        let sentences = tokenizeSentences text
        let words = tokenizeWords text

        let totalParagraphs = paragraphs.Length
        let totalSentences = sentences.Length
        let totalWords = words.Length
        let avgSentenceLength = calculateAvgSentenceLength totalWords totalSentences
        let readingEase = calculateReadingEase words totalSentences
        let topWords = getTopWords 10 words

        let report =
            {
                Timestamp = DateTime.Now
                TotalParagraphs = totalParagraphs
                TotalSentences = totalSentences
                TotalWords = totalWords
                AverageSentenceLength = avgSentenceLength
                ReadingEaseScore = readingEase
                TopWords = topWords
            }

        use dialog = new SaveFileDialog()
        dialog.Filter <- "JSON Files (*.json)|*.json"
        dialog.FileName <- "report.json"

        match dialog.ShowDialog() with
        | DialogResult.OK ->
            let outputPath = dialog.FileName
            match saveReportToJSON outputPath report with
            | Ok msg ->
                resultsBox.Text <- $"Report saved successfully:\n{outputPath}"
            | Error err ->
                resultsBox.Text <- $"Error saving report:\n{err}"
        | _ -> ()
