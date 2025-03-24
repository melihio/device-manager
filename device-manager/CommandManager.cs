namespace device_manager;

public static class CommandManager
{
    public static void HandleHelp()
    {
        Console.WriteLine(">available commands: help, add, delete");
    }
    public static void HandleGetAllDevices()
    {
        DeviceManager.GetInstance("input.txt").GetAllDevices().ForEach(Console.WriteLine);
    }

    public static void HandleDeviceTurnOn(string deviceType, string deviceId)
    {
        DeviceManager.GetInstance("input.txt").GetDevice(deviceType,deviceId).TurnOn();
        Console.WriteLine("Device successfully turned on");
    }
    public static void HandleDeviceTurnOff(string deviceType, string deviceId)
    {
        DeviceManager.GetInstance("input.txt").GetDevice(deviceType,deviceId).TurnOff();
        Console.WriteLine("Device successfully turned off");
    }
}