using System.Data;
using Microsoft.Data.SqlClient;
using NorbitWorkingWithDatabasesFromCode.Models;
using NorbitWorkingWithDatabasesFromCode.Repositories.Interfaces;
using Task = System.Threading.Tasks.Task;

namespace NorbitWorkingWithDatabasesFromCode.Repositories.AdoNet;

public class ProjectAdoRepository(string connectionString) : IProjectRepository
{
    private readonly string _connectionString =
        connectionString ?? throw new ArgumentNullException(nameof(connectionString));

    public async Task<Project?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        var sql = "SELECT Id, Name, Budget, CreatedAt FROM Projects WHERE Id = @Id";

        await using var command = new SqlCommand(sql, connection);
        command.Parameters.Add("@Id", SqlDbType.Int).Value = id;

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);

        if (await reader.ReadAsync(cancellationToken)) return MapToProject(reader);

        return null;
    }

    public async Task<IEnumerable<Project>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var projects = new List<Project>();

        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        var sql = "SELECT Id, Name, Budget, CreatedAt FROM Projects ORDER BY Id";

        await using var command = new SqlCommand(sql, connection);
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);

        while (await reader.ReadAsync(cancellationToken)) projects.Add(MapToProject(reader));

        return projects;
    }

    public async Task AddAsync(Project entity, CancellationToken cancellationToken = default)
    {
        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        var sql = """

                                  INSERT INTO Projects (Name, Budget) 
                                  VALUES (@Name, @Budget);
                                  SELECT CAST(SCOPE_IDENTITY() as int);
                  """;

        await using var command = new SqlCommand(sql, connection);
        command.Parameters.Add("@Name", SqlDbType.NVarChar, 100).Value = entity.Name;
        var budgetParameter = command.Parameters.Add("@Budget", SqlDbType.Decimal);
        budgetParameter.Precision = 10;
        budgetParameter.Scale = 2;
        budgetParameter.Value = entity.Budget.HasValue ? entity.Budget.Value : DBNull.Value;

        var result = await command.ExecuteScalarAsync(cancellationToken);
        if (result is not int id)
            throw new InvalidOperationException("Failed to insert project and retrieve generated Id.");
        entity.Id = id;
    }

    public async Task UpdateAsync(Project entity, CancellationToken cancellationToken = default)
    {
        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        var sql = """

                                  UPDATE Projects 
                                  SET Name = @Name, Budget = @Budget 
                                  WHERE Id = @Id
                  """;

        await using var command = new SqlCommand(sql, connection);
        command.Parameters.Add("@Id", SqlDbType.Int).Value = entity.Id;
        command.Parameters.Add("@Name", SqlDbType.NVarChar, 100).Value = entity.Name;
        var budgetParameter = command.Parameters.Add("@Budget", SqlDbType.Decimal);
        budgetParameter.Precision = 10;
        budgetParameter.Scale = 2;
        budgetParameter.Value = entity.Budget.HasValue ? entity.Budget.Value : DBNull.Value;

        var rowsAffected = await command.ExecuteNonQueryAsync(cancellationToken);
        if (rowsAffected == 0) throw new InvalidOperationException($"Project with Id {entity.Id} was not found.");
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        var sql = "DELETE FROM Projects WHERE Id = @Id";

        await using var command = new SqlCommand(sql, connection);
        command.Parameters.Add("@Id", SqlDbType.Int).Value = id;

        var rowsAffected = await command.ExecuteNonQueryAsync(cancellationToken);
        if (rowsAffected == 0) throw new InvalidOperationException($"Project with Id {id} was not found.");
    }

    public async Task<IEnumerable<Project>> GetProjectsByMinBudgetAsync(decimal minBudget,
        CancellationToken cancellationToken = default)
    {
        var projects = new List<Project>();

        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        var sql = "SELECT Id, Name, Budget, CreatedAt FROM Projects WHERE Budget >= @MinBudget";

        await using var command = new SqlCommand(sql, connection);
        var minBudgetParameter = command.Parameters.Add("@MinBudget", SqlDbType.Decimal);
        minBudgetParameter.Precision = 10;
        minBudgetParameter.Scale = 2;
        minBudgetParameter.Value = minBudget;

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);

        while (await reader.ReadAsync(cancellationToken)) projects.Add(MapToProject(reader));

        return projects;
    }

    private Project MapToProject(SqlDataReader reader)
    {
        return new Project
        {
            Id = reader.GetInt32(reader.GetOrdinal("Id")),
            Name = reader.GetString(reader.GetOrdinal("Name")),
            Budget = reader.IsDBNull(reader.GetOrdinal("Budget"))
                ? null
                : reader.GetDecimal(reader.GetOrdinal("Budget")),
            CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt"))
        };
    }
}