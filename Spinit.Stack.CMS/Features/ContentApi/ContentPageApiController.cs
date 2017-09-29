using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Spinit.Stack.CMS.Features.Content;
using Umbraco.Web.Models;
using Umbraco.Web.WebApi;

namespace Spinit.Stack.CMS.Features.ContentApi
{
    public class ContentPageApiController : UmbracoApiController
    {
        [HttpGet]
        public IEnumerable<string> Pages()
        {
            //var root = Umbraco.TypedContentAtRoot().First();
        
            return new []{ "test"};
        }
    }
}