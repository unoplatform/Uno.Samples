using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml.Serialization;
using System.Reflection;

namespace TelerikApp.Business.Services;

public class DataGenerator
{
    private IResourceService _resources;
    private ISerializationService _serialization;

    public DataGenerator(IResourceService resources, ISerializationService serialization)
    {
        _resources = resources;
        _serialization = serialization;
    }

    public T GetItems<T>(string path)
    {
        var stream = _resources.GetResourceStream(path);
        T items = _serialization.XmlDeserializeFromStream<T>(stream);

        return items;
    }
}

public interface ISerializationService
{
    T XmlDeserializeFromStream<T>(Stream stream);
}

public class SerializationService : ISerializationService
{
    public T XmlDeserializeFromStream<T>(Stream stream)
    {
        T deserializedObject;
        using (var reader = new StreamReader(stream))
        {
            var serializer = new XmlSerializer(typeof(T));
            deserializedObject = (T)serializer.Deserialize(reader);
        }

        return deserializedObject ?? throw new Exception("Unable to deserialize object");
    }
}


public interface IResourceService
{
    Stream GetResourceStream(string name);

    long GetResourceSize(string resourceName);

    IEnumerable<string> GetResourceNamesFromFolder(string v);
}

public class AssemblyResourceService : IResourceService
{
    public IEnumerable<string> GetResourceNamesFromFolder(string folderName)
    {
        var assembly = GetCurrentAssembly();
        var resourceNames = assembly.GetManifestResourceNames().Where(p => p.Contains(folderName)).Select(p => GetFileName(folderName, p));

        return resourceNames;
    }

    public long GetResourceSize(string resourceName)
    {
        using (Stream stream = this.GetResourceStream(resourceName))
        {
            return stream.Length;
        }
    }

    public Stream GetResourceStream(string name)
    {
        var assembly = GetCurrentAssembly();
        var resourceNames = assembly.GetManifestResourceNames();
        var resourceName = resourceNames.Where(p => p.Contains(name)).FirstOrDefault();

        if (string.IsNullOrEmpty(resourceName))
        {
            throw new ArgumentException($"Missing resource with given name: {name}", nameof(name));
        }

        return assembly.GetManifestResourceStream(resourceName) ?? Stream.Null;
    }

    private static Assembly GetCurrentAssembly()
    {
        return typeof(AssemblyResourceService).GetTypeInfo().Assembly;
    }

    private static string GetFileName(string folderName, string resourceName)
    {
        int index = resourceName.IndexOf(folderName);
        return resourceName.Substring(index + folderName.Length + 1);
    }
}
