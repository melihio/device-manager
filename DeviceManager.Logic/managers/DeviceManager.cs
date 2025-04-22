using device_manager.models;
using Microsoft.Data.SqlClient;

namespace device_manager.managers;

public class DeviceManager
{
    private readonly String _connectionString;
    
    public DeviceManager(string connectionString)
    {
        _connectionString = connectionString;
        _devices = [];
        ReadDevices();
    }
    
    private readonly List<Device> _devices;

    public List<Device> GetAllDevices()
    {
        return _devices;
    }

    public void ReadDevices()
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            var sql = "SELECT * FROM Devices";
            var command = new SqlCommand(sql, connection);
            var reader = command.ExecuteReader();
            while (reader.Read())
            {
                if (reader.HasRows)
                {
                    var id = reader["Id"].ToString();
                    var name = reader["Name"].ToString();
                    var turnedOn = false;
                    if (reader["TurnedOn"] != DBNull.Value)
                    {
                        turnedOn = Convert.ToBoolean(reader["TurnedOn"]);
                    }

                    var device = new Device
                    {
                        id = id!,
                        name = name!,
                        TurnedOn = turnedOn!
                    };
                    
                    _devices.Add(device);
                }
            }
        }
    }

    public void AddDevice(string deviceType, Device device)
    {
        // if (_devices.Count >= 15)
        // {
        //     throw new InvalidOperationException("Maximum number of devices is 15");
        // }
        //
        // foreach (var d in _devices)
        // {
        //     if (GetDeviceType(d) == deviceType && d.id == device.id)
        //     {
        //         throw new ArgumentException("Device with given id already exists");
        //     }
        // }
        //
        // string line = device switch
        // {
        //     Smartwatch sw => $"{deviceType}-{sw.id},{sw.name},{sw.TurnedOn},{sw.Battery}%",
        //     PersonalComputer pc => $"{deviceType}-{pc.id},{pc.name},{pc.TurnedOn},{pc.OperatingSystem}",
        //     EmbeddedDevice ed => $"{deviceType}-{ed.id},{ed.name},{ed.IpAddress},{ed.NetworkName}",
        //     _ => throw new ArgumentException("Invalid device type")
        // };
        //
        // FileManager.AddLine(_filePath, line);
        // ReadDevicesFromFile();
    }

    public void UpdateDevice(string deviceType, Device device)
    {
        // FileManager.UpdateLine(_filePath, deviceType ,device);
        // ReadDevicesFromFile();
    }

    public void DeleteDevice(string deviceType, string deviceId)
    {
        // var lines = FileManager.GetAllLines(_filePath);
        // if (!lines.Any(line => line.StartsWith($"{deviceType}-{deviceId},")))
        // {
        //     throw new ArgumentException("Device doesn't exists.");
        // }
        // FileManager.DeleteLine(_filePath, deviceType , deviceId);
        // ReadDevicesFromFile();
    }

    public Device GetDeviceById(string deviceId)
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            var sql = "SELECT * FROM Devices WHERE Id = @id";
            var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@id", deviceId);
            var reader = command.ExecuteReader();
            if (reader.HasRows)
            {
                reader.Read();
                var type = reader["id"].ToString()!.Split('-')[0];
                Device device = type switch
                {
                    "SW" =>  new Smartwatch(
                        id: reader["Id"].ToString()!,
                        name: reader["Name"].ToString()!,
                        turnedOn: (bool)reader["turnedOn"],
                        battery: Convert.ToInt32(reader["BatteryLevel"])
                    ),
                    "ED" => new EmbeddedDevice(
                        id: reader["Id"].ToString()!,
                        name: reader["name"].ToString()!,
                        ipAddress: reader["IpAddress"].ToString()!,
                        networkName: reader["WifiName"].ToString()!
                    ),
                    "P" => new PersonalComputer(
                            id: reader["Id"].ToString()!,
                            name: reader["Name"].ToString()!,
                            turnedOn: (bool)reader["TurnedOn"],
                            operatingSystem: reader["operatingSystem"].ToString()
                    ),
                };
                return device;
            }
            else
            {
                throw new KeyNotFoundException("Device not found");
            }
        }
    }

    private void ReadDevicesFromFile()
    {
        // var lines = FileManager.GetAllLines(_filePath);
        // foreach (var device in lines.Select(GetDeviceByString).OfType<Device>())
        //     _devices.Add(device);
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
}