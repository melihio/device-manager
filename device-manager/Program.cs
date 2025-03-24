using device_manager;

Console.WriteLine("type help to see available commands.");
while (true)
{
    DeviceManager deviceManager = DeviceManager.GetInstance("./input.txt");
    var command = Console.ReadLine()?.Split(" ");
    
    if(command == null)
    {
        Console.WriteLine("Please enter a command");
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
            Console.WriteLine(">add");
            break;
        case "delete":
            Console.WriteLine(">delete");
            break;
        case "turn-on":
            break;
        default: 
            Console.WriteLine(">unexpected input");
            break;
    }
}
