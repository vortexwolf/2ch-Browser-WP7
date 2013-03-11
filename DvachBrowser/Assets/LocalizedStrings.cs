using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DvachBrowser.Assets.Resources;

namespace DvachBrowser.Assets
{
    public class LocalizedStrings
    {
        public LocalizedStrings()
        {
            Strings = new DvachBrowser.Assets.Resources.Strings();
        }

        public static Strings Strings { get; private set; }
    }
}
