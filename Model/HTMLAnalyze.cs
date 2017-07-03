using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LMS_Project.Model
{
    public static class HTMLAnalyze
    {
        public static List<string> ToCodeList(string content)
        {
            content = WebUtility.HtmlDecode(content);
            var result = new List<string>();
            int begin = 0;
            int end = 0;
            for (int i = 0; i < content.Length; i++)
            {
                if (content[i].Equals('\n'))
                    end = i;
                if (end > begin)
                {
                    result.Add(content.Substring(begin, end - begin));
                    begin = end + 1;
                }
            }
            return result;
        }
        public static LinkedList<string> ToTextList(List<string> Code_HTML_Content)
        {
            var Result = new LinkedList<string>();
            for(int i = 0; i < Code_HTML_Content.Count; i++)
            {
                string value = Code_HTML_Content[i];
                if(Regex.IsMatch(value, @"<[^/]+>"))
                {
                    if (value.Contains("table"))
                    {
                        Result.AddLast("TABLE BEGIN");
                    }
                    if (value.Contains("tbody"))
                    {
                        Result.AddLast("TABLE BODY BEGIN");
                    }
                    if (value.Contains("tr"))
                    {
                        Result.AddLast("TABLE ROW BEGIN");
                    }
                    if (value.Contains("td"))
                    {
                        Result.AddLast("TABLE CELL BEGIN");
                    }
                    if (value.Contains("div"))
                    {
                        Result.AddLast("CONTENT BEGIN");
                    }
                    if (value.Contains("br"))
                    {
                        Result.AddLast("");
                    }
                }
                else if(Regex.IsMatch(value, @"</[\s,\w,\W]+>"))
                {
                    if (value.Contains("table"))
                    {
                        Result.AddLast("TABLE END");
                    }
                    if (value.Contains("tbody"))
                    {
                        Result.AddLast("TABLE BODY END");
                    }
                    if (value.Contains("tr"))
                    {
                        Result.AddLast("TABLE ROW END");
                    }
                    if (value.Contains("td"))
                    {
                        Result.AddLast("TABLE CELL END");
                    }
                    if (value.Contains("div"))
                    {
                        Result.AddLast("CONTENT END");
                    }
                }
                else if(Regex.IsMatch(value, @"<[\s,\w,\W]+/>"))
                {
                    if (value.Contains("br"))
                    {
                        Result.AddLast("");
                    }
                }
                else
                {
                    Result.AddLast(value);
                }
            }
            return Result;
        }
    }
}
