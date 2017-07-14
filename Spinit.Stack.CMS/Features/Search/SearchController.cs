using System.Linq;
using System.Net;
using System.Net.Http;
using Umbraco.Web.WebApi;

namespace Spinit.Stack.CMS.Features.Search
{
    public class SearchController : UmbracoApiController
    {
        [System.Web.Http.HttpGet]
        public HttpResponseMessage Content(string query)
        {
            var umbracoPagesResults = Umbraco.TypedSearch(query);

            var results = umbracoPagesResults.Select(umbracoPage =>
            new {
                name = umbracoPage.GetProperty("PageTitle")?.Value ?? umbracoPage.Name,
                url = umbracoPage.Url

            });

            return Request.CreateResponse(HttpStatusCode.OK, results);
        }
    }
}