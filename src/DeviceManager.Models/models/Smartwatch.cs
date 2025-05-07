using device_manager.exceptions;
using device_manager.models;

namespace device_manager.models;

interface IPowerNotifier
{
    void CheckBattery();
}

public class Smartwatch: Device, IPowerNotifier
{
    public int Battery { get; set; }

    public Smartwatch(int battery,bool turnedOn,string id, string name)
    {
        Battery = battery;
        Id = id;
        Name = name;
        TurnedOn = turnedOn;
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

    public override void Validate()
    {
        CheckBattery();
    }

    public void CheckBattery()
    {
        switch (Battery)
        {
            case > 100:
                throw new ArgumentOutOfRangeException(nameof(Battery), "Battery percentage can't exceed 100%");
            case < 20:
                throw new EmptyBatteryException();
        }
    }
    
    public override string ToString()
    {
        return $"SmartWatch - Name: {Name} Id: {Id} Battery: {Battery} Status: {TurnedOn})";
    }
}