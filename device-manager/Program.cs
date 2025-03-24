Console.WriteLine("you can type help to see available commands.");
while (true)
{
    var command = Console.ReadLine().Split(" ");

    switch (command[0])
    {
        case "help":
            Console.WriteLine(">available commands: help, add, delete");       
            break;
        case "add":
            Console.WriteLine(">add");
            break;
        case "delete":
            Console.WriteLine(">delete");
            break;
    }
}
