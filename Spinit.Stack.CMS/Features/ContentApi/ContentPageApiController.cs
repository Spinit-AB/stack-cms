using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Newtonsoft.Json;
using Spinit.Stack.CMS.Features.Content;
using Umbraco.Core.Models;
using Umbraco.Web;
using Umbraco.Web.Models;
using Umbraco.Web.WebApi;

namespace Spinit.Stack.CMS.Features.ContentApi
{
    public class ContentPageApiController : UmbracoApiController
    {
        [System.Web.Http.HttpGet]
        public object Page(int id)
        {
            //var root = Umbraco.TypedContentAtRoot().First();

            var page = Umbraco.TypedContent(id);

            var customProperties = page.Properties.ToDictionary(prop => prop.PropertyTypeAlias, prop => prop.Value);

            var umbracoProperties = new Dictionary<string, object>
            {
                {"Id", page.Id},
                {"Url", page.Url},
                {"Name", page.Name},
                {"WriterName", page.WriterName},
                {"CreateDate", page.CreateDate},
                {"DocumentTypeAlias", page.DocumentTypeAlias},
                {"CustomProperties", customProperties}
            };

            return umbracoProperties;
        }
    }
}