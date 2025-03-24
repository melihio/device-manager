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
}