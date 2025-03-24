namespace device_manager;

interface IPowerNotifier
{
    bool CheckBattery();
}

public class Smartwatch: Device, IPowerNotifier
{
    private int battery { get; set; }

    public Smartwatch(int battery,bool turnedOn,string id, string name)
    {
        this.battery = battery;
    }

    public void TurnOn()
    {
        CheckBattery();
        TurnedOn = true;
    }

    public bool CheckBattery()
    {
        if (battery > 100)
        {
            Console.WriteLine("battery level higher than 100");
            return false;
        }
        else if (battery < 0)
        {
            Console.WriteLine("battery level lower than 0");
            return false;
        }
        else if (battery < 20)
        {
            Console.WriteLine("battery too low.");
            return false;
        }
        return true;
    }
    
    public override string ToString()
    {
        return $"SmartWatch - Name: {name} Id: {id} Battery: {battery} Status: {TurnedOn})";
    }
}