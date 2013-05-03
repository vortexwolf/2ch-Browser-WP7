using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace DvachBrowser.Assets
{
    public class YoutubeHelper
    {
        public static readonly Regex YoutubeRegex = new Regex("(?:https?://)?(?:www\\.)?(?:m\\.)?youtube\\.com/(?:(?:v/)|(?:(?:#/)?watch\\?v=))([\\w\\-]{11})");

        public string GetYouTubeCode(string html)
        {
            if (string.IsNullOrEmpty(html))
            {
                return null;
            }

            Match match = YoutubeRegex.Match(html);
            if (match.Success)
            {
                string videoCode = match.Groups[1].Value;
                return videoCode;
            }

            return null;
        }

        public string GetVideoUrl(string code)
        {
            return "http://www.youtube.com/watch?v=" + code;
        }

        public string GetThumbnailUrl(string code)
        {
            return "http://img.youtube.com/vi/" + code + "/default.jpg";
        }
    }
}
