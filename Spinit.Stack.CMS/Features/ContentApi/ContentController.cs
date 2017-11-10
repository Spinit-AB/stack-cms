using System.Collections.Generic;
using System.Linq;
using Our.Umbraco.Vorto.Extensions;
using Umbraco.Core;
using Umbraco.Web;
using Umbraco.Web.Extensions;
using Umbraco.Web.WebApi;

namespace Spinit.Stack.CMS.Features.ContentApi
{
    public class ContentController : UmbracoApiController
    {

        [System.Web.Http.HttpGet]
        public object MainMenu(string language = null)
        {
            var rootPage = Umbraco.TypedContentAtRoot().First();

            var menu = new List<object>();

            foreach (var menuParent in rootPage.Children)
            {
                var menuItems = menuParent.Children.Select(page => new
                {
                    Id = page.Id,
                    pageTitle = page.GetVortoValue("pageTitle", language)

                });

                menu.Add(new
                {
                    Id = menuParent.Id,
                    pageTitle = menuParent.GetVortoValue("pageTitle", language),
                    items = menuItems
                });
            }
            
            return menu;
        }

        [System.Web.Http.HttpGet]
        public object Page(int id, string language = null)
        {
            var page = Umbraco.TypedContent(id);

            var contentType = Services.ContentService.GetById(page.Id);

            var customProperties = contentType.Properties.ToDictionary(prop => prop.Alias,
                property => 
                    property.PropertyType?.PropertyEditorAlias == "Umbraco.MediaPicker2" ?
                    Udi.Parse(property.Value.ToString()).ToPublishedContent().Url :

                    property.PropertyType?.PropertyEditorAlias == "Umbraco.ContentPicker2" ?
                    Udi.Parse(property.Value.ToString()).ToPublishedContent().Id :

                    property.PropertyType?.PropertyEditorAlias == "Our.Umbraco.Vorto" ?
                    page.GetVortoValue(property.Alias, language) :  
                    
                    property.Value
                );

            var umbracoProperties = new Dictionary<string, object>
            {
                {"Id", page.Id},
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