using device_manager.exceptions;
using device_manager.models;

namespace device_manager.models;

interface IPowerNotifier
{
    bool CheckBattery();
}

public class Smartwatch: Device, IPowerNotifier
{
    public int Battery { get; set; }

    public Smartwatch(int battery,bool turnedOn,string id, string name)
    {
        this.Battery = battery;
        this.id = id;
        this.name = name;
        this.TurnedOn = turnedOn;
    }

    public override void TurnOn()
    {
        CheckBattery();
        TurnedOn = true;
    }

    public override void TurnOff()
    {
        TurnedOn = false;
    }

    public bool CheckBattery()
    {
        switch (Battery)
        {
            case > 100:
                Console.WriteLine("battery level higher than 100");
                return false;
            case < 10:
                throw new EmptyBatteryException();
            case < 20:
                Console.WriteLine("battery too low.");
                return false;
            default:
                return true;
        }
    }
    
    public override string ToString()
    {
        return $"SmartWatch - Name: {name} Id: {id} Battery: {Battery} Status: {TurnedOn})";
    }
}