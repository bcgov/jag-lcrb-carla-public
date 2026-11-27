namespace Portal.ArchitectureTests;

public class ProjectReferenceTests
{
    public static TheoryData<string> SourceProjects =>
        [.. Backend.Projects.Where(project => project.Kind is not ProjectKind.Test and not ProjectKind.Host).Select(project => project.Name)];

    [Theory]
    [MemberData(nameof(SourceProjects))]
    public void Project_references_only_allowed_projects(string name)
    {
        var project = Backend.Find(name);
        string[] allowed = project.Kind switch
        {
            ProjectKind.SharedKernel => [],
            ProjectKind.Platform => [Backend.SharedKernel],
            ProjectKind.Module or ProjectKind.Integration => [Backend.SharedKernel, Backend.Platform],
            _ => throw new InvalidOperationException($"No reference rule for {project.Kind}"),
        };

        var disallowed = project.ProjectReferences.Except(allowed).ToList();

        Assert.True(disallowed.Count == 0, $"{name} must not reference: {string.Join(", ", disallowed)}");
    }

    [Fact]
    public void Host_references_every_module_and_integration()
    {
        var missing = Backend.Modules.Concat(Backend.Integrations)
            .Except(Backend.Find(Backend.Host).ProjectReferences)
            .ToList();

        Assert.True(missing.Count == 0, $"Portal.Host does not compose: {string.Join(", ", missing)}");
    }
}
