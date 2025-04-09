namespace device_manager.managers;

public static class CommandManager
{
    public static void HandleHelp()
    {
        Print("available commands: help, exit, view, add-data, add-device, delete, update, turn-on, turn-off");
    }

    public static void HandleExit()
    {
        Environment.Exit(0);
    }

    public static void HandleAddData(string[] command)
    {
        var line = "";
        foreach (var cmd in command.Skip(1).ToArray())
        {
            line += " " + cmd;
        }
        FileManager.AddLine("input.txt",line);
    }
    
    public static void HandleGetAllDevices()
    {
        DeviceManager.GetInstance("input.txt").GetAllDevices().ForEach(Console.WriteLine);
    }

    public static void HandleClear()
    {
        Console.Clear();
    }

    public static void HandleUpdateDevice(string[] command)
    {
        try
        {
            var line = "";
            foreach (var cmd in command.Skip(1).ToArray())
                line += cmd;

            var device = DeviceManager.GetDeviceByString(line);

            var deviceType = DeviceManager.GetDeviceType(device);
            DeviceManager.GetInstance("input.txt").UpdateDevice(deviceType, device);
            Print("device successfully updated");
        }
        catch (Exception)
        {
            Print("device not found");
            Print("usage:");
            Print("update-device (SW/P/ED)-<id>,<name>,<isTurnedOn>,<battery%>");
        }
    }

    public static void HandleUnexpectedInput()
    {
        Print("unexpected input");
    }

    public static void HandleAddDevice(string[] command)
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
            DeviceManager.GetInstance("input.txt").AddDevice(deviceType, line);
            Print("device successfully added");
        }
        catch (Exception)
        {
            Print("usage:");
            Print("add-device (SW/P/ED)-<id>,<name>,<isTurnedOn>,<battery%>");
        }
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