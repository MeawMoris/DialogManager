using System;

[AttributeUsage(AttributeTargets.Class)]
public class SelectableComponentAttribute : System.Attribute
{
    public string DisplayName { get; private set; }

    public SelectableComponentAttribute(string DisplayName)
    {
        this.DisplayName = DisplayName;
    }
}