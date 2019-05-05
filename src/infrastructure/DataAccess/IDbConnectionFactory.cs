using System.Data;

namespace Shop.DataAccess
{
    public interface IDbConnectionFactory
    {
        IDbConnection CreateCatalogConnection();
    }
}