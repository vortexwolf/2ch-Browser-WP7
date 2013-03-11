using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DvachBrowser.Assets
{
    public class QueryStringBuilder
    {
        private readonly List<string> _keys;
        private readonly List<string> _values;

        public QueryStringBuilder()
        {
            this._keys = new List<string>();
            this._values = new List<string>();
        }

        public QueryStringBuilder Add(string key, string value)
        {
            this._keys.Add(key);
            this._values.Add(value);

            return this;
        }

        public string Build()
        {
            var pairs = Enumerable.Range(0, this._keys.Count).Select(i => this._keys[i] + "=" + this._values[i]).ToArray();
            var joinedStr = string.Join("&", pairs);

            return "?" + joinedStr;
        }
    }
}
