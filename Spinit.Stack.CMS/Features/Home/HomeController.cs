using System;
using System.Linq;
using System.Web.Mvc;
using Umbraco.Core.Logging;
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
            
            LogHelper.Info(GetType(), "Test log");

            return base.CurrentTemplate(homeModel);
        }
    }
}