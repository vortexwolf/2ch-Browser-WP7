using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DvachBrowser.Assets;

using NUnit.Framework;

namespace DvachBrowser.Tests
{
    [TestFixture]
    public class QueryStringBuilderTests
    {
        [Test]
        public void OneItemTest()
        {
            var str = new QueryStringBuilder()
                .Add("key1", "1")
                .Build();
            Assert.AreEqual("?key1=1", str);
        }

        [Test]
        public void ZeroItemsTest()
        {
            var str = new QueryStringBuilder().Build();
            Assert.AreEqual("?", str);
        }

        [Test]
        public void TwoItemsTest()
        {
            var str = new QueryStringBuilder()
                .Add("key1", "1")
                .Add("key2", "2")
                .Build();
            Assert.AreEqual("?key1=1&key2=2", str);
        }

        [Test]
        public void DuplicateItemsTest()
        {
            var str = new QueryStringBuilder()
                .Add("key1", "1")
                .Add("key1", "2")
                .Build();
            Assert.AreEqual("?key1=2", str);
        }

        [Test]
        public void EscapeUrlTest()
        {
            var str = new QueryStringBuilder()
                .Add("key1", "?&/")
                .Build();
            Assert.AreEqual("?key1=%3F%26%2F", str);
        }

        [Test]
        public void NullValueTest()
        {
            var str = new QueryStringBuilder()
                .Add("key1", null)
                .Build();
            Assert.AreEqual("?key1=", str);
        }
    }
}
