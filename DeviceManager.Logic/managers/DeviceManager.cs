using device_manager.models;

namespace device_manager.managers;

public class DeviceManager
{
    public DeviceManager(string filePath)
    {
        var basePath = AppContext.BaseDirectory;
        var fullPath = Path.Combine(basePath, filePath);
        FileManager.CheckFile(fullPath);
        _filePath = fullPath;
        _devices = [];
        ReadDevicesFromFile();
    }
    
    private readonly string _filePath;
    private readonly List<Device> _devices;

    public List<Device> GetAllDevices()
    {
        return _devices;
    }

    public void AddDevice(string deviceType, Device device)
    {
        if (_devices.Count >= 15)
        {
            throw new InvalidOperationException("Maximum number of devices is 15");
        }
        
        foreach (var d in _devices)
        {
            if (GetDeviceType(d) == deviceType && d.id == device.id)
            {
                throw new ArgumentException("Device with given id already exists");
            }
        }
        
        string line = device switch
        {
            Smartwatch sw => $"{deviceType}-{sw.id},{sw.name},{sw.TurnedOn},{sw.Battery}%",
            PersonalComputer pc => $"{deviceType}-{pc.id},{pc.name},{pc.TurnedOn},{pc.OperatingSystem}",
            EmbeddedDevice ed => $"{deviceType}-{ed.id},{ed.name},{ed.IpAddress},{ed.NetworkName}",
            _ => throw new ArgumentException("Invalid device type")
        };
        
        FileManager.AddLine(_filePath, line);
        ReadDevicesFromFile();
    }

    public void UpdateDevice(string deviceType, Device device)
    {
        FileManager.UpdateLine(_filePath, deviceType ,device);
        ReadDevicesFromFile();
    }

    public void DeleteDevice(string deviceType, string deviceId)
    {
        var lines = FileManager.GetAllLines(_filePath);
        if (!lines.Any(line => line.StartsWith($"{deviceType}-{deviceId},")))
        {
            throw new ArgumentException("Device doesn't exists.");
        }
        FileManager.DeleteLine(_filePath, deviceType , deviceId);
        ReadDevicesFromFile();
    }

    public Device GetDeviceById(string deviceType, string deviceId)
    {
        return deviceType switch
        {
            "SW" => _devices.OfType<Smartwatch>().First(d => d.id == deviceId),
            "P" => _devices.OfType<PersonalComputer>().First(d => d.id == deviceId),
            "ED" => _devices.OfType<EmbeddedDevice>().First(d => d.id == deviceId),
            _ => throw new ArgumentException("Invalid device type")
        };
    }

    private void ReadDevicesFromFile()
    {
        var lines = FileManager.GetAllLines(_filePath);
        foreach (var device in lines.Select(GetDeviceByString).OfType<Device>())
            _devices.Add(device);
    }

    public static string GetDeviceType(Device device)
    {
        return device switch
        {
            Smartwatch => "SW",
            PersonalComputer => "P",
            EmbeddedDevice => "ED",
            _ => throw new ArgumentException("Unknown device type")
        };
    }

    public static Device? GetDeviceByString(string line)
    {
        var values = line.Split(',');
        var deviceType = values[0].Split('-');

        if (deviceType.Length < 2)
        {
            return null;
        }

        switch (deviceType[0])
        {
            case "P":
                var operatingSystem = values.Length == 4 ? values[3] : null;
                return new PersonalComputer(operatingSystem, bool.Parse(values[2]), deviceType[1], values[1]);
            case "ED":
                return new EmbeddedDevice(values[2], values[3], deviceType[1], values[1], false);
            case "SW":
                var battery = int.Parse(values[3].Replace("%", ""));
                return new Smartwatch(battery, bool.Parse(values[2]), deviceType[1], values[1]);
            default:
                return null;
        }
    }
}