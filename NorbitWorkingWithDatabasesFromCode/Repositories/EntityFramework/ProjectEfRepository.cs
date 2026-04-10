using Microsoft.EntityFrameworkCore;
using NorbitWorkingWithDatabasesFromCode.Models;
using NorbitWorkingWithDatabasesFromCode.Repositories.Interfaces;
using Task = System.Threading.Tasks.Task;

namespace NorbitWorkingWithDatabasesFromCode.Repositories.EntityFramework;

public class ProjectEfRepository(EducationDbContext context) : IProjectRepository
{
    private readonly EducationDbContext _context = context ?? throw new ArgumentNullException(nameof(context));

    public async Task<Project?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Projects
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<Project>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Projects
            .AsNoTracking()
            .OrderBy(p => p.Id)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Project entity, CancellationToken cancellationToken = default)
    {
        await _context.Projects.AddAsync(entity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Project entity, CancellationToken cancellationToken = default)
    {
        var rowsAffected = await _context.Projects
            .Where(p => p.Id == entity.Id)
            .ExecuteUpdateAsync(s => s
                .SetProperty(p => p.Name, entity.Name)
                .SetProperty(p => p.Budget, entity.Budget), cancellationToken);

        if (rowsAffected == 0)
            throw new InvalidOperationException($"Project with Id {entity.Id} was not found.");
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var rowsAffected = await _context.Projects
            .Where(p => p.Id == id)
            .ExecuteDeleteAsync(cancellationToken);

        if (rowsAffected == 0)
            throw new InvalidOperationException($"Project with Id {id} was not found.");
    }

    public async Task<IEnumerable<Project>> GetProjectsByMinBudgetAsync(decimal minBudget,
        CancellationToken cancellationToken = default)
    {
        return await _context.Projects
            .AsNoTracking()
            .Where(p => p.Budget >= minBudget)
            .OrderBy(p => p.Id)
            .ToListAsync(cancellationToken);
    }
}