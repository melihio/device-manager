using device_manager;

Console.WriteLine("type help to see available commands.");
while (true)
{
    DeviceManager deviceManager = DeviceManager.GetInstance("./input.txt");
    Console.Write(">");
    var command = Console.ReadLine()?.Split(" ");
    
    if(command == null)
    {
        CommandManager.Print("Please enter a command");
        continue;
    }
    
    switch (command[0])
    {
        case "help":
            CommandManager.HandleHelp();       
            break;
        case "view":
            CommandManager.HandleGetAllDevices();
            break;
        case "add":
            CommandManager.HandleAddDevice(command);
            break;
        case "delete":
            CommandManager.HandleDeleteDevice(command);
            break;
        case "update":
            CommandManager.HandleUpdateDevice(command);
            break;
        case "turn-on":
            CommandManager.HandleDeviceTurnOn(command);
            break;
        case "turn-off":
            CommandManager.HandleDeviceTurnOff(command);
            break;
        default:
            CommandManager.HandleUnexpectedInput();
            break;
    }
}
