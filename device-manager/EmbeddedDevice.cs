using System.Text.RegularExpressions;

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

    // public void TurnOn()
    // {
    //     Connect();
    // }
    
    public void Connect()
    {
        string ipPattern = @"^((25[0-5]|2[0-4][0-9]|1?[0-9][0-9]?)\.){3}(25[0-5]|2[0-4][0-9]|1?[0-9][0-9]?)$";
        if (!Regex.IsMatch(this.IpAddress, ipPattern))
        {
            throw new ArgumentException("Invalid IP address");
        }

        if (!this.NetworkName.Contains("MD Ltd."))
        {
            throw new ConnectionException();
        }
    }
}