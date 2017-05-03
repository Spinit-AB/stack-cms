using Umbraco.Core.Models;

namespace Spinit.Stack.CMS.Features.Home
{
    public class HomeModel : Umbraco.Web.PublishedContentModels.Home
    {
        public string Titel { get; set; }

        public HomeModel(IPublishedContent content) : base(content)
        {
        }
    }
}