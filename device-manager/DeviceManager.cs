namespace device_manager;

public class DeviceManager
{
    private DeviceManager(string filePath)
    {
        this._filePath = filePath;
        devices = new List<Device>();
    }

    private static DeviceManager? _deviceManager;
    private string _filePath;
    private List<Device> devices;

    public static DeviceManager GetDeviceManager(string filePath)
    {
        return _deviceManager ??= new DeviceManager(filePath);
    }

    public void ReadDevicesFromFile()
    {
        string[] lines = File.ReadAllLines(_filePath);
        for (int i = 0; i < lines.Length; i++)
        {
            string[] values = lines[i].Split(',');
            string[] deviceType = values[0].Split('-');
            bool turnedOn = false;
            switch (deviceType[0])
            {
                case "P":
                    turnedOn = bool.Parse(values[2]);
                    var operatingSystem = values.Length == 4 ? values[3] : null;
                    var pc = new PersonalComputer(operatingSystem,turnedOn,deviceType[1],values[1]);
                    devices.Add(pc);
                    break;
                case "ED":
                    var ed = new EmbeddedDevice(values[2],values[3],deviceType[1],values[1], false);
                    devices.Add(ed);
                    break;
                case "SW":
                    turnedOn = bool.Parse(values[2]);
                    var battery = int.Parse(values[3].Replace("%",""));
                    var sw = new Smartwatch(battery, turnedOn,deviceType[1],values[1]);
                    break;
            }
        }

        foreach (var device in devices)
        {
            Console.WriteLine(device.ToString());   
        }
    }
}