using NorbitWorkingWithDatabasesFromCode.Models;

namespace NorbitWorkingWithDatabasesFromCode.Repositories.Interfaces;

public interface IProjectRepository : IRepository<Project, int>
{
    Task<IEnumerable<Project>> GetProjectsByMinBudgetAsync(decimal minBudget,
        CancellationToken cancellationToken = default);
}