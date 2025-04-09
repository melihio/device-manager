namespace device_manager.models;

public class Device
{
    public bool TurnedOn { get; set; }
    public string id { get; set; }
    public string name { get; set; }

    public virtual void TurnOn(){}
    public virtual void TurnOff(){}
}