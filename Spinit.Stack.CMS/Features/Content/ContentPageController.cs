using System.Web.Mvc;
using Umbraco.Web.Models;
using Umbraco.Web.Mvc;

namespace Spinit.Stack.CMS.Features.Content
{
    public class ContentPageController : RenderMvcController
    {
        public override ActionResult Index(RenderModel model)
        {
            var contentModel = new ContentPageModel(model.Content)
            {
                Test = "Test mvc"
            };
        
            return base.CurrentTemplate(contentModel);
        }
    }
}