using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DvachBrowser.Assets
{
    public class QueryStringBuilder
    {
        private readonly Dictionary<string, string> _items;

        public QueryStringBuilder()
        {
            this._items = new Dictionary<string, string>();
        }

        public QueryStringBuilder Add(string key, string value)
        {
            value = value ?? string.Empty;

            if (this._items.ContainsKey(key))
            {
                this._items[key] = value;
            }
            else
            {
                this._items.Add(key, value);
            }

            return this;
        }

        public string Build()
        {
            var pairs = this._items.Select(kv => kv.Key + "=" + Uri.EscapeDataString(kv.Value)).ToArray();
            var joinedStr = string.Join("&", pairs);

            return "?" + joinedStr;
        }
    }
}
