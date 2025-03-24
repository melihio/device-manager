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
            Print("SW-<id>,<name>,<isTurnedOn>,<battery%>");
            Print("P-<id>,<name>,<isTurnedOn>,<OS?>");
            Print("ED-<id>,<name>,<ip>,<networkName>");

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

    public static void HandleDeleteDevice()
    {
        Print("delete");
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