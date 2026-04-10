using NorbitWorkingWithDatabasesFromCode.Models;
using NorbitWorkingWithDatabasesFromCode.Repositories.Interfaces;
using Task = System.Threading.Tasks.Task;

namespace NorbitWorkingWithDatabasesFromCode;

public sealed class ProjectDemoRunner(IProjectRepository projectRepository)
{
    public async Task RunAsync(CancellationToken cancellationToken = default)
    {
        var newProject = new Project { Name = "Новый проект", Budget = 50000m };

        await projectRepository.AddAsync(newProject, cancellationToken);
        Console.WriteLine($"Проект создан. ID: {newProject.Id}");

        newProject.Name = "Обновленный проект";
        newProject.Budget = 75000m;
        await projectRepository.UpdateAsync(newProject, cancellationToken);
        Console.WriteLine($"Проект обновлен. ID: {newProject.Id}");

        var projects = await projectRepository.GetAllAsync(cancellationToken);
        Console.WriteLine("\nСписок проектов в БД:");
        foreach (var project in projects)
            Console.WriteLine($"- [{project.Id}] {project.Name} (Бюджет: {project.Budget})");

        var minBudgetProjects = await projectRepository.GetProjectsByMinBudgetAsync(60000m, cancellationToken);
        Console.WriteLine("\nПроекты с бюджетом >= 60000:");
        foreach (var project in minBudgetProjects)
            Console.WriteLine($"- [{project.Id}] {project.Name} (Бюджет: {project.Budget})");

        await projectRepository.DeleteAsync(newProject.Id, cancellationToken);
        Console.WriteLine($"\nТестовый проект с ID {newProject.Id} удален.");
    }
}