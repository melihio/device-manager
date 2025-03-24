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

    public Device GetDevice(string deviceType, string deviceId)
    {
        switch (deviceType)
        {
            case "SW":
                return _devices.OfType<Smartwatch>().First(d => d.id == deviceId);
            case "PC":
                return _devices.OfType<PersonalComputer>().First(d => d.id == deviceId);
            case "ED":
                return _devices.OfType<EmbeddedDevice>().First(d => d.id == deviceId);
            default:
                throw new ArgumentException("Invalid device type");
        }
    }

    public void ReadDevicesFromFile()
    {
        var lines = File.ReadAllLines(_filePath);
        foreach (var t in lines)
        {
            var values = t.Split(',');
            var deviceType = values[0].Split('-');
            switch (deviceType[0])
            {
                case "P":
                    var operatingSystem = values.Length == 4 ? values[3] : null;
                    var pc = new PersonalComputer(operatingSystem,bool.Parse(values[2]),deviceType[1],values[1]);
                    _devices.Add(pc);
                    break;
                case "ED":
                    var ed = new EmbeddedDevice(values[2],values[3],deviceType[1],values[1], false);
                    _devices.Add(ed);
                    break;
                case "SW":
                    var battery = int.Parse(values[3].Replace("%",""));
                    var sw = new Smartwatch(battery, bool.Parse(values[2]),deviceType[1],values[1]);
                    _devices.Add(sw);
                    break;
            }
        }
    }
}