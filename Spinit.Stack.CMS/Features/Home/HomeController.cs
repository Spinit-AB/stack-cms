using System.Linq;
using System.Web.Mvc;
using Umbraco.Web;
using Umbraco.Web.Models;
using Umbraco.Web.Mvc;

namespace Spinit.Stack.CMS.Features.Home
{
    public class HomeController : RenderMvcController
    {
        public override ActionResult Index(RenderModel model)
        {
            var homeModel = new HomeModel(model.Content)
            {
                Titel = "Page titel"
            };
        
            return base.CurrentTemplate(homeModel);
        }
    }
}