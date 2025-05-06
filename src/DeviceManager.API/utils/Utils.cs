using device_manager.models;

namespace HTTPApi.utils;

public class Utils
{
    public static string GenerateDeviceId(string type, List<Device> devices)
    {
        var prefix = type.ToLower() switch
        {
            "sw" => "SW",
            "pc" => "PC",
            "ed" => "ED",
            _ => throw new ArgumentException($"Invalid device type: {type}")
        };
    
        var count = devices.Count(d => d.GetType().Name.ToLower() == type.ToLower() || d.Id.StartsWith(prefix));
        return $"{prefix}-{count + 1}";
    }
}