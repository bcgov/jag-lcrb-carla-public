using NetArchTest.Rules;

namespace Portal.ArchitectureTests;

public class TypeDependencyTests
{
    private static readonly string[] FrameworkNamespaces =
    [
        "Microsoft.AspNetCore",
        "Microsoft.EntityFrameworkCore",
        "Microsoft.Extensions",
        "Microsoft.PowerPlatform",
        "Microsoft.Xrm",
        "System.Net.Http",
        "System.Text.Json",
    ];

    private static readonly string[] Layers = ["Endpoints", "Application", "Domain", "Infrastructure"];

    public static TheoryData<string> Modules => [.. Backend.Modules];

    public static TheoryData<string> Integrations => [.. Backend.Integrations];

    [Theory]
    [MemberData(nameof(Modules))]
    public void Module_does_not_depend_on_other_modules_integrations_or_host(string module)
    {
        var forbidden = Backend.Modules.Where(other => other != module)
            .Concat(["Portal.Integrations", Backend.Host])
            .ToArray();

        AssertSuccess(Types.InAssembly(Backend.Load(module)).ShouldNot().HaveDependencyOnAny(forbidden).GetResult());
    }

    [Theory]
    [MemberData(nameof(Integrations))]
    public void Integration_does_not_depend_on_modules_or_host(string integration)
    {
        var forbidden = Backend.Modules.Append(Backend.Host).ToArray();

        AssertSuccess(Types.InAssembly(Backend.Load(integration)).ShouldNot().HaveDependencyOnAny(forbidden).GetResult());
    }

    [Fact]
    public void Shared_kernel_and_platform_do_not_depend_on_modules_integrations_or_host()
    {
        var forbidden = Backend.Modules.Concat(["Portal.Integrations", Backend.Host]).ToArray();

        AssertSuccess(Types.InAssembly(Backend.Load(Backend.SharedKernel)).ShouldNot().HaveDependencyOnAny([.. forbidden, Backend.Platform]).GetResult());
        AssertSuccess(Types.InAssembly(Backend.Load(Backend.Platform)).ShouldNot().HaveDependencyOnAny(forbidden).GetResult());
    }

    [Theory]
    [MemberData(nameof(Modules))]
    public void Domain_does_not_depend_on_framework_transport_or_data_platform(string module)
    {
        AssertSuccess(Types.InAssembly(Backend.Load(module))
            .That().ResideInNamespace($"{module}.Domain")
            .ShouldNot().HaveDependencyOnAny(FrameworkNamespaces)
            .GetResult());
    }

    [Theory]
    [MemberData(nameof(Modules))]
    public void Module_exposes_only_its_module_class(string module)
    {
        var moduleClass = $"{Backend.ShortName(module)}Module";

        AssertSuccess(Types.InAssembly(Backend.Load(module))
            .That().ArePublic().And().DoNotHaveName(moduleClass)
            .Should().NotBePublic()
            .GetResult());
    }

    [Theory]
    [MemberData(nameof(Modules))]
    public void Module_types_live_in_the_module_namespace_or_a_layer(string module)
    {
        var misplaced = Types.InAssembly(Backend.Load(module)).GetTypes()
            .Where(type => !type.Name.StartsWith('<'))
            .Where(type => !IsInModuleOrLayer(module, type.Namespace))
            .Select(type => type.FullName)
            .ToList();

        Assert.True(misplaced.Count == 0, $"{module} types outside its layers: {string.Join(", ", misplaced)}");
    }

    [Theory]
    [InlineData("Portal.Accounts", true)]
    [InlineData("Portal.Accounts.Domain", true)]
    [InlineData("Portal.Accounts.Domain.Rules", true)]
    [InlineData("Portal.Accounts.DomainOther", false)]
    [InlineData("Portal.Accounts.ApplicationExtra", false)]
    [InlineData("Portal.AccountsOther", false)]
    [InlineData(null, false)]
    public void Layer_namespace_check_requires_exact_or_dotted_match(string? ns, bool expected) =>
        Assert.Equal(expected, IsInModuleOrLayer("Portal.Accounts", ns));

    private static bool IsInModuleOrLayer(string module, string? ns) =>
        ns == module || Layers.Any(layer => ns == $"{module}.{layer}" || ns?.StartsWith($"{module}.{layer}.") == true);

    private static void AssertSuccess(NetArchTest.Rules.TestResult result) =>
        Assert.True(result.IsSuccessful, $"Failing types: {string.Join(", ", result.FailingTypeNames ?? [])}");
}
