using System.Xml.Linq;

namespace Portal.ArchitectureTests;

public class ProjectSettingsTests
{
    public static TheoryData<string> AllProjects => [.. Backend.Projects.Select(project => project.Name)];

    [Theory]
    [MemberData(nameof(AllProjects))]
    public void Project_uses_shared_target_framework(string name)
    {
        var document = XDocument.Load(Backend.Find(name).Path);

        Assert.DoesNotContain(document.Descendants(), element => element.Name.LocalName is "TargetFramework" or "TargetFrameworks");
    }

    [Theory]
    [MemberData(nameof(AllProjects))]
    public void Package_versions_are_central(string name)
    {
        var versioned = XDocument.Load(Backend.Find(name).Path).Descendants("PackageReference")
            .Where(reference => reference.Attribute("Version") is not null || reference.Element("Version") is not null)
            .Select(reference => reference.Attribute("Include")?.Value)
            .ToList();

        Assert.True(versioned.Count == 0, $"{name} sets its own version for: {string.Join(", ", versioned)}");
    }

    [Theory]
    [MemberData(nameof(AllProjects))]
    public void Source_projects_have_no_package_references(string name)
    {
        var project = Backend.Find(name);
        if (project.Kind is ProjectKind.Test)
        {
            return;
        }

        Assert.Empty(XDocument.Load(project.Path).Descendants("PackageReference"));
    }
}
