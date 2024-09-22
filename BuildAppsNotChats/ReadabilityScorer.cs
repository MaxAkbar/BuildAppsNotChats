using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BuildAppsNotChats;

public class ReadabilityScorer
{
    public virtual TextStatistics AnalyzeText(string text)
    {
        var words = GetWords(text);
        var sentences = GetSentences(text);
        var syllables = CountSyllables(text);

        var stats = new TextStatistics
        {
            WordCount = words.Length,
            SentenceCount = sentences.Length,
            AverageWordsPerSentence = (float)words.Length / sentences.Length,
            AverageSyllablesPerWord = (float)syllables / words.Length
        };

        stats.ReadabilityScores = CalculateReadabilityScores(stats);
        stats.OverallReadability = DetermineOverallReadability(stats.ReadabilityScores);
        stats.ImprovementSuggestions = GenerateImprovementSuggestions(stats);

        return stats;
    }

    private string[] GetWords(string text)
    {
        return text.Split(new[] { ' ', '\t', '\n' }, StringSplitOptions.RemoveEmptyEntries);
    }

    private string[] GetSentences(string text)
    {
        return Regex.Split(text, @"(?<=[.!?])\s+");
    }

    private int CountSyllables(string text)
    {
        int totalSyllables = 0;
        string[] words = GetWords(text);

        foreach (string word in words)
        {
            totalSyllables += CountSyllablesInWord(word);
        }

        return totalSyllables;
    }

    private int CountSyllablesInWord(string word)
    {
        word = word.ToLower().Trim();

        // Exception: Ignore words with 3 or fewer letters
        if (word.Length <= 3) return 1;

        // Exception: Words ending with "le" or "les"
        if (word.EndsWith("le") && !IsVowel(word[word.Length - 3]))
            return CountSyllablesInWord(word.Substring(0, word.Length - 2)) + 1;
        if (word.EndsWith("les") && !IsVowel(word[word.Length - 4]))
            return CountSyllablesInWord(word.Substring(0, word.Length - 3)) + 1;

        // Remove ending "e", "es", "ed" except when the ending is "le"
        word = Regex.Replace(word, @"(?<!l)e$", "");
        word = Regex.Replace(word, @"es$", "");
        word = Regex.Replace(word, @"ed$", "");

        // Count vowel groups
        int count = Regex.Matches(word, @"[aeiouy]+").Count;

        // Exception: Words ending with "ion"
        if (word.EndsWith("ion") && word.Length > 3)
            count++;

        // Exception: Trailing "e" (already removed unless after "l")
        if (word.EndsWith("e"))
            count--;

        // Exception: Handle some common silent vowel groups
        count -= CountSilentVowelGroups(word);

        // Ensure at least one syllable
        return Math.Max(1, count);
    }

    private bool IsVowel(char c)
    {
        return "aeiouy".Contains(c);
    }

    private int CountSilentVowelGroups(string word)
    {
        int count = 0;
        string[] silentPatterns = { "ai", "au", "ay", "ea", "ee", "ei", "eu", "ey", "ie", "oa", "oe", "oi", "oo", "ou", "oy", "ui" };

        foreach (string pattern in silentPatterns)
        {
            if (word.Contains(pattern))
                count++;
        }

        return count;
    }

    private ReadabilityScores CalculateReadabilityScores(TextStatistics stats)
    {
        var scores = new ReadabilityScores();

        // Flesch-Kincaid Grade Level
        scores.FleschKincaidGrade = 0.39 * stats.AverageWordsPerSentence + 11.8 * stats.AverageSyllablesPerWord - 15.59;

        // Flesch Reading Ease
        scores.FleschReadingEase = 206.835 - 1.015 * stats.AverageWordsPerSentence - 84.6 * stats.AverageSyllablesPerWord;

        return scores;
    }

    private string DetermineOverallReadability(ReadabilityScores scores)
    {
        if (scores.FleschReadingEase > 90) return "Very Easy";
        if (scores.FleschReadingEase > 80) return "Easy";
        if (scores.FleschReadingEase > 70) return "Fairly Easy";
        if (scores.FleschReadingEase > 60) return "Standard";
        if (scores.FleschReadingEase > 50) return "Fairly Difficult";
        if (scores.FleschReadingEase > 30) return "Difficult";
        return "Very Difficult";
    }

    private string[] GenerateImprovementSuggestions(TextStatistics stats)
    {
        var suggestions = new List<string>();

        if (stats.AverageWordsPerSentence > 20)
        {
            suggestions.Add("Consider breaking down long sentences into shorter ones.");
        }

        if (stats.AverageSyllablesPerWord > 2)
        {
            suggestions.Add("Try using simpler words with fewer syllables.");
        }

        return suggestions.ToArray();
    }
}

public class TextStatistics
{
    public int WordCount { get; set; }
    public int SentenceCount { get; set; }
    public float AverageWordsPerSentence { get; set; }
    public float AverageSyllablesPerWord { get; set; }
    public ReadabilityScores ReadabilityScores { get; set; }
    public string OverallReadability { get; set; }
    public string[] ImprovementSuggestions { get; set; }
    public int SyllableCount { get; internal set; }
    public string ContentAnalysis { get; internal set; }
    public string AudienceSpecificAnalysis { get; internal set; }
}

public class ReadabilityScores
{
    public double FleschKincaidGrade { get; set; }
    public double FleschReadingEase { get; set; }
}
