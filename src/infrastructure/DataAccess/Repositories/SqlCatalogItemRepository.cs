using Dapper;
using Shop.Domain;
using Shop.Domain.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Shop.DataAccess.Repositories
{
    public class SqlCatalogItemRepository : ICatalogItemRepository
    {
        private readonly IDbConnectionFactory _dbConnectionFactory;

        public SqlCatalogItemRepository(IDbConnectionFactory dbConnectionFactory)
        {
            _dbConnectionFactory = dbConnectionFactory;
        }

        public async Task<CatalogItem> GetCatalogItemAsync(int catalogItemId)
        {
            const string query = @"
SELECT TOP(1) Id,
              Name,
              Description,
              Price,
              PictureUrl,
              CatalogTypeId
";

            using (var connection = _dbConnectionFactory.CreateCatalogConnection())
            {
                return await connection.QueryFirstOrDefaultAsync<CatalogItem>(query, new { Id = catalogItemId }).ConfigureAwait(false);
            }
        }

        public async Task<IEnumerable<CatalogItem>> GetCatalogItemsAsync()
        {
            const string query = @"
SELECT Id,
        Name,
        Description,
        Price,
        PictureUrl,
        CatalogTypeId
";

            using (var connection = _dbConnectionFactory.CreateCatalogConnection())
            {
                return await connection.QueryAsync<CatalogItem>(query).ConfigureAwait(false);
            }
        }
    }
}