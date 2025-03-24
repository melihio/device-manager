namespace device_manager;

// public enum OperatingSystem
// {
//     Windows,
//     MacOS,
//     Linux,
// }

public class PersonalComputer: Device
{
    public string? operatingSystem { get; set; }

    public PersonalComputer(string? operatingSystem, bool turnedOn, string id, string name)
    {
        this.operatingSystem = operatingSystem;
        this.TurnedOn = turnedOn;
        this.id = id;
        this.name = name;
    }

    public void Launch()
    {
        if (operatingSystem == null) throw new EmptySystemException();
    }
}