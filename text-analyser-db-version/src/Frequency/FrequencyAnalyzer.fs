module FrequencyAnalyzer


    /// Calculates frequency of each word and returns Top N
    let getTopWords (limit: int) (words: string list) : (string * int) list =
        words
        |> List.countBy id              // Group by word and count: (word, count)
        |> List.sortByDescending snd    // Sort by count (second element) descending
        |> List.truncate limit          // Take only the top N items