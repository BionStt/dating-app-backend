using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using System.Data;

namespace Matching.Application.Infrastructure.Dapper
{
    public class DapperContext : IDapperContext
    {
        private readonly string _connectionString;
        private readonly IOptions<DapperConfig> _config;

        public DapperContext(IOptions<DapperConfig> config)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _connectionString = _config.Value.ConnectionString;
        }

        public IDbConnection CreateConnection()
        {
            return new SqlConnection(_connectionString);
        }
    }
}
