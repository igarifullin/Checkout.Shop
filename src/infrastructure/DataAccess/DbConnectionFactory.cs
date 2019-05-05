using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace Shop.DataAccess
{
    public class DbConnectionFactory : IDbConnectionFactory
    {
        private readonly string _catalogConnectionString;

        public DbConnectionFactory(IConfiguration configuration)
        {
            _catalogConnectionString = configuration.GetConnectionString("CatalogConnectionString");
        }

        public IDbConnection CreateCatalogConnection()
        {
            var connection = new SqlConnection(_catalogConnectionString);
            connection.Open();
            return connection;
        }
    }
}