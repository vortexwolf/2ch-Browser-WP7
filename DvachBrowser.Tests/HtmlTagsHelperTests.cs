using DvachBrowser.Assets.Controls;

using NUnit.Framework;

namespace DvachBrowser.Tests
{
    [TestFixture]
    public class HtmlTagsHelperTests
    {
        private readonly HtmlTagsHelper _instance = new HtmlTagsHelper();

        [TestCase("<p>test</p>", Result = "<p>test</p>")]
        [TestCase("<p>", Result = "<p></p>")]
        [TestCase("<br/>", Result = "<br/>")]
        [TestCase("<p>test", Result = "<p>test</p>")]
        [TestCase("<a><a>t1<a>t2<a>t3</a></a>", Result = "<a><a>t1<a>t2<a>t3</a></a></a></a>")]
        [TestCase("<a href='http://example.org'>example", Result = "<a href='http://example.org'>example</a>")]
        [TestCase("<span title='<test>'>", Result = "<span title='<test>'></span>")]
        public string AddClosedTagCases(string html)
        {
            string fix = this._instance.FixOpenCloseHtmlTags(html);

            return fix;
        }

        [Test]
        public void OpenTagWithoutBracket_Remove()
        {
            var html = "before tag<a ";
            string fix = this._instance.FixCutHtml(html);
            Assert.AreEqual("before tag", fix);
        }

        [Test]
        public void AmpersandWithoutSemicolon_Remove()
        {
            var html = "before&a";
            string fix = this._instance.FixCutHtml(html);
            Assert.AreEqual(fix, "before");
        }

        [Test]
        public void AmpersandWithSemicolon_NotChanged()
        {
            var html = "before&lt;after";
            string fix = this._instance.FixCutHtml(html);
            Assert.AreEqual(fix, "before&lt;after");
        }
    }
}
