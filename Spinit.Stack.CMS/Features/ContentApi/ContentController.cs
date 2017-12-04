using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
            var languages = Services.LocalizationService.GetAllLanguages();

            var requestedLanguage = languages.SingleOrDefault(x => x.IsoCode.StartsWith(language));

            if (requestedLanguage == null)
            {
                return new Result
                {
                    success = false,
                    message = $"No defined language with code:{language}"
                };
            }

            var allItems = ContentService.GetAllDictionaryItems(requestedLanguage.Id);

            var dictionaryItems = allItems.Select(x =>
               new Dictionary<string, object>()
                   {{ x.Key,
                        x.Value}
                   });

            var translations = dictionaryItems.SelectMany(d => d).ToDictionary(e => e.Key, e => e.Value);

            return new Result
            {
                success = true,
                message = ResultMessage.OK,
                data = translations
            };
        }

        // Umbraco/api/content/Page/?id=X,Y,Z&language=sv&custom={'multinodeTreepicker':{take:3,evaluate:true}}
        [System.Web.Http.HttpGet]
        public object Page(string id, string language = null, string custom = null)
        {
            if (string.IsNullOrEmpty(id))
            {
                return new Result
                {
                    success = false,
                    message = "Specify on or more id's"
                };
            }

            var customAsJson = (JObject) JsonConvert.DeserializeObject(custom);

            var allPageIds = id?.Split(',').Select(idString => Convert.ToInt32(idString));

            var pageList = GetPageContent(allPageIds, language, customAsJson);

            if (pageList.FirstOrDefault() == null)
            {
                return new Result
                {
                    success = false,
                    message = "No page found"
                };
            }

            return new Result
            {
                success = true,
                message = ResultMessage.OK,
                data = pageList
            };
        }

        private List<Dictionary<string, object>> GetPageContent(IEnumerable<int> pageIds, string language, JObject custom = null)
        {
            var pageContent = new List<Dictionary<string, object>>();

            foreach (var pageId in pageIds)
            {
                var page = Umbraco.TypedContent(pageId);

                if(page == null)
                    continue;
               
                var contentType = Services.ContentService.GetById(page.Id);

                var customProperties = contentType.Properties.ToDictionary(property => property.Alias,
                    property =>
                    {
                        var customs = custom?[property.Alias];
                        if (customs != null)
                        {
                            return GetPropertyValue(property, page, language, customs);
                        }
                        return GetPropertyValue(property, page, language);
                    }

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

                pageContent.Add(umbracoProperties);
            }

            return pageContent;
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

        private object GetPropertyValue(Property property, IPublishedContent page, string language, JToken customs = null)
        {
            object value;
            var evaluate = customs?["evaluate"] != null && customs["evaluate"].Value<bool>();
            var take = (customs != null && customs["take"]?.Value<int>() > 0) ? customs["take"]?.Value<int?>() : null;

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

                    if (evaluate && value != null)
                    {
                        value = GetPageContent(new List<int> { Convert.ToInt32(value) }, language).FirstOrDefault();
                    }
                    
                    break;

                case "Umbraco.MultiNodeTreePicker2":
                    var contentUdiList = property.Value?.ToString().Split(',');
                    var contentList = contentUdiList?.Select(GetPageByUmbLink).Select(contentUdiPage => contentUdiPage?.Id);

                    value = contentList;

                    if (evaluate && contentList != null)
                    {
                        var contentListIds = contentList.Select(id => GetPageContent(new List<int> { Convert.ToInt32(id) }, language).FirstOrDefault());

                        value = contentListIds;

                        if (take != null)
                        {
                            value = contentListIds.Take(Convert.ToInt32(take));
                        }
                    }

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