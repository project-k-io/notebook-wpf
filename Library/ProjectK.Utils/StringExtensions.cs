using System;
using System.Collections.Generic;
using System.Text;

namespace ProjectK.ToolKit.Utils;

public static class StringExtensions
{
    public static List<string> ConvertTextInMultipleLines(this string text, int maxLength)
    {
        var separators = new[] { "\r\n", "\r", "\n" };
        var paragraphs = text.Split(separators, StringSplitOptions.None);
        var lines = new List<string>();
        foreach (var paragraph in paragraphs)
        {
            var paragraphLines = ConvertTextInMultipleLines(paragraph, maxLength, ' ');
            lines.AddRange(paragraphLines);
        }

        return lines;
    }

    public static List<string> ConvertTextInMultipleLines(this string text, int maxLength, char separator)
    {
        var lines = new List<string>();
        var words = text.Split(separator);
        var sb = new StringBuilder();
        for (var i = 0; i < words.Length; i++)
        {
            var word = words[i];
            if (sb.Length + word.Length > maxLength)
            {
                lines.Add(sb.ToString());
                sb.Length = 0;
            }

            sb.Append(word);
            // don't to last word
            if (i != words.Length - 1)
                sb.Append(' ');
        }

        lines.Add(sb.ToString());

        return lines;
    }

    public static string GetUniqueName(this string name, List<string> names)
    {
        var num = 1;
        string newName;
        while (true)
        {
            newName = $"{name} {num++}";
            if (names.Contains(newName))
                continue;

            break;
        }

        return newName;
    }
}