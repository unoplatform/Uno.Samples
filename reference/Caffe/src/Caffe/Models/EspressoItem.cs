namespace Caffe.Models;

public record EspressoItem(
    string Name,
    int VolumeML,
    string Description
)
{
    public string VolumeDisplay => $"{VolumeML}ml";
}
