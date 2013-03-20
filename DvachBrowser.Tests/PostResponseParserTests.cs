using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DvachBrowser.Assets;

using NUnit.Framework;

namespace DvachBrowser.Tests
{
    [TestFixture]
    public class PostResponseParserTests
    {
        private readonly PostResponseParser _instance = new PostResponseParser();

        [Test]
        public void ParseIncorrectCaptchaResponseTest()
        {
            string response = "<hr><center><strong><font size=\"5\">Ошибка: Неверный код подтверждения.</strong></font><br /><h2 style=\"text-align: center\"><a href=\"file:///Applications/Install/5985B11F-10FE-4403-A9F5-0DDBAC1CB38A/Install/\">Назад</a><br /></h2></center><hr>";

            var result = this._instance.ParsePostResponse(response);

            Assert.False(result.IsSuccess);
            Assert.AreEqual("Ошибка: Неверный код подтверждения.", result.ErrorMessage);
        }
    }
}
