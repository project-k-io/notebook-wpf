using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectK.Utils
{
    public class StringHelper
    {
        public static List<string> ConvertTextInMultipleLines(string text, int maxLength)
        {
            if (text.Length <= maxLength)
                return null;


            var lines = new List<string>();
            var words = text.Split(" ");

            var sb = new StringBuilder();
            for(var i = 0; i < words.Length; i++)
            {
                var word = words[i];
                if (sb.Length + word.Length > maxLength)
                {
                    lines.Add(sb.ToString());
                    sb.Length = 0;
                }

                sb.Append(word);
                // don't to last word
                if(i != words.Length- 1)
                    sb.Append(" ");
            }

            if (sb.Length > 0)
                lines.Add(sb.ToString());


            return lines;
        }
    }
}
