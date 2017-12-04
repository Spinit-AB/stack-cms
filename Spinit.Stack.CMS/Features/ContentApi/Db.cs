using System.Configuration;
using System.Data.SqlClient;

namespace Spinit.Stack.CMS.Features.ContentApi
{
    public interface IDb
    {
        SqlConnection Connect(string name = "umbracoDbDSN");
    }

    public class Db : IDb
    {
        public SqlConnection Connect(string name = "umbracoDbDSN")
        {
            var db = new SqlConnection(ConfigurationManager.ConnectionStrings[name.ToLower()].ConnectionString);
            db.Open();
            return db;
        }
    }
}