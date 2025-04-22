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
            const string Sql = "SELECT * FROM Devices";
            var command = new SqlCommand(Sql, connection);
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
                        Id = id!,
                        Name = name!,
                        TurnedOn = turnedOn!
                    };
                    
                    _devices.Add(device);
                }
            }
        }
    }

    public void AddDevice(Device device)
    {
        if (_devices.Any(d => d.Id == device.Id))
            throw new InvalidOperationException($"Device with given Id already exists");

        using (var conn = new SqlConnection(_connectionString))
        {
            try
            { 
                conn.Open();
                
                const string Sql = "INSERT INTO Devices (Id, Name, DeviceType, BatteryLevel, IPAddress, WifiName, TurnedOn VALUES (@Id, @Name, @DeviceType, @Battery, @IP, @Wifi, @TurnedOn)";

                var command = new SqlCommand(Sql, conn);

                command.Parameters.AddWithValue("@Id", device.Id);
                command.Parameters.AddWithValue("@Name", device.Name);

                switch (device)
                {
                    case Smartwatch sw:
                        command.Parameters.AddWithValue("@DeviceType", "SW");
                        command.Parameters.AddWithValue("@Battery", sw.Battery);
                        command.Parameters.AddWithValue("@IP", DBNull.Value);
                        command.Parameters.AddWithValue("@Wifi", DBNull.Value);
                        command.Parameters.AddWithValue("@TurnedOn", sw.TurnedOn);
                        break;

                    case PersonalComputer pc:
                        command.Parameters.AddWithValue("@DeviceType", "PC");
                        command.Parameters.AddWithValue("@Battery", DBNull.Value);
                        command.Parameters.AddWithValue("@IP", DBNull.Value);
                        command.Parameters.AddWithValue("@Wifi", DBNull.Value);
                        command.Parameters.AddWithValue("@TurnedOn", pc.TurnedOn);
                        break;

                    case EmbeddedDevice ed:
                        command.Parameters.AddWithValue("@DeviceType", "ED");
                        command.Parameters.AddWithValue("@Battery", DBNull.Value);
                        command.Parameters.AddWithValue("@IP", ed.IpAddress);
                        command.Parameters.AddWithValue("@Wifi", ed.NetworkName);
                        command.Parameters.AddWithValue("@TurnedOn", ed.TurnedOn);
                        break;

                    default:
                        throw new ArgumentException("Unknown device type");
                }
                
                    command.ExecuteNonQuery();
                    _devices.Add(device);
                    ReadDevices();
            }
            catch (SqlException ex)
            {
                throw new InvalidOperationException($"Failed to add device: {ex.Message}");
            }  
        };
    }

    public void UpdateDevice(Device device)
    {
        if (_devices.All(d => d.Id != device.Id))
            throw new InvalidOperationException($"No device found with given Id");

        using (var conn = new SqlConnection(_connectionString))
        {
            try
            {
                conn.Open();

                const string Sql = "UPDATE Devices SET Name = @Name, DeviceType = @DeviceType, BatteryLevel = @Battery, IPAddress = @IP, WifiName = @Wifi, TurnedOn = @TurnedOn WHERE Id = @Id";

                var command = new SqlCommand(Sql, conn);

                command.Parameters.AddWithValue("@Id", device.Id);
                command.Parameters.AddWithValue("@Name", device.Name);

                switch (device)
                {
                    case Smartwatch sw:
                        command.Parameters.AddWithValue("@DeviceType", "SW");
                        command.Parameters.AddWithValue("@Battery", sw.Battery);
                        command.Parameters.AddWithValue("@IP", DBNull.Value);
                        command.Parameters.AddWithValue("@Wifi", DBNull.Value);
                        command.Parameters.AddWithValue("@TurnedOn", sw.TurnedOn);
                        break;

                    case PersonalComputer pc:
                        command.Parameters.AddWithValue("@DeviceType", "PC");
                        command.Parameters.AddWithValue("@Battery", DBNull.Value);
                        command.Parameters.AddWithValue("@IP", DBNull.Value);
                        command.Parameters.AddWithValue("@Wifi", DBNull.Value);
                        command.Parameters.AddWithValue("@TurnedOn", pc.TurnedOn);
                        break;

                    case EmbeddedDevice ed:
                        command.Parameters.AddWithValue("@DeviceType", "ED");
                        command.Parameters.AddWithValue("@Battery", DBNull.Value);
                        command.Parameters.AddWithValue("@IP", ed.IpAddress);
                        command.Parameters.AddWithValue("@Wifi", ed.NetworkName);
                        command.Parameters.AddWithValue("@TurnedOn", ed.TurnedOn);
                        break;

                    default:
                        throw new ArgumentException("Unknown device type");
                }
                
                if (command.ExecuteNonQuery() == 0)
                    throw new InvalidOperationException($"No device found with Id '{device.Id}' in the database.");
                
                ReadDevices();
            }
            catch (SqlException ex)
            {
                throw new InvalidOperationException($"Failed to update device: {ex.Message}");
            }
        }
    }

    public void DeleteDevice(string deviceId)
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            const string Sql = "DELETE FROM Devices WHERE ID = @Id";
            var command = new SqlCommand(Sql, connection);
            command.Parameters.AddWithValue("@Id", deviceId);
            
            if (command.ExecuteNonQuery() == 0)
            {
                throw new KeyNotFoundException($"No device found with given Id");
            }
        }
    }

    public Device GetDeviceById(string deviceId)
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            const string Sql = "SELECT * FROM Devices WHERE Id = @id";
            var command = new SqlCommand(Sql, connection);
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
}