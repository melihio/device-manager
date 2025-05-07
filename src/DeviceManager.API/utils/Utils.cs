using device_manager.models;

namespace HTTPApi.utils;

public class Utils
{
    public static string GenerateDeviceId(string type, List<Device> devices)
    {
        foreach (var device in devices)
        {
            Console.WriteLine(device);
        }
        
        var count = devices.Count(d => d.Id.Split('-')[0].ToLower() == type.ToLower());
        var id = $"{type}-{count + 1}";
        Console.WriteLine(id);
        return id;
    }
}