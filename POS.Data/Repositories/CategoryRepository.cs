using System.Data.Common;
using POS.Data;
using POS.Core.Contracts;
using POS.Core.Models;

namespace POS.Data.Repositories;

public sealed class CategoryRepository : ICategoryRepository
{
    private readonly IDbConnectionFactory _factory;

    public CategoryRepository(IDbConnectionFactory factory) => _factory = factory;

    public async Task<IReadOnlyList<Category>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var list = new List<Category>();
        await using var conn = _factory.CreateConnection();
        await conn.OpenAsync(cancellationToken).ConfigureAwait(false);
        await using var cmd = conn.CreateTextCommand("SELECT id, name FROM categories ORDER BY id ASC");
        await using var reader = await cmd.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);
        while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
            list.Add(new Category { Id = reader.GetInt32(0), Name = reader.GetString(1), MenuItems = new List<MenuItem>() });
        return list;
    }

    public async Task<Category?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        await using var conn = _factory.CreateConnection();
        await conn.OpenAsync(cancellationToken).ConfigureAwait(false);
        await using var cmd = conn.CreateTextCommand("SELECT id, name FROM categories WHERE id = @Id");
        cmd.Parameters.AddWithValue("@Id", id);
        await using var reader = await cmd.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);
        if (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
            return new Category { Id = reader.GetInt32(0), Name = reader.GetString(1) };
        return null;
    }

    public async Task AddAsync(Category category, CancellationToken cancellationToken = default)
    {
        await using var conn = _factory.CreateConnection();
        await conn.OpenAsync(cancellationToken).ConfigureAwait(false);
        await using var cmd = conn.CreateTextCommand("INSERT INTO categories (name) VALUES (@Name) RETURNING id");
        cmd.Parameters.AddWithValue("@Name", category.Name ?? "");
        category.Id = (int)(await cmd.ExecuteScalarAsync(cancellationToken).ConfigureAwait(false) ?? 0);
    }

    public async Task UpdateAsync(Category category, CancellationToken cancellationToken = default)
    {
        await using var conn = _factory.CreateConnection();
        await conn.OpenAsync(cancellationToken).ConfigureAwait(false);
        await using var cmd = conn.CreateTextCommand("UPDATE categories SET name = @Name WHERE id = @Id");
        cmd.Parameters.AddWithValue("@Name", category.Name ?? "");
        cmd.Parameters.AddWithValue("@Id", category.Id);
        await cmd.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
    }

    public async Task DeleteAsync(int categoryId, CancellationToken cancellationToken = default)
    {
        await using var conn = _factory.CreateConnection();
        await conn.OpenAsync(cancellationToken).ConfigureAwait(false);
        await using var cmd = conn.CreateTextCommand("DELETE FROM categories WHERE id = @Id");
        cmd.Parameters.AddWithValue("@Id", categoryId);
        await cmd.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
    }
}
