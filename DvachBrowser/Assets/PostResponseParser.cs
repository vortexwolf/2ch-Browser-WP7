using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace DvachBrowser.Assets
{
    public class PostResponseParser
    {
        private static readonly Regex ErrorRegex = new Regex("<center>.*?<font size=\"5\">(.+?)</.*?font>.*?<a.*?>Назад</a>.*?</center>");

        public ParseResult ParsePostResponse(string response)
        {
            if (string.IsNullOrEmpty(response))
            {
                return new ParseResult() { IsSuccess = true };
            }

            var match = ErrorRegex.Match(response);

            if (match.Success && match.Groups.Count > 0)
            {
                return new ParseResult() { IsSuccess = false, ErrorMessage = match.Groups[1].Value };
            }

            return new ParseResult() { IsSuccess = true };
        }

        public class ParseResult
        {
            public bool IsSuccess { get; set; }

            public string ErrorMessage { get; set; }
        }
    }
}
