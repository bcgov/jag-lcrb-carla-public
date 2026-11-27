using System.Reflection;
using System.Xml.Linq;

namespace Portal.ArchitectureTests;

internal enum ProjectKind { SharedKernel, Platform, Host, Module, Integration, Test }

internal sealed record Project(string Name, string Path, ProjectKind Kind)
{
    public IReadOnlyList<string> ProjectReferences => XDocument.Load(Path).Descendants("ProjectReference")
        .Select(reference => System.IO.Path.GetFileNameWithoutExtension(reference.Attribute("Include")!.Value.Replace('\\', '/')))
        .ToList();
}

internal static class Backend
{
    public const string SharedKernel = "Portal.SharedKernel";
    public const string Platform = "Portal.Platform";
    public const string Host = "Portal.Host";

    public static readonly string Root = FindRoot();

    public static string Src => System.IO.Path.Combine(Root, "src");

    public static IReadOnlyList<string> Modules => Folders("Modules");

    public static IReadOnlyList<string> Integrations => Folders("Integrations");

    public static IReadOnlyList<Project> Projects => Directory
        .EnumerateFiles(Root, "*.csproj", SearchOption.AllDirectories)
        .Where(path => !path.Split(System.IO.Path.DirectorySeparatorChar).Any(part => part is "bin" or "obj"))
        .Select(path => new Project(System.IO.Path.GetFileNameWithoutExtension(path), path, KindOf(path)))
        .OrderBy(project => project.Name)
        .ToList();

    public static Project Find(string name) => Projects.Single(project => project.Name == name);

    public static string ShortName(string project) => project.Split('.').Last();

    public static Assembly Load(string project) =>
        Assembly.LoadFrom(System.IO.Path.Combine(AppContext.BaseDirectory, project + ".dll"));

    private static ProjectKind KindOf(string path)
    {
        var relative = System.IO.Path.GetRelativePath(Root, path).Replace('\\', '/');
        var name = System.IO.Path.GetFileNameWithoutExtension(path);
        return relative switch
        {
            _ when relative.StartsWith("tests/") => ProjectKind.Test,
            _ when relative.StartsWith("src/Modules/") => ProjectKind.Module,
            _ when relative.StartsWith("src/Integrations/") => ProjectKind.Integration,
            _ when relative == $"src/{SharedKernel}/{SharedKernel}.csproj" => ProjectKind.SharedKernel,
            _ when relative == $"src/{Platform}/{Platform}.csproj" => ProjectKind.Platform,
            _ when relative == $"src/{Host}/{Host}.csproj" => ProjectKind.Host,
            _ => throw new InvalidOperationException($"{relative} ({name}) is outside the allowed project locations"),
        };
    }

    private static string[] Folders(string group) => Directory
        .GetDirectories(System.IO.Path.Combine(Src, group))
        .Select(folder => System.IO.Path.GetFileName(folder))
        .Order()
        .ToArray();

    private static string FindRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null && !File.Exists(System.IO.Path.Combine(directory.FullName, "Portal.slnx")))
        {
            directory = directory.Parent;
        }

        return directory?.FullName ?? throw new InvalidOperationException("Portal.slnx not found above the test output");
    }
}
