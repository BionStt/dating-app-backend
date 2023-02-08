using System.Data;

namespace Matching.Application.Infrastructure.Dapper
{
    public interface IDapperContext
    {
        IDbConnection CreateConnection();
    }
}
