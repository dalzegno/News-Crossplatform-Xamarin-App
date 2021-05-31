//#define UseNewsApiSample  // Remove or undefine to use your own code to read live data

using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

using News.Models;
using News.ModelsSampleData;

namespace News.Services
{
    public class NewsService
    {
        readonly string apiKey = "4dc6a1da34d9496483224817652900bb";

        public readonly ConcurrentDictionary<(NewsCategory, string), NewsApiData> _CachedNews = new ConcurrentDictionary<(NewsCategory, string), NewsApiData>();

        public async Task<NewsGroup> GetNewsAsync(NewsCategory category)
        {
            string date = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
            NewsApiData nAPI;
            NewsGroup ng = new NewsGroup();
            if (_CachedNews.ContainsKey((category, date)))
            {
                _CachedNews.TryGetValue((category, date), out nAPI);
                
            }
#if UseNewsApiSample
            else
            {
                nAPI = await NewsApiSampleData.GetNewsApiSampleAsync(category);
                _CachedNews.GetOrAdd((category, date), nAPI);
            }
#else
                    //https://newsapi.org/docs/endpoints/top-headlines
                    var uri = $"https://newsapi.org/v2/top-headlines?country=se&category={category}&apiKey={apiKey}";


                    //Recommend to use Newtonsoft Json Deserializer as it works best with Android
                    var webclient = new WebClient();
                    var json = await webclient.DownloadStringTaskAsync(uri);
                    nAPI = Newtonsoft.Json.JsonConvert.DeserializeObject<NewsApiData>(json);

#endif
            ng.Category = category;
            ng.Articles = nAPI.Articles.Select(x => new NewsItem()
            {
                DateTime = x.PublishedAt,
                Title = x.Title,
                Description = x.Description,
                Url = x.Url,
                UrlToImage = x.UrlToImage,
            }).ToList();
                return ng;
        }
    }
}
