using SQLite;
using MessagePack;

namespace ScottPlotDataPersistedSample;

// Using MessagePack for efficient serialization
[MessagePackObject]
public class Series
{
    // SQLite requires a primary key
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    // Store data points as a serialized string
    public string DataPointsSerialized { get; set; }

    public double Origin { get; set; }

    // This property is not stored in the database but is populated from the serialized string
    [Ignore]
    // Use IgnoreMember for MessagePack
    [IgnoreMember]
    public double[] DataPoints
    {
        get => MessagePackSerializer.Deserialize<double[]>(Convert.FromBase64String(DataPointsSerialized));
        set => DataPointsSerialized = Convert.ToBase64String(MessagePackSerializer.Serialize(value));
    }
}
