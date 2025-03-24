namespace device_manager;

public static class FileManager
{
    public static void AddLine(string filePath, string line)
    {
        File.AppendAllText(filePath,"\n"+line);
    }
    
    public static void DeleteLine(string filePath, string deviceType, string deviceId)
    {
        var lines = File.ReadAllLines(filePath).ToList();
        
        for (var i = 0; i < lines.Count; i++)
        {
            var values = lines[i].Split(',');
            var deviceInfo = values[0].Split('-');

            if (deviceInfo[0] == deviceType && deviceInfo[1] == deviceId)
            {
               lines.RemoveAt(i);
            } 
        };
        
        File.WriteAllLines(filePath, lines);
    }
    public static void UpdateLine(string filePath, string deviceType, Device updatedDevice)
    {
        var lines = File.ReadAllLines(filePath).ToList();
        
        for (var i = 0; i < lines.Count; i++)
        {
            var values = lines[i].Split(',');
            var deviceInfo = values[0].Split('-');

            if (deviceInfo[0] == deviceType && deviceInfo[1] == updatedDevice.id)
            {
                lines[i] = updatedDevice switch
                {
                    Smartwatch sw => $"{deviceType}-{sw.id},{sw.name},{sw.TurnedOn},{sw.Battery}%",
                    PersonalComputer pc => $"{deviceType}-{pc.id},{pc.name},{pc.TurnedOn},{pc.OperatingSystem}",
                    EmbeddedDevice ed => $"{deviceType}-{ed.id},{ed.name},{ed.IpAddress},{ed.NetworkName}",
                    _ => throw new ArgumentException("Invalid device type")
                };
            } 
        };
        
        File.WriteAllLines(filePath, lines);
    }
    public static List<string> GetAllLines(string filePath)
    {
        return File.ReadAllLines(filePath).ToList();
    }
}