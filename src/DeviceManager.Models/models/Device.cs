namespace device_manager.models;

public class Device
{
    public bool TurnedOn { get; set; }
    public string Id { get; set; }
    public string Name { get; set; }

    public virtual void TurnOn(){}
    public virtual void TurnOff(){}
    public virtual void Validate(){}
}