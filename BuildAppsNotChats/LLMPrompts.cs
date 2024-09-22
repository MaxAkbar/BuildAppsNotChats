namespace BuildAppsNotChats;

public class LLMPrompts
{
    public static string GetSyllableCountPrompt(string word) =>
                        @$"Count the number of syllables in the following word: '{word}'

                        Please respond with only the number of syllables, without any explanation or additional text.";

    public static string AnalyzeContentPrompt(string text) =>
                        @$"Analyze the following text for readability:

                        {text}

                        Please provide a brief analysis covering the following points:
                        1. Overall clarity and coherence
                        2. Use of jargon or technical terms
                        3. Consistency of tone and style
                        4. Presence of clear topic sentences and transitions
                        5. Appropriateness for a general audience

                        Limit your response to a maximum of 150 words.";

    public static string GenerateImprovementSuggestionsPrompt(string text, TextStatistics stats) =>
                        @$"Analyze the following text and provide suggestions for improving its readability:

                        {text}

                        Current readability statistics:
                        - Word count: {stats.WordCount}
                        - Sentence count: {stats.SentenceCount}
                        - Average words per sentence: {stats.AverageWordsPerSentence:F1}
                        - Flesch-Kincaid Grade Level: {stats.ReadabilityScores.FleschKincaidGrade:F1}
                        - Flesch Reading Ease: {stats.ReadabilityScores.FleschReadingEase:F1}

                        Please provide 3-5 specific suggestions for improving the text's readability. Focus on:
                        1. Simplifying complex words or phrases
                        2. Improving sentence structure
                        3. Enhancing overall clarity and flow
                        4. Adjusting for the appropriate reading level

                        Format each suggestion as a bullet point.";

    public static string GetAudienceSpecificAnalysisPrompt(string text, string audience) =>
                        @$"Analyze the following text for readability, specifically considering its appropriateness for a {audience} audience:

                        {text}

                        Please provide a brief analysis covering:
                        1. Whether the content is suitable for the specified audience
                        2. Any terms or concepts that might be challenging for this audience
                        3. Suggestions for adjusting the text to better suit this audience

                        Limit your response to a maximum of 150 words.";
}
