module ReportRepository

open Microsoft.Data.Sqlite
open System.Text.Json
open Database

// Redefining the type here to decouple from FileIO.
// In a full refactor, this should be in a shared Domain project/folder.
type AnalysisReport = {
    Timestamp: System.DateTime
    TotalParagraphs: int
    TotalSentences: int
    TotalWords: int
    AverageSentenceLength: float
    ReadingEaseScore: float
    TopWords: (string * int) list
}

let saveReport (report: AnalysisReport) =
    use connection = new SqliteConnection(Database.connectionString)
    connection.Open()

    let command = connection.CreateCommand()
    command.CommandText <-
        """
        INSERT INTO AnalysisHistory (Timestamp, TotalParagraphs, TotalSentences, TotalWords, AverageSentenceLength, ReadingEaseScore, TopWords)
        VALUES ($timestamp, $totalParagraphs, $totalSentences, $totalWords, $avgSentenceLen, $readingEase, $topWords);
        """
    
    command.Parameters.AddWithValue("$timestamp", report.Timestamp.ToString("O")) |> ignore
    command.Parameters.AddWithValue("$totalParagraphs", report.TotalParagraphs) |> ignore
    command.Parameters.AddWithValue("$totalSentences", report.TotalSentences) |> ignore
    command.Parameters.AddWithValue("$totalWords", report.TotalWords) |> ignore
    command.Parameters.AddWithValue("$avgSentenceLen", report.AverageSentenceLength) |> ignore
    command.Parameters.AddWithValue("$readingEase", report.ReadingEaseScore) |> ignore
    
    // Serialize TopWords to JSON string
    let topWordsJson = JsonSerializer.Serialize(report.TopWords)
    command.Parameters.AddWithValue("$topWords", topWordsJson) |> ignore

    command.ExecuteNonQuery() |> ignore

let getAllReports () =
    use connection = new SqliteConnection(Database.connectionString)
    connection.Open()

    let command = connection.CreateCommand()
    command.CommandText <- "SELECT * FROM AnalysisHistory ORDER BY Timestamp DESC"

    using (command.ExecuteReader()) (fun reader ->
        let reports = ResizeArray<AnalysisReport>()
        while reader.Read() do
            let timestampStr = reader.GetString(1)
            let topWordsJson = reader.GetString(7)
            let topWords = JsonSerializer.Deserialize<(string * int) list>(topWordsJson)

            let report = {
                Timestamp = System.DateTime.Parse(timestampStr)
                TotalParagraphs = reader.GetInt32(2)
                TotalSentences = reader.GetInt32(3)
                TotalWords = reader.GetInt32(4)
                AverageSentenceLength = reader.GetDouble(5)
                ReadingEaseScore = reader.GetDouble(6)
                TopWords = topWords
            }
            reports.Add(report)
        reports |> Seq.toList
    )
