using System.Xml.Linq;

using DvachBrowser.Assets.Controls;
using NUnit.Framework;

namespace DvachBrowser.Tests.FullFramework
{
    [TestFixture]
    public class HtmlElementToXamlElementConverterTests
    {
        private readonly HtmlElementToXamlElementConverter _instance = new HtmlElementToXamlElementConverter();

        [TestCase("<br />", Result = "<LineBreak />")]
        [TestCase("<p>test</p>", Result = "<Span>test</Span>")]
        [TestCase("<strong>test</strong>", Result = "<Bold>test</Bold>")]
        [TestCase("<em>test</em>", Result = "<Italic>test</Italic>")]
        [TestCase("<a href=\"test_uri\">test</a>", Result = "<Hyperlink NavigateUri=\"test_uri\" TargetName=\"_blank\">test</Hyperlink>")]
        [TestCase("<span class=\"u\">test</span>", Result = "<Underline>test</Underline>")]
        [TestCase("<span class=\"unkfunc\">test</span>", Result = "<Span Foreground=\"#789922\">test</Span>")]
        [TestCase("<span class=\"spoiler\">test</span>", Result = "<Span Foreground=\"#48B0FD\">test</Span>")]
        [TestCase("<font style=\"color: rgb(71, 29, 2);\">test</font>", Result = "<Span Foreground=\"#471D02\">test</Span>")]
        public string ConvertHtmlEntityToXamlTestCases(string html)
        {
            XElement element = XElement.Parse(html);
            XNode xamlNode = this._instance.ConvertHtmlEntityToXaml(element);

            return xamlNode.ToString(SaveOptions.DisableFormatting);
        }
    }
}
