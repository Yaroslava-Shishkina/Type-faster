using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Type_faster.Model
{
    public static class WordParser
    {
        public static List<string> Parse(string rawText)
        {
            if (string.IsNullOrWhiteSpace(rawText))
                return new List<string>();

            string normalized = rawText.Replace('\n', ' ')
                                       .Replace('\r', ' ')
                                       .Replace('\t', ' ');

            string[] parts = normalized.Split(new[] { ' ' }, System.StringSplitOptions.RemoveEmptyEntries);
            var result = new List<string>();

            foreach (var part in parts)
            {
                string clean = Regex.Replace(part, @"[^\p{L}\p{N}-]", "");
                if (!string.IsNullOrEmpty(clean))
                    result.Add(clean);
            }
            return result;
        }
    }
}