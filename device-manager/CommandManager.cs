namespace device_manager;

public static class CommandManager
{
    public static void HandleHelp()
    {
        Print("available commands: help, add, delete");
    }
    public static void HandleGetAllDevices()
    {
        DeviceManager.GetInstance("input.txt").GetAllDevices().ForEach(Console.WriteLine);
    }

    public static void HandleUpdateDevice(string[] command)
    {
        try
        {
            var line = "";
            foreach (var cmd in command.Skip(1).ToArray())
            {
                line += cmd;
            }

            var device = DeviceManager.GetDeviceByString(line);
            var deviceType = DeviceManager.GetDeviceType(device);
            DeviceManager.GetInstance("input.txt").UpdateDevice(deviceType, device);
            Print("device successfully updated");
        }
        catch (Exception)
        {
            Print("Correct use:");
            Print("update SW-<id>,<name>,<isTurnedOn>,<battery%>");
            Print("update P-<id>,<name>,<isTurnedOn>,<OS?>");
            Print("update ED-<id>,<name>,<ip>,<networkName>");

        }
    }

    public static void HandleUnexpectedInput()
    {
        Print("unexpected input");
    }

    public static void HandleAddDevice()
    {
        Print("add");
    }

    public static void HandleDeleteDevice(string[] command)
    {
        var deviceType = command[1].Split("-")[0];
        var deviceId = command[1].Split("-")[1];
        DeviceManager.GetInstance("input.txt").DeleteDevice(deviceType, deviceId);
        Print("device successfully deleted");
    }

    public static void HandleDeviceTurnOn(string[] command)
    {
        DeviceManager.GetInstance("input.txt").GetDeviceById(command[1],command[2]).TurnOn();
        Print("Device successfully turned on");
    }
    public static void HandleDeviceTurnOff(string[] command)
    {
        DeviceManager.GetInstance("input.txt").GetDeviceById(command[1],command[2]).TurnOff();
        Print("Device successfully turned off");
    }

    public static void Print(string? input)
    {
        Console.WriteLine("$ "+input);
    }
}