using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DvachBrowser.Assets;

using NUnit.Framework;

namespace DvachBrowser.Tests
{
    [TestFixture]
    public class YoutubeHelperTests
    {
        private readonly YoutubeHelper _instance = new YoutubeHelper();

        [TestCase("<a href=\"http://www.youtube.com/v/A4aA4a4A4A4\"/>", Result = "A4aA4a4A4A4")]
        [TestCase("http://youtube.com/watch?v=_-123ZZZzzz", Result = "_-123ZZZzzz")]
        [TestCase("https://www.youtube.com/watch?v=1-123ZZZzzz", Result = "1-123ZZZzzz")]
        [TestCase("http://m.youtube.com/#/watch?v=123456789xx", Result = "123456789xx")]
        public string GetYouTubeCodeTestCases(string text)
        {
            return this._instance.GetYouTubeCode(text);
        }

        [Test]
        public void GetVideoUrlTest()
        {
            string code = "123456789xx";
            string url = this._instance.GetVideoUrl(code);

            Assert.AreEqual("http://www.youtube.com/watch?v=123456789xx", url);
        }

        [Test]
        public void GetThumbnailUrlTest()
        {
            string code = "123456789xx";
            string url = this._instance.GetThumbnailUrl(code);

            Assert.AreEqual("http://img.youtube.com/vi/123456789xx/default.jpg", url);
        }
    }
}
