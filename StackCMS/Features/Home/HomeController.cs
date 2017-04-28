using System.Web.Mvc;
using Umbraco.Web.Models;
using Umbraco.Web.Mvc;

namespace StackCMS.Features.Home
{
    public class HomeController : RenderMvcController
    {
        public override ActionResult Index(RenderModel model)
        {
            //Do some stuff here, then return the base method
            var homeModel = new HomeModel(model.Content)
            {
                Titel = "krumler"
            };
            
            return base.CurrentTemplate(homeModel);
        }
    }
}