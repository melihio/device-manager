using device_manager.exceptions;
using device_manager.models;

namespace device_manager.models;

// public enum OperatingSystem
// {
//     Windows,
//     MacOS,
//     Linux,
// }

public class PersonalComputer: Device
{
    public string? OperatingSystem { get; set; }

    public PersonalComputer(string? operatingSystem, bool turnedOn, string id, string name)
    {
        this.OperatingSystem = operatingSystem;
        this.TurnedOn = turnedOn;
        this.Id = id;
        this.Name = name;
    }

    public override void Validate()
    {
        if (OperatingSystem == null) throw new EmptySystemException();
    }

    public override void TurnOn()
    {
        
        Validate();
        TurnedOn = true;
    }

    public override void TurnOff()
    {
        TurnedOn = false;
    }
    
    public override string ToString()
    {
        return $"Personal computer - Name: {Name} Id: {Id} OperatingSystem: {OperatingSystem} Status: {TurnedOn}";
    }
}