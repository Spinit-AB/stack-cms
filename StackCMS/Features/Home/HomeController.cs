﻿using System.Web.Mvc;
using Umbraco.Web.Models;
using Umbraco.Web.Mvc;

namespace StackCMS.Features.Home
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