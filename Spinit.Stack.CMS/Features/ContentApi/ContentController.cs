using System.Collections.Generic;
using System.Linq;
using Our.Umbraco.Vorto.Extensions;
using Spinit.Stack.CMS.Features.Language;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Web.Extensions;
using Umbraco.Web.WebApi;

namespace Spinit.Stack.CMS.Features.ContentApi
{
    public class ContentController : UmbracoApiController
    {
        // Umbraco/api/content/MainMenu
        [System.Web.Http.HttpGet]
        public object MainMenu(string language = null)
        {
            var rootPage = Umbraco.TypedContentAtRoot().First();
            
            var menuItems = GetMenuItems(rootPage, language);

            return new Result
            {
                success = true,
                message = ResultMessage.OK,
                data = menuItems
            };
        }

        // Umbraco/api/content/Translations
        [System.Web.Http.HttpGet]
        public object Translations(string language = Translate.DEFAULT_LANGUAGE)
        {
            var rootDictionaryItems = Services.LocalizationService.GetRootDictionaryItems();

            var translations = GetDictonaryItems(rootDictionaryItems, language).SelectMany(d => d).ToDictionary(e => e.Key, e => e.Value);

            return new Result
            {
                success = true,
                message = ResultMessage.OK,
                data = translations
            };
        }

        // Umbraco/api/content/Page/?id=X
        [System.Web.Http.HttpGet]
        public object Page(int? id = null, string umb = null, string language = null)
        {
            IPublishedContent page;

            if (!string.IsNullOrEmpty(umb))
            {
                page = GetPageByUmbLink(umb);
            }
            else if (id != null)
            {
                page = Umbraco.TypedContent(id);
            }
            else
            {
                page = null;
            }
            
            if(page == null)
            {
                return new Result
                {
                    success = false,
                    message = "No page found"
                };
            }

            var contentType = Services.ContentService.GetById(page.Id);

            var customProperties = contentType.Properties.ToDictionary(property => property.Alias,
                property => GetPropertyValue(property, page, language)

                );

            var umbracoProperties = new Dictionary<string, object>
            {
                {"id", page.Id},
                {"name", page.Name},
                {"writerName", page.WriterName},
                {"createDate", page.CreateDate},
                {"documentTypeAlias", page.DocumentTypeAlias},
                {"customProperties", customProperties}
            };

            return new Result
            {
                success = true,
                message = ResultMessage.OK,
                data = umbracoProperties
            };
        }
        private object GetMenuItems(IPublishedContent startNode, string language = null)
        {
            return startNode.Children.Select(page => new
            {
                id = page.Id,
                pageTitle = page.GetVortoValue("pageTitle", language),
                documentTypeAlias = page.DocumentTypeAlias,
                items = GetMenuItems(page, language)
            });
        }

        private IEnumerable<Dictionary<string, object>> GetDictonaryItems(IEnumerable<IDictionaryItem> dictionaryItems, string language)
        {
            var translations = new List<Dictionary<string, object>>();

            foreach (var dictionaryItem in dictionaryItems)
            {
                var translationInLanguage = dictionaryItem.Translations.Where(translation => translation.Language.IsoCode.StartsWith(language)).Select(x => x.Value).FirstOrDefault();

                if (string.IsNullOrEmpty(translationInLanguage))
                {
                    translationInLanguage = dictionaryItem.Translations.Where(translation => translation.Language.IsoCode.Equals(Translate.DEFAULT_LANGUAGE)).Select(x => x.Value).FirstOrDefault();
                }

                translations.Add(
                    new Dictionary<string, object>()
                    {{ dictionaryItem.ItemKey,
                        translationInLanguage}
                    });

                var childrens = Services.LocalizationService.GetDictionaryItemChildren(dictionaryItem.Key);
                if (childrens.Any())
                {
                    translations.AddRange(GetDictonaryItems(Services.LocalizationService.GetDictionaryItemChildren(dictionaryItem.Key), language));
                }
            }

            return translations;
        }

        private object GetPropertyValue(Property property, IPublishedContent page, string language)
        {
            object value;

            switch (property.PropertyType?.PropertyEditorAlias)
            {
                case "Umbraco.MediaPicker2":
                    var mediaUdi = property.Value?.ToString();

                    value = !string.IsNullOrEmpty(mediaUdi) ? Udi.Parse(property.Value?.ToString())?.ToPublishedContent()?.Url : null;
                    break;

                case "Umbraco.ContentPicker2":
                    var contentUdi = property.Value?.ToString();
                    var contentPage = GetPageByUmbLink(contentUdi);

                    value = contentPage?.Id;
                    break;

                case "Umbraco.MultiNodeTreePicker2":
                    var contentUdiList = property.Value?.ToString().Split(',');
                    var contentList = contentUdiList.Select(GetPageByUmbLink).Select(contentUdiPage => contentUdiPage?.Id);

                    value = contentList;
                    break;

                case "Our.Umbraco.Vorto":
                    value = page.GetVortoValue(property.Alias, language);
                    break;

                default:
                    value = property.Value;
                    break;
            }

            return value;
        }

        private IPublishedContent GetPageByUmbLink(string umbLink)
        {
            return !string.IsNullOrEmpty(umbLink) ? Udi.Parse(umbLink)?.ToPublishedContent() : null;
        }
    }

    public class Result
    {
        public bool success { get; set; }
        public string message { get; set; }
        public object data { get; set; }
    }

    public static class ResultMessage
    {
        public static string OK => "OK";
    }
}