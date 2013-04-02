using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DvachBrowser.Assets;

using NUnit.Framework;

namespace DvachBrowser.Tests
{
    [TestFixture]
    public class DvachUriParserTests
    {
        private readonly DvachUriParser _instance = new DvachUriParser();

        [TestCase("http://2ch.hk/test/", Result = true)]
        [TestCase("/test/", Result = true)]
        [TestCase("http://example.com", Result = false)]
        public bool IsDvachUriTestCases(string uriStr)
        {
            Uri uri = new Uri(uriStr, UriKind.RelativeOrAbsolute);

            bool result = this._instance.IsDvachUri(uri);

            return result;
        }

        [TestCase("http://2ch.hk/test/", Result = "test;;")]
        [TestCase("http://2ch.hk/test/1.html", Result = "test;1;")]
        [TestCase("http://2ch.hk/test/res/123.html", Result = "test;;123")]
        [TestCase("http://2ch.hk/test/src/12345.png", Result = null)]
        public string ParseUri(string uriStr)
        {
            Uri uri = new Uri(uriStr, UriKind.RelativeOrAbsolute);

            var result = this._instance.ParseUri(uri);

            return result == null ? null : string.Join(";", new[] { result.BoardName, result.PageNumber, result.ThreadNumber });
        }

        [TestCase("http://2ch.hk/test/res/1000.html", Result = "1000")]
        [TestCase("/b/res/1000.html", Result = "1000")]
        [TestCase("http://2ch.hk/pr/res/2000.html#222", Result = "2000")]
        public string GetThreadNumberTestCases(string uriStr)
        {
            Uri uri = new Uri(uriStr, UriKind.RelativeOrAbsolute);

            string result = this._instance.GetThreadNumber(uri);

            return result;
        }
    }
}
