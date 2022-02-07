namespace FluentWebRoutes.Attributes;

public class ProjectNameAttribute : Attribute
{
    public string Name { get; set; }

    public ProjectNameAttribute(string name)
    {
        this.Name = name;
    }
}