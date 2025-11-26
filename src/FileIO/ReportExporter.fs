module ReportExporter

open System.IO
open System.Text.Json

    type AnalysisReport = {
        Timestamp: System.DateTime
        TotalParagraphs: int
        TotalSentences: int
        TotalWords: int
        AverageSentenceLength: float
        ReadingEaseScore: float
        TopWords: (string * int) list
    }

    let saveReportToJSON (filePath: string) (report: AnalysisReport) =
        try
            let options = JsonSerializerOptions(WriteIndented = true)
            let json = JsonSerializer.Serialize(report, options)
            File.WriteAllText(filePath, json)
            Ok ("Report saved successfully to " + filePath)
        with
        | ex -> Error ("Failed to save report: " + ex.Message)