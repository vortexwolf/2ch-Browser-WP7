using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DvachBrowser.Assets
{
    public class DvachUrlBuilder
    {
        public static readonly string DomainName = "http://" + Constants.DvachHost + "/";

        public Uri FixRelativeUri(Uri uri)
        {
            if (uri == null || uri.IsAbsoluteUri)
            {
                return uri;
            }

            var absoluteUri = new Uri(new Uri(DomainName), uri);

            return absoluteUri;
        }

        public string BuildThreadsUrl(string boardName, int page)
        {
            string jsonPage = page == 0 ? "wakaba" : page.ToString();
            string threadsUrl = string.Format(DomainName + "{0}/{1}.json?nocache={2}", boardName, jsonPage, DateTime.UtcNow.Ticks);

            return threadsUrl;
        }

        public string BuildPostsUrl(string boardName, string threadNumber)
        {
            string postsUrl = string.Format(DomainName + "{0}/res/{1}.json?nocache={2}", boardName, threadNumber, DateTime.UtcNow.Ticks);

            return postsUrl;
        }

        public string BuildAddPostUrl(string boardName)
        {
            string url = string.Format(DomainName + "{0}/wakaba.pl", boardName);

            return url;
        }

        public string BuildCaptchaCheckUrl()
        {
            return string.Format(DomainName + "makaba/captcha.fcgi?nocache={0}", DateTime.UtcNow.Ticks);
        }

        public string BuildResourceUrl(string boardName, string url)
        {
            return DomainName + boardName + "/" + url;
        }
    }
}
