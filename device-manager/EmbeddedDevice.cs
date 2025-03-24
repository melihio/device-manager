namespace device_manager;

public class EmbeddedDevice: Device
{
    public string IpAddress;
    public string NetworkName;

    public EmbeddedDevice(string ipAddress, string networkName, string id, string name, bool turnedOn)
    {
        this.IpAddress = ipAddress;
        this.NetworkName = networkName;
        this.id = id;
        this.name = name;
        this.TurnedOn = turnedOn;
    }
}