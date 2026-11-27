namespace Portal.ArchitectureTests;

public class StructureTests
{
    private static readonly string[] ModuleFolders =
    [
        "Endpoints",
        "Application/Commands",
        "Application/Queries",
        "Application/Authorisation",
        "Application/Ports",
        "Domain/Status",
        "Domain/Rules",
        "Domain/Events",
        "Infrastructure/Repositories",
        "Infrastructure/Mapping",
    ];

    public static TheoryData<string> RequiredPaths =>
    [
        "Portal.slnx",
        "Directory.Build.props",
        "Directory.Packages.props",
        "src/Portal.SharedKernel/Portal.SharedKernel.csproj",
        "src/Portal.Platform/Portal.Platform.csproj",
        "src/Portal.Host/Portal.Host.csproj",
        "src/Portal.Host/Program.cs",
        "src/Portal.Host/Configuration",
        "src/Portal.Host/Authentication",
        "src/Portal.Host/Authorisation",
        "src/Portal.Host/Http",
        "src/Portal.Host/OpenApi",
        "src/Portal.Host/HealthChecks",
        "src/Portal.Host/Observability",
        "src/Modules/Portal.Identity",
        "src/Modules/Portal.ReferenceData",
        "src/Modules/Portal.Accounts",
        "src/Modules/Portal.Licences",
        "src/Modules/Portal.Applications",
        "src/Modules/Portal.SpecialEvents",
        "src/Modules/Portal.LegalEntity",
        "src/Modules/Portal.Partners",
        "src/Modules/Portal.Staff",
        "src/Modules/Portal.Assistant",
        "src/Integrations/Portal.Integrations.Dataverse",
        "src/Integrations/Portal.Integrations.Dataverse/Repositories",
        "src/Integrations/Portal.Integrations.Dataverse/Queries",
        "src/Integrations/Portal.Integrations.Dataverse/Caching",
        "src/Integrations/Portal.Integrations.Dataverse/Writes",
        "src/Integrations/Portal.Integrations.Dataverse/Resilience",
        "src/Integrations/Portal.Integrations.Documents",
        "src/Integrations/Portal.Integrations.Pdf",
        "src/Integrations/Portal.Integrations.Geocoding",
        "src/Integrations/Portal.Integrations.Registries",
        "src/Integrations/Portal.Integrations.Payments",
        "src/Integrations/Portal.Integrations.IdentityDirectory",
        "src/Integrations/Portal.Integrations.OneStop",
        "src/Integrations/Portal.Integrations.Notifications",
        "tests/Portal.UnitTests/Portal.UnitTests.csproj",
        "tests/Portal.IntegrationTests/Portal.IntegrationTests.csproj",
        "tests/Portal.ContractTests/Portal.ContractTests.csproj",
        "tests/Portal.ArchitectureTests/Portal.ArchitectureTests.csproj",
    ];

    public static TheoryData<string> Modules => [.. Backend.Modules];

    public static TheoryData<string> Integrations => [.. Backend.Integrations];

    [Theory]
    [MemberData(nameof(RequiredPaths))]
    public void Required_path_exists(string path)
    {
        var full = Path.Combine(Backend.Root, path);
        Assert.True(File.Exists(full) || Directory.Exists(full), $"{path} is missing");
    }

    [Theory]
    [MemberData(nameof(Modules))]
    public void Module_has_project_layer_folders_and_module_class(string module)
    {
        var root = Path.Combine(Backend.Src, "Modules", module);
        var moduleClass = $"{Backend.ShortName(module)}Module.cs";
        Assert.True(File.Exists(Path.Combine(root, $"{module}.csproj")), $"{module}.csproj is missing");
        Assert.True(File.Exists(Path.Combine(root, moduleClass)), $"{module} has no {moduleClass}");
        var missing = ModuleFolders.Where(folder => !Directory.Exists(Path.Combine(root, folder))).ToList();
        Assert.True(missing.Count == 0, $"{module} is missing folders: {string.Join(", ", missing)}");
    }

    [Theory]
    [MemberData(nameof(Integrations))]
    public void Integration_has_project_and_module_class(string integration)
    {
        var root = Path.Combine(Backend.Src, "Integrations", integration);
        var moduleClass = $"{Backend.ShortName(integration)}Module.cs";
        Assert.StartsWith("Portal.Integrations.", integration);
        Assert.True(File.Exists(Path.Combine(root, $"{integration}.csproj")), $"{integration}.csproj is missing");
        Assert.True(File.Exists(Path.Combine(root, moduleClass)), $"{integration} has no {moduleClass}");
    }

    [Fact]
    public void Every_project_is_in_the_solution()
    {
        var solution = File.ReadAllText(Path.Combine(Backend.Root, "Portal.slnx")).Replace('\\', '/');
        var missing = Backend.Projects
            .Where(project => !solution.Contains(Path.GetRelativePath(Backend.Root, project.Path).Replace('\\', '/')))
            .Select(project => project.Name)
            .ToList();
        Assert.True(missing.Count == 0, $"Not in Portal.slnx: {string.Join(", ", missing)}");
    }
}
