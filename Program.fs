open System
open Tokenizer
open FrequencyAnalyzer
open MetricsCalculator
open ReportExporter
open InputHandler

[<EntryPoint>]
let main argv =
    
    printfn "=== Text Analyzer ==="
    printfn "Choose input source:"
    printfn "1. Load from file"
    printfn "2. Enter text manually"
    printf "Enter option (1 or 2): "

    let choice = Console.ReadLine()

    /// STEP 1: LOAD INPUT
    let textResult =
        match choice with
        | "1" ->
            printf "Enter file path: "
            let filePath = Console.ReadLine()
            loadFromFile filePath
        | "2" ->
            readFromConsole ()
        | _ ->
            Error "Invalid choice."

    match textResult with
    | Error msg ->
        printfn "❌ Error: %s" msg
        1

    | Ok text ->

        printfn "\nProcessing...\n"

        /// STEP 2: TOKENIZE
        let paragraphs = tokenizeParagraphs text
        let sentences = tokenizeSentences text
        let words = tokenizeWords text

        /// STEP 3: METRICS
        let totalParagraphs = paragraphs.Length
        let totalSentences = sentences.Length
        let totalWords = words.Length
        let avgSentenceLength = calculateAvgSentenceLength totalWords totalSentences
        let readingEase = calculateReadingEase words totalSentences
        let topWords = getTopWords 10 words

        /// STEP 4: BUILD REPORT
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

        /// STEP 5: SAVE REPORT TO JSON
        printf "\nEnter output JSON file name (e.g. report.json): "
        let outputPath = Console.ReadLine()
        match saveReportToJSON outputPath report with
        | Ok msg -> printfn "✅ %s" msg
        | Error err -> printfn "❌ %s" err

        /// STEP 6: PRINT SUMMARY
        printfn "\n=== Analysis Summary ==="
        printfn $"Paragraphs: {totalParagraphs}"
        printfn $"Sentences: {totalSentences}"
        printfn $"Words: {totalWords}"
        printfn $"Average Sentence Length: {avgSentenceLength:F2}"
        printfn $"Reading Ease Score: {readingEase:F2}"
        printfn "\nTop Words:"
        topWords |> List.iter (fun (w, c) -> printfn $"  {w}: {c}")

        0
