using System.Globalization;
using System.Reflection;
using TelerikApp.MauiControls.Common;

namespace TelerikApp.MauiControls.Helpers;

internal static class Utils
{
    public static Type? GetExampleViewModelType(string controlName, string exampleName)
    {
        AssemblyName assemblyName = GetAssemblyName();
        string typeName = string.Format("QSF.Examples.{0}Control.{1}Example.{2}ViewModel", controlName, exampleName, exampleName);
        var type = GetTypeFromTypeName(assemblyName, typeName);
        return type;
    }

    public static View CreateConfigurationArea(Example example)
    {
        return new Label { Text = "Dan did not implement this..." };
    }

    public static string? GetExampleCodeURL(Example example)
    {
        if (example is null)
        {
            return null;
        }

        if (!string.IsNullOrEmpty(example.CodeUrl))
        {
            return example.CodeUrl;
        }
        return null;
    }

    private static Type? GetTypeFromTypeName(AssemblyName assemblyName, string typeName)
    {
        string fullTypeName = string.Format(CultureInfo.InvariantCulture, "{0}, {1}", typeName, assemblyName.FullName);
        var type = Type.GetType(fullTypeName);
        return type;
    }

    private static AssemblyName GetAssemblyName()
    {
        AssemblyName assemblyName = typeof(Utils).GetTypeInfo().Assembly.GetName();
        return assemblyName;
    }
}
