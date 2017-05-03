using Umbraco.Core.Models;

namespace Spinit.Stack.CMS.Features.Home
{
    public class HomeModel : GeneratedModels.Home
    {
        public string Titel { get; set; }

        public HomeModel(IPublishedContent content) : base(content)
        {
        }
    }
}