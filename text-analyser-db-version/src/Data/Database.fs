module Database

open Microsoft.Data.Sqlite
open System.IO

let connectionString = "Data Source=text_analyzer.db"

let initializeDatabase () =
    // Initialize SQLitePCL to ensure native dependencies are loaded
    SQLitePCL.Batteries.Init()

    use connection = new SqliteConnection(connectionString)
    connection.Open()

    let command = connection.CreateCommand()
    command.CommandText <-
        """
        CREATE TABLE IF NOT EXISTS AnalysisHistory (
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            Timestamp TEXT NOT NULL,
            TotalParagraphs INTEGER,
            TotalSentences INTEGER,
            TotalWords INTEGER,
            AverageSentenceLength REAL,
            ReadingEaseScore REAL,
            TopWords TEXT
        );
        """
    command.ExecuteNonQuery() |> ignore
