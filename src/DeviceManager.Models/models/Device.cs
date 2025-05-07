using System.Text.Json.Serialization;

namespace device_manager.models;

/// <summary>
///  A base class for existing models.
/// </summary>
public class Device
{
    public bool TurnedOn { get; set; }
    public string Id { get; set; }
    public string Name { get; set; }
    
    [JsonIgnore]
    public byte[] RowVersion { get; set; }
    public virtual void TurnOn(){}
    public virtual void TurnOff(){}
    public virtual void Validate(){}
}