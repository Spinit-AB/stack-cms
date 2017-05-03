using Umbraco.Core.Models;

namespace Spinit.Stack.CMS.Features.Content
{
    public class ContentPageModel : GeneratedModels.ContentPage
    {
        public string Test { get; set; }

        public ContentPageModel(IPublishedContent content) : base(content)
        {
        }
    }
}