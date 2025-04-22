using System.Text.RegularExpressions;
using device_manager.exceptions;

namespace device_manager.models;

public class EmbeddedDevice: Device
{
    public string IpAddress { get; set; }
    public string NetworkName {get; set;}

    public EmbeddedDevice(string ipAddress, string networkName, string id, string name)
    {
        this.IpAddress = ipAddress;
        this.NetworkName = networkName;
        this.id = id;
        this.name = name;
    }

    public override void TurnOn()
    {
        Connect();
        TurnedOn = true;
    }

    public override void TurnOff()
    {
        TurnedOn = false;
    }
    
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
    
    public override string ToString()
    {
        return $"Embedded device - Name: {name} Id: {id} Network Name: {NetworkName} Ip Address: {IpAddress} Status: {TurnedOn}";
    }
}