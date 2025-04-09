using device_manager.managers;

public class Main
{
    public static void main(string[] args)
    {
        Console.ForegroundColor = ConsoleColor.DarkBlue;
        Console.WriteLine("type help to see available commands.");
        
        while (true)
        {
            Console.Write(">");
            var command = Console.ReadLine()?.Split(" ");
            
            if(command == null)
            {
                CommandManager.Print("Please enter a command");
                continue;
            }
            
            switch (command[0])
            {
                case "exit":
                    CommandManager.HandleExit();
                    break;
                case "clear":
                    CommandManager.HandleClear();
                    break;
                case "help":
                    CommandManager.HandleHelp();       
                    break;
                case "add-data":
                    CommandManager.HandleAddData(command);
                    break;
                case "view":
                    CommandManager.HandleGetAllDevices();
                    break;
                case "add-device":
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
    }
}
