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

            switch (deviceType[0])
            {
                case "P":
                    bool turnedOn = bool.Parse(values[2]);
                    PersonalComputer pc = new PersonalComputer(values[3],turnedOn,deviceType[1],values[1]);
                    devices.Add(pc);
                    break;
            }
        }
    }
}