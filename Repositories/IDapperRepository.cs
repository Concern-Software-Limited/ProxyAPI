namespace ProxyAPI.Repositories;

/// <summary>
/// Interface for Dapper-based data access operations
/// Provides high-performance SQL queries
/// </summary>
public interface IDapperRepository
{
    Task<IEnumerable<T>> QueryAsync<T>(string sql, object? parameters = null);
    Task<T?> QueryFirstOrDefaultAsync<T>(string sql, object? parameters = null);
    Task<int> ExecuteAsync(string sql, object? parameters = null);
}
