namespace device_manager;

public class DeviceManager
{
    private DeviceManager(string filePath)
    {
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException("File not found", filePath);
        }
        _filePath = filePath;
        _devices = [];
        ReadDevicesFromFile();
    }

    private static DeviceManager? _deviceManager;
    private readonly string _filePath;
    private readonly List<Device> _devices;

    public static DeviceManager GetInstance(string filePath)
    {
        return _deviceManager ??= new DeviceManager(filePath);
    }

    public List<Device> GetAllDevices()
    {
        return _devices;
    }

    public void AddDevice(string deviceType, string deviceData)
    {
        var lines = File.ReadAllLines(_filePath);
        var values = deviceData.Split(',');

        if (values.Length < 3)
        {
            Console.WriteLine("Error: Invalid device format.");
            return;
        }

        string[] deviceInfo = values[0].Split('-');
        if (deviceInfo.Length != 2)
        {
            Console.WriteLine("Error: Invalid device ID format.");
            return;
        }

        string deviceId = deviceInfo[1];
        
        if (lines.Any(line => line.StartsWith($"{deviceType}-{deviceId},")))
        {
            Console.WriteLine("Error: Device with the same ID already exists.");
            return;
        }
        
        
        FileManager.AddLine(_filePath, deviceData);
    }



    public void UpdateDevice(String deviceType, Device device)
    {
        FileManager.UpdateLine(_filePath, deviceType ,device);
        ReadDevicesFromFile();
    }

    public void DeleteDevice(string deviceType, string deviceId)
    {
        FileManager.DeleteLine(_filePath, deviceType , deviceId);
        ReadDevicesFromFile();
    }

    public Device GetDeviceById(string deviceType, string deviceId)
    {
        switch (deviceType)
        {
            case "SW":
                return _devices.OfType<Smartwatch>().First(d => d.id == deviceId);
            case "P":
                return _devices.OfType<PersonalComputer>().First(d => d.id == deviceId);
            case "ED":
                return _devices.OfType<EmbeddedDevice>().First(d => d.id == deviceId);
            default:
                throw new ArgumentException("Invalid device type");
        }
    }

    private void ReadDevicesFromFile()
    {
        var lines = FileManager.GetAllLines(_filePath);
        foreach (var t in lines)
        {
            var device = GetDeviceByString(t);
            if(device != null)
                _devices.Add(device);
        }
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