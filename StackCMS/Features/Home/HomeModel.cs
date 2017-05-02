using Umbraco.Core.Models;

namespace StackCMS.Features.Home
{
    public class HomeModel : Umbraco.Web.PublishedContentModels.Home
    {
        public string Titel { get; set; }

        public HomeModel(IPublishedContent content) : base(content)
        {
        }
    }
}