using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace DvachBrowser.Assets
{
    public class DvachUriParser
    {
        private readonly Regex _groupsRegex = new Regex("^/(\\w+)/?(?:(?:(\\d+).html)|(?:res/(\\d+)\\.html))?$");

        public bool IsDvachUri(Uri uri)
        {
            if (!uri.IsAbsoluteUri)
            {
                return true;
            }

            return uri.Host.StartsWith(Constants.DvachHost);
        }

        public DvachUriModel ParseUri(Uri uri)
        {
            string path = uri.IsAbsoluteUri ? uri.LocalPath : new Uri(new Uri("http://example.com"), uri).LocalPath;

            Match match = this._groupsRegex.Match(path);
            if (match.Success && match.Groups.Count == 4)
            {
                return new DvachUriModel()
                           {
                               BoardName = match.Groups[1].Value,
                               PageNumber = match.Groups[2].Value,
                               ThreadNumber = match.Groups[3].Value
                           };
            }

            return null;
        }

        public string GetThreadNumber(Uri uri)
        {
            var model = this.ParseUri(uri);

            return model != null ? model.ThreadNumber : null;
        }

        public class DvachUriModel
        {
            public string BoardName { get; set; }

            public string PageNumber { get; set; }

            public string ThreadNumber { get; set; }
        }
    }
}
