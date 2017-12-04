using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using StackExchange.Profiling.Helpers.Dapper;

namespace Spinit.Stack.CMS.Features.ContentApi
{
    public class SimpleDictionaryItem
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }
    public class Language
    {
        public string languageISOCode { get; set; }
        public string languageCultureName { get; set; }
    }
    public class ContentService
    {
        public static IEnumerable<SimpleDictionaryItem> GetAllDictionaryItems(int languageId)
        {
            var db = new Db();
            var sql = $@"  
                      SELECT [key],[value] FROM [dbo].[cmsDictionary]
                      JOIN [dbo].[cmsLanguageText] ON cmsLanguageText.UniqueId = cmsDictionary.id
                      JOIN [dbo].[umbracoLanguage] ON umbracoLanguage.id = cmsLanguageText.languageId
                      WHERE umbracoLanguage.id = {languageId}
                ";

            IEnumerable<SimpleDictionaryItem> items = null;
            using (var connection = db.Connect())
            {
                items = connection.Query<SimpleDictionaryItem>(sql).ToList();
            }

            return items;
        }   
    }
}