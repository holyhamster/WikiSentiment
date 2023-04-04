using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WikiLibrary.API;

namespace WikiLibrary.DataObjects
{
    /// <summary>
    /// POJO class for a single wikipedia title
    /// </summary>
    public class Article
    {
        public string ttl { get; set; }

        public int vws { get; set; }

        public Dictionary<string, string> lngl { get; set; }    //linked articles in other languages

        /// <summary>
        /// Builds an article 
        /// </summary>
        /// <param name="client">http client, wiki API headers required</param>
        /// <param name="title">article title</param>
        /// <param name="countryCode">two letter language code</param>
        /// <param name="views">article views</param>
        /// <returns></returns>
        public static async Task<Article> Create(HttpClient client, string title, string countryCode, int views)
        {
            Article result = new Article()
            {
                ttl = title,
                vws = views,
                lngl = new Dictionary<string, string>()
            };

            //get english title of given article
            string langlinkTitle = await Langlink.Get(client, title, countryCode);

            //if failed to get english article, check if its a redirect and try again
            if (langlinkTitle != "")
            {
                result.lngl["en"] = langlinkTitle;
                return result;
            }

            //if article is a redirect, fill in correct title and try to get a new langlink
            var redirectTitle = await Redirect.Get(client, title, countryCode);
            if (redirectTitle != "")
            {
                result.ttl = redirectTitle;
                var redirectLanglink = await Langlink.Get(client, redirectTitle, countryCode);

                if (redirectLanglink != "")
                    result.lngl["en"] = redirectLanglink;
            }
            return result;
        }
    }
}
