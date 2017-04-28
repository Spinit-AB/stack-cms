using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Web.Models;

namespace StackCMS.Features.Home
{
    public class HomeModel : PublishedContentModel
    {
        public string Titel { get; set; }

        public HomeModel(IPublishedContent content) : base(content)
        {
        }
    }
}