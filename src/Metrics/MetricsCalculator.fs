module MetricsCalculator


    /// Calculates Average Sentence Length (Words / Sentences)
    let calculateAvgSentenceLength (wordCount: int) (sentenceCount: int) : float =
        if sentenceCount = 0 then 0.0
        else float wordCount / float sentenceCount

    /// Counts syllables in a word (Heuristic approach for readability)
    let private countSyllables (word: string) =
        let vowels = Set.ofList ['a'; 'e'; 'i'; 'o'; 'u'; 'y']
        let isVowel c = Set.contains c vowels
        
        let rec countChars chars wasVowel count =
            match chars with
            | [] -> max 1 count // Every word has at least 1 syllable
            | c :: rest ->
                let currentIsVowel = isVowel c
                if currentIsVowel && not wasVowel then
                    countChars rest true (count + 1)
                else
                    countChars rest currentIsVowel count
        
        // Remove trailing 'e' as it's often silent, unless word is very short
        let cleanWord = 
            if word.Length > 2 && word.EndsWith("e") then word.Substring(0, word.Length - 1)
            else word

        countChars (Seq.toList cleanWord) false 0

    /// Calculates Flesch Reading Ease Score
    /// Formula: 206.835 - 1.015(total words / total sentences) - 84.6(total syllables / total words)
    let calculateReadingEase (words: string list) (sentenceCount: int) : float =
        let totalWords = float (List.length words)
        let totalSentences = float sentenceCount
        
        if totalWords = 0.0 || totalSentences = 0.0 then 
            0.0
        else
            let totalSyllables = words |> List.sumBy countSyllables |> float
            206.835 - (1.015 * (totalWords / totalSentences)) - (84.6 * (totalSyllables / totalWords))