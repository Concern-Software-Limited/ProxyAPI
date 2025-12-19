using System.Data;
using Dapper;
using MySql.Data.MySqlClient;

namespace ProxyAPI.Repositories;

/// <summary>
/// Dapper repository for high-performance database queries
/// Uses the same connection string as EF Core
/// </summary>
public class DapperRepository : IDapperRepository
{
    private readonly IConfiguration _configuration;
    private readonly string _connectionString;

    public DapperRepository(IConfiguration configuration)
    {
        _configuration = configuration;
        _connectionString = _configuration.GetConnectionString("DefaultConnection") 
            ?? throw new InvalidOperationException("Connection string not found");
    }

    /// <summary>
    /// Create a new MySQL connection
    /// </summary>
    private IDbConnection CreateConnection()
    {
        return new MySqlConnection(_connectionString);
    }

    /// <summary>
    /// Execute a query and return multiple results
    /// </summary>
    public async Task<IEnumerable<T>> QueryAsync<T>(string sql, object? parameters = null)
    {
        using var connection = CreateConnection();
        return await connection.QueryAsync<T>(sql, parameters);
    }

    /// <summary>
    /// Execute a query and return a single result or default
    /// </summary>
    public async Task<T?> QueryFirstOrDefaultAsync<T>(string sql, object? parameters = null)
    {
        using var connection = CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<T>(sql, parameters);
    }

    /// <summary>
    /// Execute a command (INSERT, UPDATE, DELETE) and return affected rows
    /// </summary>
    public async Task<int> ExecuteAsync(string sql, object? parameters = null)
    {
        using var connection = CreateConnection();
        return await connection.ExecuteAsync(sql, parameters);
    }
}
