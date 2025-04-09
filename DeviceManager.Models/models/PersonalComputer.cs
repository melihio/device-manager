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
        this.id = id;
        this.name = name;
    }

    public override void TurnOn()
    {
        if (OperatingSystem == null) throw new EmptySystemException();
        TurnedOn = true;
    }

    public override void TurnOff()
    {
        TurnedOn = false;
    }
    
    public override string ToString()
    {
        return $"Personal computer - Name: {name} Id: {id} OperatingSystem: {OperatingSystem} Status: {TurnedOn}";
    }
}