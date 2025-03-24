using device_manager;

Console.WriteLine("you can type help to see available commands.");
while (true)
{
    DeviceManager deviceManager = DeviceManager.GetDeviceManager("./input.txt");
    var command = Console.ReadLine().Split(" ");
    
    switch (command[0])
    {
        case "help":
            Console.WriteLine(">available commands: help, add, delete");       
            break;
        case "view":
            deviceManager.ReadDevicesFromFile();
            break;
        case "add":
            Console.WriteLine(">add");
            break;
        case "delete":
            Console.WriteLine(">delete");
            break;
        default: 
            Console.WriteLine(">unexpected input");
            break;
    }
}
