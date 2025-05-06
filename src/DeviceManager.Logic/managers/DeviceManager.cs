using device_manager.models;
using Microsoft.Data.SqlClient;

namespace device_manager.managers;

public class DeviceManager
{
    private readonly string _connectionString;
    
    public DeviceManager(string connectionString)
    {
        _connectionString = connectionString;
        ReadDevices();
    }

    /// <summary>
    ///  Returns all the devices in database 
    /// </summary>
    public List<Device> GetAllDevices()
    {
        return ReadDevices();
    }

    /// <summary>
    ///  This method reads all devices from database. This method is called after every CRUD operation executed.
    /// </summary>
    public List<Device> ReadDevices()
    {
        List<Device> _devices = new List<Device>();

        using var conn = new SqlConnection(_connectionString);
        conn.Open();

        const string sql = @"
            SELECT 
                d.Id, d.Name, d.IsEnabled,
                CASE 
                    WHEN s.DeviceId IS NOT NULL THEN 'SW'
                    WHEN pc.DeviceId IS NOT NULL THEN 'P'
                    WHEN e.DeviceId IS NOT NULL THEN 'ED'
                END AS DeviceType,
                s.BatteryPercentage,
                e.IpAddress,
                e.NetworkName,
                pc.OperationSystem
            FROM Device d
            LEFT JOIN Smartwatch s ON d.Id = s.DeviceId
            LEFT JOIN PersonalComputer pc ON d.Id = pc.DeviceId
            LEFT JOIN Embedded e ON d.Id = e.DeviceId";

        var command = new SqlCommand(sql, conn);
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            string id = reader.GetString(0);
            string name = reader.GetString(1);
            bool isEnabled = reader.GetBoolean(2);
            string deviceType = reader.GetString(3);

            Device device = deviceType switch
            {
                "SW" => new Smartwatch
                (
                    id: id,
                    name: name,
                    turnedOn: isEnabled,
                    battery: reader.GetInt32(4)
                ),
                "P" => new PersonalComputer
                (
                    id: id,
                    name: name,
                    turnedOn: isEnabled,
                    operatingSystem: reader.IsDBNull(7) ? null : reader.GetString(7)
                ),
                "ED" => new EmbeddedDevice
                (
                    id: id,
                    name: name,
                    ipAddress: reader.GetString(5),
                    networkName: reader.GetString(6)
                ),
                _ => throw new InvalidOperationException("Unknown device type")
            };

            _devices.Add(device);
        }
        return _devices;
    }

    /// <summary>
    ///  This method adds given device to database.
    /// </summary>

    public void AddDevice(Device device)
    {
        List<Device> _devices = GetAllDevices();
        if (_devices.Any(d => d.Id == device.Id))
            throw new InvalidOperationException($"Device with given Id already exists");

        using var conn = new SqlConnection(_connectionString);
        try
        { 
            conn.Open();

            const string deviceSql = "INSERT INTO Device (Id, Name, IsEnabled) VALUES (@Id, @Name, @IsEnabled)";
            var deviceCommand = new SqlCommand(deviceSql, conn);
            deviceCommand.Parameters.AddWithValue("@Id", device.Id);
            deviceCommand.Parameters.AddWithValue("@Name", device.Name);
            deviceCommand.Parameters.AddWithValue("@IsEnabled", device.TurnedOn);
            deviceCommand.ExecuteNonQuery();

            switch (device)
            {
                case Smartwatch sw:
                    const string smartwatchSql = "INSERT INTO Smartwatch (BatteryPercentage, DeviceId) VALUES (@BatteryPercentage, @DeviceId)";
                    var smartwatchCommand = new SqlCommand(smartwatchSql, conn);
                    smartwatchCommand.Parameters.AddWithValue("@BatteryPercentage", sw.Battery);
                    smartwatchCommand.Parameters.AddWithValue("@DeviceId", device.Id);
                    smartwatchCommand.ExecuteNonQuery();
                    break;

                case PersonalComputer pc:
                    const string pcSql = "INSERT INTO PersonalComputer (OperationSystem, DeviceId) VALUES (@OperationSystem, @DeviceId)";
                    var pcCommand = new SqlCommand(pcSql, conn);
                    pcCommand.Parameters.AddWithValue("@OperationSystem", pc.OperatingSystem ?? (object)DBNull.Value);
                    pcCommand.Parameters.AddWithValue("@DeviceId", device.Id);
                    pcCommand.ExecuteNonQuery();
                    break;

                case EmbeddedDevice ed:
                    const string embeddedSql = "INSERT INTO Embedded (IpAddress, NetworkName, DeviceId) VALUES (@IpAddress, @NetworkName, @DeviceId)";
                    var embeddedCommand = new SqlCommand(embeddedSql, conn);
                    embeddedCommand.Parameters.AddWithValue("@IpAddress", ed.IpAddress);
                    embeddedCommand.Parameters.AddWithValue("@NetworkName", ed.NetworkName);
                    embeddedCommand.Parameters.AddWithValue("@DeviceId", device.Id);
                    embeddedCommand.ExecuteNonQuery();
                    break;

                default:
                    throw new ArgumentException("Unknown device type");
            }

            _devices.Add(device);
            ReadDevices();
        }
        catch (SqlException ex)
        {
            throw new InvalidOperationException($"Failed to add device: {ex.Message}");
        }
    }

    /// <summary>
    ///  This method updates an already existing device in the database
    /// </summary>
    public void UpdateDevice(Device device)
    {
        List<Device> _devices = GetAllDevices();
        if (_devices.All(d => d.Id != device.Id))
            throw new KeyNotFoundException($"No device found with given Id");

        using var conn = new SqlConnection(_connectionString);
        try
        {
            conn.Open();

            const string deviceSql = "UPDATE Device SET Name = @Name, IsEnabled = @IsEnabled WHERE Id = @Id";
            var deviceCommand = new SqlCommand(deviceSql, conn);
            deviceCommand.Parameters.AddWithValue("@Id", device.Id);
            deviceCommand.Parameters.AddWithValue("@Name", device.Name);
            deviceCommand.Parameters.AddWithValue("@IsEnabled", device.TurnedOn);
            deviceCommand.ExecuteNonQuery();

            switch (device)
            {
                case Smartwatch sw:
                    const string smartwatchSql = "UPDATE Smartwatch SET BatteryPercentage = @BatteryPercentage WHERE DeviceId = @DeviceId";
                    var smartwatchCommand = new SqlCommand(smartwatchSql, conn);
                    smartwatchCommand.Parameters.AddWithValue("@BatteryPercentage", sw.Battery);
                    smartwatchCommand.Parameters.AddWithValue("@DeviceId", device.Id);
                    if (smartwatchCommand.ExecuteNonQuery() == 0)
                        throw new InvalidOperationException($"No Smartwatch found with DeviceId '{device.Id}' in the database.");
                    break;

                case PersonalComputer pc:
                    const string pcSql = "UPDATE PersonalComputer SET OperationSystem = @OperationSystem WHERE DeviceId = @DeviceId";
                    var pcCommand = new SqlCommand(pcSql, conn);
                    pcCommand.Parameters.AddWithValue("@OperationSystem", pc.OperatingSystem ?? (object)DBNull.Value);
                    pcCommand.Parameters.AddWithValue("@DeviceId", device.Id);
                    if (pcCommand.ExecuteNonQuery() == 0)
                        throw new InvalidOperationException($"No PersonalComputer found with DeviceId '{device.Id}' in the database.");
                    break;

                case EmbeddedDevice ed:
                    const string embeddedSql = "UPDATE Embedded SET IpAddress = @IpAddress, NetworkName = @NetworkName WHERE DeviceId = @DeviceId";
                    var embeddedCommand = new SqlCommand(embeddedSql, conn);
                    embeddedCommand.Parameters.AddWithValue("@IpAddress", ed.IpAddress);
                    embeddedCommand.Parameters.AddWithValue("@NetworkName", ed.NetworkName);
                    embeddedCommand.Parameters.AddWithValue("@DeviceId", device.Id);
                    if (embeddedCommand.ExecuteNonQuery() == 0)
                        throw new InvalidOperationException($"No Embedded device found with DeviceId '{device.Id}' in the database.");
                    break;

                default:
                    throw new ArgumentException("Unknown device type");
            }

            ReadDevices();
        }
        catch (SqlException ex)
        {
            throw new InvalidOperationException($"Failed to update device: {ex.Message}");
        }
    }

    /// <summary>
    ///  This method deletes an existing device from database.
    /// </summary>

    public void DeleteDevice(string deviceId)
    {
        using var connection = new SqlConnection(_connectionString);
        connection.Open();

        const string childSql = @"
            DELETE FROM Smartwatch WHERE DeviceId = @Id;
            DELETE FROM PersonalComputer WHERE DeviceId = @Id;
            DELETE FROM Embedded WHERE DeviceId = @Id;";
        var childCommand = new SqlCommand(childSql, connection);
        childCommand.Parameters.AddWithValue("@Id", deviceId);
        childCommand.ExecuteNonQuery();

        const string deviceSql = "DELETE FROM Device WHERE Id = @Id";
        var deviceCommand = new SqlCommand(deviceSql, connection);
        deviceCommand.Parameters.AddWithValue("@Id", deviceId);
        if (deviceCommand.ExecuteNonQuery() == 0)
        {
            throw new KeyNotFoundException($"No device found with given Id");
        }

        ReadDevices();
    }

    /// <summary>
    ///  This method returns a single device with given id.
    /// </summary>

    public Device GetDeviceById(string deviceId)
    {
        using var connection = new SqlConnection(_connectionString);
        connection.Open();

        const string sql = @"
            SELECT 
                d.Id, d.Name, d.IsEnabled,
                CASE 
                    WHEN s.DeviceId IS NOT NULL THEN 'SW'
                    WHEN pc.DeviceId IS NOT NULL THEN 'P'
                    WHEN e.DeviceId IS NOT NULL THEN 'ED'
                END AS DeviceType,
                s.BatteryPercentage,
                e.IpAddress,
                e.NetworkName,
                pc.OperationSystem
            FROM Device d
            LEFT JOIN Smartwatch s ON d.Id = s.DeviceId
            LEFT JOIN PersonalComputer pc ON d.Id = pc.DeviceId
            LEFT JOIN Embedded e ON d.Id = e.DeviceId
            WHERE d.Id = @Id";

        var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@Id", deviceId);
        using var reader = command.ExecuteReader();
        if (reader.Read())
        {
            string type = reader.GetString(3);
            Device device = type switch
            {
                "SW" => new Smartwatch
                (
                    id: reader.GetString(0),
                    name: reader.GetString(1),
                    turnedOn: reader.GetBoolean(2),
                    battery: reader.GetInt32(4)
                ),
                "P" => new PersonalComputer
                (
                    id: reader.GetString(0),
                    name: reader.GetString(1),
                    turnedOn: reader.GetBoolean(2),
                    operatingSystem: reader.IsDBNull(7) ? null : reader.GetString(7)
                ),
                "ED" => new EmbeddedDevice
                (
                    id: reader.GetString(0),
                    name: reader.GetString(1),
                    ipAddress: reader.GetString(5),
                    networkName: reader.GetString(6)
                ),
                _ => throw new InvalidOperationException("Unknown device type")
            };
            return device;
        }
        else
        {
            throw new KeyNotFoundException("Device not found");
        }
    }
}
